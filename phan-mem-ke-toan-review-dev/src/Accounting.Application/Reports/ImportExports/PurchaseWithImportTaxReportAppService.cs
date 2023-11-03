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
using Accounting.Categories.Products;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class PurchaseWithImportTaxReportAppService : AccountingAppService
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
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly ProductGroupAppService _productGroupAppService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly CurrencyService _currencyService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public PurchaseWithImportTaxReportAppService(ReportDataService reportDataService,
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
                        ProductVoucherService productVoucherService,
                        ProductVoucherDetailService productVoucherDetailService,
                        ProductGroupAppService productGroupAppService,
                        VoucherCategoryService voucherCategoryService,
                        VoucherTypeService voucherTypeService,
                        CurrencyService currencyService,
                        AccountingCacheManager accountingCacheManager
                        )
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
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _productGroupAppService = productGroupAppService;
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _currencyService = currencyService;
            _accountingCacheManager = accountingCacheManager;
        }
        [Authorize(ReportPermissions.PurchaseImportTaxReportView)]
        public async Task<ReportResponseDto<PurchaseWithImportTaxDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var dic = GetWarehouseBookParameter(dto.Parameters);
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var incurredData = await GetIncurredData(dic);
            var voucherTypes = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucherTypes = voucherTypes.Where(p => p.Code == "PNH").ToList().FirstOrDefault();
            var lstVoucher = lstVoucherTypes.ListVoucher;
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            incurredData = (from p in incurredData
                            join b in lstPartner on p.PartnerCode equals b.Code into c
                            from pn in c.DefaultIfEmpty()
                            join pr in lstProduct on p.ProductCode equals pr.Code into pro
                            from d in pro.DefaultIfEmpty()
                            where lstVoucher.Contains(p.VoucherCode) == true
                            select new PurchaseWithImportTaxDto
                            {
                                Tag = 0,
                                Sort = 2,
                                VoucherCode = p.VoucherCode,
                                OrgCode = p.OrgCode,
                                SortGroupProduct = null,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                ProductCode = p.ProductCode,
                                PartnerCode = p.PartnerCode,
                                ProductGroupCode = p.VoucherDate.ToString(),
                                Unit = p.Unit,
                                SectionCode = p.SectionCode,
                                FProductWorkCode = p.FProductWorkCode,
                                CaseCode = p.CaseCode,
                                WarehouseCode = p.WarehouseCode,
                                DepartmentCode = p.DepartmentCode,
                                ProductOriginCode = p.ProductOriginCode,
                                ProductLotCode = p.ProductLotCode,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                VoucherNumber = p.VoucherNumber,
                                Description = p.Description,
                                Note = p.Note,
                                Quantity = p.Quantity,
                                Price = p.Price,
                                PriceCur = p.PriceCur,
                                Amount = p.Amount,
                                AmountCur = p.AmountCur,
                                VatAmount = p.VatAmount,
                                VatAmountCur = p.VatAmountCur,
                                ExpenseAmount = p.ExpenseAmount,
                                ExpenseAmountCur = p.ExpenseAmountCur,
                                ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                ImportTaxAmountCur = p.ImportTaxAmountCur,
                                AmountPay = p.AmountPay,
                                AmountPayCur = p.AmountPayCur,
                                AccCode = p.AccCode,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceNumber = p.InvoiceNumber,
                                InvoiceSymbol = p.InvoiceSymbol,
                                ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                CaseName = null,
                                PartnerName = pn != null ? pn.Name : null,
                                SectionName = null,
                                DepartmentName = null,
                                FProductWorkName = null,
                                ProductOriginName = null,
                                ProductLotName = null,
                                CurrencyName = null,
                                AccName = null,
                                VoucherDate = p.VoucherDate,
                                ProductName = d != null ? d.Name : null
                            }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = (from p in incurredData
                                where GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) <= GetVoucherNumber(p.VoucherNumber)
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    VoucherCode = p.VoucherCode,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.VoucherDate.ToString(),
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var partners = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroup);
                incurredData = (from p in incurredData
                                join b in partners on p.PartnerCode equals b.Code
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    VoucherCode = p.VoucherCode,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.VoucherDate.ToString(),
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var products = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode);
                incurredData = (from p in incurredData
                                join b in products on p.ProductCode equals b.Code
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    VoucherCode = p.VoucherCode,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.VoucherDate.ToString(),
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
            }
            var totalQuantity = incurredData.Select(p => p.Quantity).Sum();
            var totalAmount = incurredData.Select(p => p.Amount).Sum();
            var totalAmountCur = incurredData.Select(p => p.AmountCur).Sum();
            var totalVatAmount = incurredData.Select(p => p.VatAmount).Sum();
            var totalVatAmountCur = incurredData.Select(p => p.VatAmountCur).Sum();
            var toatalExpenseAmount = incurredData.Select(p => p.ExpenseAmount).Sum();
            var totalExpenseAmountCur = incurredData.Select(p => p.ExpenseAmountCur).Sum();
            var totalImportTaxAmount = incurredData.Select(p => p.ImportTaxAmount).Sum();
            var totalImportTaxAmountCur = incurredData.Select(p => p.ImportTaxAmountCur).Sum();
            var totalAmountPay = incurredData.Select(p => p.AmountPay).Sum();
            var totalAmountPayCur = incurredData.Select(p => p.AmountPay).Sum();
            List<PurchaseWithImportTaxDto> lstPurchaseWithImportTaxDtos = new List<PurchaseWithImportTaxDto>();
            if (dto.Parameters.Sort == "1")
            {

                var aktGroup1 = (from p in incurredData
                                 join n in lstProduct on p.ProductCode equals n.Code into c
                                 from pr in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     pr.Name,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur,
                                     p.Description,
                                     p.VoucherDate,
                                     p.VoucherNumber
                                 } by new
                                 {
                                     p.VoucherDate,
                                     p.VoucherNumber
                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.VoucherDate.ToString() + gr.Key.VoucherNumber,
                                     GroupCode = gr.Key.VoucherDate.ToString() + gr.Key.VoucherNumber,
                                     GroupName = gr.Max(p => p.Name),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                     Description = gr.Max(p => p.Description),
                                     VoucherDate = gr.Max(p => p.VoucherDate),
                                     VoucherNumber = gr.Max(p => p.VoucherNumber)

                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].Description;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    purchaseWithImportTaxDto.VoucherNumber = aktGroup1[i].VoucherNumber;
                    purchaseWithImportTaxDto.VoucherDate = aktGroup1[i].VoucherDate;

                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 3,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.VoucherDate.ToString() + p.VoucherNumber,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    // VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.ProductCode + " - " + p.ProductName,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    //VoucherDate = p.VoucherDate
                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "2")
            {
                var aktGroup1 = (from p in incurredData

                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.PartnerCode,
                                     p.PartnerName,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.PartnerCode,
                                     p.PartnerName
                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.PartnerCode,
                                     GroupCode = gr.Key.PartnerCode,
                                     GroupName = gr.Max(p => p.PartnerName),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate

                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "3")
            {
                var caseCode = await _accCaseService.GetQueryableAsync();
                var lstCaseCode = caseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var aktGroup1 = (from p in incurredData
                                 join b in lstCaseCode on p.CaseCode equals b.Code into c
                                 from ca in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.CaseCode,
                                     Name = ca != null ? ca.Name : null,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.CaseCode,
                                     Name = ca != null ? ca.Name : null
                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.CaseCode,
                                     GroupCode = gr.Key.CaseCode,
                                     GroupName = gr.Max(p => p.Name),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.CaseCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate

                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "4")
            {
                var sections = await _accSectionService.GetQueryableAsync();
                var lstsections = sections.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var aktGroup1 = (from p in incurredData
                                 join b in lstsections on p.WarehouseCode equals b.Code into c
                                 from wa in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.WarehouseCode,
                                     Name = wa != null ? wa.Name : null,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.SectionCode

                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.SectionCode,
                                     GroupCode = gr.Key.SectionCode,
                                     GroupName = gr.Max(p => p.Name),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.SectionCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "8")
            {
                var accountSystems = await _accountSystemService.GetQueryableAsync();
                var lstAccountSystems = accountSystems.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
                var aktGroup1 = (from p in incurredData
                                 join b in lstAccountSystems on p.AccCode equals b.AccCode into c
                                 from wa in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.AccCode,
                                     AccName = wa != null ? wa.AccName : null,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.AccCode

                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.AccCode,
                                     GroupCode = gr.Key.AccCode,
                                     GroupName = gr.Max(p => p.AccName),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.AccCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "5")
            {
                var departments = await _departmentService.GetQueryableAsync();
                var lstDepartments = departments.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var aktGroup1 = (from p in incurredData
                                 join b in lstDepartments on p.DepartmentCode equals b.Code into c
                                 from wa in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.DepartmentCode,
                                     Name = wa != null ? wa.Name : null,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.DepartmentCode,

                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.DepartmentCode,
                                     GroupCode = gr.Key.DepartmentCode,
                                     GroupName = gr.Max(p => p.Name),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.DepartmentCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "6")
            {
                var fProductWorks = await _fProductWorkService.GetQueryableAsync();
                var lstFProductWorks = fProductWorks.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var aktGroup1 = (from p in incurredData
                                 join b in lstFProductWorks on p.FProductWorkCode equals b.Code into c
                                 from wa in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.FProductWorkCode,
                                     Name = wa != null ? wa.Name : null,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.FProductWorkCode,

                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.FProductWorkCode,
                                     GroupCode = gr.Key.FProductWorkCode,
                                     GroupName = gr.Max(p => p.Name),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.FProductWorkCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }
            if (dto.Parameters.Sort == "7")
            {
                var currencies = await _currencyService.GetQueryableAsync();
                var lstcurrencies = currencies.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var aktGroup1 = (from p in incurredData
                                 join b in lstcurrencies on p.CurrencyCode equals b.Code into c
                                 from wa in c.DefaultIfEmpty()
                                 group new
                                 {
                                     p.SortGroupProduct,
                                     p.ProductCode,
                                     p.FProductWorkCode,
                                     Name = wa != null ? wa.Name : null,
                                     p.Quantity,
                                     p.Amount,
                                     p.AmountCur,
                                     p.VatAmount,
                                     p.VatAmountCur,
                                     p.ExpenseAmount,
                                     p.ExpenseAmountCur,
                                     p.ImportTaxAmount,
                                     p.ImportTaxAmountCur,
                                     p.AmountPay,
                                     p.AmountPayCur
                                 } by new
                                 {
                                     p.CurrencyCode,

                                 } into gr
                                 select new PurchaseWithImportTaxDto
                                 {
                                     ProductGroupCode = gr.Key.CurrencyCode,
                                     GroupCode = gr.Key.CurrencyCode,
                                     GroupName = gr.Max(p => p.Name),
                                     Quantity = gr.Sum(p => p.Quantity),
                                     Amount = gr.Sum(p => p.Amount),
                                     AmountCur = gr.Sum(p => p.AmountCur),
                                     VatAmount = gr.Sum(p => p.VatAmount),
                                     VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                     ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                     ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                     ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                     ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                     AmountPay = gr.Sum(p => p.AmountPay),
                                     AmountPayCur = gr.Sum(p => p.AmountPayCur)
                                 }).ToList();
                for (int i = 0; i < aktGroup1.Count; i++)
                {
                    PurchaseWithImportTaxDto purchaseWithImportTaxDto = new PurchaseWithImportTaxDto();
                    purchaseWithImportTaxDto.Tag = 0;
                    purchaseWithImportTaxDto.Bold = "C";
                    purchaseWithImportTaxDto.Sort = 1;
                    purchaseWithImportTaxDto.ProductGroupCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.ProductCode = aktGroup1[i].ProductGroupCode;
                    purchaseWithImportTaxDto.Description = aktGroup1[i].GroupCode + " - " + aktGroup1[i].GroupName;
                    purchaseWithImportTaxDto.Quantity = aktGroup1[i].Quantity;
                    purchaseWithImportTaxDto.Amount = aktGroup1[i].Amount;
                    purchaseWithImportTaxDto.AmountCur = aktGroup1[i].AmountCur;
                    purchaseWithImportTaxDto.VatAmount = aktGroup1[i].VatAmount;
                    purchaseWithImportTaxDto.VatAmountCur = aktGroup1[i].VatAmountCur;
                    purchaseWithImportTaxDto.ExpenseAmount = aktGroup1[i].ExpenseAmount;
                    purchaseWithImportTaxDto.ExpenseAmountCur = aktGroup1[i].ExpenseAmountCur;
                    purchaseWithImportTaxDto.ImportTaxAmount = aktGroup1[i].ImportTaxAmount;
                    purchaseWithImportTaxDto.ImportTaxAmountCur = aktGroup1[i].ImportTaxAmountCur;
                    purchaseWithImportTaxDto.AmountPay = aktGroup1[i].AmountPay;
                    purchaseWithImportTaxDto.AmountPayCur = aktGroup1[i].AmountPayCur;
                    lstPurchaseWithImportTaxDtos.Add(purchaseWithImportTaxDto);

                }
                incurredData = (from p in incurredData
                                select new PurchaseWithImportTaxDto
                                {
                                    Tag = 0,
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortGroupProduct = null,
                                    VoucherId = p.VoucherId,
                                    VoucherCode = p.VoucherCode,
                                    Id = p.Id,
                                    ProductCode = p.ProductCode,
                                    PartnerCode = p.PartnerCode,
                                    ProductGroupCode = p.CurrencyCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    ProductLotCode = p.ProductLotCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode + " - " + p.ProductName,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExpenseAmount = p.ExpenseAmount,
                                    ExpenseAmountCur = p.ExpenseAmountCur,
                                    ImportTaxAmount = p.ImportTaxAmount,//t_nk
                                    ImportTaxAmountCur = p.ImportTaxAmountCur,
                                    AmountPay = p.AmountPay,
                                    AmountPayCur = p.AmountPayCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = null,
                                    PartnerName = p.PartnerName,
                                    SectionName = null,
                                    DepartmentName = null,
                                    FProductWorkName = null,
                                    ProductOriginName = null,
                                    ProductLotName = null,
                                    CurrencyName = null,
                                    AccName = null,
                                    VoucherDate = p.VoucherDate
                                }).ToList();
                lstPurchaseWithImportTaxDtos.AddRange(incurredData);
            }


            PurchaseWithImportTaxDto crud = new PurchaseWithImportTaxDto();
            crud.Tag = 0;
            crud.ProductGroupCode = "zzzzzzz";
            crud.Sort = 3;
            crud.Bold = "C";
            crud.Quantity = totalQuantity;
            crud.Amount = totalAmount;
            crud.AmountCur = totalAmountCur;
            crud.ExpenseAmount = toatalExpenseAmount;
            crud.ExpenseAmountCur = totalExpenseAmountCur;
            crud.VatAmount = totalVatAmount;
            crud.VatAmountCur = totalVatAmountCur;
            crud.ImportTaxAmount = totalImportTaxAmount;
            crud.ImportTaxAmountCur = totalImportTaxAmountCur;
            crud.Description = "Tổng cộng";
            lstPurchaseWithImportTaxDtos.Add(crud);
            lstPurchaseWithImportTaxDtos = lstPurchaseWithImportTaxDtos.OrderBy(p => p.ProductGroupCode)
                                                                       .ThenBy(p => p.Sort)
                                                                       //.ThenBy(p => p.VoucherNumber)
                                                                       //.ThenBy(p => p.VoucherDate)
                                                                       .ToList();

            var reportResponse = new ReportResponseDto<PurchaseWithImportTaxDto>();
            reportResponse.Data = lstPurchaseWithImportTaxDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

        [Authorize(ReportPermissions.PurchaseImportTaxReportPrint)]
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
        #region Private
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
        private async Task<List<PurchaseWithImportTaxDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Select(p => new PurchaseWithImportTaxDto()
            {
                Tag = 0,
                Sort = 2,
                OrgCode = p.OrgCode,
                SortGroupProduct = null,
                VoucherId = p.VoucherId,
                VoucherCode = p.VoucherCode,
                Id = p.Id,
                ProductCode = p.ProductCode,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                ProductGroupCode = null,
                Unit = p.UnitCode,
                SectionCode = p.SectionCode,
                FProductWorkCode = p.FProductWorkCode,
                CaseCode = p.CaseCode,
                WarehouseCode = p.WarehouseCode,
                DepartmentCode = p.DepartmentCode,
                ProductOriginCode = p.ProductOriginCode,
                ProductLotCode = p.ProductLotCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                VoucherNumber = p.VoucherNumber,
                Description = p.Description,
                Note = p.Note,
                Quantity = p.ImportQuantity,
                Price = p.Price,
                PriceCur = p.PriceCur,
                Amount = p.Amount,
                AmountCur = p.ImportAmountCur,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                ExpenseAmount = p.ExpenseAmount,
                ExpenseAmountCur = p.ExpenseAmountCur,
                ImportTaxAmount = p.ImportTaxAmount,//t_nk
                ImportTaxAmountCur = p.ImportTaxAmountCur,
                AmountPay = p.Amount + p.ExpenseAmount + p.ImportTaxAmount,
                AmountPayCur = p.AmountCur + p.ExpenseAmountCur + p.ImportTaxAmountCur,
                AccCode = p.ImportAcc,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                InvoiceDate = p.InvoiceDate,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceSymbol = p.InvoiceSymbol,
                ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                CaseName = null,
                PartnerName = null,
                SectionName = null,
                DepartmentName = null,
                FProductWorkName = null,
                ProductOriginName = null,
                ProductLotName = null,
                CurrencyName = null,
                AccName = null,
                VoucherDate = p.VoucherDate
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
            dic.Add(WarehouseBookParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            //dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");
            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }

            if (!string.IsNullOrEmpty(dto.EndVoucherNumber))
            {
                dic.Add(WarehouseBookParameterConst.EndVoucherNumber, dto.EndVoucherNumber);
            }
            if (!string.IsNullOrEmpty(dto.BeginVoucherNumber))
            {
                dic.Add(WarehouseBookParameterConst.BeginVoucherNumber, dto.BeginVoucherNumber);
            }

            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
            }
            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            if (!string.IsNullOrEmpty(dto.PartnerGroup))
            {
                dic.Add(WarehouseBookParameterConst.PartnerGroup, dto.PartnerGroup);
            }
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(WarehouseBookParameterConst.PartnerCode, dto.PartnerCode);
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
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductLotCode, dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.CaseCode))
            {
                dic.Add(WarehouseBookParameterConst.CaseCode, dto.CaseCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(WarehouseBookParameterConst.SectionCode, dto.SectionCode);
            }

            if (!string.IsNullOrEmpty(dto.VoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.VoucherCode, dto.VoucherCode);
            }
            if (!string.IsNullOrEmpty(dto.Sort))
            {
                dic.Add(WarehouseBookParameterConst.Sort, dto.Sort);
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

