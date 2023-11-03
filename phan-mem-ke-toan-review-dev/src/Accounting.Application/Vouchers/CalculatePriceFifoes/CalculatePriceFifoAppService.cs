using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Categories.Products;
using Accounting.Categories.ProductVouchers;
using Accounting.Catgories.Others.CostOfGoods;
using Accounting.Catgories.Products;
using Accounting.Catgories.ProductVouchers;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.VoucherExciseTaxs;
using NPOI.SS.Formula.Functions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Uow;

namespace Accounting.Vouchers.CalculatePriceFifoes
{
    public class CalculatePriceFifoAppService : AccountingAppService, ICalculatePriceFifoAppService, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccPartnerService _accPartnerService;
        private readonly DepartmentService _departmentService;
        private readonly CurrencyService _currencyService;
        private readonly LedgerService _ledgerService;
        private readonly ProductService _productService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly ProductVoucherAssemblyService _productVoucherAssemblyService;
        private readonly ProductVoucherReceiptService _productVoucherReceiptService;
        private readonly ProductVoucherVatService _productVoucherVatService;
        private readonly ProductVoucherCostService _productVoucherCostService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly ProductVoucherAppService _productVoucherAppService;
        private readonly ProductGroupService _productGroupService;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        #endregion
        #region Ctor
        public CalculatePriceFifoAppService(TenantSettingService tenantSettingService,
                                VoucherCategoryService voucherCategoryService,
                                AccountSystemService accountSystemService,
                                BusinessCategoryService businessCategoryService,
                                AccPartnerService accPartnerService,
                                DepartmentService departmentService,
                                CurrencyService currencyService,
                                LedgerService ledgerService,
                                TaxCategoryService taxCategoryService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                ProductService productService,
                                ProductVoucherService productVoucherService,
                                ProductVoucherDetailService productVoucherDetailService,
                                AccTaxDetailService accTaxDetailService,
                                ProductVoucherAssemblyService productVoucherAssemblyService,
                                ProductVoucherReceiptService productVoucherReceiptService,
                                ProductVoucherVatService productVoucherVatService,
                                ProductVoucherCostService productVoucherCostService,
                                VoucherExciseTaxService voucherExciseTaxService,
                                ProductVoucherAppService productVoucherAppService,
                                ProductGroupService productGroupService,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                WarehouseBookService warehouseBookService,
                                DefaultVoucherCategoryService defaultVoucherCategoryService
                            )
        {
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _accountSystemService = accountSystemService;
            _businessCategoryService = businessCategoryService;
            _accPartnerService = accPartnerService;
            _departmentService = departmentService;
            _currencyService = currencyService;
            _ledgerService = ledgerService;
            _taxCategoryService = taxCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _productService = productService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _productVoucherAssemblyService = productVoucherAssemblyService;
            _productVoucherReceiptService = productVoucherReceiptService;
            _productVoucherVatService = productVoucherVatService;
            _productVoucherCostService = productVoucherCostService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _productVoucherAppService = productVoucherAppService;
            _productGroupService = productGroupService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _warehouseBookService = warehouseBookService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
        }
        #endregion

