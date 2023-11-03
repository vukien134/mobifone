using Accounting.Business;
using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;

using Accounting.Categories.Products;
using Accounting.Categories.ProductVouchers;
using Accounting.Catgories.FProductWorkNorms;
using Accounting.Catgories.Others.CostOfGoods;
using Accounting.Catgories.ProductVouchers;
using Accounting.Constants;
using Accounting.DomainServices.Categories;

using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Extensions;
using Accounting.Generals;
using Accounting.Helpers;
using Accounting.JsonConverters;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.CalculatePriceFifoes;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.VoucherExciseTaxs;
using Accounting.Vouchers.WarehouseBooks;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using NPOI.HPSF;
using NPOI.OpenXmlFormats.Dml;
using NPOI.OpenXmlFormats.Spreadsheet;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Zlib;
using StackExchange.Redis;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Uow;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static NPOI.SS.Formula.Functions.Countif;

namespace Accounting.Categories.CostOfGoods
{
    public class CostOfGoodsAppService : AccountingAppService
    {
        #region Fields

        private readonly WebHelper _webHelper;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly ProductGroupService _productGroupService;
        private readonly WarehouseService _warehouseService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly ProductService _productService;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherAssemblyService _productVoucherAssembly;
        private readonly ProductVoucherDetailService _productVoucherDetail;
        private readonly ProductVoucherReceiptService _productVoucherReceipt;
        private readonly ProductVoucherVatService _productVoucherVat;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly LedgerService _ledgerService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly ProductVoucherCostService _productVoucherCostService;
        private readonly InfoCalcPriceStockOutAppService _infoCalcPriceStockOutAppService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductVoucherAppService _productVoucherAppService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly ProductAppService _productAppService;
        private readonly CalculatePriceFifoAppService _calculatePriceFifoAppService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly FProductWorkNormDetailService _fProductWorkNormDetailService;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;

        #endregion
        #region Ctor
        public CostOfGoodsAppService(ProductLotService productLotService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            VoucherCategoryService voucherCategoryService,
                            TenantSettingService tenantSettingService,
                            ProductGroupService productGroupService,
                            WarehouseBookService warehouseBookService,
                            ProductService productService,
                            ProductOpeningBalanceService productOpeningBalanceService,
                            ProductVoucherService productVoucherService,
                            ProductVoucherAssemblyService productVoucherAssemblyService,
                            ProductVoucherDetailService productVoucherDetailService,
                            ProductVoucherReceiptService productVoucherReceiptService,
                            ProductVoucherVatService productVoucherVatService,
                            AccTaxDetailService accTaxDetailService,
                            LedgerService ledgerService,
                            IUnitOfWorkManager unitOfWorkManager,
                            VoucherExciseTaxService voucherExciseTaxService,
                            ProductVoucherCostService productVoucherCostService,
                            WarehouseService warehouseService,
                            InfoCalcPriceStockOutAppService infoCalcPriceStockOutAppService,
                            ProductUnitService productUnitService,
                            ProductVoucherAppService productVoucherAppService,
                            VoucherTypeService voucherTypeService,
                            ProductAppService productAppService,
                            CalculatePriceFifoAppService calculatePriceFifoAppService,
                            DefaultVoucherCategoryService defaultVoucherCategoryService,
                            DefaultTenantSettingService defaultTenantSettingService,
                            FProductWorkNormDetailService fProductWorkNormDetailService,
                            DefaultVoucherTypeService defaultVoucherTypeService
                            )
        {

            _webHelper = webHelper;
            _licenseBusiness = licenseBusiness;
            _voucherCategoryService = voucherCategoryService;
            _tenantSettingService = tenantSettingService;
            _productGroupService = productGroupService;
            _warehouseService = warehouseService;
            _warehouseBookService = warehouseBookService;
            _productService = productService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _productVoucherService = productVoucherService;
            _productVoucherAssembly = productVoucherAssemblyService;
            _productVoucherDetail = productVoucherDetailService;
            _productVoucherReceipt = productVoucherReceiptService;
            _productVoucherVat = productVoucherVatService;
            _accTaxDetailService = accTaxDetailService;
            _ledgerService = ledgerService;
            _unitOfWorkManager = unitOfWorkManager;
            _voucherExciseTaxService = voucherExciseTaxService;
            _productVoucherCostService = productVoucherCostService;
            _infoCalcPriceStockOutAppService = infoCalcPriceStockOutAppService;
            _productUnitService = productUnitService;
            _productVoucherAppService = productVoucherAppService;
            _voucherTypeService = voucherTypeService;
            _productAppService = productAppService;
            _calculatePriceFifoAppService = calculatePriceFifoAppService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _fProductWorkNormDetailService = fProductWorkNormDetailService;
            _defaultVoucherTypeService = defaultVoucherTypeService;
        }
        #endregion


        public async Task<CostOfGoodsDto> CreateCostOfGoodsAsync(CostOfGoodsDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var voucherCategories = await _voucherCategoryService.GetQueryableAsync();
            voucherCategories = voucherCategories.Where(p => p.PriceCalculatingMethod == "B" || p.PriceCalculatingMethod == "F" && p.OrgCode == dto.OrdCode && p.BookClosingDate > dto.FromDate);
            var terna = await _tenantSettingService.GetQueryableAsync();
            var ternas = terna.Where(p => p.Key == "VHT_GT_DON_KHO" && p.OrgCode == dto.OrdCode).ToList();

            decimal value = 0;
            if (ternas.Count > 0)
            {

                value = decimal.Parse(ternas[0].Value);

            }
            else
            {
                var lstdefaultTenantSeting = await _defaultTenantSettingService.GetQueryableAsync();
                var defaultTenantSeting = lstdefaultTenantSeting.Where(p => p.Key == "VHT_GT_DON_KHO").FirstOrDefault();
                value = decimal.Parse(defaultTenantSeting.Value);
            }


            if (dto.Type == "1")
            {
                if (dto.ConsecutiveMonth == 0)
                {
                    if (dto.Continuous == true)
                    {

                        List<DateTime> monthsInRange = new List<DateTime>();

                        DateTime currentMonth = dto.FromDate;

                        while (currentMonth.Year == dto.FromDate.Year)
                        {
                            monthsInRange.Add(currentMonth);
                            currentMonth = currentMonth.AddMonths(-1);
                        }


                        foreach (var item in monthsInRange.OrderBy(d => d))
                        {
                            dto.FromDate = item.Date;
                            dto.ToDate = item.Date.AddMonths(1).AddDays(-1);
                            await MonthlyAvgPrice(dto, value);
                        }

                    }

                    else
                    {
                        await MonthlyAvgPrice(dto, value);
                    }


                }
            }
            if (dto.Type == "2")
            {
                await MonthlyAvgPrices(dto, value);
            }
            if (dto.Type == "3")
            {
                await _calculatePriceFifoAppService.CalculatePriceFifo(dto);

            }

            return dto;
        }

        public async Task MonthlyAvgPrices(CostOfGoodsDto dto, decimal v)

