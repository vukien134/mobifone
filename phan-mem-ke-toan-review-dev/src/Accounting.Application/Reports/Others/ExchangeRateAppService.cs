using System;
using Accounting.DomainServices.Categories;
using Accounting.Helpers;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Hosting;
using Accounting.DomainServices.Ledgers;
using System.Linq;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Accounting.Report;
using Accounting.Constants;
using System.IO;
using Accounting.Categories.Accounts;
using NPOI.SS.Formula.Functions;
using System.Reflection;
using Accounting.Catgories.Others;
using Accounting.DomainServices.Vouchers;
using Org.BouncyCastle.Utilities;
using System.Text.Json.Nodes;
using Accounting.Caching;

namespace Accounting.Reports.Others
{
    public class ExchangeRateAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
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
        private readonly LedgerService _ledgerService;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly CurrencyService _currencyService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly AccVoucherService _accVoucherService;
        private readonly AccVoucherDetailService _accVoucherDetailService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly ProductVoucherCostService _productVoucherCostService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly ProductVoucherReceiptService _productVoucherReceiptService;
        #endregion
        public ExchangeRateAppService(
            ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        AccountingCacheManager accountingCacheManager,
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
                        LedgerService ledgerService,
                        ProductOpeningBalanceService productOpeningBalanceService,
                        AccOpeningBalanceService accOpeningBalanceService,
                        CurrencyService currencyService,
                        DefaultVoucherCategoryService defaultVoucherCategoryService,
                        AccVoucherService accVoucherService,
                        AccVoucherDetailService accVoucherDetailService,
                        AccTaxDetailService accTaxDetailService,
                        ProductVoucherCostService productVoucherCostService,
                        ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
                         ProductVoucherReceiptService productVoucherReceiptService)
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
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
            _ledgerService = ledgerService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _accOpeningBalanceService = accOpeningBalanceService;
            _currencyService = currencyService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _accVoucherService = accVoucherService;
            _accVoucherDetailService = accVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _productVoucherCostService = productVoucherCostService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
        }
        public async Task<JsonObject> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var removeDuplicate = await _tenantSettingService.GetValue("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var lstAccCodeTs = "1,2,6,8,9";
            var lstAccCodeNv = "3,4,5,7";
            var dgLai = "Lãi tỷ giá";
            var dglo = "Lỗ tỷ giá";

            var lstLedger = await _ledgerService.GetQueryableAsync();
            var lstLedgerss = (lstLedger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.VoucherDate >= dto.Parameters.FromDate
                                          && p.VoucherDate <= dto.Parameters.ToDate
                                         )).ToList();
            //var lstLedgers = lstLedgerss.Where(p => p.Ord0Extra.Contains("L") == true);
            //await _ledgerService.DeleteManyAsync(lstLedgers);
            var accountSystems = await _accountSystemService.GetAccounts(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            var accOpeningBalance = await _accOpeningBalanceService.GetAccOpeningBalances();
            var productOpeningBalanceExchanRate = (from a in accOpeningBalance
                                                   join b in accountSystems on a.AccCode equals b.AccCode
                                                   group new
                                                   {
                                                       a.AccCode,
                                                       a.PartnerCode,
                                                       a.WorkPlaceCode,
                                                       a.FProductWorkCode,
                                                       a.AccSectionCode,
                                                       a.ContractCode,
                                                       a.Debit,
                                                       a.DebitCur,
                                                       a.Credit,
                                                       a.CreditCur
                                                   } by new
                                                   {
                                                       a.AccCode,
                                                       a.PartnerCode,
                                                       a.WorkPlaceCode,
                                                       a.FProductWorkCode,
                                                       a.AccSectionCode,
                                                       a.ContractCode

                                                   } into gr
                                                   select new
                                                   {
                                                       PfKey = gr.Key.AccCode + gr.Key.PartnerCode + gr.Key.WorkPlaceCode + gr.Key.FProductWorkCode + gr.Key.AccSectionCode + gr.Key.ContractCode,
                                                       DebitCur = (decimal)(gr.Sum(p => p.DebitCur) - gr.Sum(p => p.CreditCur)),
                                                       Debit = (decimal)(gr.Sum(p => p.Debit) - gr.Sum(p => p.Credit)),
                                                   }).ToList();

            var productOpeningBalanceExchanRate1 = (from a in lstLedgerss.ToList()
                                                    join b in accountSystems on a.DebitAcc equals b.AccCode
                                                    where a.VoucherDate < dto.Parameters.FromDate
                                                    && (removeDuplicate != "C" || a.CheckDuplicate != "C") && a.CheckDuplicate0 != "C"
                                                    group new
                                                    {
                                                        a.DebitAcc,
                                                        a.DebitPartnerCode,
                                                        a.DebitContractCode,
                                                        a.DebitFProductWorkCode,
                                                        a.DebitWorkPlaceCode,
                                                        a.DebitSectionCode,
                                                        a.DebitAmountCur,
                                                        a.Amount
                                                    } by new
                                                    {
                                                        a.DebitAcc,
                                                        a.DebitPartnerCode,
                                                        a.DebitContractCode,
                                                        a.DebitFProductWorkCode,
                                                        a.DebitWorkPlaceCode,
                                                        a.DebitSectionCode
                                                    } into gr
                                                    select new
                                                    {
                                                        PfKey = gr.Key.DebitAcc + gr.Key.DebitPartnerCode + gr.Key.DebitWorkPlaceCode + gr.Key.DebitFProductWorkCode + gr.Key.DebitSectionCode + gr.Key.DebitContractCode,
                                                        DebitCur = (decimal)gr.Sum(p => p.DebitAmountCur),
                                                        Debit = (decimal)gr.Sum(p => p.Amount)
                                                    }).ToList();
            productOpeningBalanceExchanRate.AddRange(productOpeningBalanceExchanRate1);

            var productOpeningBalanceExchanRate2 = (from a in lstLedgerss.ToList()
                                                    join b in accountSystems on a.CreditAcc equals b.AccCode
                                                    where a.VoucherDate < dto.Parameters.FromDate
                                                    && (removeDuplicate != "C" || a.CheckDuplicate != "C") && a.CheckDuplicate0 != "C"
                                                    group new
                                                    {
                                                        a.CreditAcc,
                                                        a.CreditPartnerCode,
                                                        a.CreditContractCode,
                                                        a.CreditFProductWorkCode,
                                                        a.CreditWorkPlaceCode,
                                                        a.CreditSectionCode,
                                                        a.DebitAmountCur,
                                                        a.Amount
                                                    } by new
                                                    {
                                                        a.CreditAcc,
                                                        a.CreditPartnerCode,
                                                        a.CreditContractCode,
                                                        a.CreditFProductWorkCode,
                                                        a.CreditWorkPlaceCode,
                                                        a.CreditSectionCode,
                                                    } into gr
                                                    select new
                                                    {
                                                        PfKey = gr.Key.CreditAcc + gr.Key.CreditPartnerCode + gr.Key.CreditWorkPlaceCode + gr.Key.CreditFProductWorkCode + gr.Key.CreditSectionCode + gr.Key.CreditContractCode,
                                                        DebitCur = (decimal)gr.Sum(p => p.DebitAmountCur),
                                                        Debit = (decimal)gr.Sum(p => p.Amount)
                                                    }).ToList();
            productOpeningBalanceExchanRate.AddRange(productOpeningBalanceExchanRate2);
            var resultOpeningBalanceExchanRate = (from a in productOpeningBalanceExchanRate
                                                  group new
                                                  {
                                                      a.PfKey,
                                                      a.Debit,
                                                      a.DebitCur
                                                  } by new
                                                  {
                                                      a.PfKey
                                                  } into gr
                                                  select new
                                                  {
                                                      PfKey = gr.Key.PfKey,
                                                      Debit = gr.Sum(p => p.Debit),
                                                      DebitCur = gr.Sum(p => p.DebitCur)
                                                  }).ToList();
            var lstcurrency = await _currencyService.GetListAsync(_webHelper.GetCurrentOrgUnit());


            var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();
            var ledgerExchanRate = (from a in lstLedgerss.ToList()
                                    join b in accountSystems on a.DebitAcc equals b.AccCode
                                    join c in accountSystems on a.CreditAcc equals c.AccCode
                                    join d in lstcurrency on a.CurrencyCode equals d.Code
                                    where a.VoucherDate >= dto.Parameters.FromDate && a.VoucherDate <= dto.Parameters.ToDate
                                    && a.AmountCur != 0
                                    && (removeDuplicate != "C" || a.CheckDuplicate != "C") && a.CheckDuplicate0 != "C"
                                    select new
                                    {
                                        a.Id,
                                        a.Year,
                                        a.Ord0Extra,
                                        a.Ord0,
                                        a.VoucherCode,
                                        a.VoucherDate,
                                        a.VoucherNumber,
                                        a.CurrencyCode,
                                        a.ExchangeRate,
                                        a.CreationTime,
                                        DebitTsNv = a.DebitAcc.Substring(0, 1).Contains(lstAccCodeTs) == true ? "T" : "N",
                                        DebitAccCur = b.AttachCurrency == null ? "K" : b.AttachCurrency,
                                        PfKeyDebit = a.DebitAcc + a.DebitPartnerCode + a.DebitWorkPlaceCode + a.DebitFProductWorkCode + a.DebitSectionCode + a.DebitContractCode,
                                        a.DebitAcc,
                                        a.DebitPartnerCode,
                                        a.DebitWorkPlaceCode,
                                        a.DebitFProductWorkCode,
                                        a.DebitSectionCode,
                                        a.DebitContractCode,
                                        a.DebitCurrencyCode,
                                        a.DebitExchangeRate,
                                        a.DebitAmountCur,
                                        CreditTsNv = a.CreditAcc.Substring(0, 1).Contains(lstAccCodeTs) == true ? "T" : "N",
                                        CreditAccCur = c.AttachCurrency == null ? "K" : c.AttachCurrency,
                                        PfKeyCredit = a.CreditAcc + a.CreditPartnerCode + a.CreditWorkPlaceCode + a.CreditFProductWorkCode + a.CreditSectionCode + a.CreditContractCode,
                                        a.CreditAcc,
                                        a.CreditPartnerCode,
                                        a.CreditWorkPlaceCode,
                                        a.CreditFProductWorkCode,
                                        a.CreditSectionCode,
                                        a.CreditContractCode,
                                        a.CreditCurrencyCode,
                                        a.CreditExchangeRate,
                                        a.CreditAmountCur,
                                        a.Amount,
                                        a.AmountCur,
                                        d.ExchangeMethod,//Ct
                                        VoucherKind = "", //DP,
                                        a.VoucherId
                                    }).OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).ThenBy(p => p.Ord0).ToList();

            //Update vào đầu kỳ những mã phát sinh thêm
            var sumLedgerCltgDebit = (from a in ledgerExchanRate
                                      group new
                                      {
                                          a.PfKeyDebit,
                                          a.DebitAmountCur,
                                          a.Amount
                                      } by new
                                      {
                                          a.PfKeyDebit
                                      } into gr
                                      select new
                                      {
                                          PfKey = gr.Key.PfKeyDebit,
                                          Debit = gr.Sum(p => p.Amount),
                                          DebitCur = gr.Sum(p => p.DebitAmountCur)
                                      }).ToList();

            var sumLedgerCltgCredit = (from a in ledgerExchanRate
                                       group new
                                       {
                                           a.PfKeyCredit,
                                           a.CreditAmountCur,
                                           a.Amount
                                       } by new
                                       {
                                           a.PfKeyCredit
                                       } into gr
                                       select new
                                       {
                                           PfKey = gr.Key.PfKeyCredit,
                                           Debit = -gr.Sum(p => p.Amount),
                                           DebitCur = -gr.Sum(p => p.CreditAmountCur)
                                       }).ToList();
            sumLedgerCltgDebit.AddRange(sumLedgerCltgCredit);

            var resultOpeningBalanceExchanRate1 = (from a in sumLedgerCltgDebit
                                                       //join b in resultOpeningBalanceExchanRate on a.PfKey equals b.PfKey into c
                                                       //from d in c.DefaultIfEmpty()
                                                       //where string.IsNullOrEmpty(d.PfKey)
                                                   group new
                                                   {
                                                       a.PfKey
                                                   } by new
                                                   {
                                                       a.PfKey
                                                   } into gr
                                                   select new
                                                   {
                                                       PfKey = gr.Key.PfKey,
                                                       Debit = (decimal)0,
                                                       DebitCur = (decimal)0
                                                   }).ToList();
            resultOpeningBalanceExchanRate.AddRange(resultOpeningBalanceExchanRate1);
            resultOpeningBalanceExchanRate = (from a in resultOpeningBalanceExchanRate
                                              group new
                                              {
                                                  a.PfKey,
                                                  a.Debit,
                                                  a.DebitCur
                                              } by new
                                              {
                                                  a.PfKey
                                              } into gr
                                              select new
                                              {
                                                  PfKey = gr.Key.PfKey,
                                                  Debit = gr.Sum(p => p.Debit),
                                                  DebitCur = gr.Sum(p => p.DebitCur)
                                              }).ToList();
            var id = "";
            var year = 0;
            var ord0 = "";
            var voucherCode = "";
            DateTime voucherDate;
            var voucherNumber = "";
            var currencyCode = "";
            decimal exchangeRate = 0;
            var debitTsNv = "";
            var debitAccCur = "";
            var creditTsNv = "";
            var creditAccCur = "";
            var pfKeyCredit = "";
            var pfKeyDebit = "";
            var debitAcc = "";
            var debitPartnerCode = "";
            var debitWorkPlaceCode = "";
            var debitFProductWorkCode = "";
            var debitSectionCode = "";
            var debitContractCode = "";
            var debitCurrencyCode = "";
            decimal debitExchangeRate = 0;
            decimal debitAmountCur = 0;
            var creditAcc = "";
            var creditPartnerCode = "";
            var creditWorkPlaceCode = "";
            var creditFProductWorkCode = "";
            var creditSectionCode = "";
            var creditContractCode = "";
            var creditCurrencyCode = "";
            decimal creditExchangeRate = 0;
            decimal creditAmountCur = 0;
            decimal amount = 0;
            var update = "";
            decimal debitAmount = 0;
            decimal amountRefunds = 0;
            decimal creditAmount = 0;
            decimal amountCur = 0;
            bool exchangeMethod;
            decimal amount0 = 0;
            string voucherId = "";
            while (ledgerExchanRate.Count > 0)
            {
                var ledgerExchanRates = ledgerExchanRate.OrderBy(p => p.VoucherDate).ThenBy(p => p.CreationTime).ThenBy(p => p.Ord0).FirstOrDefault();
                id = ledgerExchanRates.Id;
                year = ledgerExchanRates.Year;
                ord0 = ledgerExchanRates.Ord0;
                voucherCode = ledgerExchanRates.VoucherCode;
                voucherDate = ledgerExchanRates.VoucherDate;
                voucherNumber = ledgerExchanRates.VoucherNumber;
                currencyCode = ledgerExchanRates.CurrencyCode;
                exchangeRate = (decimal)ledgerExchanRates.ExchangeRate;
                debitTsNv = ledgerExchanRates.DebitTsNv;
                debitAccCur = ledgerExchanRates.DebitAccCur;
                creditTsNv = ledgerExchanRates.CreditTsNv;
                creditAccCur = ledgerExchanRates.CreditAccCur;
                pfKeyCredit = ledgerExchanRates.PfKeyCredit;
                pfKeyDebit = ledgerExchanRates.PfKeyDebit;
                debitAcc = ledgerExchanRates.DebitAcc;
                debitPartnerCode = ledgerExchanRates.DebitPartnerCode;
                debitWorkPlaceCode = ledgerExchanRates.DebitWorkPlaceCode;
                debitFProductWorkCode = ledgerExchanRates.DebitFProductWorkCode;
                debitSectionCode = ledgerExchanRates.DebitSectionCode;
                debitContractCode = ledgerExchanRates.DebitContractCode;
                debitCurrencyCode = ledgerExchanRates.DebitCurrencyCode;
                debitExchangeRate = (decimal)ledgerExchanRates.DebitExchangeRate;
                debitAmountCur = (decimal)ledgerExchanRates.DebitAmountCur;
                creditAcc = ledgerExchanRates.CreditAcc;
                creditPartnerCode = ledgerExchanRates.CreditPartnerCode;
                creditWorkPlaceCode = ledgerExchanRates.CreditWorkPlaceCode;
                creditFProductWorkCode = ledgerExchanRates.CreditFProductWorkCode;
                creditSectionCode = ledgerExchanRates.CreditSectionCode;
                creditContractCode = ledgerExchanRates.CreditContractCode;
                creditCurrencyCode = ledgerExchanRates.CreditCurrencyCode;
                creditExchangeRate = (decimal)ledgerExchanRates.CreditExchangeRate;
                creditAmountCur = (decimal)ledgerExchanRates.CreditAmountCur;
                amountCur = (decimal)ledgerExchanRates.AmountCur;
                exchangeMethod = ledgerExchanRates.ExchangeMethod;
                amount = (decimal)ledgerExchanRates.Amount;
                voucherId = ledgerExchanRates.VoucherId;
                if (creditAmountCur == 0 && debitAmountCur == 0)
                {
                    var resultOpeningBalanceExchanRate2 = (from a in resultOpeningBalanceExchanRate
                                                           where a.PfKey == pfKeyCredit || a.PfKey == pfKeyDebit
                                                           select new
                                                           {
                                                               a.PfKey,
                                                               Debit = (decimal)(a.Debit + (a.PfKey == pfKeyDebit ? 1 : -1) * amount),

                                                           }).ToList();
                    resultOpeningBalanceExchanRate = (from a in resultOpeningBalanceExchanRate
                                                      join b in resultOpeningBalanceExchanRate2 on a.PfKey equals b.PfKey into c
                                                      from d in c.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          a.PfKey,
                                                          Debit = d != null ? d.Debit : a.Debit,
                                                          a.DebitCur
                                                      }).ToList();

                }
                else
                {

                    update = "K";
                    amountRefunds = 0;//tien_cl
                    //Tính Dư đầu kỳ
                    var resultOpeningBalanceExchanRate0 = (from a in resultOpeningBalanceExchanRate
                                                           where a.PfKey == pfKeyCredit || a.PfKey == pfKeyDebit
                                                           group new
                                                           {
                                                               a.PfKey,
                                                               a.DebitCur,
                                                               a.Debit
                                                           } by new
                                                           {
                                                               a.PfKey
                                                           } into gr
                                                           select new
                                                           {
                                                               PfKey = gr.Key.PfKey,
                                                               DebitAmountCur = gr.Key.PfKey == pfKeyDebit ? gr.Sum(p => p.DebitCur) : 0,
                                                               DebitAmount = gr.Key.PfKey == pfKeyDebit ? gr.Sum(p => p.Debit) : 0,
                                                               CreditAmountCur = gr.Key.PfKey == pfKeyCredit ? gr.Sum(p => p.DebitCur) : 0,
                                                               CreateAmount = gr.Key.PfKey == pfKeyCredit ? gr.Sum(p => p.Debit) : 0
                                                           }).ToList().FirstOrDefault();
                    debitAmountCur = resultOpeningBalanceExchanRate0.DebitAmountCur;
                    debitAmount = resultOpeningBalanceExchanRate0.DebitAmount;
                    creditAmountCur = resultOpeningBalanceExchanRate0.CreditAmountCur;
                    creditAmount = resultOpeningBalanceExchanRate0.CreateAmount;
                    // --Các trường hợp
                    //--1.Cả 2 bên đều theo dõi ngoại tệ
                    if (debitAccCur == "C" && creditAccCur == "C")
                    {//I.3.1
                        if (debitAmountCur >= 0 && creditAmountCur >= 0)
                        {
                            if (creditAmountCur >= amountCur)// I.3.1.a	-- I.3.1.d
                            {
                                update = "C";
                                debitExchangeRate = exchangeRate;
                                if (exchangeMethod == true)
                                {
                                    creditExchangeRate = Math.Round(creditAmount / (decimal)creditAmountCur, 6);
                                    amount = Math.Round((decimal)amountCur * (debitExchangeRate >= creditExchangeRate ? (decimal)creditExchangeRate : (decimal)debitExchangeRate), 0);
                                    amountRefunds = (decimal)(Math.Round((decimal)amountCur * (decimal)debitExchangeRate - (decimal)amountCur * (decimal)creditExchangeRate, 0));
                                }
                                else
                                {
                                    if (creditAmount != 0)
                                    {
                                        creditExchangeRate = Math.Round((decimal)creditAmountCur / (decimal)creditAmount, 6);
                                        amount = Math.Round((decimal)amountCur / (debitExchangeRate > creditExchangeRate ? (decimal)creditExchangeRate : (decimal)debitExchangeRate), 0);
                                        amountRefunds = Math.Round((decimal)amountCur / (decimal)debitExchangeRate - (decimal)amountCur / (decimal)creditExchangeRate, 0);
                                    }
                                }
                            }
                            if (creditAmountCur < amountCur)//-- I.3.1.b -- I.3.1.c
                            {
                                update = "C";
                                debitExchangeRate = exchangeRate;
                                if (exchangeMethod == true)
                                {
                                    creditExchangeRate = Math.Round((decimal)(creditAmount + (decimal)(amountCur - creditAmountCur) * (decimal)exchangeRate) / amountCur, 6);
                                    amount = Math.Round((decimal)amountCur * (debitExchangeRate >= creditExchangeRate ? (decimal)creditExchangeRate : (decimal)debitExchangeRate), 0);
                                    amountRefunds = Math.Round((decimal)amountCur * (decimal)debitExchangeRate - (decimal)amountCur * (decimal)creditExchangeRate, 0);
                                }
                                else
                                {
                                    creditExchangeRate = Math.Round((decimal)amountCur / (creditAmount + (decimal)(amountCur - creditAmountCur) / (decimal)exchangeRate), 6);
                                    amount = Math.Round((decimal)amountCur / (debitExchangeRate >= creditExchangeRate ? (decimal)creditExchangeRate : (decimal)debitExchangeRate), 0);
                                    amountRefunds = Math.Round((decimal)amountCur / (decimal)debitExchangeRate - (decimal)amountCur / (decimal)creditExchangeRate, 0);
                                }
                            }
                            // trường hợp đặc biệt
                            if ("111".Contains(debitAcc) == true && "112".Contains(creditAcc))
                            {
                                amountRefunds = 0;
                                debitExchangeRate = creditExchangeRate;
                                if (exchangeMethod == true)
                                {
                                    amount = Math.Round((decimal)amountCur * (decimal)creditExchangeRate, 0);
                                }
                                else
                                {
                                    amount = Math.Round((decimal)amountCur / (decimal)creditExchangeRate, 0);
                                }
                            }
                        }
                        // I.3.2
                        if (debitAmountCur >= 0 && creditAmountCur < 0)
                        {
                            update = "C";
                            debitExchangeRate = exchangeRate;
                            creditExchangeRate = exchangeRate;
                            if (exchangeMethod == true)
                            {
                                amount = Math.Round((decimal)amountCur * (decimal)exchangeRate, 0);
                            }
                            else
                            {
                                amount = Math.Round((decimal)amountCur / (decimal)exchangeRate, 0);
                            }
                        }
                        //I.3.3
                        if (debitAmountCur < 0 && creditAmountCur < 0)
                        {
                            update = "C";
                            creditExchangeRate = exchangeRate;
                            if (Math.Abs((decimal)debitAmountCur) > amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((decimal)debitAmount / (decimal)debitAmountCur, 10);
                                    amount = Math.Round((decimal)amountCur / (decimal)debitExchangeRate, 0);
                                    amountRefunds = Math.Round((decimal)amountCur * (decimal)debitExchangeRate - (decimal)amountCur * (decimal)creditExchangeRate, 0);
                                }
                                else
                                {
                                    debitExchangeRate = Math.Round((decimal)debitAmountCur / (decimal)debitAmount, 10);
                                    amount = Math.Round((decimal)amountCur / (decimal)debitExchangeRate, 0);
                                    amountRefunds = Math.Round((decimal)amountCur / (decimal)debitExchangeRate - (decimal)amountCur / (decimal)creditExchangeRate, 0);
                                }
                            }
                            else
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((Math.Abs((decimal)debitAmount) + ((decimal)amountCur - Math.Abs((decimal)debitAmountCur)) * (decimal)exchangeRate) / (decimal)amountCur, 10);
                                    amount = Math.Round((decimal)amountCur * (decimal)debitExchangeRate, 0);
                                    amountRefunds = Math.Round((decimal)amountCur * (decimal)debitExchangeRate - (decimal)amountCur * (decimal)creditExchangeRate, 0);
                                }
                                else
                                {
                                    debitExchangeRate = Math.Round((decimal)amountCur / (Math.Round(Math.Abs((decimal)creditAmount)) + ((decimal)amountCur - Math.Round((decimal)creditAmountCur)) / (decimal)exchangeRate), 10);
                                    amount = Math.Round((decimal)amountCur / (decimal)debitExchangeRate);
                                    amountRefunds = Math.Round((decimal)amountCur / (decimal)debitExchangeRate - (decimal)amountCur / (decimal)creditExchangeRate, 0);
                                }
                            }
                            // trường hợp đặc biệt
                            if ("111".Contains(debitAcc) && " 112".Contains(creditAcc))
                            {
                                amountRefunds = 0;
                                debitExchangeRate = creditExchangeRate;
                                if (exchangeMethod == true)
                                {
                                    amount = Math.Round((decimal)amountCur * (decimal)creditExchangeRate, 0);
                                }
                                else
                                {
                                    amount = Math.Round((decimal)amountCur / (decimal)creditExchangeRate, 0);
                                }
                            }
                        }
                        // I3.3.4
                        if (debitAmountCur <= 0 && creditAmountCur >= 0)
                        {
                            debitExchangeRate = exchangeRate;
                            creditExchangeRate = exchangeRate;
                            if (Math.Round((decimal)debitAmountCur) >= amountCur && creditAmountCur >= amountCur)// I3.4.a
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((decimal)debitAmount / (decimal)debitAmountCur, 6);
                                    creditExchangeRate = Math.Round((decimal)creditAmount / (decimal)creditAmountCur, 6);
                                }
                                else
                                {
                                    if (debitAmount != 0)
                                    {
                                        debitExchangeRate = Math.Round((decimal)debitAmountCur / (debitAmount), 6);
                                    }
                                    if (creditAmount != 0)
                                    {
                                        creditExchangeRate = Math.Round((decimal)creditAmountCur / (creditAmount), 6);
                                    }
                                }
                            }
                            // I 3.4.b
                            if (Math.Abs((decimal)debitAmountCur) >= amountCur && creditAmountCur < amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((decimal)debitAmount / (decimal)(debitAmountCur), 6);
                                    creditExchangeRate = Math.Round(((decimal)creditAmount + ((decimal)amountCur - (decimal)creditAmountCur) / (decimal)exchangeRate) / (decimal)amountCur, 6);

                                }
                                else
                                {
                                    if (debitAmount != 0)
                                    {
                                        debitExchangeRate = Math.Round((decimal)debitAmountCur / (decimal)debitAmount, 6);
                                    }
                                    if (creditAmount != 0)
                                    {
                                        creditExchangeRate = Math.Round((decimal)amountCur / ((decimal)creditAmount + ((decimal)amountCur - (decimal)creditAmountCur) * (decimal)exchangeRate), 6);
                                    }
                                }

                            }
                            // I3.4.c
                            if (Math.Abs(debitAmountCur) < amountCur && creditAmountCur < amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((Math.Abs(debitAmount) + (amountCur - Math.Abs(debitAmountCur)) * exchangeRate) / amountCur, 6);
                                    creditExchangeRate = Math.Round((Math.Abs(creditAmount) + (amountCur - Math.Abs(debitAmountCur)) * exchangeRate) / amountCur, 6);
                                }
                                else
                                {
                                    if (debitAmount != 0)
                                    {
                                        debitExchangeRate = Math.Round(amountCur / Math.Abs(debitAmount) + (amountCur - Math.Abs(debitAmountCur)) / exchangeRate, 6);
                                    }
                                    if (creditAmount != 0)
                                    {
                                        creditExchangeRate = Math.Round(amountCur / Math.Abs(creditAmount) + (amountCur - Math.Abs(creditAmountCur)) / exchangeRate, 6);
                                    }
                                }
                            }
                            // I3.4.d
                            if (Math.Abs((decimal)debitAmountCur) < amountCur && creditAmountCur >= amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((Math.Abs((decimal)debitAmount) + ((decimal)amountCur - Math.Round((decimal)debitAmountCur)) * (decimal)exchangeRate) / (decimal)amountCur, 6);
                                    creditExchangeRate = Math.Round((decimal)creditAmount / (decimal)creditAmountCur, 6);
                                }
                                else
                                {
                                    if (debitAmount != 0)
                                    {
                                        debitExchangeRate = Math.Round((decimal)amountCur / Math.Abs(debitAmount) + ((decimal)amountCur - Math.Abs((decimal)creditAmountCur)) / (decimal)exchangeRate, 6);
                                    }
                                    if (creditAmount != 0)
                                    {
                                        creditExchangeRate = Math.Round((decimal)creditAmountCur / (decimal)creditAmount, 0);
                                    }
                                }
                            }
                            if (debitExchangeRate != creditExchangeRate)
                            {
                                update = "C";
                                amountRefunds = 0;
                                if (exchangeMethod == true)
                                {
                                    amount = Math.Round((decimal)amountCur * (debitExchangeRate >= creditExchangeRate ? (decimal)creditExchangeRate : (decimal)debitExchangeRate), 0);
                                    amountRefunds = Math.Round((decimal)amountCur * (decimal)(debitExchangeRate) - (decimal)amountCur * (decimal)creditExchangeRate, 0);
                                }
                                else
                                {
                                    amount = Math.Round((decimal)amountCur / (debitExchangeRate >= creditExchangeRate ? (decimal)debitExchangeRate : (decimal)creditExchangeRate), 0);
                                    amountRefunds = Math.Round((decimal)amountCur / (decimal)debitExchangeRate - (decimal)amountCur / (decimal)creditExchangeRate, 0);
                                }
                                // trường hợp đặc biệt
                                if ("111".Contains(debitAcc) && "112".Contains(creditAcc))
                                {
                                    amountRefunds = 0;
                                    debitExchangeRate = creditExchangeRate;
                                    if (exchangeMethod == true)
                                    {
                                        amount = Math.Round((decimal)amountCur * (decimal)creditExchangeRate, 0);

                                    }
                                    else
                                    {
                                        amount = Math.Round((decimal)amountCur / (decimal)creditExchangeRate);
                                    }
                                }
                            }
                        }
                    }
                    // 2.Tk nợ là tài sản theo dõi ngoại tệ và tk có không theo dõi ngoại tệ
                    if (debitTsNv == "T" && debitAccCur == "C" && creditAccCur != "C")
                    {
                        // những tình huống về số liệu
                        if (debitAmountCur >= 0) // I.1.1
                        {
                            update = "C";
                            debitExchangeRate = exchangeRate;
                            if (exchangeMethod == true)
                            {
                                amount = Math.Round((decimal)amountCur * (decimal)debitExchangeRate, 0);
                            }
                            else
                            {
                                amount = Math.Round((decimal)amountCur / (decimal)debitExchangeRate, 0);
                            }
                            amount0 = amount;
                        }
                        if (debitAmountCur >= 0)//I.1.2
                        {
                            update = "C";
                            if (Math.Abs(debitAmountCur) >= amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round(debitAmount / debitAmountCur, 6);
                                    amount = Math.Round(amountCur * debitExchangeRate, 0);
                                    amount0 = Math.Round(amountCur * exchangeRate, 0);
                                }
                                else
                                {
                                    debitExchangeRate = Math.Round(debitAmountCur / debitAmount, 6);
                                    amount = Math.Round(amountCur / debitExchangeRate, 0);
                                    amount0 = Math.Round(amountCur / exchangeRate, 0);
                                }
                            }
                            else
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((Math.Abs(debitAmount) + (amountCur - Math.Abs(debitAmountCur)) * exchangeRate) / amountCur, 6);
                                    amount = Math.Round(amountCur * debitExchangeRate, 0);
                                    amount0 = Math.Round(amountCur * exchangeRate, 0);
                                }
                                else
                                {
                                    debitExchangeRate = Math.Round(amountCur / (Math.Abs(debitAmount) + (amountCur - Math.Abs(debitAmountCur)) / exchangeRate), 6);
                                    amount = Math.Round(amountCur / debitExchangeRate, 0);
                                    amount0 = Math.Round(amountCur / exchangeRate, 0);
                                }
                            }

                        }
                    }
                    //3Tk nợ là tài sản không theo dõi ngoại tệ, tài khoản có theo dõi ngoại tệ
                    if (debitTsNv == "T" && debitAccCur != "C" && creditAccCur == "C")
                    {
                        if (creditAmountCur >= 0)// I.2.1
                        {
                            update = "C";
                            if (creditAmountCur >= amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    creditExchangeRate = Math.Round(creditAmount / creditAmountCur, 6);
                                    amount = Math.Round(amountCur * creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur * exchangeRate, 0);
                                }
                                else
                                {
                                    creditExchangeRate = Math.Round(creditAmountCur / creditAmount, 6);
                                    amount = Math.Round(amountCur / creditExchangeRate, 0);
                                    amountCur = Math.Round(amountCur / exchangeRate, 0);
                                }
                            }
                            else
                            {
                                if (exchangeMethod == true)
                                {
                                    creditExchangeRate = Math.Round((creditAmount + (amountCur - creditAmountCur) * exchangeRate) / amountCur, 6);
                                    amount = Math.Round(amountCur * creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur * exchangeRate, 0);
                                }
                                else
                                {
                                    creditExchangeRate = Math.Round(amountCur / (creditAmount + (amountCur - creditAmountCur) / exchangeRate), 6);
                                    amount = Math.Round(amountCur / creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur / exchangeRate, 0);
                                }
                            }
                            // chênh lệch
                            amountRefunds = amount0 - amount;
                            if (amount0 < amount)
                            {
                                amount = amount0;
                            }
                            // trường hợp đặc biệt
                            if ("111".Contains(debitAcc) && "112".Contains(creditAcc))
                            {
                                amountRefunds = 0;
                                debitExchangeRate = 1;
                                if (exchangeMethod == true)
                                {
                                    amount = Math.Round(amountCur * creditExchangeRate, 0);
                                }
                                else
                                {
                                    amount = Math.Round(amountCur / creditExchangeRate, 0);
                                }
                            }
                        }
                        else // I.2.2
                        {
                            update = "C";
                            creditExchangeRate = exchangeRate;
                            if (exchangeMethod == true)
                            {
                                amount = Math.Round(amountCur * creditExchangeRate, 0);
                            }
                            else
                            {
                                amount = Math.Round(amountCur / creditExchangeRate, 0);
                            }
                        }
                    }
                    //4.Tk nợ là nguồn Tốn theo dõi ngoại tệ, tài khoản có không dõi ngoại tệ
                    if (debitTsNv == "N" && debitAccCur == "C" && creditAccCur != "C")
                    {
                        if (creditAmountCur >= 0)//I.1.1
                        {
                            update = "C";
                            debitExchangeRate = exchangeRate;
                            if (exchangeMethod == true)
                            {
                                amount = Math.Round(amountCur * debitExchangeRate, 0);
                            }
                            else
                            {
                                amount = Math.Round(amountCur / exchangeRate, 0);
                            }
                        }
                        if (debitAmountCur < 0)// I.1.2
                        {
                            update = "C";
                            if (Math.Abs(debitAmountCur) >= amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round(debitAmount / debitAmountCur, 10);
                                    amount = Math.Round(amountCur * debitExchangeRate, 0);
                                }
                                else
                                {
                                    debitExchangeRate = Math.Round(debitAmountCur / debitAmount, 10);
                                    amount = Math.Round(amountCur / debitExchangeRate, 0);
                                }
                            }
                            else
                            {
                                if (exchangeMethod == true)
                                {
                                    debitExchangeRate = Math.Round((Math.Abs(debitAmount) + (amountCur - Math.Abs(debitAmountCur)) * exchangeRate) / amountCur, 10);
                                    amount = Math.Round(amountCur * debitExchangeRate, 0);
                                }
                                else
                                {
                                    debitExchangeRate = Math.Round(amountCur / (Math.Abs(debitAmount) + (amountCur - Math.Abs(debitAmountCur)) / exchangeRate), 10);
                                    amount = Math.Round(amountCur / debitExchangeRate, 0);
                                }
                            }
                        }
                    }
                    // 5. Tk nợ là nguồn vốn không theo dõi ngoại tệ, tài khoản có dõi ngoại tệ
                    if (debitTsNv == "N" && creditAccCur != "C" && debitAccCur == "C")
                    {
                        if (creditAmountCur >= 0)
                        {
                            update = "C";
                            if (creditAmountCur >= amountCur)
                            {
                                if (exchangeMethod == true)
                                {
                                    creditExchangeRate = Math.Round(creditAmount / creditAmountCur, 6);
                                    amount = Math.Round(amountCur * creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur * exchangeRate, 0);
                                }
                                else
                                {
                                    creditExchangeRate = Math.Round(creditAmountCur / creditAmount, 6);
                                    amount = Math.Round(amountCur / creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur / exchangeRate, 0);
                                }
                            }
                            else
                            {
                                if (exchangeMethod == true)
                                {
                                    creditExchangeRate = Math.Round((creditAmount + (amountCur - creditAmountCur) * exchangeRate) / amountCur, 6);
                                    amount = Math.Round(amountCur * creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur * exchangeRate, 0);
                                }
                                else
                                {
                                    creditExchangeRate = Math.Round(amountCur / (creditAmount + (amountCur - creditAmountCur) / exchangeRate), 6);
                                    amount = Math.Round(amountCur / creditExchangeRate, 0);
                                    amount0 = Math.Round(amountCur / exchangeRate, 0);
                                }
                            }
                            //fix chênh lệch
                            amountRefunds = amount0 - amount;
                            if (amount0 < amount)
                            {
                                amount = amount0;
                            }
                            // trường hợp đặc biệt
                            if (debitAcc.Contains("111") && creditAcc.Contains("112"))
                            {
                                amountRefunds = 0;
                                debitExchangeRate = 1;
                                if (exchangeMethod == true)
                                {
                                    amount = Math.Round(amountCur * creditExchangeRate, 0);
                                }
                                else
                                {
                                    amount = Math.Round(amountCur / creditExchangeRate, 0);
                                }
                            }
                        }
                        else
                        {
                            update = "C";
                            creditExchangeRate = exchangeRate;
                            if (exchangeMethod == true)
                            {
                                amount = Math.Round(amountCur * creditExchangeRate);
                            }
                            else
                            {
                                amount = Math.Round(amountCur / creditExchangeRate);
                            }
                        }

                    }
                    // Nếu có thì update vào và cập nhật lại dư đầu kỳ
                    var updateOpeningBalanceExchanRate = from a in resultOpeningBalanceExchanRate
                                                         where a.PfKey == pfKeyCredit || a.PfKey == pfKeyDebit
                                                         select new
                                                         {
                                                             Debit = a.Debit + (a.PfKey == pfKeyDebit ? 1 : -1) * amount,
                                                             DebitCur = a.DebitCur + (a.PfKey == pfKeyDebit ? 1 : -1) * amountCur,
                                                             a.PfKey
                                                         };
                    resultOpeningBalanceExchanRate = (from a in resultOpeningBalanceExchanRate
                                                      join b in updateOpeningBalanceExchanRate on a.PfKey equals b.PfKey into g
                                                      from c in g.DefaultIfEmpty()
                                                      select new
                                                      {
                                                          a.PfKey,
                                                          Debit = c != null ? c.Debit : a.Debit,
                                                          DebitCur = c != null ? c.DebitCur : a.DebitCur
                                                      }).ToList();
                    // có update lại dữ liệu dòng chứng từ
                    if (update == "C")
                    {
                        var updateOpeningBalanceExchanRates = from a in resultOpeningBalanceExchanRate
                                                              where a.PfKey == (amountRefunds > 0 ? pfKeyDebit : pfKeyCredit)
                                                              select new
                                                              {
                                                                  Debit = a.Debit + amountRefunds,
                                                                  a.PfKey
                                                              };
                        resultOpeningBalanceExchanRate = (from a in resultOpeningBalanceExchanRate
                                                          join b in updateOpeningBalanceExchanRates on a.PfKey equals b.PfKey into g
                                                          from c in g.DefaultIfEmpty()
                                                          select new
                                                          {
                                                              a.PfKey,
                                                              Debit = c != null ? c.Debit : a.Debit,
                                                              a.DebitCur
                                                          }).ToList();
                        string noteRefunds = "";
                        string debitAccRefunds = "";
                        string creditAccRefunds = "";
                        string currencyRefunds = "";
                        decimal exchanrateRefunds;
                        string partnerRefunds = "";
                        string contractRefunds = "";
                        string fProductWorkRefunds = "";
                        string workPlaceRefunds = "";
                        string sectionCodeRefunds = "";

                        if (amountRefunds > 0) // lãi
                        {
                            noteRefunds = dgLai;
                            debitAccRefunds = dto.Parameters.HoleAcc;
                            creditAccRefunds = dto.Parameters.InterestAcc;//515 tí sửa
                            currencyRefunds = currencyCode;
                            exchanrateRefunds = exchangeRate;
                            partnerRefunds = debitPartnerCode;
                            contractRefunds = debitContractCode;
                            fProductWorkRefunds = debitFProductWorkCode;
                            workPlaceRefunds = debitWorkPlaceCode;
                            sectionCodeRefunds = debitSectionCode;
                            amountRefunds = Math.Abs(amountRefunds);
                        }
                        else
                        {
                            noteRefunds = dglo;
                            debitAccRefunds = dto.Parameters.HoleAcc;
                            creditAccRefunds = dto.Parameters.InterestAcc;//515 tí sửa
                            currencyRefunds = currencyCode;
                            exchanrateRefunds = exchangeRate;
                            partnerRefunds = creditPartnerCode;
                            contractRefunds = creditContractCode;
                            fProductWorkRefunds = creditFProductWorkCode;
                            workPlaceRefunds = creditWorkPlaceCode;
                            sectionCodeRefunds = creditSectionCode;
                            amountRefunds = Math.Abs(amountRefunds);
                        }
                        CrudExchanRateDto crudExchanRateDto = new CrudExchanRateDto();
                        crudExchanRateDto.Id = id;
                        crudExchanRateDto.VoucherCode = voucherCode;
                        crudExchanRateDto.VoucherDate = voucherDate;
                        crudExchanRateDto.VoucherNumber = voucherNumber;
                        crudExchanRateDto.DebitExchangeRate = debitExchangeRate;
                        crudExchanRateDto.CreditExchangeRate = creditExchangeRate;
                        crudExchanRateDto.Amount = amount;
                        crudExchanRateDto.NoteRefunds = noteRefunds;
                        crudExchanRateDto.DebitAccRefunds = debitAccRefunds;
                        crudExchanRateDto.CreditAccRefunds = creditAccRefunds;
                        crudExchanRateDto.PartnerRefunds = partnerRefunds;
                        crudExchanRateDto.ContractRefunds = contractRefunds;
                        crudExchanRateDto.FProductWorkRefunds = fProductWorkRefunds;
                        crudExchanRateDto.WorkPlaceRefunds = workPlaceRefunds;
                        crudExchanRateDto.AmountRefunds = amountRefunds;
                        crudExchanRateDto.CurrencyCode = currencyCode;
                        crudExchanRateDto.VoucherId = voucherId;
                        crudExchanRateDto.Ord0 = ord0;
                        crudExchanRateDto.ExchanRate = exchangeRate;
                        await UpdateExchanRate(crudExchanRateDto);

                    }
                }

                ledgerExchanRate.Remove(ledgerExchanRates);
            }
            JsonObject keyValues = new JsonObject();
            keyValues.Add("oke", "true");
            return keyValues;

        }

        public async Task UpdateExchanRate(CrudExchanRateDto crudExchanRateDto)
        {
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lstVoucher2 = voucherType.Where(p => p.Code == "000").FirstOrDefault().ListVoucher;
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == crudExchanRateDto.VoucherCode).ToList();
            string voucherKind = "";
            int voucherGroup = 0;
            if (lstVoucherCategory.Count != 0)
            {
                voucherKind = voucherCategory.FirstOrDefault().VoucherKind;
                voucherGroup = voucherCategory.FirstOrDefault().VoucherGroup;
            }
            else
            {
                var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();
                var lstdefaultVoucherCategory = defaultVoucherCategory.Where(p => p.Code == crudExchanRateDto.VoucherCode).ToList();
                voucherKind = lstdefaultVoucherCategory.FirstOrDefault().VoucherKind;
                voucherGroup = lstdefaultVoucherCategory.FirstOrDefault().VoucherGroup;
            }
            int isHt2;
            if (lstVoucher2.Contains(crudExchanRateDto.VoucherCode) == true)
            {
                isHt2 = 1;
            }
            else
            {
                isHt2 = 0;
            }
            var ledgers = await _ledgerService.GetQueryableAsync();
            var lstLedger = ledgers.Where(p => p.Id == crudExchanRateDto.Id).ToList();
            string currencyCode = "";
            decimal amountCur = 0;
            decimal amount = 0;
            string Ord0 = "";
            string voucherId = "";
            int ispbvat = 0;
            int ispsCost = 0;
            string id = "";
            if (lstLedger.Count > 0)
            {
                currencyCode = lstLedger.FirstOrDefault().CurrencyCode;
                amountCur = (decimal)lstLedger.FirstOrDefault().AmountCur;
                Ord0 = lstLedger.FirstOrDefault().Ord0;
                voucherId = lstLedger.FirstOrDefault().VoucherId;
            }
            var curency = await _currencyService.GetQueryableAsync();
            var lstCurrency = curency.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == crudExchanRateDto.CurrencyCode).ToList();
            bool exchngeMethod = false;
            if (lstCurrency.Count > 0)
            {
                exchngeMethod = lstCurrency.FirstOrDefault().ExchangeMethod;
            }
            decimal exchanRate;
            if (exchngeMethod == true)
            {
                exchanRate = amountCur != 0 ? Math.Round((decimal)crudExchanRateDto.Amount / amountCur, 6) : 0;

            }
            else
            {
                exchanRate = amountCur != 0 ? Math.Round(amountCur / (decimal)crudExchanRateDto.Amount, 6) : 0;
            }
            if (voucherKind == "KT")
            {
                if (Ord0.Contains("A") == true)
                {
                    var accVoucherDetail = await _accVoucherDetailService.GetQueryableAsync();

                    var lstAccVoucherDetail = accVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                && p.AccVoucherId == voucherId
                                                && p.Ord0 == Ord0
                                                );
                    foreach (var item in lstAccVoucherDetail)
                    {
                        item.CreditExchangeRate = crudExchanRateDto.CreditExchangeRate;
                        item.DebitExchageRate = crudExchanRateDto.DebitExchangeRate;
                        item.Amount = crudExchanRateDto.Amount;
                        await _accVoucherDetailService.UpdateAsync(item);
                    }

                }
                if (Ord0.Contains("Z"))
                {
                    var acctaxDetail = await _accTaxDetailService.GetQueryableAsync();
                    var lstacctaxDetail = acctaxDetail.Where(p => p.AccVoucherId == voucherId && p.Ord0 == Ord0);
                    foreach (var item in lstacctaxDetail)
                    {
                        item.CreditExchangeRate = crudExchanRateDto.CreditExchangeRate;
                        item.DebitExchangeRate = crudExchanRateDto.DebitExchangeRate;
                        item.Amount = crudExchanRateDto.Amount;
                        await _accTaxDetailService.UpdateAsync(item);
                    }
                }
            }
            else
            {
                var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
                var productVoucherDetails = productVoucherDetail.Where(p => p.ProductVoucherId == voucherId);
                var lstProductVoucherDetailReceipt = await _productVoucherDetailReceiptService.GetQueryableAsync();
                if (Ord0.Contains("A") == true || Ord0.Contains("C") == true)
                {
                    Ord0 = "A" + Ord0.Substring(2, 9);

                    var productVoucherDetailss = productVoucherDetail.Where(p => p.Ord0 == Ord0 && p.ProductVoucherId == voucherId);
                    if (isHt2 == 1)
                    {

                        foreach (var item in productVoucherDetailss)
                        {
                            item.Amount2 = crudExchanRateDto.Amount;
                            item.Price2 = item.Quantity != 0 ? Math.Round((decimal)crudExchanRateDto.Amount / (decimal)item.Quantity, 0) : item.Price2;
                            await _productVoucherDetailService.UpdateAsync(item);
                        }
                    }
                    else
                    {
                        foreach (var item in productVoucherDetailss)
                        {
                            item.Amount = crudExchanRateDto.Amount;
                            item.Price = item.Quantity != 0 ? Math.Round((decimal)crudExchanRateDto.Amount / (decimal)item.Quantity, 0) : item.Price;
                            item.RevenueAmount = (voucherGroup.ToString().Contains("1,3") == true && item.CreditAcc == crudExchanRateDto.DebitAccRefunds) || (voucherGroup.ToString().Contains("2.3")) ? crudExchanRateDto.AmountRefunds : 0;
                            await _productVoucherDetailService.UpdateAsync(item);
                        }
                    }
                }
                if (Ord0.Contains("Z") == true)
                {
                    var acctaxDetail = await _accTaxDetailService.GetQueryableAsync();
                    var lstacctaxDetail = acctaxDetail.Where(p => p.AccVoucherId == voucherId && p.Ord0 == Ord0);
                    foreach (var item in lstacctaxDetail)
                    {
                        item.CreditExchangeRate = crudExchanRateDto.CreditExchangeRate;
                        item.DebitExchangeRate = crudExchanRateDto.DebitExchangeRate;
                        item.Amount = crudExchanRateDto.Amount;
                        await _accTaxDetailService.UpdateAsync(item);
                        ispbvat = 1;
                    }
                }
                if (Ord0.Contains("X") == true)
                {
                    var lstProductVoucheCost = await _productVoucherCostService.GetQueryableAsync();
                    var productVoucheCost = lstProductVoucheCost.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductVoucherId == voucherId);
                    foreach (var item in productVoucheCost)
                    {
                        item.DebitExchange = crudExchanRateDto.DebitExchangeRate;
                        item.CreditExchange = crudExchanRateDto.CreditExchangeRate;
                        item.Amount = crudExchanRateDto.Amount;
                        await _productVoucherCostService.UpdateAsync(item);
                    }
                    ispsCost = 1;
                }
                if (Ord0.Contains("G") == true)// giảm giá
                {

                    foreach (var item in productVoucherDetails)
                    {
                        item.DecreaseAmount = exchngeMethod == true ? Math.Round((decimal)item.DecreaseAmount * exchanRate, 0) : Math.Round((decimal)item.DecreaseAmount / exchanRate, 0);
                    }
                    // xử lý chênh lệch nếu có
                    var amountDetail = productVoucherDetail.Where(p => p.ProductVoucherId == voucherId).Sum(p => p.DecreaseAmount);
                    if (crudExchanRateDto.Amount != amountDetail)
                    {
                        var lstproductVoucherDetails = productVoucherDetail.Where(p => p.ProductVoucherId == voucherId && p.DecreaseAmount != 0).FirstOrDefault();
                        id = lstproductVoucherDetails.Id;
                        foreach (var item in productVoucherDetails)
                        {
                            item.DecreaseAmount = crudExchanRateDto.Amount - amountDetail;
                            await _productVoucherDetailService.UpdateAsync(item);
                        }
                    }
                }
                if (Ord0 == "K000000001")// chiết khấu chi tiết
                {


                    foreach (var item in productVoucherDetails)
                    {
                        var productVoucherDetailReceipt = lstProductVoucherDetailReceipt.Where(p => p.ProductVoucherDetailId == item.Id);
                        foreach (var items in productVoucherDetailReceipt)
                        {
                            items.DiscountAmount = exchngeMethod == true ? Math.Round((decimal)items.DiscountAmountCur * exchanRate, 0) : Math.Round((decimal)items.DiscountAmountCur / exchanRate, 0);
                            await _productVoucherDetailReceiptService.UpdateAsync(items);
                        }
                        // xử lý chênh lệch nếu có
                        amount = (decimal)productVoucherDetailReceipt.Sum(p => p.DiscountAmount);
                        if (crudExchanRateDto.Amount != amount)
                        {
                            var idproduct = productVoucherDetailReceipt.Where(p => p.DiscountAmount != 0).FirstOrDefault().Id;
                            var productVoucherDetailReceipts = productVoucherDetailReceipt.Where(p => p.Id == idproduct);
                            foreach (var itemss in productVoucherDetailReceipts)
                            {
                                itemss.DiscountAmount += (crudExchanRateDto.Amount - amount);
                                await _productVoucherDetailReceiptService.UpdateAsync(itemss);
                            }
                        }
                    }

                }
                if (Ord0 == "K000000002")// chiết khấu thanh toán
                {
                    var productVoucherReceip = await _productVoucherReceiptService.GetQueryableAsync();
                    var lstproductVoucherReceip = productVoucherReceip.Where(p => p.ProductVoucherId == voucherId);
                    foreach (var item in lstproductVoucherReceip)
                    {
                        item.DiscountAmount0 = exchngeMethod == true ? Math.Round((decimal)item.DiscountAmount0 * exchanRate, 0) : Math.Round((decimal)item.DiscountAmount0 / exchanRate, 0);
                        await _productVoucherReceiptService.UpdateAsync(item);
                    }
                    // xử lý chênh lệch nếu có
                    amount = (decimal)lstproductVoucherReceip.Sum(p => p.DiscountAmount0);
                    if (crudExchanRateDto.Amount != amount)
                    {
                        foreach (var item in lstproductVoucherReceip)
                        {
                            item.DiscountAmount0 += (crudExchanRateDto.Amount - amount);
                            await _productVoucherReceiptService.UpdateAsync(item);
                        }
                    }
                }
                if (Ord0.Contains("NK") == true)// nhập khẩu
                {
                    foreach (var item in productVoucherDetails)
                    {
                        var productVoucherDetailRe = lstProductVoucherDetailReceipt.Where(p => p.ProductVoucherDetailId == item.Id);
                        foreach (var items in productVoucherDetailRe)
                        {
                            items.ImportTaxAmount = exchngeMethod == true ? Math.Round((decimal)items.ImportTaxAmountCur * exchanRate, 0) : Math.Round((decimal)items.ImportTaxAmountCur / exchanRate, 0);
                            await _productVoucherDetailReceiptService.UpdateAsync(items);
                        }
                        // xử lý chênh lệch
                        amount = (decimal)productVoucherDetailRe.Sum(p => p.ImportTaxAmount);
                        if (crudExchanRateDto.Amount != amount)
                        {
                            var idProduct = productVoucherDetailRe.FirstOrDefault().Id;
                            var productVoucherDetailReceipt = productVoucherDetailRe.Where(p => p.Id == idProduct);
                            foreach (var items in productVoucherDetailReceipt)
                            {
                                items.ImportTaxAmount += (crudExchanRateDto.Amount - amount);
                                await _productVoucherDetailReceiptService.UpdateAsync(items);
                            }
                        }
                    }
                }
                if (Ord0.Contains("DB") == true)
                {
                    foreach (var item in productVoucherDetails)
                    {
                        var productVoucherDetailRe = lstProductVoucherDetailReceipt.Where(p => p.ProductVoucherDetailId == item.Id);
                        foreach (var items in productVoucherDetailRe)
                        {
                            items.ExciseTaxAmount = exchngeMethod == true ? Math.Round((decimal)items.ExciseTaxAmountCur * exchanRate, 0) : Math.Round((decimal)items.ExciseTaxAmountCur / exchanRate, 0);
                            await _productVoucherDetailReceiptService.UpdateAsync(items);
                        }
                        // xử lý chênh lệch
                        amount = (decimal)productVoucherDetailRe.Sum(p => p.ExciseTaxAmount);
                        if (crudExchanRateDto.Amount != amount)
                        {
                            var idProduct = productVoucherDetailRe.FirstOrDefault().Id;
                            var productVoucherDetailReceipt = productVoucherDetailRe.Where(p => p.Id == idProduct && p.ExciseTaxAmount != 0);
                            foreach (var items in productVoucherDetailReceipt)
                            {
                                items.ExciseTaxAmount += (crudExchanRateDto.Amount - amount);
                                await _productVoucherDetailReceiptService.UpdateAsync(items);
                            }
                        }
                    }
                }
            }

            await UpdateProductVoucher(crudExchanRateDto);
            //Update chứng từ gốc
            await PostLedger(crudExchanRateDto);

        }
        public async Task UpdateProductVoucher(CrudExchanRateDto crudExchanRateDto)
        {
            string vId;
            decimal vExchanRate;
            decimal totalAmountProduct = 0;
            string accountPb;
            decimal amountPb0 = 0;
            decimal amountPb = 0;
            decimal amountPbCur0 = 0;
            decimal totalAmountPb = 0;
            decimal amountPbCur = 0;
            decimal amountTl2Tb0 = 0;
            decimal amountTl2Tb0Cur = 0;
            decimal amountTl3Tb = 0;
            decimal amountTl3PbCur = 0;
            decimal expenseAmount0 = 0;
            decimal expenseAmount1 = 0;
            decimal discountAmount = 0;
            decimal vatAmount = 0;
            decimal importAmount = 0;
            decimal excixeAmount = 0;
            decimal costAmount1 = 0;
            decimal costAmount = 0;
            decimal totalAmountPay = 0;//t_tien_tt
            decimal totalAmount = 0;
            decimal devaluationAmount = 0;
            if (crudExchanRateDto.VoucherKind == "HV")
            {
                var lstProductVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
                var productDetail = lstProductVoucherDetail.Where(p => p.ProductVoucherId == crudExchanRateDto.Id);
                totalAmountProduct = crudExchanRateDto.IsHt2 == 1 ? (decimal)productDetail.Sum(p => p.Amount2) : (decimal)productDetail.Sum(p => p.Amount2);
                var lstProductVoucherDetailReceipts = await _productVoucherDetailReceiptService.GetQueryableAsync();
                if (crudExchanRateDto.IsPbVat == 1)
                {
                    var accTaxDetail = await _accTaxDetailService.GetByProductIdAsync(crudExchanRateDto.VoucherId);
                    amountPbCur0 = (decimal)accTaxDetail.Sum(p => p.AmountCur);
                    amountPb0 = (decimal)accTaxDetail.Sum(p => p.Amount);
                    if (crudExchanRateDto.ExchangeMethod == 1)
                    {
                        vExchanRate = amountPbCur0 != 0 ? Math.Round(amountPb0 / amountPbCur0, 10) : 1;
                    }
                    else
                    {
                        vExchanRate = amountPb0 != 0 ? Math.Round(amountPbCur0 / amountPb0, 10) : 1;
                    }

                    foreach (var item in productDetail)
                    {
                        var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.ProductVoucherDetailId == item.ProductVoucherDetailId);
                        foreach (var itesm in productVoucherDetailReceipts)
                        {
                            itesm.VatAmount = crudExchanRateDto.ExchangeMethod == 1 ? Math.Round((decimal)itesm.VatAmountCur * vExchanRate, 10) : Math.Round((decimal)itesm.VatAmountCur / vExchanRate, 10);
                            await _productVoucherDetailReceiptService.UpdateAsync(itesm);
                        }
                    }
                    // xử lý chênh lệch nếu có
                    foreach (var item in productDetail)
                    {
                        var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.ProductVoucherDetailId == item.ProductVoucherDetailId);
                        amountPb = (decimal)productVoucherDetailReceipts.Sum(p => p.VatAmount);
                    }
                    if (amountPb != amountPb0)
                    {
                        vId = "";
                        foreach (var item in productDetail)
                        {
                            vId = lstProductVoucherDetailReceipts.Where(p => p.ProductVoucherDetailId == item.ProductVoucherDetailId && p.VatAmount != 0).FirstOrDefault().Id;

                        }

                        var productVoucherDetailReceiptService = lstProductVoucherDetailReceipts.Where(p => p.Id == vId);
                        foreach (var item in productVoucherDetailReceiptService)
                        {
                            item.VatAmount = item.VatAmount + amountPb0 - amountPb;
                            await _productVoucherDetailReceiptService.UpdateAsync(item);
                        }
                    }
                }
                if (crudExchanRateDto.IsPbCf == 1)
                {
                    var lstProductVoucherCost = await _productVoucherCostService.GetQueryableAsync();
                    var productVoucherCosts = from a in lstProductVoucherCost
                                              group new
                                              {
                                                  a.DebitAcc,
                                                  a.Id,
                                                  a.CostType,
                                                  a.AmountCur,
                                                  a.Amount
                                              } by new
                                              {
                                                  a.DebitAcc
                                              } into gr
                                              where gr.Max(p => p.Id) == crudExchanRateDto.VoucherId
                                              select new
                                              {
                                                  DebitAcc = gr.Key.DebitAcc,
                                                  amountTl2Tb0Cur = gr.Max(p => p.CostType) == "PL2" ? gr.Sum(p => p.AmountCur) : 0,
                                                  amountTl2Tb0 = gr.Max(p => p.CostType) == "PL2" ? gr.Sum(p => p.Amount) : 0,
                                                  amountTl3PbCur = gr.Max(p => p.CostType) == "PL3" ? gr.Sum(p => p.AmountCur) : 0,
                                                  amountTl3Tb = gr.Max(p => p.CostType) == "PL3" ? gr.Sum(p => p.Amount) : 0
                                              };
                    foreach (var item in productVoucherCosts)
                    {
                        // chi phí 1
                        amountTl2Tb0Cur = (decimal)item.amountTl2Tb0Cur;
                        amountTl2Tb0 = (decimal)item.amountTl2Tb0;
                        amountTl3PbCur = (decimal)item.amountTl3PbCur;
                        amountTl3Tb = (decimal)item.amountTl3Tb;
                        accountPb = item.DebitAcc;
                        if (crudExchanRateDto.ExchangeMethod == 1)
                        {
                            vExchanRate = amountTl2Tb0Cur != 0 ? Math.Round(amountTl2Tb0 / amountTl2Tb0Cur, 10) : 1;

                        }
                        else
                        {
                            vExchanRate = amountTl2Tb0 != 0 ? Math.Round(amountTl2Tb0Cur / amountTl2Tb0, 10) : 1;
                        }

                        var lstproductDetail = productDetail.Where(p => p.DebitAcc == accountPb);
                        foreach (var items in lstproductDetail)
                        {
                            var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId);
                            foreach (var ss in productVoucherDetailReceipts)
                            {
                                ss.ExpenseAmount1 = crudExchanRateDto.ExchangeMethod == 1 ? Math.Round((decimal)ss.ExpenseAmountCur1 * vExchanRate, 10) : Math.Round((decimal)ss.ExpenseAmountCur1 / vExchanRate, 10);
                                await _productVoucherDetailReceiptService.UpdateAsync(ss);

                            }

                        }
                        // xử lý chênh lệch nếu có
                        foreach (var items in lstproductDetail)
                        {
                            var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId);
                            amountPb = (decimal)productVoucherDetailReceipts.Sum(p => p.ExpenseAmount1);
                        }
                        if (amountTl2Tb0 != amountPb)
                        {
                            foreach (var items in lstproductDetail)
                            {
                                vId = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId && p.ExpenseAmount1 != 0).FirstOrDefault().Id;
                                var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.Id == vId);
                                foreach (var itemss in productVoucherDetailReceipts)
                                {
                                    itemss.ExpenseAmount1 += amountTl2Tb0 - amountPb;
                                    await _productVoucherDetailReceiptService.UpdateAsync(itemss);
                                }
                            }
                        }
                        // chi phí0
                        if (crudExchanRateDto.ExchangeMethod == 1)
                        {
                            vExchanRate = amountTl3PbCur != 0 ? Math.Round(amountTl3Tb / amountTl3PbCur, 10) : 1;

                        }
                        else
                        {
                            vExchanRate = amountTl3Tb != 0 ? Math.Round(amountTl3Tb / amountTl3PbCur, 10) : 1;
                        }
                        foreach (var items in lstproductDetail)
                        {
                            var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId);
                            foreach (var ss in productVoucherDetailReceipts)
                            {
                                ss.ExpenseAmount0 = crudExchanRateDto.ExchangeMethod == 1 ? Math.Round((decimal)ss.ExpenseAmountCur0 * vExchanRate, 10) : Math.Round((decimal)ss.ExpenseAmountCur0 / vExchanRate, 10);
                                await _productVoucherDetailReceiptService.UpdateAsync(ss);

                            }

                        }
                        // xử lý chênh lệch nếu có'

                        foreach (var items in lstproductDetail)
                        {
                            var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId);
                            amountPb = (decimal)productVoucherDetailReceipts.Sum(p => p.ExpenseAmount0);
                        }
                        if (amountTl3Tb != amountPb)
                        {
                            foreach (var items in lstproductDetail)
                            {
                                vId = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId && p.ExpenseAmount0 != 0).FirstOrDefault().Id;
                                var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.Id == vId);
                                foreach (var itemss in productVoucherDetailReceipts)
                                {
                                    itemss.ExpenseAmount0 += amountTl3Tb - amountPb;
                                    await _productVoucherDetailReceiptService.UpdateAsync(itemss);
                                }
                            }
                        }
                    }

                    // tổng chi phí

                    var productVoucherCost = lstProductVoucherCost.Where(p => p.ProductVoucherId == crudExchanRateDto.Id);
                    var lstProductVoucherCosts = from a in productVoucherCost
                                                 group new
                                                 {
                                                     a.ProductVoucherId,
                                                     a.Amount,
                                                     a.AmountCur
                                                 } by new
                                                 {
                                                     a.ProductVoucherId
                                                 } into gr
                                                 select new
                                                 {
                                                     amountPbCur0 = gr.Sum(p => p.AmountCur),
                                                     amountPb0 = gr.Sum(p => p.Amount)
                                                 };
                    foreach (var item in lstProductVoucherCosts)
                    {
                        amountPbCur0 = (decimal)item.amountPbCur0;
                        amountPb0 = (decimal)item.amountPb0;
                        if (crudExchanRateDto.ExchangeMethod == 1)
                        {
                            vExchanRate = amountPbCur0 != 0 ? Math.Round(amountPb0 / amountPbCur0, 10) : 1;

                        }
                        else
                        {
                            vExchanRate = amountPb0 != 0 ? Math.Round(amountPbCur0 / amountPb0, 10) : 1;
                        }
                        foreach (var items in productDetail)
                        {
                            var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.ProductVoucherDetailId == items.Id);
                            foreach (var itemSS in productVoucherDetailReceipts)
                            {
                                itemSS.ExpenseAmount = crudExchanRateDto.ExchangeMethod == 1 ? Math.Round((decimal)itemSS.ExpenseAmountCur * vExchanRate, 10) : Math.Round((decimal)itemSS.ExpenseAmountCur / vExchanRate, 10);
                                await _productVoucherDetailReceiptService.UpdateAsync(itemSS);
                            }
                        }
                        // xử lý chênh lệch nếu có
                        foreach (var items in productDetail)
                        {
                            var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.ProductVoucherDetailId == items.Id);
                            foreach (var itemss in productVoucherDetailReceipts)
                            {
                                var productVoucherDetailReceipts1 = lstProductVoucherDetailReceipts.Where(p => p.Id == itemss.ProductVoucherDetailId);
                                amountPb = (decimal)productVoucherDetailReceipts1.Sum(p => p.ExpenseAmount0);
                            }
                            if (amountPb0 != amountPb)
                            {
                                foreach (var itemsss in productVoucherDetailReceipts)
                                {
                                    vId = lstProductVoucherDetailReceipts.Where(p => p.Id == items.ProductVoucherDetailId && p.ExpenseAmount != 0).FirstOrDefault().Id;
                                    var productVoucherDetailReceipts1 = lstProductVoucherDetailReceipts.Where(p => p.Id == vId);
                                    foreach (var itemss in productVoucherDetailReceipts1)
                                    {
                                        itemss.ExpenseAmount += amountPb0 - amountPb;
                                        await _productVoucherDetailReceiptService.UpdateAsync(itemss);
                                    }
                                }
                            }
                        }


                    }

                }
                // refresh đầu phiếu
                foreach (var item in productDetail)
                {
                    var productVoucherDetailReceipts = lstProductVoucherDetailReceipts.Where(p => p.ProductVoucherDetailId == item.Id);
                    excixeAmount = (decimal)productVoucherDetailReceipts.Sum(p => p.ExciseTaxAmount);
                    discountAmount = (decimal)productVoucherDetailReceipts.Sum(p => p.DiscountAmount);
                    vatAmount = (decimal)productVoucherDetailReceipts.Sum(p => p.VatAmount);
                    importAmount = (decimal)productVoucherDetailReceipts.Sum(p => p.ImportTaxAmount);
                    // devaluationAmount = (decimal)productVoucherDetailReceipts.Sum(p=>p.)
                    expenseAmount0 = (decimal)productVoucherDetailReceipts.Sum(p => p.ExpenseAmount0);
                    excixeAmount = (decimal)productVoucherDetailReceipts.Sum(p => p.ExpenseAmount);
                    expenseAmount1 = (decimal)productVoucherDetailReceipts.Sum(p => p.ExpenseAmount1);
                }
                if (crudExchanRateDto.VoucherCode == "PCP")
                {
                    var acctaxdetail = await _accTaxDetailService.GetByProductIdAsync(crudExchanRateDto.VoucherId);
                    totalAmountPay = (decimal)acctaxdetail.Sum(p => p.TotalAmount);
                }
                else
                {
                    totalAmountPay = totalAmountProduct + excixeAmount;// 
                }
                totalAmount = totalAmountPay + vatAmount + importAmount + crudExchanRateDto.VoucherCode == "PCP" ? 0 : excixeAmount;
                var productVoucher = await _productVoucherService.GetByProductIdAsync(crudExchanRateDto.VoucherId);
                foreach (var item in productVoucher)
                {
                    item.TotalProductAmount = totalAmountProduct;
                    item.TotalDiscountAmount = discountAmount;
                    item.TotalExpenseAmount0 = expenseAmount0;
                    item.TotalExpenseAmount = excixeAmount;
                    item.TotalVatAmount = vatAmount;
                    item.TotalImportTaxAmount = importAmount;
                    item.TotalExciseTaxAmount = excixeAmount;
                    item.TotalAmountWithoutVat = totalAmountPay;
                    item.TotalAmount = totalAmount;
                    await _productVoucherService.UpdateAsync(item);
                }
            }
            else
            {
                var accVoucherDetail = await _accVoucherDetailService.GetByAccVoucherIdAsync(crudExchanRateDto.VoucherId);
                totalAmountPay = (decimal)accVoucherDetail.Sum(p => p.Amount);
                if (totalAmountPay == 0)
                {
                    var acctax = await _accTaxDetailService.GetByAccVoucherIdAsync(crudExchanRateDto.VoucherId);
                    totalAmountPay = (decimal)acctax.Sum(p => p.AmountWithoutVat);
                }
                var lstaccVoucher = await _accVoucherService.GetQueryableAsync();
                var accVoucher = lstaccVoucher.Where(p => p.Id == crudExchanRateDto.VoucherId);
                foreach (var item in accVoucher)
                {
                    item.TotalAmountWithoutVat = totalAmountPay;
                    item.TotalAmountVat = vatAmount;
                    item.TotalAmount = totalAmount;
                    await _accVoucherService.UpdateAsync(item);
                }
            }

        }
        public async Task PostLedger(CrudExchanRateDto crudExchanRateDto)
        {

            var lstledger = await _ledgerService.GetQueryableAsync();
            var ledger = lstledger.Where(p => p.VoucherId == crudExchanRateDto.VoucherId && p.Ord0 == crudExchanRateDto.Ord0);
            foreach (var item in ledger)
            {
                item.Amount = crudExchanRateDto.Amount;
                item.Note = crudExchanRateDto.NoteRefunds;
                item.NoteE = crudExchanRateDto.NoteRefunds;
                item.ExchangeRate = crudExchanRateDto.ExchanRate;
                // item.DebitAcc = crudExchanRateDto.DebitAccRefunds;
                //item.CreditAcc = crudExchanRateDto.CreditAccRefunds;
                item.DebitExchangeRate = crudExchanRateDto.ExchanRate;
                item.CreditExchangeRate = crudExchanRateDto.ExchanRate;
                item.Description = crudExchanRateDto.NoteRefunds;
                await _ledgerService.UpdateAsync(item);
            }
        }
    }
}

