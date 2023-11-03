using Accounting.Caching;
using Accounting.Categories.CostProductions;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.CostReports;
using Accounting.Reports.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Reports.Costs
{
    public class WorkPriceCardReportAppService : AccountingAppService
    {
        #region Fields
        private readonly SoTHZService _soTHZService;
        private readonly InfoZService _infoZService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;        
        private readonly CircularsService _circularsService;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public WorkPriceCardReportAppService(SoTHZService soTHZService,
                    WebHelper webHelper,
                    YearCategoryService yearCategoryService,
                    InfoZService infoZService,                    
                    CircularsService circularsService,
                    OrgUnitService orgUnitService,
                    TenantSettingService tenantSettingService,
                    IWebHostEnvironment hostingEnvironment,
                    ReportTemplateService reportTemplateService,
                    AccountingCacheManager accountingCacheManager
                )
        {
            _soTHZService = soTHZService;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _infoZService = infoZService;
            _circularsService = circularsService;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.WorkPriceBookReportView)]
        public async Task<ReportResponseDto<WorkPriceBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var result = await this.GetInfoWorkPrice(dto.Parameters);
            var reportResponse = new ReportResponseDto<WorkPriceBookDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.WorkPriceBookReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var dataSource = await CreateDataAsync(dto);
            var currencyFormats = await _accountingCacheManager.GetCurrencyFormats(_webHelper.GetCurrentOrgUnit());
            var reportTemplate = await _reportTemplateService.GetByCodeAsync(dto.ReportTemplateCode);
            string fileTemplate = reportTemplate.FileTemplate.Replace(".xml", "");
            fileTemplate = fileTemplate + "_" + dto.VndNt;
            if (!File.Exists(GetFileTemplatePath(fileTemplate + ".xml")))
            {
                throw new Exception("Không tìm thấy mẫu in!");
            }
            var renderOption = new RenderOption()
            {
                DataSource = dataSource,
                TypePrint = dto.Type,
                TemplateFile = GetFileTemplatePath(fileTemplate + ".xml"),
                CurrencyFormats = currencyFormats
            };
            var render = new ReportRender(renderOption);
            var result = render.Execute();

            return new FileContentResult(result, MIMETYPE.GetContentType(dto.Type.ToLower()))
            {
                FileDownloadName = $"{fileTemplate}.{dto.Type}"
            };
        }
        #endregion
        #region Privates
        private async Task<List<WorkPriceBookDto>> GetInfoWorkPrice(ReportBaseParameterDto dto)
        {
            var soTHZs = await this.GetSoTHZsAsync(dto);
            var infoZs = await this.GetInfoZsAsync(dto);
            foreach(var item in infoZs)
            {
                item.BeginQuantity = item.BeginM == dto.FromDate ? item.BeginQuantity : 0;
                item.BeginAmount = item.BeginM == dto.FromDate ? item.BeginAmount : 0;
                item.EndQuantity = item.EndM == dto.ToDate ? item.EndQuantity : 0;
                item.EndAmount = item.EndM == dto.ToDate ? item.EndAmount : 0;
            }
            var filterSoTHZ = soTHZs.Where(p => p.FieldName.StartsWith("PS_621") || p.FieldName.StartsWith("PS_622")
                                            || p.FieldName.StartsWith("PS_623") || p.FieldName.StartsWith("PS_627")
                                            || p.FieldName.StartsWith("DK_154") || p.FieldName.StartsWith("CK_154"))
                                .OrderBy(p => p.Ord).ToList();

            var queryableSoTHZ = filterSoTHZ.AsQueryable<SoTHZ>();
            var openAmount = this.GetCalculatingAmount(1,filterSoTHZ.Where(p => p.FieldName == "DK_154"), infoZs);
            var endAmount = this.GetCalculatingAmount(4, filterSoTHZ.Where(p => p.FieldName == "CK_154"), infoZs);
            var incurredAmount = this.GetCalculatingAmount(2, filterSoTHZ.Where(p => p.FieldName != "DK_154"
                                                && p.FieldName != "CK_154"), infoZs);

            var result = new List<WorkPriceBookDto>()
            {
                openAmount,
                incurredAmount,
                new WorkPriceBookDto()
                {
                    Note = "Giá thành sản phẩm, dịch vụ trong kỳ",
                    Ord = 3,
                    TotalZ = this.IsNull(openAmount.Dk154) + this.IsNull(incurredAmount.Amount) - this.IsNull(endAmount.Ck154)
                },
                endAmount
            };

            return result;
        }
        private WorkPriceBookDto GetCalculatingAmount(int ord,IEnumerable<SoTHZ> filterSoTHZ,List<InfoZ> infoZs)
        {
            var dto = new WorkPriceBookDto();
            dto.Ord = ord;
            dto.Note = ord switch
            {
                1 => "Chi phí SXKD dở dang đầu kỳ",
                2 => "Chi phí SXKD phát sinh trong kỳ",
                4 => "Chi phí SXKD dở dang cuối kỳ",
                _ => null
            };            

            var queryableInfoz = infoZs.AsQueryable();
            var sumSoThz = new List<SumSoThz>();

            foreach(var item in filterSoTHZ)
            {
                if (string.IsNullOrEmpty(item.TSum))
                {
                    var amount = this.GetSumAmount(item, queryableInfoz,ord.ToString());
                    sumSoThz.Add(new SumSoThz()
                    {
                        FieldName = item.FieldName,
                        Amount = amount
                    });
                }
                else
                {
                    string tSum = item.TSum.Replace("+", ",+").Replace("-", ",-");
                    string[] parts = tSum.Split(',');
                    decimal? totalAmount = 0;

                    foreach(string part in parts)
                    {
                        decimal sign = 1;
                        if (part.StartsWith("-")) sign = -1;
                        var fieldName = part.Substring(1);
                        var amount = sumSoThz.Where(p => p.FieldName == fieldName)
                                                .Select(p => p.Amount).FirstOrDefault();
                        if (amount != null)
                        {
                            amount = amount * sign;
                            totalAmount = totalAmount + amount;
                        }
                    }
                    sumSoThz.Add(new SumSoThz()
                    {
                        FieldName = item.FieldName,
                        Amount = totalAmount
                    });

                }
            }            

            foreach(var item in sumSoThz)
            {
                if (item.FieldName.Equals("PS_621"))
                {
                    dto.Ps621 = item.Amount;
                    continue;
                }
                else if (item.FieldName.Equals("PS_622"))
                {
                    dto.Ps622 = item.Amount;
                    continue;
                }
                else if (item.FieldName.Equals("PS_623"))
                {
                    dto.Ps623 = item.Amount;
                    continue;
                }
                else if (item.FieldName.Equals("PS_627"))
                {
                    dto.Ps627 = item.Amount;
                    continue;
                }
                else if (item.FieldName.Equals("DK_154"))
                {
                    dto.Dk154 = item.Amount;
                    continue;
                }
                else if (item.FieldName.Equals("CK_154"))
                {
                    dto.Ck154 = item.Amount;
                    continue;
                }
            }
            
            return dto;
        }        
        private decimal? GetSumAmount(SoTHZ soTHZ, IQueryable<InfoZ> queryableInfoz,string type)
        {
            var queryable = this.GetQueryableSum(soTHZ, queryableInfoz);
            var amount = type switch
            {
                "1" => queryable.Sum(p => p.BeginAmount),
                "2" => queryable.Sum(p => p.Amount),
                "4" => queryable.Sum(p => p.EndAmount),
                _ => 0
            };
            return amount;
        }
        private IQueryable<InfoZ> GetQueryableSum(SoTHZ soTHZ,IQueryable<InfoZ> queryableInfoz)
        {
            if (!string.IsNullOrEmpty(soTHZ.DebitAcc))
            {
                queryableInfoz = queryableInfoz.Where(p => p.DebitAcc.StartsWith(soTHZ.DebitAcc));
            }
            if (!string.IsNullOrEmpty(soTHZ.DebitSection))
            {
                queryableInfoz = queryableInfoz.Where(p => p.DebitSectionCode == soTHZ.DebitSection);
            }
            if (!string.IsNullOrEmpty(soTHZ.CreditAcc))
            {
                queryableInfoz = queryableInfoz.Where(p => p.CreditAcc.StartsWith(soTHZ.CreditAcc));
            }
            if (!string.IsNullOrEmpty(soTHZ.CreditSection))
            {
                queryableInfoz = queryableInfoz.Where(p => p.CreditSectionCode == soTHZ.CreditSection);
            }
            return queryableInfoz;
        }
        private async Task<List<SoTHZ>> GetSoTHZsAsync(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;
            var queryable = await _soTHZService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Year == year
                                && p.FProductOrWork.Equals(dto.FProductOrWork) 
                                && p.UsingDecision.Equals(yearCategory.UsingDecision)
                                && string.IsNullOrEmpty(p.TSum))
                .OrderBy(p => p.Ord);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<List<InfoZ>> GetInfoZsAsync(ReportBaseParameterDto dto)
        {
            var queryable = await this.GetQueryableInfoZ(dto);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<IQueryable<InfoZ>> GetQueryableInfoZ(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var infoZ = await _infoZService.GetQueryableAsync();
            infoZ = infoZ.Where(p => p.OrgCode == orgCode && p.Year == year
                            && p.BeginM >= dto.FromDate && p.EndM <= dto.ToDate
                            && p.DebitAcc.StartsWith("154")
                            && p.FProductWork.Equals(dto.FProductOrWork));
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                infoZ = infoZ.Where(p => p.FProductWorkCode.Equals(dto.FProductWorkCode));
            }
            return infoZ;
        }
        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
        }
        private async Task<dynamic> GetTenantSetting(string orgCode)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            var tenantSettings = await _tenantSettingService.GetBySettingTypeAsync(orgCode, TenantSettingType.Report);
            foreach (var setting in tenantSettings)
            {
                ((IDictionary<String, Object>)exo).Add(setting.Key, setting.Value);
            }
            return exo;
        }
        private async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private decimal? IsNull(decimal? number)
        {
            if (number == null) return 0;
            return number;
        }
        #endregion
    }
}
