
using System;
using Accounting.Catgories.ProductVouchers;
using System.Threading.Tasks;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.DomainServices.Windows;
using Accounting.Helpers;
using Accounting.Vouchers.VoucherNumbers;
using Volo.Abp.Uow;
using System.Linq;
using Accounting.Reports;
using System.Collections.Generic;
using Accounting.Reports.Others;
using Accounting.JsonConverters;
using Accounting.Vouchers;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Math;
using static NPOI.HSSF.Util.HSSFColor;
using Volo.Abp.ObjectMapping;
using Accounting.Business;
using Accounting.Caching;

namespace Accounting.Categories.ProductVouchers
{
    public class CostPriceUpdateAppService : AccountingAppService
    {
        #region Field
        private readonly ProductVoucherService _productVoucher;
        private readonly ProductVoucherDetailService _productVoucherDetail;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly ProductVoucherAssemblyService _productVoucherAssembly;
        private readonly ProductVoucherReceiptService _productVoucherReceipt;
        private readonly ProductVoucherVatService _productVoucherVat;
        private readonly UserService _userService;
        private readonly LedgerService _ledgerService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductService _product;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly ProductGroupService _productGroupService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WarehouseService _warehouseService;
        private readonly AccountSystemService _accountSystemService;
        private readonly ProductVoucherCostService _productVoucherCostService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        private readonly WindowService _windowService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly ExcelService _excelService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly PaymentTermService _paymentTermService;
        private readonly PaymentTermDetailService _paymentTermDetailService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly ProductVoucherAppService _productVoucherAppService;

        #endregion
        #region Ctor
        public CostPriceUpdateAppService(ProductVoucherService productVoucherService,
                                ProductVoucherDetailService productVoucherDetailService,
                                AccTaxDetailService accTaxDetailService,
                                ProductVoucherAssemblyService productVoucherAssemblyService,
                                ProductVoucherReceiptService productVoucherReceiptService,
                                ProductVoucherVatService productVoucherVatService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                AccountingCacheManager accountingCacheManager,
                                LedgerService ledger,
                                VoucherCategoryService voucherCategoryService,
                                TenantSettingService tenantSettingService,
                                TaxCategoryService taxCategoryService,
                                ProductUnitService productUnitService,
                                ProductService product,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                WarehouseBookService warehouseBookService,
                                VoucherExciseTaxService voucherExciseTaxService,
                                PartnerGroupService partnerGroupService,
                                ProductGroupService productGroupService,
                                AccPartnerService accPartnerService,
                                WarehouseService warehouseService,
                                AccountSystemService accountSystemService,
                                ProductVoucherCostService productVoucherCostService,
                                VoucherNumberBusiness voucherNumberBusiness,
                                WindowService windowService,
                                ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
                                ExcelService excelService,
                                VoucherPaymentBookService voucherPaymentBookService,
                                PaymentTermService paymentTermService,
                                PaymentTermDetailService paymentTermDetailService,
            VoucherTypeService voucherTypeService,
            ProductVoucherAppService productVoucherAppService)
        {
            _productVoucher = productVoucherService;
            _productVoucherDetail = productVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _productVoucherAssembly = productVoucherAssemblyService;
            _productVoucherReceipt = productVoucherReceiptService;
            _productVoucherVat = productVoucherVatService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _ledgerService = ledger;
            _voucherCategoryService = voucherCategoryService;
            _tenantSettingService = tenantSettingService;
            _taxCategoryService = taxCategoryService;
            _productUnitService = productUnitService;
            _product = product;
            _productOpeningBalanceService = productOpeningBalanceService;
            _warehouseBookService = warehouseBookService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _partnerGroupService = partnerGroupService;
            _productGroupService = productGroupService;
            _accPartnerService = accPartnerService;
            _warehouseService = warehouseService;
            _accountSystemService = accountSystemService;
            _productVoucherCostService = productVoucherCostService;
            _voucherNumberBusiness = voucherNumberBusiness;
            _windowService = windowService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
            _excelService = excelService;
            _voucherPaymentBookService = voucherPaymentBookService;
            _paymentTermService = paymentTermService;
            _paymentTermDetailService = paymentTermDetailService;
            _voucherTypeService = voucherTypeService;
            _productVoucherAppService = productVoucherAppService;
        }
        #endregion
        public async Task<List<CostPriceUpdateDto>> CreateAsync(ReportBaseParameterDto dto)
        {
            await _licenseBusiness.CheckExpired();
            CrudProductVoucherDto crudProductVoucherDto = new CrudProductVoucherDto();
            var queryable = await _productVoucher.GetQueryableAsync();

            var lstqueryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                && p.VoucherDate >= dto.FromDate
                                                && p.VoucherDate <= dto.ToDate
                                                && p.VoucherCode == dto.VoucherCode
                                              ).ToList();
            var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var reusl = (from a in lstqueryable
                         join b in lstProductVoucherDetail on a.Id equals b.ProductVoucherId
                         group new
                         {
                             b.ProductCode,
                             b.ProductName,
                             b.ProductLotCode,
                             b.WarehouseCode,
                             b.ProductOriginCode,
                             b.UnitCode,
                             a.VoucherCode,
                             a.VoucherGroup,
                             b.Quantity,
                             b.Amount,
                             b.Price
                         } by new
                         {
                             b.ProductCode,
                             b.ProductName,
                             b.ProductLotCode,
                             b.WarehouseCode,
                             b.ProductOriginCode,
                             b.UnitCode,
                             a.VoucherCode,
                             a.VoucherGroup,
                         } into gr
                         select new CostPriceUpdateDto
                         {

                             VoucherCode = gr.Key.VoucherCode,
                             VoucherGroup = gr.Key.VoucherGroup,
                             Quantity = gr.Sum(p => p.Quantity),
                             Amount = gr.Sum(p => p.Amount),
                             Price = gr.Sum(p => p.Amount) / gr.Sum(p => p.Quantity),
                             ProductCode = gr.Key.ProductCode,
                             ProductName = gr.Key.ProductName,
                             WarehouseCode = gr.Key.WarehouseCode,
                             ProductLotCode = gr.Key.ProductLotCode,
                             ProductOriginCode = gr.Key.ProductOriginCode,
                             UnitCode = gr.Key.UnitCode
                         }).ToList();



