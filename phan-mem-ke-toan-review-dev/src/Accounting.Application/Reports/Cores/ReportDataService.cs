using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.Partners;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Ledgers;
using Accounting.Helpers;
using Accounting.Reports.ImportExports;
using Accounting.Reports.Others;
using Accounting.Vouchers;
using AutoMapper.Internal.Mappers;
using IdentityServer4.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Linq;

namespace Accounting.Reports.Cores
{
    public class ReportDataService : ITransientDependency
    {
        #region Fields
        private readonly LedgerService _ledgerService;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly IAsyncQueryableExecuter _asyncExecuter;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly ProductAppService _productAppService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly VoucherCategoryService _voucherCategoryService;
        #endregion
        #region Ctor
        public ReportDataService(LedgerService ledgerService,
                        WebHelper webHelper,
                        AccountingCacheManager accountingCacheManager,
                        IAsyncQueryableExecuter asyncExecuter,
                        AccOpeningBalanceService accOpeningBalanceService,
                        YearCategoryService yearCategoryService,
                        ProductAppService productAppService,
                        WarehouseBookService warehouseBookService,
                        ProductOpeningBalanceService productOpeningBalanceService,
                        VoucherCategoryService voucherCategoryService
                        )
        {
            _ledgerService = ledgerService;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _asyncExecuter = asyncExecuter;
            _accOpeningBalanceService = accOpeningBalanceService;
            _yearCategoryService = yearCategoryService;
            _productAppService = productAppService;
            _warehouseBookService = warehouseBookService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _voucherCategoryService = voucherCategoryService;
        }
        #endregion
        #region Methods
        public async Task<List<AccountBalanceDto>> GetAccBalancesAsync(Dictionary<string, object> parameters)
        {
            var result = new List<AccountBalanceDto>();

            if (parameters.ContainsKey(LedgerParameterConst.Year))
            {
                var openingData = await GetOpeningBalanceData(parameters);

                var openingDto = openingData.Select(p => new AccountBalanceDto()
                {
                    AccCode = p.AccCode,
                    ContractCode = p.ContractCode,
                    Credit = p.Credit,
                    CreditCur = p.CreditCur,
                    CreditIncurred = p.CreditIncurred,
                    CreditIncurredCur = p.CreditIncurredCur,
                    CurrencyCode = p.CurrencyCode,
                    Debit = p.Debit,
                    DebitCur = p.DebitCur,
                    DebitIncurred = p.DebitIncurred,
                    DebitIncurredCur = p.DebitIncurredCur,
                    FProductCode = p.FProductCode,
                    PartnerCode = p.PartnerCode,
                    SectionCode = p.SectionCode,
                    WorkPlaceCode = p.WorkPlaceCode
                }).ToList();
                result.AddRange(openingDto);
            }

            var ledgerData = await this.GetLedgerData(parameters);

            var ledgerDto = ledgerData.Select(p => new AccountBalanceDto()
            {
                AccCode = p.Acc,
                PartnerCode = p.PartnerCode,
                SectionCode = p.SectionCode,
                WorkPlaceCode = p.WorkPlaceCode,
                CurrencyCode = p.CurrencyCode,
                FProductCode = p.FProductWorkCode,
                ContractCode = p.ContractCode,
                Debit = p.DebitIncurred,
                DebitCur = p.DebitIncurredCur,
                Credit = p.CreditIncurred,
                CreditCur = p.CreditIncurredCur,
                CreditIncurred = p.CreditIncurred,
                CreditIncurredCur = p.CreditIncurredCur,
                DebitIncurred = p.DebitIncurred,
                DebitIncurredCur = p.DebitIncurredCur
            }).ToList();
            result.AddRange(ledgerDto);

            return result.GroupBy(g => new
            {
                g.AccCode,
                g.PartnerCode,
                g.SectionCode,
                g.WorkPlaceCode,
                g.CurrencyCode,
                g.FProductCode,
                g.ContractCode
            }).Select(p => new AccountBalanceDto()
            {
                AccCode = p.Key.AccCode,
                PartnerCode = p.Key.PartnerCode,
                SectionCode = p.Key.SectionCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                CurrencyCode = p.Key.CurrencyCode,
                FProductCode = p.Key.FProductCode,
                ContractCode = p.Key.ContractCode,
                Credit = p.Sum(s => s.Credit),
                CreditCur = p.Sum(s => s.CreditCur),
                CreditIncurred = p.Sum(s => s.CreditIncurred),
                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur),
                Debit = p.Sum(s => s.Debit),
                DebitCur = p.Sum(s => s.DebitCur),
                DebitIncurred = p.Sum(s => s.DebitIncurred),
                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur)
            }).ToList();
        }
        public async Task<List<AccountBalanceDto>> GetAccountBalancesAsync(Dictionary<string, object> parameters)
        {
            var openingData = await GetOpeningBalanceData(parameters);
            var beginDate = new DateTime(_webHelper.GetCurrentYear(), 1, 1);
            var fromDateParam = Convert.ToDateTime(parameters[LedgerParameterConst.FromDate]);
            var parameterBalances = new Dictionary<string, object>();
            foreach (var item in parameters)
            {
                parameterBalances.Add(item.Key, item.Value);
            }
            parameterBalances[LedgerParameterConst.FromDate] = beginDate;
            parameterBalances[LedgerParameterConst.ToDate] = fromDateParam;
            var ledgerData = await GetLedgerData(parameterBalances);
            var incurredData = ledgerData.Where(p => p.VoucherDate < fromDateParam).Select(p => new AccountBalanceDto()
            {
                AccCode = p.Acc,
                PartnerCode = p.PartnerCode,
                SectionCode = p.SectionCode,
                WorkPlaceCode = p.WorkPlaceCode,
                CurrencyCode = p.CurrencyCode,
                FProductCode = p.FProductWorkCode,
                ContractCode = p.ContractCode,
                Debit = p.DebitIncurred,
                DebitCur = p.DebitIncurredCur,
                Credit = p.CreditIncurred,
                CreditCur = p.CreditIncurredCur,
                CreditIncurred = p.CreditIncurred,
                CreditIncurredCur = p.CreditIncurredCur,
                DebitIncurred = p.DebitIncurred,
                DebitIncurredCur = p.DebitIncurredCur
            }).ToList();

            var unionData = openingData.Union(incurredData);
            return unionData.GroupBy(p => new
            {
                p.AccCode,
                PartnerCode = p.PartnerCode ?? "",
                SectionCode = p.SectionCode ?? "",
                WorkPlaceCode = p.WorkPlaceCode ?? "",
                CurrencyCode = p.CurrencyCode ?? "",
                ContractCode = p.ContractCode ?? "",
                FProductCode = p.FProductCode ?? ""
            }).Select(p => new AccountBalanceDto()
            {
                AccCode = p.Key.AccCode,
                PartnerCode = p.Key.PartnerCode,
                SectionCode = p.Key.SectionCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                CurrencyCode = p.Key.CurrencyCode,
                ContractCode = p.Key.ContractCode,
                FProductCode = p.Key.FProductCode,
                Debit = p.Sum(s => s.Debit),
                DebitCur = p.Sum(s => s.DebitCur),
                Credit = p.Sum(s => s.Credit),
                CreditCur = p.Sum(s => s.CreditCur),
                CreditIncurred = p.Sum(p => p.CreditIncurred),
                CreditIncurredCur = p.Sum(p => p.CreditIncurredCur),
                DebitIncurred = p.Sum(p => p.DebitIncurred),
                DebitIncurredCur = p.Sum(p => p.DebitIncurredCur)
            }).ToList();
        }
        public async Task<List<LedgerGeneralDto>> GetLedgerData(Dictionary<string, object> parameters)
        {
            string debitOrCredit = parameters[LedgerParameterConst.DebitOrCredit].ToString();
            var queryable = await GetGeneralLedgerQueryable(parameters);
            switch (debitOrCredit)
            {
                case "C":
                    var creditQueryable = GetCreditLedgerQueryable(queryable, parameters);
                    return await GetCreditLedger(creditQueryable);
                case "N":
                    var debitQueryable = GetDebitLedgerQueryable(queryable, parameters);
                    return await GetDebitLedger(debitQueryable);
                default:
                    return await GetLedgerDataByTypeAll(parameters);
            }
        }
        public async Task<List<WarehouseBookGeneralDto>> GetWarehouseBookData(Dictionary<string, object> parameters)
        {
            var queryable = await GetWarehouseBookQueryable(parameters);
            var wareHouseBooks = await _asyncExecuter.ToListAsync(queryable);
            var dtos = wareHouseBooks.Select(p => new WarehouseBookGeneralDto()
            {
                Address = p.Address,
                Amount = p.Amount,
                Amount2 = p.Amount2,
                AmountCur = p.AmountCur,
                AmountCur2 = p.AmountCur2,
                BillNumber = p.BillNumber,
                BusinessAcc = p.BusinessAcc,
                BusinessCode = p.BusinessCode,
                CalculateTransfering = p.CalculateTransfering,
                CaseCode = p.CaseCode,
                ContractCode = p.ContractCode,
                CreationTime = p.CreationTime,
                CreditAcc = p.CreditAcc,
                CreatorName = p.CreatorName,
                CreditAcc2 = p.CreditAcc2,
                CurrencyCode = p.CurrencyCode,
                DebitAcc = p.DebitAcc,
                DebitAcc2 = p.DebitAcc2,
                DebitOrCredit = p.DebitOrCredit,
                DepartmentCode = p.DepartmentCode,
                Description = p.Description,
                DescriptionE = p.DescriptionE,
                DevaluationAmount = p.DevaluationAmount,
                DevaluationAmountCur = p.DevaluationAmountCur,
                DevaluationPercentage = p.DevaluationPercentage,
                DevaluationPrice = p.DevaluationPrice,
                DevaluationPriceCur = p.DevaluationPriceCur,
                DiscountAmount = p.DiscountAmount,
                DiscountAmountCur = p.DiscountAmountCur,
                DiscountPercentage = p.DiscountPercentage,
                ExchangeRate = p.ExchangeRate,
                ExciseTaxAmount = p.ExciseTaxAmount,
                ExciseTaxAmountCur = p.ExciseTaxAmountCur,
                ExciseTaxPercentage = p.ExciseTaxPercentage,
                ExpenseAmount = p.ExpenseAmount,
                ExpenseAmount0 = p.ExpenseAmount0,
                ExpenseAmount1 = p.ExpenseAmount1,
                ExpenseAmountCur0 = p.ExpenseAmountCur0,
                ExpenseAmountCur1 = p.ExpenseAmountCur1,
                ExportAcc = p.ExportAcc,
                ExportAmount = p.ExportAmount,
                ExportAmountCur = p.ExportAmountCur,
                ExportQuantity = p.ExportQuantity,
                ExprenseAmountCur = p.ExprenseAmountCur,
                FixedPrice = p.FixedPrice,
                FProductWorkCode = p.FProductWorkCode,
                Id = p.Id,
                ImportAcc = p.ImportAcc,
                ImportAmount = p.ImportAmount,
                ImportAmountCur = p.ImportAmountCur,
                ImportQuantity = p.ImportQuantity,
                ImportTaxAmount = p.ImportTaxAmount,
                ImportTaxAmountCur = p.ImportTaxAmountCur,
                ImportTaxPercentage = p.ImportTaxPercentage,
                InvoiceDate = p.InvoiceDate,
                InvoiceNumber = p.InvoiceNumber,
                InvoicePartnerAddress = p.InvoicePartnerAddress,
                InvoicePartnerName = p.InvoicePartnerName,
                InvoiceSymbol = p.InvoiceSymbol,
                LastModificationTime = p.LastModificationTime,
                LastModifierId = p.LastModifierId,
                Note = p.Note,
                NoteE = p.NoteE,
                Ord0 = p.Ord0,
                Ord0Extra = p.Ord0Extra,
                OrgCode = p.OrgCode,
                OriginVoucher = p.OriginVoucher,
                PartnerCode = p.PartnerCode,
                PartnerCode0 = p.PartnerCode0,
                PaymentTermsCode = p.PaymentTermsCode,
                Place = p.Place,
                Price = p.Price,
                Price0 = p.Price0,
                Price2 = p.Price2,
                PriceCur = p.PriceCur,
                PriceCur0 = p.PriceCur0,
                PriceCur2 = p.PriceCur2,
                ProductCode = p.ProductCode,
                ProductLotCode = p.ProductLotCode,
                ProductOriginCode = p.ProductOriginCode,
                Quantity = p.Quantity,
                Representative = p.Representative,
                SalesChannelCode = p.SalesChannelCode,
                SectionCode = p.SectionCode,
                Status = p.Status,
                TaxCode = p.TaxCode,
                Tel = p.Tel,
                TransferingUnitCode = p.TransferingUnitCode,
                TransProductCode = p.TransProductCode,
                TransProductLotCode = p.TransProductLotCode,
                TransProductOriginCode = p.TransProductOriginCode,
                TransWarehouseCode = p.TransWarehouseCode,
                TrxExportQuantity = p.TrxExportQuantity,
                TrxImportQuantity = p.TrxImportQuantity,
                TrxQuantity = p.TrxQuantity,
                UnitCode = p.UnitCode,
                VarianceAmount = p.VarianceAmount,
                VatAmount = p.VatAmount,
                VatAmountCur = p.VatAmountCur,
                VatPercentage = p.VatPercentage,
                VatPrice = p.VatPrice,
                VatPriceCur = p.VatPriceCur,
                VoucherCode = p.VoucherCode,
                VoucherDate = p.VoucherDate,
                VoucherGroup = p.VoucherGroup,
                VoucherId = p.ProductVoucherId,
                VoucherNumber = p.VoucherNumber,
                WarehouseCode = p.WarehouseCode,
                WorkPlaceCode = p.WorkPlaceCode,
                Year = p.Year,
                AccCode = !string.IsNullOrEmpty(p.ImportAcc) ? p.ImportAcc : p.ExportAcc,
                TotalAmount2 = p.TotalAmount2
            }).ToList();
            return dtos;
        }
        #endregion
        #region Private
        private async Task<IQueryable<Ledger>> GetGeneralLedgerQueryable(Dictionary<string, object> parameters)
        {
            var queryable = await _ledgerService.GetQueryableAsync();
            queryable = queryable.Where(p => p.Status != VoucherStatusType.UnSave);

            foreach (var entry in parameters)
            {
                if (entry.Value == null) continue;
                if (string.IsNullOrEmpty(entry.Value.ToString())) continue;

                if (entry.Key.Equals(LedgerParameterConst.LstOrgCode))
                {
                    string lstOrgCode = entry.Value.ToString() + ",";
                    queryable = queryable.Where(p => lstOrgCode.Contains(p.OrgCode + ","));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.FromDate))
                {
                    DateTime date = Convert.ToDateTime(entry.Value);
                    queryable = queryable.Where(p => p.VoucherDate >= date);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ToDate))
                {
                    DateTime date = Convert.ToDateTime(entry.Value);
                    queryable = queryable.Where(p => p.VoucherDate <= date);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.CurrencyCode))
                {
                    string currencyCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CurrencyCode.Equals(currencyCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.BusinessCode))
                {
                    string businessCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.BusinessCode.Equals(businessCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.DepartmentCode))
                {
                    string departmentCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DepartmentCode.Equals(departmentCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.CaseCode))
                {
                    string caseCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CaseCode.Equals(caseCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.LstVoucherCode))
                {
                    string lstVoucherCode = entry.Value.ToString();
                    queryable = queryable.Where(p => lstVoucherCode.Contains(p.VoucherCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.CreatorName))
                {
                    string creatorName = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CreatorName.Equals(creatorName));
                    continue;
                }
            }
            return queryable;
        }
        private IQueryable<Ledger> GetDebitLedgerQueryable(IQueryable<Ledger> queryable, Dictionary<string, object> parameters)
        {
            queryable = queryable.Where(p => p.CheckDuplicate != CkktType.C);
            queryable = queryable.Where(p => p.CheckDuplicate0 != CkktType.C);

            foreach (var entry in parameters)
            {
                if (entry.Value == null) continue;
                if (string.IsNullOrEmpty(entry.Value.ToString())) continue;
                if (entry.Key.Equals(LedgerParameterConst.Acc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitAcc.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalAcc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CreditAcc.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.PartnerCode))
                {
                    string partnerCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.PartnerCode.Equals(partnerCode) ||
                                        p.DebitPartnerCode.Equals(partnerCode) || p.ClearingPartnerCode.Equals(partnerCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalPartnerCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ClearingPartnerCode.Equals(code) ||
                                        p.CreditPartnerCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.WorkPlaceCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WorkPlaceCode.Equals(code) ||
                                        p.DebitWorkPlaceCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ContractCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ContractCode.Equals(code) ||
                                        p.DebitContractCode.Equals(code) || p.ClearingContractCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.FProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.FProductWorkCode.StartsWith(code) ||
                                        p.DebitFProductWorkCode.StartsWith(code) || p.ClearingFProductWorkCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalFProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CreditFProductWorkCode.StartsWith(code)
                                        || p.ClearingFProductWorkCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.SectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.SectionCode.Equals(code) ||
                                        p.DebitSectionCode.Equals(code) || p.ClearingSectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalSectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p =>
                                        p.CreditSectionCode.Equals(code) || p.ClearingSectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.BeginVoucherNumber))
                {
                    string value = entry.Value.ToString();
                    queryable = queryable.Where(p => string.Compare(p.VoucherNumber, value) >= 0);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.EndVoucherNumber))
                {
                    string value = entry.Value.ToString();
                    queryable = queryable.Where(p => string.Compare(p.VoucherNumber, value) <= 0);
                    continue;
                }
            }
            return queryable;
        }
        private IQueryable<Ledger> GetCreditLedgerQueryable(IQueryable<Ledger> queryable, Dictionary<string, object> parameters)
        {
            queryable = queryable.Where(p => p.CheckDuplicate != CkktType.N);
            queryable = queryable.Where(p => p.CheckDuplicate0 != CkktType.N);

            foreach (var entry in parameters)
            {
                if (entry.Value == null) continue;
                if (string.IsNullOrEmpty(entry.Value.ToString())) continue;
                if (entry.Key.Equals(LedgerParameterConst.Acc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CreditAcc.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalAcc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitAcc.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.PartnerCode))
                {
                    string partnerCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.PartnerCode.Equals(partnerCode) ||
                                        p.CreditPartnerCode.Equals(partnerCode) || p.ClearingPartnerCode.Equals(partnerCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalPartnerCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ClearingPartnerCode.Equals(code) ||
                                        p.DebitPartnerCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.WorkPlaceCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WorkPlaceCode.Equals(code) ||
                                        p.CreditWorkPlaceCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ContractCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ContractCode.Equals(code) ||
                                        p.CreditContractCode.Equals(code) || p.ClearingContractCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.FProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.FProductWorkCode.StartsWith(code) ||
                                        p.CreditFProductWorkCode.StartsWith(code) || p.ClearingFProductWorkCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalFProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitFProductWorkCode.StartsWith(code)
                                        || p.ClearingFProductWorkCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.SectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.SectionCode.Equals(code) ||
                                        p.CreditSectionCode.Equals(code) || p.ClearingSectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalSectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p =>
                                        p.DebitSectionCode.Equals(code) || p.ClearingSectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.BeginVoucherNumber))
                {
                    string value = entry.Value.ToString();
                    queryable = queryable.Where(p => string.Compare(p.VoucherNumber, value) >= 0);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.EndVoucherNumber))
                {
                    string value = entry.Value.ToString();
                    queryable = queryable.Where(p => string.Compare(p.VoucherNumber, value) <= 0);
                    continue;
                }
            }
            return queryable;
        }
        private IQueryable<Ledger> GetTypeAllLedgerQueryable(IQueryable<Ledger> queryable, Dictionary<string, object> parameters)
        {
            queryable = queryable.Where(p => p.CheckDuplicate != CkktType.C);
            queryable = queryable.Where(p => p.CheckDuplicate0 != CkktType.C);

            foreach (var entry in parameters)
            {
                if (entry.Value == null) continue;
                if (string.IsNullOrEmpty(entry.Value.ToString())) continue;

                if (entry.Key.Equals(LedgerParameterConst.Acc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CreditAcc.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalAcc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitAcc.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.PartnerCode))
                {
                    string partnerCode = entry.Value.ToString();
                    queryable = queryable.Where(p => p.PartnerCode.Equals(partnerCode) ||
                                        p.CreditPartnerCode.Equals(partnerCode) || p.ClearingPartnerCode.Equals(partnerCode));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalPartnerCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ClearingPartnerCode.Equals(code) ||
                                        p.DebitPartnerCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.WorkPlaceCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WorkPlaceCode.Equals(code) ||
                                        p.CreditWorkPlaceCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ContractCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ContractCode.Equals(code) ||
                                        p.CreditContractCode.Equals(code) || p.ClearingContractCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.FProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.FProductWorkCode.StartsWith(code) ||
                                        p.CreditFProductWorkCode.StartsWith(code) || p.ClearingFProductWorkCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalFProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitFProductWorkCode.StartsWith(code)
                                        || p.ClearingFProductWorkCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.SectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.SectionCode.Equals(code) ||
                                        p.CreditSectionCode.Equals(code) || p.ClearingSectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ReciprocalSectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p =>
                                        p.DebitSectionCode.Equals(code) || p.ClearingSectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.BeginVoucherNumber))
                {
                    string value = entry.Value.ToString();
                    queryable = queryable.Where(p => string.Compare(p.VoucherNumber, value) >= 0);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.EndVoucherNumber))
                {
                    string value = entry.Value.ToString();
                    queryable = queryable.Where(p => string.Compare(p.VoucherNumber, value) <= 0);
                    continue;
                }
            }

            return queryable;
        }
        private async Task<List<LedgerGeneralDto>> GetLedgerDataByTypeAll(Dictionary<string, object> parameters)
        {
            var queryable = await GetGeneralLedgerQueryable(parameters);

            var debitQueryable = GetDebitLedgerQueryable(queryable, parameters);
            var debitLedgers = await GetDebitLedger(debitQueryable);

            var creditQueryable = GetCreditLedgerQueryable(queryable, parameters);
            var creditLedgers = await GetCreditLedger(creditQueryable);

            var result = new List<LedgerGeneralDto>();
            result.AddRange(debitLedgers);
            result.AddRange(creditLedgers);

            return result;
        }
        private async Task<List<LedgerGeneralDto>> GetDebitLedger(IQueryable<Ledger> queryable)
        {
            var debitLedgers = await _asyncExecuter.ToListAsync(queryable);
            var result = new List<LedgerGeneralDto>();
            foreach (var ledger in debitLedgers)
            {
                var dto = new LedgerGeneralDto()
                {
                    IncurredType = "A",
                    Id = ledger.Id,
                    VoucherId = ledger.VoucherId,

                    Ord0 = ledger.Ord0,
                    OrgCode = ledger.OrgCode,
                    Year = ledger.Year,
                    DepartmentCode = ledger.DepartmentCode,
                    VoucherCode = ledger.VoucherCode,
                    VoucherGroup = ledger.VoucherGroup,
                    BusinessCode = ledger.BusinessCode,
                    BusinessAcc = ledger.BusinessAcc,
                    CheckDuplicate = ledger.CheckDuplicate,
                    VoucherNumber = ledger.VoucherNumber,
                    InvoiceNbr = ledger.InvoiceNbr,
                    RecordingVoucherNumber = ledger.RecordingVoucherNumber,
                    VoucherDate = ledger.VoucherDate,
                    Days = ledger.Days,
                    PaymentTermsCode = ledger.PaymentTermsCode,
                    Representative = ledger.Representative,
                    Address = ledger.Address,
                    Description = ledger.Description,
                    DescriptionE = ledger.DescriptionE,
                    OriginVoucher = ledger.OriginVoucher,
                    CreatorName = ledger.CreatorName,
                    CreationTime = ledger.CreationTime,
                    LastModificationTime = ledger.LastModificationTime,
                    LastModifierId = ledger.LastModifierId,
                    Status = ledger.Status,
                    DebitAcc = ledger.DebitAcc,
                    DebitCurrencyCode = ledger.DebitCurrencyCode,
                    DebitExchangeRate = ledger.DebitExchangeRate,
                    DebitPartnerCode = ledger.DebitPartnerCode,
                    DebitContractCode = ledger.DebitContractCode,
                    DebitFProductWorkCode = ledger.DebitFProductWorkCode,
                    DebitSectionCode = ledger.DebitSectionCode,
                    DebitWorkPlaceCode = ledger.DebitWorkPlaceCode,
                    DebitAmountCur = ledger.DebitAmountCur,
                    Acc = ledger.DebitAcc,
                    CurrencyCode = ledger.CurrencyCode,
                    ExchangeRate = ledger.ExchangeRate,
                    PartnerCode = ledger.VoucherCode != "BCN" ? ledger.PartnerCode : ledger.DebitPartnerCode,
                    ContractCode = ledger.ContractCode,
                    FProductWorkCode = ledger.FProductWorkCode,
                    SectionCode = ledger.SectionCode,
                    WorkPlaceCode = ledger.WorkPlaceCode,
                    CreditAcc = ledger.CreditAcc,
                    CreditCurrencyCode = ledger.CreditCurrencyCode,
                    CreditExchangeRate = ledger.CreditExchangeRate,
                    CreditPartnerCode = ledger.CreditPartnerCode,
                    CreditContractCode = ledger.CreditContractCode,
                    CreditFProductWorkCode = ledger.CreditFProductWorkCode,
                    CreditSectionCode = ledger.CreditSectionCode,
                    CreditWorkPlaceCode = ledger.CreditWorkPlaceCode,
                    CreditAmountCur = ledger.CreditAmountCur,
                    ReciprocalAcc = ledger.CreditAcc,
                    ReciprocalContractCode = ledger.CreditContractCode,
                    ReciprocalCurrencyCode = ledger.CreditCurrencyCode,
                    ReciprocalExchangeRate = ledger.CreditExchangeRate,
                    ReciprocalFProductWorkCode = ledger.CreditFProductWorkCode,
                    ReciprocalPartnerCode = ledger.CreditPartnerCode,
                    ReciprocalSectionCode = ledger.CreditSectionCode,
                    ReciprocalWorkPlaceCode = ledger.CreditWorkPlaceCode,
                    DebitIncurredCur = ledger.DebitAmountCur,
                    DebitIncurred = ledger.Amount,
                    CreditIncurred = 0,
                    CreditIncurredCur = 0,
                    Note = ledger.Note,
                    NoteE = ledger.NoteE,
                    AmountCur0 = ledger.AmountCur,
                    Amount0 = ledger.Amount,
                    ContractCode0 = ledger.ContractCode,
                    FProductWorkCode0 = ledger.FProductWorkCode,
                    PartnerCode0 = ledger.PartnerCode0,
                    PartnerName0 = ledger.PartnerName0,
                    SectionCode0 = ledger.SectionCode,
                    WorkPlaceCode0 = ledger.WorkPlaceCode,
                    CaseCode0 = ledger.CaseCode,
                    ClearingContractCode = ledger.ClearingContractCode,
                    ClearingFProductWorkCode = ledger.ClearingFProductWorkCode,
                    ClearingPartnerCode = ledger.ClearingPartnerCode,
                    ClearingSectionCode = ledger.ClearingSectionCode,
                    WarehouseCode = ledger.WarehouseCode,
                    TransWarehouseCode = ledger.TransWarehouseCode,
                    ProductCode = ledger.ProductCode,
                    ProductLotCode = ledger.ProductLotCode,
                    ProducOrginCode = ledger.ProductOriginCode,
                    UnitCode = ledger.UnitCode,
                    TrxQuantity = ledger.TrxQuantity,
                    Quantity = ledger.Quantity,
                    TrxPromotionQuantity = ledger.TrxPromotionQuantity,
                    PromotionQuantity = ledger.PromotionQuantity,
                    PriceCur = ledger.PriceCur,
                    Price = ledger.Price,
                    TaxCategoryCode = ledger.TaxCategoryCode,
                    VatPercenatge = ledger.VatPercentage,
                    InvoiceNumber = ledger.InvoiceNumber,
                    InvoiceSymbol = ledger.InvoiceSymbol,
                    InvoiceDate = ledger.InvoiceDate,
                    InvoicePartnerAddress = ledger.InvoicePartnerAddress,
                    InvoicePartnerName = ledger.InvoicePartnerName,
                    TaxCode = ledger.TaxCode,
                    DebitOrCredit = ledger.DebitOrCredit,
                    CheckDuplicate0 = ledger.CheckDuplicate0
                };
                result.Add(dto);
            }
            return result;
        }
        private async Task<List<LedgerGeneralDto>> GetCreditLedger(IQueryable<Ledger> queryable)
        {
            var creditLedgers = await _asyncExecuter.ToListAsync(queryable);
            var result = new List<LedgerGeneralDto>();
            foreach (var ledger in creditLedgers)
            {
                var dto = new LedgerGeneralDto()
                {
                    IncurredType = "B",
                    Id = ledger.Id,
                    VoucherId = ledger.VoucherId,

                    Ord0 = ledger.Ord0,
                    OrgCode = ledger.OrgCode,
                    Year = ledger.Year,
                    DepartmentCode = ledger.DepartmentCode,
                    VoucherCode = ledger.VoucherCode,
                    VoucherGroup = ledger.VoucherGroup,
                    BusinessCode = ledger.BusinessCode,
                    BusinessAcc = ledger.BusinessAcc,
                    CheckDuplicate = ledger.CheckDuplicate,
                    VoucherNumber = ledger.VoucherNumber,
                    InvoiceNbr = ledger.InvoiceNbr,
                    RecordingVoucherNumber = ledger.RecordingVoucherNumber,
                    VoucherDate = ledger.VoucherDate,
                    Days = ledger.Days,
                    PaymentTermsCode = ledger.PaymentTermsCode,
                    Representative = ledger.Representative,
                    Address = ledger.Address,
                    Description = ledger.Description,
                    DescriptionE = ledger.DescriptionE,
                    OriginVoucher = ledger.OriginVoucher,
                    CreatorName = ledger.CreatorName,
                    CreationTime = ledger.CreationTime,
                    LastModificationTime = ledger.LastModificationTime,
                    LastModifierId = ledger.LastModifierId,
                    Status = ledger.Status,
                    DebitAcc = ledger.DebitAcc,
                    DebitCurrencyCode = ledger.DebitCurrencyCode,
                    DebitExchangeRate = ledger.DebitExchangeRate,
                    DebitPartnerCode = ledger.DebitPartnerCode,
                    DebitContractCode = ledger.DebitContractCode,
                    DebitFProductWorkCode = ledger.DebitFProductWorkCode,
                    DebitSectionCode = ledger.DebitSectionCode,
                    DebitWorkPlaceCode = ledger.DebitWorkPlaceCode,
                    DebitAmountCur = ledger.DebitAmountCur,
                    Acc = ledger.CreditAcc,
                    CurrencyCode = ledger.CurrencyCode,
                    ExchangeRate = ledger.ExchangeRate,
                    PartnerCode = ledger.VoucherCode != "BCN" ? ledger.PartnerCode : ledger.CreditPartnerCode,
                    ContractCode = ledger.ContractCode,
                    FProductWorkCode = ledger.FProductWorkCode,
                    SectionCode = ledger.SectionCode,
                    WorkPlaceCode = ledger.WorkPlaceCode,
                    CreditAcc = ledger.CreditAcc,
                    CreditCurrencyCode = ledger.CreditCurrencyCode,
                    CreditExchangeRate = ledger.CreditExchangeRate,
                    CreditPartnerCode = ledger.CreditPartnerCode,
                    CreditContractCode = ledger.CreditContractCode,
                    CreditFProductWorkCode = ledger.CreditFProductWorkCode,
                    CreditSectionCode = ledger.CreditSectionCode,
                    CreditWorkPlaceCode = ledger.CreditWorkPlaceCode,
                    CreditAmountCur = ledger.CreditAmountCur,
                    ReciprocalAcc = ledger.DebitAcc,
                    ReciprocalContractCode = ledger.DebitContractCode,
                    ReciprocalCurrencyCode = ledger.DebitCurrencyCode,
                    ReciprocalExchangeRate = ledger.DebitExchangeRate,
                    ReciprocalFProductWorkCode = ledger.DebitFProductWorkCode,
                    ReciprocalPartnerCode = ledger.DebitPartnerCode,
                    ReciprocalSectionCode = ledger.DebitSectionCode,
                    ReciprocalWorkPlaceCode = ledger.DebitWorkPlaceCode,
                    DebitIncurredCur = 0,
                    DebitIncurred = 0,
                    CreditIncurred = ledger.Amount,
                    CreditIncurredCur = ledger.CreditAmountCur,
                    Note = ledger.Note,
                    NoteE = ledger.NoteE,
                    AmountCur0 = ledger.AmountCur,
                    Amount0 = ledger.Amount,
                    ContractCode0 = ledger.ContractCode,
                    FProductWorkCode0 = ledger.FProductWorkCode,
                    PartnerCode0 = ledger.PartnerCode0,
                    PartnerName0 = ledger.PartnerName0,
                    SectionCode0 = ledger.SectionCode,
                    WorkPlaceCode0 = ledger.WorkPlaceCode,
                    CaseCode0 = ledger.CaseCode,
                    ClearingContractCode = ledger.ClearingContractCode,
                    ClearingFProductWorkCode = ledger.ClearingFProductWorkCode,
                    ClearingPartnerCode = ledger.ClearingPartnerCode,
                    ClearingSectionCode = ledger.ClearingSectionCode,
                    WarehouseCode = ledger.WarehouseCode,
                    TransWarehouseCode = ledger.TransWarehouseCode,
                    ProductCode = ledger.ProductCode,
                    ProductLotCode = ledger.ProductLotCode,
                    ProducOrginCode = ledger.ProductOriginCode,
                    UnitCode = ledger.UnitCode,
                    TrxQuantity = ledger.TrxQuantity,
                    Quantity = ledger.Quantity,
                    TrxPromotionQuantity = ledger.TrxPromotionQuantity,
                    PromotionQuantity = ledger.PromotionQuantity,
                    PriceCur = ledger.PriceCur,
                    Price = ledger.Price,
                    TaxCategoryCode = ledger.TaxCategoryCode,
                    VatPercenatge = ledger.VatPercentage,
                    InvoiceNumber = ledger.InvoiceNumber,
                    InvoiceSymbol = ledger.InvoiceSymbol,
                    InvoiceDate = ledger.InvoiceDate,
                    InvoicePartnerAddress = ledger.InvoicePartnerAddress,
                    InvoicePartnerName = ledger.InvoicePartnerName,
                    TaxCode = ledger.TaxCode,
                    DebitOrCredit = ledger.DebitOrCredit,
                    CheckDuplicate0 = ledger.CheckDuplicate0
                };
                result.Add(dto);
            }
            return result;
        }
        private async Task<IQueryable<AccOpeningBalance>> GetOpeningBalanceQueryable(Dictionary<string, object> parameters)
        {
            var queryable = await _accOpeningBalanceService.GetQueryableAsync();

            foreach (var entry in parameters)
            {
                if (entry.Key.Equals(LedgerParameterConst.LstOrgCode))
                {
                    string lstOrgCode = entry.Value.ToString() + ",";
                    queryable = queryable.Where(p => lstOrgCode.Contains(p.OrgCode + ","));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.Year))
                {
                    int year = Convert.ToInt32(entry.Value.ToString());
                    queryable = queryable.Where(p => p.Year == year);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.Acc))
                {
                    string acc = entry.Value.ToString();
                    queryable = queryable.Where(p => p.AccCode.StartsWith(acc));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.PartnerCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.PartnerCode == code);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.SectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.AccSectionCode == code);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.WorkPlaceCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WorkPlaceCode == code);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.FProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.FProductWorkCode == code);
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.ContractCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ContractCode == code);
                    continue;
                }
            }
            return queryable;
        }
        private async Task<List<AccountBalanceDto>> GetOpeningBalanceData(Dictionary<string, object> paramerters)
        {
            var queryable = await GetOpeningBalanceQueryable(paramerters);
            var openingBalances = await _asyncExecuter.ToListAsync(queryable);

            return openingBalances.Select(p => new AccountBalanceDto()
            {
                AccCode = p.AccCode,
                PartnerCode = p.PartnerCode,
                WorkPlaceCode = p.WorkPlaceCode,
                FProductCode = p.FProductWorkCode,
                SectionCode = p.AccSectionCode,
                CurrencyCode = p.CurrencyCode,
                ContractCode = p.ContractCode,
                Credit = p.Credit,
                CreditCur = p.CreditCur,
                Debit = p.Debit,
                DebitCur = p.DebitCur
            }).ToList();
        }
        private async Task<IQueryable<WarehouseBook>> GetWarehouseBookQueryable(Dictionary<string, object> parameters)
        {
            var queryable = await _warehouseBookService.GetQueryableAsync();
            queryable = queryable.Where(p => p.Status != "2");
            string debitOrCredit = "*";
            if (parameters.ContainsKey(WarehouseBookParameterConst.DebitOrCredit))
            {
                debitOrCredit = parameters[WarehouseBookParameterConst.DebitOrCredit] == null ? "*"
                                    : parameters[WarehouseBookParameterConst.DebitOrCredit].ToString();
            }

            foreach (var entry in parameters)
            {
                if (entry.Value == null) continue;
                if (string.IsNullOrEmpty(entry.Value.ToString())) continue;
                if (entry.Key.Equals(WarehouseBookParameterConst.OrgCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.OrgCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.LstOrgCode))
                {
                    string lstOrgCode = entry.Value.ToString() + ",";
                    queryable = queryable.Where(p => lstOrgCode.Contains(p.OrgCode + ","));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.VoucherGroup))
                {
                    int voucherGroup = Convert.ToInt32(entry.Value.ToString());
                    queryable = queryable.Where(p => p.VoucherGroup == voucherGroup);
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.FromDate))
                {
                    DateTime fromDate = Convert.ToDateTime(entry.Value);
                    queryable = queryable.Where(p => p.VoucherDate >= fromDate);
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ToDate))
                {
                    DateTime toDate = Convert.ToDateTime(entry.Value);
                    queryable = queryable.Where(p => p.VoucherDate <= toDate);
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.DepartmentCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DepartmentCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ChannelCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ChannelCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.BusinessCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.BusinessCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.CurrencyCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CurrencyCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.LstVoucherCode))
                {
                    string lstVoucherCode = entry.Value.ToString();
                    queryable = queryable.Where(p => lstVoucherCode.Contains(p.VoucherCode));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.WarehouseCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WarehouseCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ProductCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductLotCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ProductLotCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductOriginCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ProductOriginCode.Equals(code));
                    continue;
                }          
                if (entry.Key.Equals(WarehouseBookParameterConst.WorkPlaceCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WorkPlaceCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.PartnerCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.PartnerCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.SectionCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.SectionCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.CaseCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CaseCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ContractCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ContractCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.FProductWorkCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.FProductWorkCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductGroupCode))
                {
                    string code = entry.Value.ToString();
                    var lstProduct = await _productAppService.GetListByProductGroupCode(code);
                    queryable = queryable.Where(p => lstProduct.Select(a => a.Code).Contains(p.ProductCode));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.DebitAcc))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitAcc.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.CreditAcc2))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.CreditAcc2.StartsWith(code));
                    continue;
                }
                /*if (entry.Key.Equals(WarehouseBookParameterConst.Acc))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ImportAcc.StartsWith(code) || p.ExportAcc.StartsWith(code));
                    continue;
                }*/               
                if (entry.Key.Equals(WarehouseBookParameterConst.Acc))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.DebitAcc.StartsWith(code) || p.CreditAcc.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.Acc1))
                {
                    string code = entry.Value.ToString();
                    if (debitOrCredit.Equals("*"))
                    {
                        queryable = queryable.Where(p => p.DebitAcc.StartsWith(code) || p.CreditAcc.StartsWith(code)
                                        || p.DebitAcc2.StartsWith(code) || p.CreditAcc2.StartsWith(code));
                    }
                    else if (debitOrCredit.Equals("C"))
                    {
                        queryable = queryable.Where(p => p.CreditAcc.StartsWith(code) || p.CreditAcc2.StartsWith(code));
                    }
                    else if (debitOrCredit.Equals("N"))
                    {
                        queryable = queryable.Where(p => p.DebitAcc.StartsWith(code) || p.DebitAcc2.StartsWith(code));
                    }

                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ReciprocalAcc))
                {
                    string code = entry.Value.ToString();
                    if (debitOrCredit.Equals("*"))
                    {
                        queryable = queryable.Where(p => p.DebitAcc.StartsWith(code) || p.CreditAcc.StartsWith(code)
                                        || p.DebitAcc2.StartsWith(code) || p.CreditAcc2.StartsWith(code));
                    }
                    else if (debitOrCredit.Equals("N"))
                    {
                        queryable = queryable.Where(p => p.CreditAcc.StartsWith(code) || p.CreditAcc2.StartsWith(code));
                    }
                    else if (debitOrCredit.Equals("C"))
                    {
                        queryable = queryable.Where(p => p.DebitAcc.StartsWith(code) || p.DebitAcc2.StartsWith(code));
                    }

                    continue;
                }
            }
            return queryable;
        }
        private async Task<IQueryable<ProductOpeningBalance>> GetProductOpeningBalancesQueryableAsync(Dictionary<string, object> parameters)
        {
            var queryable = await _productOpeningBalanceService.GetQueryableAsync();
            foreach (var entry in parameters)
            {
                if (entry.Value == null) continue;
                if (entry.Key.Equals(WarehouseBookParameterConst.OrgCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.OrgCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(LedgerParameterConst.LstOrgCode))
                {
                    string lstOrgCode = entry.Value.ToString();
                    queryable = queryable.Where(p => lstOrgCode.Contains(p.OrgCode));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.Year))
                {
                    int year = Convert.ToInt32(entry.Value);
                    queryable = queryable.Where(p => p.Year == year);
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.OpenYear))
                {
                    int year = Convert.ToInt32(entry.Value);
                    queryable = queryable.Where(p => p.Year == year);
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.WarehouseCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.WarehouseCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.Acc))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.AccCode.StartsWith(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ProductCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductGroupCode))
                {
                    string code = entry.Value.ToString();
                    var lstProduct = await _productAppService.GetListByProductGroupCode(code);
                    queryable = queryable.Where(p => lstProduct.Select(a => a.Code).Contains(p.ProductCode));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductLotCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ProductLotCode.Equals(code));
                    continue;
                }
                if (entry.Key.Equals(WarehouseBookParameterConst.ProductOriginCode))
                {
                    string code = entry.Value.ToString();
                    queryable = queryable.Where(p => p.ProductOriginCode.Equals(code));
                    continue;
                }
            }
            return queryable;
        }
        public async Task<List<ProductBalanceDto>> GetProductOpeningBalancesAsync(Dictionary<string, object> parameters)
        {
            var queryable = await this.GetProductOpeningBalancesQueryableAsync(parameters);
            var result = queryable.GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,
                g.AccCode,
                g.ProductCode,
                g.ProductLotCode,
                g.ProductOriginCode
            }).Select(p => new ProductBalanceDto()
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                ProductCode = p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode,
                ImportQuantity = p.Sum(s => s.Quantity),
                ImportAmount = p.Sum(s => s.Amount),
                ImportAmountCur = p.Sum(s => s.AmountCur)
            }).ToList();
            return result;
        }
        public async Task<List<ProductBalanceDto>> GetProductBalancesAsync(Dictionary<string, object> parameters)
        {
            var productOpenings = await GetProductOpeningBalancesAsync(parameters);
            var tets = parameters["fromDate"].ToString();
            var wareHouseBookDtos = await GetWarehouseBookData(parameters);
            var wareHouseBalanceDtos = wareHouseBookDtos.Where(p=>p.VoucherDate<DateTime.Parse(parameters["toDate"].ToString())).GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,
                g.AccCode,
                g.ProductCode,
                g.ProductLotCode,
                g.ProductOriginCode
            }).Select(p => new ProductBalanceDto()
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                ProductCode = p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode,
                ImportQuantity = p.Sum(s => s.ImportQuantity),
                ImportAmount = p.Sum(s => s.ImportAmount),
                ImportAmountCur = p.Sum(s => s.ImportAmountCur),
                ExportQuantity = p.Sum(s => s.ExportQuantity),
                ExportAmount = p.Sum(s => s.ExportAmount),
                ExportAmountCur = p.Sum(s => s.ExportAmountCur)
            }).ToList();

            var result = new List<ProductBalanceDto>();
            result.AddRange(productOpenings);
            result.AddRange(wareHouseBalanceDtos);

            result = result.GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,
                g.AccCode,
                g.ProductCode,
                g.ProductLotCode,
                g.ProductOriginCode
            }).Where(p => p.Key.OrgCode == _webHelper.GetCurrentOrgUnit()).Select(p => new ProductBalanceDto()
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                ProductCode = p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode,
                ImportQuantity = p.Sum(s => s.ImportQuantity),
                ImportAmount = p.Sum(s => s.ImportAmount),
                ImportAmountCur = p.Sum(s => s.ImportAmountCur),
                ExportQuantity = p.Sum(s => s.ExportQuantity),
                ExportAmount = p.Sum(s => s.ExportAmount),
                ExportAmountCur = p.Sum(s => s.ExportAmountCur)
            }).ToList();

            return result;
        }

        public async Task<List<IEInventoryDto>> GetIEInventoryAsync(Dictionary<string, object> parameters)
        {
            var beginDate = new DateTime(_webHelper.GetCurrentYear(), 1, 1);
            var fromDateParam = Convert.ToDateTime(parameters[LedgerParameterConst.FromDate]);
            var parameterBalances = new Dictionary<string, object>();
            foreach (var item in parameters)
            {
                parameterBalances.Add(item.Key, item.Value);
            }
            parameterBalances[LedgerParameterConst.FromDate] = beginDate;
            parameterBalances[LedgerParameterConst.ToDate] = fromDateParam;
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var warehouseBook = await this.GetWarehouseBookData(parameters);
            var productOpeningBalance = await this.GetProductBalancesAsync(parameterBalances);
            var isTransfer = parameters.Where(p => p.Key == "isTransfer").Select(p => p.Value).FirstOrDefault() ?? "0";
            if (isTransfer.ToString() == "True")
            {
                warehouseBook = (from a in warehouseBook
                                 join b in lstVoucherCategory on a.VoucherCode equals b.Code
                                 where b.IsTransfer != "C"
                                 select a).ToList();
            }
            var lstWarehouseBook = warehouseBook.GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,
                g.AccCode,
                g.ProductCode,
                ProductLotCode = g.ProductLotCode ?? "",
                ProductOriginCode = g.ProductOriginCode ?? "",
                g.VoucherGroup
            }).Select(p => new IEInventoryDto
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                ProductCode = p.Key.ProductCode == null ? "" : p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode == null ? "" : p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode == null ? "" : p.Key.ProductOriginCode,
                ImportQuantity = p.Sum(s => s.ImportQuantity ?? 0),
                ImportAmount = p.Sum(s => s.ImportAmount ?? 0),
                ImportAmountCur = p.Sum(s => s.ImportAmountCur ?? 0),
                ExportQuantity = p.Sum(s => s.ExportQuantity ?? 0),
                ExportAmount = p.Sum(s => s.ExportAmount ?? 0),
                ExportAmountCur = p.Sum(s => s.ExportAmountCur ?? 0),
                ImportQuantity2 = (p.Key.VoucherGroup == 1) ? p.Sum(s => s.ImportQuantity ?? 0) : p.Sum(s => s.ExportQuantity * (-1) ?? 0),
                ImportAmount2 = (p.Key.VoucherGroup == 1) ? p.Sum(s => s.ImportAmount ?? 0) : p.Sum(s => s.ExportAmount * (-1) ?? 0),
                ImportAmountCur2 = (p.Key.VoucherGroup == 1) ? p.Sum(s => s.ImportAmountCur ?? 0) : p.Sum(s => s.ExportAmountCur * (-1) ?? 0),
                Amount2 = p.Sum(s => s.Amount2 ?? 0),
                AmountCur2 = p.Sum(s => s.AmountCur2 ?? 0),
            }).ToList();
            var lstProductOpeningBalance = productOpeningBalance.GroupBy(g => new
            {
                g.OrgCode,
                g.WarehouseCode,
                g.AccCode,
                g.ProductCode,
                ProductLotCode = g.ProductLotCode ?? "",
                ProductOriginCode = g.ProductOriginCode ?? "",
            }).Select(p => new IEInventoryDto
            {
                WarehouseCode = p.Key.WarehouseCode,
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                ProductCode = p.Key.ProductCode,
                ProductLotCode = p.Key.ProductLotCode,
                ProductOriginCode = p.Key.ProductOriginCode,
                ImportQuantity1 = p.Sum(s => (s.ImportQuantity ?? 0) - (s.ExportQuantity ?? 0)),
                ImportAmount1 = p.Sum(s => (s.ImportAmount ?? 0) - (s.ExportAmount ?? 0)),
                ImportAmountCur1 = p.Sum(s => (s.ImportAmountCur ?? 0) - (s.ExportAmountCur ?? 0)),
                ImportQuantity2 = p.Sum(s => (s.ImportQuantity ?? 0) - (s.ExportQuantity ?? 0)),
                ImportAmount2 = p.Sum(s => (s.ImportAmount ?? 0) - (s.ExportAmount ?? 0)),
                ImportAmountCur2 = p.Sum(s => (s.ImportAmountCur ?? 0) - (s.ExportAmountCur ?? 0))
            }).ToList();
            var data = Enumerable.Concat(lstWarehouseBook, lstProductOpeningBalance).ToList();
            data = (from a in data
                    where a.ProductCode != null && a.ProductCode != ""
                    group new { a }
                    by new
                    {
                        a.OrgCode,
                        a.WarehouseCode,
                        a.AccCode,
                        a.ProductCode,
                        ProductLotCode = a.ProductLotCode ?? "",
                        ProductOriginCode = a.ProductOriginCode ?? "",
                    }
                   into g
                    where g.Sum(p => p.a.ImportQuantity1) != 0 || g.Sum(p => p.a.ImportAmount1) != 0 || g.Sum(p => p.a.ImportQuantity) != 0 || g.Sum(p => p.a.ImportAmount) != 0 || g.Sum(p => p.a.ImportQuantity2) != 0 || g.Sum(p => p.a.ImportAmountCur2) != 0
                    select new IEInventoryDto
                    {
                        WarehouseCode = g.Key.WarehouseCode,
                        OrgCode = g.Key.OrgCode,
                        AccCode = g.Key.AccCode,
                        ProductCode = g.Key.ProductCode,
                        ProductLotCode = g.Key.ProductLotCode,
                        ProductOriginCode = g.Key.ProductOriginCode,
                        ImportQuantity = g.Sum(s => s.a.ImportQuantity ?? 0),
                        ImportAmount = g.Sum(s => s.a.ImportAmount ?? 0),
                        ImportAmountCur = g.Sum(s => s.a.ImportAmountCur ?? 0),
                        ExportQuantity = g.Sum(s => s.a.ExportQuantity ?? 0),
                        ExportAmount = g.Sum(s => s.a.ExportAmount ?? 0),
                        ExportAmountCur = g.Sum(s => s.a.ExportAmountCur ?? 0),
                        ImportQuantity1 = g.Sum(s => s.a.ImportQuantity1 ?? 0),
                        ImportAmount1 = g.Sum(s => s.a.ImportAmount1 ?? 0),
                        ImportAmountCur1 = g.Sum(s => s.a.ImportAmountCur1 ?? 0),
                        ImportQuantity2 = g.Sum(s => s.a.ImportQuantity2 ?? 0),
                        ImportAmount2 = g.Sum(s => s.a.ImportAmount2 ?? 0),
                        ImportAmountCur2 = g.Sum(s => s.a.ImportAmountCur2 ?? 0),
                        Amount2 = g.Sum(s => s.a.Amount2 ?? 0),
                        AmountCur2 = g.Sum(s => s.a.AmountCur2 ?? 0),
                    }).ToList();
            return data;
        }


        #endregion
    }
}
