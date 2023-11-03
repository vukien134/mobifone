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
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Reports.DebitBooks;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using Accounting.DomainServices.Vouchers;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.GeneralDiaries
{
    public class ListOfVoucherPaymentBooksAppService : AccountingAppService
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
        private readonly AccVoucherService _accVoucherService;
        private readonly AccountingCacheManager _accountingCacheManager;
        public ListOfVoucherPaymentBooksAppService(VoucherPaymentBookService voucherPaymentBookService,
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
                            AccVoucherService accVoucherService,
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
            _accVoucherService = accVoucherService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.VoucherPaymentBookReportView)]
        public async Task<ReportResponseDto<ListOfVoucherPaymentBooksDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {

            var reportResponse = new ReportResponseDto<ListOfVoucherPaymentBooksDto>();

            var voucherPaymentBook = await _voucherPaymentBookService.GetQueryableAsync();
            var lstvoucherPaymentBook = voucherPaymentBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.VoucherDate >= dto.Parameters.FromDate && p.VoucherDate <= dto.Parameters.ToDate).ToList();
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            var lstproductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var paymentTerm = await _paymentTermService.GetQueryableAsync();
            var lstPaymentTerm = paymentTerm.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accVoucher = await _accVoucherService.GetQueryableAsync();
            var lstAccVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var result01 = (from a in lstvoucherPaymentBook
                            join c in lstPartner on a.PartnerCode equals c.Code

                            group new
                            {
                                a.DocumentId,
                                a.VoucherDate,
                                a.VoucherNumber,
                                a.PartnerCode,
                                c.Name,
                                a.AmountReceivable,
                                a.AmountReceived,
                                a.Amount,
                                a.VatAmount,
                                a.DiscountAmount,
                                a.TotalAmount
                            } by new
                            {
                                a.DocumentId,
                                a.VoucherDate,
                                a.VoucherNumber,
                                a.PartnerCode,
                                c.Name,


                            } into gr
                            select new ListOfVoucherPaymentBooksDto
                            {
                                Sort1 = 1,
                                Sort2 = 1,
                                Bold = "C",
                                DocumentId = gr.Key.DocumentId,
                                VoucherDate = gr.Key.VoucherDate,
                                VoucherNumber = gr.Key.VoucherNumber,
                                VoucherNumber0 = gr.Key.VoucherNumber,
                                PartnerCode = gr.Key.PartnerCode,
                                PartnerName = gr.Key.Name,
                                Note = gr.Key.PartnerCode + " - " + gr.Key.Name,
                                //AmountReceivable = gr.Max(p => p.AmountReceivable),
                                //AmountReceived = gr.Max(p => p.AmountReceived),
                                Amount = gr.Max(p => p.Amount),
                                VatAmount = gr.Max(p => p.VatAmount),
                                DiscountAmount = gr.Max(p => p.DiscountAmount),
                                TotalAmount = gr.Max(p => p.TotalAmount)
                            }).ToList();
            var resul02 = (from a in lstvoucherPaymentBook
                           join b in lstAccVoucher on a.AccVoucherId equals b.Id into c
                           from d in c.DefaultIfEmpty()

                           group new
                           {
                               a.VoucherDate,
                               a.VoucherNumber,
                               a.Amount,
                               a.Times,
                               a.AmountReceivable,
                               a.AmountReceived,
                               a.VatAmount,
                               a.DiscountAmount,
                               a.PartnerCode

                           } by new
                           {
                               a.VoucherDate,
                               a.VoucherNumber,
                               a.DeadlinePayment,
                               a.Times,
                               a.PartnerCode
                           } into gr
                           select new ListOfVoucherPaymentBooksDto
                           {
                               Sort1 = 1,
                               Sort2 = 2,
                               Bold = "K",
                               DocumentId = null,
                               VoucherDate = gr.Key.VoucherDate,
                               VoucherNumber = gr.Key.VoucherNumber,
                               VoucherNumber0 = gr.Key.VoucherNumber,
                               PartnerCode = gr.Key.PartnerCode,
                               Note = "Lần thu " + gr.Key.Times,
                               Amount = gr.Max(p => p.Amount),
                               AmountReceivable = gr.Max(p => p.AmountReceivable),
                               AmountReceived = gr.Max(p => p.AmountReceived),
                               DiscountAmount = gr.Max(p => p.DiscountAmount),
                               AmountRemaining = gr.Max(p => p.AmountReceivable) - gr.Max(p => p.AmountReceived),
                               TotalAmount = 0,
                               VatAmount = 0
                           }).ToList();
            decimal? amount = 0;
            decimal? vatAmount = 0;
            decimal? discountAmount = 0;
            decimal? amountReceivable = 0;
            decimal? amountReceived = 0;
            decimal? totalAmount = 0;
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                result01 = (from a in lstvoucherPaymentBook
                            join c in lstPartner on a.PartnerCode equals c.Code
                            where a.PartnerCode == dto.Parameters.PartnerCode
                            group new
                            {
                                a.DocumentId,
                                a.VoucherDate,
                                a.VoucherNumber,
                                a.PartnerCode,
                                c.Name,
                                a.AmountReceivable,
                                a.AmountReceived,
                                a.Amount,
                                a.VatAmount,
                                a.DiscountAmount,
                                a.TotalAmount
                            } by new
                            {
                                a.DocumentId,
                                a.VoucherDate,
                                a.VoucherNumber,
                                a.PartnerCode,
                                c.Name,

                            } into gr
                            select new ListOfVoucherPaymentBooksDto
                            {
                                Sort1 = 1,
                                Sort2 = 1,
                                Bold = "C",
                                DocumentId = gr.Key.DocumentId,
                                VoucherDate = gr.Key.VoucherDate,
                                VoucherNumber = gr.Key.VoucherNumber,
                                VoucherNumber0 = gr.Key.VoucherNumber,
                                PartnerCode = gr.Key.PartnerCode,
                                PartnerName = gr.Key.Name,
                                Note = gr.Key.PartnerCode + " - " + gr.Key.Name,
                                AmountReceivable = 0,
                                AmountReceived = gr.Max(p => p.AmountReceived),
                                Amount = gr.Max(p => p.Amount),
                                VatAmount = gr.Max(p => p.VatAmount),
                                DiscountAmount = gr.Max(p => p.DiscountAmount),
                                TotalAmount = gr.Max(p => p.TotalAmount)
                            }).ToList();
                resul02 = (from a in lstvoucherPaymentBook
                           join b in lstAccVoucher on a.AccVoucherId equals b.Id into c
                           from d in c.DefaultIfEmpty()

                           where a.PartnerCode == dto.Parameters.PartnerCode
                           group new
                           {
                               a.VoucherDate,
                               a.VoucherNumber,
                               a.Amount,
                               a.Times,
                               a.AmountReceivable,
                               a.AmountReceived,
                               a.VatAmount,
                               a.DiscountAmount,
                               a.PartnerCode

                           } by new
                           {
                               a.VoucherDate,
                               a.VoucherNumber,
                               a.DeadlinePayment,
                               a.Times,
                               a.PartnerCode
                           } into gr
                           select new ListOfVoucherPaymentBooksDto
                           {
                               Sort1 = 1,
                               Sort2 = 2,
                               Bold = "K",
                               DocumentId = null,
                               VoucherDate = gr.Key.VoucherDate,
                               VoucherNumber = gr.Key.VoucherNumber,
                               VoucherNumber0 = gr.Key.VoucherNumber,
                               PartnerCode = gr.Key.PartnerCode,
                               Note = "Lần thu " + gr.Key.Times,
                               Amount = 0,
                               AmountReceivable = gr.Max(p => p.AmountReceivable),
                               AmountReceived = gr.Max(p => p.AmountReceived),
                               AmountRemaining = gr.Max(p => p.AmountReceivable) - gr.Max(p => p.AmountReceived),
                               DiscountAmount = 0,
                               TotalAmount = 0,
                               VatAmount = 0
                           }).ToList();
            }

            resul02.AddRange(result01);
            amount = resul02.Select(p => p.Amount).Sum();
            vatAmount = resul02.Select(p => p.VatAmount).Sum();
            discountAmount = resul02.Select(p => p.DiscountAmount).Sum();
            amountReceivable = resul02.Select(p => p.AmountReceivable).Sum();
            amountReceived = resul02.Select(p => p.AmountReceived).Sum();
            totalAmount = resul02.Select(p => p.TotalAmount).Sum();


            ListOfVoucherPaymentBooksDto listOfVoucherPaymentBooksDto = new ListOfVoucherPaymentBooksDto();
            listOfVoucherPaymentBooksDto.Sort1 = 3;
            listOfVoucherPaymentBooksDto.Sort2 = 3;
            listOfVoucherPaymentBooksDto.Bold = "C";
            listOfVoucherPaymentBooksDto.Note = "Tổng cộng ";
            listOfVoucherPaymentBooksDto.Amount = amount;
            listOfVoucherPaymentBooksDto.DiscountAmount = discountAmount;
            listOfVoucherPaymentBooksDto.VatAmount = vatAmount;
            listOfVoucherPaymentBooksDto.AmountReceivable = amountReceivable;
            listOfVoucherPaymentBooksDto.AmountReceived = amountReceived;
            listOfVoucherPaymentBooksDto.TotalAmount = totalAmount;
            listOfVoucherPaymentBooksDto.AmountRemaining = totalAmount - amountReceived;
            listOfVoucherPaymentBooksDto.VoucherNumber0 = "zzzzzzzz";
            resul02.Add(listOfVoucherPaymentBooksDto);
            resul02 = resul02.OrderBy(p => p.VoucherNumber0)
                             .ThenBy(p => p.Sort1)
                             .ThenBy(p => p.Sort2)
                             //.OrderBy(p => p.VoucherDate)
                             //.OrderBy(p => p.VoucherNumber)
                             .ToList();
            reportResponse.Data = resul02;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.VoucherPaymentBookReportPrint)]
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

