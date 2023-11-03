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
using Accounting.Reports.Others;
using Microsoft.AspNetCore.Hosting;
using Volo.Abp.ObjectMapping;
using System.Linq;
using Accounting.Categories.Products;
using Accounting.Vouchers;
using Org.BouncyCastle.Math;
using NPOI.SS.Formula.Functions;
using static NPOI.HSSF.Util.HSSFColor;
using Accounting.Report;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Accounting.Permissions;
using Accounting.Caching;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization;

namespace Accounting.Reports.ImportExports
{
    public class DetailedInventoryBookAppService : AccountingAppService
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
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public DetailedInventoryBookAppService(
                        ReportDataService reportDataService,
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
                        DefaultVoucherCategoryService defaultVoucherCategoryService,
                        AccountingCacheManager accountingCacheManager,
                        IStringLocalizer<AccountingResource> localizer
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
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _accountingCacheManager = accountingCacheManager;
            _localizer = localizer;
        }
        #region Methods
        public async Task<ReportResponseDto<DetailedInventoryBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
            var dic = GetWarehouseBookParameter(dto.Parameters);
            var incurredData = await GetIncurredData(dic);
            var debitCredit = 0;
            var productionPeriodCode = ""; // ma_sx
            if (string.IsNullOrEmpty(dto.Parameters.DebitCredit))
            {
                debitCredit = 2;
            }
            var round = "K";
            var nC = "";
            var ctSp = "";

            //var sterilizations = await _tenantSettingService.GetTenantSettingByKeyAsync("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            //var sterilization = sterilizations.Value;
            var Acc = await _accountSystemService.GetQueryableAsync();
            var lstAcc = Acc.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();

