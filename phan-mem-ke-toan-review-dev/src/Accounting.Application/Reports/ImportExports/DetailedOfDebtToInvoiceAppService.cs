using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectMapping;
using System.Linq;
using Accounting.DomainServices.Vouchers;
using StackExchange.Redis;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Vouchers;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class DetailedOfDebtToInvoiceAppService : AccountingAppService
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
        private readonly AccPartnerService _accPartnerService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly CircularsService _circularsService;
        private readonly LedgerService _ledgerService;
        private readonly VoucherPaymentBeginningService _voucherPaymentBeginningService;
        private readonly VoucherPaymentBeginningDetailService _voucherPaymentBeginningDetailService;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public DetailedOfDebtToInvoiceAppService(ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        AccPartnerService accPartnerService,
                        VoucherPaymentBookService voucherPaymentBookService,
                        CircularsService circularsService,
                        LedgerService ledgerService,
                        VoucherPaymentBeginningService voucherPaymentBeginningService,
                        VoucherPaymentBeginningDetailService voucherPaymentBeginningDetailService,
                        AccVoucherService accVoucherService,
                        ProductVoucherService productVoucherService,
                        YearCategoryService yearCategoryService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _accPartnerService = accPartnerService;
            _voucherPaymentBookService = voucherPaymentBookService;
            _circularsService = circularsService;
            _ledgerService = ledgerService;
            _voucherPaymentBeginningService = voucherPaymentBeginningService;
            _voucherPaymentBeginningDetailService = voucherPaymentBeginningDetailService;
            _accVoucherService = accVoucherService;
            _productVoucherService = productVoucherService;
            _yearCategoryService = yearCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.DetailDebtInvoiceReportView)]
        public async Task<ReportResponseDto<DetailedOfDebtToInvoiceDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();

            var voucherPayment = await _voucherPaymentBookService.GetQueryableAsync();
            var lstVoucherpayment = voucherPayment.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            lstVoucherpayment = (from a in lstVoucherpayment
                                 group new
                                 {
                                     a.PartnerCode,
                                     a.AccCode,
                                     a.VoucherDate,
                                     a.Amount,
                                     a.AmountReceivable,
                                     a.AmountReceived,
                                     a.DiscountAmount,
                                     a.TotalAmount,
                                     a.DeadlinePayment,
                                     a.Times,
                                     a.AccType,
                                     a.DocumentId,
                                     a.VoucherNumber,

                                 } by new
                                 {
                                     a.PartnerCode,
                                     a.AccCode,
                                     a.Times,
                                     a.DocumentId,
                                     a.VoucherDate,
                                     a.VoucherNumber,
                                     a.DeadlinePayment
                                 } into gr
                                 select new VoucherPaymentBook
                                 {
                                     AccCode = gr.Key.AccCode,
                                     PartnerCode = gr.Key.PartnerCode,
                                     Times = gr.Key.Times,
                                     Amount = gr.Max(p => p.Amount),
                                     AmountReceivable = gr.Max(p => p.AmountReceivable),
                                     AmountReceived = gr.Max(p => p.AmountReceived),
                                     DocumentId = gr.Key.DocumentId,
                                     VoucherDate = gr.Key.VoucherDate,
                                     VoucherNumber = gr.Key.VoucherNumber,
                                     DeadlinePayment = gr.Key.DeadlinePayment,
                                     TotalAmount = gr.Max(p => p.TotalAmount),

                                 }).ToList();
            var partners = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partners.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var ledgerService = await _ledgerService.GetQueryableAsync();
            var lstWareHouseBook = ledgerService.Where(p => p.Year == _webHelper.GetCurrentYear() && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();



            var resul = (from a in lstVoucherpayment
                         join b in lstPartner on a.PartnerCode equals b.Code
                         join c in lstWareHouseBook on a.DocumentId equals c.VoucherId
                         where //a.PartnerCode == dto.Parameters.PartnerCode || dto.Parameters.PartnerCode == ""
                            (dto.Parameters.AccCode.Contains(c.CreditAcc) == true || dto.Parameters.AccCode.Contains(c.DebitAcc) == true || dto.Parameters.AccCode.Contains(a.AccCode) == true)
                         && a.VoucherDate <= dto.Parameters.ToDate
                         group new
                         {

                             a.DocumentId,
                             a.VoucherDate,
                             a.VoucherNumber,
                             a.AccCode,
                             a.PartnerCode,
                             a.DeadlinePayment,
                             a.OrgCode,
                             a.Year,
                             a.AccType,
                             b.Name,
                             b.Address,
                             b.Representative,
                             a.Times,
                             c.Description,
                             a.VatAmount,
                             a.DiscountAmount,
                             a.TotalAmount,
                             a.AmountReceivable,
                             a.AmountReceived
                         } by new
                         {

                             a.DocumentId,
                             a.VoucherDate,
                             a.VoucherNumber,
                             a.AccCode,
                             a.PartnerCode,
                             a.DeadlinePayment,
                             a.OrgCode,
                             a.Year,
                             a.AccType,
                             b.Name,
                             b.Address,
                             b.Representative,
                             a.Times
                         } into gr
                         where dto.Parameters.DebtType == 1 ? gr.Max(p => p.AmountReceivable) - gr.Max(p => p.AmountReceived) > 0 : 1 == 1
                         select new DetailedOfDebtToInvoiceDto
                         {

                             DocumentId = gr.Key.DocumentId,
                             VoucherDate = gr.Key.VoucherDate,
                             VoucherNumber = gr.Key.VoucherNumber,
                             AccCode = gr.Key.AccCode,
                             PartnerCode = gr.Key.PartnerCode,
                             PartnerName = gr.Key.Name,
                             DeadlinePayment = gr.Key.DeadlinePayment,
                             OrgCode = gr.Key.OrgCode,
                             Year = gr.Key.Year,
                             AccType = gr.Key.AccType,
                             Address = gr.Key.Address,
                             Representative = gr.Key.Representative,
                             Times = gr.Key.Times,
                             Note = gr.Max(p => p.Description),
                             VatAmount = gr.Max(p => p.VatAmount),
                             DiscountAmount = gr.Max(p => p.DiscountAmount),
                             TotalAmount = gr.Max(p => p.TotalAmount),
                             AmountReceivable = gr.Max(p => p.AmountReceivable),
                             AmountReceived = gr.Max(p => p.AmountReceived),
                             PaymentType = "",
                             OrdRec = "",
                             Remain = gr.Max(p => p.AmountReceivable) - gr.Max(p => p.AmountReceived),
                             D0to15 = (decimal)0,
                             D16to30 = (decimal)0,
                             D31to60 = (decimal)0,
                             D61to90 = (decimal)0,
                             D91to120 = (decimal)0,
                             DOver120 = (decimal)0,
                             Sort = 1
                         }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                resul = (from a in resul
                         where a.PartnerCode == dto.Parameters.PartnerCode
                         select new DetailedOfDebtToInvoiceDto
                         {
                             Id = a.Id,
                             DocumentId = a.DocumentId,
                             VoucherDate = a.VoucherDate,
                             VoucherNumber = a.VoucherNumber,
                             AccCode = a.AccCode,
                             PartnerCode = a.PartnerCode,
                             PartnerName = a.PartnerName,
                             DeadlinePayment = a.DeadlinePayment,
                             OrgCode = a.OrgCode,
                             Year = a.Year,
                             AccType = a.AccType,
                             Address = a.Address,
                             Note = a.Note,
                             Times = a.Times,
                             Description = a.Description,
                             VatAmount = a.VatAmount,
                             DiscountAmount = a.DiscountAmount,
                             TotalAmount = a.TotalAmount,
                             AmountReceivable = a.AmountReceivable,
                             AmountReceived = a.AmountReceived,
                             PaymentType = "",
                             OrdRec = "",
                             Remain = a.Remain,
                             D0to15 = (decimal)0,
                             D16to30 = (decimal)0,
                             D31to60 = (decimal)0,
                             D61to90 = (decimal)0,
                             D91to120 = (decimal)0,
                             DOver120 = (decimal)0,
                             Sort = 1
                         }).ToList();
            }
            var voucherPaymentBeginning = await _voucherPaymentBeginningService.GetQueryableAsync();
            var lstVoucherPaymentBeginning = voucherPaymentBeginning.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


            var voucherPaymentBeginningdetail = await _voucherPaymentBeginningDetailService.GetQueryableAsync();
            var lstvoucherPaymentBeginningdetail = voucherPaymentBeginningdetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var resul0 = (from a in lstVoucherPaymentBeginning
                          join b in lstvoucherPaymentBeginningdetail on a.Id equals b.VoucherPaymentBeginningId
                          where a.VoucherDate <= dto.Parameters.ToDate
                          select new DetailedOfDebtToInvoiceDto
                          {
                              Id = a.Id,
                              DocumentId = a.Id,
                              VoucherDate = a.VoucherDate,
                              VoucherNumber = a.VoucherNumber,
                              AccCode = a.AccCode,
                              PartnerCode = a.PartnerCode,
                              PartnerName = null,
                              DeadlinePayment = b.DeadlinePayment,
                              OrgCode = a.OrgCode,
                              Year = a.Year,
                              AccType = "",
                              Address = "",
                              Note = "",
                              Times = b.Times,
                              Description = a.Description,
                              VatAmount = a.TotalAmountVat,
                              DiscountAmount = a.TotalAmountDiscount,
                              TotalAmount = a.TotalAmount,
                              AmountReceivable = b.Amount,
                              AmountReceived = (decimal)0,
                              PaymentType = a.PaymentType,
                              OrdRec = a.VoucherNumber + a.VoucherDate,
                              Remain = (decimal)0,
                              D0to15 = (decimal)0,
                              D16to30 = (decimal)0,
                              D31to60 = (decimal)0,
                              D61to90 = (decimal)0,
                              D91to120 = (decimal)0,
                              DOver120 = (decimal)0,
                              Sort = 1
                          }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerCode))
            {
                resul0 = (from a in resul0
                          where a.PartnerCode.Contains(dto.Parameters.PartnerCode) == true
                          select new DetailedOfDebtToInvoiceDto
                          {
                              Id = a.Id,
                              DocumentId = a.Id,
                              VoucherDate = a.VoucherDate,
                              VoucherNumber = a.VoucherNumber,
                              AccCode = a.AccCode,
                              PartnerCode = a.PartnerCode,
                              PartnerName = a.PartnerName,
                              DeadlinePayment = a.DeadlinePayment,
                              OrgCode = a.OrgCode,
                              Year = a.Year,
                              AccType = "",
                              Address = "",
                              //Note = "",
                              Times = a.Times,
                              Note = a.Description,
                              VatAmount = a.VatAmount,
                              DiscountAmount = a.DiscountAmount,
                              TotalAmount = a.TotalAmount,
                              AmountReceivable = a.AmountReceivable,
                              AmountReceived = (decimal)0,
                              PaymentType = a.PaymentType,
                              OrdRec = a.VoucherNumber + a.VoucherDate,
                              Remain = (decimal)0,
                              D0to15 = (decimal)0,
                              D16to30 = (decimal)0,
                              D31to60 = (decimal)0,
                              D61to90 = (decimal)0,
                              D91to120 = (decimal)0,
                              DOver120 = (decimal)0,
                              Sort = 1
                          }).ToList();

            }
            resul.AddRange(resul0);
            var sumAmountReceived = (from a in lstVoucherpayment
                                     group new
                                     {
                                         a.DocumentId,
                                         a.AccCode,
                                         a.VoucherDate,
                                         a.VoucherNumber,
                                         a.PartnerCode,
                                         a.OrgCode,
                                         a.Times,
                                         a.AmountReceived,

                                     } by new
                                     {
                                         a.DocumentId,
                                         a.AccCode,
                                         a.VoucherDate,
                                         a.VoucherNumber,
                                         a.PartnerCode,
                                         a.OrgCode,
                                         a.Times,

                                     } into gr
                                     select new
                                     {

                                         DocumentId = gr.Key.DocumentId,
                                         AccCode = gr.Key.AccCode,
                                         VoucherDate = gr.Key.VoucherDate,
                                         VoucherNumber = gr.Key.VoucherNumber,
                                         PartnerCode = gr.Key.PartnerCode,
                                         OrgCode = gr.Key.OrgCode,
                                         Times = gr.Key.Times,
                                         AmountReceived = gr.Sum(p => p.AmountReceived)

                                     }).ToList();
            var resuls = from a in resul
                         join b in sumAmountReceived on new { a.DocumentId, a.Times } equals new { b.DocumentId, b.Times } into d
                         from su in d.DefaultIfEmpty()
                         select new
                         {
                             a.DocumentId,
                             a.Times,
                             AmountReceived = su != null ? su.AmountReceived : 0,
                             a.Id


                         };
            resul = (from a in resul
                     join b in resuls on new
                     {
                         a.DocumentId,
                         a.Times

                     } equals new
                     {
                         b.DocumentId,
                         b.Times

                     } into c
                     from up in c.DefaultIfEmpty()
                     select new DetailedOfDebtToInvoiceDto
                     {
                         Id = this.GetNewObjectId(),
                         DocumentId = a.Id,
                         VoucherDate = a.VoucherDate,
                         VoucherNumber = a.VoucherNumber,
                         AccCode = a.AccCode,
                         PartnerCode = a.PartnerCode,
                         PartnerName = a.PartnerName,
                         DeadlinePayment = a.DeadlinePayment,
                         OrgCode = a.OrgCode,
                         Year = a.Year,
                         AccType = "",
                         Address = "",
                         Representative = "",
                         Times = a.Times,
                         Note = a.Note,
                         VatAmount = a.VatAmount,
                         DiscountAmount = a.DiscountAmount,
                         TotalAmount = a.TotalAmount,
                         AmountReceivable = (decimal)(a.AmountReceivable),
                         AmountReceived = up != null ? up.AmountReceived : a.AmountReceived,
                         PaymentType = a.PaymentType,
                         OrdRec = a.OrdRec,
                         Remain = (decimal)(a.AmountReceivable - (up != null ? up.AmountReceived : 0)),
                         D0to15 = (decimal)0,
                         D16to30 = (decimal)0,
                         D31to60 = (decimal)0,
                         D61to90 = (decimal)0,
                         D91to120 = (decimal)0,
                         DOver120 = (decimal)0,
                         Sort = 1
                     }).ToList();
            //where dto.Parameters.DebtType == 1 ? gr.Max(p => p.AmountReceivable) - gr.Max(p => p.AmountReceived) > 0 : 1 == 1
            if (dto.Parameters.DebtType == 1)
            {
                resul = resul.Where(p => p.AmountReceivable - p.AmountReceived > 0).ToList();

            }

            foreach (var item in resul.ToList())
            {
                DateTime todate = Convert.ToDateTime(dto.Parameters.ToDate);
                DateTime DeadlinePayment = Convert.ToDateTime(item.DeadlinePayment);
                TimeSpan time = todate - DeadlinePayment;
                int tongSoNgay = Math.Abs(time.Days);
                if (tongSoNgay >= 0 && tongSoNgay <= 15)
                {
                    var resulall = (from a in resul
                                    where a.Id == item.Id
                                    select new DetailedOfDebtToInvoiceDto
                                    {

                                        Id = a.Id,
                                        DocumentId = a.Id,
                                        VoucherDate = a.VoucherDate,
                                        VoucherNumber = a.VoucherNumber,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName = a.PartnerName,
                                        DeadlinePayment = a.DeadlinePayment,
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccType = "",
                                        Address = "",
                                        Note = a.Note,
                                        Times = a.Times,
                                        Description = a.Description,
                                        VatAmount = a.VatAmount,
                                        DiscountAmount = a.DiscountAmount,
                                        TotalAmount = a.TotalAmount,
                                        AmountReceivable = (decimal)(a.AmountReceivable),
                                        AmountReceived = (decimal)a.AmountReceived,
                                        PaymentType = a.PaymentType,
                                        OrdRec = a.OrdRec,
                                        Remain = a.Remain,
                                        D0to15 = (decimal)a.Remain,
                                        D16to30 = (decimal)0,
                                        D31to60 = (decimal)0,
                                        D61to90 = (decimal)0,
                                        D91to120 = (decimal)0,
                                        DOver120 = (decimal)0,
                                        Sort = 1
                                    }).ToList();
                    resul = (from a in resul
                             join b in resulall on a.Id equals b.Id into c
                             from d in c.DefaultIfEmpty()
                             select new DetailedOfDebtToInvoiceDto
                             {
                                 Id = a.Id,
                                 DocumentId = a.Id,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 PartnerName = a.PartnerName,
                                 DeadlinePayment = a.DeadlinePayment,
                                 OrgCode = a.OrgCode,
                                 Year = a.Year,
                                 AccType = "",
                                 Address = "",
                                 Note = a.Note,
                                 Times = a.Times,
                                 Description = a.Description,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = (decimal)(a.AmountReceivable),
                                 AmountReceived = (decimal)a.AmountReceived,
                                 PaymentType = a.PaymentType,
                                 OrdRec = a.OrdRec,
                                 Remain = a.Remain,
                                 //D0to15 = (decimal)a.Remain,
                                 D16to30 = a.D16to30,
                                 D31to60 = a.D31to60,
                                 D61to90 = a.D61to90,
                                 D91to120 = a.D91to120,
                                 DOver120 = a.DOver120,
                                 Sort = 1,
                                 D0to15 = d != null ? d.D0to15 : a.D0to15
                             }).ToList();
                }
                if (tongSoNgay > 15 && tongSoNgay <= 30)
                {
                    var resulall = (from a in resul
                                    where a.Id == item.Id
                                    select new DetailedOfDebtToInvoiceDto
                                    {

                                        Id = a.Id,
                                        DocumentId = a.Id,
                                        VoucherDate = a.VoucherDate,
                                        VoucherNumber = a.VoucherNumber,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName = a.PartnerName,
                                        DeadlinePayment = a.DeadlinePayment,
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccType = "",
                                        Address = "",
                                        Note = a.Note,
                                        Times = a.Times,
                                        Description = a.Description,
                                        VatAmount = a.VatAmount,
                                        DiscountAmount = a.DiscountAmount,
                                        TotalAmount = a.TotalAmount,
                                        AmountReceivable = (decimal)(a.AmountReceivable),
                                        AmountReceived = (decimal)a.AmountReceived,
                                        PaymentType = a.PaymentType,
                                        OrdRec = a.OrdRec,
                                        Remain = a.Remain,
                                        D0to15 = (decimal)a.D0to15,
                                        D16to30 = (decimal)a.Remain,
                                        D31to60 = (decimal)0,
                                        D61to90 = (decimal)0,
                                        D91to120 = (decimal)0,
                                        DOver120 = (decimal)0,
                                        Sort = 1
                                    }).ToList();
                    resul = (from a in resul
                             join b in resulall on a.Id equals b.Id into c
                             from d in c.DefaultIfEmpty()
                             select new DetailedOfDebtToInvoiceDto
                             {
                                 Id = a.Id,
                                 DocumentId = a.Id,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 PartnerName = a.PartnerName,
                                 DeadlinePayment = a.DeadlinePayment,
                                 OrgCode = a.OrgCode,
                                 Year = a.Year,
                                 AccType = "",
                                 Address = "",
                                 Note = a.Note,
                                 Times = a.Times,
                                 Description = a.Description,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = (decimal)(a.AmountReceivable),
                                 AmountReceived = (decimal)a.AmountReceived,
                                 PaymentType = a.PaymentType,
                                 OrdRec = a.OrdRec,
                                 Remain = a.Remain,
                                 //D0to15 = (decimal)a.Remain,
                                 D16to30 = d != null ? d.D16to30 : a.D16to30,
                                 D31to60 = a.D31to60,
                                 D61to90 = a.D61to90,
                                 D91to120 = a.D91to120,
                                 DOver120 = a.DOver120,
                                 Sort = 1,
                                 D0to15 = a.D0to15
                             }).ToList();

                }
                if (tongSoNgay > 30 && tongSoNgay <= 60)
                {
                    var resulall = (from a in resul
                                    where a.Id == item.Id
                                    select new DetailedOfDebtToInvoiceDto
                                    {

                                        Id = a.Id,
                                        DocumentId = a.Id,
                                        VoucherDate = a.VoucherDate,
                                        VoucherNumber = a.VoucherNumber,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName = a.PartnerName,
                                        DeadlinePayment = a.DeadlinePayment,
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccType = "",
                                        Address = "",
                                        Note = a.Note,
                                        Times = a.Times,
                                        Description = a.Description,
                                        VatAmount = a.VatAmount,
                                        DiscountAmount = a.DiscountAmount,
                                        TotalAmount = a.TotalAmount,
                                        AmountReceivable = (decimal)(a.AmountReceivable),
                                        AmountReceived = (decimal)a.AmountReceived,
                                        PaymentType = a.PaymentType,
                                        OrdRec = a.OrdRec,
                                        Remain = a.Remain,
                                        D0to15 = (decimal)a.D0to15,
                                        D16to30 = (decimal)a.D16to30,
                                        D31to60 = (decimal)a.Remain,
                                        D61to90 = (decimal)0,
                                        D91to120 = (decimal)0,
                                        DOver120 = (decimal)0,
                                        Sort = 1
                                    }).ToList();
                    resul = (from a in resul
                             join b in resulall on a.Id equals b.Id into c
                             from d in c.DefaultIfEmpty()
                             select new DetailedOfDebtToInvoiceDto
                             {
                                 Id = a.Id,
                                 DocumentId = a.Id,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 PartnerName = a.PartnerName,
                                 DeadlinePayment = a.DeadlinePayment,
                                 OrgCode = a.OrgCode,
                                 Year = a.Year,
                                 AccType = "",
                                 Address = "",
                                 Note = a.Note,
                                 Times = a.Times,
                                 Description = a.Description,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = (decimal)(a.AmountReceivable),
                                 AmountReceived = (decimal)a.AmountReceived,
                                 PaymentType = a.PaymentType,
                                 OrdRec = a.OrdRec,
                                 Remain = a.Remain,
                                 //D0to15 = (decimal)a.Remain,
                                 D16to30 = a.D16to30,
                                 D31to60 = d != null ? d.D31to60 : a.D31to60,
                                 D61to90 = a.D61to90,
                                 D91to120 = a.D91to120,
                                 DOver120 = a.DOver120,
                                 Sort = 1,
                                 D0to15 = a.D0to15
                             }).ToList();
                }
                if (tongSoNgay > 60 && tongSoNgay <= 90)
                {
                    var resulall = (from a in resul
                                    where a.Id == item.Id
                                    select new DetailedOfDebtToInvoiceDto
                                    {

                                        Id = a.Id,
                                        DocumentId = a.Id,
                                        VoucherDate = a.VoucherDate,
                                        VoucherNumber = a.VoucherNumber,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName = a.PartnerName,
                                        DeadlinePayment = a.DeadlinePayment,
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccType = "",
                                        Address = "",
                                        Note = a.Note,
                                        Times = a.Times,
                                        Description = a.Description,
                                        VatAmount = a.VatAmount,
                                        DiscountAmount = a.DiscountAmount,
                                        TotalAmount = a.TotalAmount,
                                        AmountReceivable = (decimal)(a.AmountReceivable),
                                        AmountReceived = (decimal)a.AmountReceived,
                                        PaymentType = a.PaymentType,
                                        OrdRec = a.OrdRec,
                                        Remain = a.Remain,
                                        D0to15 = (decimal)a.D0to15,
                                        D16to30 = (decimal)a.D16to30,
                                        D31to60 = (decimal)a.D31to60,
                                        D61to90 = (decimal)a.Remain,
                                        D91to120 = (decimal)0,
                                        DOver120 = (decimal)0,
                                        Sort = 1
                                    }).ToList();
                    resul = (from a in resul
                             join b in resulall on a.Id equals b.Id into c
                             from d in c.DefaultIfEmpty()
                             select new DetailedOfDebtToInvoiceDto
                             {
                                 Id = a.Id,
                                 DocumentId = a.Id,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 PartnerName = a.PartnerName,
                                 DeadlinePayment = a.DeadlinePayment,
                                 OrgCode = a.OrgCode,
                                 Year = a.Year,
                                 AccType = "",
                                 Address = "",
                                 Note = a.Note,
                                 Times = a.Times,
                                 Description = a.Description,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = (decimal)(a.AmountReceivable),
                                 AmountReceived = (decimal)a.AmountReceived,
                                 PaymentType = a.PaymentType,
                                 OrdRec = a.OrdRec,
                                 Remain = a.Remain,
                                 //D0to15 = (decimal)a.Remain,
                                 D16to30 = a.D16to30,
                                 D31to60 = a.D31to60,
                                 D61to90 = d != null ? d.D61to90 : a.D61to90,
                                 D91to120 = a.D91to120,
                                 DOver120 = a.DOver120,
                                 Sort = 1,
                                 D0to15 = a.D0to15
                             }).ToList();
                }
                if (tongSoNgay > 90 && tongSoNgay <= 120)
                {
                    var resulall = (from a in resul
                                    where a.Id == item.Id
                                    select new DetailedOfDebtToInvoiceDto
                                    {

                                        Id = a.Id,
                                        DocumentId = a.Id,
                                        VoucherDate = a.VoucherDate,
                                        VoucherNumber = a.VoucherNumber,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName = a.PartnerName,
                                        DeadlinePayment = a.DeadlinePayment,
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccType = "",
                                        Address = "",
                                        Note = a.Note,
                                        Times = a.Times,
                                        Description = a.Description,
                                        VatAmount = a.VatAmount,
                                        DiscountAmount = a.DiscountAmount,
                                        TotalAmount = a.TotalAmount,
                                        AmountReceivable = (decimal)(a.AmountReceivable),
                                        AmountReceived = (decimal)a.AmountReceived,
                                        PaymentType = a.PaymentType,
                                        OrdRec = a.OrdRec,
                                        Remain = a.Remain,
                                        D0to15 = (decimal)a.D0to15,
                                        D16to30 = (decimal)a.D16to30,
                                        D31to60 = (decimal)a.D31to60,
                                        D61to90 = (decimal)a.D61to90,
                                        D91to120 = (decimal)a.Remain,
                                        DOver120 = a.DOver120,
                                        Sort = 1
                                    }).ToList();
                    resul = (from a in resul
                             join b in resulall on a.Id equals b.Id into c
                             from d in c.DefaultIfEmpty()
                             select new DetailedOfDebtToInvoiceDto
                             {
                                 Id = a.Id,
                                 DocumentId = a.Id,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 PartnerName = a.PartnerName,
                                 DeadlinePayment = a.DeadlinePayment,
                                 OrgCode = a.OrgCode,
                                 Year = a.Year,
                                 AccType = "",
                                 Address = "",
                                 Note = a.Note,
                                 Times = a.Times,
                                 Description = a.Description,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = a.AmountReceivable,
                                 AmountReceived = a.AmountReceived,
                                 PaymentType = a.PaymentType,
                                 OrdRec = a.OrdRec,
                                 Remain = a.Remain,
                                 D16to30 = a.D16to30,
                                 D31to60 = a.D31to60,
                                 D61to90 = a.D61to90,
                                 D91to120 = d != null ? d.D91to120 : a.D91to120,
                                 DOver120 = a.DOver120,
                                 Sort = 1,
                                 D0to15 = a.D0to15
                             }).ToList();
                }
                if (tongSoNgay > 120)
                {
                    var resulall = (from a in resul
                                    where a.Id == item.Id
                                    select new DetailedOfDebtToInvoiceDto
                                    {

                                        Id = a.Id,
                                        DocumentId = a.Id,
                                        VoucherDate = a.VoucherDate,
                                        VoucherNumber = a.VoucherNumber,
                                        AccCode = a.AccCode,
                                        PartnerCode = a.PartnerCode,
                                        PartnerName = a.PartnerName,
                                        DeadlinePayment = a.DeadlinePayment,
                                        OrgCode = a.OrgCode,
                                        Year = a.Year,
                                        AccType = "",
                                        Address = "",
                                        Note = a.Note,
                                        Times = a.Times,
                                        Description = a.Description,
                                        VatAmount = a.VatAmount,
                                        DiscountAmount = a.DiscountAmount,
                                        TotalAmount = a.TotalAmount,
                                        AmountReceivable = (decimal)(a.AmountReceivable),
                                        AmountReceived = (decimal)a.AmountReceived,
                                        PaymentType = a.PaymentType,
                                        OrdRec = a.OrdRec,
                                        Remain = a.Remain,
                                        D0to15 = (decimal)a.D0to15,
                                        D16to30 = (decimal)a.D16to30,
                                        D31to60 = (decimal)a.D31to60,
                                        D61to90 = (decimal)a.D61to90,
                                        D91to120 = (decimal)a.D91to120,
                                        DOver120 = (decimal)a.Remain,
                                        Sort = 1
                                    }).ToList();
                    resul = (from a in resul
                             join b in resulall on a.Id equals b.Id into c
                             from d in c.DefaultIfEmpty()
                             select new DetailedOfDebtToInvoiceDto
                             {
                                 Id = a.Id,
                                 DocumentId = a.Id,
                                 VoucherDate = a.VoucherDate,
                                 VoucherNumber = a.VoucherNumber,
                                 AccCode = a.AccCode,
                                 PartnerCode = a.PartnerCode,
                                 PartnerName = a.PartnerName,
                                 DeadlinePayment = a.DeadlinePayment,
                                 OrgCode = a.OrgCode,
                                 Year = a.Year,
                                 AccType = "",
                                 Address = "",
                                 Note = a.Note,
                                 Times = a.Times,
                                 Description = a.Description,
                                 VatAmount = a.VatAmount,
                                 DiscountAmount = a.DiscountAmount,
                                 TotalAmount = a.TotalAmount,
                                 AmountReceivable = (decimal)(a.AmountReceivable),
                                 AmountReceived = (decimal)a.AmountReceived,
                                 PaymentType = a.PaymentType,
                                 OrdRec = a.OrdRec,
                                 Remain = a.Remain,
                                 //D0to15 = (decimal)a.Remain,
                                 D16to30 = a.D16to30,
                                 D31to60 = a.D31to60,
                                 D61to90 = a.D61to90,
                                 D91to120 = a.D91to120,
                                 DOver120 = d != null ? d.DOver120 : a.DOver120,
                                 Sort = 1,
                                 D0to15 = a.D0to15
                             }).ToList();
                }
            }
            var resulAll = (from a in resul
                            group new
                            {
                                a.PartnerCode,
                                a.PartnerName,
                                a.Remain,
                                a.D0to15,
                                a.D16to30,
                                a.D31to60,
                                a.D61to90,
                                a.D91to120,
                                a.DOver120
                            } by new
                            {
                                a.PartnerCode,
                                // a.PartnerName
                            } into gr
                            select new DetailedOfDebtToInvoiceDto
                            {
                                Bold = "C",
                                Sort = 2,
                                PartnerCode = gr.Key.PartnerCode,
                                PartnerName = gr.Max(p => p.PartnerName),
                                Remain = (decimal)gr.Sum(p => p.Remain),
                                Note = "Tổng tiền",
                                D0to15 = gr.Sum(p => p.D0to15),
                                D16to30 = gr.Sum(p => p.D16to30),
                                D31to60 = gr.Sum(p => p.D31to60),
                                D61to90 = gr.Sum(p => p.D61to90),
                                D91to120 = gr.Sum(p => p.D91to120),
                                DOver120 = gr.Sum(p => p.DOver120)
                            }).ToList();

            List<DetailedOfDebtToInvoiceDto> detailedOfDebtToInvoiceDtos = new List<DetailedOfDebtToInvoiceDto>();
            detailedOfDebtToInvoiceDtos.AddRange(resul);
            detailedOfDebtToInvoiceDtos.AddRange(resulAll);

            //var resulVoucherdetai = await _voucherPaymentBookService.GetQueryableAsync();
            //var lstVoucherpaymet = resulVoucherdetai.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


            //var accVoucherService = await _accVoucherService.GetQueryableAsync();
            //var lstAccVoucherService = accVoucherService.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            //var resul1 = from a in lstVoucherpaymet
            //             join b in lstAccVoucherService on a.AccVoucherId equals b.Id
            //             where a.DocumentId.Contains(resul.Select(p => p.DocumentId).ToString()) == true
            //             select new DetailedOfDebtToInvoiceDto
            //             {

            //                 Id = a.Id,
            //                 DocumentId = a.Id,
            //                 VoucherDate = a.VoucherDate,
            //                 VoucherNumber = a.VoucherNumber,
            //                 AccCode = a.AccCode,
            //                 PartnerCode = a.PartnerCode,
            //                 PartnerName = "",
            //                 DeadlinePayment = a.DeadlinePayment,
            //                 OrgCode = a.OrgCode,
            //                 Year = a.Year,
            //                 AccType = "",
            //                 Address = "",
            //                 Representative = "",
            //                 Times = a.Times,
            //                 Description = b.Description,
            //                 VatAmount = a.VatAmount,
            //                 DiscountAmount = a.DiscountAmount,
            //                 TotalAmount = a.TotalAmount,
            //                 AmountReceivable = (decimal)(a.AmountReceivable),
            //                 AmountReceived = (decimal)a.AmountReceived,
            //                 PaymentType = "",
            //                 OrdRec = "",
            //                 Remain = (decimal)0,
            //                 D0to15 = (decimal)0,
            //                 D16to30 = (decimal)0,
            //                 D31to60 = (decimal)0,
            //                 D61to90 = (decimal)0,
            //                 D91to120 = (decimal)0,
            //                 DOver120 = (decimal)0,
            //                 Sort = 1
            //             };
            //resul.AddRange(resul1);


            //var productVoucher = await _productVoucherService.GetQueryableAsync();
            //var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            //var resul2 = from a in lstVoucherpaymet
            //             join b in lstProductVoucher on a.DocumentId equals b.Id
            //             where a.DocumentId.Contains(resul.Select(p => p.DocumentId).ToString()) == true
            //             select new DetailedOfDebtToInvoiceDto
            //             {

            //                 Id = a.Id,
            //                 DocumentId = a.Id,
            //                 VoucherDate = a.VoucherDate,
            //                 VoucherNumber = a.VoucherNumber,
            //                 AccCode = a.AccCode,
            //                 PartnerCode = a.PartnerCode,
            //                 PartnerName = "",
            //                 DeadlinePayment = a.DeadlinePayment,
            //                 OrgCode = a.OrgCode,
            //                 Year = a.Year,
            //                 AccType = "",
            //                 Address = "",
            //                 Representative = "",
            //                 Times = a.Times,
            //                 Description = b.Description,
            //                 VatAmount = a.VatAmount,
            //                 DiscountAmount = a.DiscountAmount,
            //                 TotalAmount = a.TotalAmount,
            //                 AmountReceivable = (decimal)(a.AmountReceivable),
            //                 AmountReceived = (decimal)0,
            //                 PaymentType = "",
            //                 OrdRec = "",
            //                 Remain = (decimal)0,
            //                 D0to15 = (decimal)0,
            //                 D16to30 = (decimal)0,
            //                 D31to60 = (decimal)0,
            //                 D61to90 = (decimal)0,
            //                 D91to120 = (decimal)0,
            //                 DOver120 = (decimal)0,
            //                 Sort = 1
            //             };
            //resul.AddRange(resul2);
            detailedOfDebtToInvoiceDtos = detailedOfDebtToInvoiceDtos.OrderBy(p => p.PartnerCode).ThenBy(p => p.Sort).ThenBy(p => p.VoucherDate).ToList();
            var reportResponse = new ReportResponseDto<DetailedOfDebtToInvoiceDto>();
            reportResponse.Data = detailedOfDebtToInvoiceDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.DetailDebtInvoiceReportPrint)]
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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
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

