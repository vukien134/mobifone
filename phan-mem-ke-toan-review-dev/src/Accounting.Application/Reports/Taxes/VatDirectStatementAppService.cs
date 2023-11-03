using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.Helpers;
using Accounting.Reports.DebitBooks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Vouchers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.Taxes
{
    public class VatDirectStatementAppService : AccountingAppService
    {
        private readonly ExciseTaxService _exciseTaxService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WebHelper _webHelper;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly CircularsService _circularsService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;

        public VatDirectStatementAppService(ExciseTaxService exciseTaxService,
            VoucherExciseTaxService voucherExciseTaxService,
            AccPartnerService accPartnerService,
            WebHelper webHelper,
            OrgUnitService orgUnitService,
            YearCategoryService yearCategoryService,
            AccTaxDetailService accTaxDetailService,
            TenantSettingService tenantSettingService,
            TaxCategoryService taxCategoryService,
            CircularsService circularsService,
            IWebHostEnvironment hostingEnvironment,
            ReportTemplateService reportTemplateService,
            AccountingCacheManager accountingCacheManager
            )
        {
            _exciseTaxService = exciseTaxService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _accPartnerService = accPartnerService;
            _webHelper = webHelper;
            _orgUnitService = orgUnitService;
            _yearCategoryService = yearCategoryService;
            _accTaxDetailService = accTaxDetailService;
            _tenantSettingService = tenantSettingService;
            _taxCategoryService = taxCategoryService;
            _circularsService = circularsService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.VatDirectStatementReportView)]
        public async Task<ReportResponseDto<VatDirectStatementDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var reportResponse = new ReportResponseDto<VatDirectStatementDto>();
            var accTaxDetail = await _accTaxDetailService.GetQueryableAsync();
            var lstAccTaxDetail = accTaxDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var taxCategory = await _taxCategoryService.GetQueryableAsync();
            var lstTaxCategory = taxCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var dataTax = (from a in lstAccTaxDetail
                          join b in lstTaxCategory on a.TaxCategoryCode equals b.Code
                          where (a.CheckDuplicate == null || a.CheckDuplicate != "*")
                              && a.InvoiceDate >= dto.Parameters.FromDate
                              && a.InvoiceDate <= dto.Parameters.ToDate
                              && b.IsDirect == true && b.OutOrIn == "R"
                          group new { a, b } by new
                          {
                              a.VatPercentage
                          } into gr
                          select new AccTaxDetail
                          {
                              VatPercentage = gr.Key.VatPercentage,
                              AmountWithoutVat = gr.Sum(p => p.a.AmountWithoutVat),
                              Amount = gr.Sum(p => p.a.Amount),
                          }).ToList();
            var amountWithoutVat = dataTax.Sum(p => p.AmountWithoutVat ?? 0);
            var amount = dataTax.Sum(p => p.Amount ?? 0);
            var amountWithoutVat0 = dataTax.Sum(p => p.VatPercentage == 0 ? p.AmountWithoutVat ?? 0 : 0);
            var amount0 = dataTax.Sum(p => p.VatPercentage == 0 ? p.Amount ?? 0 : 0);
            var amountWithoutVat1 = dataTax.Sum(p => p.VatPercentage == 1 ? p.AmountWithoutVat ?? 0 : 0);
            var amount1 = dataTax.Sum(p => p.VatPercentage == 1 ? p.Amount ?? 0 : 0);
            var amountWithoutVat2 = dataTax.Sum(p => p.VatPercentage == 2 ? p.AmountWithoutVat ?? 0 : 0);
            var amount2 = dataTax.Sum(p => p.VatPercentage == 2 ? p.Amount ?? 0 : 0);
            var amountWithoutVat3 = dataTax.Sum(p => p.VatPercentage == 3 ? p.AmountWithoutVat ?? 0 : 0);
            var amount3 = dataTax.Sum(p => p.VatPercentage == 3 ? p.Amount ?? 0 : 0);
            var amountWithoutVat5 = dataTax.Sum(p => p.VatPercentage == 5 ? p.AmountWithoutVat ?? 0 : 0);
            var amount5 = dataTax.Sum(p => p.VatPercentage == 5 ? p.Amount ?? 0 : 0);
            
            var lst = new List<VatDirectStatementDto>();
            lst.Add(new VatDirectStatementDto
            {
                Sort = "A",
                Bold = "K",
                Ord = "1",
                CareerGroup = "Phân phối, cung cấp hàng hóa",
                NumberCode1 = "[21]",
                Turnover1 = amountWithoutVat0,
                NumberCode2 = "[23]",
                Turnover2 = amountWithoutVat1,
                TurnoverRatio = "1%",
                NumberCode3 = "[23]=[22]x1%",
                Turnover3 = amount1,
            });
            lst.Add(new VatDirectStatementDto
            {
                Sort = "A",
                Bold = "K",
                Ord = "2",
                CareerGroup = "Dịch vụ, xây dựng không bao thầu nguyên vật liệu",
                NumberCode1 = "",
                Turnover1 = 0,
                NumberCode2 = "[24]",
                Turnover2 = amountWithoutVat5,
                TurnoverRatio = "1%",
                NumberCode3 = "[25]=[24]x5%",
                Turnover3 = amount5,
            });
            lst.Add(new VatDirectStatementDto
            {
                Sort = "A",
                Bold = "K",
                Ord = "3",
                CareerGroup = "Sản xuất, vận tải, dịch vụ có gắn với hàng hóa, xây dựng có bao thầu nguyên vật liệu",
                NumberCode1 = "",
                Turnover1 = 0,
                NumberCode2 = "[26]",
                Turnover2 = amountWithoutVat3,
                TurnoverRatio = "1%",
                NumberCode3 = "[27]=[26]x3%",
                Turnover3 = amount3,
            });
            lst.Add(new VatDirectStatementDto
            {
                Sort = "A",
                Bold = "K",
                Ord = "4",
                CareerGroup = "Hoạt động kinh doanh khác",
                NumberCode1 = "",
                Turnover1 = 0,
                NumberCode2 = "[28]",
                Turnover2 = amountWithoutVat2,
                TurnoverRatio = "1%",
                NumberCode3 = "[29]=[28]x2%",
                Turnover3 = amount2,
            });
            lst.Add(new VatDirectStatementDto
            {
                Sort = "B",
                Bold = "C",
                Ord = "",
                CareerGroup = "TỔNG CỘNG",
                NumberCode1 = "",
                Turnover1 = 0,
                NumberCode2 = "[30]=[22]+[24]+[26]+[28]",
                Turnover2 = amountWithoutVat - amountWithoutVat0,
                TurnoverRatio = "1%",
                NumberCode3 = "[29]=[28]x2%",
                Turnover3 = amount,
            });
            reportResponse.Data = lst.OrderBy(p => p.Sort).ThenBy(p => p.Ord).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.VatDirectStatementReportPrint)]
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
    }
}

