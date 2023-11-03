using System;
using Accounting.Constants;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Mvc;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.DomainServices.Categories.Others;
using Microsoft.AspNetCore.Hosting;
using System.Linq;
using Accounting.Vouchers;
using NPOI.SS.Formula.Functions;
using Accounting.Reports.DebitBooks;
using System.IO;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.GeneralDiaries
{
    public class ListOfDebtsAccordingToInvoicesAppService : AccountingAppService
    {
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly WebHelper _webHelper;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly PaymentTermService _paymentTermService;
        private readonly AccountingCacheManager _accountingCacheManager;
        public ListOfDebtsAccordingToInvoicesAppService(
                            VoucherPaymentBookService voucherPaymentBookService,
                            WebHelper webHelper,
                            ReportTemplateService reportTemplateService,
                            IWebHostEnvironment hostingEnvironment,
                            TenantSettingService tenantSettingService,
                            OrgUnitService orgUnitService,
                            CircularsService circularsService,
                            YearCategoryService yearCategoryService,
                            AccountSystemService accountSystemService,
                            AccPartnerService accPartnerService,
                            ProductVoucherService productVoucherService,
                            PaymentTermService paymentTermService,
                            AccountingCacheManager accountingCacheManager)
        {
            _voucherPaymentBookService = voucherPaymentBookService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accPartnerService = accPartnerService;
            _productVoucherService = productVoucherService;
            _paymentTermService = paymentTermService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.DebtAccordingInvoiceReportView)]
        public async Task<ReportResponseDto<ListOfDebtsAccordingToInvoicesDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var reportResponse = new ReportResponseDto<ListOfDebtsAccordingToInvoicesDto>();

            var voucherPaymentBook = await _voucherPaymentBookService.GetQueryableAsync();
            var lstvoucherPaymentBook = voucherPaymentBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.VoucherDate >= dto.Parameters.FromDate && p.VoucherDate <= dto.Parameters.ToDate).ToList();
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstproductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var paymentTerm = await _paymentTermService.GetQueryableAsync();
            var lstPaymentTerm = paymentTerm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var result = (from a in lstvoucherPaymentBook
                          join b in lstproductVoucher on a.DocumentId equals b.Id
                          join c in lstPartner on b.PartnerCode0 equals c.Code
                          join d in lstPaymentTerm on b.PaymentTermsCode equals d.Code
                          //where dto.Parameters.FromDate >= a.VoucherDate && a.VoucherDate <= dto.Parameters.ToDate
                          group new
                          {
                              a.DocumentId,
                              a.VoucherDate,
                              a.VoucherNumber,
                              b.PartnerCode0,
                              c.Name,
                              d.Code,
                              a.AmountReceivable,
                              a.AmountReceived,
                              b.Description,
                              a.TotalAmount
                          } by new
                          {
                              a.DocumentId,
                              a.VoucherDate,
                              a.VoucherNumber,
                              b.PartnerCode0,
                              c.Name,
                              d.Code,
                              b.Description,

                          } into gr
                          select new ListOfDebtsAccordingToInvoicesDto
                          {
                              VoucherId = gr.Key.DocumentId,
                              VoucherDate = gr.Key.VoucherDate,
                              VoucherNumber = gr.Key.VoucherNumber,
                              PartnerCode0 = gr.Key.PartnerCode0,
                              PartnerName = gr.Key.Name,
                              PaymentTermCode = gr.Key.Code,
                              Description = gr.Key.Description,
                              AmountReceivable = gr.Max(p => p.TotalAmount),
                              AmountReceived = gr.Sum(p => p.AmountReceived),
                              AmountRemaining = gr.Max(p => p.TotalAmount) - gr.Sum(p => p.AmountReceived)
                          }).ToList();


            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.DebtAccordingInvoiceReportPrint)]
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
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, FolderConst.Report,
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