        public async Task<ResultDto> CalculatePriceFifo(CostOfGoodsDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var result = new ResultDto();
            var warehouseBooks = await _warehouseBookService.GetQueryableAsync();
            warehouseBooks = warehouseBooks.Where(p => p.OrgCode == dto.OrdCode
                                      && p.Year == dto.Year
                                      );
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.ProductOriginCode == dto.ProductOriginCode);
            }
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.WarehouseCode == dto.WareHouseCode);
            }

            var voucherCategories = await _voucherCategoryService.GetQueryableAsync();
            voucherCategories = voucherCategories.Where(p => p.OrgCode == dto.OrdCode);
            var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();

            if (voucherCategories.ToList().Count() == 0)
            {
                voucherCategories = from a in defaultVoucherCategory
                                    select new VoucherCategory
                                    {
                                        Code = a.Code,
                                        Name = a.Name,
                                        NameE = a.NameE,
                                        VoucherGroup = a.VoucherGroup,
                                        VoucherType = a.VoucherType,
                                        VoucherOrd = a.VoucherOrd,
                                        CurrencyCode = a.CurrencyCode,
                                        AttachBusiness = a.AttachBusiness,
                                        IncreaseNumberMethod = a.IncreaseNumberMethod,
                                        ProductType = a.ProductType,
                                        ChkDuplicateVoucherNumber = a.ChkDuplicateVoucherNumber,
                                        IsTransfer = a.IsTransfer,
                                        IsAssembly = a.IsAssembly,
                                        PriceCalculatingMethod = a.PriceCalculatingMethod,
                                        IsSavingLedger = a.IsSavingLedger,
                                        IsSavingWarehouseBook = a.IsSavingWarehouseBook,
                                        IsCalculateBalanceAcc = a.IsCalculateBalanceAcc,
                                        IsCalculateBalanceProduct = a.IsCalculateBalanceProduct,
                                        Prefix = a.Prefix,
                                        SeparatorCharacter = a.SeparatorCharacter,
                                        Suffix = a.Suffix,
                                        BookClosingDate = a.BookClosingDate,
                                        BusinessBeginningDate = a.BusinessBeginningDate,
                                        BusinessEndingDate = a.BusinessEndingDate,
                                        TaxType = a.TaxType,
                                        VoucherKind = a.VoucherKind,
                                        AttachPartnerPrice = a.AttachPartnerPrice,
                                        TenantType = a.TenantType
                                    };
            }
            var products = await _productService.GetQueryableAsync();
            products = products.Where(p => p.OrgCode == dto.OrdCode
                                        && p.ProductType == dto.ProductType || dto.ProductType == null || dto.ProductType == "*" || dto.ProductType == "0"
                                        && p.ProductType != "D"

                                        && (dto.ProductionPeriodCode == null || p.ProductionPeriodCode == dto.ProductionPeriodCode));
            var test = products.ToList();
            if (!string.IsNullOrEmpty(dto.ProductType))
            {
                products = products.Where(p => p.ProductType == dto.ProductType);
            }
            if (!string.IsNullOrEmpty(dto.ProductionPeriodCode))
            {
                products = products.Where(p => p.ProductionPeriodCode == dto.ProductionPeriodCode);
            }
            var productGroups = await _productGroupService.GetQueryableAsync();
            productGroups = productGroups.Where(p => p.OrgCode == dto.OrdCode);
            var productOpeningBalance = await _productOpeningBalanceService.GetQueryableAsync();
            productOpeningBalance = productOpeningBalance.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year);
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.ProductCode == dto.ProductCode);
                products = products.Where(p => p.Code == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.ProductOriginCode == dto.ProductOriginCode);
            }
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.WarehouseCode == dto.WareHouseCode);
            }
            if (dto.ProductGroup != null && dto.ProductGroup != "")
            {
                productGroups = productGroups.Where(p => p.OrgCode == dto.OrdCode);
                var dataProductGroup = productGroups.Where(p => p.Code == dto.ProductGroup.ToString()).Select(p => ObjectMapper.Map<ProductGroup, ProductGroupDto>(p)).ToList();
                var dataProductGroupNew = GetProductGroupCodeChild(dataProductGroup).Result;
                while (dataProductGroupNew.Count() > 0)
                {
                    foreach (var item in dataProductGroupNew)
                    {
                        dataProductGroup.Add(item);
                    }
                    dataProductGroupNew = GetProductGroupCodeChild(dataProductGroupNew).Result;
                }
                products = from q in products
                           where (from dtpg in dataProductGroup select dtpg.Code).Contains(q.ProductGroupCode) && q.ProductGroupCode != null
                           select q;
            }


            var totalExportArising = from whb in warehouseBooks.ToList()
                                     join v in voucherCategories.ToList() on whb.VoucherCode equals v.Code
                                     join p in products.ToList() on whb.ProductCode equals p.Code
                                     where whb.VoucherDate >= dto.FromDate && whb.VoucherDate <= dto.ToDate && (whb.Status == "1" || whb.Status == "0") //&& v.VoucherGroup == 2
                                     group new { whb } by new
                                     {
                                         whb.WarehouseCode,
                                         whb.ProductLotCode,
                                         whb.ProductCode,
                                         whb.ProductOriginCode
                                     } into gr
                                     select new SurvivingVoucherDto
                                     {
                                         WarehouseCode = gr.Key.WarehouseCode,
                                         ProductLotCode = gr.Key.ProductLotCode,
                                         ProductCode = gr.Key.ProductCode,
                                         ProductOriginCode = gr.Key.ProductOriginCode
                                     };

            foreach (var item in totalExportArising)
            {
                //dto.WareHouseCode = item.WarehouseCode;
                dto.ProductCode = item.ProductCode;
                dto.ProductOriginCode = item.ProductOriginCode;
                dto.ProductLotCode = item.ProductLotCode;
                await CalculatePriceFifoDetail(dto);
            }


            result.Ok = true;
            return result;
        }

        public async Task<ResultDto> CalculatePriceFifoDetail(CostOfGoodsDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var result = new ResultDto();
            var warehouseBooks = await _warehouseBookService.GetQueryableAsync();
            warehouseBooks = warehouseBooks.Where(p => p.OrgCode == dto.OrdCode
                                      && p.Year == dto.Year);
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.ProductOriginCode == dto.ProductOriginCode);
            }
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                warehouseBooks = warehouseBooks.Where(p => p.WarehouseCode == dto.WareHouseCode);
            }

            var voucherCategories = await _voucherCategoryService.GetQueryableAsync();
            voucherCategories = voucherCategories.Where(p => p.OrgCode == dto.OrdCode);
            var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();

            if (voucherCategories.ToList().Count == 0)
            {
                voucherCategories = from a in defaultVoucherCategory
                                    select new VoucherCategory
                                    {
                                        Code = a.Code,
                                        Name = a.Name,
                                        NameE = a.NameE,
                                        VoucherGroup = a.VoucherGroup,
                                        VoucherType = a.VoucherType,
                                        VoucherOrd = a.VoucherOrd,
                                        CurrencyCode = a.CurrencyCode,
                                        AttachBusiness = a.AttachBusiness,
                                        IncreaseNumberMethod = a.IncreaseNumberMethod,
                                        ProductType = a.ProductType,
                                        ChkDuplicateVoucherNumber = a.ChkDuplicateVoucherNumber,
                                        IsTransfer = a.IsTransfer,
                                        IsAssembly = a.IsAssembly,
                                        PriceCalculatingMethod = a.PriceCalculatingMethod,
                                        IsSavingLedger = a.IsSavingLedger,
                                        IsSavingWarehouseBook = a.IsSavingWarehouseBook,
                                        IsCalculateBalanceAcc = a.IsCalculateBalanceAcc,
                                        IsCalculateBalanceProduct = a.IsCalculateBalanceProduct,
                                        Prefix = a.Prefix,
                                        SeparatorCharacter = a.SeparatorCharacter,
                                        Suffix = a.Suffix,
                                        BookClosingDate = a.BookClosingDate,
                                        BusinessBeginningDate = a.BusinessBeginningDate,
                                        BusinessEndingDate = a.BusinessEndingDate,
                                        TaxType = a.TaxType,
                                        VoucherKind = a.VoucherKind,
                                        AttachPartnerPrice = a.AttachPartnerPrice,
                                        TenantType = a.TenantType
                                    };
            }
            var products = await _productService.GetQueryableAsync();
            products = products.Where(p => p.OrgCode == dto.OrdCode

                                        && p.ProductType != "D");
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                products = products.Where(p => p.Code == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductionPeriodCode))
            {
                products = products.Where(p => p.ProductionPeriodCode == dto.ProductionPeriodCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductType))
            {
                products = products.Where(p => p.ProductType == dto.ProductType);
            }

            var productGroups = await _productGroupService.GetQueryableAsync();
            productGroups = productGroups.Where(p => p.OrgCode == dto.OrdCode);
            var productOpeningBalance = await _productOpeningBalanceService.GetQueryableAsync();
            productOpeningBalance = productOpeningBalance.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year);
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.ProductOriginCode == dto.ProductOriginCode);
            }
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                productOpeningBalance = productOpeningBalance.Where(p => p.WarehouseCode == dto.WareHouseCode);
            }
            if (dto.ProductGroup != null && dto.ProductGroup != "")
            {
                productGroups = productGroups.Where(p => p.OrgCode == dto.OrdCode);
                var dataProductGroup = productGroups.Where(p => p.Code == dto.ProductGroup.ToString()).Select(p => ObjectMapper.Map<ProductGroup, ProductGroupDto>(p)).ToList();
                var dataProductGroupNew = GetProductGroupCodeChild(dataProductGroup).Result;
                while (dataProductGroupNew.Count() > 0)
                {
                    foreach (var item in dataProductGroupNew)
                    {
                        dataProductGroup.Add(item);
                    }
                    dataProductGroupNew = GetProductGroupCodeChild(dataProductGroupNew).Result;
                }
                products = from q in products
                           where (from dtpg in dataProductGroup select dtpg.Code).Contains(q.ProductGroupCode) && q.ProductGroupCode != null
                           select q;
            }

            ////Tổng xuất trước ngày chứng từ 1
            var totalExportBeginVoucherDate1 = from w in warehouseBooks
                                               join p in products on w.ProductCode equals p.Code
                                               where w.VoucherGroup == 2 && (w.Status == "1" || w.Status == "0") && w.VoucherDate < dto.FromDate
                                               group new { w, p } by new
                                               {
                                                   w.WarehouseCode,
                                                   w.ProductLotCode,
                                                   w.ProductCode,
                                                   w.ProductOriginCode
                                               } into s
                                               select new
                                               {
                                                   WarehouseCode = s.Key.WarehouseCode,
                                                   ProductLotCode = s.Key.ProductLotCode,
                                                   ProductCode = s.Key.ProductCode,
                                                   ProductOriginCode = s.Key.ProductOriginCode,
                                                   Quantity = s.Sum(p => p.w.ExportQuantity),
                                                   AmountCur = s.Sum(p => p.w.ExportAmountCur),
                                                   Amount = s.Sum(p => p.w.ExportAmount)
                                               };
            var ábs = totalExportBeginVoucherDate1.ToList();
            //Tính nhưng chứng từ còn tồn (đầu kỳ)
            //Chứng từ đầu kỳ survivingVouchers

            var voucherInventory = from pob in productOpeningBalance
                                       //  join p in products on pob.ProductCode equals p.Code
                                   select new
                                   {
                                       Id = pob.Id,
                                       OrgCode = pob.OrgCode,
                                       Ord = pob.Id,
                                       VoucherDate = DateTime.Parse((pob.Year - 1).ToString() + "/12/31"),
                                       Ord0 = "A000000001",
                                       VoucherOrd = "00000",
                                       VoucherCode = "DK000",
                                       VoucherGroup = 1,
                                       VoucherNumber = "",
                                       WarehouseCode = pob.WarehouseCode,
                                       ProductCode = pob.ProductCode,
                                       ProductLotCode = (pob.ProductLotCode == null) ? "" : pob.ProductLotCode,
                                       ProductOriginCode = (pob.ProductOriginCode == null) ? "" : pob.ProductOriginCode,
                                       Quantity = pob.Quantity,
                                       AmountCur = pob.AmountCur,
                                       Amount = pob.Amount,
                                       DateNew = DateTime.Parse((pob.Year - 1).ToString() + "/12/31"),
                                   };


            //Chứng từ còn tồn (đầu kỳ)


            var survivingVouchers = from s in voucherInventory

                                    group new
                                    {
                                        s.WarehouseCode,
                                        s.ProductLotCode,
                                        s.ProductCode,
                                        s.ProductOriginCode,
                                        s.Quantity,
                                        s.Amount,
                                        s.AmountCur,
                                        s.VoucherGroup
                                    } by new
                                    {
                                        s.WarehouseCode,
                                        s.ProductLotCode,
                                        s.ProductCode,
                                        s.ProductOriginCode
                                    } into gr
                                    select new SurvivingVoucherDto
                                    {

                                        WarehouseCode = gr.Key.WarehouseCode,
                                        ProductLotCode = gr.Key.ProductLotCode,
                                        ProductCode = gr.Key.ProductCode,
                                        ProductOriginCode = gr.Key.ProductOriginCode,
                                        Quantity = (gr.Sum(p => p.Quantity)),
                                        AmountCur = gr.Sum(p => p.AmountCur),
                                        Amount = gr.Sum(p => p.Amount),
                                        VoucherGroup = gr.Max(p => p.VoucherGroup)

                                    };
            var dataSurvivingVouchers = survivingVouchers.ToList();
            //TỔNG PHÁT SINH XUẤT TRƯỚC NGÀY CT1
            var totalBeginVoucherDate1 = from whb in warehouseBooks.ToList()
                                         join v in voucherCategories.ToList() on whb.VoucherCode equals v.Code
                                         join p in products.ToList() on whb.ProductCode equals p.Code
                                         where whb.VoucherDate < dto.FromDate && (whb.Status == "1" || whb.Status == "0") && v.VoucherGroup == 2
                                         orderby whb.VoucherDate
                                         select new
                                         {
                                             Id = whb.Id,
                                             OrgCode = whb.OrgCode,
                                             Ord = whb.ProductVoucherId,
                                             Ord0 = whb.Ord0,
                                             //Ord0DC = "B" + whb.Ord0.Substring(2),
                                             VoucherCode = whb.VoucherCode,
                                             VoucherOrd = v.VoucherOrd,
                                             VoucherGroup = 1,
                                             VoucherDate = whb.VoucherDate,
                                             VoucherNumber = whb.VoucherNumber,
                                             DateNew = whb.CreationTime,
                                             WarehouseCode = whb.WarehouseCode,
                                             ProductCode = whb.ProductCode,
                                             ProductLotCode = (whb.ProductLotCode == null) ? "" : whb.ProductLotCode,
                                             ProductOriginCode = (whb.ProductOriginCode == null) ? "" : whb.ProductOriginCode,
                                             UnitCode = whb.UnitCode,
                                             TrxImportQuantity = (whb.TrxImportQuantity == null) ? whb.Quantity : whb.TrxImportQuantity,
                                             Quantity = whb.Quantity ?? 0,
                                             Price = whb.Price0 ?? 0,
                                             PriceCur = whb.PriceCur0 ?? 0,
                                             AmountCur = whb.AmountCur ?? 0,
                                             Amount = whb.Amount ?? 0,
                                             ImportQuantity = whb.ImportQuantity ?? 0,
                                             ImportAmount = whb.ImportAmount ?? 0,
                                             ImportAmountCur = whb.ImportAmountCur ?? 0,
                                             ExportQuantity = whb.ExportQuantity ?? 0,
                                             ExportAmount = whb.ExportAmount ?? 0,
                                             ExportAmountCur = whb.ExportAmountCur ?? 0,
                                             PriceCalculatingMethod = v.PriceCalculatingMethod,
                                             IsTransfer = v.IsTransfer,
                                             IsAssembly = v.IsAssembly,
                                             FixedPrice = whb.FixedPrice,
                                             GetPrice = (v.PriceCalculatingMethod == "F") ? "C" : "K",
                                             AssemblyTransfer = (v.PriceCalculatingMethod == "F") ? "B1" : "B2",
                                             ImportAverage = (v.PriceCalculatingMethod == "F" && whb.VoucherGroup == 1) ? "C" : "K",
                                             OnTop = (v.PriceCalculatingMethod == "F" && v.IsAssembly == "C" && whb.VoucherGroup == 2) ? "A" : "B",
                                         };
            //Tổng phát sinh  trong kỳ (ngày ct1 đến ngày ct2)

            var totalExportArising = from whb in warehouseBooks.ToList()
                                     join v in voucherCategories.ToList() on whb.VoucherCode equals v.Code
                                     join p in products.ToList() on whb.ProductCode equals p.Code
                                     where whb.VoucherDate >= dto.FromDate && whb.VoucherDate <= dto.ToDate && (whb.Status == "1" || whb.Status == "0") //&& v.VoucherGroup == 2
                                     orderby whb.VoucherDate
                                     select new
                                     {
                                         Id = whb.Id,
                                         OrgCode = whb.OrgCode,
                                         ProductVoucherId = whb.ProductVoucherId,
                                         Ord0 = whb.Ord0,
                                         //Ord0DC = "B" + whb.Ord0.Substring(2),
                                         VoucherCode = whb.VoucherCode,
                                         VoucherOrd = v.VoucherOrd,
                                         VoucherGroup = whb.VoucherGroup,
                                         VoucherDate = whb.VoucherDate,
                                         VoucherNumber = whb.VoucherNumber,
                                         DateNew = whb.CreationTime,
                                         WarehouseCode = whb.WarehouseCode,
                                         ProductCode = whb.ProductCode,
                                         ProductLotCode = (whb.ProductLotCode == null) ? "" : whb.ProductLotCode,
                                         ProductOriginCode = (whb.ProductOriginCode == null) ? "" : whb.ProductOriginCode,
                                         UnitCode = whb.UnitCode,
                                         TrxImportQuantity = (whb.TrxImportQuantity == null) ? whb.Quantity : whb.TrxImportQuantity,
                                         Quantity = whb.Quantity ?? 0,
                                         Price = whb.Price0 ?? 0,
                                         PriceCur = whb.PriceCur0 ?? 0,
                                         AmountCur = whb.AmountCur ?? 0,
                                         Amount = whb.Amount ?? 0,
                                         ImportQuantity = whb.ImportQuantity ?? 0,
                                         ImportAmount = whb.ImportAmount ?? 0,
                                         ImportAmountCur = whb.ImportAmountCur ?? 0,
                                         ExportQuantity = whb.ExportQuantity ?? 0,
                                         ExportAmount = whb.ExportAmount ?? 0,
                                         ExportAmountCur = whb.ExportAmountCur ?? 0,
                                         PriceCalculatingMethod = v.PriceCalculatingMethod,
                                         IsTransfer = v.IsTransfer,
                                         IsAssembly = v.IsAssembly,
                                         FixedPrice = whb.FixedPrice,
                                         GetPrice = (v.PriceCalculatingMethod == "F") ? "C" : "K",
                                         AssemblyTransfer = (v.PriceCalculatingMethod == "F") ? "B1" : "B2",
                                         ImportAverage = (v.PriceCalculatingMethod == "F" && whb.VoucherGroup == 1) ? "C" : "K",
                                         OnTop = (v.PriceCalculatingMethod == "F" && v.IsAssembly == "C" && whb.VoucherGroup == 2) ? "A" : "B",
                                         CreateTime = whb.CreationTime,
                                         Year = whb.Year
                                     };

            //Dữ liệu tồn đầu kỳ
            var dataTotalBeginVoucherDate1 = totalBeginVoucherDate1.ToList(); //Dữ liệu tổng ps nhập xuất trước ngày ct 1
            var dataTotalExportArising = totalExportArising.OrderBy(p => p.VoucherDate).ThenBy(p => p.CreateTime).ThenBy(p => p.ProductVoucherId).ThenByDescending(p => p.VoucherGroup).ToList(); //Dữ liệu tổng ps nhập xuất trong kỳ

            // Xử lý dữ liệu xuất trong kỳ nhưng trước ngày ct1 để tìm tồn đầu
            if (dataTotalBeginVoucherDate1.Count() > 0)
            {
                for (int i = 0; i < dataTotalBeginVoucherDate1.Count(); i++)
                {
                    if (dataTotalBeginVoucherDate1[i].VoucherGroup == 2)
                    {
                        Decimal importQuantity = 0;
                        Decimal totalImportAmount = 0;
                        Decimal exportQuantity = dataTotalBeginVoucherDate1[i].Quantity;
                        Decimal totalExportAmount = dataTotalBeginVoucherDate1[i].Amount;
                        var j = 0;
                        // So sánh số lượng xuất với số lượng nhập của các chứng từ và tồn đầu kỳ, nếu sl xuất <= sl nhập sẽ dừng lại và update cột Quantity của các chứng từ đầu kỳ đã lặp qua = 0
                        while (importQuantity < exportQuantity)
                        {
                            importQuantity += dataSurvivingVouchers[j].Quantity;
                            totalImportAmount += dataSurvivingVouchers[j].Amount;
                            dataSurvivingVouchers[j].Quantity = 0;
                            j++;
                            if (j == dataSurvivingVouchers.Count() && importQuantity < exportQuantity)
                            {
                                result.Ok = false;
                                result.Message = "Số lượng nhập nhỏ hơn số lượng xuất";
                                return result;
                            }
                        }
                        dataSurvivingVouchers[j - 1].Quantity = importQuantity - exportQuantity;
                        dataSurvivingVouchers[j - 1].Amount = totalImportAmount - totalExportAmount;
                        // Xóa các chứng từ đầu kỳ có số lượng bằng 0
                        if (dataSurvivingVouchers[j - 1].Quantity != 0)
                        {
                            int vtXoa = 0;
                            while (vtXoa == j)
                            {
                                dataSurvivingVouchers.RemoveAt(vtXoa);
                                vtXoa++;
                            }
                        }
                        else
                        {
                            int vtXoa = 0;
                            while (vtXoa > j)
                            {
                                dataSurvivingVouchers.RemoveAt(vtXoa);
                                vtXoa++;
                            }
                        }
                    }
                }
            }
            // xử lý xong tồn đầu
            // Tính giá vốn
            var testa = dataTotalExportArising.Where(p => p.Ord0.Contains("A") == true);

            foreach (var item in testa)
            {


                Decimal totalImportQuantity = item.ImportQuantity;
                Decimal exportQuantity = item.ExportQuantity;
                Decimal totalExportAmount = item.Price;
                var j = 0;

                var dataSurvivingVoucher = dataSurvivingVouchers.Where(p => p.ProductCode == item.ProductCode
                                                                 ).ToList();
                if (!string.IsNullOrEmpty(item.WarehouseCode))
                {
                    dataSurvivingVoucher = dataSurvivingVoucher.Where(p => p.WarehouseCode == item.WarehouseCode).ToList();
                }
                if (!string.IsNullOrEmpty(item.ProductLotCode))
                {
                    dataSurvivingVoucher = dataSurvivingVoucher.Where(p => p.ProductLotCode == item.ProductLotCode).ToList();
                }
                if (!string.IsNullOrEmpty(item.ProductOriginCode))
                {
                    dataSurvivingVoucher = dataSurvivingVoucher.Where(p => p.ProductOriginCode == item.ProductOriginCode).ToList();
                }
                Decimal importQuantity = 0;
                Decimal importAmount = 0;
                Decimal totalExportAmounts = 0;
                Decimal quantity = 0;
                decimal GiaVon = 0;
                Decimal exportQuantitys = 0;
                exportQuantitys = exportQuantity;

                foreach (var items in dataSurvivingVoucher.OrderBy(p => p.VoucherDate))
                {
                    if (item.PriceCalculatingMethod == "B")
                    {
                        importQuantity = items.Quantity;
                        if (items.Amount != 0 && items.Quantity != 0)
                        {
                            importAmount = items.Amount / items.Quantity;
                        }

                        if (exportQuantitys - items.Quantity > 0)
                        {

                            totalExportAmount = items.Quantity * importAmount;
                        }
                        else
                        {
                            totalExportAmount = exportQuantitys * importAmount;
                        }
                        if (exportQuantitys != 0)
                        {
                            items.Quantity = importQuantity - exportQuantitys;
                        }
                        else
                        {
                            items.Quantity = importQuantity - Math.Abs(quantity);
                        }


                        exportQuantitys = 0;
                        if (items.Quantity == 0)
                        {
                            quantity = 0;
                        }
                        else
                        {
                            if (items.Quantity > 0)
                            {
                                quantity = items.Quantity;
                            }
                            else
                            {
                                quantity = items.Quantity - Math.Abs(quantity);
                            }



                        }


                        items.Amount = items.Amount - (totalExportAmount == 0 ? importAmount * (importQuantity - items.Quantity) : totalExportAmount);
                        if (totalExportAmounts == 0)
                        {
                            totalExportAmounts = Math.Abs(totalExportAmount - items.Amount);
                        }
                        //else
                        //{
                        //    totalExportAmounts += importAmount * (importQuantity - items.Quantity);

                        //}



                        if (!string.IsNullOrEmpty(items.Ord0))
                        {

                            if (quantity <= 0)
                            {
                                dataSurvivingVouchers.Remove(items);

                            }

                            if (quantity >= 0)
                            {
                                if (items.Amount != 0 && items.Quantity != 0)
                                {
                                    GiaVon = items.Amount / items.Quantity;
                                }


                                if (GiaVon == 0)
                                {
                                    totalExportAmounts = totalExportAmounts + importAmount * importQuantity;
                                }
                                else
                                {
                                    totalExportAmounts = totalExportAmounts + GiaVon * (importQuantity - quantity);
                                }

                                break;
                            }

                        }
                    }


                    items.Ord0 = item.Ord0;
                }

                if (item.PriceCalculatingMethod == "T")
                {
                    SurvivingVoucherDto survivingVoucherDto = new SurvivingVoucherDto();
                    survivingVoucherDto.Quantity = item.Quantity;
                    survivingVoucherDto.Amount = item.Amount;
                    survivingVoucherDto.WarehouseCode = item.WarehouseCode;
                    survivingVoucherDto.ProductCode = item.ProductCode;
                    survivingVoucherDto.VoucherDate = item.CreateTime;
                    survivingVoucherDto.Ord0 = item.Ord0;
                    survivingVoucherDto.VoucherGroup = item.VoucherGroup;
                    survivingVoucherDto.VoucherNumber = item.VoucherNumber;
                    dataSurvivingVouchers.Add(survivingVoucherDto);
                }
                //Tính giá vốn



                if (exportQuantity != 0 && totalExportAmounts != 0)
                {
                    GiaVon = totalExportAmounts / exportQuantity;

                }
                if (exportQuantity != 0 && totalExportAmount != 0)
                {
                    GiaVon = totalExportAmount / exportQuantity;

                }

                var resulWarehoseBook = from a in totalExportArising
                                        where a.Id == item.Id && a.PriceCalculatingMethod == "B"
                                        select new
                                        {
                                            Id = a.Id,
                                            OrgCode = a.OrgCode,
                                            ProductVoucherId = a.ProductVoucherId,
                                            Ord0 = a.Ord0,
                                            //Ord0DC = "B" + whb.Ord0.Substring(2),
                                            VoucherCode = a.VoucherCode,
                                            VoucherOrd = a.VoucherOrd,
                                            VoucherGroup = a.VoucherGroup,
                                            VoucherDate = a.VoucherDate,
                                            VoucherNumber = a.VoucherNumber,
                                            DateNew = a.DateNew,
                                            WarehouseCode = a.WarehouseCode,
                                            ProductCode = a.ProductCode,
                                            ProductLotCode = a.ProductLotCode,
                                            ProductOriginCode = a.ProductOriginCode,
                                            UnitCode = a.UnitCode,
                                            TrxImportQuantity = a.TrxImportQuantity,
                                            Quantity = a.Quantity,
                                            Price = GiaVon,
                                            PriceCur = (decimal)0,
                                            AmountCur = a.AmountCur,
                                            Amount = Math.Round(a.Quantity * GiaVon),
                                            ImportQuantity = a.ImportQuantity,
                                            ImportAmount = a.ImportAmount,
                                            ImportAmountCur = a.ImportAmountCur,
                                            ExportQuantity = a.ExportQuantity,
                                            ExportAmount = Math.Round(a.Quantity * GiaVon),
                                            ExportAmountCur = a.ExportAmountCur,
                                            PriceCalculatingMethod = a.PriceCalculatingMethod,
                                            IsTransfer = a.IsTransfer,
                                            IsAssembly = a.IsAssembly,
                                            FixedPrice = a.FixedPrice,
                                            GetPrice = a.GetPrice,
                                            AssemblyTransfer = a.AssemblyTransfer,
                                            ImportAverage = a.ImportAverage,
                                            OnTop = a.OnTop
                                        };

                dataTotalExportArising = (from a in dataTotalExportArising
                                          join b in resulWarehoseBook on a.Id equals b.Id
                                          into c
                                          from d in c.DefaultIfEmpty()
                                          select new
                                          {
                                              Id = a.Id,
                                              OrgCode = a.OrgCode,
                                              ProductVoucherId = a.ProductVoucherId,
                                              Ord0 = a.Ord0,
                                              //Ord0DC = "B" + whb.Ord0.Substring(2),
                                              VoucherCode = a.VoucherCode,
                                              VoucherOrd = a.VoucherOrd,
                                              VoucherGroup = a.VoucherGroup,
                                              VoucherDate = a.VoucherDate,
                                              VoucherNumber = a.VoucherNumber,
                                              DateNew = a.DateNew,
                                              WarehouseCode = a.WarehouseCode,
                                              ProductCode = a.ProductCode,
                                              ProductLotCode = a.ProductLotCode,
                                              ProductOriginCode = a.ProductOriginCode,
                                              UnitCode = a.UnitCode,
                                              TrxImportQuantity = a.TrxImportQuantity,
                                              Quantity = a.Quantity,
                                              Price = d != null ? d.Price : a.Price,
                                              PriceCur = (decimal)0,
                                              AmountCur = a.AmountCur,
                                              Amount = d != null ? d.Amount : a.Amount,
                                              ImportQuantity = a.ImportQuantity,
                                              ImportAmount = a.ImportAmount,
                                              ImportAmountCur = a.ImportAmountCur,
                                              ExportQuantity = a.ExportQuantity,
                                              ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                              ExportAmountCur = a.ExportAmountCur,
                                              PriceCalculatingMethod = a.PriceCalculatingMethod,
                                              IsTransfer = a.IsTransfer,
                                              IsAssembly = a.IsAssembly,
                                              FixedPrice = a.FixedPrice,
                                              GetPrice = a.GetPrice,
                                              AssemblyTransfer = a.AssemblyTransfer,
                                              ImportAverage = a.ImportAverage,
                                              OnTop = a.OnTop,
                                              a.CreateTime,
                                              a.Year
                                          }).ToList();


                // update lại B-00001
                if (item.IsTransfer == "C")
                {

                    var resulWarehoseBookTrs = from a in dataTotalExportArising
                                               where a.ProductVoucherId == item.ProductVoucherId && a.Ord0 == "B" + a.Ord0.Substring(1, 9)
                                               && a.PriceCalculatingMethod == "B"
                                               select new
                                               {
                                                   Id = a.Id,
                                                   OrgCode = a.OrgCode,
                                                   ProductVoucherId = a.ProductVoucherId,
                                                   Ord0 = a.Ord0,
                                                   //Ord0DC = "B" + whb.Ord0.Substring(2),
                                                   VoucherCode = a.VoucherCode,
                                                   VoucherOrd = a.VoucherOrd,
                                                   VoucherGroup = a.VoucherGroup,
                                                   VoucherDate = a.VoucherDate,
                                                   VoucherNumber = a.VoucherNumber,
                                                   DateNew = a.DateNew,
                                                   WarehouseCode = a.WarehouseCode,
                                                   ProductCode = a.ProductCode,
                                                   ProductLotCode = a.ProductLotCode,
                                                   ProductOriginCode = a.ProductOriginCode,
                                                   UnitCode = a.UnitCode,
                                                   TrxImportQuantity = a.TrxImportQuantity,
                                                   Quantity = a.Quantity,
                                                   Price = GiaVon,
                                                   PriceCur = 0,
                                                   AmountCur = a.AmountCur,
                                                   Amount = Math.Round(a.Quantity * GiaVon),
                                                   ImportQuantity = a.ImportQuantity,
                                                   ImportAmount = Math.Round(a.Quantity * GiaVon),
                                                   ImportAmountCur = a.ImportAmountCur,
                                                   ExportQuantity = a.ExportQuantity,
                                                   ExportAmount = a.ExportAmount,
                                                   ExportAmountCur = a.ExportAmountCur,
                                                   PriceCalculatingMethod = a.PriceCalculatingMethod,
                                                   IsTransfer = a.IsTransfer,
                                                   IsAssembly = a.IsAssembly,
                                                   FixedPrice = a.FixedPrice,
                                                   GetPrice = a.GetPrice,
                                                   AssemblyTransfer = a.AssemblyTransfer,
                                                   ImportAverage = a.ImportAverage,
                                                   OnTop = a.OnTop,
                                                   a.CreateTime
                                               };
                    var testss = resulWarehoseBookTrs.ToList();
                    if (resulWarehoseBookTrs.FirstOrDefault().Quantity > 0)
                    {
                        SurvivingVoucherDto survivingVoucherDto = new SurvivingVoucherDto();
                        survivingVoucherDto.Quantity = resulWarehoseBookTrs.FirstOrDefault().Quantity;
                        survivingVoucherDto.Amount = resulWarehoseBookTrs.FirstOrDefault().Amount;
                        survivingVoucherDto.WarehouseCode = resulWarehoseBookTrs.FirstOrDefault().WarehouseCode;
                        survivingVoucherDto.ProductCode = resulWarehoseBookTrs.FirstOrDefault().ProductCode;
                        survivingVoucherDto.VoucherDate = resulWarehoseBookTrs.FirstOrDefault().CreateTime;
                        survivingVoucherDto.Ord0 = resulWarehoseBookTrs.FirstOrDefault().Ord0;
                        survivingVoucherDto.VoucherGroup = resulWarehoseBookTrs.FirstOrDefault().VoucherGroup;
                        survivingVoucherDto.VoucherNumber = resulWarehoseBookTrs.FirstOrDefault().VoucherNumber;
                        dataSurvivingVouchers.Add(survivingVoucherDto);
                    }
                    dataTotalExportArising = (from a in dataTotalExportArising
                                              join b in resulWarehoseBookTrs.ToList() on a.Id equals b.Id
                                              into c
                                              from d in c.DefaultIfEmpty()
                                              select new
                                              {
                                                  Id = a.Id,
                                                  OrgCode = a.OrgCode,
                                                  ProductVoucherId = a.ProductVoucherId,
                                                  Ord0 = a.Ord0,
                                                  //Ord0DC = "B" + whb.Ord0.Substring(2),
                                                  VoucherCode = a.VoucherCode,
                                                  VoucherOrd = a.VoucherOrd,
                                                  VoucherGroup = a.VoucherGroup,
                                                  VoucherDate = a.VoucherDate,
                                                  VoucherNumber = a.VoucherNumber,
                                                  DateNew = a.DateNew,
                                                  WarehouseCode = a.WarehouseCode,
                                                  ProductCode = a.ProductCode,
                                                  ProductLotCode = a.ProductLotCode,
                                                  ProductOriginCode = a.ProductOriginCode,
                                                  UnitCode = a.UnitCode,
                                                  TrxImportQuantity = a.TrxImportQuantity,
                                                  Quantity = a.Quantity,
                                                  Price = d != null ? d.Price : a.Price,
                                                  PriceCur = (decimal)0,
                                                  AmountCur = a.AmountCur,
                                                  Amount = d != null ? d.Amount : a.Amount,
                                                  ImportQuantity = a.ImportQuantity,
                                                  ImportAmount = a.ImportAmount,
                                                  ImportAmountCur = a.ImportAmountCur,
                                                  ExportQuantity = a.ExportQuantity,
                                                  ExportAmount = a.ExportAmount,
                                                  ExportAmountCur = a.ExportAmountCur,
                                                  PriceCalculatingMethod = a.PriceCalculatingMethod,
                                                  IsTransfer = a.IsTransfer,
                                                  IsAssembly = a.IsAssembly,
                                                  FixedPrice = a.FixedPrice,
                                                  GetPrice = a.GetPrice,
                                                  AssemblyTransfer = a.AssemblyTransfer,
                                                  ImportAverage = a.ImportAverage,
                                                  OnTop = a.OnTop,
                                                  a.CreateTime,
                                                  a.Year
                                              }).ToList();

                }



            }
            // update vào phiếu
            foreach (var item in dataTotalExportArising)
            {
                var warehouseBook = await _warehouseBookService.GetQueryableAsync();
                warehouseBook = warehouseBook.Where(p => p.Id == item.Id);
                foreach (var items in warehouseBook)
                {
                    items.Price = item.Price;
                    items.Amount = item.Amount;
                    items.ImportAmount = item.ImportAmount;
                    items.ExportAmount = item.ExportAmount;
                    items.Price0 = item.Price;
                    await _warehouseBookService.UpdateAsync(items, true);
                }
                var ledger = await _ledgerService.GetQueryableAsync();
                ledger = ledger.Where(p => p.VoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0);
                foreach (var items in ledger)
                {
                    items.Price = item.Price;
                    items.Amount = item.Amount;
                    await _ledgerService.UpdateAsync(items, true);
                }
                var productDetail = await _productVoucherDetailService.GetQueryableAsync();
                productDetail = productDetail.Where(p => p.ProductVoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0);
                foreach (var items in productDetail)
                {
                    items.Price = item.Price;
                    items.Amount = item.Amount;
                    await _productVoucherDetailService.UpdateAsync(items, true);
                }
            }



            result.Ok = true;
            return result;
        }
        #region Private

        private async Task<List<ProductOpeningBalance>> GetProductOpeningBalancesAsync(int year, string ordCode, string productCode, string warehoseCode, string productLotCode, string productOriginCode)
        {
            var productOpeningBalance = await _productOpeningBalanceService.GetQueryableAsync();
            var productOpeningBalances = productOpeningBalance.Where(p => p.OrgCode == ordCode && p.Year == year).ToList();
            if (!string.IsNullOrEmpty(productCode))
            {
                productOpeningBalances = productOpeningBalances.Where(p => p.ProductCode == productCode).ToList();
            }
            if (!string.IsNullOrEmpty(warehoseCode))
            {
                productOpeningBalances = productOpeningBalances.Where(p => p.WarehouseCode == warehoseCode).ToList();
            }
            if (!string.IsNullOrEmpty(productLotCode))
            {
                productOpeningBalances = productOpeningBalances.Where(p => p.ProductLotCode == productLotCode).ToList();
            }
            if (!string.IsNullOrEmpty(productOriginCode))
            {
                productOpeningBalances = productOpeningBalances.Where(p => p.ProductOriginCode == productOriginCode).ToList();
            }
            return productOpeningBalances;
        }

        private async Task<List<ProductGroupDto>> GetProductGroupCodeChild(List<ProductGroupDto> listData)
        {
            var productGroups = await _productGroupService.GetQueryableAsync();
            var productGroupCodeChild = from p in productGroups
                                        where (from aS0 in listData
                                               select aS0.Id).Contains(p.ParentId) && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        select p;
            var res = productGroupCodeChild.Select(p => ObjectMapper.Map<ProductGroup, ProductGroupDto>(p)).ToList();
            return res;
        }

        private async Task<List<CrudProductVoucherDetailDto>> GetProductVoucherDetailAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherDetailDto>();
            var query = await _productVoucherDetailService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherDetail, CrudProductVoucherDetailDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudAccTaxDetailDto>> GetAccTaxDetailAsync(string productVoucherId)
        {
            var result = new List<CrudAccTaxDetailDto>();
            var query = await _accTaxDetailService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AccTaxDetail, CrudAccTaxDetailDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudProductVoucherAssemblyDto>> GetProductVoucherAssemblyAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherAssemblyDto>();
            var query = await _productVoucherAssemblyService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherAssembly, CrudProductVoucherAssemblyDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudProductVoucherReceiptDto>> GetProductVoucherReceiptAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherReceiptDto>();
            var query = await _productVoucherReceiptService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherReceipt, CrudProductVoucherReceiptDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudProductVoucherVatDto>> GetProductVoucherVatAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherVatDto>();
            var query = await _productVoucherVatService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherVat, CrudProductVoucherVatDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudProductVoucherCostDto>> GetProductVoucherCostAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherCostDto>();
            var query = await _productVoucherCostService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherCost, CrudProductVoucherCostDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudVoucherExciseTaxDto>> GetVoucherExciseTaxAsync(string productVoucherId)
        {
            var result = new List<CrudVoucherExciseTaxDto>();
            var query = await _voucherExciseTaxService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<VoucherExciseTax, CrudVoucherExciseTaxDto>(p)).ToList();
            return result;
        }
        #endregion
    }
}
