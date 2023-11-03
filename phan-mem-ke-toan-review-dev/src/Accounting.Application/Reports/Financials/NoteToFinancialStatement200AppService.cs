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
using Accounting.DomainServices.Reports.TT200;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.Financials;
using Accounting.Reports.GeneralDiaries;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.Reports.Statements.T200.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports.Financials
{
    public class NoteToFinancialStatement200AppService : AccountingAppService
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
        private readonly FStatement200L01Service _fStatement200L01Service;
        private readonly FStatement200L02Service _fStatement200L02Service;
        private readonly FStatement200L03Service _fStatement200L03Service;
        private readonly FStatement200L04Service _fStatement200L04Service;
        private readonly FStatement200L05Service _fStatement200L05Service;
        private readonly FStatement200L06Service _fStatement200L06Service;
        private readonly FStatement200L07Service _fStatement200L07Service;
        private readonly FStatement200L08Service _fStatement200L08Service;
        private readonly FStatement200L09Service _fStatement200L09Service;
        private readonly FStatement200L10Service _fStatement200L10Service;
        private readonly FStatement200L11Service _fStatement200L11Service;
        private readonly FStatement200L12Service _fStatement200L12Service;
        private readonly FStatement200L13Service _fStatement200L13Service;
        private readonly FStatement200L14Service _fStatement200L14Service;
        private readonly FStatement200L15Service _fStatement200L15Service;
        private readonly FStatement200L16Service _fStatement200L16Service;
        private readonly FStatement200L17Service _fStatement200L17Service;
        private readonly FStatement200L18Service _fStatement200L18Service;
        private readonly FStatement200L19Service _fStatement200L19Service;
        private readonly GeneralDomainService _generalDomainService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        public NoteToFinancialStatement200AppService(ReportDataService reportDataService,
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
                        FStatement200L01Service fStatement200L01Service,
                        FStatement200L02Service fStatement200L02Service,
                        FStatement200L03Service fStatement200L03Service,
                        FStatement200L04Service fStatement200L04Service,
                        FStatement200L05Service fStatement200L05Service,
                        FStatement200L06Service fStatement200L06Service,
                        FStatement200L07Service fStatement200L07Service,
                        FStatement200L08Service fStatement200L08Service,
                        FStatement200L09Service fStatement200L09Service,
                        FStatement200L10Service fStatement200L10Service,
                        FStatement200L11Service fStatement200L11Service,
                        FStatement200L12Service fStatement200L12Service,
                        FStatement200L13Service fStatement200L13Service,
                        FStatement200L14Service fStatement200L14Service,
                        FStatement200L15Service fStatement200L15Service,
                        FStatement200L16Service fStatement200L16Service,
                        FStatement200L17Service fStatement200L17Service,
                        FStatement200L18Service fStatement200L18Service,
                        FStatement200L19Service fStatement200L19Service,
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
            _fStatement200L01Service = fStatement200L01Service;
            _fStatement200L02Service = fStatement200L02Service;
            _fStatement200L03Service = fStatement200L03Service;
            _fStatement200L04Service = fStatement200L04Service;
            _fStatement200L05Service = fStatement200L05Service;
            _fStatement200L06Service = fStatement200L06Service;
            _fStatement200L07Service = fStatement200L07Service;
            _fStatement200L08Service = fStatement200L08Service;
            _fStatement200L09Service = fStatement200L09Service;
            _fStatement200L10Service = fStatement200L10Service;
            _fStatement200L11Service = fStatement200L11Service;
            _fStatement200L12Service = fStatement200L12Service;
            _fStatement200L13Service = fStatement200L13Service;
            _fStatement200L14Service = fStatement200L14Service;
            _fStatement200L15Service = fStatement200L15Service;
            _fStatement200L16Service = fStatement200L16Service;
            _fStatement200L17Service = fStatement200L17Service;
            _fStatement200L18Service = fStatement200L18Service;
            _fStatement200L19Service = fStatement200L19Service;
            _generalDomainService = generalDomainService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.FinancialStatementReportView)]
        public async Task<ReportResponseDto<NoteToFinancialStatement200Dto>> CreateDataAsync(ReportRequestDto<NTFS200ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var year = dto.Parameters.FromDate.Value.Year;
            var yearPre = dto.Parameters.FromDatePre.Value.Year;
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            if (yearCategory.UsingDecision != 200)
            {
                throw new Exception("Thông tư của báo cáo khác với thông tư áp dụng của năm làm việc!");
            }
            int usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 200 : dto.Parameters.UsingDecision ?? 200;
            var dataLedger = await GetDataLedger(dto);
            var dataBalance = await GetDataBalance(dto);
            var dataAssetTool = await GetAssetTool(dto);
            var data = new List<NoteToFinancialStatement200Dto>();
            var para = new NTFS200DataParameterDto { Year = year, UsingDecision = usingDecision};
            data.AddRange(await Data01(para));
            data.AddRange(await Data02(para, dataLedger, dataBalance));
            data.AddRange(await Data03(para, dataLedger, dataBalance));
            data.AddRange(await Data04(para, dataLedger, dataBalance));
            data.AddRange(await Data05(para));
            data.AddRange(await Data06(para));
            data.AddRange(await Data07(para, dataLedger, dataBalance));
            data.AddRange(await Data08(para));
            data.AddRange(await Data09(para, dataAssetTool));
            data.AddRange(await Data10(para, dataAssetTool));
            data.AddRange(await Data11(para, dataAssetTool));
            data.AddRange(await Data12(para));
            data.AddRange(await Data13(para, dataLedger, dataBalance));
            data.AddRange(await Data14(para));
            data.AddRange(await Data15(para));
            data.AddRange(await Data16(para, dataBalance));
            data.AddRange(await Data17(para, dataLedger, dataBalance));
            data.AddRange(await Data18(para));
            data.AddRange(await Data19(para, dataLedger, dataBalance));
            var reportResponse = new ReportResponseDto<NoteToFinancialStatement200Dto>();
            reportResponse.Data = data.Where(p => p.Printable != "K").OrderBy(p => p.Sort).ThenBy(p => p.Ord).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data01(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L01 = (await _fStatement200L01Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L01 = await _generalDomainService.GetListAsync<DefaultFStatement200L01, string, TenantFStatement200L01>(p => 
                                    ObjectMapper.Map<DefaultFStatement200L01, TenantFStatement200L01>(p));
            var fStatement200L01 = (tenantFStatement200L01.Count() == 0 ? defaultFStatement200L01 : tenantFStatement200L01);
            var data = fStatement200L01.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = (p.Description ?? "") + (p.DescriptionE ?? ""),
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data02(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L02 = (await _fStatement200L02Service.GetQueryableAsync())
                                        .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                        .ToList();
            var defaultFStatement200L02 = await _generalDomainService.GetListAsync<DefaultFStatement200L02, string, TenantFStatement200L02>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L02, TenantFStatement200L02>(p));
            var fStatement200L02 = (tenantFStatement200L02.Count() == 0 ? defaultFStatement200L02 : tenantFStatement200L02);
            var data02 = fStatement200L02.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L02>();
            var rank = data02.Max(p => p.Rank);
            while (rank >= 0)
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
            var res = data.Select(p => new NoteToFinancialStatement200Dto
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

        public async Task<List<NoteToFinancialStatement200Dto>> Data03(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L03 = (await _fStatement200L03Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L03 = await _generalDomainService.GetListAsync<DefaultFStatement200L03, string, TenantFStatement200L03>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L03, TenantFStatement200L03>(p)); 
            var fStatement200L03 = (tenantFStatement200L03.Count() == 0 ? defaultFStatement200L03 : tenantFStatement200L03);
            var data03 = fStatement200L03.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L03>();
            var rank = data03.Max(p => p.Rank);
            while (rank >= 0)
            {
                var dataByRank = data03.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal originalPrice1 = 0;
                    decimal recordingPrice1 = 0;
                    decimal preventivePrice1 = 0;
                    decimal originalPrice2 = 0;
                    decimal recordingPrice2 = 0;
                    decimal preventivePrice2 = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        // Giá gốc
                        if (!string.IsNullOrEmpty(item.OriginalPriceAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                originalPrice1 = debit;
                                originalPrice2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                originalPrice1 = (debit - credit > 0 ? debit - credit : 0);
                                originalPrice2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                originalPrice1 = (debit - credit);
                                originalPrice2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                originalPrice1 = credit;
                                originalPrice2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                originalPrice1 = (credit - debit > 0 ? credit - debit : 0);
                                originalPrice2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                originalPrice1 = (credit - debit);
                                originalPrice2 = (credit2 - debit2);
                            }
                        }
                        // Ghi sổ
                        if (!string.IsNullOrEmpty(item.RecordingPriceAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.RecordingPriceAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.RecordingPriceAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.RecordingPriceAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.RecordingPriceAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                recordingPrice1 = debit;
                                recordingPrice2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                recordingPrice1 = (debit - credit > 0 ? debit - credit : 0);
                                recordingPrice2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                recordingPrice1 = (debit - credit);
                                recordingPrice2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                recordingPrice1 = credit;
                                recordingPrice2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                recordingPrice1 = (credit - debit > 0 ? credit - debit : 0);
                                recordingPrice2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                recordingPrice1 = (credit - debit);
                                recordingPrice2 = (credit2 - debit2);
                            }
                        }
                        // Dự phòng
                        if (!string.IsNullOrEmpty(item.PreventivePriceAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                preventivePrice1 = debit;
                                preventivePrice2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                preventivePrice1 = (debit - credit > 0 ? debit - credit : 0);
                                preventivePrice2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                preventivePrice1 = (debit - credit);
                                preventivePrice2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                preventivePrice1 = credit;
                                preventivePrice2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                preventivePrice1 = (credit - debit > 0 ? credit - debit : 0);
                                preventivePrice2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                preventivePrice1 = (credit - debit);
                                preventivePrice2 = (credit2 - debit2);
                            }
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
                                            OriginalPrice1 = (a.Math == "+") ? b?.OriginalPrice1 ?? 0 : -1 * b?.OriginalPrice1 ?? 0,
                                            OriginalPrice2 = (a.Math == "+") ? b?.OriginalPrice2 ?? 0 : -1 * b?.OriginalPrice2 ?? 0,
                                            RecordingPrice1 = (a.Math == "+") ? b?.RecordingPrice1 ?? 0 : -1 * b?.RecordingPrice1 ?? 0,
                                            RecordingPrice2 = (a.Math == "+") ? b?.RecordingPrice2 ?? 0 : -1 * b?.RecordingPrice2 ?? 0,
                                            PreventivePrice1 = (a.Math == "+") ? b?.PreventivePrice1 ?? 0 : -1 * b?.PreventivePrice1 ?? 0,
                                            PreventivePrice2 = (a.Math == "+") ? b?.PreventivePrice2 ?? 0 : -1 * b?.PreventivePrice2 ?? 0,
                                        }).ToList();
                        originalPrice1 = dataJoin.Sum(p => p.OriginalPrice1);
                        originalPrice2 = dataJoin.Sum(p => p.OriginalPrice2);
                        recordingPrice1 = dataJoin.Sum(p => p.RecordingPrice1);
                        recordingPrice2 = dataJoin.Sum(p => p.RecordingPrice2);
                        preventivePrice1 = dataJoin.Sum(p => p.PreventivePrice1);
                        preventivePrice2 = dataJoin.Sum(p => p.PreventivePrice2);
                    }
                    item.OriginalPrice1 = originalPrice1;
                    item.OriginalPrice2 = originalPrice2;
                    item.RecordingPrice1 = recordingPrice1;
                    item.RecordingPrice2 = recordingPrice2;
                    item.PreventivePrice1 = preventivePrice1;
                    item.PreventivePrice2 = preventivePrice2;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num04 = p.OriginalPrice2.GetDefaultNullIfZero(),
                Num05 = p.RecordingPrice2.GetDefaultNullIfZero(),
                Num06 = p.PreventivePrice2.GetDefaultNullIfZero(),
                Num07 = p.OriginalPrice1.GetDefaultNullIfZero(),
                Num08 = p.RecordingPrice1.GetDefaultNullIfZero(),
                Num09 = p.PreventivePrice1.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data04(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L04 = (await _fStatement200L04Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L04 = await _generalDomainService.GetListAsync<DefaultFStatement200L04, string, TenantFStatement200L04>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L04, TenantFStatement200L04>(p));
            var fStatement200L04 = (tenantFStatement200L04.Count() == 0 ? defaultFStatement200L04 : tenantFStatement200L04);
            var data04 = fStatement200L04.Where(p => p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L04>();
            var rank = data04.Max(p => p.Rank);
            while (rank >= 0)
            {
                var dataByRank = data04.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal valueAmount1 = 0;
                    decimal preventiveAmount1 = 0;
                    decimal valueAmount2 = 0;
                    decimal preventiveAmount2 = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        // Giá gốc
                        if (!string.IsNullOrEmpty(item.ValueAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                valueAmount1 = debit;
                                valueAmount2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                valueAmount1 = (debit - credit > 0 ? debit - credit : 0);
                                valueAmount2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                valueAmount1 = (debit - credit);
                                valueAmount2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                valueAmount1 = credit;
                                valueAmount2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                valueAmount1 = (credit - debit > 0 ? credit - debit : 0);
                                valueAmount2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                valueAmount1 = (credit - debit);
                                valueAmount2 = (credit2 - debit2);
                            }
                        }
                        // Dự phòng
                        if (!string.IsNullOrEmpty(item.PreventiveAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                preventiveAmount1 = debit;
                                preventiveAmount2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                preventiveAmount1 = (debit - credit > 0 ? debit - credit : 0);
                                preventiveAmount2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                preventiveAmount1 = (debit - credit);
                                preventiveAmount2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                preventiveAmount1 = credit;
                                preventiveAmount2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                preventiveAmount1 = (credit - debit > 0 ? credit - debit : 0);
                                preventiveAmount2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                preventiveAmount1 = (credit - debit);
                                preventiveAmount2 = (credit2 - debit2);
                            }
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
                                            ValueAmount1 = (a.Math == "+") ? b?.ValueAmount1 ?? 0 : -1 * b?.ValueAmount1 ?? 0,
                                            ValueAmount2 = (a.Math == "+") ? b?.ValueAmount2 ?? 0 : -1 * b?.ValueAmount2 ?? 0,
                                            PreventiveAmount1 = (a.Math == "+") ? b?.PreventiveAmount1 ?? 0 : -1 * b?.PreventiveAmount1 ?? 0,
                                            PreventiveAmount2 = (a.Math == "+") ? b?.PreventiveAmount2 ?? 0 : -1 * b?.PreventiveAmount2 ?? 0,
                                        }).ToList();
                        valueAmount1 = dataJoin.Sum(p => p.ValueAmount1);
                        valueAmount2 = dataJoin.Sum(p => p.ValueAmount2);
                        preventiveAmount1 = dataJoin.Sum(p => p.PreventiveAmount1);
                        preventiveAmount2 = dataJoin.Sum(p => p.PreventiveAmount2);
                    }
                    item.ValueAmount1 = valueAmount1;
                    item.ValueAmount2 = valueAmount2;
                    item.PreventiveAmount1 = preventiveAmount1;
                    item.PreventiveAmount2 = preventiveAmount2;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.ValueAmount2.GetDefaultNullIfZero(),
                Num07 = p.PreventiveAmount2.GetDefaultNullIfZero(),
                Num08 = p.ValueAmount1.GetDefaultNullIfZero(),
                Num09 = p.PreventiveAmount1.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data05(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L05 = (await _fStatement200L05Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L05 = await _generalDomainService.GetListAsync<DefaultFStatement200L05, string, TenantFStatement200L05>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L05, TenantFStatement200L05>(p));
            var fStatement200L05 = (tenantFStatement200L05.Count() == 0 ? defaultFStatement200L05 : tenantFStatement200L05);
            var data = fStatement200L05.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data06(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L06 = (await _fStatement200L06Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L06 = await _generalDomainService.GetListAsync<DefaultFStatement200L06, string, TenantFStatement200L06>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L06, TenantFStatement200L06>(p));
            var fStatement200L06 = (tenantFStatement200L06.Count() == 0 ? defaultFStatement200L06 : tenantFStatement200L06);
            var data = fStatement200L06.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data07(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L07 = (await _fStatement200L07Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L07 = await _generalDomainService.GetListAsync<DefaultFStatement200L07, string, TenantFStatement200L07>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L07, TenantFStatement200L07>(p));
            var fStatement200L07 = (tenantFStatement200L07.Count() == 0 ? defaultFStatement200L07 : tenantFStatement200L07);
            var data07 = fStatement200L07.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L07>();
            var rank = data07.Max(p => p.Rank);
            while (rank >= 0)
            {
                var dataByRank = data07.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal originalPrice1 = 0;
                    decimal preventivePrice1 = 0;
                    decimal originalPrice2 = 0;
                    decimal preventivePrice2 = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        // Giá gốc
                        if (!string.IsNullOrEmpty(item.OriginalPriceAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.OriginalPriceAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                originalPrice1 = debit;
                                originalPrice2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                originalPrice1 = (debit - credit > 0 ? debit - credit : 0);
                                originalPrice2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                originalPrice1 = (debit - credit);
                                originalPrice2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                originalPrice1 = credit;
                                originalPrice2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                originalPrice1 = (credit - debit > 0 ? credit - debit : 0);
                                originalPrice2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                originalPrice1 = (credit - debit);
                                originalPrice2 = (credit2 - debit2);
                            }
                        }
                        // Dự phòng
                        if (!string.IsNullOrEmpty(item.PreventivePriceAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventivePriceAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                preventivePrice1 = debit;
                                preventivePrice2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                preventivePrice1 = (debit - credit > 0 ? debit - credit : 0);
                                preventivePrice2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                preventivePrice1 = (debit - credit);
                                preventivePrice2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                preventivePrice1 = credit;
                                preventivePrice2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                preventivePrice1 = (credit - debit > 0 ? credit - debit : 0);
                                preventivePrice2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                preventivePrice1 = (credit - debit);
                                preventivePrice2 = (credit2 - debit2);
                            }
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
                                            OriginalPrice1 = (a.Math == "+") ? b?.OriginalPrice1 ?? 0 : -1 * b?.OriginalPrice1 ?? 0,
                                            OriginalPrice2 = (a.Math == "+") ? b?.OriginalPrice2 ?? 0 : -1 * b?.OriginalPrice2 ?? 0,
                                            PreventivePrice1 = (a.Math == "+") ? b?.PreventivePrice1 ?? 0 : -1 * b?.PreventivePrice1 ?? 0,
                                            PreventivePrice2 = (a.Math == "+") ? b?.PreventivePrice2 ?? 0 : -1 * b?.PreventivePrice2 ?? 0,
                                        }).ToList();
                        originalPrice1 = dataJoin.Sum(p => p.OriginalPrice1);
                        originalPrice2 = dataJoin.Sum(p => p.OriginalPrice2);
                        preventivePrice1 = dataJoin.Sum(p => p.PreventivePrice1);
                        preventivePrice2 = dataJoin.Sum(p => p.PreventivePrice2);
                    }
                    item.OriginalPrice1 = originalPrice1;
                    item.OriginalPrice2 = originalPrice2;
                    item.PreventivePrice1 = preventivePrice1;
                    item.PreventivePrice2 = preventivePrice2;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.OriginalPrice2.GetDefaultNullIfZero(),
                Num07 = p.PreventivePrice2.GetDefaultNullIfZero(),
                Num08 = p.OriginalPrice1.GetDefaultNullIfZero(),
                Num09 = p.PreventivePrice1.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data08(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L08 = (await _fStatement200L08Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L08 = await _generalDomainService.GetListAsync<DefaultFStatement200L08, string, TenantFStatement200L08>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L08, TenantFStatement200L08>(p)); ;
            var fStatement200L08 = (tenantFStatement200L08.Count() == 0 ? defaultFStatement200L08 : tenantFStatement200L08);
            var data = fStatement200L08.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data09(NTFS200DataParameterDto dto, List<AssetBookDto> dataAssetTool)
        {
            var tenantFStatement200L09 = (await _fStatement200L09Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L09 = await _generalDomainService.GetListAsync<DefaultFStatement200L09, string, TenantFStatement200L09>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L09, TenantFStatement200L09>(p));
            var fStatement200L09 = (tenantFStatement200L09.Count() == 0 ? defaultFStatement200L09 : tenantFStatement200L09);
            var data09 = fStatement200L09.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L09>();
            var rank = data09.Max(p => p.Rank);
            var lstAssetToolGroup = "HH1,HH2,HH3,HH4,HH5,HH6,HH7";
            var lstJobAssetTool = new List<JsonObject>();
            foreach (var item in dataAssetTool)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                lstJobAssetTool.Add((JsonObject)job);
            }
            while (rank >= 0)
            {
                var dataByRank = data09.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal vHH1 = 0;
                    decimal vHH2 = 0;
                    decimal vHH3 = 0;
                    decimal vHH4 = 0;
                    decimal vHH5 = 0;
                    decimal vHH6 = 0;
                    decimal vHH7 = 0;
                    decimal total = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (item.Condition != "")
                        {
                            int ord = 1;
                            var lstAssetToolGroupCode = GetSplit(lstAssetToolGroup, ',');
                            foreach (var itemGroupCode in lstAssetToolGroupCode)
                            {
                                decimal vGT = 0;
                                var lstJobFind = await AddCondition(lstJobAssetTool, item.Condition);
                                lstJobFind = lstJobFind.Where(p => (itemGroupCode.Data).Contains(p["AssetGroupCode"].ToString())).ToList();
                                vGT = lstJobFind.Sum(p => decimal.Parse((p[item.FieldName.ToString()] ?? "0").ToString()));
                                if(ord == 1) vHH1 = vGT;
                                if(ord == 2) vHH2 = vGT;
                                if(ord == 3) vHH3 = vGT;
                                if(ord == 4) vHH4 = vGT;
                                if(ord == 5) vHH5 = vGT;
                                if(ord == 6) vHH6 = vGT;
                                if(ord == 7) vHH7 = vGT;
                                total += vGT;
                                ord++;
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data09 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            HH1 = (a.Math == "+") ? b?.HH1 ?? 0 : -1 * b?.HH1 ?? 0,
                                            HH2 = (a.Math == "+") ? b?.HH2 ?? 0 : -1 * b?.HH2 ?? 0,
                                            HH3 = (a.Math == "+") ? b?.HH3 ?? 0 : -1 * b?.HH3 ?? 0,
                                            HH4 = (a.Math == "+") ? b?.HH4 ?? 0 : -1 * b?.HH4 ?? 0,
                                            HH5 = (a.Math == "+") ? b?.HH5 ?? 0 : -1 * b?.HH5 ?? 0,
                                            HH6 = (a.Math == "+") ? b?.HH6 ?? 0 : -1 * b?.HH6 ?? 0,
                                            HH7 = (a.Math == "+") ? b?.HH7 ?? 0 : -1 * b?.HH7 ?? 0,
                                            Total = (a.Math == "+") ? b?.Total ?? 0 : -1 * b?.Total ?? 0,
                                        }).ToList();
                        vHH1 = dataJoin.Sum(p => p.HH1);
                        vHH2 = dataJoin.Sum(p => p.HH2);
                        vHH3 = dataJoin.Sum(p => p.HH3);
                        vHH4 = dataJoin.Sum(p => p.HH4);
                        vHH5 = dataJoin.Sum(p => p.HH5);
                        vHH6 = dataJoin.Sum(p => p.HH6);
                        vHH7 = dataJoin.Sum(p => p.HH7);
                        total = dataJoin.Sum(p => p.Total);
                    }
                    item.HH1 = vHH1;
                    item.HH2 = vHH2;
                    item.HH3 = vHH3;
                    item.HH4 = vHH4;
                    item.HH5 = vHH5;
                    item.HH6 = vHH6;
                    item.HH7 = vHH7;
                    item.Total = total;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num02 = p.HH1.GetDefaultNullIfZero(),
                Num03 = p.HH2.GetDefaultNullIfZero(),
                Num04 = p.HH3.GetDefaultNullIfZero(),
                Num05 = p.HH4.GetDefaultNullIfZero(),
                Num06 = p.HH5.GetDefaultNullIfZero(),
                Num07 = p.HH6.GetDefaultNullIfZero(),
                Num08 = p.HH7.GetDefaultNullIfZero(),
                Num09 = p.Total.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data10(NTFS200DataParameterDto dto, List<AssetBookDto> dataAssetTool)
        {
            var tenantFStatement200L10 = (await _fStatement200L10Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L10 = await _generalDomainService.GetListAsync<DefaultFStatement200L10, string, TenantFStatement200L10>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L10, TenantFStatement200L10>(p));
            var fStatement200L10 = (tenantFStatement200L10.Count() == 0 ? defaultFStatement200L10 : tenantFStatement200L10);

            var data10 = fStatement200L10.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L10>();
            var rank = data10.Max(p => p.Rank);
            var lstAssetToolGroup = "VH1,VH2,VH3,VH4,VH5,VH6,VH7";
            var lstJobAssetTool = new List<JsonObject>();
            foreach (var item in dataAssetTool)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                lstJobAssetTool.Add((JsonObject)job);
            }
            while (rank >= 0)
            {
                var dataByRank = data10.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal vVH1 = 0;
                    decimal vVH2 = 0;
                    decimal vVH3 = 0;
                    decimal vVH4 = 0;
                    decimal vVH5 = 0;
                    decimal vVH6 = 0;
                    decimal vVH7 = 0;
                    decimal total = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (item.Condition != "")
                        {
                            int ord = 1;
                            var lstAssetToolGroupCode = GetSplit(lstAssetToolGroup, ',');
                            foreach (var itemGroupCode in lstAssetToolGroupCode)
                            {
                                decimal vGT = 0;
                                var lstJobFind = await AddCondition(lstJobAssetTool, item.Condition);
                                lstJobFind = lstJobFind.Where(p => (itemGroupCode.Data).Contains(p["AssetGroupCode"].ToString())).ToList();
                                vGT = lstJobFind.Sum(p => decimal.Parse((p[item.FieldName.ToString()] ?? "0").ToString() ?? "0"));
                                if (ord == 1) vVH1 = vGT;
                                if (ord == 2) vVH2 = vGT;
                                if (ord == 3) vVH3 = vGT;
                                if (ord == 4) vVH4 = vGT;
                                if (ord == 5) vVH5 = vGT;
                                if (ord == 6) vVH6 = vGT;
                                if (ord == 7) vVH7 = vGT;
                                total += vGT;
                                ord++;
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data10 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            VH1 = (a.Math == "+") ? b?.VH1 ?? 0 : -1 * b?.VH1 ?? 0,
                                            VH2 = (a.Math == "+") ? b?.VH2 ?? 0 : -1 * b?.VH2 ?? 0,
                                            VH3 = (a.Math == "+") ? b?.VH3 ?? 0 : -1 * b?.VH3 ?? 0,
                                            VH4 = (a.Math == "+") ? b?.VH4 ?? 0 : -1 * b?.VH4 ?? 0,
                                            VH5 = (a.Math == "+") ? b?.VH5 ?? 0 : -1 * b?.VH5 ?? 0,
                                            VH6 = (a.Math == "+") ? b?.VH6 ?? 0 : -1 * b?.VH6 ?? 0,
                                            VH7 = (a.Math == "+") ? b?.VH7 ?? 0 : -1 * b?.VH7 ?? 0,
                                            Total = (a.Math == "+") ? b?.Total ?? 0 : -1 * b?.Total ?? 0,
                                        }).ToList();
                        vVH1 = dataJoin.Sum(p => p.VH1);
                        vVH2 = dataJoin.Sum(p => p.VH2);
                        vVH3 = dataJoin.Sum(p => p.VH3);
                        vVH4 = dataJoin.Sum(p => p.VH4);
                        vVH5 = dataJoin.Sum(p => p.VH5);
                        vVH6 = dataJoin.Sum(p => p.VH6);
                        vVH7 = dataJoin.Sum(p => p.VH7);
                        total = dataJoin.Sum(p => p.Total);
                    }
                    item.VH1 = vVH1;
                    item.VH2 = vVH2;
                    item.VH3 = vVH3;
                    item.VH4 = vVH4;
                    item.VH5 = vVH5;
                    item.VH6 = vVH6;
                    item.VH7 = vVH7;
                    item.Total = total;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num02 = p.VH1.GetDefaultNullIfZero(),
                Num03 = p.VH2.GetDefaultNullIfZero(),
                Num04 = p.VH3.GetDefaultNullIfZero(),
                Num05 = p.VH4.GetDefaultNullIfZero(),
                Num06 = p.VH5.GetDefaultNullIfZero(),
                Num07 = p.VH6.GetDefaultNullIfZero(),
                Num08 = p.VH7.GetDefaultNullIfZero(),
                Num09 = p.Total.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data11(NTFS200DataParameterDto dto, List<AssetBookDto> dataAssetTool)
        {
            var tenantFStatement200L11 = (await _fStatement200L11Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L11 = await _generalDomainService.GetListAsync<DefaultFStatement200L11, string, TenantFStatement200L11>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L11, TenantFStatement200L11>(p));
            var fStatement200L11 = (tenantFStatement200L11.Count() == 0 ? defaultFStatement200L11 : tenantFStatement200L11);
            var data11 = fStatement200L11.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L11>();
            var rank = data11.Max(p => p.Rank);
            var lstAssetToolGroup = "TC1,TC2,TC3,TC4,TC5,TC6,TC7";
            var lstJobAssetTool = new List<JsonObject>();
            foreach (var item in dataAssetTool)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                lstJobAssetTool.Add((JsonObject)job);
            }
            while (rank >= 0)
            {
                var dataByRank = data11.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal vTC1 = 0;
                    decimal vTC2 = 0;
                    decimal vTC3 = 0;
                    decimal vTC4 = 0;
                    decimal vTC5 = 0;
                    decimal vTC6 = 0;
                    decimal vTC7 = 0;
                    decimal total = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        if (item.Condition != "")
                        {
                            int ord = 1;
                            var lstAssetToolGroupCode = GetSplit(lstAssetToolGroup, ',');
                            foreach (var itemGroupCode in lstAssetToolGroupCode)
                            {
                                decimal vGT = 0;
                                var lstJobFind = await AddCondition(lstJobAssetTool, item.Condition);
                                lstJobFind = lstJobFind.Where(p => (itemGroupCode.Data).Contains(p["AssetGroupCode"].ToString())).ToList();
                                vGT = lstJobFind.Sum(p => decimal.Parse((p[item.FieldName.ToString()] ?? "0").ToString() ?? "0"));
                                if (ord == 1) vTC1 = vGT;
                                if (ord == 2) vTC2 = vGT;
                                if (ord == 3) vTC3 = vGT;
                                if (ord == 4) vTC4 = vGT;
                                if (ord == 5) vTC5 = vGT;
                                if (ord == 6) vTC6 = vGT;
                                if (ord == 7) vTC7 = vGT;
                                total += vGT;
                                ord++;
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data11 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            TC1 = (a.Math == "+") ? b?.TC1 ?? 0 : -1 * b?.TC1 ?? 0,
                                            TC2 = (a.Math == "+") ? b?.TC2 ?? 0 : -1 * b?.TC2 ?? 0,
                                            TC3 = (a.Math == "+") ? b?.TC3 ?? 0 : -1 * b?.TC3 ?? 0,
                                            TC4 = (a.Math == "+") ? b?.TC4 ?? 0 : -1 * b?.TC4 ?? 0,
                                            TC5 = (a.Math == "+") ? b?.TC5 ?? 0 : -1 * b?.TC5 ?? 0,
                                            TC6 = (a.Math == "+") ? b?.TC6 ?? 0 : -1 * b?.TC6 ?? 0,
                                            TC7 = (a.Math == "+") ? b?.TC7 ?? 0 : -1 * b?.TC7 ?? 0,
                                            Total = (a.Math == "+") ? b?.Total ?? 0 : -1 * b?.Total ?? 0,
                                        }).ToList();
                        vTC1 = dataJoin.Sum(p => p.TC1);
                        vTC2 = dataJoin.Sum(p => p.TC2);
                        vTC3 = dataJoin.Sum(p => p.TC3);
                        vTC4 = dataJoin.Sum(p => p.TC4);
                        vTC5 = dataJoin.Sum(p => p.TC5);
                        vTC6 = dataJoin.Sum(p => p.TC6);
                        vTC7 = dataJoin.Sum(p => p.TC7);
                        total = dataJoin.Sum(p => p.Total);
                    }
                    item.TC1 = vTC1;
                    item.TC2 = vTC2;
                    item.TC3 = vTC3;
                    item.TC4 = vTC4;
                    item.TC5 = vTC5;
                    item.TC6 = vTC6;
                    item.TC7 = vTC7;
                    item.Total = total;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num02 = p.TC1.GetDefaultNullIfZero(),
                Num03 = p.TC2.GetDefaultNullIfZero(),
                Num04 = p.TC3.GetDefaultNullIfZero(),
                Num05 = p.TC4.GetDefaultNullIfZero(),
                Num06 = p.TC5.GetDefaultNullIfZero(),
                Num07 = p.TC6.GetDefaultNullIfZero(),
                Num08 = p.TC7.GetDefaultNullIfZero(),
                Num09 = p.Total.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data12(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L12 = (await _fStatement200L12Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L12 = await _generalDomainService.GetListAsync<DefaultFStatement200L12, string, TenantFStatement200L12>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L12, TenantFStatement200L12>(p));
            var fStatement200L12 = (tenantFStatement200L12.Count() == 0 ? defaultFStatement200L12 : tenantFStatement200L12);
            var data = fStatement200L12.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data13(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L13 = (await _fStatement200L13Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L13 = await _generalDomainService.GetListAsync<DefaultFStatement200L13, string, TenantFStatement200L13>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L13, TenantFStatement200L13>(p));
            var fStatement200L13 = (tenantFStatement200L13.Count() == 0 ? defaultFStatement200L13 : tenantFStatement200L13);
            var data13 = fStatement200L13.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            decimal debit1 = 0;
            decimal debit2 = 0;
            decimal credit1 = 0;
            decimal credit2 = 0;
            decimal debitIncurred = 0;
            decimal creditIncurred = 0;
            var data = new List<TenantFStatement200L13>();
            var rank = data13.Max(p => p.Rank);
            while (rank >= 0)
            {
                var dataByRank = data13.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal valueAmount1 = 0;
                    decimal up = 0;
                    decimal down = 0;
                    decimal valueAmount2 = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        // Giá trị + phát sinh
                        if (!string.IsNullOrEmpty(item.Acc))
                        {
                            // số dư
                            debit1 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit ?? 0);
                            credit1 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit ?? 0);
                            debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.Acc ?? " ")).Sum(p => p.Credit2 ?? 0);

                            // phát sinh
                            debitIncurred = dataLedger.Where(p => (p.DebitAcc ?? "").StartsWith(item.Acc ?? " ") || (p.CreditAcc ?? "").StartsWith(item.Acc ?? " "))
                                                      .Sum(p => (p.DebitAcc ?? "").StartsWith(item.Acc ?? " ") ? p.Amount ?? 0 : 0);
                            creditIncurred = dataLedger.Where(p => (p.DebitAcc ?? "").StartsWith(item.Acc ?? " ") || (p.CreditAcc ?? "").StartsWith(item.Acc ?? " "))
                                                       .Sum(p => (p.CreditAcc ?? "").StartsWith(item.Acc ?? " ") ? p.Amount ?? 0 : 0);
                            if (item.Type == "N")
                            {
                                valueAmount1 = debit1 - credit1;
                                valueAmount2 = debit2 - credit2;
                                up = debitIncurred;
                                down = creditIncurred;
                            }
                            else if (item.Type == "C")
                            {
                                valueAmount1 = credit1 - debit1;
                                valueAmount2 = credit2 - debit2;
                                up = creditIncurred;
                                down = debitIncurred;
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data13 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            ValueAmount1 = (a.Math == "+") ? b?.ValueAmount1 ?? 0 : -1 * b?.ValueAmount1 ?? 0,
                                            Up = (a.Math == "+") ? b?.Up ?? 0 : -1 * b?.Up ?? 0,
                                            Down = (a.Math == "+") ? b?.Down ?? 0 : -1 * b?.Down ?? 0,
                                            ValueAmount2 = (a.Math == "+") ? b?.ValueAmount2 ?? 0 : -1 * b?.ValueAmount2 ?? 0,
                                        }).ToList();
                        valueAmount1 = dataJoin.Sum(p => p.ValueAmount1);
                        up = dataJoin.Sum(p => p.Up);
                        down = dataJoin.Sum(p => p.Down);
                    }
                    item.ValueAmount1 = valueAmount1;
                    item.Up = up;
                    item.Down = down;
                    item.ValueAmount2 = valueAmount2;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num02 = p.ValueAmount1.GetDefaultNullIfZero(),
                Num05 = p.Up.GetDefaultNullIfZero(),
                Num06 = p.Down.GetDefaultNullIfZero(),
                Num07 = p.ValueAmount2.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data14(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L14 = (await _fStatement200L14Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L14 = await _generalDomainService.GetListAsync<DefaultFStatement200L14, string, TenantFStatement200L14>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L14, TenantFStatement200L14>(p));
            var fStatement200L14 = (tenantFStatement200L14.Count() == 0 ? defaultFStatement200L14 : tenantFStatement200L14);
            var data = fStatement200L14.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data15(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L15 = (await _fStatement200L15Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L15 = await _generalDomainService.GetListAsync<DefaultFStatement200L15, string, TenantFStatement200L15>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L15, TenantFStatement200L15>(p));
            var fStatement200L15 = (tenantFStatement200L15.Count() == 0 ? defaultFStatement200L15 : tenantFStatement200L15);
            var data = fStatement200L15.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data16(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L16 = (await _fStatement200L16Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L16 = await _generalDomainService.GetListAsync<DefaultFStatement200L16, string, TenantFStatement200L16>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L16, TenantFStatement200L16>(p));
            var fStatement200L16 = (tenantFStatement200L16.Count() == 0 ? defaultFStatement200L16 : tenantFStatement200L16);
            var data16 = fStatement200L16.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L16>();
            var rank = data16.Max(p => p.Rank);
            while (rank >= 0)
            {
                var dataByRank = data16.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal valueAmount1 = 0;
                    decimal preventiveAmount1 = 0;
                    decimal valueAmount2 = 0;
                    decimal preventiveAmount2 = 0;
                    if (string.IsNullOrEmpty(item.Formular))
                    {
                        // Giá gốc
                        if (!string.IsNullOrEmpty(item.ValueAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.ValueAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                valueAmount1 = debit;
                                valueAmount2 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                valueAmount1 = (debit - credit > 0 ? debit - credit : 0);
                                valueAmount2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                valueAmount1 = (debit - credit);
                                valueAmount2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                valueAmount1 = credit;
                                valueAmount2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                valueAmount1 = (credit - debit > 0 ? credit - debit : 0);
                                valueAmount2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                valueAmount1 = (credit - debit);
                                valueAmount2 = (credit2 - debit2);
                            }
                        }
                        // Dự phòng
                        if (!string.IsNullOrEmpty(item.PreventiveAcc))
                        {
                            var debit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Debit ?? 0);
                            var credit = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Credit ?? 0);
                            var debit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Debit2 ?? 0);
                            var credit2 = dataBalance.Where(p => (p.AccCode ?? "").StartsWith(item.PreventiveAcc ?? " ")).Sum(p => p.Credit2 ?? 0);
                            if (item.DebitOrCredit == "N" && item.Type == "L")
                            {
                                preventiveAmount1 = debit;
                                preventiveAmount1 = debit2;
                            }
                            else if (item.DebitOrCredit == "N" && item.Type == "U")
                            {
                                preventiveAmount1 = (debit - credit > 0 ? debit - credit : 0);
                                preventiveAmount2 = (debit2 - credit2 > 0 ? debit2 - credit2 : 0);
                            }
                            else if (item.DebitOrCredit == "N" && item.Type != "L" && item.Type != "U")
                            {
                                preventiveAmount1 = (debit - credit);
                                preventiveAmount2 = (debit2 - credit2);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "L")
                            {
                                preventiveAmount1 = credit;
                                preventiveAmount2 = credit2;
                            }
                            else if (item.DebitOrCredit == "C" && item.Type == "U")
                            {
                                preventiveAmount1 = (credit - debit > 0 ? credit - debit : 0);
                                preventiveAmount2 = (credit2 - debit2 > 0 ? credit2 - debit2 : 0);
                            }
                            else if (item.DebitOrCredit == "C" && item.Type != "L" && item.Type != "U")
                            {
                                preventiveAmount1 = (credit - debit);
                                preventiveAmount2 = (credit2 - debit2);
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data16 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            ValueAmount1 = (a.Math == "+") ? b?.ValueAmount1 ?? 0 : -1 * b?.ValueAmount1 ?? 0,
                                            ValueAmount2 = (a.Math == "+") ? b?.ValueAmount2 ?? 0 : -1 * b?.ValueAmount2 ?? 0,
                                            PreventiveAmount1 = (a.Math == "+") ? b?.PreventiveAmount1 ?? 0 : -1 * b?.PreventiveAmount1 ?? 0,
                                            PreventiveAmount2 = (a.Math == "+") ? b?.PreventiveAmount2 ?? 0 : -1 * b?.PreventiveAmount2 ?? 0,
                                        }).ToList();
                        valueAmount1 = dataJoin.Sum(p => p.ValueAmount1);
                        valueAmount2 = dataJoin.Sum(p => p.ValueAmount2);
                        preventiveAmount1 = dataJoin.Sum(p => p.PreventiveAmount1);
                        preventiveAmount2 = dataJoin.Sum(p => p.PreventiveAmount2);
                    }
                    item.ValueAmount1 = valueAmount1;
                    item.ValueAmount2 = valueAmount2;
                    item.PreventiveAmount1 = preventiveAmount1;
                    item.PreventiveAmount2 = preventiveAmount2;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num06 = p.ValueAmount2.GetDefaultNullIfZero(),
                Num07 = p.PreventiveAmount2.GetDefaultNullIfZero(),
                Num08 = p.ValueAmount1.GetDefaultNullIfZero(),
                Num09 = p.PreventiveAmount1.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data17(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L17 = (await _fStatement200L17Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L17 = await _generalDomainService.GetListAsync<DefaultFStatement200L17, string, TenantFStatement200L17>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L17, TenantFStatement200L17>(p));
            var fStatement200L17 = (tenantFStatement200L17.Count() == 0 ? defaultFStatement200L17 : tenantFStatement200L17);

            var data17 = fStatement200L17.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L17>();
            var lstJobLedger = new List<JsonObject>();
            foreach (var item in dataLedger)
            {
                var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                var job = (JsonObject)JsonObject.Parse(json);
                lstJobLedger.Add((JsonObject)job);
            }
            var rank = data17.Max(p => p.Rank);
            while (rank >= 0)
            {
                var dataByRank = data17.Where(p => (p.Rank ?? 1) == rank).ToList();
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
                        if (!string.IsNullOrEmpty(item.Acc) || (item.Condition ?? "") != "")
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
                            }
                            if ((item.Method ?? "") == "debitIncurred")
                            {
                                lstJobLedger = lstJobLedger.Where(p => ((p["DebitAcc"] ?? "").ToString()).StartsWith(item.Acc)).ToList();
                                if ((item.Condition ?? "") != "")
                                {
                                    lstJobLedger = await AddCondition(lstJobLedger, item.Condition);
                                }
                                debitIncurred = lstJobLedger.Sum(p => decimal.Parse(p["Amount"].ToString() ?? "0"));
                            }
                            if ((item.Method ?? "") == "creditIncurred")
                            {
                                lstJobLedger = lstJobLedger.Where(p => ((p["CreditAcc"] ?? "").ToString()).StartsWith(item.Acc)).ToList();
                                if ((item.Condition ?? "") != "")
                                {
                                    lstJobLedger = await AddCondition(lstJobLedger, item.Condition);
                                }
                                creditIncurred = lstJobLedger.Sum(p => decimal.Parse(p["Amount"].ToString() ?? "0"));
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data17 on a.Code equals b.NumberCode into ajb
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
            var res = data.Select(p => new NoteToFinancialStatement200Dto
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

        public async Task<List<NoteToFinancialStatement200Dto>> Data18(NTFS200DataParameterDto dto)
        {
            var tenantFStatement200L18 = (await _fStatement200L18Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L18 = await _generalDomainService.GetListAsync<DefaultFStatement200L18, string, TenantFStatement200L18>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L18, TenantFStatement200L18>(p));
            var fStatement200L18 = (tenantFStatement200L18.Count() == 0 ? defaultFStatement200L18 : tenantFStatement200L18);
            var data = fStatement200L18.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => new NoteToFinancialStatement200Dto
                                       {
                                           GroupId = p.GroupId,
                                           Sort = p.Sort ?? 0,
                                           Ord = p.Ord ?? 0,
                                           Printable = p.Printable,
                                           Bold = p.Bold,
                                           Target = p.Description ?? "",
                                       }).ToList();
            return data;
        }

        public async Task<List<NoteToFinancialStatement200Dto>> Data19(NTFS200DataParameterDto dto, List<NTFS200LedgerDto> dataLedger, List<NTFS200LedgerDto> dataBalance)
        {
            var tenantFStatement200L19 = (await _fStatement200L19Service.GetQueryableAsync())
                                         .Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == dto.Year && p.UsingDecision == dto.UsingDecision)
                                         .ToList();
            var defaultFStatement200L19 = await _generalDomainService.GetListAsync<DefaultFStatement200L19, string, TenantFStatement200L19>(p =>
                                    ObjectMapper.Map<DefaultFStatement200L19, TenantFStatement200L19>(p));
            var fStatement200L19 = (tenantFStatement200L19.Count() == 0 ? defaultFStatement200L19 : tenantFStatement200L19);
            var data19 = fStatement200L19.Where(p =>  p.UsingDecision == dto.UsingDecision)
                                       .OrderBy(p => p.Ord)
                                       .Select(p => p).ToList();
            var data = new List<TenantFStatement200L19>();
            var rank = data19.Max(p => p.Rank);
            var lstAccCode0 = "4111|4112|4118|419|412|413|441|414,415,418,421,431,461,466";
            decimal vGT = 0;
            while (rank >= 0)
            {
                var dataByRank = data19.Where(p => (p.Rank ?? 1) == rank).ToList();
                foreach (var item in dataByRank)
                {
                    decimal vNV1 = 0;
                    decimal vNV2 = 0;
                    decimal vNV3 = 0;
                    decimal vNV4 = 0;
                    decimal vNV5 = 0;
                    decimal vNV6 = 0;
                    decimal vNV7 = 0;
                    decimal vNV8 = 0;
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
                                    var lstAcc19 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc19 on 0 equals 0
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
                                    var lstAcc19 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.CreditAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc19 on 0 equals 0
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
                                    var lstAcc19 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc19 on 0 equals 0
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
                                    var lstAcc19 = GetSplit(item.Acc, ',');
                                    var dataJoin = (from a in dataLedger
                                                    join b in lstAcc on 0 equals 0
                                                    where (a.CreditAcc ?? "").StartsWith(b.Data)
                                                    join c in lstAcc19 on 0 equals 0
                                                    where (a.DebitAcc ?? "").StartsWith(c.Data)
                                                    select new
                                                    {
                                                        Amount = a.Amount,
                                                    }).ToList();
                                    vGT = dataJoin.Sum(p => p.Amount ?? 0);
                                }
                            }
                            if (ord == 1) vNV1 = vGT;
                            if (ord == 2) vNV2 = vGT;
                            if (ord == 3) vNV3 = vGT;
                            if (ord == 4) vNV4 = vGT;
                            if (ord == 5) vNV5 = vGT;
                            if (ord == 6) vNV6 = vGT;
                            if (ord == 7) vNV7 = vGT;
                            if (ord == 8) vNV8 = vGT;
                            total += vGT;
                            ord++;
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in data19 on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            NV1 = (a.Math == "+") ? b?.NV1 ?? 0 : -1 * b?.NV1 ?? 0,
                                            NV2 = (a.Math == "+") ? b?.NV2 ?? 0 : -1 * b?.NV2 ?? 0,
                                            NV3 = (a.Math == "+") ? b?.NV3 ?? 0 : -1 * b?.NV3 ?? 0,
                                            NV4 = (a.Math == "+") ? b?.NV4 ?? 0 : -1 * b?.NV4 ?? 0,
                                            NV5 = (a.Math == "+") ? b?.NV5 ?? 0 : -1 * b?.NV5 ?? 0,
                                            NV6 = (a.Math == "+") ? b?.NV6 ?? 0 : -1 * b?.NV6 ?? 0,
                                            NV7 = (a.Math == "+") ? b?.NV7 ?? 0 : -1 * b?.NV7 ?? 0,
                                            NV8 = (a.Math == "+") ? b?.NV8 ?? 0 : -1 * b?.NV8 ?? 0,
                                            Total = (a.Math == "+") ? b?.Total ?? 0 : -1 * b?.Total ?? 0,
                                        }).ToList();
                        vNV1 = dataJoin.Sum(p => p.NV1);
                        vNV2 = dataJoin.Sum(p => p.NV2);
                        vNV3 = dataJoin.Sum(p => p.NV3);
                        vNV4 = dataJoin.Sum(p => p.NV4);
                        vNV5 = dataJoin.Sum(p => p.NV5);
                        vNV6 = dataJoin.Sum(p => p.NV6);
                        vNV7 = dataJoin.Sum(p => p.NV7);
                        vNV8 = dataJoin.Sum(p => p.NV8);
                        total = dataJoin.Sum(p => p.Total);
                    }
                    item.NV1 = vNV1;
                    item.NV2 = vNV2;
                    item.NV3 = vNV3;
                    item.NV4 = vNV4;
                    item.NV5 = vNV5;
                    item.NV6 = vNV6;
                    item.NV7 = vNV7;
                    item.Total = total;
                }
                data.AddRange(dataByRank);
                rank--;
            }
            var res = data.Select(p => new NoteToFinancialStatement200Dto
            {
                GroupId = p.GroupId,
                Sort = p.Sort ?? 0,
                Ord = p.Ord ?? 0,
                Bold = p.Bold,
                Printable = p.Printable,
                Target = p.Description,
                Num01 = p.NV1.GetDefaultNullIfZero(),
                Num02 = p.NV2.GetDefaultNullIfZero(),
                Num03 = p.NV3.GetDefaultNullIfZero(),
                Num04 = p.NV4.GetDefaultNullIfZero(),
                Num05 = p.NV5.GetDefaultNullIfZero(),
                Num06 = p.NV6.GetDefaultNullIfZero(),
                Num07 = p.NV7.GetDefaultNullIfZero(),
                Num08 = p.NV8.GetDefaultNullIfZero(),
                Num09 = p.Total.GetDefaultNullIfZero(),
            }).ToList();
            return res;
        }

        // ------------------------------------------------------
        [Authorize(ReportPermissions.FinancialStatementReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<NTFS200ParameterDto> dto)
        {
            if (dto == null)
            {
                throw new ArgumentNullException(nameof(dto));
            }
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

        public async Task<List<AssetBookDto>> GetAssetTool(ReportRequestDto<NTFS200ParameterDto> dto)
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

        public async Task<List<NTFS200LedgerDto>> GetDataBalance(ReportRequestDto<NTFS200ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lst = new List<NoteToFinancialStatement200Dto>();
            var year = dto.Parameters.FromDate.Value.Year;
            var yearPre = dto.Parameters.FromDatePre.Value.Year;
            var beginDate = DateTime.Parse(dto.Parameters.FromDate.Value.Year + "/01/01");
            var beginDatePre = DateTime.Parse(dto.Parameters.FromDatePre.Value.Year + "/01/01");
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            int? usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 200 : dto.Parameters.UsingDecision;
            var removeDuplicate = await _tenantSettingService.GetValue("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var ledger = await _ledgerService.GetQueryableAsync();
            var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
            //Lấy dữ liệu phát sinh
            var dataLedger0 = (from a in ledger
                               where a.OrgCode == orgCode && String.Compare(a.Status, "2") < 0
                                  && (a.VoucherDate >= beginDatePre && a.VoucherDate <= dto.Parameters.ToDatePre
                                      || a.VoucherDate >= beginDate && a.VoucherDate <= dto.Parameters.ToDate)
                               select new NTFS200LedgerDto
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
                                          .Select(p => new NTFS200LedgerDto
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
                                        select new NTFS200LedgerDto
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
                                           select new NTFS200LedgerDto
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
                                           select new NTFS200LedgerDto
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
                                .Select(p => new NTFS200LedgerDto
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
            }).Select(p => new NTFS200LedgerDto
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

        public async Task<List<NTFS200LedgerDto>> GetDataLedger(ReportRequestDto<NTFS200ParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lst = new List<NoteToFinancialStatement200Dto>();
            var year = dto.Parameters.FromDate.Value.Year;
            var yearPre = dto.Parameters.FromDatePre.Value.Year;
            var beginDate = DateTime.Parse(dto.Parameters.FromDate.Value.Year + "/01/01");
            var beginDatePre = DateTime.Parse(dto.Parameters.FromDatePre.Value.Year + "/01/01");
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            int? usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 200 : dto.Parameters.UsingDecision;
            var removeDuplicate = await _tenantSettingService.GetValue("VHT_CO_KHU_TRUNG", _webHelper.GetCurrentOrgUnit());
            var ledger = await _ledgerService.GetQueryableAsync();
            var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
            //Lấy dữ liệu phát sinh
            var dataLedger0 = (from a in ledger
                               where a.OrgCode == orgCode && String.Compare(a.Status, "2") < 0
                                  && (a.VoucherDate >= beginDatePre && a.VoucherDate <= dto.Parameters.ToDatePre
                                      || a.VoucherDate >= beginDate && a.VoucherDate <= dto.Parameters.ToDate)
                               select new NTFS200LedgerDto
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
                                          .Select(p => new NTFS200LedgerDto
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
                        jsonObject = jsonObject.Where(p => item["Value"].ToString().Contains((p[item["FieldName"].ToString()] ?? "_").ToString())).ToList();
                        break;
                    case "!Contains":
                        jsonObject = jsonObject.Where(p => !item["Value"].ToString().Contains((p[item["FieldName"].ToString()] ?? "_").ToString())).ToList();
                        break;
                    case "!=":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString() != ((p[item["FieldName"].ToString()] ?? "_").ToString())).ToList();
                        break;
                    case "==":
                        jsonObject = jsonObject.Where(p => item["Value"].ToString() == ((p[item["FieldName"].ToString()] ?? "_").ToString())).ToList();
                        break;
                    case "1=1":
                        jsonObject = jsonObject.ToList();
                        break;
                    case "1=0":
                        jsonObject = jsonObject.Where(p => "1" == "0").ToList();
                        break;
                    case "StartsWith":
                        jsonObject = jsonObject.Where(p => ((p[item["FieldName"].ToString()] ?? "").ToString().StartsWith(item["Value"].ToString()))).ToList();
                        break;
                    case ">":
                        jsonObject = jsonObject.Where(p => decimal.Parse((p[item["FieldName"].ToString()] ?? "").ToString()) > decimal.Parse(item["Value"].ToString())).ToList();
                        break;
                    case "<":
                        jsonObject = jsonObject.Where(p => decimal.Parse((p[item["FieldName"].ToString()] ?? "").ToString()) < decimal.Parse(item["Value"].ToString())).ToList();
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
        #endregion
    }
}