            return reusl;
        }
        public async Task PostPriceUpdateAsync(UpdatePriceDto updatePriceDto)
        {
            await _licenseBusiness.CheckExpired();
            var lstvoucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType = lstvoucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            int isHt2 = 0;
            if (voucherType.Contains(updatePriceDto.VoucherCode) == true)
            {
                isHt2 = 1;
            }
            var lstCost = updatePriceDto.costPriceUpdateDtos;
            var productVoucher = await _productVoucher.GetQueryableAsync();
            var lstProductVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                         && p.VoucherDate >= updatePriceDto.FromDate
                                                         && p.VoucherDate <= updatePriceDto.ToDate
                                                         && p.VoucherCode == updatePriceDto.VoucherCode
                                                         && p.Status != "2").ToList();
            var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstProductVouchersDetail = (from a in lstProductVoucher
                                            join b in lstProductVoucherDetail on a.Id equals b.ProductVoucherId
                                            join c in lstCost on new { b.ProductCode, b.ProductLotCode, b.ProductOriginCode, b.WarehouseCode } equals new { c.ProductCode, c.ProductLotCode, c.ProductOriginCode, c.WarehouseCode }
                                            select new
                                            {
                                                Id = b.Id,
                                                OrgCode = b.OrgCode,
                                                ProductVoucherId = b.ProductVoucherId,
                                                Ord0 = b.Ord0,
                                                Year = b.Year,
                                                ProductCode = b.ProductCode,
                                                TransProductCode = b.TransProductCode,
                                                ProductName = b.ProductName,
                                                ProductName0 = b.ProductName0,
                                                UnitCode = b.UnitCode,
                                                TransUnitCode = b.TransUnitCode,
                                                WarehouseCode = b.WarehouseCode,
                                                TransWarehouseCode = b.TransWarehouseCode,
                                                TrxQuantity = b.TrxQuantity,
                                                Quantity = b.Quantity,
                                                PriceCur = b.PriceCur,
                                                Price = b.Price,
                                                TrxAmountCur = b.TrxAmountCur,
                                                TrxAmount = b.TrxAmount,
                                                AmountCur = b.AmountCur,
                                                Amount = b.Amount,
                                                FixedPrice = b.FixedPrice,
                                                DebitAcc = b.DebitAcc,
                                                CreditAcc = b.CreditAcc,
                                                ProductLotCode = b.ProductLotCode,
                                                TransProductLotCode = b.TransProductLotCode,
                                                ProductOriginCode = b.ProductOriginCode,
                                                TransProductOriginCode = b.TransProductOriginCode,
                                                PartnerCode = b.PartnerCode,
                                                FProductWorkCode = b.FProductWorkCode,
                                                ContractCode = b.ContractCode,
                                                WorkPlaceCode = b.WorkPlaceCode,
                                                SectionCode = b.SectionCode,
                                                CaseCode = b.CaseCode,
                                                PriceCur2 = b.PriceCur2,
                                                Price2 = b.Price2,
                                                AmountCur2 = b.AmountCur2,
                                                Amount2 = b.Amount2,
                                                DebitAcc2 = b.DebitAcc2,
                                                CreditAcc2 = b.CreditAcc2,
                                                TaxCategoryCode = b.TaxCategoryCode,
                                                InsuranceDate = b.InsuranceDate,
                                                Note = b.Note,
                                                NoteE = b.NoteE,
                                                HTPercentage = b.HTPercentage,
                                                RevenueAmount = b.RevenueAmount,
                                                VatPriceCur = b.VatPriceCur,
                                                VatPrice = b.VatPrice,
                                                DevaluationPercentage = b.DevaluationPercentage,
                                                DevaluationPriceCur = b.DevaluationPriceCur,
                                                DevaluationPrice = b.DevaluationPrice,
                                                DevaluationAmountCur = b.DevaluationAmountCur,
                                                DevaluationAmount = b.DevaluationAmount,
                                                VarianceAmount = b.VarianceAmount,
                                                RefId = b.RefId,
                                                ProductVoucherDetailId = b.ProductVoucherDetailId,
                                                VatTaxAmount = b.VatTaxAmount,
                                                DecreasePercentage = b.DecreasePercentage,
                                                DecreaseAmount = b.DecreaseAmount,
                                                AmountWithVat = b.AmountWithVat,
                                                AmountAfterDecrease = b.AmountAfterDecrease,
                                                VatAmount = b.VatAmount,
                                                AmountUpdate = c.Amount,
                                                PriceUpdate = c.Price
                                            }).ToList();
            lstProductVouchersDetail = (from b in lstProductVouchersDetail
                                        select new
                                        {
                                            Id = b.Id,
                                            OrgCode = b.OrgCode,
                                            ProductVoucherId = b.ProductVoucherId,
                                            Ord0 = b.Ord0,
                                            Year = b.Year,
                                            ProductCode = b.ProductCode,
                                            TransProductCode = b.TransProductCode,
                                            ProductName = b.ProductName,
                                            ProductName0 = b.ProductName0,
                                            UnitCode = b.UnitCode,
                                            TransUnitCode = b.TransUnitCode,
                                            WarehouseCode = b.WarehouseCode,
                                            TransWarehouseCode = b.TransWarehouseCode,
                                            TrxQuantity = b.TrxQuantity,
                                            Quantity = b.Quantity,
                                            PriceCur = b.PriceCur,
                                            Price = b.PriceUpdate,
                                            TrxAmountCur = b.TrxAmountCur,
                                            TrxAmount = b.TrxAmount,
                                            AmountCur = b.AmountCur,
                                            Amount = b.Quantity * b.PriceUpdate,
                                            FixedPrice = b.FixedPrice,
                                            DebitAcc = b.DebitAcc,
                                            CreditAcc = b.CreditAcc,
                                            ProductLotCode = b.ProductLotCode,
                                            TransProductLotCode = b.TransProductLotCode,
                                            ProductOriginCode = b.ProductOriginCode,
                                            TransProductOriginCode = b.TransProductOriginCode,
                                            PartnerCode = b.PartnerCode,
                                            FProductWorkCode = b.FProductWorkCode,
                                            ContractCode = b.ContractCode,
                                            WorkPlaceCode = b.WorkPlaceCode,
                                            SectionCode = b.SectionCode,
                                            CaseCode = b.CaseCode,
                                            PriceCur2 = b.PriceCur2,
                                            Price2 = b.Price2,
                                            AmountCur2 = b.AmountCur2,
                                            Amount2 = b.Amount2,
                                            DebitAcc2 = b.DebitAcc2,
                                            CreditAcc2 = b.CreditAcc2,
                                            TaxCategoryCode = b.TaxCategoryCode,
                                            InsuranceDate = b.InsuranceDate,
                                            Note = b.Note,
                                            NoteE = b.NoteE,
                                            HTPercentage = b.HTPercentage,
                                            RevenueAmount = b.RevenueAmount,
                                            VatPriceCur = b.VatPriceCur,
                                            VatPrice = b.VatPrice,
                                            DevaluationPercentage = b.DevaluationPercentage,
                                            DevaluationPriceCur = b.DevaluationPriceCur,
                                            DevaluationPrice = b.DevaluationPrice,
                                            DevaluationAmountCur = b.DevaluationAmountCur,
                                            DevaluationAmount = b.DevaluationAmount,
                                            VarianceAmount = b.VarianceAmount,
                                            RefId = b.RefId,
                                            ProductVoucherDetailId = b.ProductVoucherDetailId,
                                            VatTaxAmount = b.VatTaxAmount,
                                            DecreasePercentage = b.DecreasePercentage,
                                            DecreaseAmount = b.DecreaseAmount,
                                            AmountWithVat = b.AmountWithVat,
                                            AmountAfterDecrease = b.AmountAfterDecrease,
                                            VatAmount = b.VatAmount,
                                            AmountUpdate = b.Amount,
                                            PriceUpdate = b.Price
                                        }).ToList();
            var productVoucherdetailFix = (from a in lstProductVouchersDetail
                                           group new
                                           {
                                               a.OrgCode,
                                               a.ProductCode,
                                               a.ProductLotCode,
                                               a.ProductOriginCode,
                                               a.Id,
                                               a.AmountUpdate,
                                               a.Amount
                                           }
                                           by new
                                           {
                                               a.OrgCode,
                                               a.ProductCode,
                                               a.ProductLotCode,
                                               a.ProductOriginCode
                                           } into gr
                                           where gr.Max(p => p.AmountUpdate) - gr.Sum(p => p.Amount) != 0
                                           select new
                                           {
                                               Id = gr.Max(p => p.Id),
                                               AmountRefunds = gr.Max(p => p.AmountUpdate) - gr.Sum(p => p.Amount)
                                           }).ToList();
            lstProductVouchersDetail = (from b in lstProductVouchersDetail
                                        join c in productVoucherdetailFix on b.Id equals c.Id
                                        select new
                                        {
                                            Id = b.Id,
                                            OrgCode = b.OrgCode,
                                            ProductVoucherId = b.ProductVoucherId,
                                            Ord0 = b.Ord0,
                                            Year = b.Year,
                                            ProductCode = b.ProductCode,
                                            TransProductCode = b.TransProductCode,
                                            ProductName = b.ProductName,
                                            ProductName0 = b.ProductName0,
                                            UnitCode = b.UnitCode,
                                            TransUnitCode = b.TransUnitCode,
                                            WarehouseCode = b.WarehouseCode,
                                            TransWarehouseCode = b.TransWarehouseCode,
                                            TrxQuantity = b.TrxQuantity,
                                            Quantity = b.Quantity,
                                            PriceCur = b.PriceCur,
                                            Price = b.PriceUpdate,
                                            TrxAmountCur = b.TrxAmountCur,
                                            TrxAmount = b.TrxAmount,
                                            AmountCur = b.AmountCur,
                                            Amount = b.Amount + c.AmountRefunds,
                                            FixedPrice = b.FixedPrice,
                                            DebitAcc = b.DebitAcc,
                                            CreditAcc = b.CreditAcc,
                                            ProductLotCode = b.ProductLotCode,
                                            TransProductLotCode = b.TransProductLotCode,
                                            ProductOriginCode = b.ProductOriginCode,
                                            TransProductOriginCode = b.TransProductOriginCode,
                                            PartnerCode = b.PartnerCode,
                                            FProductWorkCode = b.FProductWorkCode,
                                            ContractCode = b.ContractCode,
                                            WorkPlaceCode = b.WorkPlaceCode,
                                            SectionCode = b.SectionCode,
                                            CaseCode = b.CaseCode,
                                            PriceCur2 = b.PriceCur2,
                                            Price2 = b.Price2,
                                            AmountCur2 = b.AmountCur2,
                                            Amount2 = b.Amount2,
                                            DebitAcc2 = b.DebitAcc2,
                                            CreditAcc2 = b.CreditAcc2,
                                            TaxCategoryCode = b.TaxCategoryCode,
                                            InsuranceDate = b.InsuranceDate,
                                            Note = b.Note,
                                            NoteE = b.NoteE,
                                            HTPercentage = b.HTPercentage,
                                            RevenueAmount = b.RevenueAmount,
                                            VatPriceCur = b.VatPriceCur,
                                            VatPrice = b.VatPrice,
                                            DevaluationPercentage = b.DevaluationPercentage,
                                            DevaluationPriceCur = b.DevaluationPriceCur,
                                            DevaluationPrice = b.DevaluationPrice,
                                            DevaluationAmountCur = b.DevaluationAmountCur,
                                            DevaluationAmount = b.DevaluationAmount,
                                            VarianceAmount = b.VarianceAmount,
                                            RefId = b.RefId,
                                            ProductVoucherDetailId = b.ProductVoucherDetailId,
                                            VatTaxAmount = b.VatTaxAmount,
                                            DecreasePercentage = b.DecreasePercentage,
                                            DecreaseAmount = b.DecreaseAmount,
                                            AmountWithVat = b.AmountWithVat,
                                            AmountAfterDecrease = b.AmountAfterDecrease,
                                            VatAmount = b.VatAmount,
                                            AmountUpdate = b.Amount,
                                            PriceUpdate = b.Price
                                        }).ToList();
            if (isHt2 == 0)
            {
                var lstProductVouchers = (from a in lstProductVouchersDetail
                                          group new
                                          {
                                              a.ProductVoucherId,
                                              a.AmountUpdate
                                          } by new
                                          {
                                              a.ProductVoucherId
                                          } into gr
                                          select new
                                          {
                                              Id = gr.Key.ProductVoucherId,
                                              AmountTt = gr.Sum(p => p.AmountUpdate)
                                          }).ToList();
                var resulProductVoucher = (from a in lstProductVoucher
                                           join b in lstProductVouchers on a.Id equals b.Id
                                           select new CrudProductVoucherDto
                                           {
                                               Id = a.Id,
                                               TotalProductAmount = b.AmountTt,
                                               TotalAmount = b.AmountTt,
                                               ProductVoucherDetails = null
                                           }).ToList();
                foreach (var item in resulProductVoucher)
                {
                    var lstProuct = await _productVoucher.GetByProductIdAsync(item.Id);
                    foreach (var items in lstProuct)
                    {
                        items.TotalProductAmount = item.TotalProductAmount;
                        items.TotalAmount = item.TotalAmount + items.TotalVatAmount ?? 0;
                        items.TotalAmountWithoutVat = item.TotalProductAmount;

                        await _productVoucher.UpdateAsync(items);
                    }

                }
                //for (int i = 0; i < resulProductVoucher.Count; i++)
                //{

                //var productVoucherDetails = await _productVoucherDetail.GetByProductIdAsync(resulProductVoucher[i].Id);
                //List<CrudProductVoucherDetailDto> crud = new List<CrudProductVoucherDetailDto>();
                //for (int k = 0; k < productVoucherDetails.Count; k++)
                //{
                //    var resul = ObjectMapper.Map<ProductVoucherDetail, CrudProductVoucherDetailDto>(productVoucherDetails[k]);
                //    crud.Add(resul);
                //}
                //resulProductVoucher[i].ProductVoucherDetails = crud;
                //await _productVoucherAppService.UpdateAsync(resulProductVoucher[i].Id, resulProductVoucher[i]);
                //  }
            }

