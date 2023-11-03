using Accounting.Categories.Accounts;
using Accounting.Catgories.ProductVouchers;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.WarehouseBooks;
using Accounting.Windows;
using Microsoft.Extensions.Logging;
using NPOI.SS.Formula.Functions;
using Org.BouncyCastle.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Accounting.DomainServices.Categories
{
    public class WarehouseBookService : BaseDomainService<WarehouseBook, string>
    {
        #region Field
        private readonly ILogger<WarehouseBookService> _logger;
        private readonly ProductVoucherService _productVoucher;
        private readonly ProductVoucherDetailService _productVoucherDetail;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly ProductVoucherAssemblyService _productVoucherAssembly;
        private readonly ProductVoucherReceiptService _productVoucherReceipt;
        private readonly ProductVoucherVatService _productVoucherVat;
        private readonly UserService _userService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductService _product;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly WarehouseService _warehouseService;
        private readonly AccountSystemService _accountSystemService;
        private readonly ProductService _productService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;
        #endregion
        #region Ctor
        public WarehouseBookService(IRepository<WarehouseBook, string> repository,
                                ILogger<WarehouseBookService> logger,
                                ProductVoucherService productVoucherService,
                                ProductVoucherDetailService productVoucherDetailService,
                                AccTaxDetailService accTaxDetailService,
                                ProductVoucherAssemblyService productVoucherAssemblyService,
                                ProductVoucherReceiptService productVoucherReceiptService,
                                ProductVoucherVatService productVoucherVatService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                WebHelper webHelper,
                                BusinessCategoryService businessCategoryService,
                                VoucherCategoryService voucherCategoryService,
                                TenantSettingService tenantSettingService,
                                TaxCategoryService taxCategoryService,
                                ProductUnitService productUnitService,
                                ProductService product,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
                                VoucherTypeService voucherTypeService,
                                WarehouseService warehouseService,
                                AccountSystemService accountSystemService,
                                ProductService productService,
                                DefaultVoucherCategoryService defaultVoucherCategoryService,
                                DefaultVoucherTypeService defaultVoucherTypeService
                                )
        : base(repository)
        {
            _logger = logger;
            _productVoucher = productVoucherService;
            _productVoucherDetail = productVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _productVoucherAssembly = productVoucherAssemblyService;
            _productVoucherReceipt = productVoucherReceiptService;
            _productVoucherVat = productVoucherVatService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _businessCategoryService = businessCategoryService;
            _voucherCategoryService = voucherCategoryService;
            _tenantSettingService = tenantSettingService;
            _taxCategoryService = taxCategoryService;
            _productUnitService = productUnitService;
            _product = product;
            _productOpeningBalanceService = productOpeningBalanceService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
            _voucherTypeService = voucherTypeService;
            _warehouseService = warehouseService;
            _accountSystemService = accountSystemService;
            _productService = productService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _defaultVoucherTypeService = defaultVoucherTypeService;
        }
        #endregion
        public async Task<List<WarehouseBook>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task DeleteByVoucherId(string id)
        {
            var books = await this.GetRepository()
                                    .GetListAsync(p => p.ProductVoucherId == id);
            if (books.Count == 0) return;
            await this.GetRepository().DeleteManyAsync(books);
        }
        public async Task<List<WarehouseBook>> GetByWareHouseBookAsync(string orgCdoe, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCdoe && p.Year == year);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<WarehouseBook>> GetByListWarehouseBookAsync(string ordCode, DateTime voucherDate1, DateTime voucherDate2, string wareHouseCode, string productCode, string productLotCode, string productOriginCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == ordCode && p.VoucherDate >= voucherDate1 && p.VoucherDate <= voucherDate2);
            if (string.IsNullOrEmpty(wareHouseCode) == false)
            {
                queryable = queryable.Where(p => p.WarehouseCode == wareHouseCode);
            }
            if (string.IsNullOrEmpty(productCode) == false)
            {
                queryable = queryable.Where(p => p.ProductCode == productCode);
            }
            if (string.IsNullOrEmpty(productLotCode) == false)
            {
                queryable = queryable.Where(p => p.ProductLotCode == productLotCode);
            }
            if (string.IsNullOrEmpty(productOriginCode) == false)
            {
                queryable = queryable.Where(p => p.ProductOriginCode == productOriginCode);
            }

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<WarehouseBook>> GetByWarehouseBookAsync(string productId, int Year, DateTime VoucherDate, string OrdCode, DateTime? CreationTime)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId != productId && p.Year == Convert.ToInt32(Year) && p.VoucherDate <= VoucherDate || (p.VoucherDate == VoucherDate && p.CreationTime <= CreationTime) && Convert.ToInt32(p.Status) < 2);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<WarehouseBook>> GetByWarehouseBookIdAsync(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId.Contains(id));

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<ErrorDto> CheckSoundOutput(CrudProductVoucherDto entity)
        {
            ErrorDto errorDto = new ErrorDto();
            var VHT_XUAT_AM = await _tenantSettingService.GetValue("VHT_XUAT_AM", entity.OrgCode);
            var voucherDate = entity.VoucherDate;
            DateTime now = DateTime.Now;
            var voucherGroup = entity.VoucherGroup;
            int status = int.Parse(entity.Status);
            var productunit = await _productUnitService.GetQueryableAsync();
            var lstproductunit1 = productunit.Where(p => p.IsBasicUnit == true).ToList();
            var lstproductunit2 = productunit.ToList();
            var product = await _productService.GetQueryableAsync();
            var lstproduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            if (VHT_XUAT_AM == "C" && voucherGroup == 2 && status < 2)
            {
                List<CrudProductVoucherDetailDto> productVoucherdetails = entity.ProductVoucherDetails;
                productVoucherdetails = productVoucherdetails.OrderBy(p => p.Ord0).ToList();
                var productvoucherdetail = (from a in productVoucherdetails
                                            join b in lstproduct on a.ProductCode equals b.Code
                                            where b.ProductType != "D"
                                            group new
                                            {
                                                a.ProductCode,
                                                a.ProductLotCode,
                                                a.ProductOriginCode,
                                                a.WarehouseCode,
                                                a.FProductWorkCode,
                                                a.Ord0,

                                                a.Quantity
                                            } by new
                                            {
                                                a.ProductCode,
                                                a.ProductLotCode,
                                                a.ProductOriginCode,
                                                a.WarehouseCode,

                                            } into gr
                                            select new
                                            {
                                                ProductCode = gr.Key.ProductCode,
                                                ProductLotCode = gr.Key.ProductLotCode ?? "",
                                                ProductOriginCode = gr.Key.ProductOriginCode ?? "",
                                                WarehouseCode = gr.Key.WarehouseCode,
                                                Quantity = gr.Sum(p => p.Quantity),
                                                QuantityT = (decimal)0
                                            }).ToList();
                int i = 0;
                foreach (var item in productvoucherdetail)
                {
                    i += 1;
                    var productopeningbalance = await _productOpeningBalanceService.GetQueryableAsync();
                    var lstproductOpeningbalance = productopeningbalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
                    var lstproductOpeningbalances = (from a in lstproductOpeningbalance
                                                     where a.ProductCode == item.ProductCode
                                                           && a.WarehouseCode == item.WarehouseCode
                                                           && a.ProductLotCode == item.ProductLotCode
                                                           && a.ProductOriginCode == item.ProductOriginCode
                                                     group new
                                                     {
                                                         a.ProductCode,
                                                         a.ProductLotCode,
                                                         a.ProductOriginCode,
                                                         a.WarehouseCode,
                                                         a.Quantity
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
                                                         ProductLotCode = gr.Key.ProductLotCode,
                                                         ProductOriginCode = gr.Key.ProductOriginCode,
                                                         WarehouseCode = gr.Key.WarehouseCode,
                                                         Quantity = gr.Sum(p => p.Quantity),
                                                         QuantityT = (decimal)0
                                                     }).ToList();
                    var warehouseBook = await this.GetQueryableAsync();
                    var lstWarehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
                    var resul = (from a in lstWarehouseBook
                                 where a.ProductCode == item.ProductCode
                                                           && a.WarehouseCode == item.WarehouseCode
                                                           && a.ProductLotCode == item.ProductLotCode
                                                           && a.ProductOriginCode == item.ProductOriginCode
                                 where int.Parse(a.Status) < 2 && (a.VoucherDate < voucherDate || (a.VoucherDate == voucherDate && a.CreationTime < now))
                                 group new
                                 {
                                     a.ProductCode,
                                     a.ProductLotCode,
                                     a.ProductOriginCode,
                                     a.WarehouseCode,
                                     a.ExportQuantity,
                                     a.ImportQuantity
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
                                     ProductLotCode = gr.Key.ProductLotCode,
                                     ProductOriginCode = gr.Key.ProductOriginCode,
                                     WarehouseCode = gr.Key.WarehouseCode,
                                     Quantity = (decimal)(gr.Sum(p => p.ImportQuantity) - gr.Sum(p => p.ExportQuantity)),
                                     QuantityT = (decimal)0
                                 }).ToList();
                    lstproductOpeningbalances.AddRange(resul);

                    lstproductOpeningbalances = (from a in lstproductOpeningbalances
                                                 group new
                                                 {
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode,
                                                     a.WarehouseCode,
                                                     a.Quantity
                                                 } by new
                                                 {
                                                     a.ProductCode,
                                                     a.ProductLotCode,
                                                     a.ProductOriginCode,
                                                     a.WarehouseCode,
                                                 } into gr

                                                 select new
                                                 {
                                                     ProductCode = gr.Key.ProductCode,
                                                     ProductLotCode = gr.Key.ProductLotCode,
                                                     ProductOriginCode = gr.Key.ProductOriginCode,
                                                     WarehouseCode = gr.Key.WarehouseCode,
                                                     Quantity = gr.Sum(p => p.Quantity),
                                                     QuantityT = (decimal)0
                                                 }).ToList();

                    lstproductOpeningbalances = (from a in lstproductOpeningbalances

                                                 select new
                                                 {
                                                     ProductCode = a.ProductCode,
                                                     ProductLotCode = a.ProductLotCode,
                                                     ProductOriginCode = a.ProductOriginCode,
                                                     WarehouseCode = a.WarehouseCode,
                                                     //FProductWorkCode = a.FProductWorkCode,
                                                     Quantity = a.Quantity,
                                                     QuantityT = (decimal)item.Quantity
                                                 }).ToList();
                    if (lstproductOpeningbalances.Count > 0)
                    {
                        var warehouse = lstproductOpeningbalances.FirstOrDefault().WarehouseCode;
                        var quantity = lstproductOpeningbalances.FirstOrDefault().Quantity;
                        var quantityT = lstproductOpeningbalances.FirstOrDefault().QuantityT;
                        var products = lstproductOpeningbalances.FirstOrDefault().ProductCode;
                        if (quantity < quantityT)
                        {
                            string mess = ("Dòng lỗi: " + i + " " + "<br>" + " Mã kho :" + warehouse + "</br>" + " " + " Mã hàng :" + products + " không còn đủ trong kho. Vui lòng kiểm tra lại. ( Xuất " + quantityT + " Dư " + quantity + ")");

                            errorDto.Message = mess;
                        }
                    }



                }


            }
            return errorDto;
        }
        public async Task<List<CrudWarehouseBookDto>> CreatewarehouseBookAsync(CrudProductVoucherDto entity)
        {
            var product = await _product.GetQueryableAsync();
            string lstVoucherCode = "";
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            var voucherCode = voucherCategory.Where(p => p.Code == entity.VoucherCode && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList().FirstOrDefault();
            if (voucherCode == null)
            {
                var defaultVoucherCategory = await _defaultVoucherCategoryService.GetByCodeAsync(entity.VoucherCode);
                lstVoucherCode = defaultVoucherCategory.IsTransfer;
            }
            else
            {
                lstVoucherCode = voucherCode.IsTransfer;
            }
            List<CrudWarehouseBookDto> CrudWarehouseBookDtos = new List<CrudWarehouseBookDto>();
            List<CrudProductVoucherDetailDto> productVoucherDetails = (List<CrudProductVoucherDetailDto>)entity.ProductVoucherDetails;
            var wareHose = await _warehouseService.GetQueryableAsync();
            var wareHoses = wareHose.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productUnit = await _productUnitService.GetQueryableAsync();
            var productUnit1 = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.IsBasicUnit == true).ToList();
            var productUnit2 = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            productVoucherDetails = (from a in productVoucherDetails
                                         //join c in lstproductVocherDetailResicrp on a.Id equals c.ProductVoucherDetailId into d
                                         //from pdr in d.DefaultIfEmpty()
                                     join b in lstProduct on a.ProductCode equals b.Code into k
                                     from pr in k.DefaultIfEmpty()
                                     join o in wareHoses on a.WarehouseCode equals o.Code into p
                                     from wa in p.DefaultIfEmpty()
                                     join u in productUnit1 on a.ProductCode equals u.ProductCode into y
                                     from p1 in y.DefaultIfEmpty()
                                     join t in productUnit2 on new { a.ProductCode, a.UnitCode } equals new { t.ProductCode, t.UnitCode } into w
                                     from p2 in w.DefaultIfEmpty()
                                     select new CrudProductVoucherDetailDto
                                     {
                                         ProductVoucherId = a.ProductVoucherId,
                                         Ord0 = a.Ord0,
                                         Year = a.Year,
                                         ProductCode = a.ProductCode,
                                         TransProductCode = a.TransProductCode,
                                         ProductName = a.ProductName,
                                         ProductName0 = a.ProductName0,
                                         UnitCode = a.UnitCode,
                                         TransUnitCode = a.TransUnitCode,
                                         WarehouseCode = a.WarehouseCode,
                                         TransWarehouseCode = a.TransWarehouseCode,
                                         TrxQuantity = p2 != null ? a.Quantity * p1?.ExchangeRate ?? 1 / p2.ExchangeRate : 0,
                                         Quantity = p2 != null ? a.Quantity * p1?.ExchangeRate ?? 1 / p2.ExchangeRate : 0,
                                         PriceCur = a.PriceCur,
                                         Price = a.Price,
                                         TrxAmountCur = a.TrxAmountCur,
                                         TrxAmount = a.TrxAmount,
                                         TrxPrice = a.Price,
                                         TrxPriceCur = a.PriceCur,
                                         AmountCur = string.IsNullOrEmpty(entity.DiscountDebitAcc) == false && string.IsNullOrEmpty(entity.DiscountCreditAcc) == false ? a.AmountCur : a.AmountCur - a.DiscountAmountCur,
                                         Amount = string.IsNullOrEmpty(entity.DiscountDebitAcc) == false && string.IsNullOrEmpty(entity.DiscountCreditAcc) == false ? a.Amount : a.Amount - a.DiscountAmount,
                                         FixedPrice = a.FixedPrice,
                                         DebitAcc = a.DebitAcc,
                                         CreditAcc = a.CreditAcc,
                                         ProductLotCode = a.ProductLotCode,
                                         TransProductLotCode = a.TransProductLotCode,
                                         ProductOriginCode = a.ProductOriginCode,
                                         TransProductOriginCode = a.TransProductOriginCode,
                                         PartnerCode = a.PartnerCode,
                                         FProductWorkCode = a.FProductWorkCode,
                                         ContractCode = a.ContractCode,
                                         WorkPlaceCode = a.WorkPlaceCode,
                                         SectionCode = a.SectionCode,
                                         CaseCode = a.CaseCode,
                                         PriceCur2 = a.PriceCur2,
                                         Price2 = a.Price2,
                                         AmountCur2 = a.AmountCur2,
                                         Amount2 = a.Amount2,
                                         DebitAcc2 = a.DebitAcc2,
                                         CreditAcc2 = a.CreditAcc2,
                                         TaxCategoryCode = a.TaxCategoryCode,
                                         InsuranceDate = a.InsuranceDate,
                                         Note = a.Note,
                                         NoteE = a.NoteE,
                                         HTPercentage = a.HTPercentage,
                                         RevenueAmount = a.RevenueAmount,
                                         VatPriceCur = a.VatPriceCur,
                                         VatPrice = a.VatPrice,
                                         DevaluationPercentage = a.DevaluationPercentage,
                                         DevaluationPriceCur = a.DevaluationPriceCur ?? 0,
                                         DevaluationPrice = a.DevaluationPrice ?? 0,
                                         DevaluationAmountCur = a.DevaluationAmountCur ?? 0,
                                         DevaluationAmount = a.DevaluationAmount ?? 0,
                                         VarianceAmount = a.VarianceAmount ?? 0,
                                         RefId = a.RefId,
                                         ProductVoucherDetailId = a.ProductVoucherDetailId,
                                         VatTaxAmount = a.VatTaxAmount,
                                         DecreasePercentage = a.DecreasePercentage,
                                         DecreaseAmount = a.DecreaseAmount ?? 0,
                                         AmountWithVat = a.AmountWithVat ?? 0,
                                         AmountAfterDecrease = a.AmountAfterDecrease ?? 0,
                                         VatAmount = a.VatAmount ?? 0,
                                         VatPercentage = a.VatPercentage,
                                         VatAmountCur = a.VatAmountCur ?? 0,
                                         DiscountPercentage = a.DiscountPercentage,
                                         DiscountAmount = a.DiscountAmount ?? 0,
                                         DiscountAmountCur = a.DiscountAmountCur ?? 0,
                                         ImportTaxPercentage = a.ImportTaxPercentage,
                                         ImportTaxAmountCur = a.ImportTaxAmountCur ?? 0,
                                         ImportTaxAmount = a.ImportTaxAmount ?? 0,
                                         ExciseTaxPercentage = a.ExciseTaxPercentage,
                                         ExciseTaxAmountCur = a.ExciseTaxAmountCur ?? 0,
                                         ExciseTaxAmount = a.ExciseTaxAmount ?? 0,
                                         ExpenseAmountCur0 = a.ExpenseAmountCur0 ?? 0,
                                         ExpenseAmount0 = a.ExpenseAmount0 ?? 0,
                                         ExpenseAmountCur1 = a.ExpenseAmountCur1 ?? 0,
                                         ExpenseAmount1 = a.ExpenseAmount1 ?? 0,
                                         ExpenseAmountCur = a.ExpenseAmountCur ?? 0,
                                         ExpenseAmount = a.ExpenseAmount ?? 0,
                                         ExpenseImportTaxAmountCur = a.ExpenseImportTaxAmountCur ?? 0,
                                         ExpenseImportTaxAmount = a.ExpenseImportTaxAmount ?? 0,
                                         ExciseAmountCur = a.ExciseAmountCur ?? 0,
                                         ExciseAmount = a.ExciseAmount ?? 0,
                                         AttachProductLot = a.AttachProductLot,
                                         AttachProductOrigin = a.AttachProductOrigin,
                                         ProductAcc = pr != null ? pr.ProductAcc : null,
                                         ProductType = pr != null ? (pr.ProductType == null ? "D" : pr.ProductType) : null,
                                         WareHouseAcc = wa != null ? wa.WarehouseAcc : null,
                                         TrxPrice2 = a.Price2,
                                         TrxPriceCur2 = a.PriceCur2,
                                         ImportAcc = entity.BusinessAcc,
                                         ExportAcc = entity.BusinessAcc,
                                         TrxImportQuantity = p2 != null ? a.Quantity * p1.ExchangeRate / p2.ExchangeRate : 0,
                                         TrxExportQuantity = p2 != null ? a.Quantity * p1.ExchangeRate / p2.ExchangeRate : 0,
                                         ImportQuantity = p2 != null ? a.Quantity * p1.ExchangeRate / p2.ExchangeRate : 0,
                                         ImportAmountCur = entity.VoucherGroup == 1 ? a.AmountCur : 0,
                                         ImportAmount = entity.VoucherGroup == 1 ? a.Amount : 0,
                                         ExportQuantity = p2 != null ? a.Quantity * p1.ExchangeRate / p2.ExchangeRate : 0,
                                         ExportAmountCur = entity.VoucherGroup == 2 ? a.AmountCur : 0,
                                         ExportAmount = entity.VoucherGroup == 2 ? a.Amount : 0,
                                         QuantityTrx = p2 != null ? a.Quantity * p1.ExchangeRate / p2.ExchangeRate : 0,
                                         Price0 = a.Price ?? 0,
                                         PriceCur0 = a.PriceCur0


                                     }).ToList();

            List<CrudAccTaxDetailDto> accTaxDetails = (List<CrudAccTaxDetailDto>)entity.AccTaxDetails;
            _logger.LogInformation("lstVoucherCode == C: zzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            if (lstVoucherCode == "C")
            {
                for (int i = 0; i < productVoucherDetails.Count; i++)
                {
                    List<CrudProductVoucherDetailReceiptDto> productVoucherDetailReceipts = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[i].ProductVoucherDetailReceipts;

                    CrudWarehouseBookDto CrudWarehouseBookDto = new CrudWarehouseBookDto();
                    CrudWarehouseBookDto.OrgCode = entity.OrgCode;
                    CrudWarehouseBookDto.ProductVoucherId = entity.Id;
                    CrudWarehouseBookDto.VoucherCode = entity.VoucherCode;
                    CrudWarehouseBookDto.VoucherGroup = 1;
                    CrudWarehouseBookDto.BusinessAcc = entity.DebitAcc;
                    CrudWarehouseBookDto.Ord0 = "B" + (i + 1).ToString().PadLeft(9, '0');
                    CrudWarehouseBookDto.VoucherNumber = entity.VoucherNumber;
                    CrudWarehouseBookDto.DepartmentCode = entity.DepartmentCode;
                    CrudWarehouseBookDto.BusinessCode = entity.BusinessCode;
                    CrudWarehouseBookDto.Ord0Extra = "B" + (i + 1).ToString().PadLeft(9, '0');
                    CrudWarehouseBookDto.VoucherDate = entity.VoucherDate;
                    CrudWarehouseBookDto.CurrencyCode = entity.CurrencyCode;
                    CrudWarehouseBookDto.ExchangeRate = entity.ExchangeRate;
                    CrudWarehouseBookDto.PartnerCode0 = entity.PartnerCode0;
                    CrudWarehouseBookDto.Representative = entity.Representative;
                    CrudWarehouseBookDto.Address = entity.Address;
                    CrudWarehouseBookDto.OriginVoucher = entity.OriginVoucher;
                    CrudWarehouseBookDto.Description = entity.Description;
                    CrudWarehouseBookDto.PaymentTermsCode = entity.PaymentTermsCode;
                    CrudWarehouseBookDto.ContractCode = productVoucherDetails[i].ContractCode;
                    CrudWarehouseBookDto.TransProductCode = entity.VoucherCode == "PDC" ? productVoucherDetails[i].TransProductCode : productVoucherDetails[i].ProductCode;
                    CrudWarehouseBookDto.ProductCode = entity.VoucherCode == "PDC" ? productVoucherDetails[i].ProductCode : productVoucherDetails[i].TransProductCode;
                    CrudWarehouseBookDto.TransWarehouseCode = entity.VoucherCode == "PDC" ? productVoucherDetails[i].WarehouseCode : productVoucherDetails[i].TransWarehouseCode;
                    CrudWarehouseBookDto.WarehouseCode = entity.VoucherCode == "PDC" ? productVoucherDetails[i].TransWarehouseCode : productVoucherDetails[i].WarehouseCode;
                    CrudWarehouseBookDto.ProductLotCode = productVoucherDetails[i].ProductLotCode;
                    CrudWarehouseBookDto.TransProductLotCode = productVoucherDetails[i].TransProductLotCode;
                    CrudWarehouseBookDto.ProductOriginCode = productVoucherDetails[i].ProductOriginCode;
                    CrudWarehouseBookDto.TransProductOriginCode = productVoucherDetails[i].TransProductOriginCode;
                    CrudWarehouseBookDto.UnitCode = productVoucherDetails[i].UnitCode;
                    CrudWarehouseBookDto.TransferingUnitCode = productVoucherDetails[i].TransUnitCode;
                    CrudWarehouseBookDto.PartnerCode = productVoucherDetails[i].PartnerCode;
                    CrudWarehouseBookDto.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                    CrudWarehouseBookDto.ProductName0 = productVoucherDetails[i].ProductName0;
                    CrudWarehouseBookDto.Quantity = productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.Price = productVoucherDetails[i].Price;
                    CrudWarehouseBookDto.PriceCur = productVoucherDetails[i].PriceCur;
                    CrudWarehouseBookDto.AmountCur = productVoucherDetails[i].AmountCur;
                    CrudWarehouseBookDto.Amount = productVoucherDetails[i].Amount;
                    CrudWarehouseBookDto.ExpenseAmount0 = productVoucherDetails[i].ExpenseAmount0;
                    CrudWarehouseBookDto.ExpenseAmountCur0 = productVoucherDetails[i].ExpenseAmountCur0;
                    CrudWarehouseBookDto.ExpenseAmount1 = productVoucherDetails[i].ExpenseAmountCur1;
                    CrudWarehouseBookDto.ExpenseAmount = productVoucherDetails[i].ExpenseAmount;
                    CrudWarehouseBookDto.ExprenseAmountCur = productVoucherDetails[i].ExpenseAmountCur;
                    if (accTaxDetails != null)
                    {
                        if (accTaxDetails.Count > 0)
                        {
                            CrudWarehouseBookDto.VatPercentage = accTaxDetails[0].VatPercentage;
                            CrudWarehouseBookDto.TaxCode = accTaxDetails[0].TaxCode;
                            CrudWarehouseBookDto.InvoiceSymbol = accTaxDetails[0].InvoiceSymbol;
                            CrudWarehouseBookDto.InvoiceNumber = accTaxDetails[0].InvoiceNumber;
                            CrudWarehouseBookDto.InvoiceDate = accTaxDetails[0].InvoiceDate;

                        }


                    }

                    CrudWarehouseBookDto.VatAmount = productVoucherDetails[i].VatAmount;
                    CrudWarehouseBookDto.ExpenseAmountCur0 = productVoucherDetails[i].ExpenseAmountCur0;
                    CrudWarehouseBookDto.ExpenseAmount0 = productVoucherDetails[i].ExpenseAmount0;
                    CrudWarehouseBookDto.ExpenseAmountCur1 = productVoucherDetails[i].ExpenseAmountCur1;
                    CrudWarehouseBookDto.ExpenseAmount1 = productVoucherDetails[i].ExpenseAmount1;
                    CrudWarehouseBookDto.ExportAmountCur = productVoucherDetails[i].ExportAmountCur;
                    CrudWarehouseBookDto.ExpenseAmount = productVoucherDetails[i].ExpenseAmount;
                    CrudWarehouseBookDto.DiscountPercentage = productVoucherDetails[i].DiscountPercentage;
                    CrudWarehouseBookDto.DiscountAmountCur = productVoucherDetails[i].DiscountAmountCur;
                    CrudWarehouseBookDto.DiscountAmount = productVoucherDetails[i].DiscountAmount;
                    CrudWarehouseBookDto.ImportTaxPercentage = productVoucherDetails[i].ImportTaxPercentage;
                    CrudWarehouseBookDto.ImportTaxAmountCur = productVoucherDetails[i].ImportTaxAmountCur;
                    CrudWarehouseBookDto.ImportTaxAmount = productVoucherDetails[i].ImportTaxAmount;
                    CrudWarehouseBookDto.ImportAcc = productVoucherDetails[i].DebitAcc;
                    CrudWarehouseBookDto.ImportQuantity = productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.TrxImportQuantity = productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.TrxExportQuantity = 0;
                    CrudWarehouseBookDto.ExportQuantity = 0;
                    CrudWarehouseBookDto.ExportAmount = 0;
                    CrudWarehouseBookDto.ExciseTaxPercentage = productVoucherDetails[i].ExciseTaxPercentage;
                    CrudWarehouseBookDto.ExportAcc = productVoucherDetails[i].ExportAcc;
                    CrudWarehouseBookDto.ExciseTaxAmountCur = productVoucherDetails[i].ExciseTaxAmountCur;
                    CrudWarehouseBookDto.ExciseTaxAmount = productVoucherDetails[i].ExciseTaxAmount;
                    CrudWarehouseBookDto.DebitAcc = productVoucherDetails[i].DebitAcc;
                    CrudWarehouseBookDto.CreditAcc = productVoucherDetails[i].CreditAcc;
                    CrudWarehouseBookDto.PriceCur2 = productVoucherDetails[i].PriceCur2;
                    CrudWarehouseBookDto.Price = productVoucherDetails[i].Price;
                    CrudWarehouseBookDto.PriceCur = productVoucherDetails[i].PriceCur;
                    CrudWarehouseBookDto.Price2 = productVoucherDetails[i].Price2;
                    CrudWarehouseBookDto.AmountCur2 = productVoucherDetails[i].AmountCur2 - productVoucherDetails[i].DecreaseAmount ?? 0;
                    CrudWarehouseBookDto.Amount2 = productVoucherDetails[i].Amount2 - productVoucherDetails[i].DecreaseAmount ?? 0;
                    CrudWarehouseBookDto.DebitAcc2 = productVoucherDetails[i].DebitAcc2;
                    CrudWarehouseBookDto.CreditAcc2 = productVoucherDetails[i].CreditAcc2;
                    CrudWarehouseBookDto.Note = productVoucherDetails[i].Note;
                    CrudWarehouseBookDto.NoteE = productVoucherDetails[i].NoteE;
                    CrudWarehouseBookDto.SectionCode = productVoucherDetails[i].SectionCode;
                    CrudWarehouseBookDto.CaseCode = productVoucherDetails[i].CaseCode;
                    CrudWarehouseBookDto.WorkPlaceCode = productVoucherDetails[i].WorkPlaceCode;
                    CrudWarehouseBookDto.FixedPrice = productVoucherDetails[i].FixedPrice;
                    CrudWarehouseBookDto.VatPrice = productVoucherDetails[i].VatPrice;
                    CrudWarehouseBookDto.VatPriceCur = productVoucherDetails[i].VatPriceCur;
                    CrudWarehouseBookDto.DevaluationPriceCur = productVoucherDetails[i].DevaluationPriceCur;
                    CrudWarehouseBookDto.DevaluationPrice = productVoucherDetails[i].DevaluationPrice;
                    CrudWarehouseBookDto.DevaluationPriceCur = productVoucherDetails[i].DevaluationPriceCur;
                    CrudWarehouseBookDto.DevaluationAmountCur = productVoucherDetails[i].DevaluationAmountCur;
                    CrudWarehouseBookDto.DevaluationAmount = productVoucherDetails[i].DevaluationAmount;
                    CrudWarehouseBookDto.VarianceAmount = productVoucherDetails[i].VarianceAmount;
                    CrudWarehouseBookDto.TotalAmount2 = productVoucherDetails[i].Amount2;
                    CrudWarehouseBookDto.TotalDiscountAmount = productVoucherDetails[i].DiscountAmount;
                    CrudWarehouseBookDto.TrxQuantity = productVoucherDetails[i].TrxQuantity;
                    CrudWarehouseBookDto.TrxPriceCur = productVoucherDetails[i].PriceCur;
                    CrudWarehouseBookDto.TrxPrice = productVoucherDetails[i].Price;
                    CrudWarehouseBookDto.TrxPrice2 = productVoucherDetails[i].Price2;
                    CrudWarehouseBookDto.TrxPriceCur2 = productVoucherDetails[i].PriceCur2;
                    CrudWarehouseBookDto.Year = entity.Year;
                    CrudWarehouseBookDto.Status = entity.Status;
                    CrudWarehouseBookDto.QuantityCur = 0;
                    CrudWarehouseBookDto.Price0 = 0;
                    CrudWarehouseBookDto.PriceCur0 = 0;
                    CrudWarehouseBookDto.ImportAmount = productVoucherDetails[i].Amount;
                    CrudWarehouseBookDto.ImportAmountCur = productVoucherDetails[i].ImportAmountCur;
                    if (entity.CreationTime != null)
                    {
                        CrudWarehouseBookDto.CreationTime = entity.CreationTime;
                    }
                    else
                    {
                        CrudWarehouseBookDto.CreationTime = DateTime.Now;
                    }


                    CrudWarehouseBookDtos.Add(CrudWarehouseBookDto);

                }
            }
            _logger.LogInformation("entity.VoucherGroup == 3: zzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            if (entity.VoucherGroup == 3)
            {
                for (int i = 0; i < productVoucherDetails.Count; i++)
                {
                    List<CrudProductVoucherDetailReceiptDto> productVoucherDetailReceipts = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[i].ProductVoucherDetailReceipts;

                    CrudWarehouseBookDto CrudWarehouseBookDto = new CrudWarehouseBookDto();
                    CrudWarehouseBookDto.OrgCode = entity.OrgCode;
                    CrudWarehouseBookDto.ProductVoucherId = entity.Id;
                    CrudWarehouseBookDto.VoucherCode = entity.VoucherCode;
                    CrudWarehouseBookDto.VoucherGroup = 2;
                    CrudWarehouseBookDto.BusinessAcc = entity.DebitAcc;
                    CrudWarehouseBookDto.VoucherNumber = entity.VoucherNumber;
                    CrudWarehouseBookDto.DepartmentCode = entity.DepartmentCode;
                    CrudWarehouseBookDto.BusinessCode = entity.BusinessCode;
                    CrudWarehouseBookDto.Ord0 = "B" + (i + 1).ToString().PadLeft(9, '0');
                    CrudWarehouseBookDto.Ord0Extra = "B" + (i + 1).ToString().PadLeft(9, '0');
                    CrudWarehouseBookDto.VoucherDate = entity.VoucherDate;
                    CrudWarehouseBookDto.CurrencyCode = entity.CurrencyCode;
                    CrudWarehouseBookDto.ExchangeRate = entity.ExchangeRate;
                    CrudWarehouseBookDto.PartnerCode0 = entity.PartnerCode0;
                    CrudWarehouseBookDto.Representative = entity.Representative;
                    CrudWarehouseBookDto.Address = entity.Address;
                    CrudWarehouseBookDto.OriginVoucher = entity.OriginVoucher;
                    CrudWarehouseBookDto.Description = entity.Description;
                    CrudWarehouseBookDto.PaymentTermsCode = entity.PaymentTermsCode;
                    CrudWarehouseBookDto.ContractCode = productVoucherDetails[i].ContractCode;
                    CrudWarehouseBookDto.TransProductCode = null;
                    CrudWarehouseBookDto.ProductCode = productVoucherDetails[i].ProductCode;
                    CrudWarehouseBookDto.TransWarehouseCode = null;
                    CrudWarehouseBookDto.WarehouseCode = productVoucherDetails[i].WarehouseCode;
                    CrudWarehouseBookDto.ProductLotCode = productVoucherDetails[i].ProductLotCode;
                    CrudWarehouseBookDto.TransProductLotCode = null;
                    CrudWarehouseBookDto.ProductOriginCode = productVoucherDetails[i].ProductOriginCode;
                    CrudWarehouseBookDto.TransProductOriginCode = null;
                    CrudWarehouseBookDto.UnitCode = productVoucherDetails[i].UnitCode;
                    CrudWarehouseBookDto.TransferingUnitCode = null;
                    CrudWarehouseBookDto.PartnerCode = productVoucherDetails[i].PartnerCode;
                    CrudWarehouseBookDto.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                    CrudWarehouseBookDto.ProductName0 = productVoucherDetails[i].ProductName0;
                    CrudWarehouseBookDto.Quantity = productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.Price = productVoucherDetails[i].Price;
                    CrudWarehouseBookDto.PriceCur = productVoucherDetails[i].PriceCur;
                    CrudWarehouseBookDto.AmountCur = productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExpenseAmountCur1 + productVoucherDetails[i].ExpenseAmountCur0;
                    CrudWarehouseBookDto.Amount = productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount1 + productVoucherDetails[i].ExpenseAmount0;
                    CrudWarehouseBookDto.ExpenseAmount0 = productVoucherDetails[i].ExpenseAmount0;
                    CrudWarehouseBookDto.ExpenseAmountCur0 = productVoucherDetails[i].ExpenseAmountCur0;
                    CrudWarehouseBookDto.ExpenseAmount1 = productVoucherDetails[i].ExpenseAmountCur1;
                    CrudWarehouseBookDto.ExpenseAmount = productVoucherDetails[i].ExpenseAmount;
                    CrudWarehouseBookDto.ExprenseAmountCur = productVoucherDetails[i].ExpenseAmountCur;
                    CrudWarehouseBookDto.VatAmount = productVoucherDetails[i].VatAmount;
                    CrudWarehouseBookDto.VatAmountCur = productVoucherDetails[i].VatAmountCur;
                    CrudWarehouseBookDto.QuantityCur = productVoucherDetails[i].Quantity;
                    if (accTaxDetails != null)
                    {
                        if (accTaxDetails.Count > 0)
                        {
                            CrudWarehouseBookDto.VatPercentage = accTaxDetails[0].VatPercentage;
                            CrudWarehouseBookDto.TaxCode = accTaxDetails[0].TaxCode;
                            CrudWarehouseBookDto.InvoiceSymbol = accTaxDetails[0].InvoiceSymbol;
                            CrudWarehouseBookDto.InvoiceNumber = accTaxDetails[0].InvoiceNumber;
                            CrudWarehouseBookDto.InvoiceDate = accTaxDetails[0].InvoiceDate;

                        }
                    }
                    CrudWarehouseBookDto.Price = (productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount0 + productVoucherDetails[i].ExpenseAmount1) / productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.PriceCur = (productVoucherDetails[i].PriceCur + productVoucherDetails[i].ExpenseAmountCur0 + productVoucherDetails[i].ExpenseAmountCur1) / productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.VatAmount = productVoucherDetails[i].VatAmount;
                    CrudWarehouseBookDto.ExpenseAmountCur0 = 0;
                    CrudWarehouseBookDto.ExpenseAmount0 = 0;
                    CrudWarehouseBookDto.ExpenseAmountCur1 = 0;
                    CrudWarehouseBookDto.ExpenseAmount1 = 0;
                    CrudWarehouseBookDto.ExportAmountCur = 0;
                    CrudWarehouseBookDto.ExpenseAmount = 0;
                    CrudWarehouseBookDto.DiscountPercentage = 0;
                    CrudWarehouseBookDto.DiscountAmountCur = 0;
                    CrudWarehouseBookDto.DiscountAmount = 0;
                    CrudWarehouseBookDto.ImportTaxPercentage = 0;
                    CrudWarehouseBookDto.ImportTaxAmountCur = 0;
                    CrudWarehouseBookDto.ImportTaxAmount = 0;
                    CrudWarehouseBookDto.ExciseTaxPercentage = 0;
                    CrudWarehouseBookDto.ExciseTaxAmountCur = 0;
                    CrudWarehouseBookDto.ExciseTaxAmount = 0;
                    CrudWarehouseBookDto.ExportAcc = productVoucherDetails[i].ProductAcc;
                    CrudWarehouseBookDto.ExportAmount = productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount0 + productVoucherDetails[i].ExpenseAmount1;
                    CrudWarehouseBookDto.ExportAmountCur = productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExpenseAmountCur0 + productVoucherDetails[i].ExpenseAmountCur1;
                    CrudWarehouseBookDto.TrxExportQuantity = productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.ExportQuantity = productVoucherDetails[i].Quantity;
                    CrudWarehouseBookDto.DebitAcc = productVoucherDetails[i].DebitAcc;
                    CrudWarehouseBookDto.CreditAcc = productVoucherDetails[i].ProductAcc;
                    CrudWarehouseBookDto.PriceCur2 = productVoucherDetails[i].PriceCur2;
                    //CrudWarehouseBookDto.Price = productVoucherDetails[i].Price;
                    //CrudWarehouseBookDto.PriceCur = productVoucherDetails[i].PriceCur;
                    CrudWarehouseBookDto.Price2 = productVoucherDetails[i].Price2;
                    CrudWarehouseBookDto.AmountCur2 = productVoucherDetails[i].AmountCur2 - productVoucherDetails[i].DecreaseAmount ?? 0;
                    CrudWarehouseBookDto.Amount2 = productVoucherDetails[i].Amount2 - productVoucherDetails[i].DecreaseAmount ?? 0;
                    CrudWarehouseBookDto.DebitAcc2 = productVoucherDetails[i].DebitAcc2;
                    CrudWarehouseBookDto.CreditAcc2 = productVoucherDetails[i].CreditAcc2;
                    CrudWarehouseBookDto.Note = productVoucherDetails[i].Note;
                    CrudWarehouseBookDto.NoteE = productVoucherDetails[i].NoteE;
                    CrudWarehouseBookDto.SectionCode = productVoucherDetails[i].SectionCode;
                    CrudWarehouseBookDto.CaseCode = productVoucherDetails[i].CaseCode;
                    CrudWarehouseBookDto.WorkPlaceCode = productVoucherDetails[i].WorkPlaceCode;
                    CrudWarehouseBookDto.FixedPrice = productVoucherDetails[i].FixedPrice;
                    CrudWarehouseBookDto.VatPrice = productVoucherDetails[i].VatPrice;
                    CrudWarehouseBookDto.VatPriceCur = productVoucherDetails[i].VatPriceCur;
                    CrudWarehouseBookDto.DevaluationPriceCur = productVoucherDetails[i].DevaluationPriceCur;
                    CrudWarehouseBookDto.DevaluationPrice = productVoucherDetails[i].DevaluationPrice;
                    CrudWarehouseBookDto.DevaluationPriceCur = productVoucherDetails[i].DevaluationPriceCur;
                    CrudWarehouseBookDto.DevaluationAmountCur = productVoucherDetails[i].DevaluationAmountCur;
                    CrudWarehouseBookDto.DevaluationAmount = productVoucherDetails[i].DevaluationAmount;
                    CrudWarehouseBookDto.VarianceAmount = productVoucherDetails[i].VarianceAmount;
                    CrudWarehouseBookDto.TotalAmount2 = productVoucherDetails[i].Amount2;
                    CrudWarehouseBookDto.TotalDiscountAmount = productVoucherDetails[i].DiscountAmount;
                    CrudWarehouseBookDto.TrxQuantity = productVoucherDetails[i].TrxQuantity;
                    CrudWarehouseBookDto.TrxPriceCur = productVoucherDetails[i].PriceCur;
                    CrudWarehouseBookDto.TrxPrice = productVoucherDetails[i].Price;
                    CrudWarehouseBookDto.TrxPrice2 = productVoucherDetails[i].Price2;
                    CrudWarehouseBookDto.TrxPriceCur2 = productVoucherDetails[i].PriceCur2;
                    CrudWarehouseBookDto.Year = entity.Year;
                    CrudWarehouseBookDto.Status = entity.Status;
                    CrudWarehouseBookDto.ImportAmount = 0;
                    CrudWarehouseBookDto.ImportAmountCur = 0;
                    CrudWarehouseBookDto.ImportQuantity = 0;
                    if (entity.CreationTime != null)
                    {
                        CrudWarehouseBookDto.CreationTime = entity.CreationTime;
                    }
                    else
                    {
                        CrudWarehouseBookDto.CreationTime = DateTime.Now;
                    }
                    CrudWarehouseBookDtos.Add(CrudWarehouseBookDto);

                }
            }
            entity.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
            _logger.LogInformation("for: zzzzzzzzzzzzzzzzzzzzzzzzzzzzz");
            for (int i = 0; i < productVoucherDetails.Count; i++)
            {
                var lstProducts = await _product.GetQueryableAsync();
                var product1 = lstProducts.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == productVoucherDetails[i].ProductCode).FirstOrDefault();
                var defaultVoucherTypelst = await _defaultVoucherTypeService.GetQueryableAsync();
                var defaultVoucherType = defaultVoucherTypelst.Where(p => p.Code == "PCP").FirstOrDefault();
                var listVoucher = "";
                var LstMA_CT = await _voucherTypeService.GetByVoucherTypeAsync("PCP");
                if (LstMA_CT.Count > 0)
                {
                    listVoucher = LstMA_CT[0].ListVoucher;
                }
                else
                {
                    listVoucher = defaultVoucherType.ListVoucher;
                }


                if (product1 != null)
                {
                    productVoucherDetails[i].ProductType = product1.ProductType;
                }

                if (listVoucher.Contains(entity.VoucherCode) == true)
                {
                    productVoucherDetails[i].TrxQuantity = 0;
                    productVoucherDetails[i].Quantity = 0;
                    productVoucherDetails[i].Price = 0;
                    productVoucherDetails[i].PriceCur = 0;
                    productVoucherDetails[i].AmountCur = 0;
                    productVoucherDetails[i].Amount = 0;
                    productVoucherDetails[i].DiscountPercentage = 0;
                    productVoucherDetails[i].DiscountAmount = 0;
                    productVoucherDetails[i].DiscountAmountCur = 0;
                    productVoucherDetails[i].ImportTaxPercentage = 0;
                    productVoucherDetails[i].ImportTaxAmount = 0;
                    productVoucherDetails[i].ImportTaxAmountCur = 0;
                    productVoucherDetails[i].ImportQuantity = 0;
                    productVoucherDetails[i].ExciseTaxPercentage = 0;
                    productVoucherDetails[i].ExciseTaxAmountCur = 0;
                    productVoucherDetails[i].ExciseTaxAmount = 0;
                    productVoucherDetails[i].Price2 = 0;
                    productVoucherDetails[i].PriceCur2 = 0;
                    productVoucherDetails[i].Amount2 = 0;
                    productVoucherDetails[i].AmountCur2 = 0;
                    productVoucherDetails[i].DebitAcc2 = "";
                    productVoucherDetails[i].CreditAcc2 = "";
                    productVoucherDetails[i].ImportAcc = "";
                    productVoucherDetails[i].ExportAcc = "";

                }
                if (productVoucherDetails[i].ProductType == "D")
                {
                    productVoucherDetails[i].Price = 0;
                    productVoucherDetails[i].PriceCur = 0;
                    productVoucherDetails[i].ImportAcc = "";
                    productVoucherDetails[i].ExportAcc = "";
                    productVoucherDetails[i].TrxImportQuantity = 0;
                    productVoucherDetails[i].ImportQuantity = 0;
                    productVoucherDetails[i].ImportAmount = 0;
                    productVoucherDetails[i].ImportAmountCur = 0;
                    productVoucherDetails[i].ExportAmount = 0;
                    productVoucherDetails[i].ExportAmountCur = 0;
                    productVoucherDetails[i].ExportQuantity = 0;
                    productVoucherDetails[i].TrxExportQuantity = 0;

                }
                if (productVoucherDetails[i].ProductType != "D")
                {
                    productVoucherDetails[i].Quantity = productVoucherDetails[i].QuantityTrx;
                    productVoucherDetails[i].ImportAcc = entity.VoucherGroup == 1 ? productVoucherDetails[i].DebitAcc : null;
                    productVoucherDetails[i].TrxImportQuantity = entity.VoucherGroup == 1 ? productVoucherDetails[i].TrxImportQuantity : 0;
                    productVoucherDetails[i].ImportQuantity = entity.VoucherGroup == 1 ? productVoucherDetails[i].QuantityTrx : 0;
                    productVoucherDetails[i].ImportAmountCur = entity.VoucherGroup == 1 ? (productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExpenseAmountCur1 + productVoucherDetails[i].ExpenseAmountCur0 + productVoucherDetails[i].ImportTaxAmountCur + productVoucherDetails[i].ExciseTaxAmountCur) : 0;
                    productVoucherDetails[i].ImportAmount = entity.VoucherGroup == 1 ? (productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount1 + productVoucherDetails[i].ExpenseAmount0 + productVoucherDetails[i].ImportTaxAmount + productVoucherDetails[i].ExciseTaxAmount) : 0;
                    productVoucherDetails[i].ExportAcc = entity.VoucherGroup == 2 ? productVoucherDetails[i].CreditAcc : null;
                    productVoucherDetails[i].TrxExportQuantity = entity.VoucherGroup == 2 ? productVoucherDetails[i].TrxExportQuantity : 0;
                    productVoucherDetails[i].ExportQuantity = entity.VoucherGroup == 2 ? productVoucherDetails[i].QuantityTrx : 0;
                    productVoucherDetails[i].ExportAmount = entity.VoucherGroup == 2 ? productVoucherDetails[i].Amount + productVoucherDetails[i].ExciseAmount + productVoucherDetails[i].ExpenseAmount1 + productVoucherDetails[i].ExpenseAmount0 + productVoucherDetails[i].ImportTaxAmount : 0;
                    productVoucherDetails[i].ExportAmountCur = entity.VoucherGroup == 2 ? productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExciseAmountCur + productVoucherDetails[i].ExpenseAmountCur1 + productVoucherDetails[i].ExpenseAmountCur0 + productVoucherDetails[i].ImportTaxAmountCur : 0;
                }
                if (productVoucherDetails[i].ProductType != "D")
                {
                    productVoucherDetails[i].Price0 = productVoucherDetails[i].ExportQuantity + productVoucherDetails[i].ImportQuantity != 0 ? (productVoucherDetails[i].ImportAmount + productVoucherDetails[i].ExportAmount) / (productVoucherDetails[i].ExportQuantity + productVoucherDetails[i].ImportQuantity) : 0;
                    productVoucherDetails[i].PriceCur0 = productVoucherDetails[i].ExportQuantity + productVoucherDetails[i].ImportQuantity != 0 ? (productVoucherDetails[i].ImportAmountCur + productVoucherDetails[i].ExportAmountCur) / (productVoucherDetails[i].ExportQuantity + productVoucherDetails[i].ImportQuantity) : 0;
                }
                List<CrudProductVoucherDetailReceiptDto> productVoucherDetailReceipts = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[i].ProductVoucherDetailReceipts;

                CrudWarehouseBookDto CrudWarehouseBookDto = new CrudWarehouseBookDto();
                CrudWarehouseBookDto.OrgCode = entity.OrgCode;
                CrudWarehouseBookDto.ProductVoucherId = entity.Id;
                CrudWarehouseBookDto.Ord0 = "A" + (i + 1).ToString().PadLeft(9, '0');
                CrudWarehouseBookDto.Year = entity.Year;
                CrudWarehouseBookDto.Ord0Extra = "A" + (i + 1).ToString().PadLeft(9, '0');
                CrudWarehouseBookDto.DepartmentCode = entity.DepartmentCode;
                CrudWarehouseBookDto.VoucherCode = entity.VoucherCode;
                CrudWarehouseBookDto.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                CrudWarehouseBookDto.BusinessCode = entity.BusinessCode;
                CrudWarehouseBookDto.BusinessAcc = entity.BusinessAcc;
                CrudWarehouseBookDto.VoucherNumber = entity.VoucherNumber;
                CrudWarehouseBookDto.VoucherDate = entity.VoucherDate;
                CrudWarehouseBookDto.PaymentTermsCode = entity.PaymentTermsCode;
                CrudWarehouseBookDto.ContractCode = productVoucherDetails[i].ContractCode;
                CrudWarehouseBookDto.CurrencyCode = entity.CurrencyCode;
                CrudWarehouseBookDto.ExchangeRate = entity.ExchangeRate;
                CrudWarehouseBookDto.PartnerCode0 = entity.PartnerCode0;
                CrudWarehouseBookDto.Representative = entity.Representative;
                CrudWarehouseBookDto.Address = entity.Address;
                CrudWarehouseBookDto.Tel = entity.Tel;
                CrudWarehouseBookDto.Place = entity.Place;
                CrudWarehouseBookDto.OriginVoucher = entity.OriginVoucher;
                CrudWarehouseBookDto.Description = entity.Description;
                CrudWarehouseBookDto.DescriptionE = entity.DescriptionE;
                CrudWarehouseBookDto.ProductCode = productVoucherDetails[i].ProductCode;
                CrudWarehouseBookDto.TransProductCode = productVoucherDetails[i].TransProductCode;
                CrudWarehouseBookDto.UnitCode = productVoucherDetails[i].UnitCode;
                CrudWarehouseBookDto.TransferingUnitCode = productVoucherDetails[i].TransUnitCode;
                CrudWarehouseBookDto.WarehouseCode = productVoucherDetails[i].WarehouseCode;
                CrudWarehouseBookDto.TransWarehouseCode = productVoucherDetails[i].TransWarehouseCode;
                CrudWarehouseBookDto.ProductLotCode = productVoucherDetails[i].ProductLotCode;
                CrudWarehouseBookDto.TransProductLotCode = productVoucherDetails[i].TransProductLotCode;
                CrudWarehouseBookDto.ProductOriginCode = productVoucherDetails[i].ProductOriginCode;
                CrudWarehouseBookDto.TransProductOriginCode = productVoucherDetails[i].TransProductOriginCode;
                CrudWarehouseBookDto.PartnerCode = productVoucherDetails[i].PartnerCode;
                CrudWarehouseBookDto.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                CrudWarehouseBookDto.WorkPlaceCode = productVoucherDetails[i].WorkPlaceCode;
                CrudWarehouseBookDto.SectionCode = productVoucherDetails[i].SectionCode;
                CrudWarehouseBookDto.CaseCode = productVoucherDetails[i].CaseCode;
                CrudWarehouseBookDto.TrxQuantity = entity.VoucherCode == "PCP" ? 0 : productVoucherDetails[i].TrxQuantity;
                CrudWarehouseBookDto.TrxPrice = productVoucherDetails[i].TrxPrice;
                CrudWarehouseBookDto.TrxPriceCur = productVoucherDetails[i].TrxPriceCur;
                CrudWarehouseBookDto.Quantity = entity.VoucherCode == "PCP" ? 0 : productVoucherDetails[i].Quantity;
                CrudWarehouseBookDto.AmountCur = productVoucherDetails[i].AmountCur;
                CrudWarehouseBookDto.Amount = productVoucherDetails[i].Amount;
                CrudWarehouseBookDto.ExpenseAmountCur0 = productVoucherDetails[i].ExpenseAmountCur0;
                CrudWarehouseBookDto.ExpenseAmount0 = productVoucherDetails[i].ExpenseAmount0;
                CrudWarehouseBookDto.ExpenseAmountCur1 = productVoucherDetails[i].ExpenseAmountCur1;
                CrudWarehouseBookDto.ExpenseAmount1 = productVoucherDetails[i].ExpenseAmount1;
                CrudWarehouseBookDto.ExprenseAmountCur = productVoucherDetails[i].ExpenseAmountCur;
                CrudWarehouseBookDto.ExpenseAmount = productVoucherDetails[i].ExpenseAmount;
                CrudWarehouseBookDto.VatAmount = productVoucherDetails[i].VatAmount;
                CrudWarehouseBookDto.DiscountPercentage = productVoucherDetails[i].DiscountPercentage;
                CrudWarehouseBookDto.DiscountAmountCur = productVoucherDetails[i].DiscountAmountCur;
                CrudWarehouseBookDto.DiscountAmount = productVoucherDetails[i].DiscountAmount;
                CrudWarehouseBookDto.ImportTaxPercentage = productVoucherDetails[i].ImportTaxPercentage;
                CrudWarehouseBookDto.ImportTaxAmountCur = productVoucherDetails[i].ImportTaxAmountCur;
                CrudWarehouseBookDto.ImportTaxAmount = productVoucherDetails[i].ImportTaxAmount;
                CrudWarehouseBookDto.ExciseTaxPercentage = productVoucherDetails[i].ExciseTaxPercentage;
                CrudWarehouseBookDto.ExciseTaxAmountCur = productVoucherDetails[i].ExciseTaxAmountCur;
                CrudWarehouseBookDto.ExciseTaxAmount = productVoucherDetails[i].ExciseTaxAmount;
                CrudWarehouseBookDto.VatAmount = productVoucherDetails[i].VatAmount;
                CrudWarehouseBookDto.DebitAcc = productVoucherDetails[i].DebitAcc;
                CrudWarehouseBookDto.CreditAcc = productVoucherDetails[i].CreditAcc;
                CrudWarehouseBookDto.TrxPriceCur2 = productVoucherDetails[i].TrxPriceCur2;
                CrudWarehouseBookDto.TrxPrice2 = productVoucherDetails[i].TrxPrice2;
                CrudWarehouseBookDto.PriceCur2 = productVoucherDetails[i].PriceCur2;
                CrudWarehouseBookDto.Price = productVoucherDetails[i].Price;
                CrudWarehouseBookDto.PriceCur = productVoucherDetails[i].PriceCur;
                CrudWarehouseBookDto.Price2 = productVoucherDetails[i].Price2;
                CrudWarehouseBookDto.AmountCur2 = productVoucherDetails[i].AmountCur2 - productVoucherDetails[i].DecreaseAmount ?? 0;
                CrudWarehouseBookDto.Amount2 = productVoucherDetails[i].Amount2 - productVoucherDetails[i].DecreaseAmount ?? 0;
                CrudWarehouseBookDto.DebitAcc2 = productVoucherDetails[i].DebitAcc2;
                CrudWarehouseBookDto.CreditAcc2 = productVoucherDetails[i].CreditAcc2;
                CrudWarehouseBookDto.Note = productVoucherDetails[i].Note;
                CrudWarehouseBookDto.NoteE = productVoucherDetails[i].NoteE;
                CrudWarehouseBookDto.PriceCur0 = productVoucherDetails[i].PriceCur0;
                CrudWarehouseBookDto.Price0 = productVoucherDetails[i].Price0;
                CrudWarehouseBookDto.ImportAcc = entity.VoucherGroup == 1 ? productVoucherDetails[i].ProductAcc : productVoucherDetails[i].ImportAcc;
                CrudWarehouseBookDto.TrxImportQuantity = entity.VoucherCode == "PCP" ? 0 : (entity.VoucherGroup == 1 ? productVoucherDetails[i].Quantity : productVoucherDetails[i].TrxImportQuantity);
                CrudWarehouseBookDto.ImportQuantity = entity.VoucherCode == "PCP" ? 0 : (entity.VoucherGroup == 1 ? productVoucherDetails[i].Quantity : productVoucherDetails[i].ImportQuantity);
                CrudWarehouseBookDto.ImportAmountCur = entity.VoucherGroup == 1 ? productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExpenseAmountCur1 + productVoucherDetails[i].ExpenseAmountCur0 : productVoucherDetails[i].ImportAmountCur;
                CrudWarehouseBookDto.ImportAmount = entity.VoucherGroup == 1 ? productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount1 + productVoucherDetails[i].ExpenseAmount0 : productVoucherDetails[i].ImportAmount;
                CrudWarehouseBookDto.ExportAcc = entity.VoucherGroup == 2 ? productVoucherDetails[i].ProductAcc : productVoucherDetails[i].ExportAcc;
                CrudWarehouseBookDto.TrxExportQuantity = entity.VoucherCode == "PCP" ? 0 : (entity.VoucherGroup == 2 ? productVoucherDetails[i].Quantity : productVoucherDetails[i].TrxExportQuantity);
                CrudWarehouseBookDto.ExportQuantity = entity.VoucherCode == "PCP" ? 0 : (entity.VoucherGroup == 2 ? productVoucherDetails[i].Quantity : productVoucherDetails[i].ExportQuantity);
                CrudWarehouseBookDto.ExportAmountCur = entity.VoucherGroup == 2 ? productVoucherDetails[i].AmountCur : productVoucherDetails[i].ExportAmountCur;
                CrudWarehouseBookDto.ExportAmount = entity.VoucherGroup == 2 ? productVoucherDetails[i].Amount : productVoucherDetails[i].ExportAmount;
                CrudWarehouseBookDto.FixedPrice = productVoucherDetails[i].FixedPrice;
                CrudWarehouseBookDto.CalculateTransfering = false;
                CrudWarehouseBookDto.DebitOrCredit = null;
                CrudWarehouseBookDto.QuantityCur = entity.VoucherCode == "PCP" ? 0 : productVoucherDetails[i].Quantity;
                if (accTaxDetails != null)
                {
                    if (accTaxDetails.Count > 0)
                    {
                        CrudWarehouseBookDto.VatPercentage = accTaxDetails[0].VatPercentage;
                        CrudWarehouseBookDto.TaxCode = accTaxDetails[0].TaxCode;
                        CrudWarehouseBookDto.InvoiceSymbol = accTaxDetails[0].InvoiceSymbol;
                        CrudWarehouseBookDto.InvoiceNumber = accTaxDetails[0].InvoiceNumber;
                        CrudWarehouseBookDto.InvoiceDate = accTaxDetails[0].InvoiceDate;
                    }

                }
                CrudWarehouseBookDto.InvoicePartnerName = entity.PartnerName0;
                CrudWarehouseBookDto.InvoicePartnerAddress = entity.Address;
                CrudWarehouseBookDto.CreationTime = entity.VoucherDate;
                CrudWarehouseBookDto.SalesChannelCode = entity.SalesChannelCode;
                CrudWarehouseBookDto.BillNumber = entity.BillNumber;
                CrudWarehouseBookDto.VatAmountCur = productVoucherDetails[i].VatAmountCur;
                CrudWarehouseBookDto.VatPrice = productVoucherDetails[i].VatPrice;
                CrudWarehouseBookDto.DevaluationPriceCur = productVoucherDetails[i].DevaluationPriceCur;
                CrudWarehouseBookDto.DevaluationPrice = productVoucherDetails[i].DevaluationPrice;
                CrudWarehouseBookDto.DevaluationPriceCur = productVoucherDetails[i].DevaluationPriceCur;
                CrudWarehouseBookDto.DevaluationAmountCur = productVoucherDetails[i].DevaluationAmountCur;
                CrudWarehouseBookDto.DevaluationAmount = 0;
                CrudWarehouseBookDto.VarianceAmount = productVoucherDetails[i].VarianceAmount;
                CrudWarehouseBookDto.ProductName0 = productVoucherDetails[i].ProductName;
                CrudWarehouseBookDto.TotalAmount2 = productVoucherDetails[i].Amount2;
                CrudWarehouseBookDto.TotalDiscountAmount = entity.TotalDiscountAmount;
                CrudWarehouseBookDto.Status = entity.Status;
                if (entity.CreationTime != null)
                {
                    CrudWarehouseBookDto.CreationTime = entity.CreationTime;
                }
                else
                {
                    CrudWarehouseBookDto.CreationTime = DateTime.Now;
                }
                CrudWarehouseBookDtos.Add(CrudWarehouseBookDto);

            }

            return CrudWarehouseBookDtos;
        }
        public async Task<List<CrudWarehouseBookDto>> CreatewarehouseBookLrAsync(CrudProductVoucherDto dto)
        {
            List<CrudWarehouseBookDto> crudWarehouseBookDtos = new List<CrudWarehouseBookDto>();

            var vLISTCTLR = "";
            var vLGLR = 0;
            var vVHTKHU_HV = "";
            var vHACHTOAN = "C";
            var vSOKHO = "C";
            var vMAHT = "";
            var vLstMACT = "";
            var vLGHT2 = "";
            var vNHOMCT = "";
            var vMant0 = "";
            var vTTDBVALUE = "";
            var defaultVoucherTypelst = await _defaultVoucherTypeService.GetQueryableAsync();
            var defaultVoucherType = defaultVoucherTypelst.Where(p => p.Code == "PLR").FirstOrDefault();
            var listVoucher = "";
            var LstMA_CT = await _voucherTypeService.GetByVoucherTypeAsync("PLR");
            if (LstMA_CT.Count > 0)
            {
                listVoucher = LstMA_CT[0].ListVoucher;
            }
            else
            {
                listVoucher = defaultVoucherType.ListVoucher;
            }

            vLISTCTLR = listVoucher;


            if (string.IsNullOrEmpty(vLISTCTLR))
            {
                vLISTCTLR = "PLR";
            }

            if (vLISTCTLR.Contains(dto.VoucherCode) == true)
            {
                vLGLR = 1;

            }

            vVHTKHU_HV = await _tenantSettingService.GetValue("VHT_KHU_TRUNG_HV", dto.OrgCode);

            var vHACHTOANs = await _voucherCategoryService.GetByCodeAsync(dto.VoucherCode, _webHelper.GetCurrentOrgUnit());
            if (vHACHTOANs != null)
            {
                vHACHTOAN = vHACHTOANs.IsSavingLedger == null ? "C" : vHACHTOANs.IsSavingLedger;
                vSOKHO = vHACHTOANs.IsSavingWarehouseBook == null ? "C" : "K";
            }
            else
            {
                var defaultVoucher = await _defaultVoucherCategoryService.GetQueryableAsync();
                var lstDeffault = defaultVoucher.Where(p => p.Code == dto.VoucherCode).FirstOrDefault();
                vHACHTOAN = lstDeffault.IsSavingLedger == null ? "C" : lstDeffault.IsSavingLedger;
                vSOKHO = lstDeffault.IsSavingWarehouseBook == null ? "C" : "K";
            }


            if (vHACHTOAN == "K" && !string.IsNullOrEmpty(vSOKHO))
            {
                var businessCategories = await _businessCategoryService.GetLstBusinessByCodeAsync(dto.VoucherCode, _webHelper.GetCurrentOrgUnit());
                if (businessCategories.Count > 0)
                {
                    vHACHTOAN = businessCategories[0].IsAccVoucher == null ? "C" : "K";
                    vSOKHO = businessCategories[0].IsProductVoucher == null ? "C" : "K";

                }
            }

            if (vSOKHO != "K" && !string.IsNullOrEmpty(vMAHT))
            {
                var businessCategories = await _businessCategoryService.GetLstBusinessByCodeAsync(dto.VoucherCode, _webHelper.GetCurrentOrgUnit());
                var businessCategorie = businessCategories.Where(p => p.Code == vMAHT);
                if (businessCategorie.Count() > 0)
                {

                    vSOKHO = businessCategories[0].IsProductVoucher == null ? "C" : "K";

                }
            }
            // lấy dữ liệu đầu phiếu

            List<CrudProductVoucherDto> crudProductVoucherDtos = new List<CrudProductVoucherDto>();
            crudProductVoucherDtos.Add(dto);
            List<CrudAccTaxDetailDto> crudAccTaxDetailDtos = new List<CrudAccTaxDetailDto>();
            CrudAccTaxDetailDto crudAccTaxDetails = new CrudAccTaxDetailDto();
            // SELECT DPHV_id, MA_THUE, NGAY_HD, SO_SERIAL, SO_HD, PT_THUE, TEN_DT_KT, DIA_CHI, MS_THUE FROM dbo.PSTHUE WHERE DPHV_id = @p_id AND STT_REC0 = 'Z000000001'
            if (dto.AccTaxDetails.Count > 0)
            {
                List<CrudAccTaxDetailDto> cruds = new List<CrudAccTaxDetailDto>();
                cruds.Add(dto.AccTaxDetails[0]);
                crudAccTaxDetails.ProductVoucherId = dto.Id;
                if (cruds.Count > 0)
                {
                    crudAccTaxDetails.TaxCategoryCode = cruds[0].TaxCategoryCode;
                    crudAccTaxDetails.InvoiceDate = cruds[0].InvoiceDate;
                    crudAccTaxDetails.InvoiceSymbol = cruds[0].InvoiceSymbol;
                    crudAccTaxDetails.InvoiceNumber = cruds[0].InvoiceNumber;
                    crudAccTaxDetails.VatPercentage = cruds[0].VatPercentage;
                    crudAccTaxDetails.ClearingPartnerCode = cruds[0].ClearingPartnerCode;
                    crudAccTaxDetails.Address = cruds[0].Address;
                    crudAccTaxDetails.TaxCode = cruds[0].TaxCode;
                }
                crudAccTaxDetailDtos.Add(crudAccTaxDetails);


            }
            var productVoucher = from p in crudProductVoucherDtos
                                 join c in crudAccTaxDetailDtos on p.Id equals c.ProductVoucherId into d
                                 from e in d.DefaultIfEmpty()
                                 select new
                                 {
                                     Id = p.Id,
                                     OrgCode = p.OrgCode,
                                     Year = p.Year,
                                     DepartmentCode = p.DepartmentCode,
                                     VoucherCode = p.VoucherCode,
                                     VoucherGroup0 = p.VoucherGroup,
                                     NoteE = p.NoteE,
                                     Note = p.Note,
                                     VoucherGroup = p.VoucherGroup,
                                     BusinessCode = p.BusinessCode,
                                     BusinessAcc = p.BusinessAcc,
                                     VoucherNumber = p.VoucherNumber,
                                     InvoiceNumber = p.InvoiceNumber,
                                     VoucherDate = p.VoucherDate,
                                     PaymentTermsCode = p.PaymentTermsCode,
                                     FProductWorkCode = p.FProductWorkCode,
                                     CurrencyCode = p.CurrencyCode,
                                     ExchangeRate = p.ExchangeRate,
                                     PartnerCode0 = p.PartnerCode0,
                                     Representative = p.Representative,
                                     Address = p.Address,
                                     OriginVoucher = p.OriginVoucher,
                                     Description = p.Description,
                                     DescriptionE = p.DescriptionE,
                                     Status = p.Status,
                                     Place = p.Place,
                                     DebitAcc = p.DebitAcc,
                                     CreditAcc = p.CreditAcc,
                                     VatAmountCur = p.VatAmountCur,
                                     VatAmount = p.VatAmount,
                                     DevaluationDebitAcc = p.DevaluationDebitAcc,// tk giảm giá
                                     DevaluationCreditAcc = p.DevaluationCreditAcc,
                                     DevaluationAmount = p.TotalAmount,
                                     DevaluationAmountCur = p.TotalAmountCur,
                                     DiscountPercentage = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountPercentage : null,
                                     DiscountCreditAcc = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountCreditAcc : null,
                                     DiscountDebitAcc = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountDebitAcc : null,
                                     DiscountDescription = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountDescription : "Chiết khấu thương mại",
                                     DiscountAmountCru = p.TotalAmountCur,
                                     DiscountAmount = p.TotalAmount,
                                     DiscountCreditAcc0 = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountCreditAcc0 : null,
                                     DiscountDebitAcc0 = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountDebitAcc0 : null,
                                     DiscountDescription0 = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountDescription0 : "Chiết khấu thanh toán",
                                     DiscountAmountCru0 = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountAmountCur0 : null,
                                     DiscountAmount0 = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].DiscountAmount0 : null,
                                     ImportTaxPercentage = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ImportTaxPercentage : null,
                                     ImportCreditAcc = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ImportCreditAcc : null,
                                     ImportDebitAcc = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ImportDebitAcc : null,
                                     ImportDescription = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ImportDescription : "Thuế nhập khẩu",
                                     ImportAmountCru = p.TotalAmountCur,
                                     ImportAmount = p.TotalAmount,
                                     ExciseTaxPercentage = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ExciseTaxPercentage : null,
                                     ExciseTaxDebitAcc = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ExciseTaxDebitAcc : null,
                                     ExciseTaxCreditAcc = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ExciseTaxCreditAcc : null,
                                     ExciseTaxDescription = p.ProductVoucherReceipts != null ? p.ProductVoucherReceipts[0].ExciseTaxDescription : null,
                                     ExciseTaxAmountCru = p.TotalAmountCur,
                                     ExciseTaxAmount = p.TotalAmount,
                                     TaxCategoryCode = e != null ? e.TaxCategoryCode : null,
                                     TaxCode = e != null ? e.TaxCode : null,
                                     InvoiceDate = (DateTime?)(e != null ? e.InvoiceDate : null),
                                     InvoiceSymbol = e != null ? e.InvoiceSymbol : null,
                                     InvoiceNbr = e != null ? e.InvoiceNumber : null,
                                     BillNumber = p.BillNumber,
                                     VatPercentage = e != null ? e.VatPercentage : null,
                                     ClearingPartnerCode = e != null ? e.ClearingPartnerCode : null,
                                     AddressInv = e != null ? e.Address : null,
                                     PaymentTermsCodes = p.PaymentTermsCode,
                                     SalesChannelCode = p.SalesChannelCode,
                                     PartnerCode = p.PartnerCode0,
                                     ContractCode = p.ContractCode,
                                     WorkPlaceCode = p.PartnerCode0,
                                     SectionCode = p.SectionCode,
                                     CaseCode = p.CaseCode,
                                     AssemblyWarehouseCode = p.ProductVoucherAssemblies != null ? p.ProductVoucherAssemblies[0].AssemblyWarehouseCode : null,
                                     AssemblyProductCode = p.ProductVoucherAssemblies != null ? p.ProductVoucherAssemblies[0].AssemblyProductCode : null,
                                     AssemblyProductLotCode = p.ProductVoucherAssemblies != null ? p.ProductVoucherAssemblies[0].AssemblyProductLotCode : null,
                                     AssemblyWorkPlaceCode = p.ProductVoucherAssemblies != null ? p.ProductVoucherAssemblies[0].AssemblyWorkPlaceCode : null,
                                     AssemblyUnitCode = p.ProductVoucherAssemblies != null ? p.ProductVoucherAssemblies[0].AssemblyUnitCode : null,
                                     AssemblyPrice = p.ProductVoucherAssemblies != null ? p.ProductVoucherAssemblies[0].Price : null,
                                     AssemblyQuantity = p.Quantity,
                                     AssemblyPriceCur = p.PriceCur,
                                     AssemblyAmountCru = p.TotalProductAmountCur,
                                     AssemblyAmount = p.TotalProductAmount,
                                     ExpenseAmount = p.TotalExpenseAmount,
                                     ExpenseAmountCur = p.TotalExpenseAmountCur
                                 };
            List<CrudProductVoucherDetailDto> productVoucherdetails = crudProductVoucherDtos[0].ProductVoucherDetails;
            List<CrudProductVoucherCostDto> crudProductVoucherCostDtos = crudProductVoucherDtos[0].ProductVoucherCostDetails;
            var productVocherDetailResicrp = await _productVoucherDetailReceiptService.GetQueryableAsync();
            var lstproductVocherDetailResicrp = productVocherDetailResicrp.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var product = await _product.GetByProductAsync(_webHelper.GetCurrentOrgUnit());
            var wareHose = await _warehouseService.GetQueryableAsync();
            var wareHoses = wareHose.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productUnit = await _productUnitService.GetQueryableAsync();
            var productUnit1 = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.IsBasicUnit == true).ToList();
            var productUnit2 = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucherdetail = from b in productVoucherdetails
                                       join s in lstproductVocherDetailResicrp on b.Id equals s.ProductVoucherDetailId into o
                                       from i in o.DefaultIfEmpty()
                                       join c in product on b.ProductCode equals c.Code into p
                                       from e in p.DefaultIfEmpty()
                                       join w in wareHoses on b.WarehouseCode equals w.Code into n
                                       from m in n.DefaultIfEmpty()
                                       join ps in productUnit1 on b.ProductCode equals ps.ProductCode into pp
                                       from p1 in pp.DefaultIfEmpty()
                                       join pn in productUnit2 on new { b.ProductCode, b.UnitCode } equals new { pn.ProductCode, pn.UnitCode } into ph
                                       from p2 in ph.DefaultIfEmpty()
                                       join dp in productVoucher on b.ProductVoucherId equals dp.Id
                                       select new
                                       {
                                           VoucherGroup0 = dp.VoucherGroup,
                                           VoucherGroup = vLGLR == 1 ? 2 : 1,
                                           BusinessAcc = dp.BusinessAcc,
                                           DevaluationCreditAcc = dp.DevaluationCreditAcc,
                                           DevaluationDebitAcc = dp.DevaluationDebitAcc,
                                           DiscountCreditAcc = dp.DiscountCreditAcc,
                                           DiscountDebitAcc = dp.DiscountDebitAcc,
                                           Id = b.Id,
                                           ProductVoucherId = dp.Id,
                                           Rec0 = vLGLR == 1 ? b.Ord0 : "B" + b.Ord0.Substring(1, b.Ord0.Length - 1).ToString(),
                                           Ord0 = vLGLR == 1 ? b.Ord0 : "B" + b.Ord0.Substring(1, b.Ord0.Length - 1).ToString(),
                                           ContractCode = b.ContractCode,
                                           ProductCode = b.ProductCode,
                                           TransProductCode = b.TransProductCode,
                                           TransWarehouseCode = b.TransWarehouseCode,
                                           WarehouseCode = b.WarehouseCode,
                                           ProductLotCode = b.ProductLotCode,
                                           TransProductLotCode = b.TransProductLotCode,
                                           ProductOriginCode = b.ProductOriginCode,
                                           TransProductOriginCode = b.TransProductOriginCode,
                                           PartnerCode = !string.IsNullOrEmpty(b.PartnerCode) ? dp.PartnerCode0 : b.PartnerCode,
                                           FProductWorkCode = b.FProductWorkCode,
                                           UnitCode = b.UnitCode,
                                           TransUnitCode = b.TransUnitCode,
                                           ProductName0 = b.ProductName0,
                                           Quantity = b.Quantity,
                                           Price = b.Price,
                                           PriceCur = b.PriceCur,
                                           AmountCur = b.AmountCur,
                                           Amount = b.Amount,
                                           ExpenseAmountCur0 = i != null ? i.ExpenseAmountCur0 : 0,
                                           ExpenseAmount0 = i != null ? i.ExpenseAmount0 : 0,
                                           ExpenseAmount1 = i != null ? i.ExpenseAmount1 : 0,
                                           ExpenseAmountCur1 = i != null ? i.ExpenseAmountCur1 : 0,
                                           ExpenseAmountCur = i != null ? i.ExpenseAmountCur : 0,
                                           ExpenseAmount = i != null ? i.ExpenseAmount : 0,
                                           VatPercentage = i != null ? i.VatPercentage : 0,
                                           VatAmountCur = i != null ? i.VatAmountCur : 0,
                                           VatAmount = i != null ? i.VatAmount : 0,
                                           DiscountPercentage = i != null ? i.DiscountPercentage : 0,
                                           DiscountAmount = i != null ? i.DiscountAmount : 0,
                                           DiscountAmountCur = i != null ? i.DiscountAmountCur : 0,
                                           ImportTaxPercentage = i != null ? i.ImportTaxPercentage : 0,
                                           ImportTaxAmount = i != null ? i.ImportTaxAmount : 0,
                                           ImportTaxAmountCur = i != null ? i.ImportTaxAmountCur : 0,
                                           ExciseTaxPercentage = i != null ? i.ExciseTaxPercentage : 0,
                                           ExciseTaxAmount = i != null ? i.ExciseTaxAmount : 0,
                                           ExciseTaxAmountCur = i != null ? i.ExciseTaxAmountCur : 0,
                                           DebitAcc0 = b.DebitAcc,
                                           DebitAcc = b.DebitAcc,
                                           CreditAcc0 = b.CreditAcc,
                                           CreditAcc = b.CreditAcc,
                                           PriceCur2 = b.PriceCur2,
                                           Price2 = b.Price2,
                                           AmountCur2 = b.AmountCur2,
                                           Amount2 = b.Amount2,
                                           DebitAcc2 = b.DebitAcc2,
                                           CreditAcc2 = b.CreditAcc2,
                                           Note = !string.IsNullOrEmpty(b.Note) ? dp.Description : b.Note,
                                           NoteE = !string.IsNullOrEmpty(b.NoteE) ? dp.DescriptionE : b.NoteE,
                                           SectionCode = b.SectionCode,
                                           WorkPlaceCode = b.WorkPlaceCode,
                                           CaseCode = b.CaseCode,
                                           FixedPrice = b.FixedPrice,
                                           VatPrice = b.VatPrice,

                                           DecreasePercentage = b.DecreasePercentage,
                                           DevaluationPrice = b.DevaluationPrice,
                                           DevaluationPriceCur = b.DevaluationPriceCur,
                                           DevaluationAmount = b.DevaluationAmount,
                                           DevaluationAmountCur = b.DevaluationAmountCur,
                                           VarianceAmount = b.VarianceAmount,
                                           TaxCategoryCode = b.TaxCategoryCode,
                                           ProductType = e != null ? e.ProductType : "D",
                                           //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                           WarehouseAcc = m != null ? m.WarehouseAcc : null,
                                           ProductAcc = e != null ? e.ProductAcc : null,
                                           IsOriginVoucher = "C",//ctgoc
                                           IsLedger = "K",
                                           IsLedger2 = "K",
                                           TrxQuantity = b.Quantity,
                                           TrxPriceCru = b.PriceCur,
                                           TrxPrice = b.Price,
                                           TrxPriceCru2 = b.PriceCur2,
                                           TrxPrice2 = b.Price2,
                                           QuantityQd = 0,
                                           PriceCur0 = b.PriceCur,
                                           Price0 = b.Price,
                                           AccImport = dp.BusinessAcc,//tknhap
                                           QuantityTrxN = b.Quantity,
                                           ImportQuantity = b.Quantity,
                                           ImportAmountCru = b.AmountCur,
                                           ImportAmount = b.Amount,
                                           AccExport = dp.BusinessAcc,
                                           QuantityTrxX = b.Quantity,
                                           ExportQuantity = b.Quantity,
                                           ExportAmountCru = b.AmountCur,
                                           ExportAmount = b.Amount,
                                           AssemblyWarehouseCode = dp.AssemblyWarehouseCode,
                                           AssemblyProductCode = dp.AssemblyProductCode,
                                           AssemblyProductLotCode = dp.AssemblyProductLotCode,
                                           AssemblyUnitCode = dp.AssemblyUnitCode,
                                           AssemblyQuantity = dp.AssemblyQuantity,
                                           AssemblyPriceCur = dp.AssemblyPriceCur,
                                           AssemblyPrice = dp.AssemblyPrice,
                                           AssemblyAmountCru = dp.AssemblyAmountCru + b.AmountCur,
                                           AssemblyAmount = dp.AssemblyAmount + b.Amount,
                                           VatPriceCur = b.VatPriceCur,
                                           DevaluationPercentage = b.DevaluationPercentage,
                                           e = b.ExciseTaxAmountCur
                                       };
            //Các mã là dịch vụ 'D'
            var productVoucherdetailUpdate1 = from a in productVoucherdetail
                                              where a.ProductType == "D"
                                              select new
                                              {
                                                  VoucherGroup0 = a.VoucherGroup,
                                                  VoucherGroup = a.VoucherGroup,
                                                  BusinessAcc = a.BusinessAcc,
                                                  DevaluationCreditAcc = a.DevaluationCreditAcc,
                                                  DevaluationDebitAcc = a.DevaluationDebitAcc,
                                                  DiscountCreditAcc = a.DiscountCreditAcc,
                                                  DiscountDebitAcc = a.DiscountDebitAcc,
                                                  Id = a.Id,
                                                  ProductVoucherId = a.ProductVoucherId,
                                                  Rec0 = a.Ord0,
                                                  Ord0 = a.Ord0,
                                                  ContractCode = a.ContractCode,
                                                  ProductCode = a.ProductCode,
                                                  TransProductCode = a.TransProductCode,
                                                  TransWarehouseCode = a.TransWarehouseCode,
                                                  WarehouseCode = a.WarehouseCode,
                                                  ProductLotCode = a.ProductLotCode,
                                                  TransProductLotCode = a.TransProductLotCode,
                                                  ProductOriginCode = a.ProductOriginCode,
                                                  TransProductOriginCode = a.TransProductOriginCode,
                                                  PartnerCode = a.PartnerCode,
                                                  FProductWorkCode = a.FProductWorkCode,
                                                  UnitCode = a.UnitCode,
                                                  TransUnitCode = a.TransUnitCode,
                                                  ProductName0 = a.ProductName0,
                                                  Quantity = a.Quantity,
                                                  TaxCategoryCode = a.TaxCategoryCode,
                                                  Price = a.Price,
                                                  PriceCur = a.PriceCur,
                                                  AmountCur = a.AmountCur,
                                                  Amount = a.Amount,
                                                  ExpenseAmountCur0 = a.ExpenseAmountCur0,
                                                  ExpenseAmount0 = a.ExpenseAmount0,
                                                  ExpenseAmount1 = a.ExpenseAmount1,
                                                  ExpenseAmountCur1 = a.ExpenseAmountCur1,
                                                  ExpenseAmountCur = a.ExpenseAmountCur,
                                                  ExpenseAmount = a.ExpenseAmount,
                                                  VatPercentage = a.VatPercentage,
                                                  VatAmountCur = a.VatAmountCur,
                                                  VatAmount = a.VatAmount,
                                                  DiscountPercentage = a.DiscountPercentage,
                                                  DiscountAmount = a.DiscountAmount,
                                                  DiscountAmountCur = a.DiscountAmountCur,
                                                  ImportTaxPercentage = a.ImportTaxPercentage,
                                                  ImportTaxAmount = a.ImportTaxAmount,
                                                  ImportTaxAmountCur = a.ImportTaxAmountCur,
                                                  ExciseTaxPercentage = a.ExciseTaxPercentage,
                                                  ExciseAmount = a.ExciseTaxAmount,
                                                  ExciseAmountCur = a.ExciseTaxAmountCur,
                                                  DebitAcc0 = a.DebitAcc,
                                                  DebitAcc = a.DebitAcc,
                                                  CreditAcc0 = a.CreditAcc,
                                                  CreditAcc = a.CreditAcc,
                                                  PriceCur2 = a.PriceCur2,
                                                  Price2 = a.Price2,
                                                  AmountCur2 = a.AmountCur2,
                                                  Amount2 = a.Amount2,
                                                  DebitAcc2 = a.DebitAcc2,
                                                  CreditAcc2 = a.CreditAcc2,
                                                  Note = a.Note,
                                                  NoteE = a.NoteE,
                                                  SectionCode = a.SectionCode,
                                                  WorkPlaceCode = a.WorkPlaceCode,
                                                  CaseCode = a.CaseCode,
                                                  FixedPrice = a.FixedPrice,
                                                  VatPrice = a.VatPrice,
                                                  DecreasePercentage = a.DecreasePercentage,
                                                  DevaluationPrice = a.DevaluationPrice,
                                                  DevaluationPriceCur = a.DevaluationPriceCur,
                                                  DevaluationAmount = a.DevaluationAmount,
                                                  DevaluationAmountCur = a.DevaluationAmountCur,
                                                  VarianceAmount = a.VarianceAmount,
                                                  ProductType = a.ProductType,
                                                  //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                                  WarehouseAcc = a.WarehouseAcc,
                                                  ProductAcc = a.ProductAcc,
                                                  IsOriginVoucher = a.IsOriginVoucher,//ctgoc
                                                  IsLedger = a.IsLedger,
                                                  IsLedger2 = a.IsLedger2,
                                                  TrxQuantity = a.Quantity,
                                                  TrxPriceCru = a.PriceCur,
                                                  TrxPrice = a.Price,
                                                  TrxPriceCru2 = a.PriceCur2,
                                                  TrxPrice2 = a.Price2,
                                                  QuantityQd = a.QuantityQd,
                                                  PriceCur0 = 0,
                                                  Price0 = 0,
                                                  AccImport = "",//tknhap
                                                  QuantityTrxN = 0,
                                                  ImportQuantity = 0,
                                                  ImportAmountCru = 0,
                                                  ImportAmount = 0,
                                                  AccExport = "",
                                                  QuantityTrxX = 0,
                                                  ExportQuantity = 0,
                                                  ExportAmountCru = 0,
                                                  ExportAmount = 0,
                                                  AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                                  AssemblyProductCode = a.AssemblyProductCode,
                                                  AssemblyProductLotCode = a.AssemblyProductLotCode,
                                                  AssemblyUnitCode = a.AssemblyUnitCode,
                                                  AssemblyQuantity = a.AssemblyQuantity,
                                                  AssemblyPriceCur = a.AssemblyPriceCur,
                                                  AssemblyPrice = a.AssemblyPrice,
                                                  AssemblyAmountCru = a.AssemblyAmountCru,
                                                  AssemblyAmount = a.AssemblyAmount,
                                                  VatPriceCur = a.VatPriceCur,
                                                  DevaluationPercentage = a.DevaluationPercentage,
                                              };
            var resulProductVoucherDetal = from a in productVoucherdetail
                                           join b in productVoucherdetailUpdate1 on a.Id equals b.Id into c
                                           from d in c.DefaultIfEmpty()
                                           select new
                                           {
                                               VoucherGroup0 = a.VoucherGroup,
                                               VoucherGroup = a.VoucherGroup,
                                               BusinessAcc = a.BusinessAcc,
                                               DevaluationCreditAcc = a.DevaluationCreditAcc,
                                               DevaluationDebitAcc = a.DevaluationDebitAcc,
                                               DiscountCreditAcc = a.DiscountCreditAcc,
                                               DiscountDebitAcc = a.DiscountDebitAcc,
                                               Id = a.Id,
                                               ProductVoucherId = a.ProductVoucherId,
                                               Rec0 = a.Ord0,
                                               Ord0 = a.Ord0,
                                               ContractCode = a.ContractCode,
                                               ProductCode = a.ProductCode,
                                               TransProductCode = a.TransProductCode,
                                               TransWarehouseCode = a.TransWarehouseCode,
                                               WarehouseCode = a.WarehouseCode,
                                               ProductLotCode = a.ProductLotCode,
                                               TransProductLotCode = a.TransProductLotCode,
                                               ProductOriginCode = a.ProductOriginCode,
                                               TransProductOriginCode = a.TransProductOriginCode,
                                               PartnerCode = a.PartnerCode,
                                               FProductWorkCode = a.FProductWorkCode,
                                               UnitCode = a.UnitCode,
                                               TaxCategoryCode = a.TaxCategoryCode,
                                               TransUnitCode = a.TransUnitCode,
                                               ProductName0 = a.ProductName0,
                                               Quantity = a.Quantity,
                                               Price = a.Price,
                                               PriceCur = a.PriceCur,
                                               AmountCur = a.AmountCur,
                                               Amount = a.Amount,
                                               ExpenseAmountCur0 = a.ExpenseAmountCur0,
                                               ExpenseAmount0 = a.ExpenseAmount0,
                                               ExpenseAmount1 = a.ExpenseAmount1,
                                               ExpenseAmountCur1 = a.ExpenseAmountCur1,
                                               ExpenseAmountCur = a.ExpenseAmountCur,
                                               ExpenseAmount = a.ExpenseAmount,
                                               VatPercentage = a.VatPercentage,
                                               VatAmountCur = a.VatAmountCur,
                                               VatAmount = a.VatAmount,
                                               DiscountPercentage = a.DiscountPercentage,
                                               DiscountAmount = a.DiscountAmount,
                                               DiscountAmountCur = a.DiscountAmountCur,
                                               ImportTaxPercentage = a.ImportTaxPercentage,
                                               ImportTaxAmount = a.ImportTaxAmount,
                                               ImportTaxAmountCur = a.ImportTaxAmountCur,
                                               ExciseTaxPercentage = a.ExciseTaxPercentage,
                                               ExciseAmount = a.ExciseTaxAmount,
                                               ExciseAmountCur = a.ExciseTaxAmountCur,
                                               DebitAcc0 = a.DebitAcc,
                                               DebitAcc = a.DebitAcc,
                                               CreditAcc0 = a.CreditAcc,
                                               CreditAcc = a.CreditAcc,
                                               PriceCur2 = a.PriceCur2,
                                               Price2 = a.Price2,
                                               AmountCur2 = a.AmountCur2,
                                               Amount2 = a.Amount2,
                                               DebitAcc2 = a.DebitAcc2,
                                               CreditAcc2 = a.CreditAcc2,
                                               Note = a.Note,
                                               NoteE = a.NoteE,
                                               SectionCode = a.SectionCode,
                                               WorkPlaceCode = a.WorkPlaceCode,
                                               CaseCode = a.CaseCode,
                                               FixedPrice = a.FixedPrice,
                                               VatPrice = a.VatPrice,
                                               DecreasePercentage = a.DecreasePercentage,
                                               DevaluationPrice = a.DevaluationPrice,
                                               DevaluationPriceCur = a.DevaluationPriceCur,
                                               DevaluationAmount = a.DevaluationAmount,
                                               DevaluationAmountCur = a.DevaluationAmountCur,
                                               VarianceAmount = a.VarianceAmount,
                                               ProductType = a.ProductType,
                                               //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                               WarehouseAcc = a.WarehouseAcc,
                                               ProductAcc = a.ProductAcc,
                                               IsOriginVoucher = a.IsOriginVoucher,//ctgoc
                                               IsLedger = a.IsLedger,
                                               IsLedger2 = a.IsLedger2,
                                               TrxQuantity = a.Quantity,
                                               TrxPriceCru = a.PriceCur,
                                               TrxPrice = a.Price,
                                               TrxPriceCru2 = a.PriceCur2,
                                               TrxPrice2 = a.Price2,
                                               QuantityQd = a.QuantityQd,
                                               PriceCur0 = d != null ? d.PriceCur0 : a.PriceCur0,
                                               Price0 = d != null ? d.Price0 : a.Price0,
                                               AccImport = d != null ? d.AccExport : a.AccExport,//tknhap
                                               QuantityTrxN = d != null ? d.QuantityQd : a.QuantityQd,
                                               ImportQuantity = d != null ? d.ImportQuantity : a.ImportQuantity,
                                               ImportAmountCru = d != null ? d.ImportAmountCru : a.ImportAmountCru,
                                               ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                               AccExport = d != null ? d.AccExport : a.AccExport,
                                               QuantityTrxX = d != null ? d.QuantityTrxX : a.QuantityTrxX,
                                               ExportQuantity = d != null ? d.ExportQuantity : a.ExportQuantity,
                                               ExportAmountCru = d != null ? d.ExportAmountCru : a.ExportAmountCru,
                                               ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                               AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                               AssemblyProductCode = a.AssemblyProductCode,
                                               AssemblyProductLotCode = a.AssemblyProductLotCode,
                                               AssemblyUnitCode = a.AssemblyUnitCode,
                                               AssemblyQuantity = a.AssemblyQuantity,
                                               AssemblyPriceCur = a.AssemblyPriceCur,
                                               AssemblyPrice = a.AssemblyPrice,
                                               AssemblyAmountCru = a.AssemblyAmountCru,
                                               AssemblyAmount = a.AssemblyAmount,
                                               VatPriceCur = a.VatPriceCur,
                                               DevaluationPercentage = a.DevaluationPercentage,
                                           };
            //Cập nhật những mã ko phải dịch vụ
            var productVoucherdetailUpdate2 = from a in productVoucherdetail
                                              where a.ProductType != "D"
                                              select new
                                              {
                                                  VoucherGroup0 = a.VoucherGroup,
                                                  VoucherGroup = a.VoucherGroup,
                                                  BusinessAcc = a.BusinessAcc,
                                                  DevaluationCreditAcc = a.DevaluationCreditAcc,
                                                  DevaluationDebitAcc = a.DevaluationDebitAcc,
                                                  DiscountCreditAcc = a.DiscountCreditAcc,
                                                  DiscountDebitAcc = a.DiscountDebitAcc,
                                                  Id = a.Id,
                                                  ProductVoucherId = a.ProductVoucherId,
                                                  Rec0 = a.Ord0,
                                                  Ord0 = a.Ord0,
                                                  ContractCode = a.ContractCode,
                                                  ProductCode = a.ProductCode,
                                                  TransProductCode = a.TransProductCode,
                                                  TransWarehouseCode = a.TransWarehouseCode,
                                                  WarehouseCode = a.WarehouseCode,
                                                  ProductLotCode = a.ProductLotCode,
                                                  TaxCategoryCode = a.TaxCategoryCode,
                                                  TransProductLotCode = a.TransProductLotCode,
                                                  ProductOriginCode = a.ProductOriginCode,
                                                  TransProductOriginCode = a.TransProductOriginCode,
                                                  PartnerCode = a.PartnerCode,
                                                  FProductWorkCode = a.FProductWorkCode,
                                                  UnitCode = a.UnitCode,
                                                  TransUnitCode = a.TransUnitCode,
                                                  ProductName0 = a.ProductName0,
                                                  Quantity = a.Quantity,
                                                  Price = a.Price,
                                                  PriceCur = a.PriceCur,
                                                  AmountCur = a.AmountCur,
                                                  Amount = a.Amount,
                                                  ExpenseAmountCur0 = a.ExpenseAmountCur0,
                                                  ExpenseAmount0 = a.ExpenseAmount0,
                                                  ExpenseAmount1 = a.ExpenseAmount1,
                                                  ExpenseAmountCur1 = a.ExpenseAmountCur1,
                                                  ExpenseAmountCur = a.ExpenseAmountCur,
                                                  ExpenseAmount = a.ExpenseAmount,
                                                  VatPercentage = a.VatPercentage,
                                                  VatAmountCur = a.VatAmountCur,
                                                  VatAmount = a.VatAmount,
                                                  DiscountPercentage = a.DiscountPercentage,
                                                  DiscountAmount = a.DiscountAmount,
                                                  DiscountAmountCur = a.DiscountAmountCur,
                                                  ImportTaxPercentage = a.ImportTaxPercentage,
                                                  ImportTaxAmount = a.ImportTaxAmount,
                                                  ImportTaxAmountCur = a.ImportTaxAmountCur,
                                                  ExciseTaxPercentage = a.ExciseTaxPercentage,
                                                  ExciseAmount = a.ExciseTaxAmount,
                                                  ExciseAmountCur = a.ExciseTaxAmountCur,
                                                  DebitAcc0 = a.DebitAcc,
                                                  DebitAcc = a.DebitAcc,
                                                  CreditAcc0 = a.CreditAcc,
                                                  CreditAcc = a.CreditAcc,
                                                  PriceCur2 = a.PriceCur2,
                                                  Price2 = a.Price2,
                                                  AmountCur2 = a.AmountCur2,
                                                  Amount2 = a.Amount2,
                                                  DebitAcc2 = a.DebitAcc2,
                                                  CreditAcc2 = a.CreditAcc2,
                                                  Note = a.Note,
                                                  NoteE = a.NoteE,
                                                  SectionCode = a.SectionCode,
                                                  WorkPlaceCode = a.WorkPlaceCode,
                                                  CaseCode = a.CaseCode,
                                                  FixedPrice = a.FixedPrice,
                                                  VatPrice = a.VatPrice,
                                                  DecreasePercentage = a.DecreasePercentage,
                                                  DevaluationPrice = a.DevaluationPrice,
                                                  DevaluationPriceCur = a.DevaluationPriceCur,
                                                  DevaluationAmount = a.DevaluationAmount,
                                                  DevaluationAmountCur = a.DevaluationAmountCur,
                                                  VarianceAmount = a.VarianceAmount,
                                                  ProductType = a.ProductType,
                                                  //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                                  WarehouseAcc = a.WarehouseAcc,
                                                  ProductAcc = a.ProductAcc,
                                                  IsOriginVoucher = a.IsOriginVoucher,//ctgoc
                                                  IsLedger = a.IsLedger,
                                                  IsLedger2 = a.IsLedger2,
                                                  TrxQuantity = a.Quantity,
                                                  TrxPriceCru = a.PriceCur,
                                                  TrxPrice = a.Price,
                                                  TrxPriceCru2 = a.PriceCur2,
                                                  TrxPrice2 = a.Price2,
                                                  QuantityQd = a.QuantityQd,
                                                  PriceCur0 = a.PriceCur0,
                                                  Price0 = a.Price0,
                                                  AccImport = a.VoucherGroup == 1 ? a.DebitAcc : "",//tknhap
                                                  QuantityTrxN = a.VoucherGroup == 1 ? a.QuantityTrxN : 0,
                                                  ImportQuantity = a.ImportQuantity,
                                                  ImportAmountCru = a.VoucherGroup == 1 ? (a.AmountCur != null ? a.AmountCur : 0) + (a.ImportTaxAmountCur != null ? a.ImportTaxAmountCur : 0) + (a.ExpenseAmountCur0 != null ? a.ExpenseAmountCur0 : 0) + (a.ExpenseAmountCur1 != null ? a.ExpenseAmountCur1 : 0) + (vTTDBVALUE == "C" ? 0 : a.ExciseTaxAmountCur) : 0,
                                                  ImportAmount = a.VoucherGroup == 1 ? (a.Amount != null ? a.Amount : 0) + (a.ImportAmount != null ? a.ImportAmount : 0) + (a.ExpenseAmount0 != null ? a.ExpenseAmount0 : 0) + (a.ExpenseAmount1 != null ? a.ExpenseAmount1 : 0) + (vTTDBVALUE == "C" ? 0 : a.ExciseTaxAmount) : 0,
                                                  AccExport = a.VoucherGroup == 2 ? a.CreditAcc : null,
                                                  QuantityTrxX = a.VoucherGroup == 2 ? a.QuantityTrxX : 0,
                                                  ExportQuantity = a.VoucherGroup == 2 ? a.ExportQuantity : 0,
                                                  ExportAmountCru = a.VoucherGroup == 2 ? (a.AmountCur != null ? a.AmountCur : 0) + (a.ImportTaxAmountCur != null ? a.ImportTaxAmountCur : 0) + (a.ExpenseAmountCur0 != null ? a.ExpenseAmountCur0 : 0) + (a.ExpenseAmountCur1 != null ? a.ExpenseAmountCur1 : 0) + (a.ExciseTaxAmountCur == null ? 0 : a.ExciseTaxAmountCur) : 0,
                                                  ExportAmount = a.VoucherGroup == 2 ? (a.Amount != null ? a.Amount : 0) + (a.ImportAmount != null ? a.ImportAmount : 0) + (a.ExpenseAmount0 != null ? a.ExpenseAmount0 : 0) + (a.ExpenseAmount1 != null ? a.ExpenseAmount1 : 0) + (vTTDBVALUE == "C" ? 0 : a.ExciseTaxAmount) : 0,
                                                  AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                                  AssemblyProductCode = a.AssemblyProductCode,
                                                  AssemblyProductLotCode = a.AssemblyProductLotCode,
                                                  AssemblyUnitCode = a.AssemblyUnitCode,
                                                  AssemblyQuantity = a.AssemblyQuantity,
                                                  AssemblyPriceCur = a.AssemblyPriceCur,
                                                  AssemblyPrice = a.AssemblyPrice,
                                                  AssemblyAmountCru = a.AssemblyAmountCru,
                                                  AssemblyAmount = a.AssemblyAmount,
                                                  VatPriceCur = a.VatPriceCur,
                                                  DevaluationPercentage = a.DevaluationPercentage,
                                              };
            var resulProductVoucherDetal2 = from a in resulProductVoucherDetal
                                            join b in productVoucherdetailUpdate2 on a.Id equals b.Id into c
                                            from d in c.DefaultIfEmpty()
                                            select new
                                            {
                                                VoucherGroup0 = a.VoucherGroup,
                                                VoucherGroup = a.VoucherGroup,
                                                BusinessAcc = a.BusinessAcc,
                                                DevaluationCreditAcc = a.DevaluationCreditAcc,
                                                DevaluationDebitAcc = a.DevaluationDebitAcc,
                                                DiscountCreditAcc = a.DiscountCreditAcc,
                                                DiscountDebitAcc = a.DiscountDebitAcc,
                                                Id = a.Id,
                                                ProductVoucherId = a.ProductVoucherId,
                                                Rec0 = a.Ord0,
                                                Ord0 = a.Ord0,
                                                ContractCode = a.ContractCode,
                                                ProductCode = a.ProductCode,
                                                TransProductCode = a.TransProductCode,
                                                TransWarehouseCode = a.TransWarehouseCode,
                                                WarehouseCode = a.WarehouseCode,
                                                ProductLotCode = a.ProductLotCode,
                                                TransProductLotCode = a.TransProductLotCode,
                                                ProductOriginCode = a.ProductOriginCode,
                                                TransProductOriginCode = a.TransProductOriginCode,
                                                PartnerCode = a.PartnerCode,
                                                TaxCategoryCode = a.TaxCategoryCode,
                                                FProductWorkCode = a.FProductWorkCode,
                                                UnitCode = a.UnitCode,
                                                TransUnitCode = a.TransUnitCode,
                                                ProductName0 = a.ProductName0,
                                                Quantity = a.Quantity,
                                                Price = a.Price,
                                                PriceCur = a.PriceCur,
                                                AmountCur = a.AmountCur,
                                                Amount = a.Amount,
                                                ExpenseAmountCur0 = a.ExpenseAmountCur0,
                                                ExpenseAmount0 = a.ExpenseAmount0,
                                                ExpenseAmount1 = a.ExpenseAmount1,
                                                ExpenseAmountCur1 = a.ExpenseAmountCur1,
                                                ExpenseAmountCur = a.ExpenseAmountCur,
                                                ExpenseAmount = a.ExpenseAmount,
                                                VatPercentage = a.VatPercentage,
                                                VatAmountCur = a.VatAmountCur,
                                                VatAmount = a.VatAmount,
                                                DiscountPercentage = a.DiscountPercentage,
                                                DiscountAmount = a.DiscountAmount,
                                                DiscountAmountCur = a.DiscountAmountCur,
                                                ImportTaxPercentage = a.ImportTaxPercentage,
                                                ImportTaxAmount = a.ImportTaxAmount,
                                                ImportTaxAmountCur = a.ImportTaxAmountCur,
                                                ExciseTaxPercentage = a.ExciseTaxPercentage,
                                                ExciseAmount = a.ExciseAmount,
                                                ExciseAmountCur = a.ExciseAmountCur,
                                                DebitAcc0 = a.DebitAcc,
                                                DebitAcc = a.DebitAcc,
                                                CreditAcc0 = a.CreditAcc,
                                                CreditAcc = a.CreditAcc,
                                                PriceCur2 = a.PriceCur2,
                                                Price2 = a.Price2,
                                                AmountCur2 = a.AmountCur2,
                                                Amount2 = a.Amount2,
                                                DebitAcc2 = a.DebitAcc2,
                                                CreditAcc2 = a.CreditAcc2,

                                                Note = a.Note,
                                                NoteE = a.NoteE,
                                                SectionCode = a.SectionCode,
                                                WorkPlaceCode = a.WorkPlaceCode,
                                                CaseCode = a.CaseCode,
                                                FixedPrice = a.FixedPrice,
                                                VatPrice = a.VatPrice,
                                                DecreasePercentage = a.DecreasePercentage,
                                                DevaluationPrice = a.DevaluationPrice,
                                                DevaluationPriceCur = a.DevaluationPriceCur,
                                                DevaluationAmount = a.DevaluationAmount,
                                                DevaluationAmountCur = a.DevaluationAmountCur,
                                                VarianceAmount = a.VarianceAmount,
                                                ProductType = a.ProductType,
                                                //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                                WarehouseAcc = a.WarehouseAcc,
                                                ProductAcc = a.ProductAcc,
                                                IsOriginVoucher = a.IsOriginVoucher,//ctgoc
                                                IsLedger = a.IsLedger,
                                                IsLedger2 = a.IsLedger2,
                                                TrxQuantity = a.Quantity,
                                                TrxPriceCru = a.PriceCur,
                                                TrxPrice = a.Price,
                                                TrxPriceCru2 = a.PriceCur2,
                                                TrxPrice2 = a.Price2,
                                                QuantityQd = a.QuantityQd,
                                                PriceCur0 = d != null ? d.PriceCur0 : a.PriceCur0,
                                                Price0 = d != null ? d.Price0 : a.Price0,
                                                AccImport = d.AccImport,//tknhap
                                                QuantityTrxN = d != null ? d.QuantityQd : a.QuantityQd,
                                                ImportQuantity = d != null ? d.ImportQuantity : a.ImportQuantity,
                                                ImportAmountCru = d != null ? d.ImportAmountCru : a.ImportAmountCru,
                                                ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                                AccExport = d != null ? d.AccExport : a.AccExport,
                                                QuantityTrxX = d != null ? d.QuantityTrxX : a.QuantityTrxX,
                                                ExportQuantity = d != null ? d.ExportQuantity : a.ExportQuantity,
                                                ExportAmountCru = d != null ? d.ExportAmountCru : a.ExportAmountCru,
                                                ExportAmount = d != null ? d.ExportAmount : a.ExportAmount,
                                                AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                                AssemblyProductCode = a.AssemblyProductCode,
                                                AssemblyProductLotCode = a.AssemblyProductLotCode,
                                                AssemblyUnitCode = a.AssemblyUnitCode,
                                                AssemblyQuantity = a.AssemblyQuantity,
                                                AssemblyPriceCur = a.AssemblyPriceCur,
                                                AssemblyPrice = a.AssemblyPrice,
                                                AssemblyAmountCru = a.AssemblyAmountCru,
                                                AssemblyAmount = a.AssemblyAmount,
                                                VatPriceCur = a.VatPriceCur,
                                                DevaluationPercentage = a.DevaluationPercentage

                                            };
            //Cập nhật giá vốn sau khi tính
            var resulProductVoucherDetal3 = from a in resulProductVoucherDetal2
                                            where (a.ImportQuantity + a.ExportQuantity) != 0 && (a.ImportAmountCru + a.ExportAmountCru) != 0 && a.ProductType != "D"
                                            select new
                                            {
                                                VoucherGroup0 = a.VoucherGroup,
                                                VoucherGroup = a.VoucherGroup,
                                                BusinessAcc = a.BusinessAcc,
                                                DevaluationCreditAcc = a.DevaluationCreditAcc,
                                                DevaluationDebitAcc = a.DevaluationDebitAcc,
                                                DiscountCreditAcc = a.DiscountCreditAcc,
                                                DiscountDebitAcc = a.DiscountDebitAcc,
                                                Id = a.Id,
                                                ProductVoucherId = a.ProductVoucherId,
                                                Rec0 = a.Ord0,
                                                Ord0 = a.Ord0,
                                                ContractCode = a.ContractCode,
                                                ProductCode = a.ProductCode,
                                                TransProductCode = a.TransProductCode,
                                                TransWarehouseCode = a.TransWarehouseCode,
                                                WarehouseCode = a.WarehouseCode,
                                                ProductLotCode = a.ProductLotCode,
                                                TransProductLotCode = a.TransProductLotCode,
                                                ProductOriginCode = a.ProductOriginCode,
                                                TransProductOriginCode = a.TransProductOriginCode,
                                                PartnerCode = a.PartnerCode,
                                                FProductWorkCode = a.FProductWorkCode,
                                                UnitCode = a.UnitCode,
                                                TransUnitCode = a.TransUnitCode,
                                                ProductName0 = a.ProductName0,
                                                Quantity = a.Quantity,
                                                TaxCategoryCode = a.TaxCategoryCode,
                                                Price = a.Price,
                                                PriceCur = a.PriceCur,
                                                AmountCur = a.AmountCur,
                                                Amount = a.Amount,
                                                ExpenseAmountCur0 = a.ExpenseAmountCur0,
                                                ExpenseAmount0 = a.ExpenseAmount0,
                                                ExpenseAmount1 = a.ExpenseAmount1,
                                                ExpenseAmountCur1 = a.ExpenseAmountCur1,
                                                ExpenseAmountCur = a.ExpenseAmountCur,
                                                ExpenseAmount = a.ExpenseAmount,
                                                VatPercentage = a.VatPercentage,
                                                VatAmountCur = a.VatAmountCur,
                                                VatAmount = a.VatAmount,
                                                DiscountPercentage = a.DiscountPercentage,
                                                DiscountAmount = a.DiscountAmount,
                                                DiscountAmountCur = a.DiscountAmountCur,
                                                ImportTaxPercentage = a.ImportTaxPercentage,
                                                ImportTaxAmount = a.ImportTaxAmount,
                                                ImportTaxAmountCur = a.ImportTaxAmountCur,
                                                ExciseTaxPercentage = a.ExciseTaxPercentage,
                                                ExciseAmount = a.ExciseAmount,
                                                ExciseAmountCur = a.ExciseAmountCur,
                                                DebitAcc0 = a.DebitAcc,
                                                DebitAcc = a.DebitAcc,
                                                CreditAcc0 = a.CreditAcc,
                                                CreditAcc = a.CreditAcc,
                                                PriceCur2 = a.PriceCur2,
                                                Price2 = a.Price2,
                                                AmountCur2 = a.AmountCur2,
                                                Amount2 = a.Amount2,
                                                DebitAcc2 = a.DebitAcc2,
                                                CreditAcc2 = a.CreditAcc2,
                                                Note = a.Note,
                                                NoteE = a.NoteE,
                                                SectionCode = a.SectionCode,
                                                WorkPlaceCode = a.WorkPlaceCode,
                                                CaseCode = a.CaseCode,
                                                FixedPrice = a.FixedPrice,
                                                VatPrice = a.VatPrice,
                                                DecreasePercentage = a.DecreasePercentage,
                                                DevaluationPrice = a.DevaluationPrice,
                                                DevaluationPriceCur = a.DevaluationPriceCur,
                                                DevaluationAmount = a.DevaluationAmount,
                                                DevaluationAmountCur = a.DevaluationAmountCur,
                                                VarianceAmount = a.VarianceAmount,
                                                ProductType = a.ProductType,
                                                //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                                WarehouseAcc = a.WarehouseAcc,
                                                ProductAcc = a.ProductAcc,
                                                IsOriginVoucher = a.IsOriginVoucher,//ctgoc
                                                IsLedger = a.IsLedger,
                                                IsLedger2 = a.IsLedger2,
                                                TrxQuantity = a.Quantity,
                                                TrxPriceCru = a.PriceCur,
                                                TrxPrice = a.Price,
                                                TrxPriceCru2 = a.PriceCur2,
                                                TrxPrice2 = a.Price2,
                                                QuantityQd = a.QuantityQd,
                                                PriceCur0 = (a.ImportAmountCru + a.ExportAmountCru) > 0 ? (a.ImportAmountCru + a.ExportAmountCru) / (a.QuantityTrxN + a.QuantityTrxX) : 0,
                                                Price0 = (a.ImportAmount + a.ExportAmount) > 0 ? (a.ImportAmount + a.ExportAmount) / (a.ImportQuantity + a.ExportQuantity) : 0,
                                                AccImport = a.AccExport,//tknhap
                                                QuantityTrxN = a.QuantityTrxN,
                                                ImportQuantity = a.ImportQuantity,
                                                ImportAmountCru = a.ImportAmountCru,
                                                ImportAmount = a.ImportAmount,
                                                AccExport = a.AccExport,
                                                QuantityTrxX = a.QuantityTrxX,
                                                ExportQuantity = a.ExportQuantity,
                                                ExportAmountCru = a.ExportAmountCru,
                                                ExportAmount = a.ExportAmount,
                                                AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                                AssemblyProductCode = a.AssemblyProductCode,
                                                AssemblyProductLotCode = a.AssemblyProductLotCode,
                                                AssemblyUnitCode = a.AssemblyUnitCode,
                                                AssemblyQuantity = a.AssemblyQuantity,
                                                AssemblyPriceCur = a.AssemblyPriceCur,
                                                AssemblyPrice = a.AssemblyPrice,
                                                AssemblyAmountCru = a.AssemblyAmountCru,
                                                AssemblyAmount = a.AssemblyAmount,
                                                VatPriceCur = a.VatPriceCur,
                                                DevaluationPercentage = a.DevaluationPercentage,
                                            };


            resulProductVoucherDetal2 = from a in resulProductVoucherDetal2
                                        join b in resulProductVoucherDetal3 on a.Id equals b.Id into c
                                        from d in c.DefaultIfEmpty()
                                        select new
                                        {
                                            VoucherGroup0 = a.VoucherGroup,
                                            VoucherGroup = a.VoucherGroup,
                                            BusinessAcc = a.BusinessAcc,
                                            DevaluationCreditAcc = a.DevaluationCreditAcc,
                                            DevaluationDebitAcc = a.DevaluationDebitAcc,
                                            DiscountCreditAcc = a.DiscountCreditAcc,
                                            DiscountDebitAcc = a.DiscountDebitAcc,
                                            Id = a.Id,
                                            ProductVoucherId = a.ProductVoucherId,
                                            Rec0 = a.Ord0,
                                            Ord0 = a.Ord0,
                                            ContractCode = a.ContractCode,
                                            ProductCode = a.ProductCode,
                                            TransProductCode = a.TransProductCode,
                                            TransWarehouseCode = a.TransWarehouseCode,
                                            WarehouseCode = a.WarehouseCode,
                                            ProductLotCode = a.ProductLotCode,
                                            TransProductLotCode = a.TransProductLotCode,
                                            ProductOriginCode = a.ProductOriginCode,
                                            TransProductOriginCode = a.TransProductOriginCode,
                                            PartnerCode = a.PartnerCode,
                                            TaxCategoryCode = a.TaxCategoryCode,
                                            FProductWorkCode = a.FProductWorkCode,
                                            UnitCode = a.UnitCode,
                                            TransUnitCode = a.TransUnitCode,
                                            ProductName0 = a.ProductName0,
                                            Quantity = a.Quantity,
                                            Price = a.Price,
                                            PriceCur = a.PriceCur,
                                            AmountCur = a.AmountCur,
                                            Amount = a.Amount,
                                            ExpenseAmountCur0 = a.ExpenseAmountCur0,
                                            ExpenseAmount0 = a.ExpenseAmount0,
                                            ExpenseAmount1 = a.ExpenseAmount1,
                                            ExpenseAmountCur1 = a.ExpenseAmountCur1,
                                            ExpenseAmountCur = a.ExpenseAmountCur,
                                            ExpenseAmount = a.ExpenseAmount,
                                            VatPercentage = a.VatPercentage,
                                            VatAmountCur = a.VatAmountCur,
                                            VatAmount = a.VatAmount,
                                            DiscountPercentage = a.DiscountPercentage,
                                            DiscountAmount = a.DiscountAmount,
                                            DiscountAmountCur = a.DiscountAmountCur,
                                            ImportTaxPercentage = a.ImportTaxPercentage,
                                            ImportTaxAmount = a.ImportTaxAmount,
                                            ImportTaxAmountCur = a.ImportTaxAmountCur,
                                            ExciseTaxPercentage = a.ExciseTaxPercentage,
                                            ExciseAmount = a.ExciseAmount,
                                            ExciseAmountCur = a.ExciseAmountCur,
                                            DebitAcc0 = a.DebitAcc,
                                            DebitAcc = a.DebitAcc,
                                            CreditAcc0 = a.CreditAcc,
                                            CreditAcc = a.CreditAcc,
                                            PriceCur2 = a.PriceCur2,
                                            Price2 = a.Price2,
                                            AmountCur2 = a.AmountCur2,
                                            Amount2 = a.Amount2,
                                            DebitAcc2 = a.DebitAcc2,
                                            CreditAcc2 = a.CreditAcc2,
                                            Note = a.Note,
                                            NoteE = a.NoteE,
                                            SectionCode = a.SectionCode,
                                            WorkPlaceCode = a.WorkPlaceCode,
                                            CaseCode = a.CaseCode,
                                            FixedPrice = a.FixedPrice,
                                            VatPrice = a.VatPrice,
                                            DecreasePercentage = a.DecreasePercentage,
                                            DevaluationPrice = a.DevaluationPrice,
                                            DevaluationPriceCur = a.DevaluationPriceCur,
                                            DevaluationAmount = a.DevaluationAmount,
                                            DevaluationAmountCur = a.DevaluationAmountCur,
                                            VarianceAmount = a.VarianceAmount,
                                            ProductType = a.ProductType,
                                            //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                            WarehouseAcc = a.WarehouseAcc,
                                            ProductAcc = a.ProductAcc,
                                            IsOriginVoucher = a.IsOriginVoucher,//ctgoc
                                            IsLedger = a.IsLedger,
                                            IsLedger2 = a.IsLedger2,
                                            TrxQuantity = a.Quantity,
                                            TrxPriceCru = a.PriceCur,
                                            TrxPrice = a.Price,
                                            TrxPriceCru2 = a.PriceCur2,
                                            TrxPrice2 = a.Price2,
                                            QuantityQd = a.QuantityQd,
                                            PriceCur0 = d != null ? d.PriceCur0 : a.PriceCur0,
                                            Price0 = d != null ? d.Price0 : a.Price0,
                                            AccImport = a.AccImport,//tknhap
                                            QuantityTrxN = a.QuantityTrxN,
                                            ImportQuantity = a.ImportQuantity,
                                            ImportAmountCru = a.ImportAmountCru,
                                            ImportAmount = a.ImportAmount,
                                            AccExport = a.AccExport,
                                            QuantityTrxX = a.QuantityTrxX,
                                            ExportQuantity = a.ExportQuantity,
                                            ExportAmountCru = a.ExportAmountCru,
                                            ExportAmount = a.ExportAmount,
                                            AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                            AssemblyProductCode = a.AssemblyProductCode,
                                            AssemblyProductLotCode = a.AssemblyProductLotCode,
                                            AssemblyUnitCode = a.AssemblyUnitCode,
                                            AssemblyQuantity = a.AssemblyQuantity,
                                            AssemblyPriceCur = a.AssemblyPriceCur,
                                            AssemblyPrice = a.AssemblyPrice,
                                            AssemblyAmountCru = a.AssemblyAmountCru,
                                            AssemblyAmount = a.AssemblyAmount,
                                            VatPriceCur = a.VatPriceCur,
                                            DevaluationPercentage = a.DevaluationPercentage,
                                        };
            var groupProductVoucherDetail = from a in resulProductVoucherDetal2
                                            group new
                                            {
                                                a.ProductVoucherId,
                                                a.ContractCode,
                                                a.FProductWorkCode,
                                                a.SectionCode,
                                                a.WorkPlaceCode,
                                                a.CaseCode,
                                                a.AmountCur,
                                                a.Amount,
                                                a.DevaluationAmount,
                                                a.DevaluationAmountCur,
                                                a.DiscountAmount,
                                                a.DiscountAmountCur,
                                                a.ImportAmount,
                                                a.ImportAmountCru,
                                                a.ExciseAmount,
                                                a.ExciseAmountCur
                                            } by new
                                            {
                                                a.ProductVoucherId,

                                            }
                                            into gr
                                            select new
                                            {
                                                ProductVoucherId = gr.Key.ProductVoucherId,
                                                ContractCode = gr.Max(p => p.ContractCode),
                                                FProductWorkCode = gr.Max(p => p.FProductWorkCode),
                                                SectionCode = gr.Max(p => p.SectionCode),
                                                WorkPlaceCode = gr.Max(p => p.WorkPlaceCode),
                                                CaseCode = gr.Max(p => p.CaseCode),
                                                AmountCur = gr.Sum(p => p.AmountCur),
                                                Amount = gr.Sum(p => p.Amount),
                                                DevaluationAmount = gr.Sum(p => p.DevaluationAmount),
                                                DevaluationAmountCur = gr.Sum(p => p.DevaluationAmountCur),
                                                DiscountAmount = gr.Sum(p => p.DiscountAmount),
                                                DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                                                ImportAmount = gr.Sum(p => p.ImportAmount),
                                                ImportAmountCru = gr.Sum(p => p.ImportAmountCru),
                                                ExciseAmount = gr.Sum(p => p.ExciseAmount),
                                                ExciseAmountCur = gr.Sum(p => p.ExciseAmountCur)
                                            };


            var updateProductVoucher = from a in productVoucher
                                       join d in groupProductVoucherDetail on a.Id equals d.ProductVoucherId into c
                                       from b in c.DefaultIfEmpty()
                                       select new
                                       {
                                           id = a.Id,
                                           ContractCode = b.ContractCode,
                                           FProductWorkCode = b.FProductWorkCode,
                                           SectionCode = b.SectionCode,
                                           WorkPlaceCode = b.WorkPlaceCode,
                                           CaseCode = b.CaseCode,
                                           DevaluationAmountCur = b.DevaluationAmountCur,
                                           DevaluationAmount = b.DevaluationAmount,
                                           DiscountAmountCru = b.DiscountAmountCur,
                                           DiscountAmount = b.DiscountAmount,
                                           ImportAmountCru = b.ImportAmountCru,
                                           ImportAmount = b.ImportAmount,
                                           ExciseTaxAmountCru = b.ExciseAmountCur,
                                           ExciseTaxAmount = b.ExciseAmount,
                                           AssemblyAmountCru = b.AmountCur,
                                           AssemblyAmount = b.Amount,
                                           AssemblyPriceCur = a.AssemblyQuantity != 0 ? b.AmountCur / a.AssemblyQuantity : 0,
                                           AssemblyPrice = a.AssemblyQuantity != 0 ? b.Amount / a.AssemblyQuantity : 0,

                                       };
            var resulProductVoucher = from a in productVoucher
                                      join b in updateProductVoucher on a.Id equals b.id into c
                                      from d in c.DefaultIfEmpty()
                                      select new
                                      {
                                          Id = a.Id,
                                          OrgCode = a.OrgCode,
                                          Year = a.Year,
                                          ContractCode = d != null ? d.ContractCode : a.ContractCode,
                                          DepartmentCode = a.DepartmentCode,
                                          VoucherCode = a.VoucherCode,
                                          VoucherGroup0 = a.VoucherGroup,
                                          VoucherGroup = a.VoucherGroup,
                                          BusinessCode = a.BusinessCode,
                                          BusinessAcc = a.BusinessAcc,
                                          VoucherNumber = a.VoucherNumber,
                                          InvoiceNumber = a.InvoiceNumber,
                                          VoucherDate = a.VoucherDate,
                                          PaymentTermsCode = a.PaymentTermsCode,
                                          FProductWorkCode = d != null ? d.FProductWorkCode : a.FProductWorkCode,
                                          CurrencyCode = a.CurrencyCode,
                                          ExchangeRate = a.ExchangeRate,
                                          PartnerCode0 = a.PartnerCode0,
                                          Representative = a.Representative,
                                          Address = a.Address,
                                          OriginVoucher = a.OriginVoucher,
                                          Description = a.Description,
                                          DescriptionE = a.DescriptionE,
                                          Status = a.Status,
                                          Place = a.Place,
                                          DevaluationDebitAcc = a.DevaluationDebitAcc,// tk giảm giá
                                          DevaluationCreditAcc = a.DevaluationCreditAcc,
                                          DevaluationAmount = d != null ? d.DevaluationAmount : a.DevaluationAmount,
                                          DevaluationAmountCur = d != null ? d.DevaluationAmountCur : a.DevaluationAmountCur,
                                          DiscountPercentage = a.DiscountPercentage,
                                          DiscountCreditAcc = a.DiscountCreditAcc,
                                          DiscountDebitAcc = a.DiscountDebitAcc,
                                          DiscountDescription = a.DiscountDescription,
                                          DiscountAmountCru = d != null ? d.DiscountAmountCru : a.DiscountAmountCru,
                                          DiscountAmount = d != null ? d.DiscountAmount : a.DiscountAmount,
                                          DiscountCreditAcc0 = a.DiscountCreditAcc0,
                                          DiscountDebitAcc0 = a.DiscountDebitAcc0,
                                          DiscountDescription0 = a.DiscountDescription0,
                                          DiscountAmountCru0 = a.DiscountAmountCru0,
                                          DiscountAmount0 = a.DiscountAmount0,
                                          ImportTaxPercentage = a.ImportTaxPercentage,
                                          ImportCreditAcc = a.ImportCreditAcc,
                                          ImportDebitAcc = a.ImportDebitAcc,
                                          ImportDescription = a.ImportDescription,
                                          ImportAmountCru = d != null ? d.ImportAmountCru : a.ImportAmountCru,
                                          ImportAmount = d != null ? d.ImportAmount : a.ImportAmount,
                                          ExciseTaxPercentage = a.ExciseTaxPercentage,
                                          ExciseTaxDebitAcc = a.ExciseTaxDebitAcc,
                                          ExciseTaxCreditAcc = a.ExciseTaxCreditAcc,
                                          ExciseTaxDescription = a.ExciseTaxDescription,
                                          ExciseTaxAmountCru = d != null ? d.ExciseTaxAmountCru : a.ExciseTaxAmountCru,
                                          ExciseTaxAmount = d != null ? d.ExciseTaxAmount : a.ExciseTaxAmount,
                                          TaxCategoryCode = a.TaxCategoryCode,
                                          TaxCode = a.TaxCode,
                                          InvoiceDate = a.InvoiceDate,
                                          InvoiceSymbol = a.InvoiceSymbol,
                                          InvoiceNbr = a.InvoiceNbr,
                                          VatPercentage = a.VatPercentage,
                                          ClearingPartnerCode = a.ClearingPartnerCode,
                                          AddressInv = a.AddressInv,
                                          PaymentTermsCodes = a.PaymentTermsCode,
                                          SalesChannelCode = a.SalesChannelCode,
                                          PartnerCode = a.PartnerCode0,
                                          WorkPlaceCode = d != null ? d.WorkPlaceCode : a.WorkPlaceCode,
                                          SectionCode = d != null ? d.SectionCode : a.SectionCode,
                                          CaseCode = d != null ? d.CaseCode : a.CaseCode,
                                          AssemblyWarehouseCode = a.AssemblyWarehouseCode,
                                          AssemblyProductCode = a.AssemblyProductCode,
                                          AssemblyProductLotCode = a.AssemblyProductLotCode,
                                          AssemblyWorkPlaceCode = a.AssemblyWorkPlaceCode,
                                          AssemblyUnitCode = a.AssemblyUnitCode,
                                          AssemblyPrice = d != null ? d.AssemblyPrice : a.AssemblyPrice,
                                          AssemblyQuantity = a.AssemblyQuantity,
                                          AssemblyPriceCur = d != null ? d.AssemblyPriceCur : a.AssemblyPriceCur,
                                          AssemblyAmountCru = d != null ? d.AssemblyAmountCru : a.AssemblyAmountCru,
                                          AssemblyAmount = d != null ? d.AssemblyAmount : a.AssemblyAmount,
                                          BillNumber = a.BillNumber,
                                      };
            var resulAll = (from a in resulProductVoucher
                            join b in resulProductVoucherDetal2 on a.Id equals b.ProductVoucherId into c
                            from d in c.DefaultIfEmpty()
                            select new
                            {
                                Id = a.Id,
                                Ord0 = d.Ord0,
                                OrgCode = a.OrgCode,
                                VoucherCode = a.VoucherCode,
                                Year = a.Year,
                                DepartmentCode = a.DepartmentCode,
                                VoucherGroup = d.VoucherGroup,
                                BusinessCode = a.BusinessCode,
                                BusinessAcc = a.BusinessAcc,
                                VoucherNumber = a.VoucherNumber,
                                VoucherDate = a.VoucherDate,
                                PaymentTermsCode = a.PaymentTermsCode,
                                ContractCode = a.ContractCode,
                                CurrencyCode = a.CurrencyCode,
                                ExchangeRate = a.ExchangeRate,
                                PartnerCode0 = a.PartnerCode0,
                                Representative = a.Representative,
                                Address = a.Address,
                                Place = a.Place,
                                OriginVoucher = a.OriginVoucher,
                                Description = a.Description,
                                DescriptionE = a.DescriptionE,
                                Status = a.Status,
                                ProductCode = d.ProductCode,
                                TransProductCode = d.TransProductCode,
                                UnitCode = d.UnitCode,
                                TransUnitCode = d.TransUnitCode,
                                WarehouseCode = d.WarehouseCode,
                                TransWarehouseCode = d.TransWarehouseCode,
                                ProductLotCode = d.ProductLotCode,
                                TransProductLotCode = d.TransProductLotCode,
                                ProductOriginCode = d.ProductOriginCode,
                                TransProductOriginCode = d.TransProductOriginCode,
                                PartnerCode = d.PartnerCode,
                                FProductWorkCode = d.FProductWorkCode,
                                WorkPlaceCode = d.WorkPlaceCode,
                                SectionCode = d.SectionCode,
                                CaseCode = d.CaseCode,
                                TrxQuantity = d.TrxQuantity,
                                TrxPriceCru = d.TrxPriceCru,
                                TrxPrice = d.TrxPrice,
                                Quantity = d.Quantity,
                                PriceCur = d.PriceCur,
                                Price = d.Price,
                                Amount = d.Amount,
                                AmountCur = d.AmountCur,
                                ExciseAmountCur0 = d.ExpenseAmountCur0, //cpExpenseAmountCur0
                                ExpenseAmount0 = d.ExpenseAmount0,
                                ExpenseAmountCur1 = d.ExpenseAmountCur1,
                                ExpenseAmount1 = d.ExpenseAmount1,
                                ExpenseAmountCur = d.ExpenseAmountCur,
                                ExpenseAmount = d.ExpenseAmount,
                                VatPercentage = d.VatPercentage,
                                VatAmountCur = d.VatAmountCur,
                                VatAmount = d.VatAmount,
                                DiscountPercentage = d.DiscountPercentage,
                                DiscountAmountCur = d.DiscountAmountCur,
                                DiscountAmount = d.DiscountAmount,
                                ImportTaxPercentage = d.ImportTaxPercentage,
                                ImportTaxAmountCur = d.ImportTaxAmountCur,
                                ImportTaxAmount = d.ImportTaxAmount,
                                ExciseTaxPercentage = d.ExciseTaxPercentage,//db
                                ExciseAmountCur = d.ExciseAmountCur,
                                ExciseAmount = d.ExciseAmount,
                                DebitAcc = d.DebitAcc,
                                CreditAcc = d.CreditAcc,
                                TrxPrice2 = d.TrxPrice2,
                                TrxPriceCru2 = d.TrxPriceCru2,
                                PriceCur2 = d.PriceCur2,
                                Price2 = d.Price2,
                                AmountCur2 = d.AmountCur2,
                                Amount2 = d.Amount2,
                                DebitAcc2 = d.DebitAcc2,
                                CreditAcc2 = d.CreditAcc2,
                                Note = d.Note,
                                NoteE = d.NoteE,
                                PriceCur0 = d.PriceCur0,
                                Price0 = d.Price0,
                                AccImport = d.AccImport,
                                QuantityTrxN = d.QuantityTrxN,
                                ImportQuantity = d.ImportQuantity,
                                ImportAmountCru = d.ImportAmountCru,
                                ImportAmount = d.ImportAmount,
                                AccExport = d.AccExport,
                                QuantityTrxX = d.QuantityTrxX,
                                ExportAmountCru = d.ExportAmountCru,
                                ExportAmount = d.ExportAmount,
                                FixedPrice = d.FixedPrice,
                                TaxCategoryCode = a.TaxCategoryCode,
                                InvoiceSymbol = a.InvoiceSymbol,
                                InvoiceNumber = a.InvoiceNumber,
                                ClearingPartnerCode = a.ClearingPartnerCode,
                                AddressInv = a.AddressInv,
                                TaxCode = a.TaxCode,
                                InvoiceDate = a.InvoiceDate,
                                SalesChannelCode = a.SalesChannelCode,
                                BillNumber = a.BillNumber,
                                VatPrice = d.VatPrice,
                                VatPriceCur = d.VatPriceCur,
                                DevaluationPercentage = d.DevaluationPercentage,
                                DevaluationAmountCur = d.DevaluationAmountCur,
                                DevaluationAmount = d.DevaluationAmount,
                                DevaluationPriceCur = d.DevaluationPriceCur,
                                DevaluationPrice = d.DevaluationPrice,
                                VarianceAmount = d.VarianceAmount,
                                ProductName0 = d.ProductName0,
                                TolAmount2 = d.Amount2,
                                TolDiscountAmount = d.DiscountAmount
                            }).ToList();
            // Chèn dữ liệu vào SOKHOca
            for (int i = 0; i < resulAll.Count; i++)
            {
                CrudWarehouseBookDto crudWarehouseBookDto = new CrudWarehouseBookDto();
                crudWarehouseBookDto.ProductVoucherId = resulAll[i].Id;
                crudWarehouseBookDto.Ord0 = vLGLR == 1 ? resulAll[i].Ord0 : "B" + resulAll[i].Ord0.Substring(1, resulAll[i].Ord0.Length - 1).ToString();
                crudWarehouseBookDto.OrgCode = resulAll[i].OrgCode;
                crudWarehouseBookDto.Ord0Extra = vLGLR == 1 ? resulAll[i].Ord0 : "B" + resulAll[i].Ord0.Substring(1, resulAll[i].Ord0.Length - 1).ToString();
                crudWarehouseBookDto.Year = resulAll[i].Year;
                crudWarehouseBookDto.DepartmentCode = resulAll[i].DepartmentCode;
                crudWarehouseBookDto.VoucherGroup = resulAll[i].VoucherGroup;
                crudWarehouseBookDto.BusinessCode = resulAll[i].BusinessCode;
                crudWarehouseBookDto.VoucherCode = resulAll[i].VoucherCode;
                crudWarehouseBookDto.BusinessAcc = resulAll[i].BusinessAcc;
                crudWarehouseBookDto.VoucherNumber = resulAll[i].VoucherNumber;
                crudWarehouseBookDto.VoucherDate = resulAll[i].VoucherDate;
                crudWarehouseBookDto.PaymentTermsCode = resulAll[i].PaymentTermsCode;
                crudWarehouseBookDto.ContractCode = resulAll[i].ContractCode;
                crudWarehouseBookDto.CurrencyCode = resulAll[i].CurrencyCode;
                crudWarehouseBookDto.ExchangeRate = resulAll[i].ExchangeRate;
                crudWarehouseBookDto.PartnerCode0 = resulAll[i].PartnerCode0;
                crudWarehouseBookDto.Representative = resulAll[i].Representative;
                crudWarehouseBookDto.Address = resulAll[i].Address;
                crudWarehouseBookDto.Place = resulAll[i].Place;
                crudWarehouseBookDto.OriginVoucher = resulAll[i].OriginVoucher;
                crudWarehouseBookDto.Description = resulAll[i].Description;
                crudWarehouseBookDto.DescriptionE = resulAll[i].DescriptionE;
                crudWarehouseBookDto.Status = resulAll[i].Status;
                crudWarehouseBookDto.ProductCode = resulAll[i].ProductCode;
                crudWarehouseBookDto.TransProductCode = resulAll[i].TransProductCode;
                crudWarehouseBookDto.UnitCode = resulAll[i].UnitCode;
                crudWarehouseBookDto.TransferingUnitCode = resulAll[i].TransUnitCode;
                crudWarehouseBookDto.WarehouseCode = resulAll[i].WarehouseCode;
                crudWarehouseBookDto.TransWarehouseCode = resulAll[i].TransWarehouseCode;
                crudWarehouseBookDto.ProductLotCode = resulAll[i].ProductLotCode;
                crudWarehouseBookDto.TransProductLotCode = resulAll[i].TransProductLotCode;
                crudWarehouseBookDto.ProductOriginCode = resulAll[i].ProductOriginCode;
                crudWarehouseBookDto.TransProductOriginCode = resulAll[i].TransProductOriginCode;
                crudWarehouseBookDto.PartnerCode = resulAll[i].PartnerCode;
                crudWarehouseBookDto.FProductWorkCode = resulAll[i].FProductWorkCode;
                crudWarehouseBookDto.WorkPlaceCode = resulAll[i].WorkPlaceCode;
                crudWarehouseBookDto.SectionCode = resulAll[i].SectionCode;
                crudWarehouseBookDto.CaseCode = resulAll[i].CaseCode;
                crudWarehouseBookDto.TrxQuantity = resulAll[i].TrxQuantity;
                crudWarehouseBookDto.TrxPriceCur = resulAll[i].TrxPriceCru;
                crudWarehouseBookDto.TrxPrice = resulAll[i].TrxPrice;
                crudWarehouseBookDto.Quantity = resulAll[i].Quantity;
                crudWarehouseBookDto.PriceCur = resulAll[i].PriceCur;
                crudWarehouseBookDto.Price = resulAll[i].Price;
                crudWarehouseBookDto.Amount = resulAll[i].Amount;
                crudWarehouseBookDto.AmountCur = resulAll[i].AmountCur;
                crudWarehouseBookDto.ExpenseAmountCur0 = resulAll[i].ExciseAmountCur0; //cp
                crudWarehouseBookDto.ExpenseAmount0 = resulAll[i].ExpenseAmount0;
                crudWarehouseBookDto.ExpenseAmountCur1 = resulAll[i].ExpenseAmountCur1;
                crudWarehouseBookDto.ExpenseAmount1 = resulAll[i].ExpenseAmount1;
                //crudWarehouseBookDto.expen = resulAll[i].ExpenseAmountCur;
                crudWarehouseBookDto.ExpenseAmount = resulAll[i].ExpenseAmount;
                crudWarehouseBookDto.VatPercentage = resulAll[i].VatPercentage;
                crudWarehouseBookDto.VatAmountCur = resulAll[i].VatAmountCur;
                crudWarehouseBookDto.VatAmount = resulAll[i].VatAmount;
                crudWarehouseBookDto.DiscountPercentage = resulAll[i].DiscountPercentage;
                crudWarehouseBookDto.DiscountAmountCur = resulAll[i].DiscountAmountCur;
                crudWarehouseBookDto.DiscountAmount = resulAll[i].DiscountAmount;
                crudWarehouseBookDto.ImportTaxPercentage = resulAll[i].ImportTaxPercentage;
                crudWarehouseBookDto.ImportTaxAmountCur = resulAll[i].ImportTaxAmountCur;
                crudWarehouseBookDto.ImportTaxAmount = resulAll[i].ImportTaxAmount;
                crudWarehouseBookDto.ExciseTaxPercentage = resulAll[i].ExciseTaxPercentage;//db
                crudWarehouseBookDto.ExciseTaxAmountCur = resulAll[i].ExciseAmountCur;
                crudWarehouseBookDto.ExciseTaxAmount = resulAll[i].ExciseAmount;
                crudWarehouseBookDto.DebitAcc = resulAll[i].DebitAcc;
                crudWarehouseBookDto.CreditAcc = resulAll[i].CreditAcc;
                crudWarehouseBookDto.TrxPrice2 = resulAll[i].TrxPrice2;
                crudWarehouseBookDto.TrxPriceCur2 = resulAll[i].TrxPriceCru2;
                crudWarehouseBookDto.PriceCur2 = resulAll[i].PriceCur2;
                crudWarehouseBookDto.Price2 = resulAll[i].Price2;
                crudWarehouseBookDto.AmountCur2 = resulAll[i].AmountCur2;
                crudWarehouseBookDto.Amount2 = resulAll[i].Amount2;
                crudWarehouseBookDto.DebitAcc2 = resulAll[i].DebitAcc2;
                crudWarehouseBookDto.CreditAcc2 = resulAll[i].CreditAcc2;
                crudWarehouseBookDto.Note = resulAll[i].Note;
                crudWarehouseBookDto.NoteE = resulAll[i].NoteE;
                crudWarehouseBookDto.PriceCur0 = resulAll[i].PriceCur0;
                crudWarehouseBookDto.Price0 = resulAll[i].Price0;
                crudWarehouseBookDto.TrxImportQuantity = vLGLR != 1 ? resulAll[i].Quantity : 0;
                crudWarehouseBookDto.ImportQuantity = vLGLR != 1 ? resulAll[i].Quantity : 0;
                crudWarehouseBookDto.ImportAcc = vLGLR != 1 ? resulAll[i].AccImport : null;
                crudWarehouseBookDto.ImportAmountCur = vLGLR != 1 ? resulAll[i].AmountCur : 0;
                crudWarehouseBookDto.ImportAmount = vLGLR != 1 ? resulAll[i].Amount : 0;
                crudWarehouseBookDto.ExportAcc = vLGLR == 1 ? resulAll[i].AccExport : null;
                crudWarehouseBookDto.ExportQuantity = vLGLR == 1 ? resulAll[i].QuantityTrxX : 0;
                crudWarehouseBookDto.ExportAmountCur = vLGLR == 1 ? resulAll[i].ExportAmountCru : 0;
                crudWarehouseBookDto.ExportAmount = vLGLR == 1 ? resulAll[i].ExportAmount : 0;
                crudWarehouseBookDto.TrxExportQuantity = vLGLR == 1 ? resulAll[i].QuantityTrxX : 0;
                crudWarehouseBookDto.FixedPrice = resulAll[i].FixedPrice;
                crudWarehouseBookDto.VatPercentage = resulAll[i].VatPercentage;
                crudWarehouseBookDto.InvoiceSymbol = resulAll[i].InvoiceSymbol;
                crudWarehouseBookDto.InvoiceNumber = resulAll[i].InvoiceNumber;
                //crudWarehouseBookDto.PartnerCode = resulAll[i].ClearingPartnerCode;
                crudWarehouseBookDto.InvoicePartnerAddress = resulAll[i].AddressInv;
                crudWarehouseBookDto.TaxCode = resulAll[i].TaxCode;
                crudWarehouseBookDto.InvoiceDate = resulAll[i].InvoiceDate;
                crudWarehouseBookDto.SalesChannelCode = resulAll[i].SalesChannelCode;
                crudWarehouseBookDto.BillNumber = resulAll[i].BillNumber;
                crudWarehouseBookDto.VatPrice = resulAll[i].VatPrice;
                crudWarehouseBookDto.VatPriceCur = resulAll[i].VatPriceCur;
                crudWarehouseBookDto.DevaluationPercentage = resulAll[i].DevaluationPercentage;
                crudWarehouseBookDto.DevaluationAmountCur = resulAll[i].DevaluationAmountCur;
                crudWarehouseBookDto.DevaluationAmount = resulAll[i].DevaluationAmount;
                crudWarehouseBookDto.DevaluationPriceCur = resulAll[i].DevaluationPriceCur;
                crudWarehouseBookDto.DevaluationPrice = resulAll[i].DevaluationPrice;
                crudWarehouseBookDto.VarianceAmount = resulAll[i].VarianceAmount;
                crudWarehouseBookDto.ProductName0 = resulAll[i].ProductName0;
                crudWarehouseBookDto.TotalAmount2 = resulAll[i].TolAmount2;
                crudWarehouseBookDto.TotalDiscountAmount = resulAll[i].TolDiscountAmount;
                if (dto.CreationTime != null)
                {
                    crudWarehouseBookDto.CreationTime = dto.CreationTime;
                }
                else
                {
                    crudWarehouseBookDto.CreationTime = DateTime.Now;
                }
                crudWarehouseBookDtos.Add(crudWarehouseBookDto);
            }

            //Ghi đầu phiếu vào sổ kho
            var detailProductVoucher = (from a in resulProductVoucherDetal2
                                        group new
                                        {
                                            a.ProductVoucherId,
                                            a.FixedPrice,
                                            a.DebitAcc,
                                            a.CreditAcc,
                                            a.Note,
                                            a.NoteE,
                                            a.ExpenseAmount0,
                                            a.ExpenseAmountCur0,
                                            a.ExpenseAmountCur1,
                                            a.ExpenseAmount1,
                                            a.ExpenseAmountCur,
                                            a.ExpenseAmount,
                                            a.VatPercentage,
                                            a.VatAmountCur,
                                            a.VatAmount,
                                            a.DiscountAmount,
                                            a.DiscountAmountCur,
                                            a.DiscountPercentage,
                                            a.ImportTaxPercentage,
                                            a.ImportTaxAmountCur,
                                            a.ImportTaxAmount,
                                            a.ExciseTaxPercentage,
                                            a.ExciseAmountCur,
                                            a.ExciseAmount,
                                            a.PriceCur2,
                                            a.Price2,
                                            a.AmountCur2,
                                            a.Amount2,
                                            a.DebitAcc2,
                                            a.CreditAcc2,
                                            a.AmountCur,
                                            a.Amount,
                                            a.ImportAmount,
                                            a.VarianceAmount,
                                            a.AssemblyPriceCur,
                                            a.AssemblyPrice

                                        }
                                        by new
                                        {
                                            a.ProductVoucherId,
                                            a.FixedPrice
                                        } into gr
                                        select new
                                        {
                                            DebitAcc = gr.Max(p => p.DebitAcc),
                                            CreditAcc = gr.Max(p => p.CreditAcc),
                                            Note = gr.Max(p => p.Note),
                                            NoteE = gr.Max(p => p.NoteE),
                                            ExpenseAmount0 = gr.Sum(p => p.ExpenseAmount0),
                                            ExpenseAmountCur0 = gr.Sum(p => p.ExpenseAmountCur0),
                                            ExpenseAmountCur1 = gr.Sum(p => p.ExpenseAmountCur1),
                                            ExpenseAmount1 = gr.Sum(p => p.ExpenseAmount1),
                                            ExpenseAmountCur = gr.Sum(p => p.ExpenseAmountCur),
                                            ExpenseAmount = gr.Sum(p => p.ExpenseAmount),
                                            VatPercentage = gr.Max(p => p.VatPercentage),
                                            VatAmountCur = gr.Sum(p => p.VatAmountCur),
                                            VatAmount = gr.Sum(p => p.VatAmount),
                                            DiscountAmount = gr.Sum(p => p.DiscountAmount),
                                            DiscountAmountCur = gr.Sum(p => p.DiscountAmountCur),
                                            DiscountPercentage = gr.Max(p => p.DiscountPercentage),
                                            ImportTaxPercentage = gr.Max(p => p.ImportTaxPercentage),
                                            ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                            ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                            ExciseTaxPercentage = gr.Max(p => p.ExciseTaxPercentage),
                                            ExciseAmountCur = gr.Sum(p => p.ExciseAmountCur),
                                            ExciseAmount = gr.Sum(p => p.ExciseAmount),
                                            PriceCur2 = gr.Max(p => p.PriceCur2),
                                            Price2 = gr.Max(p => p.Price2),
                                            AmountCur2 = gr.Sum(p => p.AmountCur2),
                                            Amount2 = gr.Sum(p => p.Amount2),
                                            DebitAcc2 = gr.Max(p => p.DebitAcc2),
                                            CreditAcc2 = gr.Max(p => p.CreditAcc2),
                                            ImportAmountCru = gr.Sum(p => p.AmountCur + p.ImportTaxAmountCur + p.ExpenseAmountCur0 + p.ExpenseAmountCur1),
                                            ImportAmount = gr.Sum(p => p.Amount + p.ImportAmount + p.ExpenseAmount0 + p.ExpenseAmount1 + p.VarianceAmount),
                                            ProductVoucherId = gr.Key.ProductVoucherId,
                                            FixedPrice = gr.Key.FixedPrice,
                                            TotalAmount2 = gr.Max(p => p.Amount2),
                                            TotalDiscountAmount = gr.Max(p => p.DiscountAmount),
                                            AssemblyPriceCur = gr.Max(p => p.AssemblyPriceCur),
                                            AssemblyPrice = gr.Max(p => p.AssemblyPrice)
                                        }).ToList();
            var products = await _product.GetQueryableAsync();
            var lstProduct = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var accountSystems = await _accountSystemService.GetQueryableAsync();
            var accountSystem = accountSystems.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVouchers = from a in productVoucher.ToList()
                                  join c in accountSystem on new { a.OrgCode, a.Year, AccCode = a.ExciseTaxDebitAcc } equals new { c.OrgCode, c.Year, c.AccCode } into d
                                  from acc1 in d.DefaultIfEmpty()
                                  join e in accountSystem on new { a.OrgCode, a.Year, AccCode = a.ExciseTaxCreditAcc } equals new { e.OrgCode, e.Year, e.AccCode } into d2
                                  from acc2 in d2.DefaultIfEmpty()
                                  join q in lstProduct on new { a.OrgCode, Code = a.AssemblyProductCode } equals new { q.OrgCode, q.Code } into w
                                  from e in w.DefaultIfEmpty()
                                  join r in wareHoses on new { a.OrgCode, Code = a.AssemblyWarehouseCode } equals new { r.OrgCode, r.Code } into y
                                  from u in y.DefaultIfEmpty()
                                  join ps in productUnit1 on new { a.OrgCode, ProductCode = a.AssemblyProductCode } equals new { ps.OrgCode, ps.ProductCode } into pp
                                  from p1 in pp.DefaultIfEmpty()
                                  join pn in productUnit2 on new { a.OrgCode, ProductCode = a.AssemblyProductCode, UnitCode = a.AssemblyUnitCode } equals new { pn.OrgCode, pn.ProductCode, pn.UnitCode } into ph
                                  from p2 in ph.DefaultIfEmpty()
                                  join p in detailProductVoucher on a.Id equals p.ProductVoucherId into k
                                  from ct in k.DefaultIfEmpty()
                                  select new
                                  {
                                      a.Id,
                                      VoucherGroup = vLGLR == 1 ? 1 : 2,
                                      OrgCode = a.OrgCode,
                                      Ord0 = vLGLR == 1 ? "B000000001" : "A000000001",
                                      Note = ct.Note == null ? a.Note : ct.Note,
                                      DebitAcc = ct.DebitAcc,
                                      CreditAcc = ct.CreditAcc,
                                      NoteE = ct.NoteE == null ? a.NoteE : ct.NoteE,
                                      ImportCreditAcc = e.ProductType != "D" ? (vLGLR == 1 ? ct.DebitAcc : ct.CreditAcc) : null,
                                      TrxQuantity = p2.ExchangeRate != 0 ? a.AssemblyQuantity * p1.ExchangeRate / p2.ExchangeRate : 0,//slg_gd
                                      ExpenseAmountCur0 = ct.ExpenseAmountCur0,
                                      ExpenseAmount0 = ct.ExpenseAmount0,
                                      ExpenseAmountCur1 = ct.ExpenseAmountCur1,
                                      ExpenseAmount1 = ct.ExpenseAmount1,
                                      ExpenseAmountCur = a.ExpenseAmountCur,
                                      ExpenseAmount = a.ExpenseAmount,
                                      VatPercentage = ct.VatPercentage,
                                      VatAmountCur = ct.VatAmountCur,
                                      VatAmount = ct.VatAmount,
                                      DiscountPercentage = ct.DiscountPercentage,
                                      DiscountAmountCru = ct.DiscountAmountCur,
                                      DiscountAmount = ct.DiscountAmount,
                                      ImportTaxPercentage = ct.ImportTaxPercentage,
                                      ImportTaxAmountCur = ct.ImportTaxAmountCur,
                                      ImportTaxAmount = ct.ImportTaxAmount,
                                      ExciseTaxPercentage = ct.ExciseTaxPercentage,
                                      ExciseAmountCur = ct.ExciseAmountCur,
                                      ExciseAmount = ct.ExciseAmount,
                                      PriceCur2 = ct.PriceCur2,
                                      Price2 = ct.Price2,
                                      AmountCur2 = ct.AmountCur2,
                                      Amount2 = ct.Amount2,
                                      DebitAcc2 = ct.DebitAcc2,
                                      CreditAcc2 = ct.CreditAcc2,
                                      ImportAmountCru = ct.ImportAmountCru,
                                      ImportAmount = ct.ImportAmount,
                                      FixedPrice = ct.FixedPrice,
                                      TotalAmount2 = ct.TotalAmount2,
                                      TotalDiscountAmount = ct.TotalDiscountAmount,
                                      AssemblyPriceCur = ct.AssemblyPriceCur,
                                      AssemblyPrice = ct.AssemblyPrice

                                  };
            var resulAllProductVoucher = (from b in productVoucher
                                          join a in productVouchers on b.Id equals a.Id

                                          select new
                                          {

                                              b.Id,
                                              Ord0 = vLGLR == 1 ? "B000000001" : "A000000001",
                                              a.OrgCode,
                                              b.Year,
                                              b.DepartmentCode,
                                              b.VoucherCode,
                                              a.VoucherGroup,
                                              b.BusinessCode,
                                              b.BusinessAcc,
                                              b.VoucherNumber,
                                              b.VoucherDate,
                                              b.PaymentTermsCode,
                                              b.ContractCode,
                                              b.CurrencyCode,
                                              b.ExchangeRate,
                                              b.PartnerCode0,
                                              b.Representative,
                                              b.Address,
                                              b.Note,
                                              b.NoteE,
                                              b.Status,
                                              b.AssemblyProductCode,
                                              b.AssemblyUnitCode,
                                              b.AssemblyWarehouseCode,
                                              b.AssemblyProductLotCode,
                                              b.PartnerCode,
                                              b.FProductWorkCode,
                                              b.WorkPlaceCode,
                                              b.CaseCode,
                                              a.TrxQuantity,
                                              b.AssemblyPriceCur,
                                              b.AssemblyPrice,
                                              b.AssemblyQuantity,
                                              b.AssemblyAmountCru,
                                              b.AssemblyAmount,
                                              a.ExpenseAmountCur0,
                                              a.ExpenseAmount0,
                                              a.ExpenseAmountCur1,
                                              a.ExpenseAmount1,
                                              a.ExpenseAmountCur,
                                              a.ExpenseAmount,
                                              a.VatPercentage,
                                              a.VatAmountCur,
                                              a.VatAmount,
                                              a.DiscountPercentage,
                                              a.DiscountAmountCru,
                                              a.DiscountAmount,
                                              a.ImportTaxPercentage,
                                              a.ImportTaxAmountCur,
                                              a.ImportTaxAmount,
                                              a.ExciseTaxPercentage,
                                              a.ExciseAmountCur,
                                              a.ExciseAmount,
                                              a.DebitAcc,
                                              a.CreditAcc,
                                              a.Price2,
                                              a.Amount2,
                                              a.PriceCur2,
                                              a.AmountCur2,
                                              a.DebitAcc2,
                                              a.CreditAcc2,
                                              b.Description,
                                              b.DescriptionE,
                                              b.SectionCode,
                                              b.TaxCategoryCode,
                                              b.InvoiceSymbol,
                                              b.InvoiceNumber,
                                              b.ClearingPartnerCode,
                                              b.AddressInv,
                                              b.TaxCode,
                                              b.InvoiceDate,
                                              b.SalesChannelCode,
                                              b.BillNumber,
                                              a.TotalAmount2,
                                              a.TotalDiscountAmount,
                                              b.ImportCreditAcc,
                                              a.FixedPrice
                                          }).ToList();
            for (int i = 0; i < resulAllProductVoucher.Count; i++)
            {
                CrudWarehouseBookDto crudWarehouseBookDto = new CrudWarehouseBookDto();
                crudWarehouseBookDto.ProductVoucherId = resulAllProductVoucher[i].Id;
                crudWarehouseBookDto.Ord0 = resulAllProductVoucher[i].Ord0;
                crudWarehouseBookDto.OrgCode = resulAllProductVoucher[i].OrgCode;
                crudWarehouseBookDto.Ord0Extra = resulAllProductVoucher[i].Ord0;
                crudWarehouseBookDto.Year = resulAllProductVoucher[i].Year;
                crudWarehouseBookDto.DepartmentCode = resulAllProductVoucher[i].DepartmentCode;
                crudWarehouseBookDto.VoucherGroup = resulAllProductVoucher[i].VoucherGroup;
                crudWarehouseBookDto.VoucherCode = resulAllProductVoucher[i].VoucherCode;
                crudWarehouseBookDto.BusinessCode = resulAllProductVoucher[i].BusinessCode;
                crudWarehouseBookDto.BusinessAcc = resulAllProductVoucher[i].BusinessAcc;
                crudWarehouseBookDto.VoucherNumber = resulAllProductVoucher[i].VoucherNumber;
                crudWarehouseBookDto.VoucherDate = resulAllProductVoucher[i].VoucherDate;
                crudWarehouseBookDto.PaymentTermsCode = resulAllProductVoucher[i].PaymentTermsCode;
                crudWarehouseBookDto.ContractCode = resulAllProductVoucher[i].ContractCode;
                crudWarehouseBookDto.CurrencyCode = resulAllProductVoucher[i].CurrencyCode;
                crudWarehouseBookDto.ExchangeRate = resulAllProductVoucher[i].ExchangeRate;
                crudWarehouseBookDto.PartnerCode0 = resulAllProductVoucher[i].PartnerCode0;
                crudWarehouseBookDto.Representative = resulAllProductVoucher[i].Representative;
                crudWarehouseBookDto.Address = resulAllProductVoucher[i].Address;
                crudWarehouseBookDto.Description = resulAllProductVoucher[i].Description;
                crudWarehouseBookDto.DescriptionE = resulAllProductVoucher[i].DescriptionE;
                crudWarehouseBookDto.Status = resulAllProductVoucher[i].Status;
                crudWarehouseBookDto.ProductCode = resulAllProductVoucher[i].AssemblyProductCode;
                crudWarehouseBookDto.WarehouseCode = resulAllProductVoucher[i].AssemblyWarehouseCode;
                crudWarehouseBookDto.UnitCode = resulAllProductVoucher[i].AssemblyUnitCode;
                crudWarehouseBookDto.ProductLotCode = resulAllProductVoucher[i].AssemblyProductLotCode;
                crudWarehouseBookDto.PartnerCode = resulAllProductVoucher[i].PartnerCode;
                crudWarehouseBookDto.FProductWorkCode = resulAllProductVoucher[i].FProductWorkCode;
                crudWarehouseBookDto.WorkPlaceCode = resulAllProductVoucher[i].WorkPlaceCode;
                crudWarehouseBookDto.SectionCode = resulAllProductVoucher[i].SectionCode;
                crudWarehouseBookDto.CaseCode = resulAllProductVoucher[i].CaseCode;
                crudWarehouseBookDto.TrxQuantity = resulAllProductVoucher[i].TrxQuantity;
                crudWarehouseBookDto.TrxPriceCur = vLGLR != 1 ? resulAllProductVoucher[i].AssemblyAmountCru / resulAllProductVoucher[i].AssemblyQuantity : 0;
                crudWarehouseBookDto.TrxPrice = vLGLR != 1 ? resulAllProductVoucher[i].AssemblyAmount / resulAllProductVoucher[i].AssemblyQuantity : 0;
                crudWarehouseBookDto.Quantity = resulAllProductVoucher[i].AssemblyQuantity;
                crudWarehouseBookDto.PriceCur = vLGLR != 1 ? resulAllProductVoucher[i].AssemblyAmountCru / resulAllProductVoucher[i].AssemblyQuantity : resulAllProductVoucher[i].AssemblyPriceCur;
                crudWarehouseBookDto.Price = vLGLR != 1 ? resulAllProductVoucher[i].AssemblyAmount / resulAllProductVoucher[i].AssemblyQuantity : resulAllProductVoucher[i].AssemblyPrice;
                crudWarehouseBookDto.AmountCur = resulAllProductVoucher[i].AssemblyAmountCru;
                crudWarehouseBookDto.Amount = resulAllProductVoucher[i].AssemblyAmount;
                crudWarehouseBookDto.ExpenseAmountCur0 = resulAllProductVoucher[i].ExpenseAmountCur0;
                crudWarehouseBookDto.ExpenseAmount0 = resulAllProductVoucher[i].ExpenseAmount0;
                crudWarehouseBookDto.ExpenseAmountCur1 = resulAllProductVoucher[i].ExpenseAmountCur1;
                crudWarehouseBookDto.ExpenseAmount1 = resulAllProductVoucher[i].ExpenseAmount1;
                crudWarehouseBookDto.ExpenseAmount = resulAllProductVoucher[i].ExpenseAmount;
                crudWarehouseBookDto.ExprenseAmountCur = resulAllProductVoucher[i].ExpenseAmountCur;
                crudWarehouseBookDto.VatPercentage = resulAllProductVoucher[i].VatPercentage;
                crudWarehouseBookDto.VatAmountCur = resulAllProductVoucher[i].VatAmountCur;
                crudWarehouseBookDto.VatAmount = resulAllProductVoucher[i].VatAmount;
                crudWarehouseBookDto.DiscountPercentage = resulAllProductVoucher[i].DiscountPercentage;
                crudWarehouseBookDto.DiscountAmountCur = resulAllProductVoucher[i].DiscountAmountCru;
                crudWarehouseBookDto.DiscountAmount = resulAllProductVoucher[i].DiscountAmount;
                crudWarehouseBookDto.ImportTaxPercentage = resulAllProductVoucher[i].ImportTaxPercentage;
                crudWarehouseBookDto.ImportTaxAmountCur = resulAllProductVoucher[i].ImportTaxAmountCur;
                crudWarehouseBookDto.ImportTaxAmount = resulAllProductVoucher[i].ImportTaxAmount;
                crudWarehouseBookDto.ExciseTaxPercentage = resulAllProductVoucher[i].ExciseTaxPercentage;
                crudWarehouseBookDto.ExciseTaxAmountCur = resulAllProductVoucher[i].ExciseAmountCur;
                crudWarehouseBookDto.ExciseTaxAmount = resulAllProductVoucher[i].ExciseAmount;
                crudWarehouseBookDto.DebitAcc = resulAllProductVoucher[i].DebitAcc;
                crudWarehouseBookDto.CreditAcc = resulAllProductVoucher[i].CreditAcc;
                crudWarehouseBookDto.TrxPriceCur2 = resulAllProductVoucher[i].PriceCur2;
                crudWarehouseBookDto.TrxPrice2 = resulAllProductVoucher[i].PriceCur2;
                crudWarehouseBookDto.PriceCur2 = resulAllProductVoucher[i].PriceCur2;
                crudWarehouseBookDto.Price2 = resulAllProductVoucher[i].Price2;
                crudWarehouseBookDto.Price0 = resulAllProductVoucher[i].AssemblyAmount / resulAllProductVoucher[i].AssemblyQuantity;
                crudWarehouseBookDto.PriceCur0 = resulAllProductVoucher[i].AssemblyAmountCru / resulAllProductVoucher[i].AssemblyQuantity;
                crudWarehouseBookDto.AmountCur2 = resulAllProductVoucher[i].AmountCur2;
                crudWarehouseBookDto.Amount2 = resulAllProductVoucher[i].Amount2;
                crudWarehouseBookDto.DebitAcc2 = resulAllProductVoucher[i].DebitAcc2;
                crudWarehouseBookDto.CreditAcc2 = resulAllProductVoucher[i].CreditAcc2;
                crudWarehouseBookDto.Description = resulAllProductVoucher[i].Description;
                crudWarehouseBookDto.DescriptionE = resulAllProductVoucher[i].DescriptionE;
                //crudWarehouseBookDto.pri gia_nt0
                crudWarehouseBookDto.ImportAcc = vLGLR == 1 ? resulAllProductVoucher[i].CreditAcc : null;
                crudWarehouseBookDto.TrxImportQuantity = vLGLR == 1 ? resulAllProductVoucher[i].TrxQuantity : 0;
                crudWarehouseBookDto.ImportQuantity = vLGLR == 1 ? resulAllProductVoucher[i].AssemblyQuantity : 0;
                crudWarehouseBookDto.ImportAmountCur = vLGLR == 1 ? resulAllProductVoucher[i].AssemblyAmountCru : 0;
                crudWarehouseBookDto.ImportAmount = vLGLR == 1 ? resulAllProductVoucher[i].AssemblyAmount : 0;
                crudWarehouseBookDto.ExportAcc = vLGLR == 1 ? null : resulAllProductVoucher[i].CreditAcc;
                crudWarehouseBookDto.TrxExportQuantity = vLGLR == 1 ? 0 : resulAllProductVoucher[i].TrxQuantity;
                crudWarehouseBookDto.ExportQuantity = vLGLR == 1 ? 0 : resulAllProductVoucher[i].AssemblyQuantity;
                crudWarehouseBookDto.ExportAmountCur = vLGLR == 1 ? 0 : resulAllProductVoucher[i].AssemblyAmountCru;
                crudWarehouseBookDto.ExportAmount = vLGLR == 1 ? 0 : resulAllProductVoucher[i].AssemblyAmount;
                crudWarehouseBookDto.FixedPrice = resulAllProductVoucher[i].FixedPrice;
                crudWarehouseBookDto.TaxCategoryCode = resulAllProductVoucher[i].TaxCategoryCode;
                crudWarehouseBookDto.InvoiceSymbol = resulAllProductVoucher[i].InvoiceSymbol;
                crudWarehouseBookDto.InvoiceNumber = resulAllProductVoucher[i].InvoiceNumber;
                crudWarehouseBookDto.InvoicePartnerName = resulAllProductVoucher[i].ClearingPartnerCode;
                crudWarehouseBookDto.InvoicePartnerAddress = resulAllProductVoucher[i].AddressInv;
                crudWarehouseBookDto.TaxCode = resulAllProductVoucher[i].TaxCode;
                crudWarehouseBookDto.InvoiceDate = resulAllProductVoucher[i].InvoiceDate;
                crudWarehouseBookDto.SalesChannelCode = resulAllProductVoucher[i].SalesChannelCode;
                crudWarehouseBookDto.BillNumber = resulAllProductVoucher[i].BillNumber;
                crudWarehouseBookDto.TotalAmount2 = resulAllProductVoucher[i].TotalAmount2;
                crudWarehouseBookDto.TotalDiscountAmount = resulAllProductVoucher[i].TotalDiscountAmount;
                if (dto.CreationTime != null)
                {
                    crudWarehouseBookDto.CreationTime = dto.CreationTime;
                }
                else
                {
                    crudWarehouseBookDto.CreationTime = DateTime.Now;
                }
                crudWarehouseBookDtos.Add(crudWarehouseBookDto);
            }
            return crudWarehouseBookDtos;
        }

    }
}