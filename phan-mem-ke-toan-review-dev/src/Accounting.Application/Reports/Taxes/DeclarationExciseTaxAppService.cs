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
    public class DeclarationExciseTaxAppService : AccountingAppService
    {
        private readonly ExciseTaxService _exciseTaxService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WebHelper _webHelper;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly ProductService _productService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly CircularsService _circularsService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;
        public DeclarationExciseTaxAppService(ExciseTaxService exciseTaxService,
            VoucherExciseTaxService voucherExciseTaxService,
            AccPartnerService accPartnerService,
            WebHelper webHelper,
            OrgUnitService orgUnitService,
            YearCategoryService yearCategoryService,
            AccTaxDetailService accTaxDetailService,
            TenantSettingService tenantSettingService,
            TaxCategoryService taxCategoryService,
            ProductService productService,
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
            _productService = productService;
            _circularsService = circularsService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.DeclarationExciseTaxReportView)]
        public async Task<ReportResponseDto<DeclarationExciseTaxDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var reportResponse = new ReportResponseDto<DeclarationExciseTaxDto>();
            var voucherExciseTax = await _voucherExciseTaxService.GetQueryableAsync();
            var lstVoucherExciseTax = voucherExciseTax.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var exciseTax = await _exciseTaxService.GetQueryableAsync();
            var lstExciseTax = exciseTax.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var data = (from a in lstVoucherExciseTax
                        join b in lstExciseTax on a.ExciseTaxCode equals b.Code
                        join c in lstProduct on a.ProductCode0 equals c.Code
                        where a.VoucherDate >= dto.Parameters.FromDate
                           && a.VoucherDate <= dto.Parameters.ToDate
                        group new { a, b, c } by new
                        {
                            c.ProductType,
                            b.Code,
                            b.Name,
                            b.HtkkName,
                            b.Htkk,
                            b.Htkk0,
                            a.ExciseTaxPercentage
                        } into gr
                        select new DeclarationExciseTaxDto
                        {
                            Bold = "K",
                            ExciseTaxCode = gr.Key.Code,
                            ProductName = gr.Key.Name,
                            UnitCode = gr.Max(p => p.a.UnitCode0),
                            QuantityConsume = gr.Sum(p => p.a.Quantity ?? 0),
                            TurnoverWithoutVat = gr.Sum(p => (p.a.AmountWithoutTax ?? 0) + (p.a.Amount ?? 0)),
                            TurnoverExcise = gr.Sum(p => p.a.AmountWithoutTax ?? 0),
                            ExciseTaxPercentage = gr.Key.ExciseTaxPercentage,
                            ExciseTaxDeduct = 0,
                            AmountExciseTax = gr.Sum(p => p.a.Amount ?? 0),
                            Htkk = gr.Key.Htkk,
                            Htkk0 = gr.Key.Htkk0,
                            HtkkName = gr.Key.HtkkName,
                            Type = gr.Key.ProductType == "D" ? "B" : "A"
                        }).ToList();
            var totalTurnoverWithoutVat = data.Sum(p => p.TurnoverWithoutVat ?? 0);
            var totalTurnoverExcise = data.Sum(p => p.TurnoverExcise ?? 0);
            var totalAmountExciseTax = data.Sum(p => p.AmountExciseTax ?? 0);

            var lst = new List<DeclarationExciseTaxDto>();
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "A", Ord = "0", Ord0 = "", Bold = "C", ProductName = "Không phát sinh giá trị tính thuế TTĐB trong kỳ", Type = ""
            });
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "B", Ord = "0", Ord0 = "I", Bold = "C", ProductName = "Hàng hóa chịu thuế TTĐB", Type = ""
            });
            int i = 1;
            lst.AddRange((from a in data
                          where a.Type == "A"
                          orderby a.ExciseTaxCode
                        select new DeclarationExciseTaxDto
                        {
                            Sort = "B",
                            Ord = "1",
                            Ord0 = i++.ToString(),
                            Bold = "K",
                            ExciseTaxCode = a.ExciseTaxCode,
                            ProductName = a.ProductName,
                            UnitCode = a.UnitCode,
                            QuantityConsume = a.QuantityConsume,
                            TurnoverWithoutVat = a.TurnoverWithoutVat,
                            TurnoverExcise = a.TurnoverExcise,
                            ExciseTaxPercentage = a.ExciseTaxPercentage,
                            ExciseTaxDeduct = a.ExciseTaxDeduct,
                            AmountExciseTax = a.AmountExciseTax,
                            Htkk = a.Htkk,
                            Htkk0 = a.Htkk0,
                            HtkkName = a.HtkkName,
                            Type = a.Type,
                        }).ToList());
            // -------------------------------
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "C", Ord = "0", Ord0 = "II", Bold = "C", ProductName = "Dịch vụ chịu thuế TTĐB", Type = ""
            });
            lst.AddRange((from a in data
                          where a.Type == "B"
                          orderby a.ExciseTaxCode
                          select new DeclarationExciseTaxDto
                          {
                              Sort = "C",
                              Ord = "1",
                              Ord0 = i++.ToString(),
                              Bold = "K",
                              ExciseTaxCode = a.ExciseTaxCode,
                              ProductName = a.ProductName,
                              UnitCode = a.UnitCode,
                              QuantityConsume = a.QuantityConsume,
                              TurnoverWithoutVat = a.TurnoverWithoutVat,
                              TurnoverExcise = a.TurnoverExcise,
                              ExciseTaxPercentage = a.ExciseTaxPercentage,
                              ExciseTaxDeduct = a.ExciseTaxDeduct,
                              AmountExciseTax = a.AmountExciseTax,
                              Htkk = a.Htkk,
                              Htkk0 = a.Htkk0,
                              HtkkName = a.HtkkName,
                              Type = a.Type,
                          }).ToList());
            // ---------------
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D", Ord = "0", Ord0 = "III", Bold = "C", ProductName = "Hàng hóa thuộc trường hợp không phải chịu thuế TTĐB", Type = ""
            });
            // ---------------
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D1", Ord = "0", Ord0 = "a", Bold = "K", ProductName = "Hàng hóa xuất khẩu", Type = ""
            });
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D1", Ord = "0", Bold = "K", Type = ""
            });
            // ---------------
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D2", Ord = "0", Ord0 = "b", Bold = "K", ProductName = "Hàng hóa bán để xuất khẩu", Type = ""
            });
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D2", Ord = "0", Bold = "K", Type = ""
            });
            // ---------------
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D3", Ord = "0", Ord0 = "c", Bold = "K", ProductName = "Hàng hóa gia công để xuất khẩu", Type = ""
            });
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "D3", Ord = "0", Bold = "K", Type = ""
            });
            // ---------------
            lst.Add(new DeclarationExciseTaxDto
            {
                Sort = "E",
                Ord = "0",
                Ord0 = "",
                Bold = "C",
                ProductName = "TỔNG CỘNG",
                Type = "",
                TurnoverWithoutVat = totalTurnoverWithoutVat,
                TurnoverExcise = totalTurnoverExcise,
                AmountExciseTax = totalAmountExciseTax,
            });
            reportResponse.Data = lst.OrderBy(p => p.Sort).ThenBy(p => p.Ord).ThenBy(p => p.ExciseTaxCode).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.DeclarationExciseTaxReportPrint)]
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

