using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
using Accounting.Catgories.ProductVouchers;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.Helpers;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.VoucherExciseTaxs;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.DomainServices.Ledgers
{
    public class LedgerService : BaseDomainService<Ledger, string>
    {
        private readonly ILogger<LedgerService> _logger;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly IObjectMapper _objectMapper;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly DefaultBusinessCategoryService _defaultBusinessCategoryService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly ProductVoucherService _productVoucher;
        private readonly ProductVoucherDetailService _productVoucherDetail;
        private readonly ProductVoucherAssemblyService _productVoucherAssembly;
        private readonly ProductVoucherReceiptService _productVoucherReceipt;
        private readonly ProductVoucherVatService _productVoucherVat;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductService _product;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly PaymentTermService _paymentTermService;
        private readonly PaymentTermDetailService _paymentTermDetailService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly OrgUnitService _orgUnitService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly WarehouseService _warehouseService;
        private readonly DefaultAccountSystemService _defaultAccountSystem;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;
        public LedgerService(IRepository<Ledger, string> repository,
                                TenantSettingService tenantSettingService,
                                VoucherCategoryService voucherCategoryService,
                                IObjectMapper objectMapper,
                                ILogger<LedgerService> logger,
                                DefaultVoucherCategoryService defaultVoucherCategoryService,
                                DefaultBusinessCategoryService defaultBusinessCategoryService,
                                DefaultTenantSettingService defaultTenantSettingService,
                                AccountSystemService accountSystemService,
                                BusinessCategoryService businessCategoryService,
                                TaxCategoryService taxCategoryService,
                                IUnitOfWorkManager unitOfWorkManager,
                                ProductVoucherService productVoucherService,
                                ProductVoucherDetailService productVoucherDetailService,
                                ProductVoucherAssemblyService productVoucherAssemblyService,
                                ProductVoucherReceiptService productVoucherReceiptService,
                                ProductVoucherVatService productVoucherVatService,
                                ProductUnitService productUnitService,
                                ProductService productService,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                VoucherTypeService voucherTypeService,
                                WarehouseBookService warehouseBookService,
                                PaymentTermService paymentTermService,
                                PaymentTermDetailService paymentTermDetailService,
                                WebHelper webHelper,
                                OrgUnitService orgUnitService,
                                YearCategoryService yearCategoryService,
                                ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
                                WarehouseService warehouse,
                                DefaultAccountSystemService defaultAccountSystem,
                                DefaultVoucherTypeService defaultVoucherTypeService
                                ) : base(repository)
        {
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _objectMapper = objectMapper;
            _logger = logger;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _defaultBusinessCategoryService = defaultBusinessCategoryService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _accountSystemService = accountSystemService;
            _businessCategoryService = businessCategoryService;
            _taxCategoryService = taxCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _productVoucher = productVoucherService;
            _productVoucherDetail = productVoucherDetailService;
            _productVoucherAssembly = productVoucherAssemblyService;
            _productVoucherReceipt = productVoucherReceiptService;
            _productVoucherVat = productVoucherVatService;
            _productUnitService = productUnitService;
            _product = productService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _voucherTypeService = voucherTypeService;
            _warehouseBookService = warehouseBookService;
            _paymentTermService = paymentTermService;
            _webHelper = webHelper;
            _orgUnitService = orgUnitService;
            _yearCategoryService = yearCategoryService;
            _paymentTermDetailService = paymentTermDetailService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
            _warehouseService = warehouse;
            _defaultAccountSystem = defaultAccountSystem;
            _defaultVoucherTypeService = defaultVoucherTypeService;
        }

        public async Task<List<Ledger>> GetByAccVoucherIdAsync(string accVoucherId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.VoucherId == accVoucherId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistCode(Ledger entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public async Task<List<Ledger>> GetLedgersAsync(string orgCode, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.Year == year);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<CrudLedgerDto>> MapLedger(CrudAccVoucherDto dto)
        {
            List<CrudLedgerDto> ledgers = new List<CrudLedgerDto>();
            using var unitOfWork = _unitOfWorkManager.Begin();
            var curencyCode = await _tenantSettingService.GetValue("M_MA_NT0", dto.OrgCode);
            if (curencyCode == null)
            {
                var defaultTenantSetting = await _defaultTenantSettingService.GetByKeyAsync("M_MA_NT0");
                curencyCode = _objectMapper.Map<DefaultTenantSetting, TenantSetting>(defaultTenantSetting).Value;
            }
            var voucherCategory = await _voucherCategoryService.CheckIsSavingLedgerAsync(dto.VoucherCode, dto.OrgCode);
            if (voucherCategory == null)
            {
                var defaultVoucherCategory = await _defaultVoucherCategoryService.GetByCodeAsync(dto.VoucherCode);
                voucherCategory = _objectMapper.Map<DefaultVoucherCategory, VoucherCategory>(defaultVoucherCategory);
                voucherCategory.OrgCode = _webHelper.GetCurrentOrgUnit();
            }
            var businessCategory = await _businessCategoryService.GetBusinessByCodeAsync(dto.BusinessCode, dto.OrgCode);
            if (businessCategory == null)
            {
                var defaultBusinessCategory = await _defaultBusinessCategoryService.GetByCodeAsync(dto.BusinessCode);
                businessCategory = _objectMapper.Map<DefaultBusinessCategory, BusinessCategory>(defaultBusinessCategory);
            }
            if (voucherCategory.IsSavingLedger == "C" || (businessCategory != null && businessCategory.IsAccVoucher))
            {
                if (dto.AccVoucherDetails != null)
                    for (int i = 0; i < dto.AccVoucherDetails.Count; i++)
                    {
                        int STT = i + 1;
                        var accVoucherDetail = dto.AccVoucherDetails[i];
                        var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(accVoucherDetail.DebitAcc, dto.OrgCode, dto.Year);
                        var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(accVoucherDetail.CreditAcc, dto.OrgCode, dto.Year);
                        CrudLedgerDto ledger = new CrudLedgerDto();
                        ledger.VoucherId = dto.Id;
                        ledger.Year = dto.Year;
                        ledger.OrgCode = dto.OrgCode;
                        ledger.Ord0 = accVoucherDetail.Ord0;
                        ledger.Ord0Extra = accVoucherDetail.Ord0;
                        ledger.DepartmentCode = dto.DepartmentCode;
                        ledger.VoucherCode = dto.VoucherCode;
                        ledger.VoucherGroup = dto.VoucherGroup;
                        ledger.BusinessCode = dto.BusinessCode;
                        ledger.BusinessAcc = dto.BusinessAcc;
                        ledger.VoucherNumber = dto.VoucherNumber;
                        ledger.InvoiceNbr = dto.InvoiceNumber;
                        ledger.RecordingVoucherNumber = "";
                        ledger.VoucherDate = dto.VoucherDate;
                        ledger.PaymentTermsCode = dto.PaymentTermsCode;
                        ledger.CurrencyCode = dto.CurrencyCode;
                        ledger.ExchangeRate = dto.ExchangeRate;
                        ledger.PartnerCode0 = dto.PartnerCode0;
                        ledger.PartnerName0 = dto.PartnerName0;
                        ledger.Representative = dto.Representative;
                        ledger.Address = dto.Address;
                        ledger.Description = dto.Description;
                        ledger.DescriptionE = "";
                        ledger.OriginVoucher = dto.OriginVoucher;
                        ledger.DebitAcc = accVoucherDetail.DebitAcc;
                        ledger.ContractCode = accVoucherDetail.ContractCode;
                        ledger.DebitExchangeRate = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                        ledger.DebitCurrencyCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                        ledger.DebitPartnerCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachPartner == "C") ? accVoucherDetail.PartnerCode : "";
                        ledger.DebitContractCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachContract == "C") ? accVoucherDetail.ContractCode : "";
                        ledger.DebitFProductWorkCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachProductCost == "C") ? accVoucherDetail.FProductWorkCode : "";
                        ledger.DebitSectionCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachAccSection == "C") ? accVoucherDetail.SectionCode : "";
                        ledger.DebitWorkPlaceCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachWorkPlace == "C") ? accVoucherDetail.WorkPlaceCode : "";
                        ledger.DebitAmountCur = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? accVoucherDetail.AmountCur : 0;
                        ledger.CreditAcc = accVoucherDetail.CreditAcc;
                        ledger.CreditExchangeRate = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                        ledger.CreditCurrencyCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                        ledger.CreditPartnerCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachPartner == "C") ? ((accVoucherDetail.ClearingPartnerCode != "" && accVoucherDetail.ClearingPartnerCode != null) ? accVoucherDetail.ClearingPartnerCode : accVoucherDetail.PartnerCode) : "";
                        ledger.CreditContractCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachContract == "C") ? ((accVoucherDetail.ClearingContractCode != "" && accVoucherDetail.ClearingContractCode != null) ? accVoucherDetail.ClearingContractCode : accVoucherDetail.ContractCode) : "";
                        ledger.CreditFProductWorkCode = accVoucherDetail.FProductWorkCode;
                        ledger.CreditSectionCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachAccSection == "C") ? ((accVoucherDetail.ClearingSectionCode != "" && accVoucherDetail.ClearingSectionCode != null) ? accVoucherDetail.ClearingSectionCode : accVoucherDetail.SectionCode) : "";
                        ledger.CreditWorkPlaceCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachWorkPlace == "C") ? ((accVoucherDetail.ClearingWorkPlaceCode != "" && accVoucherDetail.ClearingWorkPlaceCode != null) ? accVoucherDetail.ClearingWorkPlaceCode : accVoucherDetail.WorkPlaceCode) : "";
                        ledger.CreditAmountCur = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accVoucherDetail.AmountCur : 0;
                        ledger.CreditAmount = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accVoucherDetail.AmountCur : 0;
                        ledger.AmountCur = accVoucherDetail.AmountCur;
                        ledger.Amount = accVoucherDetail.Amount;
                        ledger.Note = dto.Description;
                        ledger.NoteE = accVoucherDetail.Note;
                        ledger.FProductWorkCode = accVoucherDetail.FProductWorkCode;
                        ledger.PartnerCode = accVoucherDetail.PartnerCode;
                        ledger.SectionCode = accVoucherDetail.SectionCode;
                        ledger.ClearingFProductWorkCode = accVoucherDetail.ClearingFProductWorkCode;
                        ledger.ClearingPartnerCode = accVoucherDetail.ClearingPartnerCode;
                        ledger.ClearingSectionCode = accVoucherDetail.ClearingSectionCode;
                        ledger.WorkPlaceCode = accVoucherDetail.WorkPlaceCode;
                        ledger.CaseCode = accVoucherDetail.CaseCode;
                        ledger.DebitOrCredit = dto.DebitOrCredit;
                        ledger.Status = dto.Status;
                        ledger.InvoiceDate = (dto.AccTaxDetails != null && dto.AccTaxDetails.Count > 0) ? dto.AccTaxDetails[0].InvoiceDate : null;
                        //Check khử trùng KT
                        ledger.CheckDuplicate = await _tenantSettingService.CheckRemoveDuplicateAccVoucher(dto.OrgCode, dto.VoucherCode, accVoucherDetail.CreditAcc, accVoucherDetail.DebitAcc);
                        ledgers.Add(ledger);
                    }

                if (dto.AccTaxDetails != null)
                    for (int i = 0; i < dto.AccTaxDetails.Count; i++)
                    {
                        int STT = i + 1;
                        var accTaxDetail = dto.AccTaxDetails[i];
                        var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(accTaxDetail.DebitAcc, dto.OrgCode, dto.Year);
                        var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(accTaxDetail.CreditAcc, dto.OrgCode, dto.Year);
                        var taxCategory = await _taxCategoryService.GetTaxByCodeAsync(accTaxDetail.TaxCategoryCode, dto.OrgCode);
                        if (taxCategory != null && taxCategory.IsDirect == false)
                        {
                            CrudLedgerDto Ledger = new CrudLedgerDto();
                            Ledger.VoucherId = dto.Id;
                            Ledger.Year = dto.Year;
                            Ledger.OrgCode = dto.OrgCode;
                            Ledger.Ord0 = accTaxDetail.Ord0;
                            Ledger.Ord0Extra = accTaxDetail.Ord0;
                            Ledger.DepartmentCode = dto.DepartmentCode;
                            Ledger.VoucherCode = dto.VoucherCode;
                            Ledger.VoucherGroup = dto.VoucherGroup;
                            Ledger.BusinessCode = dto.BusinessCode;
                            Ledger.BusinessAcc = dto.BusinessAcc;
                            Ledger.CheckDuplicate = dto.AccTaxDetails[0].CheckDuplicate;
                            Ledger.VoucherNumber = dto.VoucherNumber;
                            Ledger.InvoiceNbr = dto.InvoiceNumber;
                            Ledger.RecordingVoucherNumber = accTaxDetail.Ord0;
                            Ledger.VoucherDate = dto.VoucherDate;
                            Ledger.PaymentTermsCode = dto.PaymentTermsCode;
                            Ledger.CurrencyCode = dto.CurrencyCode;
                            Ledger.ExchangeRate = dto.ExchangeRate;
                            Ledger.PartnerCode0 = dto.PartnerCode0;
                            Ledger.PartnerName0 = dto.PartnerName0;
                            Ledger.Representative = dto.Representative;
                            Ledger.Address = dto.Address;
                            Ledger.Description = dto.Description;
                            Ledger.DescriptionE = accTaxDetail.Note;
                            Ledger.OriginVoucher = dto.OriginVoucher;
                            Ledger.DebitAcc = accTaxDetail.DebitAcc;
                            Ledger.ContractCode = accTaxDetail.ContractCode;
                            Ledger.DebitExchangeRate = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                            Ledger.DebitCurrencyCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                            Ledger.DebitPartnerCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachPartner == "C") ? accTaxDetail.PartnerCode : "";
                            Ledger.DebitContractCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachContract == "C") ? accTaxDetail.ContractCode : "";
                            Ledger.DebitFProductWorkCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachProductCost == "C") ? accTaxDetail.FProductWorkCode : "";
                            Ledger.DebitSectionCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachAccSection == "C") ? accTaxDetail.SectionCode : "";
                            Ledger.DebitWorkPlaceCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachWorkPlace == "C") ? accTaxDetail.WorkPlaceCode : "";
                            Ledger.DebitAmountCur = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? accTaxDetail.AmountCur : 0;
                            Ledger.CreditAcc = accTaxDetail.CreditAcc;
                            Ledger.CreditExchangeRate = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                            Ledger.CreditCurrencyCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                            Ledger.CreditPartnerCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachPartner == "C") ? ((accTaxDetail.ClearingPartnerCode != "" && accTaxDetail.ClearingPartnerCode != null) ? accTaxDetail.ClearingPartnerCode : accTaxDetail.PartnerCode) : "";
                            Ledger.CreditContractCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachContract == "C") ? ((accTaxDetail.ClearingContractCode != "" && accTaxDetail.ClearingContractCode != null) ? accTaxDetail.ClearingContractCode : accTaxDetail.ContractCode) : "";
                            Ledger.CreditFProductWorkCode = accTaxDetail.FProductWorkCode;
                            Ledger.CreditSectionCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachAccSection == "C") ? ((accTaxDetail.ClearingSectionCode != "" && accTaxDetail.ClearingSectionCode != null) ? accTaxDetail.ClearingSectionCode : accTaxDetail.SectionCode) : "";
                            Ledger.CreditWorkPlaceCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachWorkPlace == "C") ? ((accTaxDetail.ClearingWorkPlaceCode != "" && accTaxDetail.ClearingWorkPlaceCode != null) ? accTaxDetail.ClearingWorkPlaceCode : accTaxDetail.WorkPlaceCode) : "";
                            Ledger.CreditAmountCur = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accTaxDetail.AmountCur : 0;
                            Ledger.CreditAmount = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accTaxDetail.AmountCur : 0;
                            Ledger.AmountCur = accTaxDetail.AmountCur;
                            Ledger.Amount = accTaxDetail.Amount;
                            Ledger.Note = accTaxDetail.Note;
                            Ledger.NoteE = accTaxDetail.NoteE;
                            Ledger.FProductWorkCode = accTaxDetail.FProductWorkCode;
                            Ledger.PartnerCode = accTaxDetail.PartnerCode;
                            Ledger.SectionCode = accTaxDetail.SectionCode;
                            Ledger.ClearingFProductWorkCode = accTaxDetail.ClearingFProductWorkCode;
                            Ledger.ClearingPartnerCode = accTaxDetail.ClearingPartnerCode;
                            Ledger.ClearingSectionCode = accTaxDetail.ClearingSectionCode;
                            Ledger.WorkPlaceCode = accTaxDetail.WorkPlaceCode;
                            Ledger.CaseCode = accTaxDetail.CaseCode;
                            Ledger.DebitOrCredit = dto.DebitOrCredit;
                            Ledger.InvoiceDate = dto.AccTaxDetails[i].InvoiceDate;
                            Ledger.Status = dto.Status;
                            //Check khử trùng KT
                            Ledger.CheckDuplicate = await _tenantSettingService.CheckRemoveDuplicateAccVoucher(dto.OrgCode, dto.VoucherCode, accTaxDetail.CreditAcc, accTaxDetail.DebitAcc);
                            ledgers.Add(Ledger);
                        }
                    }
            }
            return ledgers;
        }


        public async Task<List<Ledger>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.VoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task DeleteByVoucherId(string id)
        {
            var ledgers = await this.GetRepository()
                                    .GetListAsync(p => p.VoucherId == id);
            if (ledgers.Count == 0) return;
            await this.GetRepository().DeleteManyAsync(ledgers);
        }
        public async Task WareSoundAsync(CrudProductVoucherDto entity)
        {
            var proDuctVoucher = _productVoucher.GetByProductIdAsync(entity.Id);

            List<CrudProductVoucherDetailDto> productVoucherDetails = (List<CrudProductVoucherDetailDto>)entity.ProductVoucherDetails;
            var produc = _product.GetByProductCodeAsync(productVoucherDetails[0].ProductCode, entity.OrgCode);
            List<Product> producs = await produc;
            var proDuctUnit = _productUnitService.GetByProductCodeAsync(productVoucherDetails[0].ProductCode, entity.OrgCode);
            var proDuctUnit2 = _productUnitService.GetByProductUnitAsync(productVoucherDetails[0].ProductCode, entity.OrgCode, productVoucherDetails[0].UnitCode);

            List<ProductUnit> productUnits = await proDuctUnit;
            List<ProductUnit> proDuctUni0t = await proDuctUnit2;
            List<ProductVoucher> productVouchers = await proDuctVoucher;
            var query = from a in productVouchers
                        join b in productVoucherDetails on a.Id equals b.ProductVoucherId
                        join c in productUnits on b.ProductCode equals c.ProductCode
                        join d in producs on b.ProductCode equals d.Code
                        join e in proDuctUni0t on b.ProductCode equals e.ProductCode
                        where d.ProductType != "D" && a.Id == entity.Id
                        group new { a, b, c, d, e } by new
                        {
                            a.OrgCode,
                            b.ProductCode,
                            b.ProductLotCode,
                            b.ProductOriginCode,
                            b.WarehouseCode,
                            a.ImportDate,
                            a.Year,
                            b.Quantity
                        } into g
                        orderby g.Key.ProductCode
                        select new
                        {

                            Quantity = g.Max(band => band.b.Quantity),
                            Quantity2 = g.Sum(d => d.c.ExchangeRate != 0 ? d.b.Quantity * d.c.ExchangeRate / d.e.ExchangeRate : 0),
                            OrgCode = g.Key.OrgCode,
                            ProductCode = g.Key.ProductCode,
                            ProductLotCode = g.Key.ProductLotCode,
                            ProductOriginCode = g.Key.ProductOriginCode,
                            WarehouseCode = g.Key.WarehouseCode,
                            ImportDate = g.Key.ImportDate,
                            Year = g.Key.Year,

                        };

            var quantyti = query.Select(p => p.Quantity);

            var _productOpeningBalances = _productOpeningBalanceService.GetByProductOpeningBalanceAsync(productVoucherDetails[0].ProductCode, entity.OrgCode, productVoucherDetails[0].ProductLotCode, productVoucherDetails[0].WarehouseCode, productVoucherDetails[0].ProductOriginCode);
            List<ProductOpeningBalance> productOpeningBalance = await _productOpeningBalances;


            var WareHouseBook = _warehouseBookService.GetByWarehouseBookAsync(entity.Id, entity.Year, entity.VoucherDate, entity.OrgCode, entity.ImportDate);
            List<WarehouseBook> _warehouseBooks = await WareHouseBook;

            var queryProduct = from A in productOpeningBalance
                               join B in query on A.ProductCode equals B.ProductCode
                               where A.Year == entity.Year && A.ProductLotCode == B.ProductLotCode
                               && A.WarehouseCode == B.WarehouseCode && A.ProductOriginCode == B.ProductOriginCode
                               group new { A, B } by new
                               {
                                   A.OrgCode,
                                   B.ProductCode,
                                   B.ProductLotCode,
                                   B.ProductOriginCode,
                                   B.WarehouseCode,
                                   B.ImportDate,
                                   B.Year,
                                   B.Quantity
                               } into g
                               orderby g.Key.ProductCode
                               select new
                               {
                                   Quantity = g.Sum(d => d.A.Quantity),
                                   OrgCode = g.Key.OrgCode,
                                   ProductCode = g.Key.ProductCode,
                                   ProductLotCode = g.Key.ProductLotCode,
                                   ProductOriginCode = g.Key.ProductOriginCode,
                                   WarehouseCode = g.Key.WarehouseCode,
                                   ImportDate = g.Key.ImportDate,
                                   Year = g.Key.Year,

                               };
            var queryWareBook = from A in _warehouseBooks
                                join B in query on A.ProductCode equals B.ProductCode

                                where A.Year == entity.Year && A.ProductLotCode == B.ProductLotCode
                                && A.WarehouseCode == B.WarehouseCode && A.ProductOriginCode == B.ProductOriginCode
                                group new { A, B } by new
                                {
                                    A.OrgCode,
                                    B.ProductCode,
                                    B.ProductLotCode,
                                    B.ProductOriginCode,
                                    B.WarehouseCode,
                                    B.ImportDate,
                                    B.Year,
                                    B.Quantity
                                } into g
                                orderby g.Key.ProductCode
                                select new
                                {
                                    Quantity = g.Sum(d => d.A.ImportQuantity - d.A.ExportQuantity),
                                    OrgCode = g.Key.OrgCode,
                                    ProductCode = g.Key.ProductCode,
                                    ProductLotCode = g.Key.ProductLotCode,
                                    ProductOriginCode = g.Key.ProductOriginCode,
                                    WarehouseCode = g.Key.WarehouseCode,
                                    ImportDate = g.Key.ImportDate,
                                    Year = g.Key.Year,

                                };



            var total = from A in queryProduct
                        join b in queryWareBook on A.ProductCode equals b.ProductCode
                        where A.ProductCode == b.ProductCode && A.ProductLotCode == b.ProductLotCode &&
                                A.ProductOriginCode == b.ProductOriginCode && A.WarehouseCode == b.WarehouseCode

                        group new { A, b } by new
                        {
                            A.OrgCode,
                            b.ProductCode,
                            b.ProductLotCode,
                            b.ProductOriginCode,
                            b.WarehouseCode,
                            b.ImportDate,
                            b.Year,
                            b.Quantity
                        } into g
                        orderby g.Key.ProductCode
                        select new { quantyti = g.Sum(a => a.A.Quantity) };
            var QuantytyCrud = total.Select(p => p.quantyti);

            if (Convert.ToDecimal(quantyti) - Convert.ToDecimal(QuantytyCrud) < 0)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
            }

        }
        public async Task<List<CrudVoucherExciseTaxDto>> VoucherExcitaxAsync(CrudProductVoucherDto entity)
        {
            var voucherTypelst = (await _voucherTypeService.GetByCodeAsync("000"));
            var ListVoucher = "";
            if (voucherTypelst != null)
            {
                ListVoucher = voucherTypelst.ListVoucher;
            }
            else
            {
                var defaultVoucherTypelst = await _defaultVoucherTypeService.GetQueryableAsync();
                ListVoucher = defaultVoucherTypelst.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            }

            //SET @v_IsHt2 = IIF(CHARINDEX(@p_MA_CT, @v_LST_CT2) > 0, 1, 0)
            List<CrudProductVoucherDetailDto> productVoucherDetails = (List<CrudProductVoucherDetailDto>)entity.ProductVoucherDetails;
            List<CrudAccTaxDetailDto> CrudAccTaxDetailDto = (List<CrudAccTaxDetailDto>)entity.AccTaxDetails;
            List<CrudVoucherExciseTaxDto> voucherExciseTaxes = new List<CrudVoucherExciseTaxDto>();
            List<CrudProductVoucherDetailReceiptDto> CrudProductVoucherDetailReceiptDto = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[0].ProductVoucherDetailReceipts;
            if (CrudProductVoucherDetailReceiptDto != null && CrudProductVoucherDetailReceiptDto.Any())
            {
                if (CrudProductVoucherDetailReceiptDto[0].ExciseTaxAmount > 0 || CrudProductVoucherDetailReceiptDto[0].ExciseTaxAmountCur > 0)
                {
                    var dmhv = await _product.GetByProductCodeAsync(productVoucherDetails[0].ProductCode, entity.OrgCode);
                    var isHt2 = 0;
                    if (ListVoucher.Contains(entity.VoucherCode) == false)
                    {
                        isHt2 = 1;
                    }

                    CrudVoucherExciseTaxDto crudVoucherExciseTax = new CrudVoucherExciseTaxDto();
                    crudVoucherExciseTax.ProductVoucherId = entity.Id;
                    crudVoucherExciseTax.Ord0 = "T000000001";
                    crudVoucherExciseTax.OrgCode = entity.OrgCode;
                    crudVoucherExciseTax.Year = entity.Year;
                    crudVoucherExciseTax.VoucherCode = entity.VoucherCode;
                    crudVoucherExciseTax.DepartmentCode = entity.DepartmentCode;
                    crudVoucherExciseTax.VoucherNumber = entity.VoucherNumber;
                    crudVoucherExciseTax.VoucherDate = entity.VoucherDate;
                    crudVoucherExciseTax.ExciseTaxCode = dmhv.Count() > 0 ? dmhv[0].ExciseTaxCode : "";
                    crudVoucherExciseTax.InvoiceGroup = CrudAccTaxDetailDto.Count() > 0 ? CrudAccTaxDetailDto[0].InvoiceGroup : "";
                    crudVoucherExciseTax.InvoiceSymbol = CrudAccTaxDetailDto.Count() > 0 ? CrudAccTaxDetailDto[0].InvoiceSymbol : "";
                    crudVoucherExciseTax.InvoiceDate = CrudAccTaxDetailDto.Count() > 0 ? CrudAccTaxDetailDto[0].InvoiceDate : (DateTime?)null;
                    crudVoucherExciseTax.InvoiceNumber = CrudAccTaxDetailDto.Count() > 0 ? CrudAccTaxDetailDto[0].InvoiceNumber : null;
                    crudVoucherExciseTax.DebitAcc = isHt2 == 1 ? productVoucherDetails[0].DebitAcc2 : productVoucherDetails[0].DebitAcc;
                    crudVoucherExciseTax.PartnerCode = entity.PartnerCode0;
                    crudVoucherExciseTax.FProductWorkCode = productVoucherDetails[0].FProductWorkCode;
                    crudVoucherExciseTax.ContractCode = productVoucherDetails[0].ContractCode;
                    crudVoucherExciseTax.SectionCode = productVoucherDetails[0].SectionCode;
                    crudVoucherExciseTax.WorkPlaceCode = productVoucherDetails[0].WorkPlaceCode;
                    crudVoucherExciseTax.CreditAcc = isHt2 == 1 ? productVoucherDetails[0].CreditAcc2 : productVoucherDetails[0].CreditAcc;
                    crudVoucherExciseTax.Address = entity.Address;
                    crudVoucherExciseTax.TaxCode = CrudAccTaxDetailDto.Count() > 0 ? CrudAccTaxDetailDto[0].TaxCode : null;
                    crudVoucherExciseTax.ProductName = productVoucherDetails[0].ProductName;
                    crudVoucherExciseTax.AmountWithoutTaxCur = isHt2 == 1 ? productVoucherDetails[0].AmountCur : productVoucherDetails[0].AmountCur2;
                    crudVoucherExciseTax.AmountWithoutTax = isHt2 == 1 ? productVoucherDetails[0].Amount : productVoucherDetails[0].Amount2;
                    crudVoucherExciseTax.ExciseTaxPercentage = CrudProductVoucherDetailReceiptDto.Count() > 0 ? CrudProductVoucherDetailReceiptDto[0].ExciseTaxPercentage : null;
                    crudVoucherExciseTax.Amount = CrudProductVoucherDetailReceiptDto.Count() > 0 ? CrudProductVoucherDetailReceiptDto[0].ExciseTaxAmount : null;
                    crudVoucherExciseTax.AmountCur = CrudProductVoucherDetailReceiptDto.Count() > 0 ? CrudProductVoucherDetailReceiptDto[0].ExciseTaxAmountCur : null;
                    crudVoucherExciseTax.Note = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].Note : null;
                    crudVoucherExciseTax.NoteE = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].NoteE : null;
                    crudVoucherExciseTax.CaseCode = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].CaseCode : null;
                    crudVoucherExciseTax.Quantity = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].Quantity : null;
                    crudVoucherExciseTax.PriceCur = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].PriceCur : null;
                    crudVoucherExciseTax.Price = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].Price : null;
                    crudVoucherExciseTax.ProductCode0 = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].ProductCode : null;
                    crudVoucherExciseTax.ProductName0 = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].ProductName0 : null;
                    crudVoucherExciseTax.UnitCode0 = productVoucherDetails.Count() > 0 ? productVoucherDetails[0].UnitCode : null;
                    crudVoucherExciseTax.Type = "R";
                    voucherExciseTaxes.Add(crudVoucherExciseTax);
                }
            }



            return voucherExciseTaxes;




        }
        public async Task<List<CrudVoucherPaymentBookDto>> CreateVoucherPaymentAsync(CrudProductVoucherDto entity)
        {

            List<CrudVoucherPaymentBookDto> crudVoucherPaymentBookDto = new List<CrudVoucherPaymentBookDto>();
            List<CrudProductVoucherDetailDto> productVoucherDetails = (List<CrudProductVoucherDetailDto>)entity.ProductVoucherDetails;
            List<CrudAccTaxDetailDto> AccTaxDetails = (List<CrudAccTaxDetailDto>)entity.AccTaxDetails;
            if (string.IsNullOrEmpty(entity.PaymentTermsCode) == false)
            {
                var listPayment = await _paymentTermService.GetByPaymentTermAsync(entity.PaymentTermsCode, _webHelper.GetCurrentOrgUnit());

                if (listPayment != null)
                {
                    if (listPayment.Count > 0)
                    {
                        var accAcout = "";
                        var accType = "";
                        if (entity.VoucherCode == "PBH")
                        {
                            accAcout = productVoucherDetails[0].DebitAcc2;
                            accType = "C";
                        }
                        else if (entity.VoucherCode == "PDV")
                        {
                            accAcout = productVoucherDetails[0].DebitAcc;
                            accType = "C";
                        }
                        else if (entity.VoucherCode == "PNH")
                        {
                            accAcout = productVoucherDetails[0].CreditAcc;
                            accType = "N";
                        }
                        else if (entity.VoucherCode == "PNX")
                        {
                            accAcout = productVoucherDetails[0].CreditAcc;
                            accType = "N";
                        }

                        if (productVoucherDetails != null)
                        {
                            if (productVoucherDetails.Count > 0)
                            {
                                var debitAcc = await _accountSystemService.GetAccountByAccCodeAsync(productVoucherDetails[0].DebitAcc, _webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());

                            }
                        }

                        var paymentDetail = await _paymentTermDetailService.GetByPamentTermId(listPayment[0].Id);


                        foreach (var item in paymentDetail)
                        {

                            CrudVoucherPaymentBookDto crud = new CrudVoucherPaymentBookDto();

                            crud.DocumentId = entity.Id;
                            crud.VoucherNumber = entity.VoucherNumber;
                            crud.VoucherDate = entity.VoucherDate;
                            crud.VoucherNumber = entity.VoucherNumber;
                            crud.AccCode = accAcout;
                            crud.PartnerCode = entity.PartnerCode == null || entity.PartnerCode == "" ? entity.PartnerCode0 : entity.PartnerCode;
                            crud.Amount = entity.TotalAmountWithoutVat;
                            crud.VatAmount = entity.TotalVatAmount;
                            crud.DiscountAmount = entity.TotalDiscountAmount;
                            crud.TotalAmount = entity.TotalAmount;
                            crud.DeadlinePayment = entity.VoucherDate.AddDays((int)item.Days);
                            crud.AmountReceivable = (decimal)(entity.TotalAmount * item.Percentage / 100);
                            crud.Times = (int)item.Ord;
                            crud.Year = _webHelper.GetCurrentYear();
                            crud.AccType = accType;
                            crudVoucherPaymentBookDto.Add(crud);
                        }

                    }
                }


            }
            return crudVoucherPaymentBookDto;
        }

        public async Task<List<CrudLedgerDto>> CreateLedgerAsync(CrudProductVoucherDto entity)
        {
            var lstAccSystem = (await _accountSystemService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            if (lstAccSystem.Count == 0)
            {
                var yearCategory = await _yearCategoryService.GetByYearAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
                lstAccSystem = (await _defaultAccountSystem.GetListAsync(yearCategory.UsingDecision ?? 200)).Select(p => _objectMapper.Map<DefaultAccountSystem, AccountSystem>(p)).ToList();
            }
            var defaultVoucherType = (await _defaultVoucherTypeService.GetQueryableAsync()).ToList();
            var voucherTypeService = (await _voucherTypeService.GetQueryableAsync()).ToList();
            var ListVoucher = "";
            string LISTCT_NO1 = "";
            string LstMA_CT = "";
            if (voucherTypeService.Count > 0)
            {
                // var LISTCT_HT2 = await _voucherTypeService.GetByVoucherTypeAsync("000");
                ListVoucher = voucherTypeService.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
                LISTCT_NO1 = voucherTypeService.Where(p => p.Code == "NO1").FirstOrDefault().ListVoucher;
                LstMA_CT = voucherTypeService.Where(p => p.Code == "PCP").FirstOrDefault().ListVoucher;

            }
            else
            {
                ListVoucher = defaultVoucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
                LISTCT_NO1 = defaultVoucherType.Where(p => p.Code == "NO1").FirstOrDefault().ListVoucher;
                LstMA_CT = defaultVoucherType.Where(p => p.Code == "PCP").FirstOrDefault().ListVoucher;
            }

            List<CrudLedgerDto> Ledgers = new List<CrudLedgerDto>();
            //var LISTCT_NO1 = await _voucherTypeService.GetByVoucherTypeAsync("NO1");
            //var LstMA_CT = await _voucherTypeService.GetByVoucherTypeAsync("PCP");
            var vLstMA_CT = "";
            var vLISTCT_HT2 = "";
            var vLISTCT_NO1 = "";
            if (LISTCT_NO1 == null)
            {
                vLISTCT_NO1 = "";
            }
            else
            {
                vLISTCT_NO1 = vLISTCT_NO1 + ",";
            }

            vLstMA_CT = LstMA_CT;



            vLISTCT_HT2 = ListVoucher;


            if (vLstMA_CT == null)
            {
                vLISTCT_HT2 = "PBH,PTL";
            }
            var v_LG_HT2 = 0;
            if (vLISTCT_HT2.Contains(entity.VoucherCode) == true)
            {
                v_LG_HT2 = 1;
            }
            var LISTCT_NO2 = "";

            LISTCT_NO2 = LISTCT_NO1;

            if (LISTCT_NO2 == null)
            {
                LISTCT_NO2 = "";
            }

            if (LISTCT_NO2 != null)
            {
                LISTCT_NO2 = LISTCT_NO2 + ",";
            }
            var p_Ma_ct = entity.VoucherCode;
            var voucherCategory = await _voucherCategoryService.CheckIsSavingLedgerAsync(entity.VoucherCode, entity.OrgCode);
            var IsSOCAI = "K";
            var IsSOCAI2 = "K";
            // WareSoundAsync(entity);

            var curencyCode = await _tenantSettingService.GetValue("M_MA_NT0", entity.OrgCode);
            var VHT_KHU_TRUNG_HV = await _tenantSettingService.GetValue("VHT_KHU_TRUNG_HV", entity.OrgCode);
            var VHT_TTDB_VALUE = await _tenantSettingService.GetValue("VHT_TTDB_VALUE", entity.OrgCode);
            var VHT_XUAT_AM = await _tenantSettingService.GetValue("VHT_XUAT_AM", entity.OrgCode);

            List<CrudProductVoucherDetailDto> productVoucherDetails = (List<CrudProductVoucherDetailDto>)entity.ProductVoucherDetails;
            List<CrudAccTaxDetailDto> AccTaxDetails = (List<CrudAccTaxDetailDto>)entity.AccTaxDetails;

            for (int i = 0; i < productVoucherDetails.Count(); i++)
            {
                var typeProduct = await _product.GetByProductCodeAsync(productVoucherDetails[i].ProductCode, entity.OrgCode);
                var debitAcc = productVoucherDetails[i].DebitAcc == null ? null : productVoucherDetails[i].DebitAcc.Left(3);
                var creditAcc = productVoucherDetails[i].CreditAcc == null ? null : productVoucherDetails[i].CreditAcc.Left(3);
                var test = VHT_KHU_TRUNG_HV.Contains(debitAcc);
                if (string.IsNullOrEmpty(debitAcc) == false && string.IsNullOrEmpty(creditAcc) == false)
                {
                    if (VHT_KHU_TRUNG_HV.Contains(debitAcc) == false
                       && VHT_KHU_TRUNG_HV.Contains(creditAcc) == false
                       && vLstMA_CT.Contains(p_Ma_ct) == false
                       && vLISTCT_NO1.Contains(p_Ma_ct) == false
                       && (v_LG_HT2 != 1 || typeProduct[0].ProductType != "D")
                       )
                    {
                        IsSOCAI = "C";
                    }
                }

                var debitAcc2 = productVoucherDetails[i].DebitAcc2 == null ? null : productVoucherDetails[i].DebitAcc2;
                var creditAcc2 = productVoucherDetails[i].CreditAcc2 == null ? null : productVoucherDetails[i].CreditAcc2;
                if (string.IsNullOrEmpty(debitAcc2) == false && string.IsNullOrEmpty(creditAcc2) == false)
                {
                    if (VHT_KHU_TRUNG_HV.Contains(debitAcc2.Left(3)) == false
                 && VHT_KHU_TRUNG_HV.Contains(creditAcc2.Left(3)) == false
                 && vLstMA_CT.Contains(p_Ma_ct) == false
                 && productVoucherDetails[i].Quantity != 0
                 && productVoucherDetails[i].AmountCur2 + productVoucherDetails[i].Amount2 != 0
                  && (v_LG_HT2 == 1)
                 )
                    {
                        IsSOCAI2 = "C";
                    }
                }
                
;                var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[i].DebitAcc, entity.OrgCode, _webHelper.GetCurrentYear());
                var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[i].CreditAcc, entity.OrgCode, _webHelper.GetCurrentYear());
                if (accountSystemDebitAcc == null)
                {
                    var defaultAccountSystems = await _defaultAccountSystem.GetQueryableAsync();
                    accountSystemDebitAcc = (from a in defaultAccountSystems
                                             where a.AccCode == productVoucherDetails[i].DebitAcc
                                             select new AccountSystem
                                             {
                                                 AccCode = a.AccCode,
                                                 AccPattern = a.AccPattern,
                                                 AccSectionCode = a.AccSectionCode,
                                                 AttachAccSection = a.AttachAccSection,
                                                 AttachContract = a.AttachContract,
                                                 AttachCurrency = a.AttachCurrency,
                                                 AttachPartner = a.AttachPartner,
                                                 AttachProductCost = a.AttachProductCost,
                                                 AttachWorkPlace = a.AttachWorkPlace,
                                                 IsBalanceSheetAcc = a.IsBalanceSheetAcc
                                             }).FirstOrDefault();
                }
                if (accountSystemCreditAcc == null)
                {
                    var defaultAccountSystems = await _defaultAccountSystem.GetQueryableAsync();
                    accountSystemCreditAcc = (from a in defaultAccountSystems
                                              where a.AccCode == productVoucherDetails[i].CreditAcc
                                              select new AccountSystem
                                              {
                                                  AccCode = a.AccCode,
                                                  AccPattern = a.AccPattern,
                                                  AccSectionCode = a.AccSectionCode,
                                                  AttachAccSection = a.AttachAccSection,
                                                  AttachContract = a.AttachContract,
                                                  AttachCurrency = a.AttachCurrency,
                                                  AttachPartner = a.AttachPartner,
                                                  AttachProductCost = a.AttachProductCost,
                                                  AttachWorkPlace = a.AttachWorkPlace,
                                                  IsBalanceSheetAcc = a.IsBalanceSheetAcc

                                              }).FirstOrDefault();
                }
                var AttachCurrency = "";
                if (accountSystemCreditAcc != null)
                {
                    AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                }
                if (entity.VoucherGroup == 3)
                {
                    if (!string.IsNullOrEmpty(productVoucherDetails[i].ProductAcc))
                    {
                        accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[i].ProductAcc, entity.OrgCode, _webHelper.GetCurrentYear());
                        //accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem,productVoucherDetails[0].CreditAcc, entity.OrgCode, _webHelper.GetCurrentYear());

                    }
                }

                if (IsSOCAI == "C")
                {

                    CrudLedgerDto Ledger = new CrudLedgerDto();
                    Ledger.VoucherId = entity.Id;
                    Ledger.Ord0 = productVoucherDetails[i].Ord0;
                    Ledger.Year = entity.Year;
                    Ledger.Ord0Extra = productVoucherDetails[i].Ord0;
                    Ledger.OrgCode = entity.OrgCode;
                    Ledger.DepartmentCode = entity.DepartmentCode;
                    Ledger.VoucherCode = entity.VoucherCode;
                    Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                    Ledger.BusinessCode = entity.BusinessCode;
                    Ledger.BusinessAcc = entity.BusinessAcc;
                    Ledger.VoucherNumber = entity.VoucherNumber;
                    Ledger.InvoiceNbr = entity.InvoiceNumber;
                    Ledger.VoucherDate = entity.VoucherDate;
                    Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                    Ledger.CurrencyCode = entity.CurrencyCode;
                    Ledger.ExchangeRate = entity.ExchangeRate;
                    Ledger.PartnerCode0 = entity.PartnerCode0;
                    Ledger.PartnerName0 = entity.PartnerName0;
                    Ledger.Representative = entity.Representative;
                    Ledger.Address = entity.Address;
                    Ledger.Description = entity.Description;
                    Ledger.DescriptionE = entity.DescriptionE;
                    Ledger.DebitAcc = entity.VoucherGroup == 3 ? productVoucherDetails[i].ProductAcc : productVoucherDetails[i].DebitAcc;
                    Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                    Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                    Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" && entity.VoucherGroup == 1 && string.IsNullOrEmpty(entity.DiscountCreditAcc) == true && string.IsNullOrEmpty(entity.DiscountDebitAcc) == true ? productVoucherDetails[i].AmountCur - productVoucherDetails[i].DiscountAmountCur : 0;
                    Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                    Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                    Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                    Ledger.DebitFProductWorkCode = accountSystemDebitAcc.AttachProductCost == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                    Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                    Ledger.CreditAcc = productVoucherDetails[i].CreditAcc;
                    Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                    Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                    Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                    Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                    Ledger.CreditAmountCur = entity.VoucherGroup == 3 ? productVoucherDetails[i].AmountCur : (AttachCurrency == "C" && entity.VoucherGroup == 1 && entity.DiscountCreditAcc == null && entity.DiscountDebitAcc == null ? productVoucherDetails[i].AmountCur - productVoucherDetails[i].DiscountAmountCur : 0);
                    Ledger.CreditAmount = entity.VoucherGroup == 3 ? productVoucherDetails[i].AmountCur : (AttachCurrency == "C" && entity.VoucherGroup == 1 && entity.DiscountCreditAcc == null && entity.DiscountDebitAcc == null ? productVoucherDetails[i].Amount - productVoucherDetails[i].DiscountAmount : 0);
                    if (entity.VoucherGroup == 4)
                    {
                        Ledger.AmountCur = productVoucherDetails[i].AmountCur;
                        Ledger.Amount = productVoucherDetails[i].Amount;

                    }
                    else
                    {
                        Ledger.AmountCur = entity.VoucherGroup == 1 ? (string.IsNullOrEmpty(entity.DiscountCreditAcc) == true && string.IsNullOrEmpty(entity.DiscountDebitAcc) == true ? productVoucherDetails[i].AmountCur - productVoucherDetails[i].DiscountAmountCur : productVoucherDetails[i].AmountCur) : productVoucherDetails[i].AmountCur2 != 0 && productVoucherDetails[i].AmountCur2 != null ? productVoucherDetails[i].AmountCur2 : productVoucherDetails[i].AmountCur;
                        Ledger.Amount = entity.VoucherGroup == 1 ? (string.IsNullOrEmpty(entity.DiscountCreditAcc) == true && string.IsNullOrEmpty(entity.DiscountDebitAcc) == true ? productVoucherDetails[i].Amount - productVoucherDetails[i].DiscountAmount : productVoucherDetails[i].Amount) : productVoucherDetails[i].Amount2 != 0 && productVoucherDetails[i].Amount2 != null ? productVoucherDetails[i].Amount2 : productVoucherDetails[i].Amount;

                    }
                    if (entity.VoucherCode == "PBH")
                    {
                        Ledger.AmountCur = productVoucherDetails[i].AmountCur ?? 0;
                        Ledger.Amount = productVoucherDetails[i].Amount ?? 0;
                    }
                    if (entity.VoucherCode == "BH8")
                    {
                        Ledger.AmountCur = productVoucherDetails[i].AmountCur2 ?? 0;
                        Ledger.Amount = productVoucherDetails[i].Amount2 - productVoucherDetails[i].DecreaseAmount ?? 0;

                    }
                    Ledger.Note = entity.Description;
                    Ledger.NoteE = entity.DescriptionE;
                    Ledger.CreditFProductWorkCode = accountSystemCreditAcc.AttachProductCost == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                    Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                    Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                    Ledger.CaseCode = productVoucherDetails[i].CaseCode;
                    Ledger.WarehouseCode = productVoucherDetails[i].WarehouseCode;
                    Ledger.TransWarehouseCode = productVoucherDetails[i].TransWarehouseCode;
                    Ledger.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                    Ledger.ProductCode = productVoucherDetails[i].ProductCode;
                    Ledger.ProductLotCode = productVoucherDetails[i].ProductLotCode;
                    Ledger.ProductOriginCode = productVoucherDetails[i].ProductOriginCode;
                    Ledger.SectionCode = productVoucherDetails[i].SectionCode;
                    Ledger.UnitCode = productVoucherDetails[i].UnitCode;
                    Ledger.Quantity = productVoucherDetails[i].Quantity;
                    Ledger.PriceCur = productVoucherDetails[i].PriceCur;
                    Ledger.Price = productVoucherDetails[i].Price;
                    Ledger.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                    if (AccTaxDetails != null)
                    {
                        if (AccTaxDetails.Count > 0)
                        {
                            Ledger.VatPercentage = AccTaxDetails[0].VatPercentage;
                            Ledger.InvoiceNumber = AccTaxDetails[0].InvoiceNumber;
                            Ledger.InvoiceSymbol = AccTaxDetails[0].InvoiceSymbol;
                            Ledger.InvoiceDate = AccTaxDetails[0].InvoiceDate;
                            Ledger.TaxCode = AccTaxDetails[0].TaxCode;
                            Ledger.CheckDuplicate = AccTaxDetails[0].CheckDuplicate;
                            Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                        }

                    }

                    Ledger.DebitOrCredit = null;
                    Ledger.SalesChannelCode = entity.SalesChannelCode;
                    Ledger.ProductName0 = productVoucherDetails[i].ProductName0;
                    Ledger.PartnerCode = entity.PartnerCode ?? entity.PartnerCode0;
                    Ledger.ContractCode = productVoucherDetails[i].ContractCode;
                    Ledger.Status = entity.Status;
                    Ledgers.Add(Ledger);
                }
            }
            if (IsSOCAI2 == "C")
            {

                for (int i = 0; i < productVoucherDetails.Count(); i++)
                {
                    var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[0].DebitAcc2, entity.OrgCode, entity.Year);
                    var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[0].CreditAcc2, entity.OrgCode, entity.Year);
                    if (accountSystemDebitAcc == null)
                    {
                        var defaultAccountSystems = await _defaultAccountSystem.GetQueryableAsync();
                        accountSystemDebitAcc = (from a in defaultAccountSystems
                                                 where a.AccCode == productVoucherDetails[i].DebitAcc
                                                 select new AccountSystem
                                                 {
                                                     AccCode = a.AccCode,
                                                     AccPattern = a.AccPattern,
                                                     AccSectionCode = a.AccSectionCode,
                                                     AttachAccSection = a.AttachAccSection,
                                                     AttachContract = a.AttachContract,
                                                     AttachCurrency = a.AttachCurrency,
                                                     AttachPartner = a.AttachPartner,
                                                     AttachProductCost = a.AttachProductCost,
                                                     AttachWorkPlace = a.AttachWorkPlace,
                                                     IsBalanceSheetAcc = a.IsBalanceSheetAcc
                                                 }).FirstOrDefault();
                    }
                    if (accountSystemCreditAcc == null)
                    {
                        var defaultAccountSystems = await _defaultAccountSystem.GetQueryableAsync();
                        accountSystemCreditAcc = (from a in defaultAccountSystems
                                                  where a.AccCode == productVoucherDetails[i].CreditAcc
                                                  select new AccountSystem
                                                  {
                                                      AccCode = a.AccCode,
                                                      AccPattern = a.AccPattern,
                                                      AccSectionCode = a.AccSectionCode,
                                                      AttachAccSection = a.AttachAccSection,
                                                      AttachContract = a.AttachContract,
                                                      AttachCurrency = a.AttachCurrency,
                                                      AttachPartner = a.AttachPartner,
                                                      AttachProductCost = a.AttachProductCost,
                                                      AttachWorkPlace = a.AttachWorkPlace,
                                                      IsBalanceSheetAcc = a.IsBalanceSheetAcc

                                                  }).FirstOrDefault();
                    }
                    var AttachCurrency = "";
                    if (accountSystemCreditAcc != null)
                    {
                        AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                    }
                    CrudLedgerDto Ledger = new CrudLedgerDto();

                    Ledger.VoucherId = entity.Id;
                    Ledger.Ord0 = "C" + (i + 1).ToString().PadLeft(9, '0');
                    Ledger.Year = entity.Year;
                    Ledger.Ord0Extra = "C" + (i + 1).ToString().PadLeft(9, '0');
                    Ledger.OrgCode = productVoucherDetails[i].OrgCode;
                    Ledger.DepartmentCode = entity.DepartmentCode;
                    Ledger.VoucherCode = entity.VoucherCode;
                    Ledger.VoucherGroup = entity.VoucherGroup;
                    Ledger.BusinessCode = entity.BusinessCode;
                    Ledger.BusinessAcc = entity.BusinessAcc;
                    Ledger.VoucherNumber = entity.VoucherNumber;
                    Ledger.InvoiceNbr = entity.InvoiceNumber;
                    Ledger.VoucherDate = entity.VoucherDate;
                    Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                    Ledger.CurrencyCode = entity.CurrencyCode;
                    Ledger.ExchangeRate = entity.ExchangeRate;
                    Ledger.PartnerCode0 = entity.PartnerCode0;
                    Ledger.PartnerName0 = entity.PartnerName0;
                    Ledger.Representative = entity.Representative;
                    Ledger.Address = entity.Address;
                    Ledger.Description = entity.Description;
                    Ledger.DescriptionE = entity.DescriptionE;
                    Ledger.DebitAcc = productVoucherDetails[i].DebitAcc2;
                    Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                    Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                    Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? productVoucherDetails[i].AmountCur2 : 0;
                    Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                    Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                    Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                    Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                    Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                    Ledger.CreditAcc = productVoucherDetails[i].CreditAcc2;
                    Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                    Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                    Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                    Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                    Ledger.CreditAmountCur = 0;
                    Ledger.CreditAmount = 0;
                    Ledger.AmountCur = productVoucherDetails[i].AmountCur2;
                    Ledger.Amount = productVoucherDetails[i].Amount2;
                    Ledger.Note = entity.Description;
                    Ledger.NoteE = entity.DescriptionE;//
                    Ledger.CreditFProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                    Ledger.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                    Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                    Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                    Ledger.PartnerCode = entity.PartnerCode0;
                    Ledger.SectionCode = null;
                    Ledger.WorkPlaceCode = productVoucherDetails[i].WorkPlaceCode;
                    Ledger.CaseCode = productVoucherDetails[i].CaseCode;
                    Ledger.WarehouseCode = productVoucherDetails[i].WarehouseCode;
                    Ledger.TransWarehouseCode = productVoucherDetails[i].TransWarehouseCode;
                    Ledger.ProductCode = productVoucherDetails[i].ProductCode;
                    Ledger.ProductLotCode = productVoucherDetails[i].ProductLotCode;
                    Ledger.ProductOriginCode = productVoucherDetails[i].ProductOriginCode;
                    Ledger.UnitCode = productVoucherDetails[i].UnitCode;
                    Ledger.Quantity = productVoucherDetails[i].Quantity;
                    Ledger.PriceCur = productVoucherDetails[i].PriceCur2;
                    Ledger.Price = productVoucherDetails[i].Price2;
                    Ledger.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                    Ledger.InvoicePartnerName = entity.PartnerName0;
                    Ledger.InvoicePartnerAddress = entity.Address;
                    if (AccTaxDetails != null)
                    {
                        if (AccTaxDetails.Count > 0)
                        {
                            Ledger.VatPercentage = AccTaxDetails[0].VatPercentage;
                            Ledger.InvoiceNumber = AccTaxDetails[0].InvoiceNumber;
                            Ledger.InvoiceSymbol = AccTaxDetails[0].InvoiceSymbol;
                            Ledger.InvoiceDate = AccTaxDetails[0].InvoiceDate;
                            Ledger.TaxCode = AccTaxDetails[0].TaxCode;
                            Ledger.CheckDuplicate = AccTaxDetails[0].CheckDuplicate;
                            Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                        }
                    }
                    Ledger.DebitOrCredit = null;
                    Ledger.SalesChannelCode = entity.SalesChannelCode;
                    Ledger.ProductName0 = productVoucherDetails[i].ProductName0;
                    Ledger.Status = entity.Status;
                    Ledger.SectionCode = productVoucherDetails[i].SectionCode;
                    Ledger.ContractCode = productVoucherDetails[i].ContractCode;
                    Ledgers.Add(Ledger);

                }

            }
            if (entity.VoucherGroup == 3)
            {
                for (int i = 0; i < productVoucherDetails.Count(); i++)
                {
                    var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[i].DebitAcc, entity.OrgCode, entity.Year);
                    var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[i].CreditAcc, entity.OrgCode, entity.Year);
                    var AttachCurrency = "";

                    if (!string.IsNullOrEmpty(productVoucherDetails[i].ProductAcc))
                    {

                        accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherDetails[i].ProductAcc, entity.OrgCode, _webHelper.GetCurrentYear());

                    }

                    if (accountSystemCreditAcc != null)
                    {
                        AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                    }

                    CrudLedgerDto Ledger = new CrudLedgerDto();
                    Ledger.VoucherId = entity.Id;
                    Ledger.Ord0 = "B" + (i + 1).ToString().PadLeft(9, '0');
                    Ledger.Year = entity.Year;
                    Ledger.Ord0Extra = "B" + (i + 1).ToString().PadLeft(9, '0');
                    Ledger.OrgCode = entity.OrgCode;
                    Ledger.DepartmentCode = entity.DepartmentCode;
                    Ledger.VoucherCode = entity.VoucherCode;
                    Ledger.VoucherGroup = 2;
                    Ledger.BusinessCode = entity.BusinessCode;
                    Ledger.BusinessAcc = entity.BusinessAcc;
                    Ledger.VoucherNumber = entity.VoucherNumber;
                    Ledger.InvoiceNbr = entity.InvoiceNumber;
                    Ledger.VoucherDate = entity.VoucherDate;
                    Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                    Ledger.CurrencyCode = entity.CurrencyCode;
                    Ledger.ExchangeRate = entity.ExchangeRate;
                    Ledger.PartnerCode0 = entity.PartnerCode0;
                    Ledger.PartnerName0 = entity.PartnerName0;
                    Ledger.Representative = entity.Representative;
                    Ledger.Address = entity.Address;
                    Ledger.Description = entity.Description;
                    Ledger.DescriptionE = entity.DescriptionE;
                    Ledger.DebitAcc = productVoucherDetails[i].DebitAcc;
                    Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                    Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                    Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? productVoucherDetails[i].AmountCur - productVoucherDetails[i].DiscountAmountCur : 0;
                    Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                    Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                    Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                    Ledger.DebitFProductWorkCode = accountSystemDebitAcc.AttachProductCost == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                    Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                    Ledger.CreditAcc = productVoucherDetails[i].ProductAcc;
                    Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                    Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                    Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                    Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                    Ledger.CreditAmountCur = AttachCurrency == "C" ? productVoucherDetails[i].AmountCur - productVoucherDetails[i].DiscountAmountCur : 0;
                    Ledger.CreditAmount = AttachCurrency == "C" ? productVoucherDetails[i].Amount - productVoucherDetails[i].DiscountAmount : 0;
                    Ledger.AmountCur = productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExpenseAmountCur0 + productVoucherDetails[i].ExpenseAmountCur1 - productVoucherDetails[i].DiscountAmountCur;
                    Ledger.Amount = productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount0 + productVoucherDetails[i].ExpenseAmount1 - productVoucherDetails[i].DiscountAmount;
                    Ledger.Note = entity.Description;
                    Ledger.NoteE = entity.DescriptionE;
                    Ledger.CreditFProductWorkCode = accountSystemCreditAcc.AttachProductCost == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                    Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                    Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                    Ledger.CaseCode = productVoucherDetails[i].CaseCode;
                    Ledger.WarehouseCode = productVoucherDetails[i].WarehouseCode;
                    Ledger.TransWarehouseCode = productVoucherDetails[i].TransWarehouseCode;
                    Ledger.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                    Ledger.ProductCode = productVoucherDetails[i].ProductCode;
                    Ledger.ProductLotCode = productVoucherDetails[i].ProductLotCode;
                    Ledger.ProductOriginCode = productVoucherDetails[i].ProductOriginCode;
                    Ledger.UnitCode = productVoucherDetails[i].UnitCode;
                    Ledger.Quantity = productVoucherDetails[i].Quantity;
                    Ledger.PriceCur = (productVoucherDetails[i].AmountCur + productVoucherDetails[i].ExpenseAmountCur0 + productVoucherDetails[i].ExpenseAmountCur1 - productVoucherDetails[i].DiscountAmountCur) / productVoucherDetails[i].Quantity;
                    Ledger.Price = (productVoucherDetails[i].Amount + productVoucherDetails[i].ExpenseAmount0 + productVoucherDetails[i].ExpenseAmount1 - productVoucherDetails[i].DiscountAmount) / productVoucherDetails[i].Quantity;
                    Ledger.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                    if (AccTaxDetails != null)
                    {
                        if (AccTaxDetails.Count > 0)
                        {
                            Ledger.VatPercentage = AccTaxDetails[0].VatPercentage;
                            Ledger.InvoiceNumber = AccTaxDetails[0].InvoiceNumber;
                            Ledger.InvoiceSymbol = AccTaxDetails[0].InvoiceSymbol;
                            Ledger.InvoiceDate = AccTaxDetails[0].InvoiceDate;
                            Ledger.TaxCode = AccTaxDetails[0].TaxCode;
                            Ledger.CheckDuplicate = AccTaxDetails[0].CheckDuplicate;
                            Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                        }

                    }

                    Ledger.DebitOrCredit = null;
                    Ledger.SalesChannelCode = entity.SalesChannelCode;
                    Ledger.ProductName0 = productVoucherDetails[i].ProductName0;
                    Ledger.PartnerCode = entity.PartnerCode ?? entity.PartnerCode0;
                    Ledger.SectionCode = productVoucherDetails[i].SectionCode;
                    Ledger.Status = entity.Status;
                    Ledger.ContractCode = productVoucherDetails[i].ContractCode;
                    Ledgers.Add(Ledger);

                }
            }
            if (AccTaxDetails != null)
            {
                if (AccTaxDetails.Count > 0)
                {
                    for (int i = 0; i < AccTaxDetails.Count; i++) // i = 1
                    {

                        var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, AccTaxDetails[0].DebitAcc, entity.OrgCode, entity.Year);
                        var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, AccTaxDetails[0].CreditAcc, entity.OrgCode, entity.Year);
                        var AttachCurrency = "";
                        if (accountSystemCreditAcc != null)
                        {
                            AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                        }
                        var taxCategoryCode = await _taxCategoryService.GetTaxByCodeAsync(AccTaxDetails[i].TaxCategoryCode, entity.OrgCode);
                        if (VHT_KHU_TRUNG_HV.Contains(AccTaxDetails[i].DebitAcc.Left(3)) == false && VHT_KHU_TRUNG_HV.Contains(AccTaxDetails[i].CreditAcc.Left(3)) == false)
                        {
                            CrudLedgerDto Ledger = new CrudLedgerDto();

                            Ledger.VoucherId = entity.Id;
                            Ledger.Ord0 = AccTaxDetails[i].Ord0;
                            Ledger.Year = entity.Year;
                            Ledger.Ord0Extra = AccTaxDetails[i].Ord0;
                            Ledger.OrgCode = AccTaxDetails[i].OrgCode;
                            Ledger.DepartmentCode = entity.DepartmentCode;
                            Ledger.VoucherCode = entity.VoucherCode;
                            Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                            Ledger.BusinessCode = entity.BusinessCode;
                            Ledger.BusinessAcc = entity.BusinessAcc;
                            Ledger.CheckDuplicate = AccTaxDetails[i].CheckDuplicate;
                            Ledger.VoucherNumber = entity.VoucherNumber;
                            Ledger.InvoiceNbr = entity.InvoiceNumber;
                            Ledger.VoucherDate = entity.VoucherDate;
                            Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                            Ledger.CurrencyCode = entity.CurrencyCode;
                            Ledger.ExchangeRate = entity.ExchangeRate;
                            Ledger.PartnerCode0 = AccTaxDetails[i].PartnerCode;
                            Ledger.PartnerName0 = AccTaxDetails[i].PartnerName;
                            Ledger.Representative = entity.Representative;
                            Ledger.Address = entity.Address;
                            Ledger.Description = entity.Description;
                            Ledger.DescriptionE = entity.DescriptionE;
                            Ledger.DebitAcc = AccTaxDetails[i].DebitAcc;
                            Ledger.CreditAcc = AccTaxDetails[i].CreditAcc;
                            Ledger.CreditAmountCur = 0;
                            Ledger.CreditAmount = 0;
                            Ledger.Amount = AccTaxDetails[i].Amount;
                            Ledger.AmountCur = AccTaxDetails[i].AmountCur;
                            Ledger.Note = AccTaxDetails[i].Note;
                            Ledger.NoteE = AccTaxDetails[i].NoteE;
                            Ledger.PartnerCode = AccTaxDetails[i].PartnerCode;
                            Ledger.InvoicePartnerAddress = entity.Address;
                            Ledger.InvoicePartnerName = entity.PartnerName0;
                            Ledger.SectionCode = null;
                            Ledger.WorkPlaceCode = null;
                            Ledger.CaseCode = AccTaxDetails[i].CaseCode;
                            Ledger.WarehouseCode = null;
                            Ledger.TransWarehouseCode = null;
                            Ledger.ProductCode = null;
                            Ledger.ProductLotCode = null;
                            Ledger.ProductOriginCode = null;
                            Ledger.UnitCode = null;
                            Ledger.Quantity = 0;
                            Ledger.PriceCur = 0;
                            Ledger.Price = 0;
                            Ledger.TaxCategoryCode = AccTaxDetails[i].TaxCategoryCode;
                            Ledger.VatPercentage = AccTaxDetails[i].VatPercentage;
                            Ledger.InvoiceNumber = entity.InvoiceNumber;
                            Ledger.InvoiceSymbol = AccTaxDetails[i].InvoiceSymbol;
                            Ledger.InvoiceDate = AccTaxDetails[i].InvoiceDate;
                            Ledger.TaxCode = AccTaxDetails[i].TaxCode;
                            Ledger.DebitOrCredit = null;
                            Ledger.SalesChannelCode = entity.SalesChannelCode;
                            Ledger.ProductName0 = AccTaxDetails[i].ProductName;
                            Ledger.SecurityNo = AccTaxDetails[i].SecurityNo;
                            Ledger.Status = entity.Status;
                            Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                            Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                            Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? AccTaxDetails[i].AmountCur : 0;
                            Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                            Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? AccTaxDetails[i].ContractCode : "";
                            Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? AccTaxDetails[i].SectionCode : "";
                            Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? AccTaxDetails[i].FProductWorkCode : "";
                            Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? AccTaxDetails[i].WorkPlaceCode : "";
                            Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                            Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                            Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? AccTaxDetails[i].PartnerCode : "";
                            Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? AccTaxDetails[i].ContractCode : "";
                            Ledger.CreditAmountCur = accountSystemCreditAcc.AttachCurrency == "C" ? AccTaxDetails[i].AmountCur : 0;
                            Ledger.CreditAmount = accountSystemCreditAcc.AttachCurrency == "C" ? AccTaxDetails[i].Amount : 0;
                            Ledger.CreditFProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? AccTaxDetails[i].FProductWorkCode : "";
                            Ledger.FProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? AccTaxDetails[i].FProductWorkCode : "";
                            Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? AccTaxDetails[i].SectionCode : "";
                            Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? AccTaxDetails[i].WorkPlaceCode : "";
                            Ledger.ContractCode = AccTaxDetails[i].ContractCode;
                            Ledgers.Add(Ledger);

                        }


                    }
                }
            }

            List<CrudProductVoucherCostDto> ProductVoucherCost = (List<CrudProductVoucherCostDto>)entity.ProductVoucherCostDetails;
            if (ProductVoucherCost != null)
            {
                for (int i = 0; i < ProductVoucherCost.Count; i++)
                {
                    var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, ProductVoucherCost[0].DebitAcc, entity.OrgCode, entity.Year);
                    var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, ProductVoucherCost[0].CreditAcc, entity.OrgCode, entity.Year);
                    var AttachCurrency = "";
                    if (accountSystemCreditAcc != null)
                    {
                        AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                    }
                    if (VHT_KHU_TRUNG_HV.Contains(ProductVoucherCost[i].DebitAcc.Left(3)) == false
                            && VHT_KHU_TRUNG_HV.Contains(ProductVoucherCost[i].CreditAcc.Left(3)) == false)
                    {
                        CrudLedgerDto Ledger = new CrudLedgerDto();

                        Ledger.VoucherId = entity.Id;
                        Ledger.Ord0 = ProductVoucherCost[i].Ord0;
                        Ledger.Year = entity.Year;
                        Ledger.Ord0Extra = ProductVoucherCost[i].Ord0;
                        Ledger.OrgCode = ProductVoucherCost[i].OrgCode;
                        Ledger.DepartmentCode = entity.DepartmentCode;
                        Ledger.VoucherCode = entity.VoucherCode;
                        Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                        Ledger.BusinessCode = entity.BusinessCode;
                        Ledger.BusinessAcc = entity.BusinessAcc;
                        Ledger.VoucherNumber = entity.VoucherNumber;
                        Ledger.InvoiceNbr = entity.InvoiceNumber;
                        Ledger.VoucherDate = entity.VoucherDate;
                        Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                        Ledger.CurrencyCode = entity.CurrencyCode;
                        Ledger.ExchangeRate = entity.ExchangeRate;
                        Ledger.PartnerCode0 = ProductVoucherCost[i].PartnerCode;
                        Ledger.PartnerName0 = null;//entity.PartnerName0;
                        Ledger.Representative = entity.Representative;
                        Ledger.Address = entity.Address;
                        Ledger.Description = entity.Description;
                        Ledger.DescriptionE = entity.DescriptionE;
                        Ledger.DebitAcc = ProductVoucherCost[i].DebitAcc;
                        Ledger.CreditAcc = ProductVoucherCost[i].CreditAcc;
                        Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                        Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                        Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? ProductVoucherCost[i].AmountCur : 0;
                        Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? ProductVoucherCost[i].PartnerCode : "";
                        Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? ProductVoucherCost[i].ContractCode : "";
                        Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? ProductVoucherCost[i].SectionCode : "";
                        Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? ProductVoucherCost[i].FProductWorkCode : "";
                        Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? ProductVoucherCost[i].WorkPlaceCode : "";
                        Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                        Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                        Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? ProductVoucherCost[i].PartnerCode : "";
                        Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? ProductVoucherCost[i].ContractCode : "";
                        Ledger.AmountCur = AttachCurrency == "C" ? ProductVoucherCost[i].AmountCur : 0;
                        Ledger.CreditFProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? ProductVoucherCost[i].FProductWorkCode : "";
                        Ledger.FProductWorkCode = ProductVoucherCost[i].FProductWorkCode;
                        Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? ProductVoucherCost[i].SectionCode : "";
                        Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? ProductVoucherCost[i].WorkPlaceCode : "";
                        Ledger.CreditAmountCur = accountSystemCreditAcc.AttachCurrency == "C" ? ProductVoucherCost[i].AmountCur : 0;
                        Ledger.CreditAmount = accountSystemCreditAcc.AttachCurrency == "C" ? ProductVoucherCost[i].Amount : 0;
                        Ledger.AmountCur = ProductVoucherCost[i].AmountCur;
                        Ledger.Amount = ProductVoucherCost[i].Amount;
                        Ledger.Note = ProductVoucherCost[i].Note;
                        Ledger.NoteE = null;
                        Ledger.FProductWorkCode = ProductVoucherCost[i].FProductWorkCode;
                        Ledger.PartnerCode = ProductVoucherCost[i].PartnerCode;
                        Ledger.SectionCode = null;
                        Ledger.WorkPlaceCode = ProductVoucherCost[i].WorkPlaceCode;
                        Ledger.CaseCode = ProductVoucherCost[i].CaseCode;
                        Ledger.Price = 0;
                        if (AccTaxDetails != null)
                        {
                            if (AccTaxDetails.Count > 0)
                            {
                                Ledger.CheckDuplicate = AccTaxDetails[0].CheckDuplicate;
                                Ledger.VatPercentage = AccTaxDetails[0].VatPercentage;
                                Ledger.InvoiceNumber = AccTaxDetails[0].InvoiceNumber;
                                Ledger.InvoiceSymbol = AccTaxDetails[0].InvoiceSymbol;
                                Ledger.InvoiceDate = AccTaxDetails.Count != 0 ? AccTaxDetails[0].InvoiceDate : (DateTime?)null;
                                Ledger.TaxCode = AccTaxDetails[0].TaxCode;
                                Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                            }

                        }

                        Ledger.DebitOrCredit = null;
                        Ledger.SalesChannelCode = entity.SalesChannelCode;
                        Ledger.Status = entity.Status;
                        Ledgers.Add(Ledger);

                    }


                }
            }

            List<CrudProductVoucherReceiptDto> productVoucherReceipts = (List<CrudProductVoucherReceiptDto>)entity.ProductVoucherReceipts;
            if (productVoucherReceipts != null)
            {
                foreach (var item in productVoucherReceipts)
                {
                    var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, item.DiscountDebitAcc, entity.OrgCode, entity.Year);
                    var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, item.DiscountCreditAcc, entity.OrgCode, entity.Year);
                    var AttachCurrency = "";
                    if (accountSystemCreditAcc != null)
                    {
                        AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                    }
                    if (entity.TotalDiscountAmount > 0
                        && item.DiscountDebitAcc + item.DiscountCreditAcc != ""
                        && VHT_KHU_TRUNG_HV.Contains(item.DiscountCreditAcc.Left(3)) == false
                        &&
                        VHT_KHU_TRUNG_HV.Contains(item.DiscountDebitAcc.Left(3)) == false)
                    {

                        for (int i = 0; i < productVoucherDetails.Count; i++)
                        {
                            if (i < 1)
                            {
                                List<CrudProductVoucherDetailReceiptDto> ProductVoucherDetailReceipt = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[i].ProductVoucherDetailReceipts;
                                CrudLedgerDto Ledger = new CrudLedgerDto();

                                Ledger.VoucherId = entity.Id;
                                Ledger.Ord0 = "K000000001";
                                Ledger.Year = entity.Year;
                                Ledger.Ord0Extra = "K000000001";
                                Ledger.OrgCode = productVoucherDetails[i].OrgCode;
                                Ledger.DepartmentCode = entity.DepartmentCode;
                                Ledger.VoucherCode = entity.VoucherCode;
                                Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                                Ledger.BusinessCode = entity.BusinessCode;
                                Ledger.BusinessAcc = entity.BusinessAcc;
                                Ledger.VoucherNumber = entity.VoucherNumber;
                                Ledger.InvoiceNbr = entity.InvoiceNumber;
                                Ledger.VoucherDate = entity.VoucherDate;
                                Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                                Ledger.CurrencyCode = entity.CurrencyCode;
                                Ledger.ExchangeRate = entity.ExchangeRate;
                                Ledger.PartnerCode0 = entity.PartnerCode0;
                                Ledger.PartnerName0 = entity.PartnerName0;
                                Ledger.Representative = entity.Representative;
                                Ledger.Address = entity.Address;
                                Ledger.Description = entity.Description;
                                Ledger.DescriptionE = entity.DescriptionE;
                                Ledger.DebitAcc = productVoucherReceipts[i].DiscountDebitAcc;
                                Ledger.CreditAcc = productVoucherReceipts[i].DiscountCreditAcc;
                                Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? entity.TotalDiscountAmountCur : 0;
                                Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                                Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                                Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                                Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                                Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                                Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                                Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                                Ledger.AmountCur = AttachCurrency == "C" ? productVoucherDetails[i].AmountCur : 0;
                                Ledger.CreditFProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                                Ledger.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                                Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                                Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                                Ledger.CreditAmountCur = accountSystemCreditAcc.AttachCurrency == "C" ? entity.TotalDiscountAmountCur : 0;
                                Ledger.CreditAmount = accountSystemCreditAcc.AttachCurrency == "C" ? entity.TotalDiscountAmount : 0;
                                Ledger.AmountCur = entity.TotalDiscountAmountCur;
                                Ledger.Amount = entity.TotalDiscountAmount;
                                Ledger.Note = "Chiết khấu thương mại";
                                Ledger.NoteE = null;
                                Ledger.Price = 0;
                                Ledger.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                                Ledger.PartnerCode = entity.PartnerCode0;
                                Ledger.DebitOrCredit = null;
                                Ledger.SalesChannelCode = entity.SalesChannelCode;
                                Ledger.ProductName0 = productVoucherDetails[i].ProductName0;
                                if (AccTaxDetails != null)
                                {
                                    if (AccTaxDetails.Count > 0)
                                    {
                                        Ledger.CheckDuplicate = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].CheckDuplicate;
                                        Ledger.VatPercentage = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].VatPercentage;
                                        Ledger.InvoiceNumber = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].InvoiceNumber;
                                        Ledger.InvoiceSymbol = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].InvoiceSymbol;
                                        Ledger.InvoiceDate = AccTaxDetails.Count != 0 ? AccTaxDetails[0].InvoiceDate : (DateTime?)null;
                                        Ledger.TaxCode = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].TaxCode;
                                        Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                                    }

                                }

                                Ledger.Status = entity.Status;
                                Ledgers.Add(Ledger);
                                break;
                            }

                        }
                    }
                    for (int m = 0; m < productVoucherReceipts.Count; m++)
                    {
                        if (productVoucherReceipts[m].DiscountAmount0 > 0
                            && item.DiscountCreditAcc0 + item.DiscountDebitAcc0 != ""
                            && VHT_KHU_TRUNG_HV.Contains(item.DiscountCreditAcc.Left(3)) == false
                            && VHT_KHU_TRUNG_HV.Contains(item.DiscountDebitAcc.Left(3)) == false)
                        {
                            for (int i = 0; i < productVoucherDetails.Count; i++)
                            {
                                if (i < 1)
                                {

                                    if (accountSystemCreditAcc != null)
                                    {
                                        AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                                    }
                                    List<CrudProductVoucherDetailReceiptDto> ProductVoucherDetailReceipt = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[i].ProductVoucherDetailReceipts;
                                    CrudLedgerDto Ledger = new CrudLedgerDto();

                                    Ledger.VoucherId = entity.Id;
                                    Ledger.Ord0 = "K000000002";
                                    Ledger.Year = entity.Year;
                                    Ledger.Ord0Extra = "K000000002";
                                    Ledger.OrgCode = productVoucherDetails[i].OrgCode;
                                    Ledger.DepartmentCode = entity.DepartmentCode;
                                    Ledger.VoucherCode = entity.VoucherCode;
                                    Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                                    Ledger.BusinessCode = entity.BusinessCode;
                                    Ledger.BusinessAcc = entity.BusinessAcc;
                                    Ledger.VoucherNumber = entity.VoucherNumber;
                                    Ledger.InvoiceNbr = entity.InvoiceNumber;
                                    Ledger.VoucherDate = entity.VoucherDate;
                                    Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                                    Ledger.CurrencyCode = entity.CurrencyCode;
                                    Ledger.ExchangeRate = entity.ExchangeRate;
                                    Ledger.PartnerCode0 = entity.PartnerCode0;
                                    Ledger.PartnerName0 = entity.PartnerName0;
                                    Ledger.Representative = entity.Representative;
                                    Ledger.Address = entity.Address;
                                    Ledger.Description = entity.Description;
                                    Ledger.DescriptionE = entity.DescriptionE;
                                    Ledger.DebitAcc = productVoucherReceipts[i].DiscountDebitAcc0;
                                    Ledger.CreditAcc = productVoucherReceipts[i].DiscountCreditAcc0;
                                    Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                    Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                    Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? entity.DiscountAmountCur : 0;
                                    Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                                    Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                                    Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                                    Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                                    Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                                    // Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? productVoucherDetails[i].AmountCur : 0;
                                    Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                    Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                    Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                                    Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? productVoucherDetails[i].ContractCode : "";
                                    Ledger.AmountCur = AttachCurrency == "C" ? productVoucherDetails[i].AmountCur : 0;
                                    Ledger.CreditFProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? productVoucherDetails[i].FProductWorkCode : "";
                                    Ledger.FProductWorkCode = productVoucherDetails[i].FProductWorkCode;
                                    Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? productVoucherDetails[i].SectionCode : "";
                                    Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? productVoucherDetails[i].WorkPlaceCode : "";
                                    Ledger.CreditAmountCur = accountSystemCreditAcc.AttachCurrency == "C" ? entity.DiscountAmountCur : 0;
                                    Ledger.CreditAmount = accountSystemCreditAcc.AttachCurrency == "C" ? entity.DiscountAmount : 0;
                                    Ledger.AmountCur = entity.DiscountAmountCur;
                                    Ledger.Amount = entity.DiscountAmount;
                                    Ledger.Note = "Chiết khấu thanh toán";
                                    Ledger.NoteE = null;
                                    Ledger.Price = 0;
                                    Ledger.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                                    if (AccTaxDetails != null)
                                    {
                                        if (AccTaxDetails.Count > 0)
                                        {
                                            Ledger.VatPercentage = AccTaxDetails.Count() == 0 ? null : AccTaxDetails[0].VatPercentage;
                                            Ledger.InvoiceNumber = AccTaxDetails.Count() == 0 ? null : AccTaxDetails[0].InvoiceNumber;
                                            Ledger.InvoiceSymbol = AccTaxDetails.Count() == 0 ? null : AccTaxDetails[0].InvoiceSymbol;
                                            Ledger.InvoiceDate = AccTaxDetails.Count() != 0 ? AccTaxDetails[0].InvoiceDate : (DateTime?)null;
                                            Ledger.TaxCode = AccTaxDetails.Count() == 0 ? null : AccTaxDetails[0].TaxCode;
                                            Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                                            Ledger.CheckDuplicate = AccTaxDetails.Count() == 0 ? null : AccTaxDetails[0].CheckDuplicate;
                                        }

                                    }

                                    Ledger.DebitOrCredit = null;
                                    Ledger.SalesChannelCode = entity.SalesChannelCode;
                                    Ledger.ProductName0 = productVoucherDetails[i].ProductName0;
                                    Ledger.Status = entity.Status;
                                    Ledgers.Add(Ledger);
                                    break;

                                }
                            }
                        }

                    }

                }
            }

            var sumProductVoucherDetail = (from a in productVoucherDetails
                                           group new
                                           {
                                               a.DebitAcc,
                                               a.ExciseTaxAmount,
                                               a.ExciseTaxAmountCur,
                                               a.AmountCur,
                                               a.ContractCode,
                                               a.SectionCode,
                                               a.FProductWorkCode,
                                               a.WorkPlaceCode,
                                               a.TaxCategoryCode,
                                               a.ProductName0,
                                               a.ImportTaxAmount,
                                               a.ImportTaxAmountCur,
                                               a.Amount,
                                               a.PartnerCode,
                                               a.CaseCode,
                                               a.DebitAcc2
                                           } by new
                                           {
                                               a.DebitAcc
                                           } into gr

                                           select new
                                           {
                                               DebitAcc = gr.Key.DebitAcc,
                                               ExciseTaxAmount = gr.Sum(p => p.ExciseTaxAmount) ?? 0,
                                               ExciseTaxAmountCur = gr.Sum(p => p.ExciseTaxAmountCur) ?? 0,
                                               AmountCur = gr.Sum(p => p.AmountCur) ?? 0,
                                               ContractCode = gr.Max(p => p.ContractCode),
                                               SectionCode = gr.Max(p => p.SectionCode),
                                               FProductWorkCode = gr.Max(p => p.FProductWorkCode),
                                               WorkPlaceCode = gr.Max(p => p.WorkPlaceCode),
                                               TaxCategoryCode = gr.Max(p => p.TaxCategoryCode),
                                               ProductName0 = gr.Max(p => p.ProductName0),
                                               ImportTaxAmount = gr.Sum(p => p.ImportTaxAmount),
                                               ImportTaxAmountCur = gr.Sum(p => p.ImportTaxAmountCur),
                                               Amount = gr.Sum(p => p.Amount) ?? 0,
                                               PartnerCode = gr.Max(p => p.PartnerCode),
                                               CaseCode = gr.Max(p => p.CaseCode),
                                               DebitAcc2 = gr.Max(p => p.DebitAcc2)
                                           }).ToList();
            for (int i = 0; i < sumProductVoucherDetail.Count; i++)
            {
                if (productVoucherReceipts != null)
                {
                    if (productVoucherReceipts.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(productVoucherReceipts[0].ImportCreditAcc) && !string.IsNullOrEmpty(sumProductVoucherDetail[i].DebitAcc))
                        {
                            var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, sumProductVoucherDetail[i].DebitAcc, entity.OrgCode, entity.Year);
                            var accountSystemCreditAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherReceipts[0].ImportCreditAcc, entity.OrgCode, entity.Year);
                            var AttachCurrency = "";
                            if (accountSystemCreditAcc != null)
                            {
                                AttachCurrency = accountSystemCreditAcc.AttachCurrency;
                            }
                            if (
                            VHT_KHU_TRUNG_HV.Contains(productVoucherDetails[0].DebitAcc.Left(3)) == false
                            &&
                            VHT_KHU_TRUNG_HV.Contains(productVoucherReceipts[0].ImportCreditAcc.Left(3)) == false
                            )
                            {
                                CrudLedgerDto Ledger = new CrudLedgerDto();
                                Ledger.VoucherId = entity.Id;
                                Ledger.Ord0 = "NK" + (i + 1).ToString().PadLeft(8, '0');
                                Ledger.Year = entity.Year;
                                Ledger.Ord0Extra = "NK" + (i + 1).ToString().PadLeft(8, '0');
                                Ledger.OrgCode = productVoucherDetails[i].OrgCode;
                                Ledger.DepartmentCode = entity.DepartmentCode;
                                Ledger.VoucherCode = entity.VoucherCode;
                                Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                                Ledger.BusinessCode = entity.BusinessCode;
                                Ledger.BusinessAcc = entity.BusinessAcc;
                                Ledger.VoucherNumber = entity.VoucherNumber;
                                Ledger.InvoiceNbr = entity.InvoiceNumber;
                                Ledger.VoucherDate = entity.VoucherDate;
                                Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                                Ledger.CurrencyCode = entity.CurrencyCode;
                                Ledger.ExchangeRate = entity.ExchangeRate;
                                Ledger.PartnerCode0 = entity.PartnerCode0;
                                Ledger.PartnerName0 = entity.PartnerName0;
                                Ledger.Representative = entity.Representative;
                                Ledger.Address = entity.Address;
                                Ledger.Description = entity.Description;
                                Ledger.DescriptionE = entity.DescriptionE;
                                Ledger.DebitAcc = sumProductVoucherDetail[i].DebitAcc;
                                Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? sumProductVoucherDetail[i].AmountCur : 0;
                                Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                                Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? sumProductVoucherDetail[i].ContractCode : "";
                                Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? sumProductVoucherDetail[i].SectionCode : "";
                                Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? sumProductVoucherDetail[i].FProductWorkCode : "";
                                Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? sumProductVoucherDetail[i].WorkPlaceCode : "";
                                Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                Ledger.CreditPartnerCode = accountSystemCreditAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                                Ledger.CreditContractCode = accountSystemCreditAcc.AttachContract == "C" ? sumProductVoucherDetail[i].ContractCode : "";
                                Ledger.CreditAcc = entity.ImportCreditAcc;
                                //Ledger.AmountCur = AttachCurrency == "C" ? sumProductVoucherDetail[i].AmountCur : 0;
                                Ledger.CreditFProductWorkCode = accountSystemCreditAcc.IsBalanceSheetAcc == "C" ? sumProductVoucherDetail[i].FProductWorkCode : "";
                                Ledger.FProductWorkCode = sumProductVoucherDetail[i].FProductWorkCode;
                                Ledger.CreditSectionCode = accountSystemCreditAcc.AttachAccSection == "C" ? sumProductVoucherDetail[i].SectionCode : "";
                                Ledger.CreditWorkPlaceCode = accountSystemCreditAcc.AttachWorkPlace == "C" ? sumProductVoucherDetail[i].WorkPlaceCode : "";
                                Ledger.CreditAmountCur = accountSystemCreditAcc.AttachCurrency == "C" ? sumProductVoucherDetail[i].AmountCur : 0;
                                Ledger.CreditAmount = accountSystemCreditAcc.AttachCurrency == "C" ? sumProductVoucherDetail[i].Amount : 0;
                                Ledger.AmountCur = sumProductVoucherDetail[i].ImportTaxAmountCur;
                                Ledger.Amount = sumProductVoucherDetail[i].ImportTaxAmount;
                                Ledger.Note = "Thuế xuất, nhập khẩu";
                                Ledger.NoteE = null;
                                Ledger.FProductWorkCode = sumProductVoucherDetail[i].FProductWorkCode;
                                Ledger.SectionCode = null;
                                Ledger.WorkPlaceCode = sumProductVoucherDetail[i].WorkPlaceCode;
                                Ledger.CaseCode = sumProductVoucherDetail[i].CaseCode;
                                Ledger.PartnerCode = entity.PartnerCode0;
                                if (AccTaxDetails != null)
                                {
                                    if (AccTaxDetails.Count > 0)
                                    {
                                        Ledger.CheckDuplicate = AccTaxDetails[0].CheckDuplicate;
                                        Ledger.VatPercentage = AccTaxDetails[0].VatPercentage;
                                        Ledger.InvoiceNumber = AccTaxDetails[0].InvoiceNumber;
                                        Ledger.InvoiceSymbol = AccTaxDetails[0].InvoiceSymbol;
                                        Ledger.InvoiceDate = AccTaxDetails[0].InvoiceDate;
                                        Ledger.TaxCode = AccTaxDetails[0].TaxCode;
                                        Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;
                                    }
                                }

                                Ledger.DebitOrCredit = null;
                                Ledger.SalesChannelCode = entity.SalesChannelCode;
                                Ledger.ProductName0 = productVoucherDetails[i].ProductName0;

                                Ledger.Status = entity.Status;
                                Ledgers.Add(Ledger);

                            }
                        }


                    }
                }

            }

            if (entity.VoucherGroup.ToString() == "1" && VHT_TTDB_VALUE != "C")
            {
                for (int i = 0; i < sumProductVoucherDetail.Count; i++)
                {
                    var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, sumProductVoucherDetail[i].DebitAcc, entity.OrgCode, entity.Year);
                    var accountSystemCreditAcc = "";
                    var AttachPartner = "";
                    var AttachContract = "";
                    var AttachAccSection = "";
                    var IsBalanceSheetAcc = "";
                    var AttachWorkPlace = "";
                    if (productVoucherReceipts != null && productVoucherReceipts.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(productVoucherReceipts[0].ExciseTaxCreditAcc))
                        {
                            var accountSystemCreditAccs = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherReceipts[0].ExciseTaxCreditAcc, entity.OrgCode, entity.Year);
                            accountSystemCreditAcc = accountSystemCreditAccs.AttachCurrency;
                            AttachPartner = accountSystemCreditAccs.AttachPartner;
                            AttachContract = accountSystemCreditAccs.AttachContract;
                            AttachAccSection = accountSystemCreditAccs.AttachAccSection;
                            IsBalanceSheetAcc = accountSystemCreditAccs.IsBalanceSheetAcc;
                            AttachWorkPlace = accountSystemCreditAccs.AttachWorkPlace;
                            var AttachCurrency = "";
                            if (accountSystemCreditAcc != null)
                            {
                                AttachCurrency = accountSystemCreditAcc;
                            }
                            if ((productVoucherReceipts.Count == 0 ? null : productVoucherReceipts[0].ExciseTaxCreditAcc) != ""
                           && productVoucherDetails[i].DebitAcc != null
                           && (productVoucherReceipts.Count == 0 ? null : productVoucherReceipts[0].ExciseTaxCreditAcc) != ""
                           && VHT_KHU_TRUNG_HV.Contains(productVoucherReceipts[0].ExciseTaxCreditAcc.Left(3)) == false
                           )
                            {
                                CrudLedgerDto Ledger = new CrudLedgerDto();
                                Ledger.VoucherId = entity.Id;
                                Ledger.Ord0 = "DB" + (i + 1).ToString().PadLeft(8, '0');
                                Ledger.Year = entity.Year;
                                Ledger.Ord0Extra = "DB" + (i + 1).ToString().PadLeft(8, '0');
                                Ledger.OrgCode = entity.OrgCode;
                                Ledger.DepartmentCode = entity.DepartmentCode;
                                Ledger.VoucherCode = entity.VoucherCode;
                                Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                                Ledger.BusinessCode = entity.BusinessCode;
                                Ledger.BusinessAcc = entity.BusinessAcc;
                                Ledger.VoucherNumber = entity.VoucherNumber;
                                Ledger.InvoiceNbr = entity.InvoiceNumber;
                                Ledger.VoucherDate = entity.VoucherDate;
                                Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                                Ledger.CurrencyCode = entity.CurrencyCode;
                                Ledger.ExchangeRate = entity.ExchangeRate;
                                Ledger.PartnerCode0 = entity.PartnerCode0;
                                Ledger.PartnerName0 = entity.PartnerName0;
                                Ledger.Representative = entity.Representative;
                                Ledger.Address = entity.Address;
                                Ledger.Description = entity.Description;
                                Ledger.DescriptionE = entity.DescriptionE;
                                Ledger.DebitAcc = sumProductVoucherDetail[i].DebitAcc;
                                Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                Ledger.DebitAmountCur = accountSystemDebitAcc.AttachCurrency == "C" ? sumProductVoucherDetail[i].AmountCur : 0;
                                Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";//
                                Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? sumProductVoucherDetail[i].ContractCode : "";
                                Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? sumProductVoucherDetail[i].SectionCode : "";
                                Ledger.DebitFProductWorkCode = accountSystemDebitAcc.IsBalanceSheetAcc == "C" ? sumProductVoucherDetail[i].FProductWorkCode : "";
                                Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? sumProductVoucherDetail[i].WorkPlaceCode : "";
                                Ledger.CreditCurrencyCode = AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                Ledger.CreditExchangeRate = AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                Ledger.CreditPartnerCode = accountSystemCreditAccs.AttachPartner == "C" ? entity.PartnerCode0 : "";
                                Ledger.CreditContractCode = accountSystemCreditAccs.AttachContract == "C" ? sumProductVoucherDetail[i].ContractCode : "";
                                Ledger.CreditAmountCur = accountSystemCreditAccs.AttachCurrency == "C" ? sumProductVoucherDetail[i].AmountCur : 0;
                                Ledger.CreditAcc = entity.ExciseTaxCreditAcc;
                                // Ledger.AmountCur = AttachCurrency == "C" ? productVoucherDetails[i].AmountCur : 0;
                                Ledger.CreditFProductWorkCode = accountSystemCreditAccs.IsBalanceSheetAcc == "C" ? sumProductVoucherDetail[i].FProductWorkCode : "";
                                Ledger.FProductWorkCode = sumProductVoucherDetail[i].FProductWorkCode;
                                Ledger.CreditSectionCode = accountSystemCreditAccs.AttachAccSection == "C" ? sumProductVoucherDetail[i].SectionCode : "";
                                Ledger.CreditWorkPlaceCode = accountSystemCreditAccs.AttachWorkPlace == "C" ? sumProductVoucherDetail[i].WorkPlaceCode : "";
                                Ledger.AmountCur = sumProductVoucherDetail[i].ExciseTaxAmountCur;
                                Ledger.Amount = sumProductVoucherDetail[i].ExciseTaxAmount;
                                Ledger.DebitContractCode = "";
                                Ledger.DebitFProductWorkCode = "";
                                Ledger.DebitWorkPlaceCode = "";
                                Ledger.Note = "Thuế tiêu thụ đặc biệt";
                                Ledger.TaxCategoryCode = sumProductVoucherDetail[i].TaxCategoryCode;
                                Ledger.PartnerCode = entity.PartnerCode ?? entity.PartnerCode0;
                                if (AccTaxDetails != null)
                                {
                                    if (AccTaxDetails.Count > 0)
                                    {
                                        Ledger.CheckDuplicate = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].CheckDuplicate;
                                        Ledger.VatPercentage = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].VatPercentage;
                                        Ledger.InvoiceNumber = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].InvoiceNumber;
                                        Ledger.InvoiceSymbol = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].InvoiceSymbol;
                                        Ledger.InvoiceDate = AccTaxDetails.Count != 0 ? AccTaxDetails[0].InvoiceDate : (DateTime?)null;
                                        Ledger.TaxCode = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].TaxCode;
                                        Ledger.SecurityNo = AccTaxDetails.Count == 0 ? null : AccTaxDetails[0].SecurityNo;
                                    }
                                }

                                Ledger.DebitOrCredit = null;
                                Ledger.SalesChannelCode = entity.SalesChannelCode;
                                Ledger.ProductName0 = sumProductVoucherDetail[i].ProductName0;

                                Ledger.Status = entity.Status;
                                Ledgers.Add(Ledger);

                            }
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < sumProductVoucherDetail.Count(); i++)
                {
                    var accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, sumProductVoucherDetail[i].DebitAcc, entity.OrgCode, entity.Year);
                    if (productVoucherReceipts != null && productVoucherReceipts.Count > 0)
                    {
                        if (entity.VoucherGroup == 2)
                        {
                            accountSystemDebitAcc = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, sumProductVoucherDetail[i].DebitAcc2, entity.OrgCode, entity.Year);
                        }

                        var accountSystemCreditAccs = await _accountSystemService.GetAccountByAccCodeAsync(lstAccSystem, productVoucherReceipts[0].ExciseTaxCreditAcc, entity.OrgCode, entity.Year);

                        if (productVoucherReceipts.Count > 0)
                        {
                            if (i < 1)
                            {
                                if (!string.IsNullOrEmpty(productVoucherReceipts[i].ExciseTaxDebitAcc) && !string.IsNullOrEmpty(productVoucherReceipts[i].ExciseTaxCreditAcc))
                                {
                                    if (productVoucherReceipts[i].ExciseTaxDebitAcc != ""
                               && productVoucherReceipts[i].ExciseTaxCreditAcc != ""
                               && VHT_KHU_TRUNG_HV.Contains(productVoucherReceipts[i].ExciseTaxDebitAcc.Left(3)) == false
                               && VHT_KHU_TRUNG_HV.Contains(productVoucherReceipts[i].ExciseTaxCreditAcc.Left(3)) == false
                               && entity.TotalDiscountAmount != 0
                              )
                                    {
                                        CrudLedgerDto Ledger = new CrudLedgerDto();
                                        Ledger.VoucherId = entity.Id;
                                        Ledger.Ord0 = "DB00000001";
                                        Ledger.Year = entity.Year;
                                        Ledger.Ord0Extra = "DB00000001";
                                        Ledger.OrgCode = entity.OrgCode;
                                        Ledger.DepartmentCode = entity.DepartmentCode;
                                        Ledger.VoucherCode = entity.VoucherCode;
                                        Ledger.VoucherGroup = entity.VoucherGroup == 3 ? 1 : entity.VoucherGroup;
                                        Ledger.BusinessCode = entity.BusinessCode;
                                        Ledger.BusinessAcc = entity.BusinessAcc;
                                        Ledger.VoucherNumber = entity.VoucherNumber;
                                        Ledger.InvoiceNbr = entity.InvoiceNumber;
                                        Ledger.VoucherDate = entity.VoucherDate;
                                        Ledger.PaymentTermsCode = entity.PaymentTermsCode;
                                        Ledger.CurrencyCode = entity.CurrencyCode;
                                        Ledger.ExchangeRate = entity.ExchangeRate;
                                        Ledger.PartnerCode0 = entity.PartnerCode0;
                                        Ledger.PartnerName0 = entity.PartnerName0;
                                        Ledger.Representative = entity.Representative;
                                        Ledger.Address = entity.Address;
                                        Ledger.Description = entity.Description;
                                        Ledger.DescriptionE = entity.DescriptionE;
                                        Ledger.DebitAcc = entity.ExciseTaxDebitAcc;
                                        Ledger.DebitAmountCur = accountSystemCreditAccs.AttachCurrency == "C" ? entity.TotalExciseTaxAmountCur : 0;
                                        Ledger.DebitExchangeRate = accountSystemDebitAcc.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                        Ledger.DebitCurrencyCode = accountSystemDebitAcc.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                        Ledger.DebitPartnerCode = accountSystemDebitAcc.AttachPartner == "C" ? entity.PartnerCode0 : "";
                                        Ledger.DebitContractCode = accountSystemDebitAcc.AttachContract == "C" ? sumProductVoucherDetail[i].ContractCode : "";
                                        Ledger.DebitFProductWorkCode = accountSystemDebitAcc.AttachProductCost == "C" ? sumProductVoucherDetail[i].FProductWorkCode : "";
                                        Ledger.DebitWorkPlaceCode = accountSystemDebitAcc.AttachWorkPlace == "C" ? sumProductVoucherDetail[i].WorkPlaceCode : "";
                                        Ledger.DebitSectionCode = accountSystemDebitAcc.AttachAccSection == "C" ? sumProductVoucherDetail[i].SectionCode : "";
                                        Ledger.CreditAcc = entity.ExciseTaxCreditAcc;
                                        Ledger.CreditCurrencyCode = accountSystemCreditAccs.AttachCurrency == "C" ? entity.CurrencyCode : curencyCode;
                                        Ledger.CreditExchangeRate = accountSystemCreditAccs.AttachCurrency == "C" ? entity.ExchangeRate : 1;
                                        Ledger.CreditPartnerCode = accountSystemCreditAccs.AttachPartner == "C" ? sumProductVoucherDetail[i].PartnerCode : "";
                                        Ledger.CreditContractCode = accountSystemCreditAccs.AttachContract == "C" ? sumProductVoucherDetail[i].ContractCode : "";
                                        Ledger.CreditFProductWorkCode = accountSystemCreditAccs.AttachProductCost == "C" ? sumProductVoucherDetail[i].FProductWorkCode : "";
                                        Ledger.CreditSectionCode = accountSystemCreditAccs.AccSectionCode == "C" ? sumProductVoucherDetail[i].SectionCode : "";
                                        Ledger.CreditWorkPlaceCode = accountSystemCreditAccs.AttachWorkPlace == "C" ? sumProductVoucherDetail[i].WorkPlaceCode : "";
                                        Ledger.CreditAmountCur = accountSystemCreditAccs.AttachCurrency == "C" ? entity.TotalExciseTaxAmountCur : 0;
                                        Ledger.CreditAmount = accountSystemCreditAccs.AttachCurrency == "C" ? entity.TotalExciseTaxAmount : 0;
                                        Ledger.AmountCur = entity.TotalExciseTaxAmountCur;
                                        Ledger.Amount = entity.TotalExciseTaxAmount;
                                        Ledger.Note = "Thuế tiêu thụ đặc biệt";
                                        Ledger.TaxCategoryCode = sumProductVoucherDetail[i].TaxCategoryCode;
                                        if (AccTaxDetails != null)
                                        {
                                            if (AccTaxDetails.Count > 0)
                                            {
                                                Ledger.VatPercentage = AccTaxDetails[0].VatPercentage;
                                                Ledger.InvoiceNumber = AccTaxDetails[0].InvoiceNumber;
                                                Ledger.InvoiceSymbol = AccTaxDetails[0].InvoiceSymbol;
                                                Ledger.InvoiceDate = AccTaxDetails[0].InvoiceDate;
                                                Ledger.TaxCode = AccTaxDetails[0].TaxCode;
                                                Ledger.CheckDuplicate = AccTaxDetails[0].CheckDuplicate;
                                                Ledger.SecurityNo = AccTaxDetails[0].SecurityNo;

                                            }
                                        }

                                        Ledger.DebitOrCredit = "";
                                        Ledger.SalesChannelCode = entity.SalesChannelCode;
                                        Ledger.ProductName0 = sumProductVoucherDetail[i].ProductName0;
                                        Ledger.Status = entity.Status;
                                        Ledger.InvoicePartnerName = entity.PartnerName0;
                                        Ledger.InvoicePartnerAddress = entity.Address;
                                        Ledger.PartnerCode = entity.PartnerCode ?? entity.PartnerCode0;
                                        Ledgers.Add(Ledger);
                                    }
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return Ledgers;
        }
        public async Task<List<CrudLedgerDto>> CreateLedgerLrAsync(CrudProductVoucherDto dto)
        {
            List<CrudLedgerDto> ledgerDtos = new List<CrudLedgerDto>();
            CrudLedgerDto crud = new CrudLedgerDto();

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


            var vLISTCTLRs = await _voucherTypeService.GetByVoucherTypeAsync("PLR");
            if (vLISTCTLRs.Count > 0)
            {
                vLISTCTLR = vLISTCTLRs[0].ListVoucher;

            }
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
            var defaulVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();

            if (vHACHTOANs != null)
            {
                vHACHTOAN = vHACHTOANs.IsSavingLedger == null ? "C" : vHACHTOANs.IsSavingLedger;
                vSOKHO = vHACHTOANs.IsSavingWarehouseBook == null ? "C" : "K";
            }
            else
            {
                var lstVoucher = defaulVoucherCategory.Where(p => p.Code == dto.VoucherCode).FirstOrDefault();
                vHACHTOAN = lstVoucher.IsSavingLedger == null ? "C" : lstVoucher.IsSavingLedger;
                vSOKHO = lstVoucher.IsSavingWarehouseBook == null ? "C" : "K";
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
            // var vSOKHOs = await _voucherCategoryService.GetByCode(dto.VoucherCode, _webHelper.GetCurrentOrgUnit());


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
                                     VoucherGroup = p.VoucherGroup,
                                     BusinessCode = p.BusinessCode,
                                     BusinessAcc = p.BusinessAcc,
                                     VoucherNumber = p.VoucherNumber,
                                     InvoiceNumber = p.InvoiceNumber,
                                     VoucherDate = p.VoucherDate,
                                     PaymentTermsCode = p.PaymentTermsCode,
                                     CurrencyCode = p.CurrencyCode,
                                     ExchangeRate = p.ExchangeRate,
                                     PartnerCode0 = p.PartnerCode0,
                                     Representative = p.Representative,
                                     Address = p.Address,
                                     OriginVoucher = p.OriginVoucher,
                                     Description = p.Description,
                                     DescriptionE = p.DescriptionE,
                                     Status = p.Status,
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

                                     VatPercentage = e != null ? e.VatPercentage : null,
                                     ClearingPartnerCode = e != null ? e.ClearingPartnerCode : null,
                                     AddressInv = e != null ? e.Address : null,
                                     PaymentTermsCodes = p.PaymentTermsCode,
                                     SalesChannelCode = p.SalesChannelCode,
                                     PartnerCode = p.PartnerCode0,
                                     ContractCode = p.CurrencyCode,
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
                                     AssemblyAmountCru = p.TotalAmountCur,
                                     AssemblyAmount = p.TotalAmount,

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
                                           ExciseAmount = i != null ? i.ExciseTaxAmount : 0,
                                           ExciseAmountCur = i != null ? i.ExciseTaxAmountCur : 0,
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
                                           QuantityQd = p2.ExchangeRate != 0 ? b.Quantity * p1.ExchangeRate / p2.ExchangeRate : 0,
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

                                       };

            //Trường hợp tiền giảm giá trừ trực tiếp

            var productVoucherdetail1 = from b in productVoucherdetail
                                        where string.IsNullOrEmpty(b.DevaluationCreditAcc) && string.IsNullOrEmpty(b.DevaluationDebitAcc)
                                        select new
                                        {
                                            b.VoucherGroup0,
                                            b.VoucherGroup,
                                            b.BusinessAcc,
                                            DevaluationCreditAcc = b.DevaluationCreditAcc,
                                            DevaluationDebitAcc = b.DevaluationDebitAcc,
                                            DiscountCreditAcc = b.DiscountCreditAcc,
                                            DiscountDebitAcc = b.DiscountDebitAcc,
                                            Id = b.Id,
                                            ProductVoucherId = b.ProductVoucherId,
                                            Rec0 = b.Ord0,
                                            Ord0 = b.Ord0,
                                            ContractCode = b.ContractCode,
                                            ProductCode = b.ProductCode,
                                            TransProductCode = b.TransProductCode,
                                            TransWarehouseCode = b.TransWarehouseCode,
                                            WarehouseCode = b.WarehouseCode,
                                            ProductLotCode = b.ProductLotCode,
                                            TransProductLotCode = b.TransProductLotCode,
                                            ProductOriginCode = b.ProductOriginCode,
                                            TransProductOriginCode = b.TransProductOriginCode,
                                            PartnerCode = b.PartnerCode,
                                            FProductWorkCode = b.FProductWorkCode,
                                            UnitCode = b.UnitCode,
                                            TransUnitCode = b.TransUnitCode,
                                            ProductName0 = b.ProductName0,
                                            Quantity = b.Quantity,
                                            Price = b.Price,
                                            PriceCur = b.PriceCur,
                                            AmountCur = vLGLR == 0 ? b.AmountCur - b.DevaluationAmountCur : b.AmountCur,
                                            Amount = vLGLR == 0 ? b.Amount - b.DevaluationAmount : b.Amount,
                                            ExpenseAmountCur0 = b.ExpenseAmountCur0,
                                            ExpenseAmount0 = b.ExpenseAmount0,
                                            ExpenseAmount1 = b.ExpenseAmount1,
                                            ExpenseAmountCur1 = b.ExpenseAmountCur1,
                                            ExpenseAmountCur = b.ExpenseAmountCur,
                                            ExpenseAmount = b.ExpenseAmount,
                                            VatPercentage = b.VatPercentage,
                                            VatAmountCur = b.VatAmountCur,
                                            VatAmount = b.VatAmount,
                                            DiscountPercentage = b.DiscountPercentage,
                                            DiscountAmount = b.DiscountAmount,
                                            DiscountAmountCur = b.DiscountAmountCur,
                                            ImportTaxPercentage = b.ImportTaxPercentage,
                                            ImportTaxAmount = b.ImportTaxAmount,
                                            ImportTaxAmountCur = b.ImportTaxAmountCur,
                                            ExciseTaxPercentage = b.ExciseTaxPercentage,
                                            ExciseAmount = b.ExciseAmount,
                                            ExciseAmountCur = b.ExciseAmountCur,
                                            DebitAcc0 = b.DebitAcc,
                                            DebitAcc = b.DebitAcc,
                                            CreditAcc0 = b.CreditAcc,
                                            CreditAcc = b.CreditAcc,
                                            PriceCur2 = b.PriceCur2,
                                            Price2 = b.Price2,
                                            AmountCur2 = vLGLR == 1 ? b.AmountCur2 - b.DevaluationAmountCur : b.AmountCur2,
                                            Amount2 = vLGLR == 1 ? b.Amount2 - b.DevaluationAmount : b.Amount2,
                                            DebitAcc2 = b.DebitAcc2,
                                            CreditAcc2 = b.CreditAcc2,
                                            Note = b.Note,
                                            NoteE = b.NoteE,
                                            SectionCode = b.SectionCode,
                                            WorkPlaceCode = b.WorkPlaceCode,
                                            CaseCode = b.CaseCode,
                                            FixedPrice = b.FixedPrice,
                                            VatPrice = b.VatPrice,
                                            DecreasePercentage = b.DecreasePercentage,
                                            DevaluationPrice = b.DevaluationPrice,
                                            DevaluationPriceCur = 0,
                                            DevaluationAmount = 0,
                                            DevaluationAmountCur = b.DevaluationAmountCur,
                                            VarianceAmount = b.VarianceAmount,
                                            ProductType = b.ProductType,
                                            //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                            WarehouseAcc = b.WarehouseAcc,
                                            ProductAcc = b.ProductAcc,
                                            IsOriginVoucher = b.IsOriginVoucher,//ctgoc
                                            IsLedger = b.IsLedger,
                                            IsLedger2 = b.IsLedger2,
                                            TrxQuantity = b.Quantity,
                                            TrxPriceCru = b.PriceCur,
                                            TrxPrice = b.Price,
                                            TrxPriceCru2 = b.PriceCur2,
                                            TrxPrice2 = b.Price2,
                                            QuantityQd = b.QuantityQd,
                                            PriceCur0 = b.PriceCur,
                                            Price0 = b.Price,
                                            AccImport = b.BusinessAcc,//tknhap
                                            QuantityTrxN = b.Quantity,
                                            ImportQuantity = b.Quantity,
                                            ImportAmountCru = b.AmountCur,
                                            ImportAmount = b.Amount,
                                            AccExport = b.BusinessAcc,
                                            QuantityTrxX = b.Quantity,
                                            ExportQuantity = b.Quantity,
                                            ExportAmountCru = b.AmountCur,
                                            ExportAmount = b.Amount,
                                            AssemblyWarehouseCode = b.AssemblyWarehouseCode,
                                            AssemblyProductCode = b.AssemblyProductCode,
                                            AssemblyProductLotCode = b.AssemblyProductLotCode,
                                            AssemblyUnitCode = b.AssemblyUnitCode,
                                            AssemblyQuantity = b.AssemblyQuantity,
                                            AssemblyPriceCur = b.AssemblyPriceCur,
                                            AssemblyPrice = b.AssemblyPrice,
                                            AssemblyAmountCru = b.AssemblyAmountCru + b.AmountCur,
                                            AssemblyAmount = b.AssemblyAmount + b.Amount,

                                        };
            // update lại cthv
            var productVoucherdetail2 = from b in productVoucherdetail
                                        join m in productVoucherdetail1 on b.Id equals m.Id into d
                                        from c in d.DefaultIfEmpty()
                                        select new
                                        {
                                            Id = b.Id,
                                            b.ProductVoucherId,
                                            b.VoucherGroup0,
                                            b.VoucherGroup,
                                            b.BusinessAcc,
                                            DevaluationCreditAcc = b.DevaluationCreditAcc,
                                            DevaluationDebitAcc = b.DevaluationDebitAcc,
                                            DiscountCreditAcc = b.DiscountCreditAcc,
                                            DiscountDebitAcc = b.DiscountDebitAcc,
                                            Rec0 = b.Ord0,
                                            Ord0 = b.Ord0,
                                            ContractCode = b.ContractCode,
                                            ProductCode = b.ProductCode,
                                            TransProductCode = b.TransProductCode,
                                            TransWarehouseCode = b.TransWarehouseCode,
                                            WarehouseCode = b.WarehouseCode,
                                            ProductLotCode = b.ProductLotCode,
                                            TransProductLotCode = b.TransProductLotCode,
                                            ProductOriginCode = b.ProductOriginCode,
                                            TransProductOriginCode = b.TransProductOriginCode,
                                            PartnerCode = b.PartnerCode,
                                            FProductWorkCode = b.FProductWorkCode,
                                            UnitCode = b.UnitCode,
                                            TransUnitCode = b.TransUnitCode,
                                            ProductName0 = b.ProductName0,
                                            Quantity = b.Quantity,
                                            Price = b.Price,
                                            PriceCur = b.PriceCur,
                                            AmountCur = c != null ? c.AmountCur : b.AmountCur,
                                            Amount = c != null ? c.Amount : b.Amount,
                                            ExpenseAmountCur0 = b.ExpenseAmountCur0,
                                            ExpenseAmount0 = b.ExpenseAmount0,
                                            ExpenseAmount1 = b.ExpenseAmount1,
                                            ExpenseAmountCur1 = b.ExpenseAmountCur1,
                                            ExpenseAmountCur = b.ExpenseAmountCur,
                                            ExpenseAmount = b.ExpenseAmount,
                                            VatPercentage = b.VatPercentage,
                                            VatAmountCur = b.VatAmountCur,
                                            VatAmount = b.VatAmount,
                                            DiscountPercentage = b.DiscountPercentage,
                                            DiscountAmount = b.DiscountAmount,
                                            DiscountAmountCur = b.DiscountAmountCur,
                                            ImportTaxPercentage = b.ImportTaxPercentage,
                                            ImportTaxAmount = b.ImportTaxAmount,
                                            ImportTaxAmountCur = b.ImportTaxAmountCur,
                                            ExciseTaxPercentage = b.ExciseTaxPercentage,
                                            ExciseAmount = b.ExciseAmount,
                                            ExciseAmountCur = b.ExciseAmountCur,
                                            DebitAcc0 = b.DebitAcc,
                                            DebitAcc = b.DebitAcc,
                                            CreditAcc0 = b.CreditAcc,
                                            CreditAcc = b.CreditAcc,
                                            PriceCur2 = b.PriceCur2,
                                            Price2 = b.Price2,
                                            AmountCur2 = c != null ? c.AmountCur2 : b.AmountCur2,
                                            Amount2 = c != null ? c.Amount2 : b.Amount2,
                                            DebitAcc2 = b.DebitAcc2,
                                            CreditAcc2 = b.CreditAcc2,
                                            Note = b.Note,
                                            NoteE = b.NoteE,
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
                                            ProductType = b.ProductType,
                                            //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                            WarehouseAcc = b.WarehouseAcc,
                                            ProductAcc = b.ProductAcc,
                                            IsOriginVoucher = b.IsOriginVoucher,//ctgoc
                                            IsLedger = b.IsLedger,
                                            IsLedger2 = b.IsLedger2,
                                            TrxQuantity = b.Quantity,
                                            TrxPriceCru = b.PriceCur,
                                            TrxPrice = b.Price,
                                            TrxPriceCru2 = b.PriceCur2,
                                            TrxPrice2 = b.Price2,
                                            QuantityQd = b.QuantityQd,
                                            PriceCur0 = b.PriceCur,
                                            Price0 = b.Price,
                                            AccImport = b.BusinessAcc,//tknhap
                                            QuantityTrxN = b.Quantity,
                                            ImportQuantity = b.Quantity,
                                            ImportAmountCru = b.AmountCur,
                                            ImportAmount = b.Amount,
                                            AccExport = b.BusinessAcc,
                                            QuantityTrxX = b.Quantity,
                                            ExportQuantity = b.Quantity,
                                            ExportAmountCru = b.AmountCur,
                                            ExportAmount = b.Amount,
                                            AssemblyWarehouseCode = b.AssemblyWarehouseCode,
                                            AssemblyProductCode = b.AssemblyProductCode,
                                            AssemblyProductLotCode = b.AssemblyProductLotCode,
                                            AssemblyUnitCode = b.AssemblyUnitCode,
                                            AssemblyQuantity = b.AssemblyQuantity,
                                            AssemblyPriceCur = b.AssemblyPriceCur,
                                            AssemblyPrice = b.AssemblyPrice,
                                            AssemblyAmountCru = c != null ? c.AssemblyAmountCru : b.AssemblyAmountCru,
                                            AssemblyAmount = c != null ? c.AssemblyAmount : b.AssemblyAmount
                                        };
            //Hạch toán
            if (vHACHTOAN != "K")
            {
                // hạch toán 1Update 
                var productVoucherdetail3 = from b in productVoucherdetail2
                                            where vVHTKHU_HV.Contains(b.DebitAcc) == false && vVHTKHU_HV.Contains(b.CreditAcc) == false
                                            select new
                                            {
                                                b.VoucherGroup0,
                                                b.VoucherGroup,
                                                b.BusinessAcc,
                                                DevaluationCreditAcc = b.DevaluationCreditAcc,
                                                DevaluationDebitAcc = b.DevaluationDebitAcc,
                                                DiscountCreditAcc = b.DiscountCreditAcc,
                                                DiscountDebitAcc = b.DiscountDebitAcc,
                                                Id = b.Id,
                                                b.ProductVoucherId,
                                                Rec0 = b.Ord0,
                                                Ord0 = b.Ord0,
                                                ContractCode = b.ContractCode,
                                                ProductCode = b.ProductCode,
                                                TransProductCode = b.TransProductCode,
                                                TransWarehouseCode = b.TransWarehouseCode,
                                                WarehouseCode = b.WarehouseCode,
                                                ProductLotCode = b.ProductLotCode,
                                                TransProductLotCode = b.TransProductLotCode,
                                                ProductOriginCode = b.ProductOriginCode,
                                                TransProductOriginCode = b.TransProductOriginCode,
                                                PartnerCode = b.PartnerCode,
                                                FProductWorkCode = b.FProductWorkCode,
                                                UnitCode = b.UnitCode,
                                                TransUnitCode = b.TransUnitCode,
                                                ProductName0 = b.ProductName0,
                                                Quantity = b.Quantity,
                                                Price = b.Price,
                                                PriceCur = b.PriceCur,
                                                AmountCur = b.AmountCur,
                                                Amount = b.Amount,
                                                ExpenseAmountCur0 = b.ExpenseAmountCur0,
                                                ExpenseAmount0 = b.ExpenseAmount0,
                                                ExpenseAmount1 = b.ExpenseAmount1,
                                                ExpenseAmountCur1 = b.ExpenseAmountCur1,
                                                ExpenseAmountCur = b.ExpenseAmountCur,
                                                ExpenseAmount = b.ExpenseAmount,
                                                VatPercentage = b.VatPercentage,
                                                VatAmountCur = b.VatAmountCur,
                                                VatAmount = b.VatAmount,
                                                DiscountPercentage = b.DiscountPercentage,
                                                DiscountAmount = b.DiscountAmount,
                                                DiscountAmountCur = b.DiscountAmountCur,
                                                ImportTaxPercentage = b.ImportTaxPercentage,
                                                ImportTaxAmount = b.ImportTaxAmount,
                                                ImportTaxAmountCur = b.ImportTaxAmountCur,
                                                ExciseTaxPercentage = b.ExciseTaxPercentage,
                                                ExciseAmount = b.ExciseAmount,
                                                ExciseAmountCur = b.ExciseAmountCur,
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
                                                Note = b.Note,
                                                NoteE = b.NoteE,
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
                                                ProductType = b.ProductType,
                                                //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                                WarehouseAcc = b.WarehouseAcc,
                                                ProductAcc = b.ProductAcc,
                                                IsOriginVoucher = b.IsOriginVoucher,//ctgoc
                                                IsLedger = "C",
                                                IsLedger2 = b.IsLedger2,
                                                TrxQuantity = b.Quantity,
                                                TrxPriceCru = b.PriceCur,
                                                TrxPrice = b.Price,
                                                TrxPriceCru2 = b.PriceCur2,
                                                TrxPrice2 = b.Price2,
                                                QuantityQd = b.QuantityQd,
                                                PriceCur0 = b.PriceCur,
                                                Price0 = b.Price,
                                                AccImport = b.BusinessAcc,//tknhap
                                                QuantityTrxN = b.Quantity,
                                                ImportQuantity = b.Quantity,
                                                ImportAmountCru = b.AmountCur,
                                                ImportAmount = b.Amount,
                                                AccExport = b.BusinessAcc,
                                                QuantityTrxX = b.Quantity,
                                                ExportQuantity = b.Quantity,
                                                ExportAmountCru = b.AmountCur,
                                                ExportAmount = b.Amount,
                                                AssemblyWarehouseCode = b.AssemblyWarehouseCode,
                                                AssemblyProductCode = b.AssemblyProductCode,
                                                AssemblyProductLotCode = b.AssemblyProductLotCode,
                                                AssemblyUnitCode = b.AssemblyUnitCode,
                                                AssemblyQuantity = b.AssemblyQuantity,
                                                AssemblyPriceCur = b.AssemblyPriceCur,
                                                AssemblyPrice = b.AssemblyPrice,
                                                AssemblyAmountCru = b.AssemblyAmountCru,
                                                AssemblyAmount = b.AssemblyAmount,
                                            };
                var resulProductVoucherdetail = (from b in productVoucherdetail2
                                                 join c in productVoucherdetail3 on b.Id equals c.Id
                                                 select new
                                                 {
                                                     b.VoucherGroup0,
                                                     b.VoucherGroup,
                                                     b.BusinessAcc,
                                                     DevaluationCreditAcc = b.DevaluationCreditAcc,
                                                     DevaluationDebitAcc = b.DevaluationDebitAcc,
                                                     DiscountCreditAcc = b.DiscountCreditAcc,
                                                     DiscountDebitAcc = b.DiscountDebitAcc,
                                                     Id = b.Id,
                                                     b.ProductVoucherId,
                                                     Rec0 = b.Ord0,
                                                     Ord0 = b.Ord0,
                                                     ContractCode = b.ContractCode,
                                                     ProductCode = b.ProductCode,
                                                     TransProductCode = b.TransProductCode,
                                                     TransWarehouseCode = b.TransWarehouseCode,
                                                     WarehouseCode = b.WarehouseCode,
                                                     ProductLotCode = b.ProductLotCode,
                                                     TransProductLotCode = b.TransProductLotCode,
                                                     ProductOriginCode = b.ProductOriginCode,
                                                     TransProductOriginCode = b.TransProductOriginCode,
                                                     PartnerCode = b.PartnerCode,
                                                     FProductWorkCode = b.FProductWorkCode,
                                                     UnitCode = b.UnitCode,
                                                     TransUnitCode = b.TransUnitCode,
                                                     ProductName0 = b.ProductName0,
                                                     Quantity = b.Quantity,
                                                     Price = b.Price,
                                                     PriceCur = b.PriceCur,
                                                     AmountCur = b.AmountCur,
                                                     Amount = b.Amount,
                                                     ExpenseAmountCur0 = b.ExpenseAmountCur0,
                                                     ExpenseAmount0 = b.ExpenseAmount0,
                                                     ExpenseAmount1 = b.ExpenseAmount1,
                                                     ExpenseAmountCur1 = b.ExpenseAmountCur1,
                                                     ExpenseAmountCur = b.ExpenseAmountCur,
                                                     ExpenseAmount = b.ExpenseAmount,
                                                     VatPercentage = b.VatPercentage,
                                                     VatAmountCur = b.VatAmountCur,
                                                     VatAmount = b.VatAmount,
                                                     DiscountPercentage = b.DiscountPercentage,
                                                     DiscountAmount = b.DiscountAmount,
                                                     DiscountAmountCur = b.DiscountAmountCur,
                                                     ImportTaxPercentage = b.ImportTaxPercentage,
                                                     ImportTaxAmount = b.ImportTaxAmount,
                                                     ImportTaxAmountCur = b.ImportTaxAmountCur,
                                                     ExciseTaxPercentage = b.ExciseTaxPercentage,
                                                     ExciseAmount = b.ExciseAmount,
                                                     ExciseAmountCur = b.ExciseAmountCur,
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
                                                     Note = b.Note,
                                                     NoteE = b.NoteE,
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
                                                     ProductType = b.ProductType,
                                                     //DiscountAmount = i !=null ? i.DiscountAmount :null,
                                                     WarehouseAcc = b.WarehouseAcc,
                                                     ProductAcc = b.ProductAcc,
                                                     IsOriginVoucher = b.IsOriginVoucher,//ctgoc
                                                     IsLedger = c != null ? c.IsLedger : b.IsLedger,
                                                     IsLedger2 = b.IsLedger2,
                                                     TrxQuantity = b.Quantity,
                                                     TrxPriceCru = b.PriceCur,
                                                     TrxPrice = b.Price,
                                                     TrxPriceCru2 = b.PriceCur2,
                                                     TrxPrice2 = b.Price2,
                                                     QuantityQd = b.QuantityQd,
                                                     PriceCur0 = b.PriceCur,
                                                     Price0 = b.Price,
                                                     AccImport = b.BusinessAcc,//tknhap
                                                     QuantityTrxN = b.Quantity,
                                                     ImportQuantity = b.Quantity,
                                                     ImportAmountCru = b.AmountCur,
                                                     ImportAmount = b.Amount,
                                                     AccExport = b.BusinessAcc,
                                                     QuantityTrxX = b.Quantity,
                                                     ExportQuantity = b.Quantity,
                                                     ExportAmountCru = b.AmountCur,
                                                     ExportAmount = b.Amount,
                                                     AssemblyWarehouseCode = b.AssemblyWarehouseCode,
                                                     AssemblyProductCode = b.AssemblyProductCode,
                                                     AssemblyProductLotCode = b.AssemblyProductLotCode,
                                                     AssemblyUnitCode = b.AssemblyUnitCode,
                                                     AssemblyQuantity = b.AssemblyQuantity,
                                                     AssemblyPriceCur = b.AssemblyPriceCur,
                                                     AssemblyPrice = b.AssemblyPrice,
                                                     AssemblyAmountCru = b.AssemblyAmountCru,
                                                     AssemblyAmount = b.AssemblyAmount,
                                                 }).ToList();
                var accountSystems = await _accountSystemService.GetQueryableAsync();
                var accountSystem = accountSystems.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var productVouchers = productVoucher.ToList();
                var taxCategoryServices = await _taxCategoryService.GetQueryableAsync();
                var taxCategoryService = taxCategoryServices.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var ledger1 = from ct in resulProductVoucherdetail
                              join dp in productVouchers on ct.ProductVoucherId equals dp.Id
                              join p in accountSystem on new { dp.Year, dp.OrgCode, AccCode = ct.DebitAcc } equals new { p.Year, p.OrgCode, p.AccCode } into a
                              from acc in a.DefaultIfEmpty()
                              join s in accountSystem on new { dp.Year, dp.OrgCode, AccCode = ct.CreditAcc } equals new { s.Year, s.OrgCode, s.AccCode } into m
                              from acc2 in a.DefaultIfEmpty()
                              where ct.IsLedger == "C"
                              select new LedgerMultiDto
                              {
                                  OrgCode = dp.OrgCode,
                                  Year = dp.Year,
                                  DepartmentCode = dp.DepartmentCode,
                                  VoucherCode = dp.VoucherCode,
                                  VoucherGroup = dp.VoucherGroup,
                                  BusinessAcc = dp.BusinessAcc,
                                  BusinessCode = dp.BusinessCode,
                                  VoucherNumber = dp.VoucherNumber,
                                  InvoiceNumber = dp.InvoiceNumber,
                                  VoucherDate = dp.VoucherDate,
                                  CurrencyCode = dp.CurrencyCode,
                                  ExchangeRate = dp.ExchangeRate,
                                  PartnerCode0 = dp.PartnerCode0,
                                  Representative = dp.Representative,
                                  Address = dp.Address,
                                  Description = dp.Description,
                                  OriginVoucher = dp.OriginVoucher,
                                  Status = dp.Status,
                                  Id = dp.Id,
                                  Ord0 = ct.Ord0,
                                  ContractCode = ct.ContractCode,
                                  ContractCodeBT = "",
                                  CreditAcc = ct.CreditAcc,
                                  DebitAcc = ct.DebitAcc,
                                  AmountCur = ct.AmountCur,
                                  Amount = ct.Amount,
                                  Note = ct.Note,
                                  WorkPlaceCode = dp.WorkPlaceCode,
                                  FProductWorkCode = ct.FProductWorkCode,
                                  FProductWorkCodeBT = "",
                                  PartnerCode = ct.PartnerCode,
                                  PartnerCodeBt = "",
                                  SectionCode = ct.SectionCode,
                                  SectionCodeBt = "",
                                  CaseCode = ct.CaseCode,
                                  TaxCategoryCode = dp.TaxCategoryCode,
                                  InvoiceDate = dp.InvoiceDate,
                                  InvoiceSerial = "", //dp.InvoiceSerial,
                                  InvoiceNumbers = dp.InvoiceNumber,
                                  VatPercentage = dp.VatPercentage,
                                  PartnerNameBt = dp.ClearingPartnerCode,
                                  AddressInv = dp.AddressInv,
                                  TaxCode = dp.TaxCode,
                                  CurrencyCodeDebit = acc.AttachCurrency == "C" ? dp.CurrencyCode : null,
                                  ExchangeRateDebit = acc.AttachCurrency == "C" ? dp.ExchangeRate : 1,
                                  AmountCurDebit = acc.AttachCurrency == "C" ? ct.AmountCur : 0,
                                  PartnerCodeDebit = acc.AccPattern == 1 ? ct.PartnerCode : null,
                                  ContractCodeDebit = acc.AttachContract == "C" ? ct.ContractCode : null,
                                  FProductWorkCodeDebit = acc.AttachProductCost == "C" ? ct.FProductWorkCode : null,
                                  SectionCodeDebit = acc.AccSectionCode == "C" ? ct.SectionCode : null,
                                  WorkPlaceCodeDebit = acc.AttachWorkPlace == "C" ? ct.WorkPlaceCode : null,
                                  CurrencyCodeCredit = acc2.AttachCurrency == "C" ? dp.CurrencyCode : null,
                                  ExchangeRateCredit = acc2.AttachCurrency == "C" ? dp.ExchangeRate : 1,
                                  AmountCurCredit = acc2.AttachCurrency == "C" ? ct.AmountCur : 0,
                                  PartnerCodeCredit = acc2.AccPattern == 1 ? ct.PartnerCode : null,
                                  ContractCodeCredit = acc2.AttachContract == "C" ? ct.ContractCode : null,
                                  FProductWorkCodeCredit = acc2.AttachProductCost == "C" ? ct.FProductWorkCode : null,
                                  SectionCodeCredit = acc2.AccSectionCode == "C" ? ct.SectionCode : null,
                                  WorkPlaceCodeCredit = acc2.AttachWorkPlace == "C" ? ct.WorkPlaceCode : null,
                                  CheckDuplicate = "",
                                  WarehouseCode = ct.WarehouseCode,
                                  AssemblyWarehouseCode = ct.AssemblyWarehouseCode,
                                  ProductCode = ct.ProductCode,
                                  ProductLotCode = ct.ProductLotCode,
                                  ProductOriginCode = ct.ProductOriginCode,
                                  UnitCode = ct.UnitCode,
                                  Quantity = ct.Quantity,
                                  PriceCur = ct.PriceCur,
                                  Price = ct.Price,
                                  ProductName0 = ct.ProductName0
                              };

                var ledger2 = from ct in crudAccTaxDetailDtos
                              join dp in productVouchers on ct.ProductVoucherId equals dp.Id
                              join th in taxCategoryService on ct.TaxCategoryCode equals th.Code
                              join p in accountSystem on new
                              {
                                  dp.Year,
                                  dp.OrgCode,
                                  AccCode = ct.DebitAcc
                              } equals new
                              {
                                  p.Year,
                                  p.OrgCode,
                                  p.AccCode
                              } into a
                              from acc in a.DefaultIfEmpty()
                              join s in accountSystem on new
                              {
                                  dp.Year,
                                  dp.OrgCode,
                                  AccCode = ct.CreditAcc
                              } equals new
                              {
                                  s.Year,
                                  s.OrgCode,
                                  s.AccCode
                              } into m
                              from acc2 in a.DefaultIfEmpty()
                              where th.IsDirect == false && vVHTKHU_HV.Contains(ct.CreditAcc.Left(3)) == false && vVHTKHU_HV.Contains(ct.DebitAcc.Left(3)) == false
                              select new LedgerMultiDto
                              {
                                  OrgCode = dp.OrgCode,
                                  Year = dp.Year,
                                  DepartmentCode = dp.DepartmentCode,
                                  VoucherCode = dp.VoucherCode,
                                  VoucherGroup = dp.VoucherGroup,
                                  BusinessAcc = dp.BusinessAcc,
                                  BusinessCode = dp.BusinessCode,
                                  VoucherNumber = dp.VoucherNumber,
                                  InvoiceNumber = dp.InvoiceNumber,
                                  VoucherDate = dp.VoucherDate,
                                  CurrencyCode = dp.CurrencyCode,
                                  ExchangeRate = dp.ExchangeRate,
                                  PartnerCode0 = dp.PartnerCode0,
                                  Representative = dp.Representative,
                                  Address = dp.Address,
                                  Description = dp.Description,
                                  OriginVoucher = dp.OriginVoucher,
                                  Status = dp.Status,
                                  Id = dp.Id,
                                  Ord0 = ct.Ord0,
                                  ContractCode = ct.ContractCode,
                                  ContractCodeBT = "",
                                  CreditAcc = ct.CreditAcc,
                                  DebitAcc = ct.DebitAcc,
                                  AmountCur = ct.AmountCur,
                                  Amount = ct.Amount,
                                  Note = ct.Note,
                                  WorkPlaceCode = dp.WorkPlaceCode,
                                  FProductWorkCode = ct.FProductWorkCode,
                                  FProductWorkCodeBT = ct.ClearingFProductWorkCode,
                                  PartnerCode = ct.PartnerCode,
                                  PartnerCodeBt = ct.ClearingPartnerCode,
                                  SectionCode = ct.SectionCode,
                                  SectionCodeBt = ct.ClearingSectionCode,
                                  CaseCode = ct.CaseCode,
                                  TaxCategoryCode = dp.TaxCategoryCode,
                                  InvoiceDate = dp.InvoiceDate,
                                  InvoiceSerial = ct.InvoiceSymbol, //dp.InvoiceSerial,
                                  InvoiceNumbers = ct.InvoiceNumber,
                                  VatPercentage = ct.VatPercentage,
                                  PartnerNameBt = ct.ClearingPartnerCode,
                                  AddressInv = ct.Address,
                                  TaxCode = ct.TaxCode,
                                  CurrencyCodeDebit = acc.AttachCurrency == "C" ? dp.CurrencyCode : null,
                                  ExchangeRateDebit = acc.AttachCurrency == "C" ? dp.ExchangeRate : 1,
                                  AmountCurDebit = acc.AttachCurrency == "C" ? ct.AmountCur : 0,
                                  PartnerCodeDebit = acc.AccPattern == 1 ? ct.PartnerCode : null,
                                  ContractCodeDebit = acc.AttachContract == "C" ? ct.ContractCode : null,
                                  FProductWorkCodeDebit = acc.AttachProductCost == "C" ? ct.FProductWorkCode : null,
                                  SectionCodeDebit = acc.AccSectionCode == "C" ? ct.SectionCode : null,
                                  WorkPlaceCodeDebit = acc.AttachWorkPlace == "C" ? ct.WorkPlaceCode : null,
                                  CurrencyCodeCredit = acc2.AttachCurrency == "C" ? dp.CurrencyCode : null,
                                  ExchangeRateCredit = acc2.AttachCurrency == "C" ? dp.ExchangeRate : 1,
                                  AmountCurCredit = acc2.AttachCurrency == "C" ? ct.AmountCur : 0,
                                  PartnerCodeCredit = acc2.AccPattern == 1 ? ct.PartnerCode : null,
                                  ContractCodeCredit = acc2.AttachContract == "C" ? ct.ContractCode : null,
                                  FProductWorkCodeCredit = acc2.AttachProductCost == "C" ? ct.FProductWorkCode : null,
                                  SectionCodeCredit = acc2.AccSectionCode == "C" ? ct.SectionCode : null,
                                  WorkPlaceCodeCredit = acc2.AttachWorkPlace == "C" ? ct.WorkPlaceCode : null,
                                  CheckDuplicate = "",
                                  WarehouseCode = "",
                                  AssemblyWarehouseCode = "",
                                  ProductCode = "",
                                  ProductLotCode = "",
                                  ProductOriginCode = "",
                                  UnitCode = "",
                                  Quantity = 0,
                                  PriceCur = 0,
                                  Price = 0,
                                  ProductName0 = ""

                              };
                var resultLedger = Enumerable.Concat(ledger1, ledger2);
                var ledger3 = from ct in crudProductVoucherCostDtos
                              join dp in productVouchers on ct.ProductVoucherId equals dp.Id

                              join p in accountSystem on new
                              {
                                  dp.Year,
                                  dp.OrgCode,
                                  AccCode = ct.DebitAcc
                              } equals new
                              {
                                  p.Year,
                                  p.OrgCode,
                                  p.AccCode
                              } into a
                              from acc in a.DefaultIfEmpty()
                              join s in accountSystem on new
                              {
                                  dp.Year,
                                  dp.OrgCode,
                                  AccCode = ct.CreditAcc
                              } equals new
                              {
                                  s.Year,
                                  s.OrgCode,
                                  s.AccCode
                              } into m
                              from acc2 in a.DefaultIfEmpty()
                              where vVHTKHU_HV.Contains(ct.CreditAcc.Left(3)) == false && vVHTKHU_HV.Contains(ct.DebitAcc.Left(3)) == false
                              select new LedgerMultiDto
                              {
                                  OrgCode = dp.OrgCode,
                                  Year = dp.Year,
                                  DepartmentCode = dp.DepartmentCode,
                                  VoucherCode = dp.VoucherCode,
                                  VoucherGroup = dp.VoucherGroup,
                                  BusinessAcc = dp.BusinessAcc,
                                  BusinessCode = dp.BusinessCode,
                                  VoucherNumber = dp.VoucherNumber,
                                  InvoiceNumber = dp.InvoiceNumber,
                                  VoucherDate = dp.VoucherDate,
                                  CurrencyCode = dp.CurrencyCode,
                                  ExchangeRate = dp.ExchangeRate,
                                  PartnerCode0 = dp.PartnerCode0,
                                  Representative = dp.Representative,
                                  Address = dp.Address,
                                  Description = dp.Description,
                                  OriginVoucher = dp.OriginVoucher,
                                  Status = dp.Status,
                                  Id = dp.Id,
                                  Ord0 = ct.Ord0,
                                  ContractCode = ct.ContractCode,
                                  ContractCodeBT = "",
                                  CreditAcc = ct.CreditAcc,
                                  DebitAcc = ct.DebitAcc,
                                  AmountCur = ct.AmountCur,
                                  Amount = ct.Amount,
                                  Note = ct.Note,
                                  WorkPlaceCode = dp.WorkPlaceCode,
                                  FProductWorkCode = ct.FProductWorkCode,
                                  FProductWorkCodeBT = ct.ClearingFProductWorkCode,
                                  PartnerCode = ct.PartnerCode,
                                  PartnerCodeBt = ct.ClearingPartnerCode,
                                  SectionCode = ct.SectionCode,
                                  SectionCodeBt = ct.ClearingSectionCode,
                                  CaseCode = ct.CaseCode,
                                  TaxCategoryCode = dp.TaxCategoryCode,
                                  InvoiceDate = dp.InvoiceDate,
                                  InvoiceSerial = dp.InvoiceSymbol, //dp.InvoiceSerial,
                                  InvoiceNumbers = dp.InvoiceNumber,
                                  VatPercentage = dp.VatPercentage,
                                  PartnerNameBt = ct.ClearingPartnerCode,
                                  AddressInv = dp.AddressInv,
                                  TaxCode = dp.TaxCode,
                                  CurrencyCodeDebit = acc.AttachCurrency == "C" ? dp.CurrencyCode : null,
                                  ExchangeRateDebit = acc.AttachCurrency == "C" ? dp.ExchangeRate : 1,
                                  AmountCurDebit = acc.AttachCurrency == "C" ? ct.AmountCur : 0,
                                  PartnerCodeDebit = acc.AccPattern == 1 ? ct.PartnerCode : null,
                                  ContractCodeDebit = acc.AttachContract == "C" ? ct.ContractCode : null,
                                  FProductWorkCodeDebit = acc.AttachProductCost == "C" ? ct.FProductWorkCode : null,
                                  SectionCodeDebit = acc.AccSectionCode == "C" ? ct.SectionCode : null,
                                  WorkPlaceCodeDebit = acc.AttachWorkPlace == "C" ? ct.WorkPlaceCode : null,
                                  CurrencyCodeCredit = acc2.AttachCurrency == "C" ? dp.CurrencyCode : null,
                                  ExchangeRateCredit = acc2.AttachCurrency == "C" ? dp.ExchangeRate : 1,
                                  AmountCurCredit = acc2.AttachCurrency == "C" ? ct.AmountCur : 0,
                                  PartnerCodeCredit = acc2.AccPattern == 1 ? ct.PartnerCode : null,
                                  ContractCodeCredit = acc2.AttachContract == "C" ? ct.ContractCode : null,
                                  FProductWorkCodeCredit = acc2.AttachProductCost == "C" ? ct.FProductWorkCode : null,
                                  SectionCodeCredit = acc2.AccSectionCode == "C" ? ct.SectionCode : null,
                                  WorkPlaceCodeCredit = acc2.AttachWorkPlace == "C" ? ct.WorkPlaceCode : null,
                                  CheckDuplicate = "",
                                  WarehouseCode = "",
                                  AssemblyWarehouseCode = "",
                                  ProductCode = "",
                                  ProductLotCode = "",
                                  ProductOriginCode = "",
                                  UnitCode = "",
                                  Quantity = 0,
                                  PriceCur = 0,
                                  Price = 0,
                                  ProductName0 = ""

                              };
                var resultAllLedger = Enumerable.Concat(resultLedger, ledger3).ToList();
                for (int i = 0; i < resultAllLedger.Count; i++)
                {
                    CrudLedgerDto crudLedgerDto = new CrudLedgerDto();
                    crudLedgerDto.VoucherId = resultAllLedger[i].Id;
                    crudLedgerDto.OrgCode = resultAllLedger[i].OrgCode;
                    crudLedgerDto.Year = resultAllLedger[i].Year;
                    crudLedgerDto.DepartmentCode = resultAllLedger[i].DepartmentCode;
                    crudLedgerDto.VoucherCode = resultAllLedger[i].VoucherCode;
                    crudLedgerDto.VoucherGroup = resultAllLedger[i].VoucherGroup;
                    crudLedgerDto.CheckDuplicate = resultAllLedger[i].CheckDuplicate;
                    crudLedgerDto.CheckDuplicate0 = null;
                    crudLedgerDto.BusinessAcc = resultAllLedger[i].BusinessAcc;
                    crudLedgerDto.BusinessCode = resultAllLedger[i].BusinessCode;
                    crudLedgerDto.VoucherNumber = resultAllLedger[i].VoucherNumber;
                    crudLedgerDto.InvoiceNumber = resultAllLedger[i].InvoiceNumber;
                    crudLedgerDto.VoucherDate = resultAllLedger[i].VoucherDate;
                    crudLedgerDto.CurrencyCode = resultAllLedger[i].CurrencyCode;
                    crudLedgerDto.ExchangeRate = resultAllLedger[i].ExchangeRate;
                    crudLedgerDto.PartnerCode0 = resultAllLedger[i].PartnerCode0;
                    crudLedgerDto.Representative = resultAllLedger[i].Representative;
                    crudLedgerDto.Address = resultAllLedger[i].Address;
                    crudLedgerDto.Description = resultAllLedger[i].Description;
                    crudLedgerDto.OriginVoucher = resultAllLedger[i].OriginVoucher;
                    crudLedgerDto.Status = resultAllLedger[i].Status;
                    crudLedgerDto.Ord0Extra = resultAllLedger[i].Ord0;
                    crudLedgerDto.DebitAcc = resultAllLedger[i].DebitAcc;
                    crudLedgerDto.DebitCurrencyCode = resultAllLedger[i].CurrencyCodeDebit;
                    crudLedgerDto.DebitExchangeRate = resultAllLedger[i].ExchangeRateDebit;
                    crudLedgerDto.DebitPartnerCode = resultAllLedger[i].PartnerCodeDebit;
                    crudLedgerDto.DebitContractCode = resultAllLedger[i].ContractCodeDebit;
                    crudLedgerDto.DebitFProductWorkCode = resultAllLedger[i].FProductWorkCodeDebit;
                    crudLedgerDto.DebitSectionCode = resultAllLedger[i].SectionCodeDebit;
                    crudLedgerDto.DebitWorkPlaceCode = resultAllLedger[i].WorkPlaceCodeDebit;
                    crudLedgerDto.DebitAmountCur = resultAllLedger[i].AmountCurDebit;
                    crudLedgerDto.CreditAcc = resultAllLedger[i].CreditAcc;
                    crudLedgerDto.CreditCurrencyCode = resultAllLedger[i].CurrencyCodeCredit;
                    crudLedgerDto.CreditExchangeRate = resultAllLedger[i].ExchangeRateCredit;
                    crudLedgerDto.CreditPartnerCode = resultAllLedger[i].PartnerCodeCredit;
                    crudLedgerDto.CreditContractCode = resultAllLedger[i].ContractCodeCredit;
                    crudLedgerDto.CreditFProductWorkCode = resultAllLedger[i].FProductWorkCodeCredit;
                    crudLedgerDto.CreditSectionCode = resultAllLedger[i].SectionCodeCredit;
                    crudLedgerDto.CreditWorkPlaceCode = resultAllLedger[i].WorkPlaceCodeCredit;
                    crudLedgerDto.CreditAmountCur = resultAllLedger[i].AmountCurCredit;
                    crudLedgerDto.AmountCur = resultAllLedger[i].AmountCur;
                    crudLedgerDto.Amount = resultAllLedger[i].Amount;
                    crudLedgerDto.Note = resultAllLedger[i].Note;
                    crudLedgerDto.PartnerCode = resultAllLedger[i].PartnerCode;
                    crudLedgerDto.ContractCode = resultAllLedger[i].ContractCode;
                    crudLedgerDto.FProductWorkCode = resultAllLedger[i].FProductWorkCode;
                    crudLedgerDto.SectionCode = resultAllLedger[i].SectionCode;
                    crudLedgerDto.WorkPlaceCode = resultAllLedger[i].WorkPlaceCode;
                    crudLedgerDto.CaseCode = resultAllLedger[i].CaseCode;
                    crudLedgerDto.ClearingPartnerCode = resultAllLedger[i].PartnerCodeBt;
                    crudLedgerDto.ClearingContractCode = resultAllLedger[i].ContractCodeBT;
                    crudLedgerDto.ClearingFProductWorkCode = resultAllLedger[i].FProductWorkCodeBT;
                    crudLedgerDto.ClearingSectionCode = resultAllLedger[i].SectionCodeBt;
                    crudLedgerDto.TaxCategoryCode = resultAllLedger[i].TaxCategoryCode;
                    crudLedgerDto.InvoiceDate = resultAllLedger[i].InvoiceDate;
                    crudLedgerDto.InvoiceSymbol = resultAllLedger[i].InvoiceSerial;
                    crudLedgerDto.InvoiceNumber = resultAllLedger[i].InvoiceNumber;
                    crudLedgerDto.VatPercentage = resultAllLedger[i].VatPercentage;
                    crudLedgerDto.InvoicePartnerName = resultAllLedger[i].ProductName0;
                    crudLedgerDto.InvoicePartnerAddress = resultAllLedger[i].AddressInv;
                    crudLedgerDto.TaxCode = resultAllLedger[i].TaxCode;
                    crudLedgerDto.WarehouseCode = resultAllLedger[i].WarehouseCode;
                    crudLedgerDto.TransWarehouseCode = resultAllLedger[i].AssemblyWarehouseCode;
                    crudLedgerDto.ProductCode = resultAllLedger[i].ProductCode;
                    crudLedgerDto.ProductLotCode = resultAllLedger[i].ProductLotCode;
                    crudLedgerDto.ProductOriginCode = resultAllLedger[i].ProductOriginCode;
                    crudLedgerDto.UnitCode = resultAllLedger[i].UnitCode;
                    crudLedgerDto.Quantity = resultAllLedger[i].Quantity;
                    crudLedgerDto.PriceCur = resultAllLedger[i].PriceCur;
                    crudLedgerDto.Price = resultAllLedger[i].Price;
                    crudLedgerDto.ProductName0 = resultAllLedger[i].ProductName0;
                    ledgerDtos.Add(crudLedgerDto);


                }
            }
            return ledgerDtos;
        }
    }
}
