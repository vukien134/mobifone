using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Accounting.Caching;
using Accounting.Categories.AccOpeningBalances;
using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Reports.TT133;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.Financials;
using Accounting.Reports.GeneralDiaries;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports.Financials
{
    public class NoteToFinancialStatement133AppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly LedgerService _ledgerService;
        private readonly ReasonService _reasonService;
        private readonly AssetGroupAppService _assetGroupAppService;
        private readonly AssetToolDepreciationService _assetToolDepreciationService;
        private readonly AssetToolAccountService _assetToolAccountService;
        private readonly AssetToolService _assetToolService;
        private readonly AssetToolDetailService _assetToolDetailService;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly DefaultAccBalanceSheetService _defaultAccBalanceSheetService;
        private readonly DefaultCashFollowStatementService _defaultCashFollowStatementService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly TenantCashFollowStatementService _tenantCashFollowStatementService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly FStatement133L01Service _fStatement133L01Service;
        private readonly FStatement133L02Service _fStatement133L02Service;
        private readonly FStatement133L03Service _fStatement133L03Service;
        private readonly FStatement133L04Service _fStatement133L04Service;
        private readonly FStatement133L05Service _fStatement133L05Service;
        private readonly FStatement133L06Service _fStatement133L06Service;
        private readonly FStatement133L07Service _fStatement133L07Service;
        private readonly GeneralDomainService _generalDomainService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public NoteToFinancialStatement133AppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        LedgerService ledgerService,
                        ReasonService reasonService,
                        AssetGroupAppService assetGroupAppService,
                        AssetToolDepreciationService assetToolDepreciationService,
                        AssetToolAccountService assetToolAccountService,
                        AssetToolDetailService assetToolDetailService,
                        AssetToolService assetToolService,
                        AccOpeningBalanceService accOpeningBalanceService,
                        DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                        DefaultCashFollowStatementService defaultCashFollowStatementService,
                        TenantAccBalanceSheetService tenantAccBalanceSheetService,
                        TenantCashFollowStatementService tenantCashFollowStatementService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        FStatement133L01Service fStatement133L01Service,
                        FStatement133L02Service fStatement133L02Service,
                        FStatement133L03Service fStatement133L03Service,
                        FStatement133L04Service fStatement133L04Service,
                        FStatement133L05Service fStatement133L05Service,
                        FStatement133L06Service fStatement133L06Service,
                        FStatement133L07Service fStatement133L07Service,
                        GeneralDomainService generalDomainService,
                        AccountingCacheManager accountingCacheManager
                        )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _ledgerService = ledgerService;
            _reasonService = reasonService;
            _assetGroupAppService = assetGroupAppService;
            _assetToolDepreciationService = assetToolDepreciationService;
            _assetToolAccountService = assetToolAccountService;
            _assetToolDetailService = assetToolDetailService;
            _assetToolService = assetToolService;
            _accOpeningBalanceService = accOpeningBalanceService;
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _defaultCashFollowStatementService = defaultCashFollowStatementService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _tenantCashFollowStatementService = tenantCashFollowStatementService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _fStatement133L01Service = fStatement133L01Service;
            _fStatement133L02Service = fStatement133L02Service;
            _fStatement133L03Service = fStatement133L03Service;
            _fStatement133L04Service = fStatement133L04Service;
            _fStatement133L05Service = fStatement133L05Service;
            _fStatement133L06Service = fStatement133L06Service;
            _fStatement133L07Service = fStatement133L07Service;
            _generalDomainService = generalDomainService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.FinancialStatement133ReportView)]
        public async Task<ReportResponseDto<NoteToFinancialStatement133Dto>> CreateDataAsync(ReportRequestDto<NTFS133ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var year = dto.Parameters.FromDate.Value.Year;
            var yearPre = dto.Parameters.FromDatePre.Value.Year;
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            if (yearCategory.UsingDecision != 133)
            {
                throw new Exception("Thông tư của báo cáo khác với thông tư áp dụng của năm làm việc!");
            }
            int usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 133 : dto.Parameters.UsingDecision ?? 133;
            var dataLedger = await GetDataLedger(dto);
            var dataBalance = await GetDataBalance(dto);
            var dataAssetTool = await GetAssetTool(dto);
            var data = new List<NoteToFinancialStatement133Dto>();
            var para = new NTFS133DataParameterDto { Year = year, UsingDecision = usingDecision};
            data.AddRange(await Data01(para));
            data.AddRange(await Data02(para, dataLedger, dataBalance));
            data.AddRange(await Data03(para, dataLedger, dataBalance));
            data.AddRange(await Data04(para, dataLedger, dataBalance));
            data.AddRange(await Data05(para, dataLedger, dataBalance));
            data.AddRange(await Data06(para, dataLedger, dataBalance));
            data.AddRange(await Data07(para, dataLedger, dataBalance));
            var reportResponse = new ReportResponseDto<NoteToFinancialStatement133Dto>();
            reportResponse.Data = data.Where(p => p.Printable != "K").OrderBy(p => p.Sort).ThenBy(p => p.Ord).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data01(NTFS133DataParameterDto dto)
        {
            var tenantFStatement133L01 = (await _fStatement133L01Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement133L01 = await _generalDomainService.GetListAsync<DefaultFStatement133L01, string, TenantFStatement133L01>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L01, TenantFStatement133L01>(p));
            var fStatement133L01 = (tenantFStatement133L01.Count() == 0 ? defaultFStatement133L01 : tenantFStatement133L01);
            var data = fStatement133L01.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement133Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = (p.Description1 ?? "") + (p.Description2 ?? ""),
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data02(NTFS133DataParameterDto dto, List<NTFS133LedgerDto> dataLedger, List<NTFS133LedgerDto> dataBalance)
        {
            var tenantFStatement133L02 = (await _fStatement133L02Service.GetQueryableAsync())
                                        .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                        .ToList();
            var defaultFStatement133L02 = await _generalDomainService.GetListAsync<DefaultFStatement133L02, string, TenantFStatement133L02>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L02, TenantFStatement133L02>(p));
            var fStatement133L02 = (tenantFStatement133L02.Count() == 0 ? defaultFStatement133L02 : tenantFStatement133L02);
            var data02 = fStatement133L02.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement133L02>();
            var rank = data02.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataByRank = data02.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal openingAmount = 0;
                    decimal closingAmount = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (!string.IsNullOrEmpty(item.Acc) && "balance".Contains(item.Method))
                        {
                            var debitPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre ?? 0);
                            var creditPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre ?? 0);
                            var debitPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre2 ?? 0);
                            var creditPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre2 ?? 0);

                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                openingAmount = debit;
                                closingAmount = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                openingAmount = (debit - credit > 0 ? debit - credit : 0);
                                closingAmount = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (debit - credit);
                                closingAmount = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                openingAmount = credit;
                                closingAmount = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                openingAmount = (credit - debit > 0 ? credit - debit : 0);
                                closingAmount = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (credit - debit);
                                closingAmount = (credit2 - debit2);
                            }
                        }
                        if ("incurred".Contains(item.Method) && (item.DebitAcc != "" || item.CreditAcc != ""))
                        {
                            var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                            var lstCreditAcc = GetSplit(item.CreditAcc, ',');
                            var dataJoin = (from a in dataLedger
                                            join b in lstDebitAcc on 0 equals 0
                                            where (a.DebitAcc ?? "").StartsWith(b.Data)
                                            join c in lstCreditAcc on 0 equals 0
                                            where (a.CreditAcc ?? "").StartsWith(c.Data)
                                            select new
                                            {
                                                Amount = a.Amount,
                                                AmountPre = a.AmountPre,
                                            }).ToList();
                            openingAmount = dataJoin.Sum(p => p.AmountPre ?? 0);
                            closingAmount = dataJoin.Sum(p => p.Amount ?? 0);
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data02 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            OpeningAmount = (a.Math == "+") ? b?.OpeningAmount ?? 0 : -1 * b?.OpeningAmount ?? 0,
                                            ClosingAmount = (a.Math == "+") ? b?.ClosingAmount ?? 0 : -1 * b?.ClosingAmount ?? 0,
                                        }).ToList();
                        openingAmount = dataJoin.Sum(p => p.OpeningAmount);
                        closingAmount = dataJoin.Sum(p => p.ClosingAmount);
                    }
                    item.OpeningAmount = openingAmount;
                    item.ClosingAmount = closingAmount;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement133Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num08 = p.ClosingAmount.GetDefaultNullIfZero(),
                Num09 = p.OpeningAmount.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data03(NTFS133DataParameterDto dto, List<NTFS133LedgerDto> dataLedger, List<NTFS133LedgerDto> dataBalance)
        {
            var tenantFStatement133L03 = (await _fStatement133L03Service.GetQueryableAsync())
                                        .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                        .ToList();
            var defaultFStatement133L03 = await _generalDomainService.GetListAsync<DefaultFStatement133L03, string, TenantFStatement133L03>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L03, TenantFStatement133L03>(p));
            var fStatement133L03 = (tenantFStatement133L03.Count() == 0 ? defaultFStatement133L03 : tenantFStatement133L03);
            var data03 = fStatement133L03.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement133L03>();
            var rank = data03.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataByRank = data03.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal openingAmount = 0;
                    decimal increaseAmount = 0;
                    decimal decreaseAmount = 0;
                    decimal closingAmount = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (!string.IsNullOrEmpty(item.Acc) && "balance".Contains(item.Method))
                        {
                            var debitPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre ?? 0);
                            var creditPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre ?? 0);
                            var debitPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre2 ?? 0);
                            var creditPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre2 ?? 0);

                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                openingAmount = debit;
                                closingAmount = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                openingAmount = (debit - credit > 0 ? debit - credit : 0);
                                closingAmount = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (debit - credit);
                                closingAmount = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                openingAmount = credit;
                                closingAmount = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                openingAmount = (credit - debit > 0 ? credit - debit : 0);
                                closingAmount = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (credit - debit);
                                closingAmount = (credit2 - debit2);
                            }
                        }
                        if ("incurred".Contains(item.Method) && (item.DebitAcc != "" || item.CreditAcc != ""))
                        {
                            var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                            var lstCreditAcc = GetSplit(item.CreditAcc, ',');
                            var dataJoin = (from a in dataLedger
                                            join b in lstDebitAcc on 0 equals 0
                                            where (a.DebitAcc ?? "").StartsWith(b.Data)
                                            join c in lstCreditAcc on 0 equals 0
                                            where (a.CreditAcc ?? "").StartsWith(c.Data)
                                            select new
                                            {
                                                Amount = a.Amount,
                                                AmountPre = a.AmountPre,
                                            }).ToList();
                            openingAmount = dataJoin.Sum(p => p.AmountPre ?? 0);
                            closingAmount = dataJoin.Sum(p => p.Amount ?? 0);
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data03 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            OpeningAmount = (a.Math == "+") ? b?.OpeningAmount ?? 0 : -1 * b?.OpeningAmount ?? 0,
                                            IncreaseAmount = (a.Math == "+") ? b?.IncreaseAmount ?? 0 : -1 * b?.IncreaseAmount ?? 0,
                                            DecreaseAmount = (a.Math == "+") ? b?.DecreaseAmount ?? 0 : -1 * b?.DecreaseAmount ?? 0,
                                            ClosingAmount = (a.Math == "+") ? b?.ClosingAmount ?? 0 : -1 * b?.ClosingAmount ?? 0,
                                        }).ToList();
                        openingAmount = dataJoin.Sum(p => p.OpeningAmount);
                        increaseAmount = dataJoin.Sum(p => p.IncreaseAmount);
                        decreaseAmount = dataJoin.Sum(p => p.DecreaseAmount);
                        closingAmount = dataJoin.Sum(p => p.ClosingAmount);
                    }
                    item.OpeningAmount = openingAmount;
                    item.IncreaseAmount = increaseAmount;
                    item.DecreaseAmount = decreaseAmount;
                    item.ClosingAmount = closingAmount;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement133Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.OpeningAmount.GetDefaultNullIfZero(),
                Num07 = p.IncreaseAmount.GetDefaultNullIfZero(),
                Num08 = p.DecreaseAmount.GetDefaultNullIfZero(),
                Num09 = p.ClosingAmount.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data04(NTFS133DataParameterDto dto, List<NTFS133LedgerDto> dataLedger, List<NTFS133LedgerDto> dataBalance)
        {
            var tenantFStatement133L04 = (await _fStatement133L04Service.GetQueryableAsync())
                                        .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                        .ToList();
            var defaultFStatement133L04 = await _generalDomainService.GetListAsync<DefaultFStatement133L04, string, TenantFStatement133L04>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L04, TenantFStatement133L04>(p));
            var fStatement133L04 = (tenantFStatement133L04.Count() == 0 ? defaultFStatement133L04 : tenantFStatement133L04);
            var data04 = fStatement133L04.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement133L04>();
            var rank = data04.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataByRank = data04.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal openingAmount = 0;
                    decimal increaseAmount = 0;
                    decimal decreaseAmount = 0;
                    decimal closingAmount = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (!string.IsNullOrEmpty(item.Acc) && "balance".Contains(item.Method))
                        {
                            var debitPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre ?? 0);
                            var creditPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre ?? 0);
                            var debitPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre2 ?? 0);
                            var creditPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre2 ?? 0);

                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                openingAmount = debit;
                                closingAmount = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                openingAmount = (debit - credit > 0 ? debit - credit : 0);
                                closingAmount = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (debit - credit);
                                closingAmount = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                openingAmount = credit;
                                closingAmount = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                openingAmount = (credit - debit > 0 ? credit - debit : 0);
                                closingAmount = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (credit - debit);
                                closingAmount = (credit2 - debit2);
                            }
                        }
                        if ("incurred".Contains(item.Method) && (item.DebitAcc != "" || item.CreditAcc != ""))
                        {
                            var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                            var lstCreditAcc = GetSplit(item.CreditAcc, ',');
                            var dataJoin = (from a in dataLedger
                                            join b in lstDebitAcc on 0 equals 0
                                            where (a.DebitAcc ?? "").StartsWith(b.Data)
                                            join c in lstCreditAcc on 0 equals 0
                                            where (a.CreditAcc ?? "").StartsWith(c.Data)
                                            select new
                                            {
                                                Amount = a.Amount,
                                                AmountPre = a.AmountPre,
                                            }).ToList();
                            openingAmount = dataJoin.Sum(p => p.AmountPre ?? 0);
                            closingAmount = dataJoin.Sum(p => p.Amount ?? 0);
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data04 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            OpeningAmount = (a.Math == "+") ? b?.OpeningAmount ?? 0 : -1 * b?.OpeningAmount ?? 0,
                                            IncreaseAmount = (a.Math == "+") ? b?.IncreaseAmount ?? 0 : -1 * b?.IncreaseAmount ?? 0,
                                            DecreaseAmount = (a.Math == "+") ? b?.DecreaseAmount ?? 0 : -1 * b?.DecreaseAmount ?? 0,
                                            ClosingAmount = (a.Math == "+") ? b?.ClosingAmount ?? 0 : -1 * b?.ClosingAmount ?? 0,
                                        }).ToList();
                        openingAmount = dataJoin.Sum(p => p.OpeningAmount);
                        increaseAmount = dataJoin.Sum(p => p.IncreaseAmount);
                        decreaseAmount = dataJoin.Sum(p => p.DecreaseAmount);
                        closingAmount = dataJoin.Sum(p => p.ClosingAmount);
                    }
                    item.OpeningAmount = openingAmount;
                    item.IncreaseAmount = increaseAmount;
                    item.DecreaseAmount = decreaseAmount;
                    item.ClosingAmount = closingAmount;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement133Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.OpeningAmount.GetDefaultNullIfZero(),
                Num07 = p.IncreaseAmount.GetDefaultNullIfZero(),
                Num08 = p.DecreaseAmount.GetDefaultNullIfZero(),
                Num09 = p.ClosingAmount.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data05(NTFS133DataParameterDto dto, List<NTFS133LedgerDto> dataLedger, List<NTFS133LedgerDto> dataBalance)
        {
            var tenantFStatement133L05 = (await _fStatement133L05Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement133L05 = await _generalDomainService.GetListAsync<DefaultFStatement133L05, string, TenantFStatement133L05>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L05, TenantFStatement133L05>(p));
            var fStatement133L05 = (tenantFStatement133L05.Count() == 0 ? defaultFStatement133L05 : tenantFStatement133L05);

            var data05 = fStatement133L05.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement133L05>();
            var lstJobLedger = new List<JsonObject>();
            foreach (var item in dataLedger)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                lstJobLedger.Add((JsonObject)job);
            }
            var rank = data05.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataByRank = data05.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal debit = 0;
                    decimal credit = 0;
                    decimal debitIncurred = 0;
                    decimal creditIncurred = 0;
                    decimal debit2 = 0;
                    decimal credit2 = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (!string.IsNullOrEmpty(item.Acc))
                        {
                            debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit ?? 0);
                            credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit ?? 0);
                            debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if ((item.Method ?? "") == "debitCredit")
                            {
                                debit = debit - credit;
                                debit2 = debit2 - credit2;
                            }
                            if ((item.Method ?? "") == "creditDebit")
                            {
                                debit = credit - debit;
                                debit2 = credit2 - debit2;
                            }
                            if ((item.Method ?? "") == "debit")
                            {
                                debit = debit;
                                debit2 = debit2;
                            }
                            if ((item.Method ?? "") == "credit")
                            {
                                debit = credit;
                                debit2 = credit2;
                            }
                            if ((item.Method ?? "") == "debitIncurred")
                            {
                                lstJobLedger = lstJobLedger.Where(p => ((p["DebitAcc"] ?? "").ToString()).StartsWith(item.Acc)).ToList();
                                debitIncurred = lstJobLedger.Sum(p => decimal.Parse(p["Amount"].ToString() ?? "0"));
                            }
                            if ((item.Method ?? "") == "creditIncurred")
                            {
                                lstJobLedger = lstJobLedger.Where(p => ((p["CreditAcc"] ?? "").ToString()).StartsWith(item.Acc)).ToList();
                                creditIncurred = lstJobLedger.Sum(p => decimal.Parse(p["Amount"].ToString() ?? "0"));
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data05 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            Debit = (a.Math == "+") ? b?.DebitBalance1 ?? 0 : -1 * b?.DebitBalance1 ?? 0,
                                            DebitIncurred = (a.Math == "+") ? b?.Debit ?? 0 : -1 * b?.Debit ?? 0,
                                            CreditIncurred = (a.Math == "+") ? b?.Credit ?? 0 : -1 * b?.Credit ?? 0,
                                            Credit = (a.Math == "+") ? b?.DebitBalance2 ?? 0 : -1 * b?.DebitBalance2 ?? 0,
                                        }).ToList();
                        debit = dataJoin.Sum(p => p.Debit);
                        debitIncurred = dataJoin.Sum(p => p.DebitIncurred);
                        creditIncurred = dataJoin.Sum(p => p.CreditIncurred);
                        debit2 = dataJoin.Sum(p => p.Credit);
                    }
                    item.DebitBalance1 = debit;
                    item.Debit = debitIncurred;
                    item.Credit = creditIncurred;
                    item.DebitBalance2 = debit2;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement133Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.DebitBalance1.GetDefaultNullIfZero(),
                Num07 = p.Credit.GetDefaultNullIfZero(),
                Num08 = p.Debit.GetDefaultNullIfZero(),
                Num09 = p.DebitBalance2.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data06(NTFS133DataParameterDto dto, List<NTFS133LedgerDto> dataLedger, List<NTFS133LedgerDto> dataBalance)
        {
            var tenantFStatement133L06 = (await _fStatement133L06Service.GetQueryableAsync())
                                        .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                        .ToList();
            var defaultFStatement133L06 = await _generalDomainService.GetListAsync<DefaultFStatement133L06, string, TenantFStatement133L06>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L06, TenantFStatement133L06>(p));
            var fStatement133L06 = (tenantFStatement133L06.Count() == 0 ? defaultFStatement133L06 : tenantFStatement133L06);
            var data06 = fStatement133L06.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement133L06>();
            var rank = data06.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataByRank = data06.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal openingAmount = 0;
                    decimal increaseAmount = 0;
                    decimal decreaseAmount = 0;
                    decimal closingAmount = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (!string.IsNullOrEmpty(item.Acc) && "balance".Contains(item.Method))
                        {
                            var debitPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre ?? 0);
                            var creditPre = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre ?? 0);
                            var debitPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.DebitPre2 ?? 0);
                            var creditPre2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.CreditPre2 ?? 0);

                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                openingAmount = debit;
                                closingAmount = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                openingAmount = (debit - credit > 0 ? debit - credit : 0);
                                closingAmount = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (debit - credit);
                                closingAmount = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                openingAmount = credit;
                                closingAmount = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                openingAmount = (credit - debit > 0 ? credit - debit : 0);
                                closingAmount = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                openingAmount = (credit - debit);
                                closingAmount = (credit2 - debit2);
                            }
                        }
                        if ("incurred".Contains(item.Method) && (item.DebitAcc != "" || item.CreditAcc != ""))
                        {
                            var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                            var lstCreditAcc = GetSplit(item.CreditAcc, ',');
                            var dataJoin = (from a in dataLedger
                                            join b in lstDebitAcc on 0 equals 0
                                            where (a.DebitAcc ?? "").StartsWith(b.Data)
                                            join c in lstCreditAcc on 0 equals 0
                                            where (a.CreditAcc ?? "").StartsWith(c.Data)
                                            select new
                                            {
                                                Amount = a.Amount,
                                                AmountPre = a.AmountPre,
                                            }).ToList();
                            openingAmount = dataJoin.Sum(p => p.AmountPre ?? 0);
                            closingAmount = dataJoin.Sum(p => p.Amount ?? 0);
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data06 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            OpeningAmount = (a.Math == "+") ? b?.OpeningAmount ?? 0 : -1 * b?.OpeningAmount ?? 0,
                                            IncreaseAmount = (a.Math == "+") ? b?.IncreaseAmount ?? 0 : -1 * b?.IncreaseAmount ?? 0,
                                            DecreaseAmount = (a.Math == "+") ? b?.DecreaseAmount ?? 0 : -1 * b?.DecreaseAmount ?? 0,
                                            ClosingAmount = (a.Math == "+") ? b?.ClosingAmount ?? 0 : -1 * b?.ClosingAmount ?? 0,
                                        }).ToList();
                        openingAmount = dataJoin.Sum(p => p.OpeningAmount);
                        increaseAmount = dataJoin.Sum(p => p.IncreaseAmount);
                        decreaseAmount = dataJoin.Sum(p => p.DecreaseAmount);
                        closingAmount = dataJoin.Sum(p => p.ClosingAmount);
                    }
                    item.OpeningAmount = openingAmount;
                    item.IncreaseAmount = increaseAmount;
                    item.DecreaseAmount = decreaseAmount;
                    item.ClosingAmount = closingAmount;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement133Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.OpeningAmount.GetDefaultNullIfZero(),
                Num07 = p.IncreaseAmount.GetDefaultNullIfZero(),
                Num08 = p.DecreaseAmount.GetDefaultNullIfZero(),
                Num09 = p.ClosingAmount.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement133Dto>> Data07(NTFS133DataParameterDto dto, List<NTFS133LedgerDto> dataLedger, List<NTFS133LedgerDto> dataBalance)
        {
            var tenantFStatement133L07 = (await _fStatement133L07Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement133L07 = await _generalDomainService.GetListAsync<DefaultFStatement133L07, string, TenantFStatement133L07>(p =>
                                    ObjectMapper.Map<DefaultFStatement133L07, TenantFStatement133L07>(p));
            var fStatement133L07 = (tenantFStatement133L07.Count() == 0 ? defaultFStatement133L07 : tenantFStatement133L07);
            var data07 = fStatement133L07.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement133L07>();
            var rank = data07.Max(p => p.Rank);
            var lstAccCode0 = "4111|4112|4118|407|412|413|441|414,415,418,421,431,461,466";
            decimal vGT = 0;
            while (rank > 0)
            {
                var dataByRank = data07.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal vAmount1 = 0;
                    decimal vAmount2 = 0;
                    decimal vAmount3 = 0;
                    decimal vAmount4 = 0;
                    decimal vAmount5 = 0;
                    decimal vAmount6 = 0;
                    decimal vAmount7 = 0;
                    decimal vAmount8 = 0;
                    decimal total = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        int ord = 1;
                        var dataAccCode0 = GetSplit(lstAccCode0, ',');
                        foreach (var itemAccCode0 in dataAccCode0)
                        {
                            if ((item.DebitOrCredit ?? "").StartsWith("D"))
                            {
                                vGT = 0;
                                var lstAcc = GetSplit(itemAccCode0.Data, ',');
                                var dataJoin = (from a in dataBalance
                                                join b in lstAcc on 0 equals 0
                                                where (a.AccCode ?? "").StartsWith(b.Data)
                                                select new
                                                {
                                                    Amount = (item.DebitOrCredit == "D0") ? a.CreditPre - a.DebitPre : a.Credit - a.Debit,
                                                }).ToList();
                                vGT = dataJoin.Sum(p => p.Amount ?? 0);
                            }
                            else
                            {
                                if (item.DebitOrCredit == "N0")
                                {
                                    var lstAcc = GetSplit(itemAccCode0.Data, ',');
                                    var lstAcc07 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc07 on 0 equals 0
                                                    where (a.CreditAcc ?? "").StartsWith(c.Data)
                                                    select new
                                                    {
                                                        Amount = a.AmountPre,
                                                    }).ToList();
                                    vGT = dataJoin.Sum(p => p.Amount ?? 0);
                                }
                                else if (item.DebitOrCredit == "C0")
                                {
                                    var lstAcc = GetSplit(itemAccCode0.Data, ',');
                                    var lstAcc07 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.CreditAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc07 on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(c.Data)
                                                    select new
                                                    {
                                                        Amount = a.AmountPre,
                                                    }).ToList();
                                    vGT = dataJoin.Sum(p => p.Amount ?? 0);
                                }
                                else if (item.DebitOrCredit == "N1")
                                {
                                    var lstAcc = GetSplit(itemAccCode0.Data, ',');
                                    var lstAcc07 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc07 on 0 equals 0
                                                    where (a.CreditAcc ?? "").StartsWith(c.Data)
                                                    select new
                                                    {
                                                        Amount = a.Amount,
                                                    }).ToList();
                                    vGT = dataJoin.Sum(p => p.Amount ?? 0);
                                }
                                else if (item.DebitOrCredit == "C1")
                                {
                                    var lstAcc = GetSplit(itemAccCode0.Data, ',');
                                    var lstAcc07 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.CreditAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc07 on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(c.Data)
                                                    select new
                                                    {
                                                        Amount = a.Amount,
                                                    }).ToList();
                                    vGT = dataJoin.Sum(p => p.Amount ?? 0);
                                }
                            }
                            if (ord == 1) vAmount1 = vGT;
                            if (ord == 2) vAmount2 = vGT;
                            if (ord == 3) vAmount3 = vGT;
                            if (ord == 4) vAmount4 = vGT;
                            if (ord == 5) vAmount5 = vGT;
                            if (ord == 6) vAmount6 = vGT;
                            if (ord == 7) vAmount7 = vGT;
                            if (ord == 8) vAmount8 = vGT;
                            total += vGT;
                            ord++;
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data07 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            Amount1 = (a.Math == "+") ? b?.Amount1 ?? 0 : -1 * b?.Amount1 ?? 0,
                                            Amount2 = (a.Math == "+") ? b?.Amount2 ?? 0 : -1 * b?.Amount2 ?? 0,
                                            Amount3 = (a.Math == "+") ? b?.Amount3 ?? 0 : -1 * b?.Amount3 ?? 0,
                                            Amount4 = (a.Math == "+") ? b?.Amount4 ?? 0 : -1 * b?.Amount4 ?? 0,
                                            Amount5 = (a.Math == "+") ? b?.Amount5 ?? 0 : -1 * b?.Amount5 ?? 0,
                                            Amount6 = (a.Math == "+") ? b?.Amount6 ?? 0 : -1 * b?.Amount6 ?? 0,
                                            Amount7 = (a.Math == "+") ? b?.Amount7 ?? 0 : -1 * b?.Amount7 ?? 0,
                                            Amount8 = (a.Math == "+") ? b?.Amount8 ?? 0 : -1 * b?.Amount8 ?? 0,
                                            Total = (a.Math == "+") ? b?.Total ?? 0 : -1 * b?.Total ?? 0,
                                        }).ToList();
                        vAmount1 = dataJoin.Sum(p => p.Amount1);
                        vAmount2 = dataJoin.Sum(p => p.Amount2);
                        vAmount3 = dataJoin.Sum(p => p.Amount3);
                        vAmount4 = dataJoin.Sum(p => p.Amount4);
                        vAmount5 = dataJoin.Sum(p => p.Amount5);
                        vAmount6 = dataJoin.Sum(p => p.Amount6);
                        vAmount7 = dataJoin.Sum(p => p.Amount7);
                        vAmount8 = dataJoin.Sum(p => p.Amount8);
                        total = dataJoin.Sum(p => p.Total);
                    }
                    item.Amount1 = vAmount1;
                    item.Amount2 = vAmount2;
                    item.Amount3 = vAmount3;
                    item.Amount4 = vAmount4;
                    item.Amount5 = vAmount5;
                    item.Amount6 = vAmount6;
                    item.Amount7 = vAmount7;
                    item.Total = total;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement133Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num01 = p.Amount1.GetDefaultNullIfZero(),
                Num02 = p.Amount2.GetDefaultNullIfZero(),
                Num03 = p.Amount3.GetDefaultNullIfZero(),
                Num04 = p.Amount4.GetDefaultNullIfZero(),
                Num05 = p.Amount5.GetDefaultNullIfZero(),
                Num06 = p.Amount6.GetDefaultNullIfZero(),
                Num07 = p.Amount7.GetDefaultNullIfZero(),
                Num08 = p.Amount8.GetDefaultNullIfZero(),
                Num09 = p.Total.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        // ------------------------------------------------------
        [Authorize(ReportPermissions.FinancialStatement133ReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<NTFS133ParameterDto> dto)
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

        public async Task<List<AssetBookDto>> GetAssetTool(ReportRequestDto<NTFS133ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lstAssetToolGroup = await _assetGroupAppService.GetRankGroup("");
            // get list AssetToolAccount - chi tiết htts
            var assetToolAccount = await _assetToolAccountService.GetQueryableAsync();
            var lstAssetToolAccount = assetToolAccount.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetTool - đầu phiếu ts, ccdc
            var assetTool = await _assetToolService.GetQueryableAsync();
            var lstAssetTool = assetTool.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolDetail - chi tiết ts, ccdc
            var assetToolDetail = await _assetToolDetailService.GetQueryableAsync();
            var lstAssetToolDetail = assetToolDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list Reason - danh mục lý do
            var reason = await _reasonService.GetQueryableAsync();
            var lstReason = reason.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list AssetToolDepreciation - chi tiết khấu hao ts
            var assetToolDepreciation = await _assetToolDepreciationService.GetQueryableAsync();
            var lstAssetToolDepreciation = assetToolDepreciation.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // get list cttscd group
            var lstAssetToolDetailGroup = (from a in lstAssetToolDetail
                                           join b in lstReason on a.UpDownCode equals b.Code
                                           select new AssetBookDto
                                           {
                                               OrgCode = a.OrgCode,
                                               AssetToolId = a.AssetToolId,
                                               Ord0 = a.Ord0,
                                               VoucherDate = a.VoucherDate,
                                               VoucherNumber = a.VoucherNumber,
                                               UpDownDate = a.UpDownDate,
                                               UpDownCode = a.UpDownCode,
                                               OriginalPrice1 = (a.UpDownDate < dto.Parameters.FromDate) ? a.OriginalPrice * (b.ReasonType == "T" ? 1 : -1) : 0,
                                               ImpoverishmentPrice1 = (a.UpDownDate < dto.Parameters.FromDate) ? a.Impoverishment * (b.ReasonType == "T" ? 1 : -1) : 0,
                                               OriginalPriceIncreased = (a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && a.UpDownDate <= dto.Parameters.ToDate
                                                                                  && b.ReasonType == "T") ? a.OriginalPrice : 0,
                                               OriginalPriceReduced = (a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && a.UpDownDate <= dto.Parameters.ToDate
                                                                                  && b.ReasonType == "G") ? a.OriginalPrice : 0,
                                               ImpoverishmentIncrease = (a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && a.UpDownDate <= dto.Parameters.ToDate
                                                                                  && b.ReasonType == "T") ? a.Impoverishment : 0,
                                               ImpoverishmentReduced = (a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && a.UpDownDate <= dto.Parameters.ToDate
                                                                                  && b.ReasonType == "G") ? a.Impoverishment : 0,

                                           }).ToList();
            // get list ctkhts group
            var lstAssetToolDepreciationGroup = (from a in lstAssetToolDepreciation
                                                 join b in lstAssetToolDetail on new { a.AssetToolId, a.Ord0 } equals new { b.AssetToolId, b.Ord0 }
                                                 group new { a, b } by new
                                                 {
                                                     a.OrgCode,
                                                     a.AssetToolId,
                                                     a.Ord0
                                                 } into gr
                                                 select new AssetBookDto
                                                 {
                                                     OrgCode = gr.Key.OrgCode,
                                                     AssetToolId = gr.Key.AssetToolId,
                                                     Ord0 = gr.Key.Ord0,
                                                     DepreciationAccumulated = gr.Sum(p => (p.a.DepreciationBeginDate < dto.Parameters.FromDate) ? p.a.DepreciationAmount : 0),
                                                     DepreciationAmount = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && p.a.UpDownDate <= dto.Parameters.ToDate) ? p.a.DepreciationAmount : 0),
                                                     DepreciationUpAmount = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && p.a.UpDownDate <= dto.Parameters.ToDate) ? p.a.DepreciationUpAmount : 0),
                                                     DepreciationDownAmount = gr.Sum(p => (p.a.UpDownDate >= dto.Parameters.FromDate
                                                                                  && p.a.UpDownDate <= dto.Parameters.ToDate) ? p.a.DepreciationDownAmount : 0),
                                                 }).ToList();
            var data = (from a in lstAssetTool
                        join b in lstAssetToolDetailGroup on a.Id equals b.AssetToolId
                        join c in lstAssetToolDepreciationGroup on new { b.AssetToolId, b.Ord0 } equals new { c.AssetToolId, c.Ord0 } into bjc
                        from c in bjc.DefaultIfEmpty()
                        join d in lstAssetToolGroup on a.AssetToolGroupId equals d.Id into ajd
                        from d in bjc.DefaultIfEmpty()
                        where (a.ReduceDate == null || a.ReduceDate >= dto.Parameters.FromDate)
                        group new { a, b, c } by new
                        {
                            a.AssetToolGroupId,
                            ReduceCode = a.UpDownCode,
                            a.ReduceDate,
                            UpDownCode = b.UpDownCode,
                            b.UpDownDate,
                            AssetGroupCode = d?.AssetGroupCode ?? ""
                        } into gr
                        select new AssetBookDto
                        {
                            AssetGroupCode = gr.Key.AssetGroupCode,
                            ReduceCode = gr.Key.ReduceCode,
                            UpDownCode = gr.Key.UpDownCode,
                            UpDownDate = gr.Key.UpDownDate,
                            OriginalPrice1 = gr.Sum(p => p.b.OriginalPrice1),
                            ImpoverishmentPrice1 = gr.Sum(p => p.b.ImpoverishmentPrice1 + ((p.c == null || p.c.DepreciationAccumulated == null) ? 0 : p.c.DepreciationAccumulated)),
                            RemainingPrice1 = gr.Sum(p => p.b.OriginalPrice1 - p.b.ImpoverishmentPrice1 - ((p.c == null || p.c.DepreciationAccumulated == null) ? 0 : p.b.DepreciationAccumulated)),
                            Depreciation = gr.Sum(p => (p.c == null || p.c.DepreciationAmount == null) ? 0 : p.c.DepreciationAmount),
                            OriginalPriceIncreased = gr.Sum(p => (p.b.OriginalPriceIncreased ?? 0)),
                            ImpoverishmentIncrease = gr.Sum(p => (p.b.ImpoverishmentIncrease ?? 0)),
                            DepreciationUpAmount = gr.Sum(p => (p.c == null || p.c.DepreciationUpAmount == null) ? 0 : p.c.DepreciationUpAmount),
                            OriginalPriceReduced = gr.Sum(p => (p.b.OriginalPriceReduced ?? 0)),
                            ImpoverishmentReduced = gr.Sum(p => (p.b.ImpoverishmentReduced ?? 0)),
                            DepreciationDownAmount = gr.Sum(p => gr.Sum(p => (p.c == null || p.c.DepreciationDownAmount == null) ? 0 : p.c.DepreciationDownAmount)),
                            OriginalPrice2 = gr.Sum(p => (p.b.OriginalPrice1 ?? 0) + (p.b.OriginalPriceIncreased ?? 0) - (p.b.OriginalPriceReduced ?? 0)),
                            ImpoverishmentPrice2 = gr.Sum(p => (p.b.ImpoverishmentPrice1 ?? 0) + ((p.c == null || p.c.DepreciationAccumulated == null) ? 0 : p.c.DepreciationAccumulated) + (p.b.ImpoverishmentIncrease ?? 0) - (p.b.ImpoverishmentReduced ?? 0) + ((p.c == null || p.c.DepreciationAmount == null) ? 0 : p.c.DepreciationAmount)),
                            RemainingPrice2 = gr.Sum(p => (p.b.OriginalPrice1 ?? 0) + (p.b.OriginalPriceIncreased ?? 0) - (p.b.OriginalPriceReduced ?? 0) - ((p.b.ImpoverishmentPrice1 ?? 0) + ((p.c == null || p.c.DepreciationAccumulated == null) ? 0 : p.c.DepreciationAccumulated) + (p.b.ImpoverishmentIncrease ?? 0) - (p.b.ImpoverishmentReduced ?? 0) + ((p.c == null || p.c.DepreciationAmount == null) ? 0 : p.c.DepreciationAmount))),
                        }).ToList();
            // Phan giam tai san do thanh ly, nhuong ban
            data.AddRange((from a in data
                          where (a.ReduceDate == null || a.ReduceDate < dto.Parameters.FromDate)
                          group new { a } by new
                          {
                              a.AssetGroupCode,
                              a.ReduceCode,
                              a.ReduceDate,
                          } into gr
                          select new AssetBookDto
                          {
                              AssetGroupCode = gr.Key.AssetGroupCode,
                              UpDownCode = gr.Key.ReduceCode,
                              UpDownDate = gr.Key.ReduceDate,
                              OriginalPriceReduced = gr.Sum(p => p.a.OriginalPrice2 ?? 0),
                              DepreciationDownAmount = gr.Sum(p => p.a.ImpoverishmentPrice2 ?? 0),
                          }).ToList());
            return data;
        }

        public async Task<List<NTFS133LedgerDto>> GetDataBalance(ReportRequestDto<NTFS133ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lst = new List<NoteToFinancialStatement133Dto>();
            var year = dto.Parameters.FromDate.Value.Year;
            var yearPre = dto.Parameters.FromDatePre.Value.Year;
            var beginDate = DateTime.Parse(dto.Parameters.FromDate.Value.Year + "/01/01");
            var beginDatePre = DateTime.Parse(dto.Parameters.FromDatePre.Value.Year + "/01/01");
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            int? usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 133 : dto.Parameters.UsingDecision;
            var removeDuplicate = await _tenantSettingService.GetValue("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var ledger = await _ledgerService.GetQueryableAsync();
            var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
            //Lấy dữ liệu phát sinh
            var dataLedger0 = (from a in ledger
                               where a.OrgCode == orgCode && String.Compare(a.Status, "2") < 0
                                  && (a.VoucherDate >= beginDatePre && a.VoucherDate <= dto.Parameters.ToDatePre
                                      || a.VoucherDate >= beginDate && a.VoucherDate <= dto.Parameters.ToDate)
                               select new NTFS133LedgerDto
                               {
                                   OrgCode = orgCode,
                                   Year = a.Year,
                                   VoucherDate = a.VoucherDate,
                                   DebitAcc = a.DebitAcc,
                                   CreditAcc = a.CreditAcc,
                                   DebitPartnerCode = a.DebitPartnerCode,
                                   CreditPartnerCode = a.CreditPartnerCode,
                                   DebitFProductWorkCode = (a.DebitPartnerCode ?? "") != "" ? "" : a.DebitFProductWorkCode,
                                   CreditFProductWorkCode = (a.CreditPartnerCode ?? "") != "" ? "" : a.CreditFProductWorkCode,
                                   DebitAmountCur = (removeDuplicate != "C" && a.CheckDuplicate != "C" && a.CheckDuplicate0 != "C") ? a.DebitAmountCur : 0,
                                   CreditAmountCur = (removeDuplicate != "C" && a.CheckDuplicate != "N" && a.CheckDuplicate0 != "N") ? a.CreditAmountCur : 0,
                                   DebitAmount = (removeDuplicate != "C" && a.CheckDuplicate != "C" && a.CheckDuplicate0 != "C") ? a.Amount : 0,
                                   CreditAmount = (removeDuplicate != "C" && a.CheckDuplicate != "N" && a.CheckDuplicate0 != "N") ? a.Amount : 0,
                                   AmountCur = a.AmountCur,
                                   Amount = a.Amount,
                               }).ToList();
            // Tạo dữ liệu phát sinh
            var dataIncurred = dataLedger0.GroupBy(p => new { OrgCode = p.OrgCode, DebitAcc = p.DebitAcc, CreditAcc = p.CreditAcc })
                                          .Select(p => new NTFS133LedgerDto
                                          {
                                              OrgCode = p.Key.OrgCode,
                                              DebitAcc = p.Key.DebitAcc,
                                              CreditAcc = p.Key.CreditAcc,
                                              AmountCurPre = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDatePre && s.VoucherDate <= dto.Parameters.ToDatePre) ? s.AmountCur : 0),
                                              AmountPre = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDatePre && s.VoucherDate <= dto.Parameters.ToDatePre) ? s.Amount : 0),
                                              AmountCur = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDate && s.VoucherDate <= dto.Parameters.ToDate) ? s.AmountCur : 0),
                                              Amount = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDate && s.VoucherDate <= dto.Parameters.ToDate) ? s.Amount : 0),
                                          }).ToList();
            // ---------------------
            var lstAccOpeningBalance = (from a in accOpeningBalance
                                        where a.OrgCode == _webHelper.GetCurrentOrgUnit() && (a.Year == yearPre || a.Year == year)
                                        select new NTFS133LedgerDto
                                        {
                                            OrgCode = orgCode,
                                            Year = a.Year,
                                            PartnerCode = a.PartnerCode,
                                            FProductWorkCode = (a.PartnerCode ?? "") != "" ? "" : a.FProductWorkCode,
                                            DebitCurPre = (a.Year == yearPre) ? a.DebitCur : 0,
                                            DebitPre = (a.Year == yearPre) ? a.Debit : 0,
                                            CreditCurPre = (a.Year == yearPre) ? a.CreditCur : 0,
                                            CreditPre = (a.Year == yearPre) ? a.Credit : 0,
                                            DebitAccumulateCurPre = a.DebitCumCur,
                                            DebitAccumulatePre = a.DebitCum,
                                            CreditAccumulateCurPre = a.CreditCumCur,
                                            CreditAccumulatePre = a.CreditCum,
                                            DebitCur = (a.Year == year) ? a.DebitCur : 0,
                                            Debit = (a.Year == year) ? a.Debit : 0,
                                            CreditCur = (a.Year == year) ? a.CreditCur : 0,
                                            Credit = (a.Year == year) ? a.Credit : 0,
                                            DebitAccumulateCur = a.DebitCumCur,
                                            DebitAccumulate = a.DebitCum,
                                            CreditAccumulateCur = a.CreditCumCur,
                                            CreditAccumulate = a.CreditCum,
                                        }).ToList();
            // thêm dữ liệu bên nợ của dataLedger0
            lstAccOpeningBalance.AddRange((from a in dataLedger0
                                           group new { a } by new
                                           {
                                               OrgCode = a.OrgCode,
                                               AccCode = a.DebitAcc,
                                               PartnerCode = a.DebitPartnerCode,
                                               FProductWorkCode = a.DebitFProductWorkCode,
                                           } into gr
                                           select new NTFS133LedgerDto
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               AccCode = gr.Key.AccCode,
                                               PartnerCode = gr.Key.PartnerCode,
                                               FProductWorkCode = gr.Key.FProductWorkCode,
                                               DebitCurPre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmountCur : 0),
                                               DebitPre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmount : 0),
                                               CreditCurPre = 0,
                                               CreditPre = 0,
                                               DebitAccumulateCurPre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmountCur : 0),
                                               DebitAccumulatePre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmount : 0),
                                               CreditAccumulateCurPre = 0,
                                               CreditAccumulatePre = 0,
                                               DebitCur = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmountCur : 0),
                                               Debit = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmount : 0),
                                               CreditCur = 0,
                                               Credit = 0,
                                               DebitAccumulateCur = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmountCur : 0),
                                               DebitAccumulate = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.DebitAmount : 0),
                                               CreditAccumulateCur = 0,
                                               CreditAccumulate = 0,
                                           }).ToList());
            // thêm dữ liệu bên có của dataLedger0
            lstAccOpeningBalance.AddRange((from a in dataLedger0
                                           group new { a } by new
                                           {
                                               OrgCode = a.OrgCode,
                                               AccCode = a.CreditAcc,
                                               PartnerCode = a.CreditPartnerCode,
                                               FProductWorkCode = a.CreditFProductWorkCode,
                                           } into gr
                                           select new NTFS133LedgerDto
                                           {
                                               OrgCode = gr.Key.OrgCode,
                                               AccCode = gr.Key.AccCode,
                                               PartnerCode = gr.Key.PartnerCode,
                                               FProductWorkCode = gr.Key.FProductWorkCode,
                                               DebitCurPre = 0,
                                               DebitPre = 0,
                                               CreditCurPre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmountCur : 0),
                                               CreditPre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmount : 0),
                                               DebitAccumulateCurPre = 0,
                                               DebitAccumulatePre = 0,
                                               CreditAccumulateCurPre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmountCur : 0),
                                               CreditAccumulatePre = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmount : 0),
                                               DebitCur = 0,
                                               Debit = 0,
                                               CreditCur = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmountCur : 0),
                                               Credit = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmount : 0),
                                               DebitAccumulateCur = 0,
                                               DebitAccumulate = 0,
                                               CreditAccumulateCur = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmountCur : 0),
                                               CreditAccumulate = gr.Sum(p => (p.a.VoucherDate >= dto.Parameters.FromDatePre && p.a.VoucherDate <= dto.Parameters.ToDatePre) ? p.a.CreditAmount : 0),
                                           }).ToList());
            // Tạo dữ liệu số dư
            var dataBalance = lstAccOpeningBalance.GroupBy(p => new
            {
                OrgCode = p.OrgCode,
                AccCode = p.AccCode,
                PartnerCode = p.PartnerCode,
                FProductWorkCode = p.FProductWorkCode,
            })
                                .Select(p => new NTFS133LedgerDto
                                {
                                    OrgCode = p.Key.OrgCode,
                                    AccCode = p.Key.AccCode,
                                    PartnerCode = p.Key.PartnerCode,
                                    FProductWorkCode = p.Key.FProductWorkCode,
                                    DebitCurPre = p.Sum(p => p.DebitCurPre ?? 0),
                                    DebitPre = p.Sum(p => p.DebitPre ?? 0),
                                    CreditCurPre = p.Sum(p => p.CreditCurPre ?? 0),
                                    CreditPre = p.Sum(p => p.CreditPre ?? 0),
                                    DebitAccumulateCurPre = p.Sum(p => p.DebitAccumulateCurPre ?? 0),
                                    DebitAccumulatePre = p.Sum(p => p.DebitAccumulatePre ?? 0),
                                    CreditAccumulateCurPre = p.Sum(p => p.CreditAccumulateCurPre ?? 0),
                                    CreditAccumulatePre = p.Sum(p => p.CreditAccumulatePre ?? 0),
                                    DebitCurPre2 = 0,
                                    DebitPre2 = 0,
                                    CreditCurPre2 = 0,
                                    CreditPre2 = 0,
                                    DebitCur = p.Sum(p => p.DebitCur ?? 0),
                                    Debit = p.Sum(p => p.Debit ?? 0),
                                    CreditCur = p.Sum(p => p.CreditCur ?? 0),
                                    Credit = p.Sum(p => p.Credit ?? 0),
                                    DebitAccumulateCur = p.Sum(p => p.DebitAccumulateCur ?? 0),
                                    DebitAccumulate = p.Sum(p => p.DebitAccumulate ?? 0),
                                    CreditAccumulateCur = p.Sum(p => p.CreditAccumulateCur ?? 0),
                                    CreditAccumulate = p.Sum(p => p.CreditAccumulate ?? 0),
                                    DebitCur2 = 0,
                                    Debit2 = 0,
                                    CreditCur2 = 0,
                                    Credit2 = 0,
                                }).ToList();
            foreach (var item in dataBalance)
            {
                var debitCurPre = item.DebitCurPre ?? 0;
                var debitPre = item.DebitPre ?? 0;
                var creditCurPre = item.CreditCurPre ?? 0;
                var creditPre = item.CreditPre ?? 0;
                var debitAccumulateCurPre = item.DebitAccumulateCurPre ?? 0;
                var debitAccumulatePre = item.DebitAccumulatePre ?? 0;
                var creditAccumulateCurPre = item.CreditAccumulateCurPre ?? 0;
                var creditAccumulatePre = item.CreditAccumulatePre ?? 0;
                var debitCur = item.DebitCur ?? 0;
                var debit = item.Debit ?? 0;
                var creditCur = item.CreditCur ?? 0;
                var credit = item.Credit ?? 0;
                var debitAccumulateCur = item.DebitAccumulateCur ?? 0;
                var debitAccumulate = item.DebitAccumulate ?? 0;
                var creditAccumulateCur = item.CreditAccumulateCur ?? 0;
                var creditAccumulate = item.CreditAccumulate ?? 0;

                item.DebitCurPre = (debitCurPre > creditCurPre ? debitCurPre - creditCurPre : 0);
                item.DebitPre = (debitPre > creditPre ? debitPre - creditPre : 0);
                item.DebitCurPre2 = (debitCurPre - creditCurPre + debitAccumulateCurPre - creditAccumulateCurPre > 0 ? debitCurPre - creditCurPre + debitAccumulateCurPre - creditAccumulateCurPre : 0);
                item.DebitPre2 = (debitPre - creditPre + debitAccumulatePre - creditAccumulatePre > 0 ? debitPre - creditPre + debitAccumulatePre - creditAccumulatePre : 0);
                item.CreditCurPre = (creditCurPre > debitCurPre ? creditCurPre - debitCurPre : 0);
                item.CreditPre = (creditPre > debitPre ? creditPre - debitPre : 0);
                item.CreditCurPre2 = (creditCurPre - debitCurPre + creditAccumulateCurPre - debitAccumulateCurPre > 0 ? creditCurPre - debitCurPre + creditAccumulateCurPre - debitAccumulateCurPre : 0);
                item.CreditPre2 = (creditPre > debitPre + creditAccumulatePre - debitAccumulatePre ? creditPre - debitPre + creditAccumulatePre - debitAccumulatePre : 0);
                item.DebitCur = (debitCur > creditCur ? debitCur - creditCur : 0);
                item.Debit = (debit > credit ? debit - credit : 0);
                item.DebitCur2 = (debitCur - creditCur + debitAccumulateCur - creditAccumulateCur > 0 ? debitCur - creditCur + debitAccumulateCur - creditAccumulateCur : 0);
                item.Debit2 = (debit - credit + debitAccumulate - creditAccumulate > 0 ? debit - credit + debitAccumulate - creditAccumulate : 0);
                item.CreditCur = (creditCur > debitCur ? creditCur - debitCur : 0);
                item.Credit = (credit > debit ? credit - debit : 0);
                item.CreditCur2 = (creditCur - debitCur + creditAccumulateCur - debitAccumulateCur > 0 ? creditCur - debitCur + creditAccumulateCur - debitAccumulateCur : 0);
                item.Credit2 = (credit > debit + creditAccumulate - debitAccumulate ? credit - debit + creditAccumulate - debitAccumulate : 0);
            }
            dataBalance = dataBalance.GroupBy(p => new
            {
                OrgCode = p.OrgCode,
                AccCode = p.AccCode,
            }).Select(p => new NTFS133LedgerDto
            {
                OrgCode = p.Key.OrgCode,
                AccCode = p.Key.AccCode,
                DebitCurPre = p.Sum(p => p.DebitCurPre ?? 0),
                DebitPre = p.Sum(p => p.DebitPre ?? 0),
                CreditCurPre = p.Sum(p => p.CreditCurPre ?? 0),
                CreditPre = p.Sum(p => p.CreditPre ?? 0),
                DebitAccumulateCurPre = p.Sum(p => p.DebitAccumulateCurPre ?? 0),
                DebitAccumulatePre = p.Sum(p => p.DebitAccumulatePre ?? 0),
                CreditAccumulateCurPre = p.Sum(p => p.CreditAccumulateCurPre ?? 0),
                CreditAccumulatePre = p.Sum(p => p.CreditAccumulatePre ?? 0),
                DebitCurPre2 = p.Sum(p => p.DebitCurPre2 ?? 0),
                DebitPre2 = p.Sum(p => p.DebitPre2 ?? 0),
                CreditCurPre2 = p.Sum(p => p.CreditCurPre2 ?? 0),
                CreditPre2 = p.Sum(p => p.CreditPre2 ?? 0),
                DebitCur = p.Sum(p => p.DebitCur ?? 0),
                Debit = p.Sum(p => p.Debit ?? 0),
                CreditCur = p.Sum(p => p.CreditCur ?? 0),
                Credit = p.Sum(p => p.Credit ?? 0),
                DebitAccumulateCur = p.Sum(p => p.DebitAccumulateCur ?? 0),
                DebitAccumulate = p.Sum(p => p.DebitAccumulate ?? 0),
                CreditAccumulateCur = p.Sum(p => p.CreditAccumulateCur ?? 0),
                CreditAccumulate = p.Sum(p => p.CreditAccumulate ?? 0),
                DebitCur2 = p.Sum(p => p.DebitCur2 ?? 0),
                Debit2 = p.Sum(p => p.Debit2 ?? 0),
                CreditCur2 = p.Sum(p => p.CreditCur2 ?? 0),
                Credit2 = p.Sum(p => p.Credit2 ?? 0),
            }).ToList();
            return dataBalance;
        }

        public async Task<List<NTFS133LedgerDto>> GetDataLedger(ReportRequestDto<NTFS133ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lst = new List<NoteToFinancialStatement133Dto>();
            var year = dto.Parameters.FromDate.Value.Year;
            var yearPre = dto.Parameters.FromDatePre.Value.Year;
            var beginDate = DateTime.Parse(dto.Parameters.FromDate.Value.Year + "/01/01");
            var beginDatePre = DateTime.Parse(dto.Parameters.FromDatePre.Value.Year + "/01/01");
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            int? usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 133 : dto.Parameters.UsingDecision;
            var removeDuplicate = await _tenantSettingService.GetValue("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var ledger = await _ledgerService.GetQueryableAsync();
            var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
            //Lấy dữ liệu phát sinh
            var dataLedger0 = (from a in ledger
                               where a.OrgCode == orgCode && String.Compare(a.Status, "2") < 0
                                  && (a.VoucherDate >= beginDatePre && a.VoucherDate <= dto.Parameters.ToDatePre
                                      || a.VoucherDate >= beginDate && a.VoucherDate <= dto.Parameters.ToDate)
                               select new NTFS133LedgerDto
                               {
                                   OrgCode = orgCode,
                                   Year = a.Year,
                                   VoucherDate = a.VoucherDate,
                                   DebitAcc = a.DebitAcc,
                                   CreditAcc = a.CreditAcc,
                                   DebitPartnerCode = a.DebitPartnerCode,
                                   CreditPartnerCode = a.CreditPartnerCode,
                                   DebitFProductWorkCode = (a.DebitPartnerCode ?? "") != "" ? "" : a.DebitFProductWorkCode,
                                   CreditFProductWorkCode = (a.CreditPartnerCode ?? "") != "" ? "" : a.CreditFProductWorkCode,
                                   DebitAmountCur = (removeDuplicate != "C" && a.CheckDuplicate != "C" && a.CheckDuplicate0 != "C") ? a.DebitAmountCur : 0,
                                   CreditAmountCur = (removeDuplicate != "C" && a.CheckDuplicate != "N" && a.CheckDuplicate0 != "N") ? a.CreditAmountCur : 0,
                                   DebitAmount = (removeDuplicate != "C" && a.CheckDuplicate != "C" && a.CheckDuplicate0 != "C") ? a.Amount : 0,
                                   CreditAmount = (removeDuplicate != "C" && a.CheckDuplicate != "N" && a.CheckDuplicate0 != "N") ? a.Amount : 0,
                                   AmountCur = a.AmountCur,
                                   Amount = a.Amount,
                               }).ToList();
            // Tạo dữ liệu phát sinh
            var dataIncurred = dataLedger0.GroupBy(p => new { OrgCode = p.OrgCode, DebitAcc = p.DebitAcc, CreditAcc = p.CreditAcc })
                                          .Select(p => new NTFS133LedgerDto
                                          {
                                              OrgCode = p.Key.OrgCode,
                                              DebitAcc = p.Key.DebitAcc,
                                              CreditAcc = p.Key.CreditAcc,
                                              AmountCurPre = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDatePre && s.VoucherDate <= dto.Parameters.ToDatePre) ? s.AmountCur : 0),
                                              AmountPre = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDatePre && s.VoucherDate <= dto.Parameters.ToDatePre) ? s.Amount : 0),
                                              AmountCur = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDate && s.VoucherDate <= dto.Parameters.ToDate) ? s.AmountCur : 0),
                                              Amount = p.Sum(s => (s.VoucherDate >= dto.Parameters.FromDate && s.VoucherDate <= dto.Parameters.ToDate) ? s.Amount : 0),
                                          }).ToList();
            return dataIncurred;
        }

        #endregion
        #region Private
        private async Task<List<JsonObject>> AddCondition(List<JsonObject> jsonObject, string condition) // Add điều kiện
        {
            var jsonCondition = (JsonObject)JsonObject.Parse(condition);
            var lstJsonCondition = (JsonArray)jsonCondition["data"];
            foreach (var item in lstJsonCondition)
            {
                switch (item["Type"].ToString())
                {
                    case "Contains":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString().Contains(p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "!Contains":
                        jsonObject = jsonObject.Where(p => !item["Value"].ToString().Contains(p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "!=":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString() != (p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "==":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString() == (p[item["FieldName"].ToString()].ToString())).ToList();
                        break;
                    case "1=1":
                        jsonObject = jsonObject.ToList();
                        break;
                    case "1=0":
                        jsonObject = jsonObject.Where(p => "1" == "0").ToList();
                        break;
                    case "StartsWith":
                        jsonObject = jsonObject.Where(p => (p[item["FieldName"].ToString()].ToString().StartsWith(item["Value"].ToString()))).ToList();
                        break;
                    case ">":
                        jsonObject = jsonObject.Where(p => decimal.Parse((p[item["FieldName"].ToString()].ToString())) > decimal.Parse(item["Value"].ToString())).ToList();
                        break;
                    case "<":
                        jsonObject = jsonObject.Where(p => decimal.Parse((p[item["FieldName"].ToString()].ToString())) < decimal.Parse(item["Value"].ToString())).ToList();
                        break;
                    default:
                        break;
                }
            }
            return jsonObject;
        }

        private List<FormularDto> GetFormular(string formular) // lấy list công thức
        {
            var lst = new List<FormularDto>();
            formular = formular.Replace(" ", "");
            formular = formular.Replace("+", ",+,");
            formular = formular.Replace("-", ",-,");
            formular = "+," + formular;
            var lstData = formular.Split(',').ToList();
            for (var i = 0; i < lstData.Count; i+=2)
            {
                lst.Add(new FormularDto
                {
                    Code = lstData[i + 1],
                    AccCode = lstData[i+1],
                    Math = lstData[i],
                });
            }
            return lst;
        }

        private List<LstAccDto> GetSplit(string str, char spt) // lấy list
        {
            var lst = new List<LstAccDto>();
            var lstAcc = str.Split(spt).ToList();
            for (var i = 0; i < lstAcc.Count; i++)
            {
                lst.Add(new LstAccDto
                {
                    Id = i+1,
                    Data = lstAcc[i],
                });
            }
            return lst;
        }
        private async Task<List<CashFlowDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.GroupBy(g => new
            {
                g.VoucherDate,
                g.DebitAcc,
                g.CreditAcc,
                g.SectionCode
            }).Select(p => new CashFlowDto()
            {
                VoucherDate = p.Key.VoucherDate,
                DebitAcc = p.Key.DebitAcc,
                CreditAcc = p.Key.CreditAcc,
                SectionCode0 = p.Key.SectionCode,
                Amount0 = p.Sum(p => p.Amount0),
                AmountCur0 = p.Sum(p => p.AmountCur0),
            }).ToList();
            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private Dictionary<string, object> GetLedgerParameter(CashFlowBResultParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, ((dto.LstOrgCode ?? "") == "") ? _webHelper.GetCurrentOrgUnit() : dto.LstOrgCode);
            dic.Add(LedgerParameterConst.DebitOrCredit, "N");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, "");
            return dic;
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

