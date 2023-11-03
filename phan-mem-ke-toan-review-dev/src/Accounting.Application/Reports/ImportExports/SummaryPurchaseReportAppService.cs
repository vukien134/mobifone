using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class SummaryPurchaseReportAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccSectionService _accSectionService;
        private readonly DepartmentService _departmentService;
        private readonly AccCaseService _accCaseService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductGroupService _productGroupService;
        private readonly ProductService _productService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductAppService _productAppService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly SaleChannelService _saleChannelService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public SummaryPurchaseReportAppService(ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccSectionService accSectionService,
                        DepartmentService departmentService,
                        AccCaseService accCaseService,
                        FProductWorkService fProductWorkService,
                        AccPartnerService accPartnerService,
                        PartnerGroupService partnerGroupService,
                        AccPartnerAppService accPartnerAppService,
                        ProductGroupService productGroupService,
                        ProductService productService,
                        WarehouseService warehouseService,
                        ProductAppService productAppService,
                        VoucherCategoryService voucherCategoryService,
                        VoucherTypeService voucherTypeService,
                        SaleChannelService saleChannelService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accSectionService = accSectionService;
            _departmentService = departmentService;
            _accCaseService = accCaseService;
            _fProductWorkService = fProductWorkService;
            _accPartnerService = accPartnerService;
            _partnerGroupService = partnerGroupService;
            _accPartnerAppService = accPartnerAppService;
            _productGroupService = productGroupService;
            _productService = productService;
            _warehouseService = warehouseService;
            _productAppService = productAppService;
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _saleChannelService = saleChannelService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.SummaryPurchaseReportView)]
        public async Task<ReportResponseDto<SummaryPurchaseReportDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var voucherTypes = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherType = voucherTypes.Where(p => p.Code == "PNH").FirstOrDefault();
            dto.Parameters.LstVoucherCode = lstVoucherType.ListVoucher;

            var dic = GetWarehouseBookParameter(dto.Parameters);
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var productGroup = await _productGroupService.GetQueryableAsync();
            var lstProductGroup = productGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


            var incurredData = await GetIncurredData(dic);
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                incurredData = incurredData.Where(p => p.PartnerCode == dto.Parameters.PartnerCode).ToList();
            }

            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var lstpartnerGroup = await _partnerGroupService.GetQueryableAsync();
                var partnerGroup = lstpartnerGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Id == dto.Parameters.PartnerGroup).FirstOrDefault().Code;
                var partners = await _accPartnerAppService.GetListByPartnerGroupCode(partnerGroup);
                incurredData = incurredData.Where(p => partners.Select(p => p.Code).Contains(p.PartnerCode)).ToList();
            }
            incurredData = (from a in incurredData
                            join b in lstProduct on a.ProductCode equals b.Code into c
                            from pr in c.DefaultIfEmpty()
                            join d in lstProductGroup on pr.ProductGroupId equals d.Id into e
                            from g in e.DefaultIfEmpty()
                            select new SummaryPurchaseReportDto
                            {
                                Tag = 0,
                                Bold = "K",
                                Sort = 3,
                                OrgCode = a.OrgCode,
                                ProductCode = a.ProductCode,
                                ProductName = pr != null ? pr.Name : null,
                                ProductGroupCode = g != null ? g.Code : null,
                                ProductGroupName = g != null ? g.Name : null,
                                UnitCode = a.UnitCode,
                                Quantity = a.ImportQuantity,
                                Amount = a.ImportAmount,
                                AmountCur = a.ImportAmountCur,
                                Price = a.ExportQuantity != 0 ? a.ExportAmount / a.ExportQuantity : 0
                            }).ToList();
            var reusulGroup = from a in incurredData

                              group new
                              {
                                  a.OrgCode,
                                  a.ProductGroupCode,
                                  a.ProductGroupName,
                                  a.Quantity,
                                  a.Amount,
                                  a.AmountCur,
                              } by new
                              {
                                  a.OrgCode,
                                  a.ProductGroupCode
                              } into gr
                              select new SummaryPurchaseReportDto
                              {

                                  Tag = 0,
                                  Bold = "C",
                                  Sort = 2,
                                  OrgCode = gr.Key.OrgCode,
                                  ProductCode = null,
                                  ProductName = gr.Max(p => p.ProductGroupName),
                                  ProductGroupCode = gr.Key.ProductGroupCode,
                                  ProductGroupName = null,
                                  UnitCode = null,
                                  Quantity = gr.Sum(p => p.Quantity),
                                  Amount = gr.Sum(p => p.Amount),
                                  AmountCur = gr.Sum(p => p.AmountCur),
                                  Price = 0
                              };
            var reusulGroups = from a in incurredData
                               where a.Bold == "K"
                               group new
                               {
                                   a.OrgCode,
                                   a.Quantity,
                                   a.Amount,
                                   a.AmountCur,

                               } by new
                               {
                                   a.OrgCode
                               } into gr
                               select new SummaryPurchaseReportDto
                               {

                                   Tag = 0,
                                   Bold = "C",
                                   Sort = 1,
                                   OrgCode = gr.Max(p => p.OrgCode),
                                   ProductCode = null,
                                   ProductName = "Tổng cộng ",
                                   ProductGroupCode = null,
                                   ProductGroupName = null,
                                   UnitCode = null,
                                   Quantity = gr.Sum(p => p.Quantity),
                                   Amount = gr.Sum(p => p.Amount),
                                   AmountCur = gr.Sum(p => p.AmountCur),
                                   Price = 0
                               };

            incurredData.AddRange(reusulGroup);
            incurredData.AddRange(reusulGroups);
            var resul = incurredData.OrderBy(p => p.ProductGroupCode)
                                    .ThenBy(p => p.Sort)
                                    .ToList();
            var reportResponse = new ReportResponseDto<SummaryPurchaseReportDto>();
            reportResponse.Data = resul;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        [Authorize(ReportPermissions.SummaryPurchaseReportPrint)]
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
        private List<FormularDto> GetFormular(string formular)
        {
            var lst = new List<FormularDto>();
            formular = formular.Replace(" ", "");
            formular = formular.Replace("+", ",+,");
            formular = formular.Replace("-", ",-,");
            formular = "+," + formular;
            var lstData = formular.Split(',').ToList();
            for (var i = 0; i < lstData.Count; i += 2)
            {
                lst.Add(new FormularDto
                {
                    Code = lstData[i + 1],
                    AccCode = lstData[i + 1],
                    Math = lstData[i],
                });
            }
            return lst;
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        private decimal GetVoucherNumber(string VoucherNumber)
        {
            string[] numbers = Regex.Split(VoucherNumber, @"\D+");
            if (numbers.Length > 0)
            {
                return decimal.Parse(string.Join("", numbers));
            }
            else
            {
                return 0;
            }
        }
        private async Task<List<SummaryPurchaseReportDto>> GetIncurredData(Dictionary<string, object> dic)
        {

            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.GroupBy(p => new { p.OrgCode, p.ProductCode }).Select(p => new SummaryPurchaseReportDto()
            {
                Tag = 0,
                Bold = "K",
                Sort = 2,
                OrgCode = p.Max(p => p.OrgCode),
                ProductCode = p.Key.ProductCode,
                ProductName = null,
                ProductGroupCode = null,
                UnitCode = p.Max(p => p.UnitCode),
                ImportQuantity = p.Sum(p => p.ImportQuantity),
                ImportAmount = p.Sum(p => p.ImportAmount),
                ImportAmountCur = p.Sum(p => p.ImportAmountCur),
                ExportQuantity = p.Sum(p => p.ExportQuantity),
                ExportAmount = p.Sum(p => p.ExportAmount),
                ExportAmountCur = p.Sum(p => p.ExportAmountCur),
                Price = 0,
                VoucherCode = p.Max(p => p.VoucherCode),
                PartnerCode = p.Max(p => p.PartnerCode ?? p.PartnerCode0)
            }).ToList();


            return incurredData;
        }
        private async Task<List<WarehouseBookGeneralDto>> GetWarehouseBook(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetWarehouseBookData(dic);
            return data;
        }


        private Dictionary<string, object> GetWarehouseBookParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(WarehouseBookParameterConst.OrgCode, _webHelper.GetCurrentOrgUnit());
            //dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            }
            //if (!string.IsNullOrEmpty(dto.PartnerCode))
            //{
            //    dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
            //}
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            }


            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }


            if (!string.IsNullOrEmpty(dto.ProductGroupCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductGroupCode, dto.ProductGroupCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductCode, dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }

            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                dic.Add(WarehouseBookParameterConst.BusinessCode, dto.BusinessCode);
            }
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }
            if (!string.IsNullOrEmpty(dto.LstVoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.LstVoucherCode, dto.LstVoucherCode);
            }


            return dic;
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
        private async Task<string> GetPartnerName(string orgCode, string code)
        {
            if (string.IsNullOrEmpty(code)) return null;
            var partner = await _accPartnerService.GetAccPartnerByCodeAsync(code, orgCode);
            if (partner == null) return null;

            return partner.Name;
        }
        #endregion
    }
}