            var resulLstProductVoucherDetail = (from a in lstProductVoucherDetail
                                                join b in lstProductVouchersDetail on new { a.ProductVoucherId, a.Ord0 } equals new { b.ProductVoucherId, b.Ord0 }
                                                select new CrudProductVoucherDetailDto
                                                {
                                                    Id = a.Id,
                                                    Ord0 = a.Ord0,
                                                    Price = b.PriceUpdate,
                                                    ProductVoucherId = a.ProductVoucherId,
                                                    Amount = b.AmountUpdate,


                                                }).ToList();

            foreach (var item in resulLstProductVoucherDetail)
            {
                var detail = await _productVoucherDetail.GetQueryableAsync();
                var lstDetail = detail.Where(p => p.Id == item.Id).ToList();
                foreach (var items in lstDetail)
                {
                    items.Price = item.Price;
                    items.Amount = item.Amount;
                    await _productVoucherDetail.UpdateAsync(items);
                }
                var ledger = await _ledgerService.GetQueryableAsync();
                var lstLedger = ledger.Where(p => p.VoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                foreach (var itemled in lstLedger)
                {
                    itemled.Amount = item.Amount;

                    await _ledgerService.UpdateAsync(itemled);
                }
                var wareHouse = await _warehouseBookService.GetQueryableAsync();
                var lstWareHouse = wareHouse.Where(p => p.ProductVoucherId == item.ProductVoucherId && p.Ord0 == item.Ord0).ToList();
                foreach (var itemWare in lstWareHouse)
                {
                    itemWare.Price0 = item.Price;
                    itemWare.Price = item.Price;
                    itemWare.Amount = item.Amount;
                    itemWare.ImportAmount = itemWare.VoucherGroup == 1 ? item.Amount : itemWare.ImportAmount;
                    itemWare.ExportAmount = itemWare.VoucherGroup == 2 ? item.Amount : item.ExportAmount;
                    await _warehouseBookService.UpdateAsync(itemWare);

                }
            }
            //var resulAll = (from a in lstProductVoucher
            //                join b in resulLstProductVoucherDetail on a.Id equals b.ProductVoucherId
            //                select new CrudProductVoucherDto
            //                {
            //                    Id = a.Id,
            //                    Year = a.Year,
            //                    DepartmentCode = a.DepartmentCode,
            //                    VoucherCode = a.VoucherCode,
            //                    VoucherGroup = a.VoucherGroup,
            //                    BusinessCode = a.BusinessCode,
            //                    BusinessAcc = a.BusinessAcc,
            //                    VoucherNumber = a.VoucherNumber,
            //                    InvoiceNumber = a.InvoiceNumber,
            //                    VoucherDate = a.VoucherDate,
            //                    PaymentTermsCode = a.PaymentTermsCode,
            //                    PartnerCode0 = a.PartnerCode0,
            //                    PartnerName0 = a.PartnerName0,
            //                    Representative = a.Representative,
            //                    Address = a.Address,
            //                    Tel = a.Tel,
            //                    Description = a.Description,
            //                    DescriptionE = a.DescriptionE,
            //                    Place = a.Place,
            //                    OriginVoucher = a.OriginVoucher,
            //                    CurrencyCode = a.CurrencyCode,
            //                    ExchangeRate = (decimal)a.ExchangeRate,
            //                    TotalAmountWithoutVatCur = a.TotalAmountWithoutVatCur,
            //                    TotalAmountWithoutVat = a.TotalAmountWithoutVat,
            //                    TotalDiscountAmountCur = a.TotalDiscountAmountCur,
            //                    TotalDiscountAmount = a.TotalDiscountAmount,
            //                    TotalVatAmountCur = a.TotalVatAmountCur,
            //                    TotalVatAmount = a.TotalVatAmount,
            //                    DebitOrCredit = a.DebitOrCredit,
            //                    TotalAmountCur = a.TotalAmountCur,
            //                    TotalProductAmountCur = a.TotalProductAmountCur,
            //                    TotalExciseTaxAmountCur = a.TotalExciseTaxAmountCur,
            //                    TotalExciseTaxAmount = a.TotalExciseTaxAmount,
            //                    TotalQuantity = a.TotalQuantity,
            //                    ExportNumber = a.ExportNumber,
            //                    TotalExpenseAmountCur0 = a.TotalExpenseAmountCur0,
            //                    TotalExpenseAmount0 = a.TotalExpenseAmount0,
            //                    TotalImportTaxAmountCur = a.TotalImportTaxAmountCur,
            //                    TotalImportTaxAmount = a.TotalImportTaxAmount,
            //                    TotalExpenseAmountCur = a.TotalExpenseAmountCur,
            //                    TotalExpenseAmount = a.TotalExpenseAmount,
            //                    EmployeeCode = a.EmployeeCode,
            //                    DeliveryDate = (DateTime)a.DeliveryDate,
            //                    Status = a.Status,
            //                    PaymentTermsId = a.PaymentTermsId,
            //                    SalesChannelCode = a.SalesChannelCode,
            //                    BillNumber = a.BillNumber,
            //                    DevaluationPercentage = a.DevaluationPercentage,
            //                    TotalDevaluationAmountCur = a.TotalDevaluationAmountCur,
            //                    TotalDevaluationAmount = a.TotalDevaluationAmount,
            //                    PriceDebitAcc = a.PriceDebitAcc,
            //                    PriceCreditAcc = a.PriceCreditAcc,
            //                    PriceDecreasingDescription = a.PriceDecreasingDescription,
            //                    IsCreatedEInvoice = a.IsCreatedEInvoice,
            //                    InfoFilter = a.InfoFilter,
            //                    RefType = a.RefType,
            //                    ImportDate = a.ImportDate,
            //                    ExportDate = a.ExportDate,
            //                    Vehicle = a.Vehicle,
            //                    OtherDepartment = a.OtherDepartment,
            //                    CommandNumber = a.CommandNumber,
            //                    TotalExpenseAmountCur1 = a.TotalExpenseAmountCur1,
            //                    TotalExpenseAmount1 = a.TotalExpenseAmount1,
            //                    TotalProductAmount = a.TotalProductAmount,
            //                    TotalAmount = a.TotalAmount,
            //                    ProductVoucherDetails = null
            //                }).ToList();
            //for (int i = 0; i < resulAll.Count; i++)
            //{
            //    var resuldetal = resulLstProductVoucherDetail.Where(p => p.ProductVoucherId == resulAll[i].Id).ToList();
            //    List<CrudProductVoucherDetailDto> crud = new List<CrudProductVoucherDetailDto>();
            //    for (int j = 0; j < resuldetal.Count; j++)
            //    {

            //        crud.AddRange(resuldetal);
            //    }


            //    resulAll[i].ProductVoucherDetails = crud;
            //    await _productVoucherAppService.UpdateAsync(resulAll[i].Id, resulAll[i]);
            //}
        }
    }
}

