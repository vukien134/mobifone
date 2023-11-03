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
    public class DirectExportImportListAppService : AccountingAppService
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
        public readonly VoucherTypeService _voucherTypeService;
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion
        public DirectExportImportListAppService(ReportDataService reportDataService,
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
                        VoucherCategoryService voucherCategoryService,
                        VoucherTypeService voucherTypeService,
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
            _voucherCategoryService = voucherCategoryService;
            _voucherTypeService = voucherTypeService;
            _accountingCacheManager = accountingCacheManager;
        }

        #region Methods
        [Authorize(ReportPermissions.DirectImportExportListReportView)]
        public async Task<ReportResponseDto<DirectExportImportListDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstVoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType = lstVoucherType.Where(p => p.Code == "NXT").FirstOrDefault();
            if (dto.Parameters.VoucherCode == "" || string.IsNullOrEmpty(dto.Parameters.VoucherCode))
            {
                dto.Parameters.LstVoucherCode = voucherType.ListVoucher;
            }
            var dic = GetWarehouseBookParameter(dto.Parameters);
            var voucherCode2 = lstVoucherType.Where(p => p.Code == "000").FirstOrDefault();
            var incurredData = await GetIncurredData(dic);
            var partner = await _accPartnerService.GetQueryableAsync();
            var lstPartner = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productGroup = await _productGroupService.GetQueryableAsync();
            var lstProductGroup = productGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var caseCode = await _accCaseService.GetQueryableAsync();
            var lstCase = caseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var warehouse = await _warehouseService.GetQueryableAsync();
            var lstWarehouse = warehouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());

            var department = await _departmentService.GetQueryableAsync();
            var lstDepartment = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstfProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var sesion = await _accSectionService.GetQueryableAsync();
            var lstSession = sesion.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var wareHouse = await _warehouseService.GetQueryableAsync();
            var lstWareHouse = wareHouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


            var accSystem = await _accountSystemService.GetQueryableAsync();
            var lstAcc = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();



            incurredData = (from p in incurredData
                            join a in lstPartner on p.PartnerCode equals a.Code into b
                            from pa in b.DefaultIfEmpty()
                            join c in lstProduct on p.ProductCode equals c.Code into d
                            from pr in d.DefaultIfEmpty()
                            join r in lstCase on p.CaseCode equals r.Code into o
                            from ca in o.DefaultIfEmpty()
                            join u in lstDepartment on p.DepartmentCode equals u.Code into y
                            from de in y.DefaultIfEmpty()
                            join t in lstSession on p.SectionCode equals t.Code into e
                            from se in e.DefaultIfEmpty()
                            join n in lstfProductWork on p.FProductWorkCode equals n.Code into ft
                            from fp in ft.DefaultIfEmpty()
                            join i in lstWareHouse on p.WarehouseCode equals i.Code into pi
                            from wa in pi.DefaultIfEmpty()
                            join iu in lstAcc on p.AccCode equals iu.AccCode into po
                            from ac in po.DefaultIfEmpty()
                            select new DirectExportImportListDto
                            {
                                Tag = 0,
                                Bold = "K",
                                Sort = 2,
                                OrgCode = p.OrgCode,
                                SortProductGroup = pr != null ? pr.ProductGroup.Code : null,
                                VoucherId = p.VoucherId,
                                Id = p.Id,
                                Ord0 = p.Ord0,
                                VoucherCode = p.VoucherCode,
                                VoucherDate = p.VoucherDate,
                                ProductCode = p.ProductCode,
                                ProductName = pr != null ? pr.Name : null,
                                PartnerCode = p.PartnerCode,
                                GroupCode = null,
                                Unit = p.Unit,
                                SectionCode = p.SectionCode,
                                FProductWorkCode = p.FProductWorkCode,
                                CaseCode = p.CaseCode,
                                DepartmentCode = p.DepartmentCode,
                                WarehouseCode = p.WarehouseCode,
                                ProductOriginCode = p.ProductOriginCode,
                                WorkPlaceCode = p.WorkPlaceCode,
                                CurrencyCode = p.CurrencyCode,
                                ExchangeRate = p.ExchangeRate,
                                VoucherNumber = p.VoucherNumber,
                                VoucherNumber1 = p.VoucherNumber,
                                Note = p.Note,
                                Description = p.Description,
                                DescriptionE = p.DescriptionE,
                                Quantity = p.Quantity,
                                Price = p.Price,
                                PriceCur = p.PriceCur,
                                Amount = p.Amount,
                                AmountCur = p.AmountCur,
                                AccCode = p.AccCode,
                                DebitAcc = p.DebitAcc,
                                CreditAcc = p.CreditAcc,
                                InvoiceDate = p.InvoiceDate,
                                InvoiceSymbol = p.InvoiceSymbol,
                                InvoiceNumber = p.InvoiceNumber,
                                ReciprocalAcc = p.ReciprocalAcc,
                                Representative = p.Representative,
                                VatAmount = p.VatAmount,
                                VatAmountCur = p.VatAmountCur,
                                CaseName = ca != null ? ca.Name : null,
                                PartnerName = pa != null ? pa.Name : null,
                                SectionName = se != null ? se.Name : null,
                                DepartmentName = de != null ? de.Name : null,
                                FProductWorkName = fp != null ? fp.Name : null,
                                CurrencyName = null,
                                AccName = ac != null ? ac.AccName : null,
                                VoucherGroup = p.VoucherGroup,
                                VatPercentage = p.VatPercentage,
                                Address = p.Address,
                                WarehouseName = wa != null ? wa.Name : null,
                                DiscountAmount = p.DiscountAmount,
                                DiscountAmountCur = p.DiscountAmountCur,
                                DiscountPercentage = p.DiscountPercentage
                            }).ToList();

            if (!string.IsNullOrEmpty(dto.Parameters.BeginVoucherNumber) && !string.IsNullOrEmpty(dto.Parameters.EndVoucherNumber))
            {
                incurredData = (from p in incurredData
                                where GetVoucherNumber(p.VoucherNumber) >= GetVoucherNumber(dto.Parameters.BeginVoucherNumber)
                                 && GetVoucherNumber(dto.Parameters.EndVoucherNumber) <= GetVoucherNumber(p.VoucherNumber)
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = null,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Note,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
            }
            if (!string.IsNullOrEmpty(dto.Parameters.PartnerGroup))
            {
                var parnerGroup = await _partnerGroupService.GetQueryableAsync();
                var lstparnerGroup = parnerGroup.Where(p => p.Id == dto.Parameters.PartnerGroup).ToList();
                if (lstparnerGroup.Count > 0)
                {
                    var code = lstparnerGroup.FirstOrDefault().Code;
                    var partners = await _accPartnerAppService.GetListByPartnerGroupCode(code);
                    incurredData = (from p in incurredData
                                    join b in partners on p.PartnerCode equals b.Code
                                    select new DirectExportImportListDto
                                    {
                                        Tag = 0,
                                        Bold = "K",
                                        Sort = 2,
                                        OrgCode = p.OrgCode,
                                        SortProductGroup = null,
                                        VoucherId = p.VoucherId,
                                        Id = p.Id,
                                        Ord0 = p.Ord0,
                                        VoucherCode = p.VoucherCode,
                                        VoucherDate = p.VoucherDate,
                                        ProductCode = p.ProductCode,
                                        ProductName = p.ProductName,
                                        PartnerCode = p.PartnerCode,
                                        GroupCode = null,
                                        Unit = p.Unit,
                                        SectionCode = p.SectionCode,
                                        FProductWorkCode = p.FProductWorkCode,
                                        CaseCode = p.CaseCode,
                                        DepartmentCode = p.DepartmentCode,
                                        WarehouseCode = p.WarehouseCode,
                                        ProductOriginCode = p.ProductOriginCode,
                                        WorkPlaceCode = p.WorkPlaceCode,
                                        CurrencyCode = p.CurrencyCode,
                                        ExchangeRate = p.ExchangeRate,
                                        VoucherNumber = p.VoucherNumber,
                                        VoucherNumber1 = p.VoucherNumber,
                                        Note = p.Note,
                                        Description = p.Description,
                                        DescriptionE = p.DescriptionE,
                                        Quantity = p.Quantity,
                                        Price = p.Price,
                                        PriceCur = p.PriceCur,
                                        Amount = p.Amount,
                                        AmountCur = p.AmountCur,
                                        AccCode = p.AccCode,
                                        DebitAcc = p.DebitAcc,
                                        CreditAcc = p.CreditAcc,
                                        InvoiceDate = p.InvoiceDate,
                                        InvoiceSymbol = p.InvoiceSymbol,
                                        InvoiceNumber = p.InvoiceNumber,
                                        ReciprocalAcc = p.ReciprocalAcc,
                                        Representative = p.Representative,
                                        VatAmount = p.VatAmount,
                                        VatAmountCur = p.VatAmountCur,
                                        CaseName = p.CaseName,
                                        PartnerName = p.PartnerName,
                                        SectionName = p.SectionName,
                                        DepartmentName = p.DepartmentName,
                                        FProductWorkName = p.FProductWorkName,
                                        AccName = p.AccName,
                                        VoucherGroup = p.VoucherGroup,
                                        VatPercentage = p.VatPercentage,
                                        Address = p.Address,
                                        WarehouseName = p.WarehouseName,
                                        DiscountAmount = p.DiscountAmount,
                                        DiscountAmountCur = p.DiscountAmountCur,
                                        DiscountPercentage = p.DiscountPercentage
                                    }).ToList();
                }

            }
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var products = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode);
                incurredData = (from p in incurredData
                                join b in products on p.ProductCode equals b.Code
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = null,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Note,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
            }



            var totalQuantity = incurredData.Select(p => p.Quantity).Sum();
            var totalAmount = incurredData.Select(p => p.Amount).Sum();
            var totalAmountCur = incurredData.Select(p => p.AmountCur).Sum();
            List<DirectExportImportListDto> directExportImportListDtos = new List<DirectExportImportListDto>();

            if (dto.Parameters.Sort == "2")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.ProductCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.CreditAcc : p.DebitAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.ProductCode,
                                    a.ProductName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.ProductCode,
                                    a.ProductName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    ProductCode = gr.Key.ProductCode,
                                    ProductName = gr.Key.ProductName,
                                    Note = gr.Key.ProductCode + " - " + gr.Key.ProductName,
                                    GroupCode = gr.Key.ProductCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "3")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.PartnerCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.CreditAcc : p.DebitAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.PartnerCode,
                                    a.PartnerName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.PartnerCode,
                                    a.PartnerName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    PartnerCode = gr.Key.PartnerCode,
                                    PartnerName = gr.Key.PartnerName,
                                    Note = gr.Key.PartnerCode + " - " + gr.Key.PartnerName,
                                    GroupCode = gr.Key.PartnerCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "4")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.CaseCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                incurredData = (from p in incurredData
                                where p.VoucherDate != null && p.VoucherNumber != null
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.CaseCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.ProductCode + " - " + p.ProductName,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.CaseCode,
                                    a.CaseName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.CaseCode,
                                    a.CaseName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    CaseCode = gr.Key.CaseCode,
                                    CaseName = gr.Key.CaseName,
                                    Note = gr.Key.CaseCode + " - " + gr.Key.CaseName,
                                    GroupCode = gr.Key.CaseCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);

            }
            if (dto.Parameters.Sort == "5")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.WarehouseCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.VoucherGroup == 1 ? p.CreditAcc : p.DebitAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.WarehouseCode,
                                    a.WarehouseName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.WarehouseCode,
                                    a.WarehouseName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    WarehouseCode = gr.Key.WarehouseCode,
                                    WarehouseName = gr.Key.WarehouseName,
                                    Note = gr.Key.WarehouseCode + " - " + gr.Key.WarehouseName,
                                    GroupCode = gr.Key.WarehouseCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "6")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.AccCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.AccCode,
                                    a.AccName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.AccCode,
                                    a.AccName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    AccCode = gr.Key.AccCode,
                                    AccName = gr.Key.AccName,
                                    Note = gr.Key.AccCode + " - " + gr.Key.AccName,
                                    GroupCode = gr.Key.AccCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "7")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.DepartmentCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.DepartmentCode,
                                    a.DepartmentName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.DepartmentCode,
                                    a.DepartmentName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    DepartmentCode = gr.Key.DepartmentCode,
                                    DepartmentName = gr.Key.DepartmentName,
                                    Note = gr.Key.DepartmentCode + " - " + gr.Key.DepartmentName,
                                    GroupCode = gr.Key.DepartmentCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "8")
            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = null,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.FProductWorkCode,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Description,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.FProductWorkCode,
                                    a.FProductWorkName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.FProductWorkCode,
                                    a.FProductWorkName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    FProductWorkCode = gr.Key.FProductWorkCode,
                                    FProductWorkName = gr.Key.FProductWorkName,
                                    Note = gr.Key.FProductWorkCode + " - " + gr.Key.FProductWorkName,
                                    GroupCode = gr.Key.FProductWorkCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "9")
            {
                incurredData = (from p in incurredData
                                join l in lstProductGroup on p.SortProductGroup equals l.Code into g
                                from pg in g.DefaultIfEmpty()
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = p.SortProductGroup,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.SortProductGroup,
                                    GroupName = p.GroupName,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.Note,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.GroupCode,
                                    a.GroupName,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity
                                } by new
                                {
                                    a.GroupCode,
                                    a.GroupName
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,

                                    GroupName = gr.Key.GroupName,
                                    Note = gr.Key.GroupCode + " - " + gr.Key.GroupName,
                                    GroupCode = gr.Key.GroupCode,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            if (dto.Parameters.Sort == "1")

            {
                incurredData = (from p in incurredData

                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "K",
                                    Sort = 2,
                                    OrgCode = p.OrgCode,
                                    SortProductGroup = p.SortProductGroup,
                                    VoucherId = p.VoucherId,
                                    Id = p.Id,
                                    Ord0 = p.Ord0,
                                    VoucherCode = p.VoucherCode,
                                    VoucherDate = p.VoucherDate,
                                    ProductCode = p.ProductCode,
                                    ProductName = p.ProductName,
                                    PartnerCode = p.PartnerCode,
                                    GroupCode = p.VoucherDate + p.VoucherNumber,
                                    GroupName = p.GroupName,
                                    Unit = p.Unit,
                                    SectionCode = p.SectionCode,
                                    FProductWorkCode = p.FProductWorkCode,
                                    CaseCode = p.CaseCode,
                                    DepartmentCode = p.DepartmentCode,
                                    WarehouseCode = p.WarehouseCode,
                                    ProductOriginCode = p.ProductOriginCode,
                                    WorkPlaceCode = p.WorkPlaceCode,
                                    CurrencyCode = p.CurrencyCode,
                                    ExchangeRate = p.ExchangeRate,
                                    VoucherNumber = p.VoucherNumber,
                                    VoucherNumber1 = p.VoucherNumber,
                                    Note = p.ProductCode + " - " + p.ProductName,
                                    Description = p.Description,
                                    DescriptionE = p.DescriptionE,
                                    Quantity = p.Quantity,
                                    Price = p.Price,
                                    PriceCur = p.PriceCur,
                                    Amount = p.Amount,
                                    AmountCur = p.AmountCur,
                                    AccCode = p.AccCode,
                                    DebitAcc = p.DebitAcc,
                                    CreditAcc = p.CreditAcc,
                                    InvoiceDate = p.InvoiceDate,
                                    InvoiceSymbol = p.InvoiceSymbol,
                                    InvoiceNumber = p.InvoiceNumber,
                                    ReciprocalAcc = p.ReciprocalAcc,
                                    Representative = p.Representative,
                                    VatAmount = p.VatAmount,
                                    VatAmountCur = p.VatAmountCur,
                                    CaseName = p.CaseName,
                                    PartnerName = p.PartnerName,
                                    SectionName = p.SectionName,
                                    DepartmentName = p.DepartmentName,
                                    FProductWorkName = p.FProductWorkName,
                                    AccName = p.AccName,
                                    VoucherGroup = p.VoucherGroup,
                                    VatPercentage = p.VatPercentage,
                                    Address = p.Address,
                                    WarehouseName = p.WarehouseName,
                                    DiscountAmount = p.DiscountAmount,
                                    DiscountAmountCur = p.DiscountAmountCur,
                                    DiscountPercentage = p.DiscountPercentage
                                }).ToList();
                directExportImportListDtos.AddRange(incurredData);
                var aktGroup = (from a in incurredData
                                group new
                                {
                                    a.VoucherDate,
                                    a.VoucherNumber,
                                    a.Amount,
                                    a.AmountCur,
                                    a.Quantity,
                                    a.Note
                                } by new
                                {
                                    a.VoucherDate,
                                    a.VoucherNumber
                                } into gr
                                select new DirectExportImportListDto
                                {
                                    Tag = 0,
                                    Bold = "C",
                                    Sort = 1,
                                    Note = gr.Max(p => p.Note),
                                    GroupCode = gr.Key.VoucherDate + gr.Key.VoucherNumber,
                                    Quantity = gr.Sum(p => p.Quantity),
                                    Amount = gr.Sum(p => p.Amount),
                                    AmountCur = gr.Sum(p => p.AmountCur)
                                }).ToList();
                directExportImportListDtos.AddRange(aktGroup);
            }
            DirectExportImportListDto directExportImportListDto = new DirectExportImportListDto();
            directExportImportListDto.Tag = 3;
            directExportImportListDto.Bold = "C";
            directExportImportListDto.Sort = 3;
            directExportImportListDto.GroupCode = "zzzzzzz";
            directExportImportListDto.Quantity = totalQuantity;
            directExportImportListDto.Amount = totalAmount;
            directExportImportListDto.AmountCur = totalAmountCur;
            directExportImportListDto.Note = "Tổng cộng";
            directExportImportListDtos.Add(directExportImportListDto);

            var resul = directExportImportListDtos.OrderBy(p => p.GroupCode)
                                    .ThenBy(p => p.Sort)
                                    .ThenBy(p => p.Bold)
                                    .ThenBy(p => p.VoucherDate)
                                    .ThenBy(p => p.VoucherNumber)
                                    .ToList();

            var reportResponse = new ReportResponseDto<DirectExportImportListDto>();
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
        [Authorize(ReportPermissions.DirectImportExportListReportPrint)]
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
        private async Task<List<DirectExportImportListDto>> GetIncurredData(Dictionary<string, object> dic)
        {

            var warehouseBook = await GetWarehouseBook(dic);
            var incurredData = warehouseBook.Where(p => p.VoucherGroup != 2).Select(p => new DirectExportImportListDto()
            {
                Tag = 0,
                Bold = "K",
                Sort = 2,
                OrgCode = p.OrgCode,
                SortProductGroup = null,
                VoucherId = p.VoucherId,
                Id = p.Id,
                Ord0 = p.Ord0,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                ProductCode = p.ProductCode,
                ProductName = null,
                PartnerCode = p.PartnerCode == "" || string.IsNullOrEmpty(p.PartnerCode) ? p.PartnerCode0 : p.PartnerCode,
                GroupCode = null,
                Unit = p.UnitCode,
                SectionCode = p.SectionCode,
                FProductWorkCode = p.FProductWorkCode,
                CaseCode = p.CaseCode,
                DepartmentCode = p.DepartmentCode,
                WarehouseCode = p.WarehouseCode,
                ProductOriginCode = p.ProductOriginCode,
                WorkPlaceCode = p.WorkPlaceCode,
                CurrencyCode = p.CurrencyCode,
                ExchangeRate = p.ExchangeRate,
                VoucherNumber = p.VoucherNumber,
                VoucherNumber1 = p.VoucherNumber,
                Note = p.Note,
                Description = p.Description,
                DescriptionE = p.DescriptionE,
                Quantity = p.Quantity,
                Price = p.VoucherGroup > 2 ? p.Price : p.Price0,
                PriceCur = p.VoucherGroup > 2 ? p.PriceCur : p.PriceCur0,
                Amount = p.VoucherGroup > 2 ? p.Amount : p.ImportAmount,
                AmountCur = p.VoucherGroup > 2 ? p.AmountCur : p.ImportAmountCur,
                AccCode = p.ImportAcc,
                DebitAcc = p.DebitAcc,
                CreditAcc = p.CreditAcc,
                InvoiceDate = p.InvoiceDate,
                InvoiceSymbol = p.InvoiceSymbol,
                InvoiceNumber = p.InvoiceNumber,
                ReciprocalAcc = p.VoucherGroup == 1 ? p.CreditAcc : p.DebitAcc,
                Representative = p.Representative,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                CaseName = null,
                PartnerName = null,
                SectionName = null,
                DepartmentName = null,
                FProductWorkName = null,
                CurrencyName = null,
                AccName = null,
                VoucherGroup = p.VoucherGroup,
                VatPercentage = p.VatPercentage,
                Address = p.Address,
                WarehouseName = null,
                DiscountPercentage = p.DiscountPercentage,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur
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

            if (!string.IsNullOrEmpty(dto.LstVoucherCode))
            {
                dic.Add(WarehouseBookParameterConst.LstVoucherCode, dto.LstVoucherCode);
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