        {
            CostOfGoodsDto costOfGoodsDto = new CostOfGoodsDto();
            List<Product> products = new List<Product>();
            var defaultVoucherType = await _defaultVoucherTypeService.GetQueryableAsync();
            var ctLrs = await _voucherTypeService.GetQueryableAsync();
            var listCtLrs = "";
            var listCt = "";
            if (ctLrs.ToList().Count > 0)
            {
                listCtLrs = ctLrs.Where(p => p.Code == "PLR").FirstOrDefault().Code;
                listCt = ctLrs.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            }
            else
            {
                listCtLrs = defaultVoucherType.Where(p => p.Code == "PLR").FirstOrDefault().Code;
                listCt = defaultVoucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            }


            var productUnit = await _productUnitService.GetQueryableAsync();
            var lstproductUnit = productUnit.Where(p => p.OrgCode == dto.OrdCode).ToList();
            if (!string.IsNullOrEmpty(dto.ProductGroup))
            {
                products = await _productAppService.GetListByProductGroupCode(dto.ProductGroup);
            }

            var product = await _productService.GetQueryableAsync();

            var lstProduct = product.Where(p => p.OrgCode == dto.OrdCode).ToList();
            //CÁC MÃ HÀNG SỬ DỤNG TRONG KỲ
            var resulProduct = (from a in lstProduct
                                join b in products on a.Code equals b.Code into c
                                from pr in c.DefaultIfEmpty()

                                select new
                                {
                                    a.OrgCode,
                                    ProductCode = a.Code,
                                    ProductionPeriodCode = a.ProductionPeriodCode,
                                    ProductType = pr != null ? pr.ProductType : null
                                }).AsQueryable();
            if (!string.IsNullOrEmpty(dto.ProductionPeriodCode))
            {
                resulProduct = resulProduct.Where(p => p.ProductionPeriodCode == dto.ProductionPeriodCode);
            }

            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                resulProduct = resulProduct.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductType))
            {
                resulProduct = resulProduct.Where(p => p.ProductType == dto.ProductType);

            }
            var wareHouseBook = await _warehouseBookService.GetQueryableAsync();
            var lstWareHouseBook = wareHouseBook.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year && p.VoucherDate >= dto.FromDate && p.VoucherDate <= dto.ToDate).ToList();
            var lstWarehouses = wareHouseBook.Where(p => p.OrgCode == dto.OrdCode && p.VoucherDate <= dto.FromDate && p.Year == dto.Year && p.Status == "1");

            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                lstWarehouses = lstWarehouses.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                lstWarehouses = lstWarehouses.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                lstWarehouses = lstWarehouses.Where(p => p.ProductOriginCode == dto.ProductOriginCode);
            }
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                lstWarehouses = lstWarehouses.Where(p => p.WarehouseCode == dto.WareHouseCode);
            }

            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == dto.OrdCode).ToList();
            var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();
            var lstDefaultVoucherCategory = defaultVoucherCategory.ToList();
            //Tông hợp Nhập xuất trong năm trước ngày @p_Ngay1
            if (lstVoucherCategory.Count == 0)
            {
                lstVoucherCategory = (from a in lstDefaultVoucherCategory
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
                                      }).ToList();
            }
            var importExport0 = (from a in lstWarehouses.ToList()
                                 join b in resulProduct.ToList() on a.ProductCode equals b.ProductCode

                                 group new
                                 {
                                     a.ProductCode,
                                     a.ProductLotCode,
                                     a.ProductOriginCode,
                                     a.WarehouseCode,
                                     a.ImportQuantity,
                                     a.ExportQuantity,
                                     a.ImportAmount,
                                     a.ImportAmountCur,
                                     a.ExportAmount,
                                     a.ExportAmountCur
                                 } by new
                                 {
                                     a.ProductCode,
                                     a.ProductLotCode,
                                     a.ProductOriginCode,
                                     a.WarehouseCode
                                 } into gr
                                 select new
                                 {
                                     ProductCode = gr.Key.ProductCode,
                                     ProductOriginCode = gr.Key.ProductOriginCode,
                                     ProductLotCode = gr.Key.ProductLotCode,
                                     WarehouseCode = gr.Key.WarehouseCode,
                                     Quantity = (decimal)(gr.Sum(p => p.ImportQuantity) - gr.Sum(p => p.ExportQuantity)),
                                     Amount = (decimal)(gr.Sum(p => p.ImportAmount) - gr.Sum(p => p.ExportAmount)),
                                     AmountCur = (decimal)(gr.Sum(p => p.ImportAmountCur) - gr.Sum(p => p.ExportAmountCur)),

                                 }).ToList();

            var productOpeningbalance = await _productOpeningBalanceService.GetQueryableAsync();
            productOpeningbalance = productOpeningbalance.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year);
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                productOpeningbalance = productOpeningbalance.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                productOpeningbalance = productOpeningbalance.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                productOpeningbalance = productOpeningbalance.Where(p => p.ProductOriginCode == dto.ProductOriginCode);

            }

            var resulProductOpening = (from a in productOpeningbalance.ToList()
                                       join b in resulProduct.ToList() on a.ProductCode equals b.ProductCode
                                       group new
                                       {
                                           a.ProductCode,
                                           a.ProductLotCode,
                                           a.ProductOriginCode,
                                           a.WarehouseCode,
                                           a.Quantity,
                                           a.Amount,
                                           a.AmountCur
                                       } by new
                                       {
                                           a.ProductCode,
                                           a.ProductLotCode,
                                           a.ProductOriginCode,
                                           a.WarehouseCode
                                       } into gr
                                       select new
                                       {
                                           ProductCode = gr.Key.ProductCode,
                                           ProductOriginCode = gr.Key.ProductOriginCode,
                                           ProductLotCode = gr.Key.ProductLotCode,
                                           WarehouseCode = gr.Key.WarehouseCode,
                                           Quantity = (decimal)gr.Sum(p => p.Quantity),
                                           Amount = (decimal)gr.Sum(p => p.Amount),
                                           AmountCur = (decimal)gr.Sum(p => p.AmountCur)
                                       }).ToList();
            importExport0.AddRange(resulProductOpening);
            importExport0 = (from a in importExport0
                             group new
                             {
                                 a.ProductCode,
                                 a.ProductLotCode,
                                 a.ProductOriginCode,
                                 a.WarehouseCode,
                                 a.Quantity,
                                 a.Amount,
                                 a.AmountCur
                             } by new
                             {
                                 a.ProductCode,
                                 a.ProductLotCode,
                                 a.ProductOriginCode,
                                 a.WarehouseCode
                             } into gr
                             select new
                             {
                                 ProductCode = gr.Key.ProductCode,
                                 ProductOriginCode = gr.Key.ProductOriginCode,
                                 ProductLotCode = gr.Key.ProductLotCode,
                                 WarehouseCode = gr.Key.WarehouseCode,
                                 Quantity = gr.Sum(p => p.Quantity),
                                 Amount = gr.Sum(p => p.Amount),
                                 AmountCur = gr.Sum(p => p.AmountCur)
                             }).ToList();
            //TỔNG PHÁT SINH NHẬP XUẤT TRONG KỲ

            var wareHouseBookTmp = (from a in lstWareHouseBook
                                    join b in lstVoucherCategory on a.VoucherCode equals b.Code
                                    join pr in resulProduct on a.ProductCode equals pr.ProductCode
                                    where a.VoucherGroup != 4
                                    select new
                                    {
                                        a.OrgCode,
                                        a.ProductVoucherId,
                                        a.Id,
                                        SttRec = a.ProductVoucherId,
                                        a.Ord0,
                                        a.VoucherCode,
                                        a.VoucherGroup,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        a.BusinessCode,
                                        a.DebitAcc,
                                        a.CreditAcc,
                                        WarehouseCode = a.WarehouseCode ?? "",
                                        TransWarehouseCode = a.TransWarehouseCode ?? "",
                                        a.ProductCode,
                                        ProductLotCode = a.ProductLotCode ?? "",
                                        ProductOriginCode = a.ProductOriginCode ?? "",
                                        a.UnitCode,
                                        TrxQuantity = a.TrxQuantity != null ? a.TrxQuantity : a.Quantity,
                                        a.Quantity,
                                        PriceCb = a.Price,
                                        PriceCurCb = a.PriceCur,
                                        a.Price,
                                        a.PriceCur,
                                        a.Amount,
                                        a.AmountCur,
                                        a.ImportQuantity,
                                        a.ImportAmount,
                                        a.ImportAmountCur,
                                        a.ExportQuantity,
                                        a.ExportAmount,
                                        a.ExportAmountCur,
                                        b.PriceCalculatingMethod,
                                        b.IsTransfer,//dc_nb
                                        b.IsAssembly,
                                        a.FixedPrice,
                                        GetPrice = b.PriceCalculatingMethod == "B" ? "C" : "K",
                                        OnTop = (b.PriceCalculatingMethod == "B" && (b.IsAssembly == "C" || b.Code == "PCH") && a.VoucherCode == "1") ? "A1" : (b.PriceCalculatingMethod == "B" && (b.IsAssembly == "C" || b.Code == "PCH") && a.VoucherCode == "2") ? "A2" : "C",
                                        AssemblyTransfer = (b.PriceCalculatingMethod == "B") ? "B2" : "B1",
                                        CreationTime = a.CreationTime,
                                        ImportAvg = (b.PriceCalculatingMethod == "B" && (a.VoucherGroup == 1 && (b.IsTransfer == "C" || b.IsAssembly == "C" || a.VoucherCode.Contains(listCt) == true))) ? "C" : "K"

                                    }).OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).ThenByDescending(p => p.VoucherGroup).AsEnumerable();
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.ProductCode == dto.ProductCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.ProductLotCode == dto.ProductLotCode);
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.ProductOriginCode == dto.ProductOriginCode);
            }
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.WarehouseCode == dto.WareHouseCode);
            }
            var productb1 = (from a in wareHouseBookTmp
                             where a.PriceCalculatingMethod == "B" && a.AssemblyTransfer == "B2"
                             group new
                             {
                                 a.ProductCode,
                                 a.ProductLotCode,
                                 a.ProductOriginCode,
                                 a.WarehouseCode,
                                 a.ImportAvg
                             } by new
                             {
                                 a.ProductCode,
                                 a.ProductLotCode,
                                 a.ProductOriginCode,
                                 a.WarehouseCode
                             } into gr
                             where gr.Min(p => p.ImportAvg) == "K"
                             select new
                             {
                                 ProductCode = gr.Key.ProductCode,
                                 ProductLotCode = gr.Key.ProductLotCode,
                                 ProductOriginCode = gr.Key.ProductOriginCode,
                                 WarehouseCode = gr.Key.WarehouseCode
                             }).ToList();
            var productCode = "";
            var productLotCode = "";
            var productOriginCode = "";
            var warehouseCode = "";
            var lstWareHouseBookTmp = wareHouseBookTmp.ToList();
            while (productb1.Count > 0)
            {
                productCode = productb1.FirstOrDefault().ProductCode;
                productLotCode = productb1.FirstOrDefault().ProductLotCode;
                productOriginCode = productb1.FirstOrDefault().ProductOriginCode;
                warehouseCode = productb1.FirstOrDefault().WarehouseCode;
                var wareHouseBookTmpB1 = (from a in lstWareHouseBookTmp
                                          join b in productb1 on new
                                          {
                                              a.ProductCode,
                                              a.ProductLotCode,
                                              a.ProductOriginCode,
                                              a.WarehouseCode
                                          } equals new
                                          {
                                              b.ProductCode,
                                              b.ProductLotCode,
                                              b.ProductOriginCode,
                                              b.WarehouseCode
                                          }
                                          where a.GetPrice == "C"
                                          select new
                                          {
                                              a.Id,
                                              a.ProductOriginCode,
                                              a.ProductCode,
                                              a.ProductLotCode,
                                              a.WarehouseCode,
                                              a.Ord0,
                                              a.VoucherGroup,
                                              a.VoucherCode,
                                              QuantityCb = a.TrxQuantity,
                                              a.Quantity,
                                              a.IsTransfer,
                                              a.PriceCalculatingMethod,
                                              Amount = a.ExportAmount + a.ImportAmount,
                                              AmountCur = a.ExportAmountCur + a.ImportAmountCur,
                                              a.CreationTime,
                                              a.VoucherDate
                                          }).OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).ToList();
                //var id = "";
                while (wareHouseBookTmpB1.Count > 0)
                {
                    //var productOriginCode = "";
                    //var productCode = "";
                    //var productLotCode = "";
                    //var warehouseCode = "";
                    var ord0 = "";
                    int voucherGroup;
                    var voucherCode = "";
                    decimal? quantityCb;
                    decimal? quantity;
                    var isTransfer = wareHouseBookTmpB1.OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).FirstOrDefault().IsTransfer;
                    var priceCalculatingMethod = "";
                    decimal? exportAmount;
                    decimal? exportAmountCur;
                    DateTime? creationTime;
                    decimal? importQuantity;
                    decimal? importAmountCur;
                    decimal? importAmount;
                    decimal? price;
                    decimal? priceCur;
                    decimal? priceCb;
                    decimal? priceCbCur;
                    var ordRec = "";
                    decimal? amount;
                    decimal? amountCur;
                    DateTime voucherDate = wareHouseBookTmpB1.OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).FirstOrDefault().VoucherDate;
                    var wareHouseBookTmpB1s = wareHouseBookTmpB1.OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).FirstOrDefault();
                    //id = wareHouseBookTmpB1s.Id;
                    productOriginCode = wareHouseBookTmpB1s.ProductOriginCode;
                    productCode = wareHouseBookTmpB1s.ProductCode;
                    productLotCode = wareHouseBookTmpB1s.ProductLotCode;
                    warehouseCode = wareHouseBookTmpB1s.WarehouseCode;
                    //ord0 = wareHouseBookTmpB1s.Ord0;
                    voucherGroup = wareHouseBookTmpB1s.VoucherGroup;
                    // voucherCode = wareHouseBookTmpB1s.VoucherCode;

                    //isTransfer = wareHouseBookTmpB1s.IsTransfer;
                    priceCalculatingMethod = wareHouseBookTmpB1s.PriceCalculatingMethod;

                    creationTime = wareHouseBookTmpB1s.CreationTime;
                    var importExport0s = importExport0;
                    if (priceCalculatingMethod == "B")
                    {
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            importExport0s = importExport0.Where(p => p.ProductCode == productCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productLotCode))
                        {
                            importExport0s = importExport0.Where(p => p.ProductLotCode == productLotCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productOriginCode))
                        {
                            importExport0s = importExport0.Where(p => p.ProductOriginCode == productOriginCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(warehouseCode))
                        {
                            importExport0s = importExport0.Where(p => p.WarehouseCode == warehouseCode).ToList();
                        }
                        importQuantity = 0;
                        importAmount = 0;
                        importAmountCur = 0;
                        if (importExport0s.Count > 0)
                        {
                            importQuantity = importExport0s.FirstOrDefault().Quantity;
                            importAmount = importExport0s.FirstOrDefault().Amount;
                            importAmountCur = importExport0s.FirstOrDefault().AmountCur;
                        }
                        var resulwareHouseBookTmp = lstWareHouseBookTmp;
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.ProductCode == productCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productLotCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.ProductLotCode == productLotCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productOriginCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.ProductOriginCode == productOriginCode).ToList();

                        }
                        if (!string.IsNullOrEmpty(warehouseCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.WarehouseCode == warehouseCode).ToList();
                        }

                        resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.VoucherDate < voucherDate).ToList();
                        importQuantity = importQuantity + resulwareHouseBookTmp.Sum(p => p.ImportQuantity) - resulwareHouseBookTmp.Sum(p => p.ExportQuantity);
                        importAmount = importAmount + resulwareHouseBookTmp.Sum(p => p.ImportAmount) - resulwareHouseBookTmp.Sum(p => p.ExportAmount);
                        importAmountCur = importAmountCur + resulwareHouseBookTmp.Sum(p => p.ImportAmountCur) - resulwareHouseBookTmp.Sum(p => p.ExportAmountCur);


                        //exportAmount = wareHouseBookTmpB1s.Amount;
                        //exportAmountCur = wareHouseBookTmpB1s.AmountCur;
                        quantityCb = wareHouseBookTmpB1s.QuantityCb;
                        quantity = wareHouseBookTmpB1s.Quantity;
                        price = 0;
                        priceCur = 0;
                        priceCb = 0;
                        priceCbCur = 0;
                        exportAmount = 0;
                        exportAmountCur = 0;
                        if (importQuantity != 0)
                        {
                            if (importQuantity != quantity)
                            {
                                exportAmount = importAmount / importQuantity * quantity;
                                exportAmountCur = importAmountCur / importQuantity * quantity;
                            }
                            else
                            {
                                exportAmount = importAmount;
                                exportAmountCur = importAmountCur;
                            }
                            if (quantity != 0)
                            {
                                price = exportAmount / quantity;
                                priceCur = exportAmountCur / quantity;
                            }

                            if (quantityCb != 0)
                            {
                                priceCb = exportAmount / quantityCb;
                                priceCbCur = exportAmountCur / quantityCb;
                            }
                        }

                        if (voucherGroup == 2)
                        {
                            var resul = from a in lstWareHouseBookTmp
                                        where a.Ord0 == wareHouseBookTmpB1s.Ord0 && a.Id == wareHouseBookTmpB1s.Id
                                        select new
                                        {
                                            a.OrgCode,
                                            a.ProductVoucherId,
                                            a.Id,
                                            SttRec = a.ProductVoucherId,
                                            a.Ord0,
                                            a.VoucherCode,
                                            a.VoucherGroup,
                                            a.VoucherDate,
                                            a.VoucherNumber,
                                            a.BusinessCode,
                                            a.DebitAcc,
                                            a.CreditAcc,
                                            WarehouseCode = a.WarehouseCode,
                                            TransWarehouseCode = a.TransWarehouseCode,
                                            a.ProductCode,
                                            ProductLotCode = a.ProductLotCode,
                                            ProductOriginCode = a.ProductOriginCode,
                                            a.UnitCode,
                                            TrxQuantity = a.TrxQuantity,
                                            a.Quantity,
                                            PriceCb = priceCb,
                                            PriceCurCb = priceCbCur,
                                            Price = price,
                                            PriceCur = priceCur,
                                            Amount = exportAmount,
                                            AmountCur = exportAmountCur,
                                            a.ImportQuantity,
                                            a.ImportAmount,
                                            a.ImportAmountCur,
                                            a.ExportQuantity,
                                            ExportAmount = exportAmount,
                                            ExportAmountCur = exportAmountCur,
                                            PriceCalculatingMethod = "T",
                                            a.IsTransfer,//dc_nb
                                            IsAssembly = a.IsAssembly,
                                            a.FixedPrice,
                                            GetPrice = a.GetPrice,
                                            OnTop = "B",
                                            AssemblyTransfer = "B1",//lr_dc
                                            CreationTime = a.CreationTime,
                                            ImportAvg = "K"

                                        };
                            lstWareHouseBookTmp = (from a in lstWareHouseBookTmp
                                                   join b in resul on a.Id equals b.Id into c
                                                   from d in c.DefaultIfEmpty()
                                                   select new
                                                   {
                                                       a.OrgCode,
                                                       a.ProductVoucherId,
                                                       a.Id,
                                                       SttRec = a.ProductVoucherId,
                                                       a.Ord0,
                                                       a.VoucherCode,
                                                       a.VoucherGroup,
                                                       a.VoucherDate,
                                                       a.VoucherNumber,
                                                       a.BusinessCode,
                                                       a.DebitAcc,
                                                       a.CreditAcc,
                                                       WarehouseCode = a.WarehouseCode ?? "",
                                                       TransWarehouseCode = a.TransWarehouseCode ?? "",
                                                       a.ProductCode,
                                                       ProductLotCode = a.ProductLotCode ?? "",
                                                       ProductOriginCode = a.ProductOriginCode ?? "",
                                                       a.UnitCode,
                                                       TrxQuantity = a.TrxQuantity,
                                                       a.Quantity,
                                                       PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                       PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                       Price = d != null ? d.Price : a.Price,
                                                       PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                       Amount = d != null ? d.Amount : a.Amount,
                                                       AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                       a.ImportQuantity,
                                                       a.ImportAmount,
                                                       a.ImportAmountCur,
                                                       a.ExportQuantity,
                                                       ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                                       ExportAmountCur = d != null ? d.ExportAmountCur : a.ExportAmountCur,
                                                       PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                       a.IsTransfer,//dc_nb
                                                       IsAssembly = d != null ? d.IsAssembly : a.IsAssembly,
                                                       a.FixedPrice,
                                                       GetPrice = a.GetPrice,
                                                       OnTop = d != null ? d.OnTop : a.OnTop,
                                                       AssemblyTransfer = a.AssemblyTransfer,
                                                       CreationTime = a.CreationTime,
                                                       ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                                                   }).ToList();
                            if (isTransfer == "C")
                            {
                                var id = lstWareHouseBookTmp.Where(p => p.Id == wareHouseBookTmpB1s.Id).FirstOrDefault().ProductVoucherId;
                                var resuls = from a in lstWareHouseBookTmp
                                             where a.ProductVoucherId == id && a.Ord0 == "B" + wareHouseBookTmpB1s.Ord0.Substring(1, 9)
                                             select new
                                             {
                                                 a.OrgCode,
                                                 a.ProductVoucherId,
                                                 a.Id,
                                                 SttRec = a.ProductVoucherId,
                                                 a.Ord0,
                                                 a.VoucherCode,
                                                 a.VoucherGroup,
                                                 a.VoucherDate,
                                                 a.VoucherNumber,
                                                 a.BusinessCode,
                                                 a.DebitAcc,
                                                 a.CreditAcc,
                                                 WarehouseCode = a.WarehouseCode,
                                                 TransWarehouseCode = a.TransWarehouseCode,
                                                 a.ProductCode,
                                                 ProductLotCode = a.ProductLotCode,
                                                 ProductOriginCode = a.ProductOriginCode,
                                                 a.UnitCode,
                                                 TrxQuantity = a.TrxQuantity,
                                                 a.Quantity,
                                                 PriceCb = priceCb,
                                                 PriceCurCb = priceCbCur,
                                                 Price = price,
                                                 PriceCur = priceCur,
                                                 Amount = exportAmount,
                                                 AmountCur = exportAmountCur,
                                                 a.ImportQuantity,
                                                 ImportAmount = exportAmount,
                                                 ImportAmountCur = exportAmountCur,
                                                 a.ExportQuantity,
                                                 ExportAmount = a.ExportAmount,
                                                 ExportAmountCur = a.ExportAmountCur,
                                                 PriceCalculatingMethod = "T",
                                                 a.IsTransfer,//dc_nb
                                                 IsAssembly = a.IsAssembly,
                                                 a.FixedPrice,
                                                 GetPrice = a.GetPrice,
                                                 OnTop = "B",
                                                 AssemblyTransfer = "B1",
                                                 CreationTime = a.CreationTime,
                                                 ImportAvg = "K"
                                             };
                                lstWareHouseBookTmp = (from a in lstWareHouseBookTmp
                                                       join b in resuls on a.Id equals b.Id into c
                                                       from d in c.DefaultIfEmpty()
                                                       select new
                                                       {
                                                           a.OrgCode,
                                                           a.ProductVoucherId,
                                                           a.Id,
                                                           SttRec = a.ProductVoucherId,
                                                           a.Ord0,
                                                           a.VoucherCode,
                                                           a.VoucherGroup,
                                                           a.VoucherDate,
                                                           a.VoucherNumber,
                                                           a.BusinessCode,
                                                           a.DebitAcc,
                                                           a.CreditAcc,
                                                           WarehouseCode = a.WarehouseCode ?? "",
                                                           TransWarehouseCode = a.TransWarehouseCode ?? "",
                                                           a.ProductCode,
                                                           ProductLotCode = a.ProductLotCode ?? "",
                                                           ProductOriginCode = a.ProductOriginCode ?? "",
                                                           a.UnitCode,
                                                           TrxQuantity = a.TrxQuantity,
                                                           a.Quantity,
                                                           PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                           PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                           Price = d != null ? d.Price : a.Price,
                                                           PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                           Amount = d != null ? d.Amount : a.Amount,
                                                           AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                           a.ImportQuantity,
                                                           ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                           ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                           a.ExportQuantity,
                                                           ExportAmount = a.ExportAmount,
                                                           ExportAmountCur = a.ExportAmountCur,
                                                           PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                           a.IsTransfer,//dc_nb
                                                           IsAssembly = d != null ? d.IsAssembly : a.IsAssembly,
                                                           a.FixedPrice,
                                                           GetPrice = a.GetPrice,
                                                           OnTop = d != null ? d.OnTop : a.OnTop,
                                                           AssemblyTransfer = a.AssemblyTransfer,
                                                           CreationTime = a.CreationTime,
                                                           ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                                                       }).ToList();
                            }
                        }
                        else
                        {
                            var resul = (from a in lstWareHouseBookTmp
                                         where a.Ord0 == wareHouseBookTmpB1s.Ord0 && a.Id == wareHouseBookTmpB1s.Id
                                         select new
                                         {
                                             a.OrgCode,
                                             a.ProductVoucherId,
                                             a.Id,
                                             SttRec = a.ProductVoucherId,
                                             a.Ord0,
                                             a.VoucherCode,
                                             a.VoucherGroup,
                                             a.VoucherDate,
                                             a.VoucherNumber,
                                             a.BusinessCode,
                                             a.DebitAcc,
                                             a.CreditAcc,
                                             WarehouseCode = a.WarehouseCode,
                                             TransWarehouseCode = a.TransWarehouseCode,
                                             a.ProductCode,
                                             ProductLotCode = a.ProductLotCode,
                                             ProductOriginCode = a.ProductOriginCode,
                                             a.UnitCode,
                                             TrxQuantity = a.TrxQuantity,
                                             a.Quantity,
                                             PriceCb = priceCb,
                                             PriceCurCb = priceCbCur,
                                             Price = price,
                                             PriceCur = priceCur,
                                             Amount = exportAmount,
                                             AmountCur = exportAmountCur,
                                             a.ImportQuantity,
                                             ImportAmount = exportAmount,
                                             ImportAmountCur = exportAmountCur,
                                             a.ExportQuantity,
                                             ExportAmount = a.ExportAmount,
                                             ExportAmountCur = a.ExportAmountCur,
                                             PriceCalculatingMethod = "T",
                                             a.IsTransfer,//dc_nb
                                             IsAssembly = "B1",
                                             a.FixedPrice,
                                             GetPrice = a.GetPrice,
                                             OnTop = "B",
                                             AssemblyTransfer = a.AssemblyTransfer,
                                             CreationTime = a.CreationTime,
                                             ImportAvg = "K"

                                         }).ToList();
                            lstWareHouseBookTmp = (from a in lstWareHouseBookTmp
                                                   join b in resul on a.Id equals b.Id into c
                                                   from d in c.DefaultIfEmpty()
                                                   select new
                                                   {
                                                       a.OrgCode,
                                                       a.ProductVoucherId,
                                                       a.Id,
                                                       SttRec = a.ProductVoucherId,
                                                       a.Ord0,
                                                       a.VoucherCode,
                                                       a.VoucherGroup,
                                                       a.VoucherDate,
                                                       a.VoucherNumber,
                                                       a.BusinessCode,
                                                       a.DebitAcc,
                                                       a.CreditAcc,
                                                       WarehouseCode = a.WarehouseCode ?? "",
                                                       TransWarehouseCode = a.TransWarehouseCode ?? "",
                                                       a.ProductCode,
                                                       ProductLotCode = a.ProductLotCode ?? "",
                                                       ProductOriginCode = a.ProductOriginCode ?? "",
                                                       a.UnitCode,
                                                       TrxQuantity = a.TrxQuantity,
                                                       a.Quantity,
                                                       PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                       PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                       Price = d != null ? d.Price : a.Price,
                                                       PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                       Amount = d != null ? d.Amount : a.Amount,
                                                       AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                       a.ImportQuantity,
                                                       ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                       ImportAmountCur = d != null ? d.ImportAmount : a.ImportAmount,
                                                       a.ExportQuantity,
                                                       ExportAmount = a.ExportAmount,
                                                       ExportAmountCur = a.ExportAmountCur,
                                                       PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                       a.IsTransfer,//dc_nb
                                                       IsAssembly = d != null ? d.IsAssembly : a.IsAssembly,
                                                       a.FixedPrice,
                                                       GetPrice = a.GetPrice,
                                                       OnTop = d != null ? d.OnTop : a.OnTop,
                                                       AssemblyTransfer = a.AssemblyTransfer,
                                                       CreationTime = a.CreationTime,
                                                       ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                                                   }).ToList();
                        }
                    }
                    var itemToRemove = wareHouseBookTmpB1.Single(r => r.Id == wareHouseBookTmpB1s.Id);
                    wareHouseBookTmpB1.Remove(itemToRemove);
                }

                var itemToRemovem = productb1.Single(r => r.ProductCode == productCode && r.ProductLotCode == productLotCode && r.ProductOriginCode == productOriginCode && r.WarehouseCode == warehouseCode);
                productb1.Remove(itemToRemovem);

                //Trường hợp có xuất lắp ráp và tách vật tư xuất theo 1 chiều
                var fixIssasembly = (from a in lstWareHouseBookTmp
                                     join b in productb1 on new
                                     {
                                         a.WarehouseCode,
                                         a.ProductCode,
                                         a.ProductLotCode,
                                         a.ProductOriginCode
                                     } equals new
                                     {
                                         b.WarehouseCode,
                                         b.ProductCode,
                                         b.ProductLotCode,
                                         b.ProductOriginCode
                                     }
                                     where a.VoucherGroup == 2 && a.IsAssembly == "C"
                                     group new
                                     {
                                         a.ProductVoucherId,
                                         a.VoucherCode,
                                         a.PriceCalculatingMethod
                                     } by new
                                     {
                                         a.ProductVoucherId,
                                         a.VoucherCode
                                     } into gr
                                     where gr.Min(p => p.PriceCalculatingMethod) == "T"
                                     select new
                                     {
                                         ProductVoucherId = gr.Key.ProductVoucherId,
                                         VoucherCode = gr.Key.VoucherCode
                                     }).ToList();
                //while (fixIssasembly.Count > 0)
                //{
                //    var productOriginCode = "";
                //    var productCode = "";
                //    var productLotCode = "";
                //    var warehouseCode = "";
                //    var ord0 = "";
                //    int voucherGroup;
                //    var voucherCode = "";
                //    decimal? quantityCb;
                //    decimal? quantity;
                //    var isTransfer = "";
                //    var priceCalculatingMethod = "";
                //    decimal? exportAmount;
                //    decimal? exportAmountCur;
                //    DateTime? creationTime;
                //    decimal? importQuantity;
                //    decimal? importAmountCur;
                //    decimal? importAmount;
                //    decimal? price;
                //    decimal? priceCur;
                //    decimal? priceCb;
                //    decimal? priceCbCur;
                //    var ordRec = "";
                //    decimal? amount;
                //    decimal? amountCur;
                //    ordRec = fixIssasembly.FirstOrDefault().ProductVoucherId;
                //    voucherCode = fixIssasembly.FirstOrDefault().VoucherCode;
                //    exportAmount = wareHouseBookTmp.Where(p => p.ProductVoucherId == ordRec && p.VoucherGroup == 2).Sum(p => p.ExportAmount);
                //    exportAmountCur = wareHouseBookTmp.Where(p => p.ProductVoucherId == ordRec && p.VoucherGroup == 2).Sum(p => p.ExportAmountCur);
                //    var detaiDetail = from a in wareHouseBookTmp.ToList()
                //                      join b in lstproductUnit on new
                //                      {
                //                          a.OrgCode,
                //                          a.ProductCode,
                //                          a.UnitCode
                //                      } equals new
                //                      {
                //                          b.OrgCode,
                //                          b.ProductCode,
                //                          b.UnitCode
                //                      }
                //                      where a.ProductVoucherId == ordRec && a.VoucherGroup == 1
                //                      select new
                //                      {
                //                          a.Ord0,
                //                          a.ProductCode,
                //                          a.TrxQuantity,
                //                          a.Quantity,
                //                          b.PurchasePrice,
                //                          b.PurchasePriceCur,
                //                          AmountPb = a.Quantity * b.PurchasePrice,
                //                          AmountCurPb = a.Quantity * b.PurchasePriceCur,
                //                          Exchanrate = (decimal)100,
                //                          ExchanRateCur = (decimal)100,
                //                          AmountPb0 = a.Quantity * b.PurchasePrice,
                //                          AmountPbCur0 = a.Quantity * b.PurchasePriceCur
                //                      };
                //    importAmount = detaiDetail.Sum(p => p.AmountPb);
                //    importAmountCur = detaiDetail.Sum(p => p.AmountCurPb);
                //    if (importAmount == 0)
                //    {
                //        detaiDetail = from a in detaiDetail
                //                      select new
                //                      {
                //                          a.Ord0,
                //                          a.ProductCode,
                //                          a.TrxQuantity,
                //                          a.Quantity,
                //                          a.PurchasePrice,
                //                          a.PurchasePriceCur,
                //                          AmountPb = a.Quantity,
                //                          AmountCurPb = a.Quantity,
                //                          Exchanrate = a.Exchanrate,
                //                          ExchanRateCur = a.ExchanRateCur,
                //                          AmountPb0 = a.Quantity,
                //                          AmountPbCur0 = a.Quantity
                //                      };
                //        importAmount = detaiDetail.Sum(p => p.AmountPb);
                //        importAmountCur = detaiDetail.Sum(p => p.AmountCurPb);
                //    }
                //    if (importAmount != 0)
                //    {
                //        detaiDetail = from a in detaiDetail
                //                      select new
                //                      {
                //                          a.Ord0,
                //                          a.ProductCode,
                //                          a.TrxQuantity,
                //                          a.Quantity,
                //                          a.PurchasePrice,
                //                          a.PurchasePriceCur,
                //                          AmountPb = a.Quantity,
                //                          AmountCurPb = a.Quantity,
                //                          Exchanrate = (decimal)(a.AmountPb / importAmount),
                //                          ExchanRateCur = a.ExchanRateCur,
                //                          AmountPb0 = (decimal?)(a.AmountPb / importAmount),
                //                          AmountPbCur0 = a.Quantity

                //                      };
                //    }
                //    if (importAmountCur != 0)
                //    {

                //        detaiDetail = from a in detaiDetail
                //                      select new
                //                      {
                //                          a.Ord0,
                //                          a.ProductCode,
                //                          a.TrxQuantity,
                //                          a.Quantity,
                //                          a.PurchasePrice,
                //                          a.PurchasePriceCur,
                //                          AmountPb = a.Quantity,
                //                          AmountCurPb = a.Quantity,
                //                          Exchanrate = a.Exchanrate,
                //                          ExchanRateCur = (decimal)(a.AmountCurPb / importAmountCur),
                //                          AmountPb0 = a.AmountPb0,
                //                          AmountPbCur0 = (decimal?)(a.AmountCurPb / importAmountCur)

                //                      };
                //    }
                //    var detaiDetails = from a in detaiDetail
                //                       where a.Quantity != 0
                //                       select new
                //                       {
                //                           a.Ord0,
                //                           a.ProductCode,
                //                           a.TrxQuantity,
                //                           a.Quantity,
                //                           PurchasePrice = a.AmountPb0 / a.Quantity,
                //                           PurchasePriceCur = a.AmountPbCur0 / a.Quantity,
                //                           AmountPb = a.Quantity,
                //                           AmountCurPb = a.Quantity,
                //                           Exchanrate = a.Exchanrate,
                //                           ExchanRateCur = (decimal)(a.AmountCurPb / importAmountCur),
                //                           AmountPb0 = a.AmountPb0,
                //                           AmountPbCur0 = (decimal?)(a.AmountCurPb / importAmountCur)
                //                       };
                //    detaiDetail = from a in detaiDetail
                //                  join b in detaiDetails on a.ProductCode equals b.ProductCode into c
                //                  from d in c.DefaultIfEmpty()
                //                  select new
                //                  {
                //                      a.Ord0,
                //                      a.ProductCode,
                //                      a.TrxQuantity,
                //                      a.Quantity,
                //                      PurchasePrice = (decimal)(d != null ? d.PurchasePrice : a.PurchasePrice),
                //                      PurchasePriceCur = (decimal)(d != null ? d.PurchasePriceCur : a.PurchasePriceCur),
                //                      AmountPb = a.Quantity,
                //                      AmountCurPb = a.Quantity,
                //                      Exchanrate = a.Exchanrate,
                //                      ExchanRateCur = a.ExchanRateCur,
                //                      AmountPb0 = a.AmountPb0,
                //                      AmountPbCur0 = a.AmountPbCur0
                //                  };
                //    var productVoucher = await _productVoucherService.GetQueryableAsync();
                //    productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Id == ordRec);
                //    var productvoucherAss = await _productVoucherAssembly.GetQueryableAsync();
                //    productvoucherAss = productvoucherAss.Where(p => p.ProductVoucherId == ordRec && p.Quantity != 0);
                //    amount = 0;
                //    amountCur = 0;
                //    foreach (var item in productvoucherAss)
                //    {
                //        item.Price = exportAmount / item.Quantity;
                //        item.PriceCur = exportAmountCur / item.Quantity;
                //        item.Amount = exportAmount;
                //        item.AmountCur = exportAmountCur;
                //        await _productVoucherAssembly.UpdateAsync(item);
                //    }
                //    if (voucherCode.Contains(listCtLrs) == false)//-- Phieu tach vat tu
                //    {
                //        amount = detaiDetail.Sum(p => p.AmountPb0);
                //        amountCur = detaiDetail.Sum(p => p.AmountPbCur0);
                //        ord0 = detaiDetail.OrderBy(p => p.Ord0).FirstOrDefault().Ord0;
                //        if (exportAmount != amount)
                //        {
                //            var detail = from a in detaiDetail
                //                         where a.Ord0 == ord0
                //                         select new
                //                         {
                //                             a.Ord0,
                //                             a.ProductCode,
                //                             a.TrxQuantity,
                //                             a.Quantity,
                //                             PurchasePrice = a.PurchasePrice,
                //                             PurchasePriceCur = a.PurchasePriceCur,
                //                             AmountPb = a.Quantity,
                //                             AmountCurPb = a.Quantity,
                //                             Exchanrate = a.Exchanrate,
                //                             ExchanRateCur = a.ExchanRateCur,
                //                             AmountPb0 = a.AmountPb0 + exportAmount - amount,
                //                             AmountPbCur0 = a.AmountPbCur0
                //                         };
                //            detaiDetail = from a in detaiDetail
                //                          join b in detail on a.Ord0 equals b.Ord0 into c
                //                          from d in c.DefaultIfEmpty()
                //                          select new
                //                          {
                //                              a.Ord0,
                //                              a.ProductCode,
                //                              a.TrxQuantity,
                //                              a.Quantity,
                //                              PurchasePrice = a.PurchasePrice,
                //                              PurchasePriceCur = a.PurchasePriceCur,
                //                              AmountPb = a.Quantity,
                //                              AmountCurPb = a.Quantity,
                //                              Exchanrate = a.Exchanrate,
                //                              ExchanRateCur = a.ExchanRateCur,
                //                              AmountPb0 = d != null ? d.AmountPb0 : a.AmountPb0,
                //                              AmountPbCur0 = a.AmountPbCur0
                //                          };
                //        }
                //        if (exportAmountCur != amountCur)
                //        {
                //            var detail = from a in detaiDetail
                //                         where a.Ord0 == ord0
                //                         select new
                //                         {
                //                             a.Ord0,
                //                             a.ProductCode,
                //                             a.TrxQuantity,
                //                             a.Quantity,
                //                             PurchasePrice = a.PurchasePrice,
                //                             PurchasePriceCur = a.PurchasePriceCur,
                //                             AmountPb = a.Quantity,
                //                             AmountCurPb = a.Quantity,
                //                             Exchanrate = a.Exchanrate,
                //                             ExchanRateCur = a.ExchanRateCur,
                //                             AmountPb0 = a.AmountPb0,
                //                             AmountPbCur0 = a.AmountPbCur0 + exportAmountCur - amountCur
                //                         };
                //            detaiDetail = from a in detaiDetail
                //                          join b in detail on a.Ord0 equals b.Ord0 into c
                //                          from d in c.DefaultIfEmpty()
                //                          select new
                //                          {
                //                              a.Ord0,
                //                              a.ProductCode,
                //                              a.TrxQuantity,
                //                              a.Quantity,
                //                              PurchasePrice = a.PurchasePrice,
                //                              PurchasePriceCur = a.PurchasePriceCur,
                //                              AmountPb = a.Quantity,
                //                              AmountCurPb = a.Quantity,
                //                              Exchanrate = a.Exchanrate,
                //                              ExchanRateCur = a.ExchanRateCur,
                //                              AmountPb0 = a.AmountPb0,
                //                              AmountPbCur0 = d != null ? d.AmountPbCur0 : a.AmountPbCur0
                //                          };
                //        }
                //        //Update phan chi tiet phieu	
                //        while (detaiDetail.ToList().Count > 0)
                //        {
                //            var resulDetail = detaiDetail.Where(p => p.Ord0 == ord0).OrderBy(p => p.Ord0).FirstOrDefault();
                //            ord0 = resulDetail.Ord0;
                //            price = resulDetail.PurchasePrice;
                //            priceCur = resulDetail.PurchasePriceCur;
                //            amount = resulDetail.AmountPb0;
                //            amountCur = resulDetail.AmountPbCur0;
                //            var resulwareHouseBookTmp = from a in wareHouseBookTmp
                //                                        where a.ProductVoucherId == ordRec && ord0 == ord0
                //                                        select new
                //                                        {
                //                                            a.ProductVoucherId,
                //                                            a.Ord0,
                //                                            PriceCalculatingMethod = "T",
                //                                            ImportAvg = "K",
                //                                            IsAssembly = "B1",
                //                                            PriceCur = priceCur,
                //                                            Price = price,
                //                                            PriceCurCb = priceCur,
                //                                            PriceCb = price,
                //                                            Amount = amount,
                //                                            AmountCur = amountCur,
                //                                            ImportAmount = amount,
                //                                            importAmountCur = amountCur
                //                                        };
                //            wareHouseBookTmp = from a in wareHouseBookTmp
                //                               join b in resulwareHouseBookTmp on new
                //                               {
                //                                   a.ProductVoucherId,
                //                                   a.Ord0
                //                               } equals new
                //                               {
                //                                   b.ProductVoucherId,
                //                                   b.Ord0
                //                               } into c
                //                               from d in c.DefaultIfEmpty()
                //                               select new
                //                               {
                //                                   a.OrgCode,
                //                                   a.ProductVoucherId,
                //                                   a.Id,
                //                                   SttRec = a.ProductVoucherId,
                //                                   a.Ord0,
                //                                   a.VoucherCode,
                //                                   a.VoucherGroup,
                //                                   a.VoucherDate,
                //                                   a.VoucherNumber,
                //                                   a.BusinessCode,
                //                                   a.DebitAcc,
                //                                   a.CreditAcc,
                //                                   WarehouseCode = a.WarehouseCode ?? "",
                //                                   TransWarehouseCode = a.TransWarehouseCode ?? "",
                //                                   a.ProductCode,
                //                                   ProductLotCode = a.ProductLotCode ?? "",
                //                                   ProductOriginCode = a.ProductOriginCode ?? "",
                //                                   a.UnitCode,
                //                                   TrxQuantity = a.TrxQuantity != null ? a.TrxQuantity : a.Quantity,
                //                                   a.Quantity,
                //                                   PriceCb = d != null ? d.Price : a.Price,
                //                                   PriceCurCb = d != null ? d.PriceCur : a.PriceCur,
                //                                   Price = d != null ? d.Price : a.Price,
                //                                   PriceCur = d != null ? d.PriceCur : a.PriceCur,
                //                                   Amount = d != null ? d.Amount : a.Amount,
                //                                   AmountCur = d != null ? d.AmountCur : a.AmountCur,
                //                                   a.ImportQuantity,
                //                                   a.ImportAmount,
                //                                   a.ImportAmountCur,
                //                                   a.ExportQuantity,
                //                                   a.ExportAmount,
                //                                   a.ExportAmountCur,
                //                                   PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                //                                   a.IsTransfer,//dc_nb
                //                                   IsAssembly = d != null ? d.IsAssembly : a.IsAssembly,
                //                                   a.FixedPrice,
                //                                   GetPrice = a.GetPrice,
                //                                   OnTop = a.OnTop,
                //                                   AssemblyTransfer = a.AssemblyTransfer,
                //                                   CreationTime = a.CreationTime,
                //                                   ImportAvg = d != null ? d.ImportAvg : a.ImportAvg

                //                               };
                //            var productDetail = await _productVoucherDetail.GetQueryableAsync();
                //            productDetail = productDetail.Where(p => p.ProductVoucherId == ordRec && p.Ord0 == ord0);
                //            foreach (var item in productDetail)
                //            {
                //                item.PriceCur = priceCur;
                //                item.Price = price;
                //                item.Amount = amount;
                //                item.AmountCur = amountCur;
                //                await _productVoucherDetail.UpdateAsync(item);
                //            }
                //            var itemToRemoves = productDetail.Single(r => r.Ord0 == ord0);
                //            productDetail.ToList().Remove(itemToRemoves);
                //        }

                //    }
                //    else
                //    {
                //        var resulwareHouseBookTmp = from a in wareHouseBookTmp
                //                                    where a.Ord0 == "B000000001" && a.ProductVoucherId == ordRec
                //                                    select new
                //                                    {
                //                                        a.OrgCode,
                //                                        a.ProductVoucherId,
                //                                        a.Id,
                //                                        SttRec = a.ProductVoucherId,
                //                                        a.Ord0,
                //                                        a.VoucherCode,
                //                                        a.VoucherGroup,
                //                                        a.VoucherDate,
                //                                        a.VoucherNumber,
                //                                        a.BusinessCode,
                //                                        a.DebitAcc,
                //                                        a.CreditAcc,
                //                                        WarehouseCode = a.WarehouseCode ?? "",
                //                                        TransWarehouseCode = a.TransWarehouseCode ?? "",
                //                                        a.ProductCode,
                //                                        ProductLotCode = a.ProductLotCode ?? "",
                //                                        ProductOriginCode = a.ProductOriginCode ?? "",
                //                                        a.UnitCode,
                //                                        TrxQuantity = a.TrxQuantity != null ? a.TrxQuantity : a.Quantity,
                //                                        a.Quantity,
                //                                        PriceCb = a.Quantity != 0 ? a.ExportAmount / a.Quantity : 0,
                //                                        PriceCurCb = a.Quantity != 0 ? a.ExportAmountCur / a.Quantity : 0,
                //                                        Price = a.Quantity != 0 ? a.ExportAmount / a.Quantity : 0,
                //                                        PriceCur = a.Quantity != 0 ? a.ExportAmountCur / a.Quantity : 0,
                //                                        Amount = exportAmount,
                //                                        AmountCur = exportAmountCur,
                //                                        a.ImportQuantity,
                //                                        ImportAmount = exportAmount,
                //                                        ImportAmountCur = exportAmountCur,
                //                                        a.ExportQuantity,
                //                                        a.ExportAmount,
                //                                        a.ExportAmountCur,
                //                                        PriceCalculatingMethod = "T",
                //                                        a.IsTransfer,//dc_nb
                //                                        a.IsAssembly,
                //                                        a.FixedPrice,
                //                                        GetPrice = a.GetPrice,
                //                                        OnTop = a.OnTop,
                //                                        AssemblyTransfer = "B1",
                //                                        CreationTime = a.CreationTime,
                //                                        ImportAvg = "K"

                //                                    };
                //        wareHouseBookTmp = from a in wareHouseBookTmp
                //                           join b in resulwareHouseBookTmp on new
                //                           {
                //                               a.Ord0,
                //                               a.ProductVoucherId
                //                           } equals new
                //                           {
                //                               b.Ord0,
                //                               b.ProductVoucherId
                //                           } into c
                //                           from d in c.DefaultIfEmpty()
                //                           select new
                //                           {
                //                               a.OrgCode,
                //                               a.ProductVoucherId,
                //                               a.Id,
                //                               SttRec = a.ProductVoucherId,
                //                               a.Ord0,
                //                               a.VoucherCode,
                //                               a.VoucherGroup,
                //                               a.VoucherDate,
                //                               a.VoucherNumber,
                //                               a.BusinessCode,
                //                               a.DebitAcc,
                //                               a.CreditAcc,
                //                               WarehouseCode = a.WarehouseCode,
                //                               TransWarehouseCode = a.TransWarehouseCode,
                //                               a.ProductCode,
                //                               ProductLotCode = a.ProductLotCode,
                //                               ProductOriginCode = a.ProductOriginCode,
                //                               a.UnitCode,
                //                               TrxQuantity = a.TrxQuantity,
                //                               a.Quantity,
                //                               PriceCb = d != null ? d.PriceCb : a.PriceCb,
                //                               PriceCurCb = d != null ? d.PriceCurCb : d.PriceCurCb,
                //                               Price = d != null ? d.Price : a.Price,
                //                               PriceCur = d != null ? d.PriceCur : a.PriceCur,
                //                               Amount = d != null ? d.Amount : a.Amount,
                //                               AmountCur = d != null ? d.AmountCur : a.AmountCur,
                //                               a.ImportQuantity,
                //                               ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                //                               ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                //                               a.ExportQuantity,
                //                               a.ExportAmount,
                //                               a.ExportAmountCur,
                //                               PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                //                               a.IsTransfer,//dc_nb
                //                               a.IsAssembly,
                //                               a.FixedPrice,
                //                               GetPrice = a.GetPrice,
                //                               OnTop = a.OnTop,
                //                               AssemblyTransfer = d != null ? d.AssemblyTransfer : a.AssemblyTransfer,
                //                               CreationTime = a.CreationTime,
                //                               ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                //                           };
                //    }
                //    detaiDetail = detaiDetail.Where(p => p.Ord0 == "1");
                //    var itemToRemove = fixIssasembly.Single(r => r.ProductVoucherId == ordRec);
                //    fixIssasembly.Remove(itemToRemove);

                //}
            }
            // Kiểm Tra xem có B2 không
            var id1 = "";

            var wareHouseBookTmpB2 = lstWareHouseBookTmp.Where(p => p.PriceCalculatingMethod == "B");
            if (wareHouseBookTmpB2.Count() > 0)
            {
                //-- Quét vòng 2
                var resulwareHouseBookTmpB2 = (from a in lstWareHouseBookTmp
                                               where a.PriceCalculatingMethod == "B"
                                               select new
                                               {
                                                   a.Id,
                                                   a.ProductVoucherId,
                                                   a.Ord0,
                                                   a.VoucherGroup,
                                                   a.VoucherCode,
                                                   QuantityCb = a.TrxQuantity,
                                                   a.Quantity,
                                                   a.WarehouseCode,
                                                   a.ProductCode,
                                                   a.ProductLotCode,
                                                   a.ProductOriginCode,
                                                   a.IsTransfer,
                                                   a.PriceCalculatingMethod,
                                                   ExportAmount = a.ExportAmount + a.ImportAmount,
                                                   ExportAmountCur = a.ExportAmountCur + a.ImportAmountCur,
                                                   a.CreationTime
                                               }).ToList();
                while (resulwareHouseBookTmpB2.Count() > 0)
                {

                    var ord0 = "";
                    int voucherGroup;
                    var voucherCode = "";
                    decimal? quantityCb;
                    decimal? quantity;
                    var isTransfer = "";
                    var priceCalculatingMethod = "";
                    decimal? exportAmount;
                    decimal? exportAmountCur;
                    DateTime? creationTime;
                    decimal? importQuantity;
                    decimal? importAmountCur;
                    decimal? importAmount;
                    decimal? price;
                    decimal? priceCur;
                    decimal? priceCb;
                    decimal? priceCbCur;
                    var ordRec = "";
                    decimal? amount;
                    decimal? amountCur;
                    var resulwareHouseBookTmpB2s = resulwareHouseBookTmpB2.OrderBy(p => p.CreationTime).FirstOrDefault();
                    id1 = resulwareHouseBookTmpB2s.Id;
                    warehouseCode = resulwareHouseBookTmpB2s.WarehouseCode;
                    ordRec = resulwareHouseBookTmpB2s.ProductVoucherId;
                    ord0 = resulwareHouseBookTmpB2s.Ord0;
                    voucherGroup = resulwareHouseBookTmpB2s.VoucherGroup;
                    voucherCode = resulwareHouseBookTmpB2s.VoucherCode;
                    quantityCb = resulwareHouseBookTmpB2s.QuantityCb;
                    quantity = resulwareHouseBookTmpB2s.Quantity;
                    productCode = resulwareHouseBookTmpB2s.ProductCode;
                    productLotCode = resulwareHouseBookTmpB2s.ProductLotCode;
                    productOriginCode = resulwareHouseBookTmpB2s.ProductOriginCode;
                    isTransfer = resulwareHouseBookTmpB2s.IsTransfer;
                    priceCalculatingMethod = resulwareHouseBookTmpB2s.PriceCalculatingMethod;
                    exportAmount = resulwareHouseBookTmpB2s.ExportAmount;
                    exportAmountCur = resulwareHouseBookTmpB2s.ExportAmountCur;
                    creationTime = resulwareHouseBookTmpB2s.CreationTime;

                    if (priceCalculatingMethod == "B")
                    {
                        importQuantity = 0;
                        importAmount = 0;
                        importAmountCur = 0;
                        price = 0;
                        priceCur = 0;
                        priceCb = 0;
                        priceCbCur = 0;
                        exportAmount = 0;
                        exportAmountCur = 0;
                        var importExport0s = importExport0;
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            importExport0s = importExport0.Where(p => p.ProductCode == productCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productLotCode))
                        {
                            importExport0s = importExport0.Where(p => p.ProductLotCode == productLotCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productOriginCode))
                        {
                            importExport0s = importExport0.Where(p => p.ProductOriginCode == productOriginCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(warehouseCode))
                        {
                            importExport0s = importExport0.Where(p => p.WarehouseCode == warehouseCode).ToList();
                        }
                        if (importExport0s.Count > 0)
                        {
                            importQuantity = importExport0s.FirstOrDefault().Quantity;
                            importAmount = importExport0s.FirstOrDefault().Amount;
                            importAmountCur = importExport0s.FirstOrDefault().AmountCur;
                        }
                        //Tinh nhap xuat den thoi diem hien tai
                        var resulwareHouseBookTmp = lstWareHouseBookTmp;
                        if (!string.IsNullOrEmpty(productCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.ProductCode == productCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productLotCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.ProductLotCode == productLotCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(productOriginCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.ProductOriginCode == productOriginCode).ToList();
                        }
                        if (!string.IsNullOrEmpty(warehouseCode))
                        {
                            resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.WarehouseCode == warehouseCode).ToList();
                        }
                        resulwareHouseBookTmp = resulwareHouseBookTmp.Where(p => p.CreationTime < creationTime).ToList();
                        importQuantity = importQuantity + resulwareHouseBookTmp.Sum(p => p.ImportQuantity) - resulwareHouseBookTmp.Sum(p => p.ExportQuantity);
                        importAmount = importAmount + resulwareHouseBookTmp.Sum(p => p.ImportAmount) - resulwareHouseBookTmp.Sum(p => p.ExportAmount);
                        importAmountCur = importAmountCur + resulwareHouseBookTmp.Sum(p => p.ImportAmountCur) - resulwareHouseBookTmp.Sum(p => p.ExportAmountCur);

                        if (importQuantity != 0)
                        {
                            if (importQuantity != quantity)
                            {
                                exportAmount = importAmount / importQuantity * quantity;
                                exportAmountCur = importAmountCur / importQuantity * quantity;
                            }
                            else
                            {
                                exportAmount = importAmount;
                                exportAmountCur = importAmountCur;
                            }
                            price = exportAmount / quantity;
                            priceCur = exportAmountCur / quantity;
                            if (quantityCb != 0)
                            {
                                priceCb = exportAmount / quantityCb;
                                priceCbCur = exportAmountCur / quantityCb;
                            }
                        }
                        if (voucherGroup == 2)
                        {
                            var wareHouseBookTmps = (from a in lstWareHouseBookTmp
                                                     where a.ProductVoucherId == ordRec && a.Ord0 == ord0
                                                     select new
                                                     {
                                                         a.OrgCode,
                                                         a.ProductVoucherId,
                                                         a.Id,
                                                         SttRec = a.SttRec,
                                                         a.Ord0,
                                                         a.VoucherCode,
                                                         a.VoucherGroup,
                                                         a.VoucherDate,
                                                         a.VoucherNumber,
                                                         a.BusinessCode,
                                                         a.DebitAcc,
                                                         a.CreditAcc,
                                                         a.WarehouseCode,
                                                         a.TransWarehouseCode,
                                                         a.ProductCode,
                                                         a.ProductLotCode,
                                                         a.ProductOriginCode,
                                                         a.UnitCode,
                                                         a.TrxQuantity,
                                                         a.Quantity,
                                                         PriceCb = priceCb,
                                                         PriceCurCb = priceCbCur,
                                                         Price = price,
                                                         PriceCur = priceCur,
                                                         Amount = exportAmount,
                                                         AmountCur = exportAmountCur,
                                                         a.ImportQuantity,
                                                         a.ImportAmount,
                                                         a.ImportAmountCur,
                                                         a.ExportQuantity,
                                                         ExportAmount = exportAmount,
                                                         ExportAmountCur = exportAmountCur,
                                                         PriceCalculatingMethod = "T",
                                                         a.IsTransfer,//dc_nb
                                                         a.IsAssembly,
                                                         a.FixedPrice,
                                                         GetPrice = a.GetPrice,
                                                         OnTop = "B",
                                                         AssemblyTransfer = "B1",
                                                         CreationTime = a.CreationTime,
                                                         ImportAvg = "K"

                                                     }).ToList();
                            wareHouseBookTmp = from a in lstWareHouseBookTmp
                                               join b in wareHouseBookTmps on new
                                               {
                                                   a.ProductVoucherId,
                                                   a.Ord0
                                               } equals new
                                               {
                                                   b.ProductVoucherId,
                                                   b.Ord0
                                               } into c
                                               from d in c.DefaultIfEmpty()
                                               select new
                                               {
                                                   a.OrgCode,
                                                   a.ProductVoucherId,
                                                   a.Id,
                                                   SttRec = a.SttRec,
                                                   a.Ord0,
                                                   a.VoucherCode,
                                                   a.VoucherGroup,
                                                   a.VoucherDate,
                                                   a.VoucherNumber,
                                                   a.BusinessCode,
                                                   a.DebitAcc,
                                                   a.CreditAcc,
                                                   a.WarehouseCode,
                                                   a.TransWarehouseCode,
                                                   a.ProductCode,
                                                   a.ProductLotCode,
                                                   a.ProductOriginCode,
                                                   a.UnitCode,
                                                   a.TrxQuantity,
                                                   a.Quantity,
                                                   PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                   PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                   Price = d != null ? d.Price : a.Price,
                                                   PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                   Amount = d != null ? d.Amount : a.Amount,
                                                   AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                   a.ImportQuantity,
                                                   a.ImportAmount,
                                                   a.ImportAmountCur,
                                                   a.ExportQuantity,
                                                   ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                                   ExportAmountCur = d != null ? d.ExportAmountCur : a.ExportAmountCur,
                                                   PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                   a.IsTransfer,//dc_nb
                                                   a.IsAssembly,
                                                   a.FixedPrice,
                                                   GetPrice = a.GetPrice,
                                                   OnTop = d != null ? d.OnTop : a.OnTop,
                                                   AssemblyTransfer = d != null ? d.AssemblyTransfer : a.AssemblyTransfer,
                                                   CreationTime = a.CreationTime,
                                                   ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                                               };

                            if (isTransfer == "C")
                            {
                                var wareHouseBookTmpss = (from a in wareHouseBookTmp
                                                          where a.ProductVoucherId == ordRec && a.Ord0 == "B" + ord0.Substring(1, 9)
                                                          select new
                                                          {
                                                              a.OrgCode,
                                                              a.ProductVoucherId,
                                                              a.Id,
                                                              SttRec = a.SttRec,
                                                              a.Ord0,
                                                              a.VoucherCode,
                                                              a.VoucherGroup,
                                                              a.VoucherDate,
                                                              a.VoucherNumber,
                                                              a.BusinessCode,
                                                              a.DebitAcc,
                                                              a.CreditAcc,
                                                              a.WarehouseCode,
                                                              a.TransWarehouseCode,
                                                              a.ProductCode,
                                                              a.ProductLotCode,
                                                              a.ProductOriginCode,
                                                              a.UnitCode,
                                                              a.TrxQuantity,
                                                              a.Quantity,
                                                              PriceCb = priceCb,
                                                              PriceCurCb = priceCbCur,
                                                              Price = price,
                                                              PriceCur = priceCur,
                                                              Amount = exportAmount,
                                                              AmountCur = exportAmountCur,
                                                              a.ImportQuantity,
                                                              ImportAmount = exportAmount,
                                                              ImportAmountCur = exportAmountCur,
                                                              a.ExportQuantity,
                                                              a.ExportAmount,
                                                              a.ExportAmountCur,
                                                              PriceCalculatingMethod = "T",
                                                              a.IsTransfer,//dc_nb
                                                              a.IsAssembly,
                                                              a.FixedPrice,
                                                              GetPrice = a.GetPrice,
                                                              OnTop = "B",
                                                              AssemblyTransfer = "B1",
                                                              CreationTime = a.CreationTime,
                                                              ImportAvg = "K"

                                                          }).ToList();
                                wareHouseBookTmp = from a in wareHouseBookTmp
                                                   join b in wareHouseBookTmpss on new
                                                   {
                                                       a.ProductVoucherId,
                                                       a.Ord0
                                                   } equals new
                                                   {
                                                       b.ProductVoucherId,
                                                       b.Ord0
                                                   } into c
                                                   from d in c.DefaultIfEmpty()
                                                   select new
                                                   {
                                                       a.OrgCode,
                                                       a.ProductVoucherId,
                                                       a.Id,
                                                       SttRec = a.SttRec,
                                                       a.Ord0,
                                                       a.VoucherCode,
                                                       a.VoucherGroup,
                                                       a.VoucherDate,
                                                       a.VoucherNumber,
                                                       a.BusinessCode,
                                                       a.DebitAcc,
                                                       a.CreditAcc,
                                                       a.WarehouseCode,
                                                       a.TransWarehouseCode,
                                                       a.ProductCode,
                                                       a.ProductLotCode,
                                                       a.ProductOriginCode,
                                                       a.UnitCode,
                                                       a.TrxQuantity,
                                                       a.Quantity,
                                                       PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                       PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                       Price = d != null ? d.Price : a.Price,
                                                       PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                       Amount = d != null ? d.Amount : a.Amount,
                                                       AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                       a.ImportQuantity,
                                                       ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                       ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                       a.ExportQuantity,
                                                       ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                                       ExportAmountCur = d != null ? d.ExportAmountCur : a.ExportAmountCur,
                                                       PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                       a.IsTransfer,//dc_nb
                                                       a.IsAssembly,
                                                       a.FixedPrice,
                                                       GetPrice = a.GetPrice,
                                                       OnTop = d != null ? d.OnTop : a.OnTop,
                                                       AssemblyTransfer = d != null ? d.AssemblyTransfer : a.AssemblyTransfer,
                                                       CreationTime = a.CreationTime,
                                                       ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                                                   };
                            }
                        }
                    }
                    var itemToRemove = resulwareHouseBookTmpB2.Single(r => r.Id == id1);
                    resulwareHouseBookTmpB2.Remove(itemToRemove);
                    //else
                    //{
                    //    var warehouseTmp = from a in wareHouseBookTmp
                    //                       where a.ProductVoucherId == ordRec
                    //                       && a.Ord0 == ord0
                    //                       select new
                    //                       {
                    //                           a.OrgCode,
                    //                           a.ProductVoucherId,
                    //                           a.Id,
                    //                           SttRec = a.SttRec,
                    //                           a.Ord0,
                    //                           a.VoucherCode,
                    //                           a.VoucherGroup,
                    //                           a.VoucherDate,
                    //                           a.VoucherNumber,
                    //                           a.BusinessCode,
                    //                           a.DebitAcc,
                    //                           a.CreditAcc,
                    //                           a.WarehouseCode,
                    //                           a.TransWarehouseCode,
                    //                           a.ProductCode,
                    //                           a.ProductLotCode,
                    //                           a.ProductOriginCode,
                    //                           a.UnitCode,
                    //                           a.TrxQuantity,
                    //                           a.Quantity,
                    //                           PriceCb = priceCb,
                    //                           PriceCurCb = priceCbCur,
                    //                           Price = price,
                    //                           PriceCur = priceCur,
                    //                           Amount = exportAmount,
                    //                           AmountCur = exportAmountCur,
                    //                           a.ImportQuantity,
                    //                           a.ImportAmount,
                    //                           a.ImportAmountCur,
                    //                           a.ExportQuantity,
                    //                           ExportAmount = exportAmount,
                    //                           ExportAmountCur = exportAmountCur,
                    //                           PriceCalculatingMethod = "T",
                    //                           a.IsTransfer,//dc_nb
                    //                           a.IsAssembly,
                    //                           a.FixedPrice,
                    //                           GetPrice = a.GetPrice,
                    //                           OnTop = "B",
                    //                           AssemblyTransfer = "B1",
                    //                           CreationTime = a.CreationTime,
                    //                           ImportAvg = "K"
                    //                       };
                    //    wareHouseBookTmp = from a in wareHouseBookTmp
                    //                       join b in warehouseTmp on new
                    //                       {
                    //                           a.ProductVoucherId,
                    //                           a.Ord0
                    //                       } equals new
                    //                       {
                    //                           b.ProductVoucherId,
                    //                           b.Ord0
                    //                       } into c
                    //                       from d in c.DefaultIfEmpty()
                    //                       select new
                    //                       {
                    //                           a.OrgCode,
                    //                           a.ProductVoucherId,
                    //                           a.Id,
                    //                           SttRec = a.SttRec,
                    //                           a.Ord0,
                    //                           a.VoucherCode,
                    //                           a.VoucherGroup,
                    //                           a.VoucherDate,
                    //                           a.VoucherNumber,
                    //                           a.BusinessCode,
                    //                           a.DebitAcc,
                    //                           a.CreditAcc,
                    //                           a.WarehouseCode,
                    //                           a.TransWarehouseCode,
                    //                           a.ProductCode,
                    //                           a.ProductLotCode,
                    //                           a.ProductOriginCode,
                    //                           a.UnitCode,
                    //                           a.TrxQuantity,
                    //                           a.Quantity,
                    //                           PriceCb = d != null ? d.PriceCb : a.PriceCb,
                    //                           PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                    //                           Price = d != null ? d.Price : a.Price,
                    //                           PriceCur = d != null ? d.PriceCur : a.PriceCur,
                    //                           Amount = d != null ? d.Amount : a.Amount,
                    //                           AmountCur = d != null ? d.AmountCur : a.AmountCur,
                    //                           a.ImportQuantity,
                    //                           a.ImportAmount,
                    //                           a.ImportAmountCur,
                    //                           a.ExportQuantity,
                    //                           ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                    //                           ExportAmountCur = d != null ? d.ExportAmountCur : a.ExportAmountCur,
                    //                           PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                    //                           a.IsTransfer,//dc_nb
                    //                           a.IsAssembly,
                    //                           a.FixedPrice,
                    //                           GetPrice = a.GetPrice,
                    //                           OnTop = d != null ? d.OnTop : a.OnTop,
                    //                           AssemblyTransfer = d != null ? d.AssemblyTransfer : a.AssemblyTransfer,
                    //                           CreationTime = a.CreationTime,
                    //                           ImportAvg = d != null ? d.ImportAvg : a.ImportAvg
                    //                       };
                    //}
                    // Update vào phiếu xuất lắp ráp, hoặc tách thành phẩm

                }
            }
            foreach (var item in lstWareHouseBookTmp)
            {
                //var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
                //productVoucherDetail= productVoucherDetail.Where()
                var warehouseBook = await _warehouseBookService.GetQueryableAsync();
                var warehouseBooks = warehouseBook.Where(p => p.ProductVoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                foreach (var items in warehouseBooks)
                {

                    items.Price = item.Price;
                    items.PriceCur = item.PriceCur;
                    items.Amount = items.Amount;
                    items.AmountCur = item.AmountCur;
                    items.ImportAmount = item.ImportAmount;
                    items.ImportAmountCur = item.ImportAmountCur;
                    items.ExportAmount = item.ExportAmount;
                    items.ExportAmountCur = item.ExportAmountCur;
                    items.Price0 = item.Price;
                    items.PriceCur0 = item.PriceCur;
                    await _warehouseBookService.UpdateAsync(items, true);


                }
                var ledger = await _ledgerService.GetQueryableAsync();
                var lstLedger = ledger.Where(p => p.VoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                foreach (var itemLedger in lstLedger)
                {
                    itemLedger.Price = item.Price;
                    itemLedger.PriceCur = item.PriceCur;
                    itemLedger.Amount = item.Amount;
                    itemLedger.AmountCur = item.AmountCur;
                    await _ledgerService.UpdateAsync(itemLedger, true);
                }

                //CHI TIET
                // updata phiếu HV
                var productVoucherdetail = await _productVoucherDetail.GetQueryableAsync();
                var lstProductVoucher = productVoucherdetail.Where(p => p.ProductVoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                if (item.VoucherCode != "PTR")
                {
                    foreach (var itemProductVoucher in lstProductVoucher)
                    {
                        itemProductVoucher.Price = item.Price;
                        itemProductVoucher.PriceCur = item.PriceCur;
                        itemProductVoucher.Amount = item.Amount;
                        itemProductVoucher.AmountCur = item.AmountCur;
                        if (item.VoucherGroup == 2)
                        {
                            itemProductVoucher.Price2 = item.Price;
                            itemProductVoucher.PriceCur2 = item.PriceCur;
                            itemProductVoucher.Amount2 = item.Amount;
                            itemProductVoucher.AmountCur2 = item.AmountCur;
                        }
                        await _productVoucherDetail.UpdateAsync(itemProductVoucher, true);
                    }
                }

                var updateProduct = (from a in lstProductVoucher
                                     group new
                                     {
                                         a.ProductVoucherId,
                                         a.Amount,
                                         a.AmountCur,
                                         a.Price,
                                         a.PriceCur
                                     } by new
                                     {
                                         a.ProductVoucherId
                                     } into gr
                                     select new
                                     {

                                         ProductVoucherId = gr.Key.ProductVoucherId,
                                         Amount = gr.Sum(p => p.Amount),
                                         AmountCur = gr.Sum(p => p.AmountCur)
                                     }).ToList();
                foreach (var items in updateProduct)
                {
                    var productVoucher = await _productVoucherService.GetQueryableAsync();
                    var lstProductVouchers = productVoucher.Where(p => p.Id == items.ProductVoucherId).ToList();

                    foreach (var itemProductVouchers in lstProductVouchers)
                    {
                        itemProductVouchers.TotalAmount = items.Amount;
                        itemProductVouchers.TotalAmountCur = items.AmountCur;
                        itemProductVouchers.TotalProductAmount = items.Amount;
                        itemProductVouchers.TotalProductAmountCur = items.AmountCur;

                        await _productVoucherService.UpdateAsync(itemProductVouchers, true);
                    }

                }
            }

        }

        public async Task MonthlyAvgPrice(CostOfGoodsDto dto, decimal v)
        {
            CostOfGoodsDto costOfGoodsDto = new CostOfGoodsDto();
            List<Product> products = new List<Product>();
            var defaultVoucherType = await _defaultVoucherTypeService.GetQueryableAsync();
            var ctLrs = await _voucherTypeService.GetQueryableAsync();
            var listCtLrs = "";
            var listCt = "";
            if (ctLrs.ToList().Count > 0)
            {
                listCtLrs = ctLrs.Where(p => p.Code == "PLR").FirstOrDefault().Code;
                listCt = ctLrs.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            }
            else
            {
                listCtLrs = defaultVoucherType.Where(p => p.Code == "PLR").FirstOrDefault().Code;
                listCt = defaultVoucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            }

            var productUnit = await _productUnitService.GetQueryableAsync();
            var lstproductUnit = productUnit.Where(p => p.OrgCode == dto.OrdCode).ToList();
            if (!string.IsNullOrEmpty(dto.ProductGroup))
            {
                products = await _productAppService.GetListByProductGroupCode(dto.ProductGroup);
            }
            var tenantSetting = await _tenantSettingService.GetQueryableAsync();
            var lsttenantSetting = tenantSetting.Where(p => p.OrgCode == dto.OrdCode).ToList();
            var defaulttenantSetting = await _defaultTenantSettingService.GetQueryableAsync();
            string fomatQuantity;
            string fomatAmount;
            string fomatAmountCur;
            if (lsttenantSetting.Count() > 0)
            {
                fomatQuantity = lsttenantSetting.Where(p => p.Key == "SL").FirstOrDefault().Value;
                fomatAmount = lsttenantSetting.Where(p => p.Key == "TIEN").FirstOrDefault().Value;
                fomatAmountCur = lsttenantSetting.Where(p => p.Key == "TIEN_NT").FirstOrDefault().Value;
            }
            else
            {
                fomatQuantity = defaulttenantSetting.Where(p => p.Key == "SL").FirstOrDefault().Value;
                fomatAmount = defaulttenantSetting.Where(p => p.Key == "TIEN").FirstOrDefault().Value;
                fomatAmountCur = defaulttenantSetting.Where(p => p.Key == "TIEN_NT").FirstOrDefault().Value;
            }
            var product = await _productService.GetQueryableAsync();

            var lstProduct = product.Where(p => p.OrgCode == dto.OrdCode).ToList();
            //CÁC MÃ HÀNG SỬ DỤNG TRONG KỲ
            var resulProduct = from a in lstProduct
                               join b in products on a.Code equals b.Code into c
                               from pr in c.DefaultIfEmpty()
                               select new
                               {
                                   a.OrgCode,
                                   ProductCode = a.Code,
                                   ProductionPeriodCode = a.ProductionPeriodCode
                               };
            if (!string.IsNullOrEmpty(dto.ProductionPeriodCode))
            {
                resulProduct = resulProduct.Where(p => p.ProductionPeriodCode == dto.ProductionPeriodCode).ToList();
            }

            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                resulProduct = resulProduct.Where(p => p.ProductCode == dto.ProductCode).ToList();
            }
            var wareHouseBook = await _warehouseBookService.GetQueryableAsync();
            var lstWareHouseBook = wareHouseBook.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year && p.VoucherDate <= dto.ToDate && dto.ProductCode == p.ProductCode).ToList();

            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == dto.OrdCode).ToList();
            var defaultVouchercategory = await _defaultVoucherCategoryService.GetQueryableAsync();
            if (lstVoucherCategory.Count == 0)
            {
                lstVoucherCategory = (from a in defaultVouchercategory
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
                                      }).ToList();
            }
            var wareHouseBookTmp = (from a in lstWareHouseBook
                                    join b in lstVoucherCategory on a.VoucherCode equals b.Code
                                    // join pr in resulProduct on a.ProductCode equals pr.ProductCode
                                    where a.VoucherGroup != 4
                                    select new
                                    {
                                        a.OrgCode,
                                        a.ProductVoucherId,
                                        a.Id,
                                        SttRec = a.ProductVoucherId,
                                        a.Ord0,
                                        a.VoucherCode,
                                        a.VoucherGroup,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        a.BusinessCode,
                                        a.DebitAcc,
                                        a.CreditAcc,
                                        WarehouseCode = a.WarehouseCode ?? "",
                                        TransWarehouseCode = a.TransWarehouseCode ?? "",
                                        a.ProductCode,
                                        ProductLotCode = a.ProductLotCode ?? "",
                                        ProductOriginCode = a.ProductOriginCode ?? "",
                                        a.UnitCode,
                                        TrxQuantity = a.TrxQuantity != null ? a.TrxQuantity : a.Quantity,
                                        a.Quantity,
                                        PriceCb = a.Price,
                                        PriceCurCb = a.PriceCur,
                                        a.Price,
                                        a.PriceCur,
                                        Amount = a.Amount,
                                        a.AmountCur,
                                        a.ImportQuantity,
                                        a.ImportAmount,
                                        a.ImportAmountCur,
                                        a.ExportQuantity,
                                        a.ExportAmount,
                                        a.ExportAmountCur,
                                        b.PriceCalculatingMethod,
                                        b.IsTransfer,//dc_nb
                                        b.IsAssembly,
                                        a.FixedPrice,
                                        GetPrice = b.PriceCalculatingMethod == "B" ? "C" : "C",
                                        OnTop = (b.PriceCalculatingMethod == "B" && (b.IsAssembly == "C" || b.Code == "PCH") && a.VoucherCode == "1") ? "A1" : (b.PriceCalculatingMethod == "B" && (b.IsAssembly == "C" || b.Code == "PCH") && a.VoucherCode == "2") ? "A2" : "C",
                                        AssemblyTransfer = (b.PriceCalculatingMethod == "B" && (b.IsAssembly == "C" || b.IsTransfer == "C")) ? "B2" : "B1",
                                        TransProductCode = a.TransProductCode

                                    }).ToList();

            //var resulProduct
            if (!string.IsNullOrEmpty(dto.WareHouseCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.WarehouseCode == dto.WareHouseCode).ToList();
            }
            if (!string.IsNullOrEmpty(dto.ProductCode))
            {
                var wareHouseBookTmpDC = wareHouseBookTmp.Where(p => p.TransProductCode == dto.ProductCode).ToList();
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.ProductCode == dto.ProductCode).ToList();
                wareHouseBookTmp.AddRange(wareHouseBookTmpDC);
            }

            if (!string.IsNullOrEmpty(dto.ProductLotCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.ProductLotCode == dto.ProductLotCode).ToList();
            }
            if (!string.IsNullOrEmpty(dto.ProductOriginCode))
            {
                wareHouseBookTmp = wareHouseBookTmp.Where(p => p.ProductOriginCode == dto.ProductOriginCode).ToList();
            }

            //Tim kiem nhung ma hang co ca nhap va xuat lap rap + chuyen doi hang

            var importB = from a in wareHouseBookTmp
                          where a.PriceCalculatingMethod == "B" && (a.IsAssembly == "C" || a.VoucherCode == "PCH") && a.VoucherGroup == 1
                          select new
                          {
                              a.ProductCode,
                              a.ProductLotCode,
                              a.ProductOriginCode
                          };
            var exportB = from a in wareHouseBookTmp
                          where a.PriceCalculatingMethod == "B" && (a.IsAssembly == "C" || a.VoucherCode == "PCH") && a.VoucherGroup == 2
                          select new
                          {
                              a.ProductCode,
                              a.ProductLotCode,
                              a.ProductOriginCode
                          };
            var resulImExportB = from a in importB
                                 join b in exportB on new { a.ProductCode, a.ProductLotCode, a.ProductOriginCode } equals new { b.ProductCode, b.ProductLotCode, b.ProductOriginCode }
                                 select new
                                 {
                                     a.ProductCode,
                                     a.ProductLotCode,
                                     a.ProductOriginCode
                                 };
            // CÁC KHO XUẤT TRONG KỲ THEO GIÁ TRUNG BÌNH

            var wareHouseExport = from a in wareHouseBookTmp
                                  where a.GetPrice == "C"
                                  group new
                                  {
                                      a.OrgCode,
                                      a.WarehouseCode,
                                      a.ProductCode,
                                      a.ProductLotCode,
                                      a.ProductOriginCode,
                                      a.OnTop,
                                      a.IsAssembly,
                                      a.IsTransfer,
                                      a.Quantity,
                                      a.Price,
                                      a.PriceCur,
                                      a.AmountCur,
                                      a.Amount,
                                      a.AssemblyTransfer
                                  } by new
                                  {
                                      a.OrgCode,
                                      a.WarehouseCode,
                                      a.ProductCode,
                                      a.ProductLotCode,
                                      a.ProductOriginCode
                                  } into gr
                                  select new
                                  {
                                      OrgCode = gr.Key.OrgCode,
                                      WarehouseCode = gr.Key.WarehouseCode,
                                      ProductCode = gr.Key.ProductCode,
                                      ProductLotCode = gr.Key.ProductLotCode,
                                      ProductOriginCode = gr.Key.ProductOriginCode,
                                      OnTop = gr.Min(p => p.OnTop),
                                      AssemblyTransfer = gr.Max(p => p.AssemblyTransfer),
                                      IsAssembly = gr.Max(p => p.IsAssembly),
                                      IsTransfer = gr.Max(p => p.IsTransfer),
                                      Quantity = 0,
                                      PriceX = 0,
                                      PriceCurX = 0,
                                      AmountCur = 0,
                                      Amount = 0,
                                  };
            var tets = wareHouseExport.ToList();
            ///Các mặt hàng tính giá trung binh thông thường. B1
            var priceB1 = from a in wareHouseExport
                          where a.AssemblyTransfer == "B1"
                          group new
                          {
                              a.ProductCode,
                              a.ProductLotCode,
                              a.ProductOriginCode,
                              a.WarehouseCode,
                              a.OrgCode,

                          } by new
                          {
                              a.ProductCode,
                              a.ProductLotCode,
                              a.ProductOriginCode,
                              a.WarehouseCode,
                              a.OrgCode,
                          } into gr
                          select new
                          {
                              OrgCode = gr.Key.OrgCode,
                              ProductCode = gr.Key.ProductCode,
                              ProductLotCode = gr.Key.ProductLotCode ?? null,
                              ProductOriginCode = gr.Key.ProductOriginCode ?? null,
                              WarehouseCode = gr.Key.WarehouseCode
                          };
            //TỒN ĐẦU KỲ VÀ NHỮNG PHẦN NHẬP THỰC TẾ(ĐÍCH DANH) TRỪ ĐI XUẤT THỰC TẾ(ĐÍCH DANH)
            var openingBalanceProduct = await _productOpeningBalanceService.GetQueryableAsync();
            var lstOpeningBalanceProduct = //openingBalanceProduct.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year).ToList();
                (from a in openingBalanceProduct
                 where a.OrgCode == dto.OrdCode && a.Year == dto.Year
                 select new
                 {
                     a.ProductCode,
                     ProductLotCode = a.ProductLotCode ?? "",
                     ProductOriginCode = a.ProductOriginCode ?? "",
                     a.WarehouseCode,
                     a.Price,
                     a.PriceCur,
                     a.Quantity,
                     a.Year,
                     a.OrgCode,
                     a.Amount,
                     a.AmountCur
                 }).ToList();
            //DkHv0B1
            var openingBalanceProduct0B1 = (from a in lstOpeningBalanceProduct
                                            join b in priceB1.ToList() on new { a.ProductCode, a.ProductLotCode, a.ProductOriginCode, a.WarehouseCode } equals new { b.ProductCode, b.ProductLotCode, b.ProductOriginCode, b.WarehouseCode }
                                            where a.OrgCode == dto.OrdCode && a.Year == dto.Year
                                            select new
                                            {
                                                a.WarehouseCode,
                                                a.ProductCode,
                                                ProductLotCode = a.ProductLotCode ?? "",
                                                ProductOriginCode = a.ProductOriginCode ?? "",
                                                ImportQuantity = a.Quantity,
                                                ImportAmountCur = a.AmountCur,
                                                ImportAmount = a.Amount,
                                                PriceX = (decimal)0,
                                                PriceCurX = (decimal)0
                                            }).ToList();
            //-- PHÁT SINH NHẬP - XUẤT TRƯỚC NGÀY @p_NGAY1
            var priceB1Lst = from a in priceB1
                             select new
                             {
                                 OrgCode = a.OrgCode,
                                 ProductCode = a.ProductCode,
                                 ProductLotCode = a.ProductLotCode == "" ? null : "",
                                 ProductOriginCode = a.ProductOriginCode == "" ? null : "",
                                 WarehouseCode = a.WarehouseCode
                             };
            var resulopeningBalanceProduct0B1 = (from a in lstWareHouseBook
                                                 where a.VoucherDate < dto.FromDate && int.Parse(a.Status) < 2
                                                 join b in priceB1Lst on new
                                                 {
                                                     a.WarehouseCode,
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode
                                                 } equals new
                                                 {
                                                     b.WarehouseCode,
                                                     b.ProductCode,
                                                     b.ProductLotCode,
                                                     b.ProductOriginCode
                                                 }
                                                 group new
                                                 {
                                                     a.WarehouseCode,
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode,
                                                     a.ImportQuantity,
                                                     a.ExportQuantity,
                                                     a.ImportAmount,
                                                     a.ExportAmount,
                                                     a.ImportAmountCur,
                                                     a.ExportAmountCur
                                                 } by new
                                                 {
                                                     a.WarehouseCode,
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode
                                                 } into gr
                                                 select new
                                                 {
                                                     WarehouseCode = gr.Key.WarehouseCode,
                                                     ProductCode = gr.Key.ProductCode,
                                                     ProductLotCode = gr.Key.ProductLotCode ?? "",
                                                     ProductOriginCode = gr.Key.ProductOriginCode ?? "",
                                                     ImportQuantity = (decimal)(gr.Sum(p => p.ImportQuantity - p.ExportQuantity)),
                                                     ImportAmountCur = (decimal)(gr.Sum(p => p.ImportAmountCur - p.ExportAmountCur)),
                                                     ImportAmount = (decimal)(gr.Sum(p => p.ImportAmount) - gr.Sum(p => p.ExportAmount)),
                                                     PriceX = (decimal)0,
                                                     PriceCurX = (decimal)0
                                                 }).ToList();
            openingBalanceProduct0B1.AddRange(resulopeningBalanceProduct0B1);

            /// PHÁT SINH NHẬP - XUẤT TRONG KỲ GIÁ THỰC TẾ - ĐÍCH DANH
            var resulOpeningBalance = from a in wareHouseBookTmp
                                      where a.PriceCalculatingMethod != "B"
                                      join b in priceB1 on new
                                      {
                                          a.WarehouseCode,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode,

                                      } equals new
                                      {
                                          b.WarehouseCode,
                                          b.ProductCode,
                                          b.ProductLotCode,
                                          b.ProductOriginCode
                                      }
                                      group new
                                      {
                                          a.WarehouseCode,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode,
                                          a.VoucherGroup,
                                          a.ImportAmount,
                                          a.ExportAmount,
                                          a.ImportQuantity,
                                          a.ExportQuantity,
                                          a.ImportAmountCur,
                                          a.ExportAmountCur
                                      } by new
                                      {
                                          a.WarehouseCode,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode,
                                          a.VoucherGroup
                                      } into gr

                                      select new
                                      {
                                          WarehouseCode = gr.Key.WarehouseCode,
                                          ProductCode = gr.Key.ProductCode,
                                          ProductLotCode = gr.Key.ProductLotCode ?? "",
                                          ProductOriginCode = gr.Key.ProductOriginCode ?? "",
                                          ImportQuantity = (decimal)(gr.Key.VoucherGroup == 1 ? gr.Sum(p => p.ImportQuantity) : -gr.Sum(p => p.ExportQuantity)),
                                          ImportAmountCur = (decimal)(gr.Key.VoucherGroup == 1 ? gr.Sum(p => p.ImportAmountCur) : -gr.Sum(p => p.ExportAmountCur)),
                                          ImportAmount = (decimal)(gr.Key.VoucherGroup == 1 ? gr.Sum(p => p.ImportAmount) : -gr.Sum(p => p.ExportAmount)),
                                          PriceX = (decimal)0,
                                          PriceCurX = (decimal)0
                                      };

            openingBalanceProduct0B1.AddRange(resulOpeningBalance);
            var priceB1OK = from a in openingBalanceProduct0B1
                            group new
                            {
                                a.WarehouseCode,
                                a.ProductCode,
                                a.ProductLotCode,
                                a.ProductOriginCode,
                                a.ImportQuantity,
                                a.PriceX,
                                a.PriceCurX,
                                a.ImportAmountCur,
                                a.ImportAmount
                            } by new
                            {
                                a.WarehouseCode,
                                a.ProductCode,
                                a.ProductLotCode,
                                a.ProductOriginCode
                            } into gr
                            select new
                            {
                                WarehouseCode = gr.Key.WarehouseCode,
                                ProductCode = gr.Key.ProductCode,
                                ProductLotCode = gr.Key.ProductLotCode ?? "",
                                ProductOriginCode = gr.Key.ProductOriginCode ?? "",
                                ImportQuantity = gr.Sum(p => p.ImportQuantity),
                                PriceX = (decimal)0,
                                PriceCurX = (decimal)0,
                                ImportAmountCur = gr.Sum(p => p.ImportAmountCur),
                                ImportAmount = gr.Sum(p => p.ImportAmount)
                            };
            priceB1OK = from a in priceB1OK
                        select new
                        {
                            WarehouseCode = a.WarehouseCode,
                            ProductCode = a.ProductCode,
                            ProductLotCode = a.ProductLotCode ?? "",
                            ProductOriginCode = a.ProductOriginCode ?? "",
                            ImportQuantity = a.ImportQuantity,
                            PriceX = (decimal)(a.ImportAmount / a.ImportQuantity),
                            PriceCurX = (decimal)(a.ImportAmountCur / a.ImportQuantity),
                            ImportAmountCur = a.ImportAmountCur,
                            ImportAmount = a.ImportAmount
                        };

            var wareHouseBookTmp1 = from a in wareHouseBookTmp
                                    where a.PriceCalculatingMethod == "B"
                                    join b in priceB1OK on new
                                    {
                                        a.WarehouseCode,
                                        a.ProductCode,
                                        a.ProductLotCode,
                                        a.ProductOriginCode
                                    }
                                    equals new
                                    {
                                        b.WarehouseCode,
                                        b.ProductCode,
                                        b.ProductLotCode,
                                        b.ProductOriginCode
                                    }
                                    select new
                                    {
                                        a.Id,
                                        a.OrgCode,
                                        a.SttRec,
                                        a.Ord0,
                                        a.VoucherCode,
                                        a.VoucherGroup,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        a.BusinessCode,
                                        a.DebitAcc,
                                        a.CreditAcc,
                                        a.WarehouseCode,
                                        a.TransWarehouseCode,
                                        a.ProductCode,
                                        a.ProductLotCode,
                                        a.ProductOriginCode,
                                        a.UnitCode,
                                        a.TrxQuantity,
                                        a.Quantity,
                                        PriceCb = a.TrxQuantity == 0 ? 0 : (b.PriceX * a.Quantity / a.TrxQuantity),
                                        PriceCurCb = a.TrxQuantity == 0 ? 0 : (b.PriceCurX * a.Quantity / a.TrxQuantity),
                                        Price = b.PriceX,
                                        PriceCur = b.PriceCurX,
                                        Amount = b.PriceX * a.Quantity,
                                        AmountCur = b.PriceCurX * a.Quantity,
                                        a.ImportQuantity,
                                        ImportAmount = a.VoucherGroup == 1 ? b.PriceX * a.ImportQuantity : 0,
                                        ImportAmountCur = a.VoucherGroup == 1 ? b.PriceCurX * a.ImportQuantity : 0,
                                        ExportQuantity = a.ExportQuantity,
                                        ExportAmount = a.VoucherGroup == 2 ? b.PriceX * a.ExportQuantity : 0,
                                        ExportAmountCur = a.VoucherGroup == 2 ? b.PriceCurX * a.ExportQuantity : 0,
                                        a.PriceCalculatingMethod,
                                        a.IsTransfer,//dc_nb
                                        a.IsAssembly,
                                        a.GetPrice,
                                        a.OnTop,
                                        a.AssemblyTransfer

                                    };
            if (wareHouseBookTmp1.ToList().Count > 0)
            {
                wareHouseBookTmp = (from a in wareHouseBookTmp
                                    join b in wareHouseBookTmp1 on a.Id equals b.Id into c
                                    from up in c.DefaultIfEmpty()
                                    select new
                                    {
                                        a.OrgCode,
                                        a.ProductVoucherId,
                                        a.Id,
                                        SttRec = a.ProductVoucherId,
                                        a.Ord0,
                                        a.VoucherCode,
                                        a.VoucherGroup,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        a.BusinessCode,
                                        a.DebitAcc,
                                        a.CreditAcc,
                                        a.WarehouseCode,
                                        a.TransWarehouseCode,
                                        a.ProductCode,
                                        a.ProductLotCode,
                                        a.ProductOriginCode,
                                        a.UnitCode,
                                        a.TrxQuantity,
                                        a.Quantity,
                                        PriceCb = up != null ? up.PriceCb : a.PriceCb,
                                        PriceCurCb = up != null ? up.PriceCurCb : a.PriceCurCb,
                                        a.Price,
                                        a.PriceCur,
                                        a.Amount,
                                        a.AmountCur,
                                        a.ImportQuantity,
                                        ImportAmount = up != null ? up.ImportAmount : a.ImportAmount,
                                        ImportAmountCur = up != null ? up.ImportAmount : a.ImportAmountCur,
                                        ExportQuantity = up != null ? up.ExportQuantity : a.ExportQuantity,
                                        ExportAmount = up != null ? up.ExportAmount : a.ExportAmount,
                                        ExportAmountCur = up != null ? up.ExportAmountCur : a.ExportAmountCur,
                                        a.PriceCalculatingMethod,
                                        a.IsTransfer,//dc_nb
                                        a.IsAssembly,
                                        a.FixedPrice,
                                        GetPrice = a.GetPrice,
                                        OnTop = a.OnTop,
                                        AssemblyTransfer = a.AssemblyTransfer,
                                        TransProductCode = a.TransProductCode
                                    }).ToList();
            }

            //Tính tồn cuối
            var resulwareHouseBookTmp = from a in wareHouseBookTmp
                                        where a.PriceCalculatingMethod == "B"
                                        join b in priceB1 on new
                                        {
                                            a.WarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode
                                        }
                                        equals new
                                        {
                                            b.WarehouseCode,
                                            b.ProductCode,
                                            b.ProductLotCode,
                                            b.ProductOriginCode
                                        }
                                        group new
                                        {
                                            a.WarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode,
                                            a.VoucherCode,
                                            a.ImportQuantity,
                                            a.ExportQuantity,
                                            a.ExportAmount,
                                            a.ImportAmount,
                                            a.ExportAmountCur,
                                            a.ImportAmountCur,
                                            a.VoucherGroup
                                        } by new
                                        {
                                            a.WarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode
                                        } into gr
                                        select new
                                        {
                                            WarehouseCode = gr.Key.WarehouseCode,
                                            ProductCode = gr.Key.ProductCode,
                                            ProductLotCode = gr.Key.ProductLotCode,
                                            ProductOriginCode = gr.Key.ProductOriginCode,
                                            ImportQuantity = (decimal)(gr.Max(p => p.VoucherGroup) == 1 ? gr.Sum(p => p.ImportQuantity) : -gr.Sum(p => p.ExportQuantity)),
                                            ImportAmountCur = (decimal)(gr.Max(p => p.VoucherGroup) == 1 ? gr.Sum(p => p.ImportAmountCur) : -gr.Sum(p => p.ExportAmountCur)),
                                            ImportAmount = (decimal)(gr.Max(p => p.VoucherGroup) == 1 ? gr.Sum(p => p.ImportAmount) : -gr.Sum(p => p.ExportAmount)),
                                            PriceX = (decimal)0,
                                            PriceCurX = (decimal)0
                                        };


            var resulOpeningbalanceProduct0B1 = (from a in openingBalanceProduct0B1
                                                 select new
                                                 {
                                                     a.WarehouseCode,
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode,
                                                     ImportQuantity = a.ImportQuantity,
                                                     ImportAmountCur = a.ImportAmountCur,
                                                     ImportAmount = a.ImportAmount,
                                                     PriceX = a.PriceX,
                                                     PriceCurX = a.PriceCurX
                                                 }).ToList();
            resulOpeningbalanceProduct0B1.AddRange(resulwareHouseBookTmp);
            //Những mặt hàng hết số lượng vẫn còn tồn tiền
            var priceB1Fix = (from a in resulOpeningbalanceProduct0B1
                              group new
                              {
                                  a.WarehouseCode,
                                  a.ProductCode,
                                  a.ProductLotCode,
                                  a.ProductOriginCode,
                                  a.ImportQuantity,
                                  a.ImportAmount,
                                  a.ImportAmountCur
                              } by new
                              {
                                  a.WarehouseCode,
                                  a.ProductCode,
                                  a.ProductLotCode,
                                  a.ProductOriginCode
                              } into gr
                              where gr.Sum(p => p.ImportQuantity) == 0 && Math.Abs(gr.Sum(p => p.ImportAmount)) < 10000
                              select new
                              {
                                  WarehouseCode = gr.Key.WarehouseCode,
                                  ProductCode = gr.Key.ProductCode,
                                  ProductLotCode = gr.Key.ProductLotCode,
                                  ProductOriginCode = gr.Key.ProductOriginCode,
                                  ImportQuantity = gr.Sum(p => p.ImportQuantity),
                                  ImportAmount = gr.Sum(p => p.ImportAmount),
                                  ImportAmountCur = gr.Sum(p => p.ImportAmountCur)
                              }).ToList();
            var sttRec = "";
            var ord0 = "";
            var isTransfer = "";
            var updateWareHouseBookTmp = (from a in wareHouseBookTmp
                                          where 1 == 0
                                          select new
                                          {
                                              a.OrgCode,
                                              a.ProductVoucherId,
                                              a.Id,
                                              SttRec = a.ProductVoucherId,
                                              a.Ord0,
                                              a.VoucherCode,
                                              a.VoucherGroup,
                                              a.VoucherDate,
                                              a.VoucherNumber,
                                              a.BusinessCode,
                                              a.DebitAcc,
                                              a.CreditAcc,
                                              a.WarehouseCode,
                                              a.TransWarehouseCode,
                                              a.ProductCode,
                                              a.ProductLotCode,
                                              a.ProductOriginCode,
                                              a.UnitCode,
                                              a.TrxQuantity,
                                              a.Quantity,
                                              PriceCb = a.Price,
                                              PriceCurCb = a.PriceCur,
                                              a.Price,
                                              a.PriceCur,
                                              a.Amount,
                                              a.AmountCur,
                                              a.ImportQuantity,
                                              a.ImportAmount,
                                              a.ImportAmountCur,
                                              a.ExportQuantity,
                                              a.ExportAmount,
                                              a.ExportAmountCur,
                                              a.PriceCalculatingMethod,
                                              a.IsTransfer,//dc_nb
                                              a.IsAssembly,
                                              GetPrice = a.GetPrice,
                                              OnTop = a.OnTop,
                                              AssemblyTransfer = a.AssemblyTransfer



                                          }).ToList();
            foreach (var item in priceB1Fix)
            {
                var resulTMP = (from a in wareHouseBookTmp
                                where a.PriceCalculatingMethod == "B" && a.VoucherGroup == 2
                                && a.ProductCode == item.ProductCode && a.WarehouseCode == item.WarehouseCode && a.ProductLotCode == item.ProductLotCode
                                select new
                                {
                                    a.SttRec,
                                    a.Ord0,
                                    a.IsTransfer,
                                    a.TransWarehouseCode
                                }).ToList().FirstOrDefault();
                if (resulTMP != null)
                {
                    sttRec = resulTMP.SttRec;
                    ord0 = resulTMP.Ord0;
                }

                if (!string.IsNullOrEmpty(sttRec))
                {
                    var uWareHouseBookTmp = (from a in wareHouseBookTmp
                                             where a.SttRec == sttRec && a.Ord0 == ord0
                                             select new
                                             {
                                                 a.OrgCode,
                                                 a.ProductVoucherId,
                                                 a.Id,
                                                 SttRec = a.ProductVoucherId,
                                                 a.Ord0,
                                                 a.VoucherCode,
                                                 a.VoucherGroup,
                                                 a.VoucherDate,
                                                 a.VoucherNumber,
                                                 a.BusinessCode,
                                                 a.DebitAcc,
                                                 a.CreditAcc,
                                                 a.WarehouseCode,
                                                 a.TransWarehouseCode,
                                                 a.ProductCode,
                                                 a.ProductLotCode,
                                                 a.ProductOriginCode,
                                                 a.UnitCode,
                                                 a.TrxQuantity,
                                                 a.Quantity,
                                                 a.PriceCb,
                                                 a.PriceCurCb,
                                                 a.Price,
                                                 a.PriceCur,
                                                 Amount = a.Amount + item.ImportAmount,
                                                 AmountCur = a.AmountCur + item.ImportAmountCur,
                                                 a.ImportQuantity,
                                                 a.ImportAmount,
                                                 a.ImportAmountCur,
                                                 a.ExportQuantity,
                                                 ExportAmount = a.ExportAmount + item.ImportAmount,
                                                 ExportAmountCur = a.ExportAmountCur + item.ImportAmountCur,
                                                 a.PriceCalculatingMethod,
                                                 a.IsTransfer,//dc_nb
                                                 a.IsAssembly,
                                                 a.GetPrice,
                                                 a.OnTop,
                                                 a.AssemblyTransfer
                                             }).ToList();
                    updateWareHouseBookTmp.AddRange(uWareHouseBookTmp);
                }
            }

            //QUÉT CÁC MẶT HÀNG XUẤT TRONG KỲ -có điều chuyển, lắp ráp, chuyển đổi. B2
            var lstWareHouse = (from a in wareHouseExport
                                where a.AssemblyTransfer == "B2"
                                group new
                                {
                                    a.ProductCode,
                                    a.ProductLotCode,
                                    a.ProductOriginCode

                                } by new
                                {
                                    a.ProductCode,
                                    a.ProductLotCode,
                                    a.ProductOriginCode
                                } into gr
                                select new
                                {
                                    ProductCode = gr.Key.ProductCode,
                                    ProductLotCode = gr.Key.ProductLotCode,
                                    ProductOriginCode = gr.Key.ProductOriginCode
                                }).ToList();

            try
            {
                foreach (var item in lstWareHouse)
                {

                    var priceWareHouse = (from a in wareHouseExport
                                          where a.ProductCode == item.ProductCode
                                          && a.ProductLotCode == item.ProductLotCode
                                          && a.ProductOriginCode == item.ProductOriginCode
                                          select new
                                          {
                                              nRow = 0,
                                              a.WarehouseCode,
                                              a.ProductCode,
                                              a.ProductOriginCode,
                                              a.ProductLotCode,
                                              a.Quantity,
                                              a.PriceCurX,
                                              a.AmountCur,
                                              a.PriceX,
                                              a.Amount

                                          }).OrderBy(p => p.WarehouseCode)
                                            .OrderBy(p => p.ProductCode)
                                            .OrderBy(p => p.ProductOriginCode)
                                            .OrderBy(p => p.ProductLotCode)
                                          .ToList();
                    var resulPriceWareHouse = (from a in priceWareHouse
                                               where 1 == 0
                                               select new
                                               {
                                                   nRow = 0,
                                                   a.WarehouseCode,
                                                   a.ProductCode,
                                                   a.ProductOriginCode,
                                                   a.ProductLotCode,
                                                   a.Quantity,
                                                   PriceCurX = (decimal)a.PriceCurX,
                                                   a.AmountCur,
                                                   PriceX = (decimal)a.PriceX,
                                                   a.Amount

                                               }).OrderBy(p => p.WarehouseCode)
                                            .OrderBy(p => p.ProductCode)
                                            .OrderBy(p => p.ProductOriginCode)
                                            .OrderBy(p => p.ProductLotCode)
                                          .ToList();
                    for (int m = 0; m < priceWareHouse.Count; m++)
                    {
                        var priceWareHouses = (from a in wareHouseExport
                                               where a.ProductCode == priceWareHouse[m].ProductCode
                                               && a.ProductLotCode == priceWareHouse[m].ProductLotCode
                                               && a.ProductOriginCode == priceWareHouse[m].ProductOriginCode
                                               && a.WarehouseCode == priceWareHouse[m].WarehouseCode
                                               select new
                                               {
                                                   nRow = m + 1,
                                                   a.WarehouseCode,
                                                   a.ProductCode,
                                                   a.ProductOriginCode,
                                                   a.ProductLotCode,
                                                   a.Quantity,
                                                   PriceCurX = (decimal)a.PriceCurX,
                                                   a.AmountCur,
                                                   PriceX = (decimal)a.PriceX,
                                                   a.Amount

                                               }).OrderBy(p => p.WarehouseCode)
                                               .OrderBy(p => p.ProductCode)
                                               .OrderBy(p => p.ProductOriginCode)
                                               .OrderBy(p => p.ProductLotCode)
                                             .ToList();
                        resulPriceWareHouse.AddRange(priceWareHouses);
                    }
                    var openningBalanceProduct0 = await _productOpeningBalanceService.GetQueryableAsync();
                    var lstopenningBalanceProduct0 = openningBalanceProduct0.Where(p => p.OrgCode == dto.OrdCode && p.Year == dto.Year && dto.ProductCode == p.ProductCode).ToList();
                    //TỒN ĐẦU KỲ VÀ NHỮNG PHẦN NHẬP THỰC TẾ (ĐÍCH DANH) TRỪ ĐI XUẤ
                    var resulOpeningbalanceProduct0 = (from a in lstopenningBalanceProduct0
                                                       join b in resulPriceWareHouse
                                                       on a.WarehouseCode equals b.WarehouseCode
                                                       select new
                                                       {
                                                           a.WarehouseCode,
                                                           ImportQuantity = a.Quantity,
                                                           ImportAmountCur = a.AmountCur,
                                                           ImportAmount = a.Amount,
                                                           ImportQuantity0 = a.Quantity,
                                                           ImportAmountCur0 = a.AmountCur,
                                                           ImportAmount0 = a.Amount,
                                                       }).ToList();
                    if (!string.IsNullOrEmpty(item.ProductCode))
                    {
                        resulOpeningbalanceProduct0 = (from a in lstopenningBalanceProduct0
                                                       where a.ProductCode == item.ProductCode
                                                       select new
                                                       {
                                                           a.WarehouseCode,
                                                           ImportQuantity = a.Quantity,
                                                           ImportAmountCur = a.AmountCur,
                                                           ImportAmount = a.Amount,
                                                           ImportQuantity0 = a.Quantity,
                                                           ImportAmountCur0 = a.AmountCur,
                                                           ImportAmount0 = a.Amount,
                                                       }).ToList();
                    }
                    if (!string.IsNullOrEmpty(item.ProductLotCode))
                    {
                        resulOpeningbalanceProduct0 = (from a in lstopenningBalanceProduct0
                                                       where a.ProductLotCode == item.ProductLotCode
                                                       select new
                                                       {
                                                           a.WarehouseCode,
                                                           ImportQuantity = a.Quantity,
                                                           ImportAmountCur = a.AmountCur,
                                                           ImportAmount = a.Amount,
                                                           ImportQuantity0 = a.Quantity,
                                                           ImportAmountCur0 = a.AmountCur,
                                                           ImportAmount0 = a.Amount,
                                                       }).ToList();
                    }
                    if (!string.IsNullOrEmpty(item.ProductOriginCode))
                    {
                        resulOpeningbalanceProduct0 = (from a in lstopenningBalanceProduct0
                                                       where a.ProductOriginCode == item.ProductOriginCode
                                                       select new
                                                       {
                                                           a.WarehouseCode,
                                                           ImportQuantity = a.Quantity,
                                                           ImportAmountCur = a.AmountCur,
                                                           ImportAmount = a.Amount,
                                                           ImportQuantity0 = a.Quantity,
                                                           ImportAmountCur0 = a.AmountCur,
                                                           ImportAmount0 = a.Amount,
                                                       }).ToList();
                    }
                    //PHÁT SINH NHẬP - XUẤT TRƯỚC NGÀY @p_NGAY1	
                    var wareHouseBooks = await _warehouseBookService.GetQueryableAsync();
                    var lstWareHouseBooks = wareHouseBooks.Where(p => p.OrgCode == dto.OrdCode
                                                                    && p.Status != "2"
                                                                    && p.VoucherDate < dto.FromDate
                                                                    ).ToList();
                    var resulLstWareHouseBook = (from a in lstWareHouseBooks
                                                 join b in resulProduct on a.ProductCode equals b.ProductCode
                                                 join c in resulPriceWareHouse on a.WarehouseCode equals c.WarehouseCode
                                                 where a.ProductCode == item.ProductCode
                                                 && a.ProductLotCode == item.ProductLotCode
                                                 && a.ProductOriginCode == item.ProductOriginCode

                                                 group new
                                                 {
                                                     a.WarehouseCode,
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode,
                                                     a.ImportQuantity,
                                                     a.ExportQuantity,
                                                     a.ImportAmountCur,
                                                     a.ImportAmount,
                                                     a.ExportAmountCur,
                                                     a.ExportAmount
                                                 } by new
                                                 {
                                                     a.WarehouseCode,
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode
                                                 } into gr
                                                 select new
                                                 {
                                                     WarehouseCode = gr.Key.WarehouseCode,
                                                     ImportQuantity = (decimal)(gr.Sum(p => p.ImportQuantity) - gr.Sum(p => p.ExportQuantity)),
                                                     ImportAmountCur = (decimal)(gr.Sum(p => p.ImportAmountCur) - gr.Sum(p => p.ExportAmountCur)),
                                                     ImportAmount = (decimal)(gr.Sum(p => p.ImportAmount) - gr.Sum(p => p.ExportAmount)),
                                                     ImportQuantity0 = (decimal)(gr.Sum(p => p.ImportQuantity) - gr.Sum(p => p.ExportQuantity)),
                                                     ImportAmountCur0 = (decimal)(gr.Sum(p => p.ImportAmountCur) - gr.Sum(p => p.ExportAmountCur)),
                                                     ImportAmount0 = (decimal)(gr.Sum(p => p.ImportQuantity) - gr.Sum(p => p.ExportQuantity)),
                                                 }).ToList();




                    resulOpeningbalanceProduct0.AddRange(resulLstWareHouseBook);
                    //PHÁT SINH NHẬP - XUẤT TRONG KỲ GIÁ THỰC TẾ - ĐÍCH DANH
                    var resulWareHouseTmp = (from a in wareHouseBookTmp
                                             join b in resulPriceWareHouse on a.WarehouseCode equals b.WarehouseCode
                                             where a.PriceCalculatingMethod != "B"
                                             && a.ProductCode == item.ProductCode
                                             && a.ProductLotCode == item.ProductLotCode
                                             && a.ProductOriginCode == item.ProductOriginCode
                                             select new
                                             {
                                                 a.WarehouseCode,
                                                 ImportQuantity = (decimal)(a.VoucherGroup == 1 ? a.ImportQuantity ?? 0 : -a.ExportQuantity ?? 0),
                                                 ImportAmountCur = (decimal)(a.VoucherGroup == 1 ? a.ImportAmountCur ?? 0 : -a.ExportAmountCur ?? 0),
                                                 ImportAmount = (decimal)(a.VoucherGroup == 1 ? a.ImportAmount ?? 0 : -a.ExportAmount ?? 0),
                                                 ImportQuantity0 = (decimal)0,
                                                 ImportAmountCur0 = (decimal)0,
                                                 ImportAmount0 = (decimal)0
                                             }).ToList();
                    resulOpeningbalanceProduct0.AddRange(resulWareHouseTmp);
                    //Đề trường hợp không tính được giá thì lấy giá này ( cái này bỏ vì ko đúng)
                    var importKtb = from a in wareHouseBookTmp
                                    join b in resulPriceWareHouse on a.WarehouseCode equals b.WarehouseCode
                                    where a.PriceCalculatingMethod != "B" && a.ProductCode == item.ProductCode
                                    && a.ProductLotCode == item.ProductLotCode && a.ProductOriginCode == item.ProductOriginCode
                                    group new
                                    {
                                        a.WarehouseCode,
                                        a.ImportQuantity,
                                        b.PriceX,
                                        b.PriceCurX,
                                        a.ImportAmount,
                                        a.ImportAmountCur
                                    } by new
                                    {
                                        a.WarehouseCode
                                    } into gr
                                    select new
                                    {
                                        WarehouseCode = gr.Key.WarehouseCode,
                                        ImportQuantity = gr.Max(p => p.ImportQuantity),
                                        PriceX = (decimal)gr.Max(p => p.PriceX),
                                        PriceCurX = (decimal)gr.Max(p => p.PriceCurX),
                                        ImportAmount = (decimal)gr.Sum(p => p.ImportAmount),
                                        ImportAmountCur = (decimal)gr.Sum(p => p.ImportAmountCur)
                                    };
                    importKtb = from a in importKtb
                                select new
                                {
                                    WarehouseCode = a.WarehouseCode,
                                    ImportQuantity = a.ImportQuantity,
                                    PriceX = (decimal)(a.ImportAmount / a.ImportQuantity),
                                    PriceCurX = (decimal)(a.ImportAmountCur / a.ImportQuantity),
                                    ImportAmount = a.ImportAmount,
                                    ImportAmountCur = a.ImportAmountCur
                                };
                    // Tổng hợp tồn đầu + nhập xuất khác trung bình..
                    var resulOpeningbalanceProduct = (from a in resulOpeningbalanceProduct0
                                                      group new
                                                      {
                                                          a.WarehouseCode,
                                                          a.ImportQuantity,
                                                          a.ImportAmount,
                                                          a.ImportAmountCur,
                                                          a.ImportQuantity0,
                                                          a.ImportAmount0,
                                                          a.ImportAmountCur0
                                                      } by new
                                                      {
                                                          a.WarehouseCode
                                                      } into gr
                                                      select new
                                                      {
                                                          WarehouseCode = gr.Key.WarehouseCode,
                                                          ImportQuantity = gr.Sum(p => p.ImportQuantity),
                                                          ImportAmount = gr.Sum(p => p.ImportAmount),
                                                          ImportAmountCur = gr.Sum(p => p.ImportAmountCur),
                                                          ImportQuantity0 = gr.Sum(p => p.ImportQuantity0),
                                                          ImportAmount0 = gr.Sum(p => p.ImportAmount0),
                                                          ImportAmountCur0 = gr.Sum(p => p.ImportAmountCur0)
                                                      }).ToList();
                    // Them nhung kho con thieu

                    var warehouse = (from a in resulPriceWareHouse
                                     join b in resulOpeningbalanceProduct on a.WarehouseCode equals b.WarehouseCode into c
                                     from d in c.DefaultIfEmpty()
                                     where d == null
                                     select new
                                     {
                                         WarehouseCode = a.WarehouseCode,
                                         ImportQuantity = (decimal)0,
                                         ImportAmount = (decimal)0,
                                         ImportAmountCur = (decimal)0,
                                         ImportQuantity0 = (decimal)0,
                                         ImportAmount0 = (decimal)0,
                                         ImportAmountCur0 = (decimal)0
                                     }).ToList();
                    resulOpeningbalanceProduct.AddRange(warehouse);

                    // TẠO MA TRẬN XÁC ĐỊNH GIÁ
                    var matrixGaus = (from a in priceWareHouse
                                      from b in priceWareHouse
                                      select new
                                      {
                                          a.WarehouseCode,
                                          TransWarehouseCode = b.WarehouseCode,
                                          Quantity = (decimal)a.Quantity
                                      }).ToList();
                    //CÁC PHIẾU NHẬP TÍNH GIÁ TRUNG BÌNH

                    var ImportAvg = from a in wareHouseBookTmp
                                    join b in priceWareHouse on a.WarehouseCode equals b.WarehouseCode
                                    where a.PriceCalculatingMethod == "B" && a.ProductCode == item.ProductCode
                                    && a.ProductOriginCode == item.ProductOriginCode && a.ProductLotCode == item.ProductLotCode
                                    group new
                                    {
                                        a.WarehouseCode,
                                        a.TransWarehouseCode,
                                        a.ImportQuantity
                                    } by new
                                    {
                                        a.WarehouseCode,
                                        a.TransWarehouseCode
                                    } into gr

                                    select new
                                    {
                                        WarehouseCode = gr.Key.WarehouseCode,
                                        TransWarehouseCode = string.IsNullOrEmpty(gr.Key.TransWarehouseCode) == true ? gr.Key.WarehouseCode : gr.Key.TransWarehouseCode,
                                        ImportQuantity = gr.Sum(p => p.ImportQuantity)
                                    };

                    //CẬP NHẬT VÀO MA TRẬN GAUSS
                    matrixGaus = (from a in matrixGaus
                                  join b in ImportAvg on new
                                  {
                                      a.WarehouseCode,
                                      a.TransWarehouseCode
                                  } equals new
                                  {
                                      b.WarehouseCode,
                                      b.TransWarehouseCode
                                  } into c
                                  from d in c.DefaultIfEmpty()

                                  select new
                                  {
                                      WarehouseCode = a.WarehouseCode,
                                      TransWarehouseCode = a.TransWarehouseCode,
                                      Quantity = d != null ? (decimal)d.ImportQuantity : (decimal)0,


                                  }).ToList();
                    //CHUYỂN HÓA THÀNH MA TRẬN CHUẨN
                    foreach (var items in resulPriceWareHouse)
                    {
                        decimal quantityBalance = 0;
                        decimal importQuantity = 0;

                        quantityBalance = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == items.WarehouseCode).Select(p => p.ImportQuantity).ToList().FirstOrDefault();
                        importQuantity = matrixGaus.Where(p => p.WarehouseCode == items.WarehouseCode && p.TransWarehouseCode != items.WarehouseCode).Select(p => p.Quantity).Sum();
                        var matrixGauss = (from a in matrixGaus
                                           where a.WarehouseCode == items.WarehouseCode && a.TransWarehouseCode == items.WarehouseCode
                                           select new
                                           {
                                               WarehouseCode = a.WarehouseCode,
                                               TransWarehouseCode = a.TransWarehouseCode,
                                               Quantity = quantityBalance + importQuantity
                                           }).ToList();
                        matrixGaus = (from a in matrixGaus
                                      join b in matrixGauss on new { a.WarehouseCode, a.TransWarehouseCode } equals new { b.WarehouseCode, b.TransWarehouseCode }
                                      into c
                                      from d in c.DefaultIfEmpty()
                                      select new
                                      {
                                          WarehouseCode = a.WarehouseCode,
                                          TransWarehouseCode = a.TransWarehouseCode,
                                          Quantity = d != null ? d.Quantity : a.Quantity
                                      }).ToList();
                    }
                    var matrixGaus01 = from a in matrixGaus
                                       where a.TransWarehouseCode != a.WarehouseCode
                                       select new
                                       {
                                           WarehouseCode = a.WarehouseCode,
                                           TransWarehouseCode = a.TransWarehouseCode,
                                           Quantity = -a.Quantity
                                       };
                    matrixGaus = (from a in matrixGaus
                                  join b in matrixGaus01 on new { a.WarehouseCode, a.TransWarehouseCode } equals new { b.WarehouseCode, b.TransWarehouseCode }
                                  into c
                                  from d in c.DefaultIfEmpty()
                                  select new
                                  {
                                      WarehouseCode = a.WarehouseCode,
                                      TransWarehouseCode = a.TransWarehouseCode,
                                      Quantity = d != null ? d.Quantity : a.Quantity
                                  }).ToList();
                    int count = resulPriceWareHouse.Count;
                    int i = 1;
                    int k = 0;
                    int j = 0;
                    var wareHouseI = "";
                    var wareHouseK = "";
                    decimal quantity1 = 0;
                    decimal quantity2 = 0;
                    var WareHouseJ = "";
                    decimal quantity3 = 0;
                    decimal quantity4 = 0;
                    decimal quantity5 = 0;
                    int yt = count - 1;
                    //PHÉP KHỬ GAUSS

                    while (i <= count - 1)
                    {
                        wareHouseI = resulPriceWareHouse.Where(p => p.nRow == i).Select(p => p.WarehouseCode).FirstOrDefault();
                        k = i + 1;
                        while (k <= count)
                        {
                            wareHouseK = resulPriceWareHouse.Where(p => p.nRow == k).Select(p => p.WarehouseCode).FirstOrDefault();
                            quantity1 = matrixGaus.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.Quantity).FirstOrDefault();
                            quantity2 = matrixGaus.Where(p => p.WarehouseCode == wareHouseK).Select(p => p.Quantity).FirstOrDefault();
                            if (quantity1 != 0)
                            {
                                j = 1;
                                while (j <= count)
                                {
                                    WareHouseJ = resulPriceWareHouse.Where(p => p.nRow == j).Select(p => p.WarehouseCode).FirstOrDefault();
                                    quantity3 = matrixGaus.Where(p => p.WarehouseCode == wareHouseK && p.TransWarehouseCode == WareHouseJ).Select(p => p.Quantity).FirstOrDefault();
                                    quantity4 = matrixGaus.Where(p => p.WarehouseCode == wareHouseI && p.TransWarehouseCode == WareHouseJ).Select(p => p.Quantity).FirstOrDefault();

                                    if (quantity3 != 0)
                                    {

                                        quantity3 = quantity3 - Math.Round((quantity1) / (quantity1 * quantity3));
                                    }

                                    var matrixGaus2 = (from a in matrixGaus
                                                       where a.WarehouseCode == wareHouseK && a.TransWarehouseCode == WareHouseJ
                                                       select new
                                                       {
                                                           WarehouseCode = a.WarehouseCode,
                                                           TransWarehouseCode = a.TransWarehouseCode,
                                                           Quantity = quantity3
                                                       }).ToList();
                                    matrixGaus = (from a in matrixGaus
                                                  join b in matrixGaus2 on new
                                                  {
                                                      a.WarehouseCode,
                                                      a.TransWarehouseCode
                                                  } equals new
                                                  {
                                                      b.WarehouseCode,
                                                      b.TransWarehouseCode
                                                  } into c
                                                  from d in c.DefaultIfEmpty()
                                                  select new
                                                  {
                                                      WarehouseCode = a.WarehouseCode,
                                                      TransWarehouseCode = a.TransWarehouseCode,
                                                      Quantity = d != null ? d.Quantity : a.Quantity
                                                  }).ToList();
                                    j += 1;
                                }
                                //Update vào hạng mục cuối Tiền việt
                                quantity3 = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == wareHouseK).Select(p => p.ImportAmount).FirstOrDefault();
                                quantity4 = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.ImportAmount).FirstOrDefault();
                                quantity3 = quantity3 - (quantity2 / quantity1 * quantity4);
                                var resulOpeningbalanceProduct01 = from a in resulOpeningbalanceProduct
                                                                   where a.WarehouseCode == wareHouseK
                                                                   select new
                                                                   {
                                                                       a.WarehouseCode,
                                                                       ImportAmount = quantity3
                                                                   };
                                resulOpeningbalanceProduct = (from a in resulOpeningbalanceProduct
                                                              join b in resulOpeningbalanceProduct01 on a.WarehouseCode equals b.WarehouseCode into c
                                                              from d in c.DefaultIfEmpty()
                                                              select new
                                                              {
                                                                  WarehouseCode = a.WarehouseCode,
                                                                  ImportQuantity = a.ImportQuantity,
                                                                  ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                                  ImportAmountCur = a.ImportAmountCur,
                                                                  ImportQuantity0 = a.ImportQuantity0,
                                                                  ImportAmount0 = a.ImportAmount0,
                                                                  ImportAmountCur0 = a.ImportAmountCur0
                                                              }).ToList();
                                //---- Update vào hạng mục cuối Ngoại tệ
                                quantity3 = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == wareHouseK).Select(p => p.ImportAmountCur).FirstOrDefault();
                                quantity4 = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.ImportAmountCur).FirstOrDefault();
                                quantity3 = quantity3 - (quantity2 / quantity1 * quantity4);


                                var resulOpeningbalanceProduct02 = from a in resulOpeningbalanceProduct
                                                                   where a.WarehouseCode == wareHouseK
                                                                   select new
                                                                   {
                                                                       a.WarehouseCode,
                                                                       ImportAmountCur = quantity3
                                                                   };
                                resulOpeningbalanceProduct = (from a in resulOpeningbalanceProduct
                                                              join b in resulOpeningbalanceProduct02 on a.WarehouseCode equals b.WarehouseCode into c
                                                              from d in c.DefaultIfEmpty()
                                                              select new
                                                              {
                                                                  WarehouseCode = a.WarehouseCode,
                                                                  ImportQuantity = a.ImportQuantity,
                                                                  ImportAmount = a.ImportAmount,
                                                                  ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                                  ImportQuantity0 = a.ImportQuantity0,
                                                                  ImportAmount0 = a.ImportAmount0,
                                                                  ImportAmountCur0 = a.ImportAmountCur0
                                                              }).ToList();

                            }
                            k += 1;
                        }
                        i += 1;
                    }

                    //-- TÍNH GIÁ VỐN
                    i = count;

                    while (i > 0)
                    {
                        wareHouseI = resulPriceWareHouse.Where(p => p.nRow == i).Select(p => p.WarehouseCode).FirstOrDefault();
                        quantity1 = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.ImportAmount).FirstOrDefault();
                        quantity2 = resulOpeningbalanceProduct.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.ImportAmountCur).FirstOrDefault();
                        j = 1;
                        while (j <= count)
                        {
                            WareHouseJ = resulPriceWareHouse.Where(p => p.nRow == j).Select(p => p.WarehouseCode).FirstOrDefault();
                            if (i != j)
                            {
                                WareHouseJ = resulPriceWareHouse.Where(p => p.nRow == j).Select(p => p.WarehouseCode).FirstOrDefault();
                                quantity3 = matrixGaus.Where(p => p.WarehouseCode == wareHouseI && p.TransWarehouseCode == WareHouseJ).Select(p => p.Quantity).FirstOrDefault();
                                quantity4 = resulPriceWareHouse.Where(p => p.nRow == j).Select(p => p.PriceX).FirstOrDefault();
                                quantity5 = resulPriceWareHouse.Where(p => p.nRow == j).Select(p => p.PriceCurX).FirstOrDefault();
                                quantity1 = quantity1 - quantity3 * quantity4;
                                quantity2 = quantity2 - quantity4 * quantity5;
                            }
                            j += 1;
                        }
                        quantity3 = matrixGaus.Where(p => p.WarehouseCode == wareHouseI && p.TransWarehouseCode == wareHouseI).Select(p => p.Quantity).FirstOrDefault();

                        if (quantity3 != 0)
                        {
                            quantity1 = quantity1 / quantity3;
                            quantity2 = quantity2 / quantity3;
                        }
                        else
                        {
                            quantity1 = 0;
                            quantity2 = 0;

                        }
                        if (quantity1 == 0)
                        {
                            quantity1 = importKtb.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.PriceX).FirstOrDefault();
                            quantity2 = importKtb.Where(p => p.WarehouseCode == wareHouseI).Select(p => p.PriceCurX).FirstOrDefault();
                        }
                        var priceWareHouse1 = (from a in resulPriceWareHouse
                                               where a.nRow == i
                                               select new
                                               {
                                                   nRow = a.nRow,
                                                   a.WarehouseCode,
                                                   a.ProductCode,
                                                   a.ProductOriginCode,
                                                   a.ProductLotCode,
                                                   a.Quantity,
                                                   PriceCurX = Math.Abs(quantity2),
                                                   a.AmountCur,
                                                   PriceX = Math.Abs(quantity1),
                                                   a.Amount
                                               }).ToList();
                        if (priceWareHouse1.Count > 0)
                        {
                            var resulPriceWareHouses = (from a in resulPriceWareHouse
                                                        join b in priceWareHouse1 on a.nRow equals b.nRow
                                                        select new
                                                        {
                                                            nRow = a.nRow,
                                                            a.WarehouseCode,
                                                            a.ProductCode,
                                                            a.ProductOriginCode,
                                                            a.ProductLotCode,
                                                            a.Quantity,
                                                            PriceCurX = (decimal)(b != null ? b.PriceCurX : a.PriceCurX),
                                                            a.AmountCur,
                                                            PriceX = (decimal)(b != null ? b.PriceX : a.PriceX),
                                                            a.Amount
                                                        }).ToList();
                            resulPriceWareHouse = (from a in resulPriceWareHouse
                                                   join b in resulPriceWareHouses on new
                                                   {
                                                       a.nRow,
                                                       a.WarehouseCode,
                                                       a.ProductCode,
                                                       a.ProductLotCode,
                                                       a.ProductOriginCode
                                                   } equals new
                                                   {
                                                       b.nRow,
                                                       b.WarehouseCode,
                                                       b.ProductCode,
                                                       b.ProductLotCode,
                                                       b.ProductOriginCode
                                                   } into c
                                                   from d in c.DefaultIfEmpty()
                                                   select new
                                                   {
                                                       nRow = a.nRow,
                                                       a.WarehouseCode,
                                                       a.ProductCode,
                                                       a.ProductOriginCode,
                                                       a.ProductLotCode,
                                                       a.Quantity,
                                                       PriceCurX = (decimal)(d != null ? Math.Abs(d.PriceCurX) : Math.Abs(a.PriceCurX)),
                                                       a.AmountCur,
                                                       PriceX = (decimal)(d != null ? Math.Abs(d.PriceX) : Math.Abs(a.PriceX)),
                                                       a.Amount
                                                   }).ToList();
                        }




                        i -= 1;

                    }



                    // Update và xử lý trên bảng tạm trước.
                    var resulWareHouseTmp1 = from a in wareHouseBookTmp
                                             join b in resulPriceWareHouse on new { a.ProductCode, a.WarehouseCode, a.ProductLotCode, a.ProductOriginCode } equals new { b.ProductCode, b.WarehouseCode, b.ProductLotCode, b.ProductOriginCode }
                                             where a.PriceCalculatingMethod == "B" && a.FixedPrice != true && (a.VoucherGroup == 2 || a.IsTransfer == "C")
                                             select new
                                             {
                                                 a.OrgCode,
                                                 a.ProductVoucherId,
                                                 a.Id,
                                                 SttRec = a.ProductVoucherId,
                                                 a.Ord0,
                                                 a.VoucherCode,
                                                 a.VoucherGroup,
                                                 a.VoucherDate,
                                                 a.VoucherNumber,
                                                 a.BusinessCode,
                                                 a.DebitAcc,
                                                 a.CreditAcc,
                                                 a.WarehouseCode,
                                                 a.TransWarehouseCode,
                                                 a.ProductCode,
                                                 a.ProductLotCode,
                                                 a.ProductOriginCode,
                                                 a.UnitCode,
                                                 TrxQuantity = a.TrxQuantity,
                                                 a.Quantity,
                                                 PriceCb = a.TrxQuantity != 0 ? b.PriceX * a.Quantity / a.TrxQuantity : 0,
                                                 PriceCurCb = a.TrxQuantity != 0 ? b.PriceCurX * a.Quantity / a.TrxQuantity : 0,
                                                 Price = b.PriceX,
                                                 PriceCur = b.PriceCurX,
                                                 Amount = b.PriceX * a.Quantity,
                                                 AmountCur = b.PriceCurX * a.Quantity,
                                                 a.ImportQuantity,
                                                 ImportAmount = a.VoucherGroup == 1 ? b.PriceX * a.ImportQuantity : 0,
                                                 ImportAmountCur = a.VoucherGroup == 1 ? b.PriceCurX * a.ImportQuantity : 0,
                                                 a.ExportQuantity,
                                                 ExportAmount = a.VoucherGroup == 2 ? b.PriceX * a.ExportQuantity : 0,
                                                 ExportAmountCur = a.VoucherGroup == 2 ? b.PriceX * a.ExportQuantity : 0,
                                                 a.PriceCalculatingMethod,
                                                 a.IsTransfer,//dc_nb
                                                 a.IsAssembly,
                                                 a.FixedPrice,
                                                 GetPrice = a.GetPrice,
                                                 OnTop = a.OnTop,
                                                 AssemblyTransfer = a.AssemblyTransfer

                                             };
                    wareHouseBookTmp = (from a in wareHouseBookTmp
                                        join b in resulWareHouseTmp1 on a.Id equals b.Id into c
                                        from d in c.DefaultIfEmpty()
                                        select new
                                        {
                                            a.OrgCode,
                                            a.ProductVoucherId,
                                            a.Id,
                                            SttRec = a.SttRec,
                                            a.Ord0,
                                            a.VoucherCode,
                                            a.VoucherGroup,
                                            a.VoucherDate,
                                            a.VoucherNumber,
                                            a.BusinessCode,
                                            a.DebitAcc,
                                            a.CreditAcc,
                                            a.WarehouseCode,
                                            a.TransWarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode,
                                            a.UnitCode,
                                            TrxQuantity = a.TrxQuantity,
                                            a.Quantity,
                                            PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                            PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                            Price = d != null ? d.Price : a.Price,
                                            PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                            Amount = d != null ? d.Amount : a.Amount,
                                            AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                            a.ImportQuantity,
                                            ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                            ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                            a.ExportQuantity,
                                            ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                            ExportAmountCur = d != null ? d.ExportAmountCur : a.ExportAmountCur,
                                            a.PriceCalculatingMethod,
                                            a.IsTransfer,//dc_nb
                                            a.IsAssembly,
                                            a.FixedPrice,
                                            GetPrice = a.GetPrice,
                                            OnTop = a.OnTop,
                                            AssemblyTransfer = a.AssemblyTransfer,
                                            TransProductCode = a.TransProductCode
                                        }).ToList();
                    //Những dòng hàng xuất điều chuyển -- chuyển đổi
                    var trantProduct = from a in wareHouseBookTmp
                                       join b in resulPriceWareHouse on new
                                       {
                                           a.WarehouseCode,
                                           a.ProductCode,
                                           a.ProductLotCode,
                                           a.ProductOriginCode
                                       } equals new
                                       {
                                           b.WarehouseCode,
                                           b.ProductCode,
                                           b.ProductLotCode,
                                           b.ProductOriginCode
                                       } into c
                                       from d in c.DefaultIfEmpty()
                                       where a.PriceCalculatingMethod == "B" && a.FixedPrice != true && a.VoucherGroup == 2 && a.IsTransfer == "C"
                                       select new
                                       {
                                           a.SttRec,
                                           a.Ord0,
                                           a.Price,
                                           a.PriceCur,
                                           a.Amount,
                                           a.AmountCur
                                       };

                    var updateWareHouseBookTmp2 = from a in wareHouseBookTmp
                                                  join b in trantProduct on new { a.SttRec, a.Ord0 } equals new { b.SttRec, b.Ord0 }
                                                  select new
                                                  {
                                                      PriceCalculatingMethod = "T",
                                                      a.SttRec,
                                                      b.Amount,
                                                      b.AmountCur,
                                                      b.Price,
                                                      b.PriceCur
                                                  };
                    wareHouseBookTmp = (from a in wareHouseBookTmp
                                        join b in updateWareHouseBookTmp2 on a.SttRec equals b.SttRec into c
                                        from d in c.DefaultIfEmpty()
                                        select new
                                        {
                                            a.OrgCode,
                                            a.ProductVoucherId,
                                            a.Id,
                                            SttRec = a.SttRec,
                                            a.Ord0,
                                            a.VoucherCode,
                                            a.VoucherGroup,
                                            a.VoucherDate,
                                            a.VoucherNumber,
                                            a.BusinessCode,
                                            a.DebitAcc,
                                            a.CreditAcc,
                                            a.WarehouseCode,
                                            a.TransWarehouseCode,
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode,
                                            a.UnitCode,
                                            TrxQuantity = a.TrxQuantity,
                                            a.Quantity,
                                            PriceCb = d != null ? d.Price : a.PriceCb,
                                            PriceCurCb = d != null ? d.PriceCur : a.PriceCurCb,
                                            Price = d != null ? d.Price : a.Price,
                                            PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                            Amount = d != null ? d.Amount : a.Amount,
                                            AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                            a.ImportQuantity,
                                            ImportAmount = d != null ? d.Amount : a.ImportAmount,
                                            ImportAmountCur = d != null ? d.Amount : a.ImportAmountCur,
                                            a.ExportQuantity,
                                            ExportAmount = a.ExportAmount,
                                            ExportAmountCur = a.ExportAmountCur,
                                            PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                            a.IsTransfer,//dc_nb
                                            a.IsAssembly,
                                            a.FixedPrice,
                                            GetPrice = a.GetPrice,
                                            OnTop = a.OnTop,
                                            AssemblyTransfer = a.AssemblyTransfer,
                                            TransProductCode = a.TransProductCode
                                        }).ToList();
                    // Update vào phiếu xuất lắp ráp, hoặc tách thành phẩm
                    var lstDc = (from a in wareHouseBookTmp
                                 where a.PriceCalculatingMethod == "B" && a.IsAssembly == "C"
                                 && a.ProductCode == dto.ProductCode
                                 && a.ProductLotCode == dto.ProductLotCode
                                 && a.ProductOriginCode == dto.ProductOriginCode
                                 group new
                                 {
                                     a.SttRec,
                                     a.VoucherCode
                                 } by new
                                 {
                                     a.SttRec,
                                     a.VoucherCode
                                 } into gr
                                 select new
                                 {
                                     SttRec = gr.Key.SttRec,
                                     VoucherCode = gr.Key.VoucherCode
                                 }).ToList();
                    decimal? importAmountCur = 0;
                    decimal? importAmount = 0;
                    decimal? exportQuantity = 0;
                    decimal? exportAmountCur = 0;
                    decimal? exportAmount = 0;
                    foreach (var items in lstDc)
                    {
                        exportAmountCur = wareHouseBookTmp.Where(p => p.SttRec == sttRec && p.VoucherGroup == 2).Select(p => p.ImportAmountCur).Sum();
                        exportAmount = wareHouseBookTmp.Where(p => p.SttRec == sttRec && p.VoucherGroup == 2).Select(p => p.ImportAmount).Sum();

                        exportQuantity = wareHouseBookTmp.Where(p => p.SttRec == sttRec && p.VoucherGroup == 2).Select(p => p.ExportQuantity).Sum();
                        var detaiDetail = from a in wareHouseBookTmp
                                          join b in lstproductUnit on new { a.ProductCode, a.UnitCode } equals new { b.ProductCode, b.UnitCode }
                                          where a.SttRec == items.SttRec && a.VoucherGroup == 1
                                          select new
                                          {
                                              a.Ord0,
                                              a.ProductCode,
                                              a.TrxQuantity,
                                              a.Quantity,
                                              b.PurchasePrice,
                                              b.PurchasePriceCur,
                                              AmountPb = a.Quantity * b.PurchasePrice,
                                              AmountCurPb = a.Quantity * b.PurchasePriceCur,
                                              ExchanRate = (decimal)100,
                                              ExchanRateCur = (decimal)100,
                                              AmountPb0 = a.Quantity * b.PurchasePrice,
                                              AmountCurPb0 = a.Quantity * b.PurchasePriceCur,
                                          };
                        importAmount = detaiDetail.Select(p => p.AmountPb).Sum();
                        importAmountCur = detaiDetail.Select(p => p.AmountCurPb).Sum();
                        if (importAmount != 0)
                        {
                            detaiDetail = from a in detaiDetail
                                          select new
                                          {
                                              a.Ord0,
                                              a.ProductCode,
                                              a.TrxQuantity,
                                              a.Quantity,
                                              a.PurchasePrice,
                                              a.PurchasePriceCur,
                                              AmountPb = a.Quantity,
                                              AmountCurPb = a.Quantity,
                                              ExchanRate = a.ExchanRate,
                                              ExchanRateCur = a.ExchanRateCur,
                                              AmountPb0 = a.Quantity,
                                              AmountCurPb0 = a.Quantity,
                                          };
                            importAmount = detaiDetail.Select(p => p.AmountPb).Sum();
                            importAmountCur = detaiDetail.Select(p => p.AmountCurPb).Sum();
                        }
                        if (importAmount != 0)
                        {
                            detaiDetail = from a in detaiDetail
                                          select new
                                          {
                                              a.Ord0,
                                              a.ProductCode,
                                              a.TrxQuantity,
                                              a.Quantity,
                                              a.PurchasePrice,
                                              a.PurchasePriceCur,
                                              AmountPb = a.Quantity,
                                              AmountCurPb = a.Quantity,
                                              ExchanRate = (decimal)(a.AmountPb / importAmount),
                                              ExchanRateCur = a.ExchanRateCur,
                                              AmountPb0 = a.Quantity,
                                              AmountCurPb0 = a.AmountPb / importAmount * exportAmount,
                                          };
                        }
                        if (importAmountCur != 0)
                        {
                            detaiDetail = from a in detaiDetail
                                          select new
                                          {
                                              a.Ord0,
                                              a.ProductCode,
                                              a.TrxQuantity,
                                              a.Quantity,
                                              a.PurchasePrice,
                                              a.PurchasePriceCur,
                                              AmountPb = a.Quantity,
                                              AmountCurPb = a.Quantity,
                                              ExchanRate = a.ExchanRate,
                                              ExchanRateCur = (decimal)(a.AmountCurPb / importAmountCur),
                                              AmountPb0 = a.Quantity,
                                              AmountCurPb0 = a.AmountCurPb / importAmountCur * exportAmountCur,
                                          };
                        }

                        int lgLr = 0;
                        if (items.VoucherCode.Contains(listCtLrs) == true)
                        {
                            lgLr = 1;
                        }
                        var productVoucher = await _productVoucherService.GetQueryableAsync();
                        var lstProductVoucher = productVoucher.Where(p => p.Id == items.SttRec).ToList();
                        var productVoucherAssembly = await _productVoucherAssembly.GetQueryableAsync();
                        var lstproductVoucherAssembly = productVoucherAssembly.Where(p => p.ProductVoucherId == sttRec && p.Quantity != 0).ToList();
                        decimal? quantity = lstproductVoucherAssembly.Select(p => p.Quantity).FirstOrDefault();
                        foreach (var itemm in lstproductVoucherAssembly)
                        {
                            itemm.Price = exportAmount / quantity;
                            itemm.PriceCur = exportAmountCur / quantity;
                            await _productVoucherAssembly.UpdateAsync(itemm, true);
                        }
                        decimal amount = 0;
                        decimal amountCur = 0;
                        // Phieu tach vat tu

                        if (lgLr == 0)
                        {
                            //-- Fix checnh lech
                            amount = (decimal)detaiDetail.Select(p => p.AmountPb0).Sum();
                            amountCur = (decimal)detaiDetail.Select(p => p.AmountCurPb0).Sum();
                            ord0 = detaiDetail.OrderBy(p => p.Ord0).Where(p => p.AmountPb0 != 0).Select(p => p.Ord0).FirstOrDefault();
                            if (exportAmount != amount)
                            {
                                var updateDetail = from a in detaiDetail
                                                   where a.Ord0 == ord0
                                                   select new
                                                   {
                                                       a.Ord0,
                                                       AmountPb0 = a.AmountPb0 + (exportAmount - amount)
                                                   };
                                detaiDetail = from a in detaiDetail
                                              join b in updateDetail on a.Ord0 equals b.Ord0 into c
                                              from d in c.DefaultIfEmpty()
                                              select new
                                              {
                                                  a.Ord0,
                                                  a.ProductCode,
                                                  a.TrxQuantity,
                                                  a.Quantity,
                                                  a.PurchasePrice,
                                                  a.PurchasePriceCur,
                                                  AmountPb = a.Quantity,
                                                  AmountCurPb = a.Quantity,
                                                  ExchanRate = a.ExchanRate,
                                                  ExchanRateCur = a.ExchanRateCur,
                                                  AmountPb0 = d != null ? d.AmountPb0 : a.AmountPb0,
                                                  AmountCurPb0 = a.AmountCurPb0
                                              };
                            }
                            if (exportAmountCur != amountCur)
                            {
                                var updateDetail = from a in detaiDetail
                                                   where a.Ord0 == ord0
                                                   select new
                                                   {
                                                       a.Ord0,
                                                       AmountCurPb0 = a.AmountCurPb0 + (exportAmountCur - amountCur)
                                                   };
                                detaiDetail = from a in detaiDetail
                                              join b in updateDetail on a.Ord0 equals b.Ord0 into c
                                              from d in c.DefaultIfEmpty()
                                              select new
                                              {
                                                  a.Ord0,
                                                  a.ProductCode,
                                                  a.TrxQuantity,
                                                  a.Quantity,
                                                  a.PurchasePrice,
                                                  a.PurchasePriceCur,
                                                  AmountPb = a.Quantity,
                                                  AmountCurPb = a.Quantity,
                                                  ExchanRate = a.ExchanRate,
                                                  ExchanRateCur = a.ExchanRateCur,
                                                  AmountPb0 = a.AmountPb0,
                                                  AmountCurPb0 = d != null ? d.AmountCurPb0 : a.AmountCurPb0
                                              };
                            }
                            var lstDetail = detaiDetail.OrderBy(p => p.Ord0).ToList();
                            //Update phan chi tiet phieu
                            foreach (var detail in lstDetail)
                            {
                                var updateWareHousetmp = from a in wareHouseBookTmp
                                                         where a.SttRec == items.SttRec & a.Ord0 == detail.Ord0
                                                         select new
                                                         {
                                                             PriceCalculatingMethod = "T",
                                                             Price = detail.PurchasePrice,
                                                             PriceCur = detail.PurchasePriceCur,
                                                             PriceCb = detail.PurchasePrice,
                                                             PriceCurCb = detail.PurchasePriceCur,
                                                             Amount = detail.AmountPb0,
                                                             AmountCur = detail.AmountCurPb0,
                                                             ImportAmount = detail.AmountPb0,
                                                             ImportAmountCur = detail.AmountCurPb0,
                                                             a.SttRec,
                                                             a.Ord0

                                                         };
                                wareHouseBookTmp = (from a in wareHouseBookTmp
                                                    join b in updateWareHousetmp on new
                                                    {
                                                        a.SttRec,
                                                        a.Ord0
                                                    } equals new
                                                    {
                                                        b.SttRec,
                                                        b.Ord0
                                                    } into c
                                                    from d in c.DefaultIfEmpty()
                                                    select new
                                                    {
                                                        a.OrgCode,
                                                        a.ProductVoucherId,
                                                        a.Id,
                                                        SttRec = a.ProductVoucherId,
                                                        a.Ord0,
                                                        a.VoucherCode,
                                                        a.VoucherGroup,
                                                        a.VoucherDate,
                                                        a.VoucherNumber,
                                                        a.BusinessCode,
                                                        a.DebitAcc,
                                                        a.CreditAcc,
                                                        a.WarehouseCode,
                                                        a.TransWarehouseCode,
                                                        a.ProductCode,
                                                        a.ProductLotCode,
                                                        a.ProductOriginCode,
                                                        a.UnitCode,
                                                        TrxQuantity = a.TrxQuantity,
                                                        a.Quantity,
                                                        PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                        PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                        Price = d != null ? d.Price : a.Price,
                                                        PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                        Amount = d != null ? d.Amount : a.Amount,
                                                        AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                        a.ImportQuantity,
                                                        ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                        ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                        a.ExportQuantity,
                                                        a.ExportAmount,
                                                        a.ExportAmountCur,
                                                        PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                        a.IsTransfer,//dc_nb
                                                        a.IsAssembly,
                                                        a.FixedPrice,
                                                        GetPrice = a.GetPrice,
                                                        OnTop = a.OnTop,
                                                        AssemblyTransfer = a.AssemblyTransfer,
                                                        TransProductCode = a.TransProductCode
                                                    }).ToList();
                                var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
                                var lstProductVoucherDetail = productVoucherDetail.Where(p => p.ProductVoucherId == sttRec && p.Ord0 == detail.Ord0).ToList();
                                foreach (var itemm in lstProductVoucherDetail)
                                {
                                    itemm.Price = detail.PurchasePrice;
                                    itemm.PriceCur = detail.PurchasePriceCur;
                                    itemm.Amount = detail.AmountPb0;
                                    itemm.AmountCur = detail.AmountCurPb0;
                                    await _productVoucherDetail.UpdateAsync(itemm, true);
                                }
                            }

                        }
                        else
                        {
                            var updateWareHouseTmp = from a in wareHouseBookTmp
                                                     where a.SttRec == items.SttRec && a.Ord0 == "B000000001"
                                                     select new
                                                     {
                                                         a.SttRec,
                                                         PriceCalculatingMethod = "T",
                                                         Price = a.Quantity != 0 ? exportAmount / a.Quantity : 0,
                                                         PriceCur = a.Quantity != 0 ? exportAmountCur / a.Quantity : 0,
                                                         PriceCb = a.Quantity != 0 ? exportAmount / a.Quantity : 0,
                                                         PriceCurCb = a.Quantity != 0 ? exportAmountCur / a.Quantity : 0,
                                                         Amount = exportAmount,
                                                         AmountCur = exportAmountCur,
                                                         ImportAmount = exportAmount,
                                                         ImportAmountCur = exportAmountCur,
                                                         a.Ord0
                                                     };
                            wareHouseBookTmp = (from a in wareHouseBookTmp
                                                join b in updateWareHouseTmp on new
                                                {
                                                    a.SttRec,
                                                    a.Ord0
                                                } equals new
                                                {
                                                    b.SttRec,
                                                    b.Ord0
                                                } into c
                                                from d in c.DefaultIfEmpty()
                                                select new
                                                {
                                                    a.OrgCode,
                                                    a.ProductVoucherId,
                                                    a.Id,
                                                    SttRec = a.ProductVoucherId,
                                                    a.Ord0,
                                                    a.VoucherCode,
                                                    a.VoucherGroup,
                                                    a.VoucherDate,
                                                    a.VoucherNumber,
                                                    a.BusinessCode,
                                                    a.DebitAcc,
                                                    a.CreditAcc,
                                                    a.WarehouseCode,
                                                    a.TransWarehouseCode,
                                                    a.ProductCode,
                                                    a.ProductLotCode,
                                                    a.ProductOriginCode,
                                                    a.UnitCode,
                                                    TrxQuantity = a.TrxQuantity,
                                                    a.Quantity,
                                                    PriceCb = d != null ? d.PriceCb : a.PriceCb,
                                                    PriceCurCb = d != null ? d.PriceCurCb : a.PriceCurCb,
                                                    Price = d != null ? d.Price : a.Price,
                                                    PriceCur = d != null ? d.PriceCur : a.PriceCur,
                                                    Amount = d != null ? d.Amount : a.Amount,
                                                    AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                    a.ImportQuantity,
                                                    ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                    ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                    a.ExportQuantity,
                                                    a.ExportAmount,
                                                    a.ExportAmountCur,
                                                    PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                    a.IsTransfer,//dc_nb
                                                    a.IsAssembly,
                                                    a.FixedPrice,
                                                    GetPrice = a.GetPrice,
                                                    OnTop = a.OnTop,
                                                    AssemblyTransfer = a.AssemblyTransfer,
                                                    TransProductCode = a.TransProductCode
                                                }).ToList();
                        }

                    }
                    //Tính tồn cuối
                    var resulOpeningbalanceProductcl0 = (from a in resulOpeningbalanceProduct
                                                         select new
                                                         {
                                                             a.WarehouseCode,
                                                             a.ImportQuantity0,
                                                             a.ImportAmountCur0,
                                                             a.ImportAmount0
                                                         }).ToList();

                    var insertProductcl0 = (from a in wareHouseBookTmp
                                            join b in priceWareHouse on new
                                            {
                                                a.WarehouseCode,
                                                a.ProductCode,
                                                a.ProductLotCode,
                                                a.ProductOriginCode
                                            } equals new
                                            {
                                                b.WarehouseCode,
                                                b.ProductCode,
                                                b.ProductLotCode,
                                                b.ProductOriginCode
                                            }

                                            select new
                                            {
                                                a.WarehouseCode,
                                                ImportQuantity0 = (decimal)(a.ImportQuantity - a.ExportQuantity),
                                                ImportAmountCur0 = (decimal)(a.ImportAmountCur - a.ExportAmountCur),
                                                ImportAmount0 = (decimal)(a.ImportAmount - a.ExportAmount)

                                            }).ToList();

                    resulOpeningbalanceProductcl0.AddRange(insertProductcl0);
                    var resulOpeningbalanceProductcl = (from a in resulOpeningbalanceProductcl0
                                                        group new
                                                        {
                                                            a.WarehouseCode,
                                                            a.ImportQuantity0,
                                                            a.ImportAmount0
                                                        } by new
                                                        {
                                                            a.WarehouseCode
                                                        }
                                                       into gr
                                                        where gr.Sum(p => p.ImportQuantity0) == 0 && Math.Abs(gr.Sum(p => p.ImportAmount0)) <= v
                                                        select new
                                                        {
                                                            WarehouseCode = gr.Key.WarehouseCode,
                                                            ImportAmount0 = gr.Sum(p => p.ImportAmount0),
                                                            ImportQuantity0 = gr.Sum(p => p.ImportQuantity0)
                                                        }).ToList();

                    //Những mặt hàng tồn hết còn tiền
                    if (resulOpeningbalanceProductcl.Count > 0)
                    {
                        foreach (var itemo in resulOpeningbalanceProductcl)
                        {
                            var resul = (from a in wareHouseBookTmp
                                         where a.PriceCalculatingMethod == "B" && a.VoucherGroup == 2
                                         && a.WarehouseCode == itemo.WarehouseCode
                                         && a.ProductCode == item.ProductCode
                                         && a.ProductLotCode == item.ProductLotCode
                                         && a.ProductOriginCode == item.ProductOriginCode
                                         select new
                                         {
                                             a.SttRec,
                                             a.Ord0,
                                             a.IsAssembly,
                                             a.IsTransfer,
                                             a.TransWarehouseCode,
                                             a.ImportQuantity
                                         }).ToList().OrderByDescending(p => p.IsAssembly).OrderByDescending(p => p.ImportQuantity).FirstOrDefault();
                            ord0 = resul.Ord0;
                            sttRec = resul.SttRec;
                            var isAssembly = resul.IsAssembly;
                            var IsTransfer = resul.IsTransfer;//dc
                            var transWarehouseCode = resul.TransWarehouseCode;
                            if (ord0 != null)
                            {
                                exportAmount = resulOpeningbalanceProductcl.Where(p => p.WarehouseCode == itemo.WarehouseCode).Select(p => p.ImportAmount0).Sum();
                                exportAmountCur = resulOpeningbalanceProductcl.Where(p => p.WarehouseCode == itemo.WarehouseCode).Select(p => p.ImportQuantity0).Sum();
                                var updateWareTmp = from a in wareHouseBookTmp
                                                    where a.Ord0 == ord0 && a.SttRec == a.SttRec
                                                    select new
                                                    {
                                                        a.Ord0,
                                                        a.SttRec,
                                                        Amount = a.Amount + exportAmount,
                                                        AmountCur = a.AmountCur + exportAmountCur,
                                                        ExportAmount = a.ExportAmount + exportAmount,
                                                        ExportAmountCur = a.ExportAmountCur + exportAmountCur
                                                    };
                                wareHouseBookTmp = (from a in wareHouseBookTmp
                                                    join b in updateWareTmp on new
                                                    {
                                                        a.SttRec,
                                                        a.Ord0
                                                    } equals new
                                                    {
                                                        b.SttRec,
                                                        b.Ord0
                                                    } into c
                                                    from d in c.DefaultIfEmpty()
                                                    select new
                                                    {

                                                        a.OrgCode,
                                                        a.ProductVoucherId,
                                                        a.Id,
                                                        SttRec = a.SttRec,
                                                        a.Ord0,
                                                        a.VoucherCode,
                                                        a.VoucherGroup,
                                                        a.VoucherDate,
                                                        a.VoucherNumber,
                                                        a.BusinessCode,
                                                        a.DebitAcc,
                                                        a.CreditAcc,
                                                        a.WarehouseCode,
                                                        a.TransWarehouseCode,
                                                        a.ProductCode,
                                                        a.ProductLotCode,
                                                        a.ProductOriginCode,
                                                        a.UnitCode,
                                                        TrxQuantity = a.TrxQuantity,
                                                        a.Quantity,
                                                        PriceCb = a.PriceCb,
                                                        PriceCurCb = a.PriceCurCb,
                                                        Price = a.Price,
                                                        PriceCur = a.PriceCur,
                                                        Amount = d != null ? d.Amount : a.Amount,
                                                        AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                        a.ImportQuantity,
                                                        ImportAmount = a.ImportAmount,
                                                        ImportAmountCur = a.ImportAmountCur,
                                                        a.ExportQuantity,
                                                        ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                                        ExportAmountCur = d != null ? d.ExportAmountCur : a.ExportAmountCur,
                                                        PriceCalculatingMethod = a.PriceCalculatingMethod,
                                                        a.IsTransfer,//dc_nb
                                                        a.IsAssembly,
                                                        a.FixedPrice,
                                                        GetPrice = a.GetPrice,
                                                        OnTop = a.OnTop,
                                                        AssemblyTransfer = a.AssemblyTransfer,
                                                        TransProductCode = a.TransProductCode
                                                    }).ToList();

                            }
                            if (IsTransfer != null || IsTransfer != "")
                            {
                                //check sttect0 lại

                                var updatewareHouseBookTmp = from a in wareHouseBookTmp
                                                             where a.SttRec == sttRec && a.Ord0 == ord0
                                                             select new
                                                             {

                                                                 a.OrgCode,
                                                                 a.ProductVoucherId,
                                                                 a.Id,
                                                                 SttRec = a.SttRec,
                                                                 a.Ord0,
                                                                 a.VoucherCode,
                                                                 a.VoucherGroup,
                                                                 a.VoucherDate,
                                                                 a.VoucherNumber,
                                                                 a.BusinessCode,
                                                                 a.DebitAcc,
                                                                 a.CreditAcc,
                                                                 a.WarehouseCode,
                                                                 a.TransWarehouseCode,
                                                                 a.ProductCode,
                                                                 a.ProductLotCode,
                                                                 a.ProductOriginCode,
                                                                 a.UnitCode,
                                                                 TrxQuantity = a.TrxQuantity,
                                                                 a.Quantity,
                                                                 PriceCb = a.PriceCb,
                                                                 PriceCurCb = a.PriceCurCb,
                                                                 Price = a.Price,
                                                                 PriceCur = a.PriceCur,
                                                                 Amount = a.Amount + exportAmount,
                                                                 AmountCur = exportAmountCur + a.AmountCur,
                                                                 a.ImportQuantity,
                                                                 ImportAmount = exportAmount + a.ImportAmount,
                                                                 ImportAmountCur = exportAmountCur + a.ImportAmountCur,
                                                                 a.ExportQuantity,
                                                                 ExportAmount = a.ExportAmount,
                                                                 ExportAmountCur = a.ExportAmountCur,
                                                                 PriceCalculatingMethod = "T",
                                                                 a.IsTransfer,//dc_nb
                                                                 a.IsAssembly,
                                                                 a.FixedPrice,
                                                                 GetPrice = a.GetPrice,
                                                                 OnTop = a.OnTop,
                                                                 AssemblyTransfer = a.AssemblyTransfer
                                                             };
                                wareHouseBookTmp = (from a in wareHouseBookTmp
                                                    join b in updatewareHouseBookTmp on new
                                                    {
                                                        a.SttRec,
                                                        a.Ord0
                                                    } equals new
                                                    {
                                                        b.SttRec,
                                                        b.Ord0
                                                    } into c
                                                    from d in c.DefaultIfEmpty()
                                                    select new
                                                    {

                                                        a.OrgCode,
                                                        a.ProductVoucherId,
                                                        a.Id,
                                                        SttRec = a.SttRec,
                                                        a.Ord0,
                                                        a.VoucherCode,
                                                        a.VoucherGroup,
                                                        a.VoucherDate,
                                                        a.VoucherNumber,
                                                        a.BusinessCode,
                                                        a.DebitAcc,
                                                        a.CreditAcc,
                                                        a.WarehouseCode,
                                                        a.TransWarehouseCode,
                                                        a.ProductCode,
                                                        a.ProductLotCode,
                                                        a.ProductOriginCode,
                                                        a.UnitCode,
                                                        TrxQuantity = a.TrxQuantity,
                                                        a.Quantity,
                                                        PriceCb = a.PriceCb,
                                                        PriceCurCb = a.PriceCurCb,
                                                        Price = a.Price,
                                                        PriceCur = a.PriceCur,
                                                        Amount = d != null ? d.Amount : a.Amount,
                                                        AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                        a.ImportQuantity,
                                                        ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                        ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                        a.ExportQuantity,
                                                        ExportAmount = a.ExportAmount,
                                                        ExportAmountCur = a.ExportAmountCur,
                                                        PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                        a.IsTransfer,//dc_nb
                                                        a.IsAssembly,
                                                        a.FixedPrice,
                                                        GetPrice = a.GetPrice,
                                                        OnTop = a.OnTop,
                                                        AssemblyTransfer = a.AssemblyTransfer,
                                                        TransProductCode = a.TransProductCode
                                                    }).ToList();
                            }
                            if (isAssembly != null)
                            {
                                var exportall = (from a in wareHouseBookTmp
                                                 where a.SttRec == sttRec && a.VoucherGroup == 1
                                                 select new
                                                 {
                                                     a.SttRec,
                                                     a.Ord0
                                                 }).OrderBy(p => p.Ord0).FirstOrDefault();
                                if (exportall != null)
                                {
                                    sttRec = exportall.SttRec;
                                    ord0 = exportall.Ord0;
                                    if (ord0 != null)
                                    {
                                        var updatewareHouseBookTmp = from a in wareHouseBookTmp
                                                                     where a.SttRec == sttRec && a.Ord0 == ord0
                                                                     select new
                                                                     {

                                                                         a.OrgCode,
                                                                         a.ProductVoucherId,
                                                                         a.Id,
                                                                         SttRec = a.SttRec,
                                                                         a.Ord0,
                                                                         a.VoucherCode,
                                                                         a.VoucherGroup,
                                                                         a.VoucherDate,
                                                                         a.VoucherNumber,
                                                                         a.BusinessCode,
                                                                         a.DebitAcc,
                                                                         a.CreditAcc,
                                                                         a.WarehouseCode,
                                                                         a.TransWarehouseCode,
                                                                         a.ProductCode,
                                                                         a.ProductLotCode,
                                                                         a.ProductOriginCode,
                                                                         a.UnitCode,
                                                                         TrxQuantity = a.TrxQuantity,
                                                                         a.Quantity,
                                                                         PriceCb = a.PriceCb,
                                                                         PriceCurCb = a.PriceCurCb,
                                                                         Price = a.Price,
                                                                         PriceCur = a.PriceCur,
                                                                         Amount = a.Amount + exportAmount,
                                                                         AmountCur = exportAmountCur + a.AmountCur,
                                                                         a.ImportQuantity,
                                                                         ImportAmount = exportAmount + a.ImportAmount,
                                                                         ImportAmountCur = exportAmountCur + a.ImportAmountCur,
                                                                         a.ExportQuantity,
                                                                         ExportAmount = a.ExportAmount,
                                                                         ExportAmountCur = a.ExportAmountCur,
                                                                         PriceCalculatingMethod = "T",
                                                                         a.IsTransfer,//dc_nb
                                                                         a.IsAssembly,
                                                                         a.FixedPrice,
                                                                         GetPrice = a.GetPrice,
                                                                         OnTop = a.OnTop,
                                                                         AssemblyTransfer = a.AssemblyTransfer
                                                                     };
                                        wareHouseBookTmp = (from a in wareHouseBookTmp
                                                            join b in updatewareHouseBookTmp on new
                                                            {
                                                                a.SttRec,
                                                                a.Ord0
                                                            } equals new
                                                            {
                                                                b.SttRec,
                                                                b.Ord0
                                                            } into c
                                                            from d in c.DefaultIfEmpty()
                                                            select new
                                                            {

                                                                a.OrgCode,
                                                                a.ProductVoucherId,
                                                                a.Id,
                                                                SttRec = a.SttRec,
                                                                a.Ord0,
                                                                a.VoucherCode,
                                                                a.VoucherGroup,
                                                                a.VoucherDate,
                                                                a.VoucherNumber,
                                                                a.BusinessCode,
                                                                a.DebitAcc,
                                                                a.CreditAcc,
                                                                a.WarehouseCode,
                                                                a.TransWarehouseCode,
                                                                a.ProductCode,
                                                                a.ProductLotCode,
                                                                a.ProductOriginCode,
                                                                a.UnitCode,
                                                                TrxQuantity = a.TrxQuantity,
                                                                a.Quantity,
                                                                PriceCb = a.PriceCb,
                                                                PriceCurCb = a.PriceCurCb,
                                                                Price = a.Price,
                                                                PriceCur = a.PriceCur,
                                                                Amount = d != null ? d.Amount : a.Amount,
                                                                AmountCur = d != null ? d.AmountCur : a.AmountCur,
                                                                a.ImportQuantity,
                                                                ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                                ImportAmountCur = d != null ? d.ImportAmountCur : a.ImportAmountCur,
                                                                a.ExportQuantity,
                                                                ExportAmount = a.ExportAmount,
                                                                ExportAmountCur = a.ExportAmountCur,
                                                                PriceCalculatingMethod = d != null ? d.PriceCalculatingMethod : a.PriceCalculatingMethod,
                                                                a.IsTransfer,//dc_nb
                                                                a.IsAssembly,
                                                                a.FixedPrice,
                                                                GetPrice = a.GetPrice,
                                                                OnTop = a.OnTop,
                                                                AssemblyTransfer = a.AssemblyTransfer,
                                                                TransProductCode = a.TransProductCode
                                                            }).ToList();
                                    }

                                }

                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {

            }
            // Update vào dữ liệu nguồn

            foreach (var item in wareHouseBookTmp.Where(p => p.GetPrice == "C").ToList())
            {
                //SOKHO
                var warehouseBook = await _warehouseBookService.GetQueryableAsync();
                var lstWarehouseBook = warehouseBook.Where(p => p.ProductVoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0 && p.Year == dto.Year).ToList();
                foreach (var itemWarehouseBook in lstWarehouseBook)
                {
                    itemWarehouseBook.Price = item.PriceCb;
                    itemWarehouseBook.PriceCur = item.PriceCurCb;
                    itemWarehouseBook.Price0 = item.PriceCb;
                    itemWarehouseBook.PriceCur0 = item.PriceCurCb;
                    itemWarehouseBook.Amount = item.PriceCb * item.Quantity;
                    itemWarehouseBook.AmountCur = item.AmountCur;
                    itemWarehouseBook.ImportAmount = item.VoucherGroup == 1 ? item.ImportAmount : 0;
                    itemWarehouseBook.ImportAmountCur = item.VoucherGroup == 1 ? item.ImportAmountCur : 0;
                    itemWarehouseBook.ExportAmount = item.VoucherGroup == 2 ? item.ExportAmount : 0;
                    //itemWarehouseBook.ExportAmountCur = item.VoucherGroup == 2 ? item.ExportAmountCur : 0;
                    itemWarehouseBook.TrxPrice = item.PriceCb;
                    itemWarehouseBook.FixedPrice = false;
                    await _warehouseBookService.UpdateAsync(itemWarehouseBook, true);
                }
                //socai
                var ledger = await _ledgerService.GetQueryableAsync();
                var lstLedger = ledger.Where(p => p.VoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                foreach (var itemLedger in lstLedger)
                {
                    itemLedger.Price = item.PriceCb;
                    itemLedger.PriceCur = item.PriceCurCb;
                    itemLedger.Amount = item.PriceCb * item.Quantity;
                    itemLedger.AmountCur = item.PriceCurCb * item.Quantity;
                    await _ledgerService.UpdateAsync(itemLedger, true);
                }

                //CHI TIET
                // updata phiếu HV
                var productVoucherdetail = await _productVoucherDetail.GetQueryableAsync();
                var lstProductVoucher = productVoucherdetail.Where(p => p.ProductVoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                if (item.VoucherCode != "PTR")
                {
                    foreach (var itemProductVoucher in lstProductVoucher)
                    {
                        itemProductVoucher.Price = item.PriceCb;
                        itemProductVoucher.PriceCur = item.PriceCurCb;
                        itemProductVoucher.Amount = item.PriceCb * item.Quantity;
                        itemProductVoucher.AmountCur = item.PriceCurCb * item.Quantity;
                        await _productVoucherDetail.UpdateAsync(itemProductVoucher, true);
                    }
                }


            }
            //if (listCt.Contains(item.VoucherCode) == false && item.PriceCalculatingMethod == "B")
            //{
            //    sttRec = item.SttRec.GroupBy();
            //}
            //upadtedphv
            var sttRc = (from a in wareHouseBookTmp
                         where listCt.Contains(a.VoucherCode) == false && a.PriceCalculatingMethod == "B"
                         group new
                         {
                             a.SttRec
                         } by new
                         {
                             a.SttRec
                         } into gr
                         select new
                         {
                             SttRec = gr.Key.SttRec
                         }).ToList();
            foreach (var item in sttRc)
            {
                var productVoucherdetail = await _productVoucherDetail.GetQueryableAsync();
                var lstProductVoucher = productVoucherdetail.Where(p => p.ProductVoucherId == item.SttRec).ToList();
                var updateProduct = (from a in lstProductVoucher
                                     group new
                                     {
                                         a.ProductVoucherId,
                                         a.Amount,
                                         a.AmountCur,
                                         a.Price,
                                         a.PriceCur
                                     } by new
                                     {
                                         a.ProductVoucherId
                                     } into gr
                                     select new
                                     {

                                         ProductVoucherId = gr.Key.ProductVoucherId,
                                         Amount = gr.Sum(p => p.Amount),
                                         AmountCur = gr.Sum(p => p.AmountCur)
                                     }).ToList();
                foreach (var items in updateProduct)
                {
                    var productVoucher = await _productVoucherService.GetQueryableAsync();
                    var lstProductVouchers = productVoucher.Where(p => p.Id == items.ProductVoucherId && p.VoucherCode != "BH8").ToList();

                    foreach (var itemProductVouchers in lstProductVouchers)
                    {
                        itemProductVouchers.TotalAmount = items.Amount;
                        itemProductVouchers.TotalAmountCur = items.AmountCur;
                        itemProductVouchers.TotalProductAmount = items.Amount;
                        itemProductVouchers.TotalProductAmountCur = items.AmountCur;

                        await _productVoucherService.UpdateAsync(itemProductVouchers, true);
                    }

                }
            }


        }
        public async Task UpadateProductNorms(CostOfGoodsDto costOfGoods)
        {

            // dữ liệu tạm

            var wareHouseBook = await _warehouseBookService.GetQueryableAsync();
            var lstWareHouseBook = wareHouseBook.Where(p => p.OrgCode == costOfGoods.OrdCode
                                                            && p.VoucherDate >= costOfGoods.FromDate
                                                            && p.VoucherDate <= costOfGoods.ToDate
                                                            && p.VoucherGroup == 2
                                                            );
            var resulWareHouseBook = (from a in lstWareHouseBook
                                      group new
                                      {
                                          a.VoucherDate,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode,
                                          a.Price0,
                                          a.PriceCur0,
                                          a.ExportQuantity,
                                          a.ExportAmountCur,
                                          a.ExportAmount
                                      } by new
                                      {
                                          a.VoucherDate,
                                          a.ProductCode,
                                          a.ProductLotCode,
                                          a.ProductOriginCode
                                      } into gr
                                      select new
                                      {
                                          VoucherDate = gr.Key.VoucherDate,
                                          ProductCode = gr.Key.ProductCode,
                                          ProductLotCode = gr.Key.ProductLotCode,
                                          ProductOriginCode = gr.Key.ProductOriginCode,
                                          Price0 = gr.Max(p => p.Price0),
                                          PriceCur0 = gr.Max(p => p.PriceCur0),
                                          ExportQuantity = gr.Sum(p => p.ExportQuantity),
                                          ExportAmountCur = gr.Sum(p => p.ExportAmountCur),
                                          ExportAmount = gr.Sum(p => p.ExportAmount)
                                      }).ToList();
            // bảng giá xuất
            var priceProductMorms = (from a in resulWareHouseBook
                                     where 1 == 0
                                     select new
                                     {
                                         a.ProductCode,
                                         a.ProductLotCode,
                                         a.ProductOriginCode,
                                         a.Price0,
                                         a.PriceCur0
                                     }).ToList();
            double t = Math.Round(costOfGoods.ToDate.Subtract(costOfGoods.FromDate).Days / (365.25 / 12));
            if (t == 1)
            {
                t = costOfGoods.FromDate.Month;
            }
            var fProductWorkNormDetailService = await _fProductWorkNormDetailService.GetQueryableAsync();
            var lstfProductWorkNormDetailService = fProductWorkNormDetailService.Where(p => p.OrgCode == costOfGoods.OrdCode);
            List<CostOfGoodsDto> costOfGoodsDtos = new List<CostOfGoodsDto>();
            for (int i = costOfGoods.FromDate.Month; i <= t; i++)
            {
                if (i == costOfGoods.FromDate.Month)
                {
                    CostOfGoodsDto cost = new CostOfGoodsDto();
                    cost.FromDate = costOfGoods.FromDate.AddDays((-costOfGoods.FromDate.Day) + 1);
                    DateTime tem = new DateTime(costOfGoods.FromDate.Year, costOfGoods.FromDate.Month + 1, 1);
                    cost.ToDate = tem.AddDays(-1);
                    costOfGoodsDtos.Add(cost);
                }
                else
                {
                    costOfGoods.FromDate = costOfGoods.FromDate.AddMonths(1);
                    CostOfGoodsDto cost = new CostOfGoodsDto();
                    cost.FromDate = costOfGoods.FromDate.AddDays((-cost.FromDate.Day) + 1);

                    {
                        DateTime dtResult = new DateTime(costOfGoods.FromDate.Year, costOfGoods.FromDate.Month, 1);
                        dtResult = dtResult.AddMonths(1);
                        dtResult = dtResult.AddDays(-(dtResult.Day));
                        cost.ToDate = dtResult;
                        costOfGoodsDtos.Add(cost);
                    }

                }
            }
            foreach (var item in costOfGoodsDtos)
            {
                var lstfProductWorkNormDetailServices = lstfProductWorkNormDetailService.Where(p => p.BeginDate >= item.FromDate && p.EndDate <= item.ToDate).ToList();
                // Update CT_DMSP
                //foreach (var items in lstfProductWorkNormDetailServices)
                //{
                //    items.ProductLotCode = null;
                //    items.ProductOrigin = null;
                //    items.PriceCur = 0;
                //    items.Price = 0;
                //    items.Amount = 0;
                //    items.AmountCur = 0;
                //    await _fProductWorkNormDetailService.UpdateAsync(items);
                //}
                var priceProductMorm = (from a in resulWareHouseBook
                                        where a.VoucherDate >= item.FromDate && a.VoucherDate <= item.ToDate
                                        group new
                                        {
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode,
                                            a.ExportQuantity,
                                            a.ExportAmount,
                                            a.ExportAmountCur
                                        } by new
                                        {
                                            a.ProductCode,
                                            a.ProductLotCode,
                                            a.ProductOriginCode
                                        } into gr
                                        select new
                                        {

                                            ProductCode = gr.Key.ProductCode,
                                            ProductLotCode = gr.Key.ProductLotCode ?? null,
                                            ProductOriginCode = gr.Key.ProductOriginCode ?? null,
                                            Price0 = gr.Sum(p => p.ExportQuantity) != 0 ? gr.Sum(p => p.ExportAmount) / gr.Sum(p => p.ExportQuantity) : (decimal?)0,
                                            PriceCur0 = gr.Sum(p => p.ExportAmountCur) != 0 ? gr.Sum(p => p.ExportAmountCur) / gr.Sum(p => p.ExportQuantity) : (decimal?)0
                                        }).ToList();
                //priceProductMorms.AddRange(priceProductMorm);
                try
                {
                    if (lstfProductWorkNormDetailServices.Count > 0)
                    {
                        var resulFproductNorm = (from a in lstfProductWorkNormDetailServices
                                                 join b in priceProductMorm on new
                                                 {
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     ProductOriginCode = a.ProductOrigin
                                                 } equals new
                                                 {
                                                     b.ProductCode,
                                                     b.ProductLotCode,
                                                     b.ProductOriginCode
                                                 } into c
                                                 from d in c.DefaultIfEmpty()
                                                 select new CrudFProductWorkNormDetailDto
                                                 {
                                                     Id = a.Id,
                                                     FProductWorkNormId = a.FProductWorkNormId,
                                                     AccCode = a.AccCode,
                                                     ApplicableDate1 = a.ApplicableDate1,
                                                     ApplicableDate2 = a.ApplicableDate2,
                                                     BeginDate = a.BeginDate,
                                                     CreatorName = a.CreatorName,
                                                     EndDate = a.EndDate,
                                                     LastModifierName = a.LastModifierName,
                                                     Month = a.Month,
                                                     Ord0 = a.Ord0,
                                                     OrgCode = a.OrgCode,
                                                     PercentLoss = a.PercentLoss,
                                                     ProductCode = a.ProductCode,
                                                     ProductLotCode = a.ProductLotCode,
                                                     ProductOrigin = a.ProductOrigin,
                                                     Quantity = a.Quantity,
                                                     QuantityLoss = a.QuantityLoss,
                                                     SectionCode = a.SectionCode,
                                                     UnitCode = a.UnitCode,
                                                     WarehouseCode = a.WarehouseCode,
                                                     WorkPlaceCode = a.WorkPlaceCode,
                                                     Year = a.Year,
                                                     PriceCur = d != null ? (decimal)d.PriceCur0 : 0,
                                                     Price = d != null ? (decimal?)d.Price0 : 0,
                                                     AmountCur = d != null ? (decimal?)a.Quantity * (decimal)d.PriceCur0 : 0,
                                                     Amount = d != null ? (decimal?)a.Quantity * (decimal)d.Price0 : 0
                                                 }).ToList();

                        foreach (var items in resulFproductNorm)
                        {

                            var entity = ObjectMapper.Map<CrudFProductWorkNormDetailDto, FProductWorkNormDetail>(items);
                            await _fProductWorkNormDetailService.DeleteAsync(entity.Id, true);
                            items.Id = this.GetNewObjectId();
                            var entitys = ObjectMapper.Map<CrudFProductWorkNormDetailDto, FProductWorkNormDetail>(items);
                            //entity.Id = this.GetNewObjectId();
                            await _fProductWorkNormDetailService.CreateAsync(entitys);
                        }
                    }

                }
                catch (Exception ex)
                {

                }

            }
        }

        private async Task<List<CrudProductVoucherDetailDto>> GetProductVoucherDetailAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherDetailDto>();
            var query = await _productVoucherDetail.GetQueryableAsync();
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
            var query = await _productVoucherAssembly.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherAssembly, CrudProductVoucherAssemblyDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudProductVoucherReceiptDto>> GetProductVoucherReceiptAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherReceiptDto>();
            var query = await _productVoucherReceipt.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherReceipt, CrudProductVoucherReceiptDto>(p)).ToList();
            return result;
        }

        private async Task<List<CrudProductVoucherVatDto>> GetProductVoucherVatAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherVatDto>();
            var query = await _productVoucherVat.GetQueryableAsync();
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


    }
}