            var accSesion = await _accSectionService.GetQueryableAsync();
            var lstAccSesion = accSesion.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.AttachProductCost != "C").ToList();

            var importExportAll = await _reportDataService.GetIEInventoryAsync(dic);

            var lstWareHouseCode = await _warehouseService.GetQueryableAsync();
            var reusulWareHouse = "";
            if (!string.IsNullOrEmpty(dto.Parameters.WarehouseCode))
            {
                var wareHouses = lstWareHouseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.Parameters.WarehouseCode).FirstOrDefault();
                if (wareHouses != null)
                {
                    reusulWareHouse = dto.Parameters.WarehouseCode + " - " + wareHouses.Name;
                }

            }


            var products = await _productService.GetQueryableAsync();
            var lstProducts = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();


            var aImportExPort = from a in importExportAll
                                group new
                                {
                                    a.WarehouseCode,
                                    a.ProductCode,
                                    a.ProductLotCode,
                                    a.ProductOriginCode,
                                    a.OrgCode,
                                    a.AccCode,
                                    a.ImportQuantity1,
                                    a.ImportAmount1,
                                    a.ImportAmountCur1,
                                    a.ImportQuantity,
                                    a.ImportAmount,
                                    a.ImportAmountCur,
                                    a.ExportQuantity,
                                    a.ExportAmount,
                                    a.ExportAmountCur,
                                    a.ImportAmount2,
                                    a.ImportQuantity2,
                                    a.ImportAmountCur2,
                                    a.Amount2,
                                    a.AmountCur2
                                } by new
                                {
                                    a.WarehouseCode,
                                    a.ProductCode,
                                    a.ProductLotCode,
                                    a.ProductOriginCode
                                } into gr
                                select new
                                {
                                    WarehouseCode = gr.Max(p => p.WarehouseCode),
                                    ProductCode = gr.Max(p => p.ProductCode),
                                    ProductLotCode = gr.Max(p => p.ProductLotCode),
                                    ProductOriginCode = gr.Max(p => p.ProductOriginCode),
                                    OrgCode = gr.Max(p => p.OrgCode),
                                    AccCode = gr.Max(p => p.AccCode),
                                    ImportQuantity1 = gr.Sum(p => p.ImportQuantity1),
                                    ImportAmount1 = gr.Sum(p => p.ImportAmount1),
                                    ImportAmountCur1 = gr.Sum(p => p.ImportAmountCur1),
                                    ImportQuantity = gr.Sum(p => p.ImportQuantity),
                                    ImportAmount = gr.Sum(p => p.ImportAmount),
                                    ImportAmountCur = gr.Sum(p => p.ImportAmountCur),
                                    ExportQuantity = gr.Sum(p => p.ExportQuantity),
                                    ExportAmount = gr.Sum(p => p.ExportAmount),
                                    ExportAmountCur = gr.Sum(p => p.ExportAmountCur),
                                    ImportQuantity2 = gr.Sum(p => p.ImportQuantity2),
                                    ImportAmount2 = gr.Sum(p => p.ImportAmount2),
                                    ImportAmountCur2 = gr.Sum(p => p.ImportAmountCur2),
                                    Amount2 = gr.Sum(p => p.Amount2),
                                    AmountCur2 = gr.Sum(p => p.AmountCur2)

                                };

            incurredData = (from a in incurredData
                            join b in aImportExPort on new { Code = a.ProductCode, a.WarehouseCode } equals new { Code = b.ProductCode, b.WarehouseCode } into c
                            from d in c.DefaultIfEmpty()
                            select new DetailedInventoryBookDto
                            {
                                Sort = 0,
                                Sort0 = 1,
                                Bold = "K",
                                VoucherId = a.VoucherId,
                                Id = a.Id,
                                OrgCode = a.Ord0,
                                Year = a.Year,
                                VoucherCode = a.VoucherCode,
                                VoucherGroup = a.VoucherGroup,
                                VoucherNumber = a.VoucherNumber,
                                VoucherDate = a.VoucherDate,
                                ProductCode = a.ProductCode,
                                UnitCode = a.UnitCode,
                                Price = a.Price,
                                Note = a.Note,
                                Representative = a.Representative,
                                FProductWorkCode = a.FProductWorkCode,
                                ImportQuantity = a.ImportQuantity,
                                ExportQuantity = a.ExportQuantity,
                                ImportAmount = a.ImportAmount,
                                ExportAmount = a.ExportAmount,
                                ImportAmountCur = a.ImportAmountCur,
                                ExportAmountCur = a.ExportAmountCur,
                                ReciprocalAcc = a.ReciprocalAcc,//tk_du
                                OrdVoucher = null,
                                ImportAmountCur1 = d != null ? d.ImportAmountCur1 : 0,
                                ImportQuantity1 = d != null ? d.ImportQuantity1 : 0,
                                ImportAmount1 = d != null ? d.ImportAmount1 : 0,
                                WarehouseCode = a.WarehouseCode
                            }).ToList();
            if (!string.IsNullOrEmpty(dto.Parameters.ProductGroupCode))
            {
                var lstProduct = await _productAppService.GetListByProductGroupCode(dto.Parameters.ProductGroupCode);
                incurredData = (from a in incurredData
                                join b in lstProduct on new { a.OrgCode, Code = a.ProductCode } equals new { b.OrgCode, b.Code }
                                select new DetailedInventoryBookDto
                                {
                                    Sort = 0,
                                    Sort0 = 1,
                                    Bold = "K",
                                    VoucherId = a.VoucherId,
                                    Id = a.Id,
                                    OrgCode = a.Ord0,
                                    Year = a.Year,
                                    VoucherCode = a.VoucherCode,
                                    VoucherGroup = a.VoucherGroup,
                                    VoucherNumber = a.VoucherNumber,
                                    VoucherDate = a.VoucherDate,
                                    ProductCode = a.ProductCode,
                                    UnitCode = a.UnitCode,
                                    Price = a.Price,
                                    Note = a.Note,
                                    Representative = a.Representative,
                                    FProductWorkCode = a.FProductWorkCode,
                                    ImportQuantity = a.ImportQuantity,
                                    ExportQuantity = a.ExportQuantity,
                                    ImportAmount = a.ImportAmount,
                                    ExportAmount = a.ExportAmount,
                                    ImportAmountCur = a.ImportAmountCur,
                                    ExportAmountCur = a.ExportAmountCur,
                                    ReciprocalAcc = a.ReciprocalAcc,//tk_du
                                    OrdVoucher = null,
                                    ImportAmountCur1 = a.ImportAmountCur1,
                                    ImportQuantity1 = a.ImportQuantity1,
                                    ImportAmount1 = a.ImportAmount1,
                                    WarehouseCode = a.WarehouseCode
                                }).ToList();
            }
            var voucherCategoys = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstvoucherCategoys = voucherCategoys.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var defauVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();
            var lstDefaulVoucherCategory = defauVoucherCategory.ToList();
            if (lstvoucherCategoys.Count > 0)
            {
                incurredData = (from a in incurredData
                                join b in lstvoucherCategoys on a.VoucherCode equals b.Code
                                select new DetailedInventoryBookDto
                                {
                                    Sort = 0,
                                    Sort0 = 1,
                                    Bold = "K",
                                    VoucherId = a.VoucherId,
                                    Id = a.Id,
                                    OrgCode = a.Ord0,
                                    Year = a.Year,
                                    VoucherCode = a.VoucherCode,
                                    VoucherGroup = a.VoucherGroup,
                                    VoucherNumber = a.VoucherNumber,
                                    VoucherDate = a.VoucherDate,
                                    ProductCode = a.ProductCode,
                                    UnitCode = a.UnitCode,
                                    Price = a.Price,
                                    Note = a.Note,
                                    Representative = a.Representative,
                                    FProductWorkCode = a.FProductWorkCode,
                                    ImportQuantity = a.ImportQuantity,
                                    ExportQuantity = a.ExportQuantity,
                                    ImportAmount = a.ImportAmount,
                                    ExportAmount = a.ExportAmount,
                                    ImportAmountCur = a.ImportAmountCur,
                                    ExportAmountCur = a.ExportAmountCur,
                                    ReciprocalAcc = a.ReciprocalAcc,//tk_du
                                    OrdVoucher = b != null ? b.VoucherOrd : null,
                                    ImportAmountCur1 = a.ImportAmountCur1,
                                    ImportQuantity1 = a.ImportQuantity1,
                                    ImportAmount1 = a.ImportAmount1,
                                    WarehouseCode = a.WarehouseCode
                                }).OrderBy(p => p.VoucherId).ThenBy(p => p.Ord0).ToList();
            }
            else
            {
                incurredData = (from a in incurredData
                                join b in lstDefaulVoucherCategory on a.VoucherCode equals b.Code
                                select new DetailedInventoryBookDto
                                {
                                    Sort = 0,
                                    Sort0 = 1,
                                    Bold = "K",
                                    VoucherId = a.VoucherId,
                                    Id = a.Id,
                                    OrgCode = a.Ord0,
                                    Year = a.Year,
                                    VoucherCode = a.VoucherCode,
                                    VoucherGroup = a.VoucherGroup,
                                    VoucherNumber = a.VoucherNumber,
                                    VoucherDate = a.VoucherDate,
                                    ProductCode = a.ProductCode,
                                    UnitCode = a.UnitCode,
                                    Price = a.Price,
                                    Note = a.Note,
                                    Representative = a.Representative,
                                    FProductWorkCode = a.FProductWorkCode,
                                    ImportQuantity = a.ImportQuantity,
                                    ExportQuantity = a.ExportQuantity,
                                    ImportAmount = a.ImportAmount,
                                    ExportAmount = a.ExportAmount,
                                    ImportAmountCur = a.ImportAmountCur,
                                    ExportAmountCur = a.ExportAmountCur,
                                    ReciprocalAcc = a.ReciprocalAcc,//tk_du
                                    OrdVoucher = b != null ? b.VoucherOrd : null,
                                    ImportAmountCur1 = a.ImportAmountCur1,
                                    ImportQuantity1 = a.ImportQuantity1,
                                    ImportAmount1 = a.ImportAmount1,
                                    WarehouseCode = a.WarehouseCode
                                }).OrderBy(p => p.VoucherId).ThenBy(p => p.Ord0).ToList();
            }


            decimal inventoryQuantity = 0; // define a variable
            decimal idNature = -1;
            string product1 = null;
            string product2 = null;
            decimal inventoryAmount = 0;
            decimal inventoryAmountCur = 0;
            int sort0 = 0;
            var dataWareHouse = incurredData

                .OrderBy(q => q.ProductCode)
                .ThenBy(p => p.VoucherDate)
                .ThenBy(p => p.VoucherGroup)
                .ThenBy(p => p.OrdVoucher)
                .ThenBy(p => p.VoucherNumber)
                .ThenBy(p => p.VoucherId)
                .ThenBy(q => q.Ord0)
                .AsEnumerable()
                .Select(q =>
                {

                    if (product1 == null)
                    {
                        product1 = q.ProductCode;
                    }
                    else
                    {
                        product1 = q.ProductCode;
                        // product1 = product2;
                    }
                    if (product2 == product1)
                    {
                        product1 = null;
                    }


                    if (product2 == q.ProductCode)
                    {



                        inventoryQuantity = inventoryQuantity + (decimal)q.ImportQuantity - (decimal)q.ExportQuantity;
                        inventoryAmount = inventoryAmount + (decimal)q.ImportAmount - (decimal)q.ExportAmount;
                        inventoryAmountCur = inventoryAmountCur + (decimal)q.ImportAmountCur - (decimal)q.ExportAmountCur;
                        sort0 += 1;
                    }
                    else
                    {
                        inventoryQuantity = 0;
                        inventoryQuantity = (decimal)q.ImportQuantity - (decimal)q.ExportQuantity + (decimal)(q.ImportQuantity1 ?? 0);
                        inventoryAmount = (decimal)q.ImportAmount - (decimal)q.ExportAmount + (decimal)(q.ImportAmount1 ?? 0);
                        inventoryAmountCur = (decimal)q.ImportAmountCur - (decimal)q.ExportAmountCur + (decimal)(q.ImportAmountCur1 ?? 0);
                        product2 = q.ProductCode;
                    }

                    return new DetailedInventoryBookDto
                    {
                        Sort = 0,
                        Sort1 = 2,
                        Bold = "K",
                        OrgCode = q.OrgCode,
                        ProductCode = q.ProductCode,
                        ExportQuantity = q.ExportQuantity,
                        ImportQuantity = q.ImportQuantity,
                        InventoryAmount = (decimal)inventoryAmount,
                        InventoryAmountCur = (decimal)inventoryAmountCur,
                        InventoryQuantity = (decimal)inventoryQuantity,
                        VoucherNumber = q.VoucherNumber,
                        OrdVoucher = q.OrdVoucher,
                        VoucherDate = q.VoucherDate,
                        UnitCode = q.UnitCode,
                        Price = q.ImportAmount != 0 && q.ImportQuantity != 0 ? q.ImportAmount / q.ImportQuantity : (q.ExportAmount != 0 && q.ExportQuantity != 0 ? q.ExportAmount / q.ExportQuantity : q.Price0),
                        Note = q.Note,
                        Representative = q.Representative,
                        FProductWorkCode = q.FProductWorkCode,
                        ImportAmount = q.ImportAmount,
                        ImportAmountCur = q.ImportAmountCur,
                        ExportAmount = q.ExportAmount,
                        ExportAmountCur = q.ExportAmountCur,
                        ReciprocalAcc = q.ReciprocalAcc,
                        Grp1 = 2,
                        WarehouseCode = q.WarehouseCode,
                        Sort0 = sort0,
                        VoucherId = q.VoucherId,
                        VoucherCode = q.VoucherCode
                    };
                }).ToList();


            var resulsumAll = from a in dataWareHouse
                              group new
                              {
                                  a.ProductCode,
                                  a.Sort0,
                                  a.InventoryAmount,
                                  a.InventoryAmountCur,
                                  a.InventoryQuantity
                              } by new
                              {
                                  a.ProductCode,
                                  a.Sort0
                              } into gr
                              where gr.Max(p => p.Sort0) == sort0
                              select new
                              {

                                  ProductCode = gr.Key.ProductCode,
                                  Sort0 = gr.Max(p => p.Sort0),
                                  InventoryAmount = gr.Max(p => p.InventoryAmount),
                                  InventoryAmountCur = gr.Max(p => p.InventoryAmountCur),
                                  InventoryQuantity = gr.Max(p => p.InventoryQuantity)
                              };

            var listProduct = (from a in dataWareHouse
                               group new
                               {
                                   a.ProductCode,
                                   a.OrgCode
                               } by new
                               {
                                   a.OrgCode,
                                   a.ProductCode
                               }
                              into gr
                               select new
                               {
                                   ProductCode = gr.Key.ProductCode,
                                   OrgCode = gr.Key.OrgCode
                               }).ToList();
            var lstProduct2 = (from a in aImportExPort
                               group new
                               {
                                   a.ProductCode,
                                   a.OrgCode
                               } by new
                               {
                                   a.ProductCode,
                                   a.OrgCode
                               } into gr
                               select new
                               {
                                   ProductCode = gr.Key.ProductCode,
                                   OrgCode = gr.Key.OrgCode
                               }).ToList();
            listProduct.AddRange(lstProduct2);
            var sLstProduct = from a in listProduct
                              join b in lstProducts on new { a.OrgCode, Code = a.ProductCode } equals new { b.OrgCode, b.Code }
                              group new
                              {
                                  a.ProductCode,
                                  a.OrgCode,
                                  b.Name
                              } by new
                              {
                                  a.ProductCode,
                                  a.OrgCode
                              } into gr
                              select new
                              {
                                  ProductCode = gr.Key.ProductCode,
                                  OrgCode = gr.Key.OrgCode,
                                  ProductName = gr.Max(p => p.Name)
                              };


            var dataWareHouses = (from a in sLstProduct
                                  join b in aImportExPort on new
                                  {
                                      a.OrgCode,
                                      a.ProductCode
                                  } equals new
                                  {
                                      b.OrgCode,
                                      b.ProductCode
                                  } into c
                                  from d in c.DefaultIfEmpty()
                                  select new DetailedInventoryBookDto
                                  {
                                      Sort = 1,
                                      Sort1 = 2,
                                      Bold = "C",
                                      OrgCode = a.OrgCode,
                                      ProductCode = a.ProductCode,
                                      ExportQuantity = (decimal?)(0),
                                      ImportQuantity = (decimal?)(0),
                                      InventoryAmount = (decimal)(d != null ? d.ImportAmount1 : 0),
                                      InventoryAmountCur = (decimal)(d != null ? d.ImportAmountCur1 : 0),
                                      InventoryQuantity = (decimal)(d != null ? d.ImportQuantity1 : 0),
                                      VoucherNumber = "",
                                      OrdVoucher = "",
                                      VoucherDate = (DateTime?)null,
                                      UnitCode = "",
                                      Price = (decimal?)0,
                                      Note = a.ProductCode + " - " + a.ProductName,
                                      Representative = "",
                                      FProductWorkCode = "",
                                      ImportAmount = (decimal?)0,
                                      ImportAmountCur = (decimal?)0,
                                      ExportAmount = (decimal?)0,
                                      ExportAmountCur = (decimal?)0,
                                      ReciprocalAcc = "",
                                      Grp1 = 1,
                                      WarehouseCode = reusulWareHouse

                                  }).ToList();
            dataWareHouse.AddRange(dataWareHouses);
            var dataWareHouse3s = (from a in dataWareHouse
                                   join b in resulsumAll on new
                                   {

                                       a.ProductCode,

                                   } equals new
                                   {

                                       b.ProductCode
                                   } into c
                                   from d in c.DefaultIfEmpty()
                                   group new
                                   {
                                       a.ProductCode,
                                       a.ExportQuantity,
                                       a.ImportQuantity,
                                       InventoryAmount = d != null ? d.InventoryAmount : 0,
                                       InventoryQuantity = d != null ? d.InventoryQuantity : 0,
                                       InventoryAmountCur = d != null ? d.InventoryAmountCur : 0,
                                       a.ImportAmount,
                                       a.ImportAmountCur,
                                       a.ExportAmount,
                                       a.ExportAmountCur,

                                   } by new
                                   {
                                       a.ProductCode
                                   } into gr
                                   select new DetailedInventoryBookDto
                                   {
                                       Sort = 1,
                                       Sort1 = 2,
                                       Bold = "C",

                                       ProductCode = gr.Key.ProductCode,
                                       ExportQuantity = gr.Sum(p => p.ExportQuantity),
                                       ImportQuantity = gr.Sum(p => p.ImportQuantity),
                                       InventoryAmount = gr.Max(p => p.InventoryAmount),
                                       InventoryAmountCur = gr.Max(p => p.InventoryAmountCur),
                                       InventoryQuantity = gr.Max(p => p.InventoryQuantity),
                                       VoucherNumber = "",
                                       OrdVoucher = "",
                                       VoucherDate = (DateTime?)null,
                                       UnitCode = "",
                                       Price = (decimal?)0,
                                       Note = "Tổng cộng ",
                                       Representative = "",
                                       FProductWorkCode = "",
                                       ImportAmount = gr.Sum(p => p.ImportAmount),
                                       ImportAmountCur = gr.Sum(p => p.ImportAmountCur),
                                       ExportAmount = gr.Sum(p => p.ExportAmount),
                                       ExportAmountCur = gr.Sum(p => p.ExportAmountCur),
                                       ReciprocalAcc = "",
                                       Grp1 = 3,
                                       WarehouseCode = reusulWareHouse
                                   }).ToList();
            dataWareHouse.AddRange(dataWareHouse3s);

            var dataWareHouse4s = (from a in sLstProduct
                                   select new DetailedInventoryBookDto
                                   {
                                       Sort = 1,
                                       Sort1 = 2,
                                       Bold = "C",
                                       OrgCode = a.OrgCode,
                                       ProductCode = a.ProductCode,
                                       ExportQuantity = (decimal?)(0),
                                       ImportQuantity = (decimal?)(0),
                                       InventoryAmount = (decimal)(0),
                                       InventoryAmountCur = (decimal)(0),
                                       InventoryQuantity = (decimal)(0),
                                       VoucherNumber = "",
                                       OrdVoucher = "",
                                       VoucherDate = (DateTime?)null,
                                       UnitCode = "",
                                       Price = (decimal?)0,
                                       Note = "------------------------------",
                                       Representative = "",
                                       FProductWorkCode = "",
                                       ImportAmount = (decimal?)(0),
                                       ImportAmountCur = (decimal?)(0),
                                       ExportAmount = (decimal?)(0),
                                       ExportAmountCur = (decimal?)(0),
                                       ReciprocalAcc = "",
                                       Grp1 = 4,
                                       WarehouseCode = reusulWareHouse
                                   }).ToList();
            dataWareHouse.AddRange(dataWareHouse4s);
            var resul = dataWareHouse.OrderBy(p => p.ProductCode)
                                       .ThenBy(p => p.Grp1).
                                       ThenBy(p => p.VoucherDate)
                                       .ThenBy(p => p.OrdVoucher)
                                       .ThenBy(p => p.VoucherNumber);
            var reportResponse = new ReportResponseDto<DetailedInventoryBookDto>();
            reportResponse.Data = resul.ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
        
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
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
        private async Task<List<DetailedInventoryBookDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var warehouseBook = await GetWarehouseBook(dic);

            var incurredData = warehouseBook.GroupBy(g => new
            {
                g.Id,
                g.VoucherId,
                g.VoucherCode,
                g.VoucherNumber,
                g.ProductCode,
                g.UnitCode,
                g.Price0,
                g.Description,
                g.Representative,
                g.FProductWorkCode,
                g.VoucherGroup,
                g.DebitAcc,
                g.CreditAcc,
                g.WarehouseCode,
                g.Ord0

            }).Select(p => new DetailedInventoryBookDto()
            {
                Sort = 0,
                Sort0 = 1,
                Bold = "K",
                VoucherId = p.Max(p => p.VoucherId),
                WarehouseCode = p.Max(p => p.WarehouseCode),
                Id = p.Max(p => p.Id),
                OrgCode = p.Max(p => p.Ord0),
                Year = p.Max(p => p.Year),
                VoucherCode = p.Max(p => p.VoucherCode),
                VoucherGroup = p.Max(p => p.VoucherGroup),
                VoucherNumber = p.Max(p => p.VoucherNumber),
                VoucherDate = p.Max(p => p.VoucherDate),
                ProductCode = p.Max(p => p.ProductCode),
                UnitCode = p.Max(p => p.UnitCode),
                Price = p.Max(p => p.Price),
                Note = p.Max(p => p.Description),
                Representative = p.Max(p => p.Representative),
                FProductWorkCode = p.Max(p => p.FProductWorkCode),
                ImportQuantity = p.Sum(p => p.ImportQuantity),
                ExportQuantity = p.Sum(p => p.ExportQuantity),
                ImportAmount = p.Sum(p => p.ImportAmount),
                ExportAmount = p.Sum(p => p.ExportAmount),
                ImportAmountCur = p.Sum(p => p.ImportAmountCur),
                ExportAmountCur = p.Sum(p => p.ExportAmountCur),
                ReciprocalAcc = p.Max(p => p.VoucherCode == "1" ? p.DebitAcc : p.CreditAcc),//tk_du
                OrdVoucher = null,
                Ord0 = p.Key.Ord0


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
            if (!string.IsNullOrEmpty(dto.AccCode))
            {
                dic.Add(WarehouseBookParameterConst.Acc, dto.AccCode);
            }

            if (!string.IsNullOrEmpty(dto.WarehouseCode))
            {
                dic.Add(WarehouseBookParameterConst.WarehouseCode, dto.WarehouseCode);
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
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                dic.Add(WarehouseBookParameterConst.ProductOriginCode, dto.ProductOriginCode);
            }
            dic.Add(WarehouseBookParameterConst.Year, _webHelper.GetCurrentYear());
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
        protected async Task CheckPermission(string reportCode, string action)
        {
            bool isGrant = await this.IsGrantPermission(reportCode, action);
            if (!isGrant)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
        }
        private async Task<bool> IsGrantPermission(string reportCode, string action)
        {
            string permissionName = $"{reportCode}_{action}";
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
        #endregion
    }
}

