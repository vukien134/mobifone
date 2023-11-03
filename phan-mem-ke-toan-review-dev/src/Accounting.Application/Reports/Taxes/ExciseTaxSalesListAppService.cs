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
using Accounting.DomainServices.Reports;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.Taxes
{
    public class ExciseTaxSalesListAppService : AccountingAppService
    {
        private readonly ExciseTaxService _exciseTaxService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WebHelper _webHelper;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly CircularsService _circularsService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;
        public ExciseTaxSalesListAppService(ExciseTaxService exciseTaxService,
            VoucherExciseTaxService voucherExciseTaxService,
            AccPartnerService accPartnerService,
            WebHelper webHelper,
            OrgUnitService orgUnitService,
            YearCategoryService yearCategoryService,
            TenantSettingService tenantSettingService,
            CircularsService circularsService,
            IWebHostEnvironment webHostEnvironment,
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
            _tenantSettingService = tenantSettingService;
            _circularsService = circularsService;
            _hostingEnvironment = webHostEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.ExciseTaxSalesListReportView)]
        public async Task<ReportResponseDto<ExciseTaxSalesListDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var reportResponse = new ReportResponseDto<ExciseTaxSalesListDto>();
            //DMTTDB
            var exciseTax = await _exciseTaxService.GetQueryableAsync();
            var lstExciseTax = exciseTax.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            //PSTTDB
            var voucherExciseTax = await _voucherExciseTaxService.GetQueryableAsync();
            var lstVoucherExciseTax = voucherExciseTax.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var result = (from a in lstVoucherExciseTax
                          join b in lstExciseTax on a.ExciseTaxCode equals b.Code
                          join c in lstAccPartner on a.PartnerCode equals c.Code into d
                          from pn in d.DefaultIfEmpty()
                          where a.VoucherDate >= dto.Parameters.FromDate && a.VoucherDate <= dto.Parameters.ToDate
                          select new ExciseTaxSalesListDto
                          {
                              Sort = 1,
                              Bold = "K",
                              VoucherId = a.ProductVoucherId,
                              VoucherCode = a.VoucherCode,
                              VoucherDate = a.VoucherDate,
                              InvoiceNumber = a.InvoiceNumber,
                              InvoiceSerial = a.InvoiceSymbol,
                              InvoiceSymbol = a.InvoiceSymbol,
                              InvoiceDate = a.InvoiceDate,
                              PartnerCode = a.PartnerCode,
                              ProductCode0 = a.ProductCode0,
                              ProductName = a.ProductName,
                              ProductName0 = a.ProductName0,
                              ExciseTaxName = b.Name,
                              Price = a.Price,
                              PriceCur = a.PriceCur,
                              UnitCode = a.UnitCode0,
                              Quantity = a.Quantity,
                              TotalAmount = a.Amount + a.AmountWithoutTax,
                              TotalAmountCur = a.AmountCur + a.AmountWithoutTaxCur,
                              ExciseTaxAmount = a.Amount,
                              ExciseTaxAmountCur = a.AmountCur,
                              AmountWithoutTax = a.AmountWithoutTax,
                              AmountWithoutTaxCur = a.AmountWithoutTaxCur,
                              PartnerName = pn != null ? pn.Name : null
                          }).ToList();
            var totalAmount = result.Select(p => p.ExciseTaxAmount).Sum();
            var totalAmountCur = result.Select(p => p.ExciseTaxAmountCur).Sum();
            var totalAmountWithoutTax = result.Select(p => p.AmountWithoutTax).Sum();
            var totalAmountWithoutTaxCur = result.Select(p => p.AmountWithoutTaxCur).Sum();
            ExciseTaxSalesListDto exciseTaxSalesListDto = new ExciseTaxSalesListDto();
            exciseTaxSalesListDto.Sort = 2;
            exciseTaxSalesListDto.Bold = "C";
            exciseTaxSalesListDto.ProductName = "Tổng cộng";
            exciseTaxSalesListDto.ExciseTaxAmount = totalAmount;
            exciseTaxSalesListDto.ExciseTaxAmountCur = totalAmountCur;
            exciseTaxSalesListDto.AmountWithoutTax = totalAmountWithoutTax;
            exciseTaxSalesListDto.AmountWithoutTaxCur = totalAmountWithoutTaxCur;
            result.Add(exciseTaxSalesListDto);
            result = result.OrderBy(p => p.Sort)
                         //.OrderBy(p => p.VoucherDate)
                         .ToList();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.ExciseTaxSalesListReportPrint)]
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
    }
}

