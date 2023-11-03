using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace Accounting.Reports.ImportExports
{
    public class PurchaseWithTaxAppService : AccountingAppService
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
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly CurrencyService _currencyService;
        private readonly SaleChannelService _saleChannelService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public PurchaseWithTaxAppService(ReportDataService reportDataService,
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
                        WorkPlaceSevice workPlaceSevice,
                        BusinessCategoryService businessCategoryService,
                        CurrencyService currencyService,
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
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _productGroupAppService = productGroupAppService;
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _workPlaceSevice = workPlaceSevice;
            _businessCategoryService = businessCategoryService;
            _currencyService = currencyService;
            _saleChannelService = saleChannelService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.PurchaseWithTaxListReportView)]
        public async Task<ReportResponseDto<PurchaseWithTaxDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var products = await _productService.GetQueryableAsync();
            var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var lstVoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType = lstVoucherType.Where(p => p.Code == "PNH").FirstOrDefault().ListVoucher;
            dto.Parameters.LstVoucherCode = voucherType;
            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);


            var caseCodes = await _accCaseService.GetQueryableAsync();
            var lstCaseCode = caseCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var sessionCodes = await _accSectionService.GetQueryableAsync();
            var lstSessionCode = sessionCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var partnerCode = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partnerCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var departments = await _departmentService.GetQueryableAsync();
            var lstDepartmen = departments.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstFProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var currencys = await _currencyService.GetQueryableAsync();
            var lstCurrency = currencys.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accCodes = await _accountSystemService.GetQueryableAsync();
            var lstAccCode = accCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();


            var salesChannels = await _saleChannelService.GetQueryableAsync();
            var lstsalesChannels = salesChannels.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var wareHouse = await _warehouseService.GetQueryableAsync();
            var lstWareHouse = wareHouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var businessCodes = await _businessCategoryService.GetQueryableAsync();
            var lstBusinessCodes = businessCodes.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            incurredData = (from p in incurredData
                            join c in lstProducts on p.ProductCode equals c.Code into m
                            from pr in m.DefaultIfEmpty()
                            join b in lstCaseCode on p.CaseCode equals b.Code into i
                            from ca in i.DefaultIfEmpty()
                            join d in lstSessionCode on p.SectionCode equals d.Code into u
                            from se in u.DefaultIfEmpty()
                            join y in lstDepartmen on p.DepartmentCode equals y.Code into q
                            from de in q.DefaultIfEmpty()
                            join e in lstFProductWork on p.FProductWorkCode equals e.Code into r
                            from fp in r.DefaultIfEmpty()
                            join t in lstCurrency on p.CurrencyCode equals t.Code into g
                            from cu in g.DefaultIfEmpty()
                            join n in lstAccCode on p.ReciprocalAcc equals n.AccCode into k
                            from acc in k.DefaultIfEmpty()
                            join a in lstsalesChannels on p.SalesChannelCode equals a.Code into h
                            from sa in h.DefaultIfEmpty()
                            join mn in lstPartner on p.PartnerCode equals mn.Code into ps
                            from pa in ps.DefaultIfEmpty()
                            join ty in lstBusinessCodes on p.BusinessCode equals ty.Code into pe
                            from bu in pe.DefaultIfEmpty()
                            join wq in lstWareHouse on p.WarehouseCode equals wq.Code into pi
                            from wa in pi.DefaultIfEmpty()
                            select new PurchaseWithTaxDto
                            {
                                Tag = 0,
                                Sort0 = 2,
                                Bold = "K",
                                Year = p.Year,
                                SortGroupProduct = null,
                                OrgCode = p.OrgCode,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                Ord0 = p.Ord0,
                                VoucherCode = p.VoucherCode,
                                VoucherDate = p.VoucherDate,
                                ProductCode = p.ProductCode,
                                GroupCode = null,
                                PartnerName = pa != null ? pa.Name : null,
                                PartnerCode = p.PartnerCode,
                                Unit = p.Unit,
                                SectionCode = p.SectionCode,
                                FProductWorkCode = p.FProductWorkCode,
                                CaseCode = p.CaseCode,
                                WarehouseCode = p.WarehouseCode,
                                DepartmentCode = p.DepartmentCode,
                                SalesChannelCode = p.SalesChannelCode,
                                ProductLotCode = p.ProductLotCode,
                                ProductOriginCode = p.ProductOriginCode,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                Note = p.Note,
                                VoucherNumber = p.VoucherNumber,
                                Description = p.Description,
                                Quantity = p.Quantity,
                                Price = p.Price ?? 0,
                                PriceCur0 = p.PriceCur0 ?? 0,
                                Amount = p.Amount ?? 0,
                                AmountCur = p.AmountCur ?? 0,
                                VatAmount = p.VatAmount ?? 0,
                                VatAmountCur = p.VatAmountCur ?? 0,
                                AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                AccCode = p.AccCode,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceNumber = p.InvoiceNumber,
                                InvoiceSymbol = p.InvoiceSymbol,
                                ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                CaseName = ca != null ? ca.Name : null,
                                SectionName = ca != null ? ca.Name : null,
                                DepartmentName = de != null ? de.Name : null,
                                FProductWorkName = fp != null ? fp.Name : null,
                                CurrencyName = cu != null ? cu.Name : null,
                                AccName = acc != null ? acc.AccName : null,
                                WarehouseName = wa != null ? wa.Name : null,
                                Address = p.Address,
                                Representative = p.Representative,
                                ProductName = pr != null ? pr.Name : null
                            }).ToList();

            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = (from p in incurredData
                                where GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) <= GetVoucherNumber(p.VoucherNumber)
                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = null,
                                    PartnerName = p.PartnerName,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.ImportQuantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var lstpartnerGroup = await _partnerGroupService.GetQueryableAsync();
                var partnerGroup = lstpartnerGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Id == dto.Parameters.PartnerGroup).FirstOrDefault().Code;
                var partners = await _accPartnerAppService.GetListByPartnerGroupCode(partnerGroup);
                incurredData = (from p in incurredData
                                join b in partners on p.PartnerCode equals b.Code
                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = null,
                                    PartnerName = p.PartnerName,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var product = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode);
                incurredData = (from p in incurredData
                                join b in product on p.ProductCode equals b.Code
                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = null,
                                    PartnerName = p.PartnerName,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price0 ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
            }

            var totalQuantity = incurredData.Select(p => p.Quantity).Sum();
            var totalAmount = incurredData.Select(p => p.Amount).Sum();
            var totalAmountCur = incurredData.Select(p => p.AmountCur).Sum();
            var totalAmountFunds = incurredData.Select(p => p.AmountFunds).Sum();
            var totalAmountFundsCur = incurredData.Select(p => p.AmountFundsCur).Sum();
            var totalDevaluationAmount = incurredData.Select(p => p.DevaluationAmount).Sum();
            var totalDevaluationAmountCur = incurredData.Select(p => p.DevaluationAmountCur).Sum();
            var totalDiscountAmount = incurredData.Select(p => p.DiscountAmount).Sum();
            var totalDiscountAmountCur = incurredData.Select(p => p.DiscountAmountCur).Sum();
            var totalExpenseAmount = incurredData.Select(p => p.ExpenseAmount).Sum();
            var toatlExpenseAmountCur = incurredData.Select(p => p.ExpenseAmountCur).Sum();
            var totalVatAmount = incurredData.Select(p => p.VatAmount).Sum();
            var totalVatAmountCur = incurredData.Select(p => p.VatAmountCur).Sum();
            var TurnoverAmount = totalAmount + totalDiscountAmount + totalVatAmount;
            var TurnoverAmountCur = totalAmountCur + totalDiscountAmountCur + totalVatAmountCur;
            var totalAmountPay = incurredData.Select(p => p.AmountPay).Sum();
            var totalAmountPayCur = incurredData.Select(p => p.AmountPayCur).Sum();
            //product
            if (dto.Parameters.Sort == "1")
            {
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.ProductCode,
                                    p.ProductName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur,
                                    p.Description
                                } by new
                                {
                                    p.GroupCode,
                                    p.VoucherNumber,
                                    p.VoucherDate
                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.VoucherDate.ToString() + gr.Key.VoucherNumber,
                                    GroupName = null,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    VoucherNumber = gr.Key.VoucherNumber,
                                    VoucherDate = gr.Key.VoucherDate,
                                    Description = gr.Max(p => p.Description)
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.ProductCode = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.VoucherDate = aktGroup[i].VoucherDate;
                    purchaseWithTaxDto.VoucherNumber = aktGroup[i].VoucherNumber;

                    lst.Add(purchaseWithTaxDto);
                }

                incurredData = (from p in incurredData
                                where p.Bold == "K"
                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    //VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.VoucherDate.ToString() + p.VoucherNumber,
                                    PartnerName = p.PartnerName,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    //VoucherNumber = p.VoucherNumber,
                                    Description = p.ProductCode,
                                    Quantity = p.Quantity,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "2")
            {
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.PartnerCode,
                                    p.PartnerName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur
                                } by new
                                {
                                    p.PartnerCode,
                                    p.PartnerName
                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.PartnerCode,
                                    GroupName = gr.Key.PartnerName,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.PartnerCode + " - " + gr.Key.PartnerName
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.ProductCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.PartnerCode,
                                    PartnerName = null,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();

                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "3")
            {
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.CaseCode,
                                    PartnerName = null,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.CaseCode,
                                    p.CaseName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur
                                } by new
                                {
                                    p.CaseName,
                                    p.CaseCode
                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.CaseCode,
                                    GroupName = gr.Key.CaseName,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.CaseCode == null ? "Không có mã vụ việc " : gr.Key.CaseCode + " - " + gr.Key.CaseName
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.CaseCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "6")
            {
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.WarehouseCode,
                                    PartnerName = null,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.FProductWorkCode,
                                    p.FProductWorkName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur
                                } by new
                                {
                                    p.FProductWorkCode,
                                    p.FProductWorkName
                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.FProductWorkCode,
                                    GroupName = gr.Key.FProductWorkName,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.FProductWorkCode == null ? "Không có chứng từ sản phẩm " : gr.Key.FProductWorkCode + " - " + gr.Key.FProductWorkName
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.WarehouseCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "5")
            {
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.AccCode,
                                    PartnerName = null,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.AccCode,
                                    p.AccName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur
                                } by new
                                {
                                    p.DepartmentCode,
                                    p.DepartmentName
                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.DepartmentCode,
                                    GroupName = gr.Key.DepartmentName,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.DepartmentCode == null ? "Không có mã bộ phận " : gr.Key.DepartmentCode + " - " + gr.Key.DepartmentName
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.AccCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "4")
            {
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.DepartmentCode,
                                    PartnerName = null,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.DepartmentCode,
                                    p.DepartmentName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur
                                } by new
                                {
                                    p.SectionCode,
                                    p.SectionName
                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.SectionCode,
                                    GroupName = gr.Key.SectionName,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.SectionCode != null ? " Không có khoản mục " : gr.Key.SectionCode + " - " + gr.Key.SectionName
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.DepartmentCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "7")
            {
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.FProductWorkCode,
                                    PartnerName = p.PartnerName,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.AmountCur ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName,
                                    ProductName = p.ProductName
                                }).ToList();
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.FProductWorkCode,
                                    p.FProductWorkName,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur
                                } by new
                                {
                                    p.CurrencyCode,

                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.CurrencyCode,
                                    GroupName = null,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.CurrencyCode
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.FProductWorkCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData.AddRange(lst);
            }
            if (dto.Parameters.Sort == "8")

            {
                incurredData = (from p in incurredData

                                select new PurchaseWithTaxDto
                                {
                                    Tag = 0,
                                    Sort0 = 2,
                                    Bold = "K",
                                    Year = p.Year,
                                    SortGroupProduct = null,
                                    OrgCode = p.OrgCode,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    GroupCode = p.VoucherDate + p.VoucherNumber,
                                    PartnerName = null,
                                    PartnerCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    WarehouseCode = p.WarehouseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    SalesChannelCode = p.SalesChannelCode,
                                    ProductLotCode = p.ProductLotCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Note = p.Note,
                                    VoucherNumber = p.VoucherNumber,
                                    Description = p.Description,
                                    Quantity = p.Quantity ?? 0,
                                    Price = p.Price ?? 0,
                                    PriceCur0 = p.PriceCur0 ?? 0,
                                    Amount = p.Amount ?? 0,
                                    AmountCur = p.Amount ?? 0,
                                    VatAmount = p.VatAmount ?? 0,
                                    VatAmountCur = p.VatAmountCur ?? 0,
                                    AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                                    AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceNumber = p.InvoiceNumber,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                                    CaseName = p.CaseName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    CurrencyName = p.CurrencyName,
                                    AccName = p.AccName,
                                    Address = p.Address,
                                    Representative = p.Representative,
                                    WarehouseName = p.WarehouseName
                                }).ToList();
                var aktGroup = (from p in incurredData
                                group new
                                {
                                    p.VoucherDate,
                                    p.VoucherNumber,
                                    p.Quantity,
                                    p.Amount,
                                    p.AmountCur,
                                    p.VatAmount,
                                    p.VatAmountCur,
                                    p.AmountPay,
                                    p.AmountPayCur,
                                    p.Note,
                                    p.AccName
                                } by new
                                {
                                    p.AccCode

                                } into gr
                                select new PurchaseWithTaxDto
                                {
                                    GroupCode = gr.Key.AccCode,
                                    GroupName = gr.Max(p => p.Note),
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur),
                                    VatAmount = gr.Sum(p => p.VatAmount),
                                    VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                    AmountPay = gr.Sum(p => p.AmountPay),
                                    AmountPayCur = gr.Sum(p => p.AmountPayCur),
                                    Description = gr.Key.AccCode + " - " + gr.Max(p => p.AccName)
                                }).ToList();
                List<PurchaseWithTaxDto> lst = new List<PurchaseWithTaxDto>();
                for (int i = 0; i < aktGroup.Count; i++)
                {
                    PurchaseWithTaxDto purchaseWithTaxDto = new PurchaseWithTaxDto();
                    purchaseWithTaxDto.Tag = 0;
                    purchaseWithTaxDto.Bold = "C";
                    purchaseWithTaxDto.Sort0 = 1;
                    purchaseWithTaxDto.GroupCode = aktGroup[i].GroupCode;
                    purchaseWithTaxDto.VoucherDate = aktGroup[i].VoucherDate;
                    purchaseWithTaxDto.VoucherNumber = aktGroup[i].VoucherNumber;
                    purchaseWithTaxDto.Note = aktGroup[i].GroupCode + " " + aktGroup[i].GroupName;
                    purchaseWithTaxDto.Quantity = aktGroup[i].Quantity;
                    purchaseWithTaxDto.Amount = aktGroup[i].Amount;
                    purchaseWithTaxDto.AmountCur = aktGroup[i].AmountCur;
                    purchaseWithTaxDto.VatAmount = aktGroup[i].VatAmount;
                    purchaseWithTaxDto.VatAmountCur = aktGroup[i].VatAmountCur;
                    purchaseWithTaxDto.AmountPay = aktGroup[i].AmountPay;
                    purchaseWithTaxDto.AmountPayCur = aktGroup[i].AmountPayCur;
                    purchaseWithTaxDto.ReciprocalAcc = null;
                    purchaseWithTaxDto.Description = aktGroup[i].Description;
                    lst.Add(purchaseWithTaxDto);
                }
                incurredData.AddRange(lst);
            }
            List<PurchaseWithTaxDto> purchaseWithImportTaxDto = new List<PurchaseWithTaxDto>();
            PurchaseWithTaxDto crud = new PurchaseWithTaxDto();

            crud.Tag = 3;
            crud.Rank = 9;
            crud.Bold = "C";
            crud.GroupCode = "zzzzzz";
            crud.Description = "Tổng cộng";
            crud.Quantity = totalQuantity;
            crud.Amount = totalAmount;
            crud.AmountCur = totalAmountCur;
            crud.VatAmount = totalVatAmount;
            crud.VatAmountCur = totalVatAmountCur;
            crud.AmountPay = totalAmountPay;
            crud.AmountPayCur = totalAmountPayCur;
            crud.DevaluationAmount = totalDevaluationAmount;
            crud.DevaluationAmountCur = totalDevaluationAmountCur;
            crud.DiscountAmount = totalDiscountAmount;
            crud.DiscountAmountCur = totalDiscountAmountCur;
            crud.ExpenseAmountCur = toatlExpenseAmountCur;
            crud.ExpenseAmount = totalExpenseAmount;
            crud.AmountFunds = totalAmountFunds;
            crud.AmountFundsCur = totalAmountFundsCur;
            purchaseWithImportTaxDto.AddRange(incurredData);
            purchaseWithImportTaxDto.Add(crud);
            //  salesTransactionListMultiDtos.AddRange(incurredData);
            purchaseWithImportTaxDto = purchaseWithImportTaxDto.OrderBy(p => p.GroupCode).ThenBy(p => p.Ord0).ToList();
            var reportResponse = new ReportResponseDto<PurchaseWithTaxDto>();
            reportResponse.Data = purchaseWithImportTaxDto;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        [Authorize(ReportPermissions.PurchaseWithTaxListReportPrint)]
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
        private async Task<AccountBalanceDto> GetOpeningBalance(Dictionary<string, object> dic)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, yearCategory.Year);
            }
            dic[LedgerParameterConst.Year] = yearCategory.Year;

            var openingBalances = await _reportDataService.GetAccountBalancesAsync(dic);
            var balances = new AccountBalanceDto()
            {
                Debit = openingBalances.Sum(p => p.Debit),
                Credit = openingBalances.Sum(p => p.Credit),
                DebitCur = openingBalances.Sum(p => p.DebitCur),
                CreditCur = openingBalances.Sum(p => p.CreditCur)
            };

            return balances;
        }
        private async Task<List<PurchaseWithTaxDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);

            var incurredData = warehouseBook.Select(p => new PurchaseWithTaxDto()
            {
                Tag = 0,
                Sort0 = 2,
                Bold = "K",
                Year = p.Year,
                SortGroupProduct = null,
                OrgCode = p.OrgCode,
                VoucherId = p.VoucherId,
                Id = p.Id,
                Ord0 = p.Ord0,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                ProductCode = p.ProductCode,
                GroupCode = null,
                PartnerName = null,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                Unit = p.UnitCode,
                SectionCode = p.SectionCode,
                FProductWorkCode = p.FProductWorkCode,
                CaseCode = p.CaseCode,
                WarehouseCode = p.WarehouseCode,
                DepartmentCode = p.DepartmentCode,
                SalesChannelCode = p.SalesChannelCode,
                ProductLotCode = p.ProductLotCode,
                ProductOriginCode = p.ProductOriginCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                Note = p.Note,
                VoucherNumber = p.VoucherNumber,
                Description = p.Description,
                Quantity = p.ImportQuantity ?? 0,
                Price = p.Price0 ?? 0,
                PriceCur0 = p.PriceCur0 ?? 0,
                Amount = p.ImportAmount ?? 0,
                AmountCur = p.ImportAmountCur ?? 0,
                VatAmount = p.VatAmount ?? 0,
                VatAmountCur = p.VatAmountCur ?? 0,
                AmountPay = p.ImportAmount ?? 0 + p.VatAmount ?? 0,
                AmountPayCur = p.ImportAmountCur ?? p.VatAmountCur ?? 0,
                AccCode = p.ImportAcc,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                InvoiceDate = p.InvoiceDate,
                InvoiceNumber = p.InvoiceNumber,
                InvoiceSymbol = p.InvoiceSymbol,
                ReciprocalAcc = p.VoucherGroup == 1 ? p.DebitAcc : p.CreditAcc,
                CaseName = null,
                SectionName = null,
                DepartmentName = null,
                FProductWorkName = null,
                CurrencyName = null,
                AccName = null,
                Address = p.Address,
                Representative = p.Representative,
                VoucherGroup = p.VoucherGroup

            }).Where(p => p.VoucherGroup == 1).ToList();


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
            dic.Add(WarehouseBookParameterConst.DebitOrCredit, "*");

            dic.Add(WarehouseBookParameterConst.FromDate, dto.FromDate);
            dic.Add(WarehouseBookParameterConst.ToDate, dto.ToDate);
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(WarehouseBookParameterConst.CurrencyCode, dto.CurrencyCode);
            }

            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(WarehouseBookParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.ReciprocalAcc))
            {
                dic.Add(WarehouseBookParameterConst.ReciprocalAcc, dto.ReciprocalAcc);
            }
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }
            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                dic.Add(WarehouseBookParameterConst.DepartmentCode, dto.DepartmentCode);
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
            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(WarehouseBookParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                dic.Add(WarehouseBookParameterConst.BusinessCode, dto.BusinessCode);
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