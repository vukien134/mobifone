using System;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
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
using IdentityServer4.Extensions;
using Accounting.DomainServices.BusinessCategories;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Accounting.Caching;

namespace Accounting.Reports.ImportExports
{
    public class IssueTransactionListMultiAppService : AccountingAppService
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
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public IssueTransactionListMultiAppService(ReportDataService reportDataService,
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
            _workPlaceSevice = workPlaceSevice;
            _businessCategoryService = businessCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }

        #region Methods
        [Authorize(ReportPermissions.IssueTransactionListMultiReportView)]
        public async Task<ReportResponseDto<IssueTransactionListMultiDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            dto.Parameters.Type = "NH";
            var dic = GetWarehouseBookParameter(dto.Parameters);

            var incurredData = await GetIncurredData(dic);


            var products = await _productService.GetQueryableAsync();
            var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstVoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType = lstVoucherType.Where(p => p.Code == "PBH").FirstOrDefault();
            incurredData = incurredData.Where(p => string.IsNullOrEmpty(p.ProductCode.ToString()) == false).ToList();
            incurredData = (from p in incurredData
                            join c in lstProducts on p.ProductCode equals c.Code into m
                            from pr in m.DefaultIfEmpty()
                            join d in lstPartner on p.PartnerCode equals d.Code into f
                            from pa in f.DefaultIfEmpty()
                            select new IssueTransactionListMultiDto
                            {
                                Tag = 0,
                                Sort0 = 5,
                                Bold = "K",
                                OrgCode = p.OrgCode,
                                Voucher = null,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                VoucherCode = p.VoucherCode,
                                VoucherDate = p.VoucherDate,
                                VoucherNumber = p.VoucherNumber,
                                PartnerCode = p.PartnerCode,
                                ProductCode = p.ProductCode,
                                WarehouseCode = p.WarehouseCode,
                                UnitCode = p.UnitCode,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                Description = p.ProductCode,//+ " - " + pr != null ? pr.Name : null,
                                ImportQuantity = p.ImportQuantity,
                                Price = p.Price,
                                PriceCur = p.PriceCur,
                                ImportAmount = p.ImportAmount,
                                ImportAmountCur = p.ImportAmountCur,
                                DepartmentCode = p.DepartmentCode,
                                SectionCode = p.SectionCode,
                                CaseCode = p.CaseCode,
                                FProductWorkCode = p.FProductWorkCode,
                                WorkPlaceCode = p.WorkPlaceCode,
                                BusinessCode = p.BusinessCode,
                                TransWarehouseCode = p.TransWarehouseCode,
                                ExportQuantity = p.ExportQuantity,
                                ExportAmount = p.ExportAmount,
                                ExportAmountCur = p.ExportAmountCur,
                                QuantitySell = voucherType.ListVoucher.Contains(p.VoucherCode) == true ? p.Quantity : 0,
                                AmountSell = p.AmountSell,
                                Amount = p.Amount,
                                AmountCur = p.AmountCur,
                                DiscountAmount = p.DiscountAmount,
                                DiscountAmountCur = p.DiscountAmountCur,
                                VatAmount = p.VatAmount,
                                VatAmountCur = p.VatAmountCur,
                                ExportAcc = p.ExportAcc,
                                ImportAcc = p.ImportAcc,
                                AccCode = dto.Parameters.Type == "NH" ? p.ImportAcc : p.ExportAcc,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceSymbol = p.InvoiceSymbol,
                                InvoiceNumber = p.InvoiceNumber,
                                ReciprocalAcc = p.ReciprocalAcc,
                                GroupCode1 = p.PartnerCode,
                                GroupCode2 = p.PartnerCode,
                                GroupName1 = p.Description,
                                GroupName2 = p.Description,
                                DescriptionDetail = p.Description,
                                DescriptionHtml = p.Description,
                                Note = p.Note,
                                ProductName = pr.Name,
                                Quantity = p.Quantity,
                                PartnerName = pa != null ? pa.Name : null
                            }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var partners = await _accPartnerAppService.GetListByPartnerGroupCode(dto.Parameters.PartnerGroup);
                incurredData = (from p in incurredData
                                join b in partners on p.PartnerCode equals b.Code
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.PartnerCode,
                                    GroupCode2 = p.PartnerCode,
                                    GroupName1 = p.Description,
                                    GroupName2 = p.Description,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }).ToList();
            }

            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var lstProduct = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode);
                incurredData = (from p in incurredData
                                join b in lstProduct on new { p.OrgCode, Code = p.ProductCode } equals new { b.OrgCode, b.Code }
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.PartnerCode,
                                    GroupCode2 = p.PartnerCode,
                                    GroupName1 = p.Description,
                                    GroupName2 = p.Description,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = b.Name,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }).ToList();
            }
            var totalImportQuantity = incurredData.Select(p => p.ImportQuantity).Sum();
            var totalExportQuantity = incurredData.Select(p => p.ExportQuantity).Sum();
            var totalImportAmount = incurredData.Select(p => p.ImportAmount).Sum();
            var totalExportAmount = incurredData.Select(p => p.ExportAmount).Sum();
            var totalImportAmountCur = incurredData.Select(p => p.ImportAmountCur).Sum();
            var totalExportAmountCur = incurredData.Select(p => p.ExportAmountCur).Sum();
            var totalQuantitySell = incurredData.Select(p => p.QuantitySell).Sum();
            var totalAmountSell = incurredData.Select(p => p.AmountSell).Sum();
            var totalAmount = incurredData.Select(p => p.Amount).Sum();
            var totalAmountCur = incurredData.Select(p => p.AmountCur).Sum();
            var totalDiscountAmount = incurredData.Select(p => p.DiscountAmount).Sum();
            var totalDiscountAmountCur = incurredData.Select(p => p.DiscountAmountCur).Sum();
            var totalVatAmount = incurredData.Select(p => p.VatAmount).Sum();
            var totalVatAmountCur = incurredData.Select(p => p.VatAmountCur).Sum();
            var TurnoverAmount = totalAmount + totalDiscountAmount + totalVatAmount;
            var TurnoverAmountCur = totalAmountCur + totalDiscountAmountCur + totalVatAmountCur;


            var departmen = await _departmentService.GetQueryableAsync();
            var lstDepartmen = departmen.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var session = await _accSectionService.GetQueryableAsync();
            var lstSession = session.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var caseCode = await _accCaseService.GetQueryableAsync();
            var lstcaseCode = caseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.Parameters.Sort1 == 1)
            {
                incurredData = (from p in incurredData

                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.PartnerCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = string.IsNullOrEmpty(p.PartnerCode) ? "Không có  đối tượng" : p.PartnerName,
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.ProductCode,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 1)
            {
                incurredData = (from p in incurredData
                                join b in lstPartner on p.PartnerCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.PartnerCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = string.IsNullOrEmpty(p.PartnerCode) ? "Không có  đối tượng" : p.PartnerName,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort1 == 2)
            {
                incurredData = (from p in incurredData
                                join b in lstProducts on p.ProductCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.ProductCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : "Không có hàng hoá",
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.ProductCode + pn != null ? pn.Name : "Không có hàng hoá",
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 2)
            {
                incurredData = (from p in incurredData
                                join b in lstProducts on p.ProductCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.ProductCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : "Không có hàng hoá",
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.ProductCode + pn != null ? pn.Name : "Không có hàng hoá",
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort1 == 3)
            {
                incurredData = (from p in incurredData
                                join b in lstDepartmen on p.DepartmentCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.DepartmentCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : "Không có bộ phận",
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.DepartmentCode + pn != null ? pn.Name : "Không có bộ phận",
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 3)
            {
                incurredData = (from p in incurredData
                                join b in lstDepartmen on p.DepartmentCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                    //where !string.IsNullOrEmpty(p.DepartmentCode)
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.DepartmentCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : "Không có bộ phận",
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.DepartmentCode + pn != null ? pn.Name : "Không có bộ phận",
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }

            if (dto.Parameters.Sort1 == 4)
            {
                incurredData = (from p in incurredData
                                join b in lstSession on p.SectionCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.SectionCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : "Không có khoản mục",
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.SectionCode + pn != null ? pn.Name : "Không có khoản mục",
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 4)
            {
                incurredData = (from p in incurredData
                                join b in lstSession on p.SectionCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.SectionCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : "Không có khoản mục",
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.ProductCode + pn != null ? pn.Name : "Không có khoản mục",
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }

            if (dto.Parameters.Sort1 == 5)
            {
                incurredData = (from p in incurredData
                                join b in lstcaseCode on p.CaseCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.CaseCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : "Không có mã vụ việc",
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 5)
            {
                incurredData = (from p in incurredData
                                join b in lstcaseCode on p.CaseCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.CaseCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : "Không có mã vụ việc",
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            var businessCategory = await _businessCategoryService.GetQueryableAsync();
            var lstBusinessCategory = businessCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.Parameters.Sort1 == 6)
            {
                incurredData = (from p in incurredData
                                join b in lstBusinessCategory on p.BusinessCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.BusinessCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : null,
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 6)
            {
                incurredData = (from p in incurredData
                                join b in businessCategory on p.BusinessCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.BusinessCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : null,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstfProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            if (dto.Parameters.Sort1 == 7)
            {
                incurredData = (from p in incurredData
                                join b in lstfProductWork on p.FProductWorkCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.FProductWorkCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : null,
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 7)
            {
                incurredData = (from p in incurredData
                                join b in lstfProductWork on p.FProductWorkCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.FProductWorkCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : null,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            var workPlace = await _workPlaceSevice.GetQueryableAsync();
            var lstWorkPlace = workPlace.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            if (dto.Parameters.Sort1 == 8)
            {
                incurredData = (from p in incurredData
                                join b in lstWorkPlace on p.WorkPlaceCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.WorkPlaceCode,
                                    GroupCode2 = p.GroupCode2,
                                    GroupName1 = pn != null ? pn.Name : null,
                                    GroupName2 = p.GroupName2,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            if (dto.Parameters.Sort2 == 8)
            {
                incurredData = (from p in incurredData
                                join b in lstWorkPlace on p.WorkPlaceCode equals b.Code into c
                                from pn in c.DefaultIfEmpty()
                                select new IssueTransactionListMultiDto
                                {
                                    Tag = 0,
                                    Sort0 = 5,
                                    Bold = "K",
                                    OrgCode = p.OrgCode,
                                    Voucher = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    VoucherNumber = p.VoucherNumber,
                                    PartnerCode = p.PartnerCode,
                                    ProductCode = p.ProductCode,
                                    WarehouseCode = p.WarehouseCode,
                                    UnitCode = p.UnitCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    Description = p.Description,
                                    ImportQuantity = p.ImportQuantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    ImportAmount = p.ImportAmount,
                                    ImportAmountCur = p.ImportAmountCur,
                                    DepartmentCode = p.DepartmentCode,
                                    SectionCode = p.SectionCode,
                                    CaseCode = p.CaseCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    BusinessCode = p.BusinessCode,
                                    TransWarehouseCode = p.TransWarehouseCode,
                                    ExportQuantity = p.ExportQuantity,
                                    ExportAmount = p.ExportAmount,
                                    ExportAmountCur = p.ExportAmountCur,
                                    QuantitySell = p.QuantitySell,
                                    AmountSell = p.AmountSell,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    ExportAcc = p.ExportAcc,
                                    ImportAcc = p.ImportAcc,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    GroupCode1 = p.GroupCode1,
                                    GroupCode2 = p.WorkPlaceCode,
                                    GroupName1 = p.GroupName1,
                                    GroupName2 = pn != null ? pn.Name : null,
                                    DescriptionDetail = p.Description,
                                    DescriptionHtml = p.Description,
                                    Note = p.Note,
                                    ProductName = p.ProductName,
                                    Quantity = p.Quantity,
                                    PartnerName = p.PartnerName
                                }
                                ).ToList();
            }
            List<IssueTransactionListMultiDto> issueTransactionListMultiDtos = new List<IssueTransactionListMultiDto>();
            issueTransactionListMultiDtos.AddRange(incurredData);
            var sumAll = from p in issueTransactionListMultiDtos
                         where p.Bold == "K"
                         group new
                         {
                             p.Quantity,
                             p.ImportQuantity,
                             p.ImportAmount,
                             p.ExportQuantity,
                             p.ExportAmount,
                             p.ExportAmountCur,
                             p.AmountSell,
                             p.QuantitySell,
                             p.DiscountAmount,
                             p.DiscountAmountCur,
                             p.VatAmount,
                             p.VatAmountCur,
                             p.Note,
                             p.Amount,
                             p.PartnerCode,
                             p.PartnerName
                         } by new
                         {
                             p.GroupCode1,
                             p.GroupCode2,
                             p.GroupName1,
                             p.GroupName2
                         } into gr
                         select new IssueTransactionListMultiDto
                         {
                             Tag = 1,
                             Sort0 = 1,
                             Bold = "C",
                             GroupCode1 = gr.Key.GroupCode1,
                             GroupCode2 = gr.Key.GroupCode2,
                             GroupName1 = gr.Key.GroupName1,
                             Description = gr.Key.GroupName2,
                             ImportQuantity = gr.Sum(p => p.ImportQuantity),
                             ImportAmount = gr.Sum(p => p.ImportAmount),
                             ExportQuantity = gr.Sum(p => p.ExportQuantity),
                             ExportAmount = gr.Sum(p => p.ExportAmount),
                             ExportAmountCur = gr.Sum(p => p.ExportAmountCur),
                             AmountSell = gr.Sum(p => p.AmountSell),
                             QuantitySell = gr.Sum(p => p.QuantitySell),
                             DiscountAmount = gr.Sum(p => p.DiscountAmount),
                             DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                             VatAmount = gr.Sum(p => p.VatAmount),
                             VatAmountCur = gr.Sum(p => p.VatAmountCur),
                             Note = gr.Key.GroupName2,
                             Amount = gr.Sum(p => p.Amount),
                             PartnerCode = gr.Max(p => p.PartnerCode),
                             PartnerName = gr.Max(p => p.PartnerName)
                         };

            issueTransactionListMultiDtos.AddRange(sumAll);
            var test = sumAll.ToList();
            var sumAll1 = from p in issueTransactionListMultiDtos
                          where p.Bold == "C"
                          group new
                          {
                              p.Quantity,
                              p.ImportQuantity,
                              p.ImportAmount,
                              p.ExportQuantity,
                              p.ExportAmount,
                              p.ExportAmountCur,
                              p.AmountSell,
                              p.QuantitySell,
                              p.DiscountAmount,
                              p.DiscountAmountCur,
                              p.VatAmount,
                              p.VatAmountCur,
                              p.Note,
                              p.Amount
                          } by new
                          {
                              p.GroupCode1,
                              p.GroupName1,

                          } into gr
                          select new IssueTransactionListMultiDto
                          {
                              Tag = 1,
                              Sort0 = 1,
                              Bold = "C",
                              GroupCode1 = gr.Key.GroupCode1,
                              GroupCode2 = "",
                              Quantity = gr.Sum(p => p.Quantity),
                              Description = gr.Key.GroupName1,
                              ImportQuantity = gr.Sum(p => p.ImportQuantity),
                              ImportAmount = gr.Sum(p => p.ImportAmount),
                              ExportQuantity = gr.Sum(p => p.ExportQuantity),
                              ExportAmount = gr.Sum(p => p.ExportAmount),
                              ExportAmountCur = gr.Sum(p => p.ExportAmountCur),
                              AmountSell = gr.Sum(p => p.AmountSell),
                              QuantitySell = gr.Sum(p => p.QuantitySell),
                              DiscountAmount = gr.Sum(p => p.DiscountAmount),
                              DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                              VatAmount = gr.Sum(p => p.VatAmount),
                              VatAmountCur = gr.Sum(p => p.VatAmountCur),
                              Note = gr.Key.GroupName1,
                              Amount = gr.Sum(p => p.Amount)
                          };
            issueTransactionListMultiDtos.AddRange(sumAll1);

            IssueTransactionListMultiDto cru = new IssueTransactionListMultiDto();
            cru.Tag = 1;
            cru.Sort0 = 9;
            cru.Bold = "C";
            cru.GroupCode1 = "ZZZZZZZ";
            cru.GroupCode2 = "zzzzz";
            cru.Note = "Tổng cộng";
            cru.Quantity = incurredData.Select(p => p.Quantity).Sum();
            cru.ImportQuantity = totalImportQuantity;
            cru.ImportAmount = totalImportAmount;
            cru.ImportAmountCur = totalImportAmountCur;
            cru.ExportAmount = totalExportAmount;
            cru.ExportAmountCur = totalExportAmountCur;
            cru.ExportQuantity = totalExportQuantity;
            cru.AmountSell = totalAmountSell;
            cru.QuantitySell = totalQuantitySell;
            cru.DiscountAmount = totalDiscountAmount;
            cru.DiscountAmountCur = totalDiscountAmountCur;
            cru.VatAmount = totalVatAmount;
            cru.VatAmountCur = totalVatAmountCur;
            issueTransactionListMultiDtos.Add(cru);
            issueTransactionListMultiDtos = issueTransactionListMultiDtos.OrderBy(p => p.GroupCode1)
                                                                         .ThenBy(p => p.GroupCode2)
                                                                         .ThenBy(p => p.Sort0)
                                                                         .ThenBy(p => p.VoucherDate)
                                                                         .ThenBy(p => p.VoucherNumber)
                                                                         .ToList();
            var reportResponse = new ReportResponseDto<IssueTransactionListMultiDto>();
            reportResponse.Data = issueTransactionListMultiDtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion



        #region Private
        [Authorize(ReportPermissions.IssueTransactionListMultiReportPrint)]
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
        #region Private
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
        private async Task<List<IssueTransactionListMultiDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);

            var incurredData = warehouseBook.Select(p => new IssueTransactionListMultiDto()
            {
                Tag = 0,
                Sort0 = 5,
                Bold = "K",
                OrgCode = p.OrgCode,
                Voucher = null,
                VoucherId = p.VoucherId,
                Id = p.Id,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                VoucherNumber = p.VoucherNumber,
                PartnerCode = !string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode : p.PartnerCode0,
                ProductCode = p.ProductCode,
                WarehouseCode = p.WarehouseCode,
                UnitCode = p.UnitCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                Description = p.Description,
                ImportQuantity = p.ImportQuantity,
                Price = p.Price0,
                PriceCur = p.PriceCur0,
                ImportAmount = p.ImportAmount,
                ImportAmountCur = p.ImportAmountCur,
                DepartmentCode = p.DepartmentCode,
                SectionCode = p.SectionCode,
                CaseCode = p.CaseCode,
                FProductWorkCode = p.FProductWorkCode,
                WorkPlaceCode = p.WorkPlaceCode,
                BusinessCode = p.BusinessCode,
                TransWarehouseCode = p.TransWarehouseCode,
                ExportQuantity = p.ExportQuantity,
                ExportAmount = p.ExportAmount,
                ExportAmountCur = p.ExportAmountCur,
                QuantitySell = 0,
                AmountSell = p.Amount2,
                Amount = p.Amount,
                AmountCur = p.AmountCur,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                ExportAcc = p.ExportAcc,
                ImportAcc = p.ImportAcc,
                AccCode = null,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                ReciprocalAcc = p.VoucherCode == "1" ? p.DebitAcc : p.CreditAcc,
                GroupCode1 = p.PartnerCode,
                GroupCode2 = p.PartnerCode,
                GroupName1 = p.Description,
                GroupName2 = p.Description,
                DescriptionDetail = p.Description,
                DescriptionHtml = p.Description,
                Note = p.Note
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

