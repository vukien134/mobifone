using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Licenses;
using Accounting.Localization;
using Accounting.Report;
using Accounting.Reports;
using Accounting.Reports.Cores;
using Accounting.Reports.Financials;
using Accounting.Reports.Financials.StatementOfValueAddedTax;
using Accounting.Reports.Tenants;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Business
{
    public class AccBalanceSheetReportBusiness : BaseBusiness, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly DefaultAccBalanceSheetService _defaultAccBalanceSheetService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly IObjectMapper _objectMapper;

        #endregion
        public AccBalanceSheetReportBusiness(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                        TenantAccBalanceSheetService tenantAccBalanceSheetService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager,
                        IObjectMapper objectMapper,
                        IStringLocalizer<AccountingResource> localizer) : base(localizer)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
            _objectMapper = objectMapper;
        }
        #region Methods
        public async Task<ReportResponseDto<AccBalanceSheetDto>> CreateDataAsync(ReportRequestDto<AccBalanceSheetParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var accountingSystems = await this.GetAccountSystems(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dto.Parameters, accountingSystems);
            var incurredData = await GetIncurredData(dic);
            var lst = new List<AccBalanceSheetDto>();
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            int? usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 200 : dto.Parameters.UsingDecision;
            int? clearingIncurred = (dto.Parameters.ClearingIncurred == null) ? 0 : dto.Parameters.ClearingIncurred;
            int? clearingIncurredFP = (dto.Parameters.ClearingIncurredFP == null) ? 1 : dto.Parameters.ClearingIncurred;
            int? isSummary = (dto.Parameters.IsSummary == null) ? 0 : dto.Parameters.IsSummary;
            string check = ((dto.Parameters.Check ?? "") == "") ? "K" : dto.Parameters.Check;
            var tenantAccBalanceSheet = await _tenantAccBalanceSheetService.GetAllAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            var defaultAccBalanceSheet = await _defaultAccBalanceSheetService.GetAllAsync();
            openingBalance.AddRange((from a in incurredData
                                     join b in accountingSystems on a.Acc equals b.AccCode into ajb
                                     from b in ajb.DefaultIfEmpty()
                                     group new { a, b } by new
                                     {
                                         a.OrgCode,
                                         a.Year,
                                         a.Acc,
                                         a.CurrencyCode,
                                         a.PartnerCode,
                                         a.ContractCode,
                                         a.FProductWorkCode,
                                         a.WorkPlaceCode,
                                         a.SectionCode,
                                     } into gr
                                     select new AccOpeningBalanceReportDto
                                     {
                                         OrgCode = gr.Key.OrgCode,
                                         Year = gr.Key.Year,
                                         AccCode = gr.Key.Acc,
                                         PartnerCode = gr.Key.PartnerCode,
                                         FProductWorkCode = gr.Key.FProductWorkCode,
                                         WorkPlaceCode = gr.Key.WorkPlaceCode,
                                         SectionCode = gr.Key.SectionCode,
                                         CurrencyCode = gr.Key.CurrencyCode,
                                         ContractCode = gr.Key.ContractCode,
                                         DebitCur1 = 0,
                                         Debit1 = 0,
                                         CreditCur1 = 0,
                                         Credit1 = 0,
                                         DebitIncurredCur = gr.Sum(p => p.a.DebitIncurredCur ?? 0),
                                         DebitIncurred = gr.Sum(p => p.a.DebitIncurred ?? 0),
                                         CreditIncurredCur = gr.Sum(p => p.a.CreditIncurredCur ?? 0),
                                         CreditIncurred = gr.Sum(p => p.a.CreditIncurred ?? 0),
                                         DebitAccumulationCur = gr.Sum(p => p.a.DebitIncurredCur ?? 0),
                                         DebitAccumulation = gr.Sum(p => p.a.DebitIncurred ?? 0),
                                         CreditAccumulationCur = gr.Sum(p => p.a.CreditIncurredCur ?? 0),
                                         CreditAccumulation = gr.Sum(p => p.a.CreditIncurred ?? 0),
                                         AttachPartner = gr.Max(p => p.b?.AttachPartner ?? ""),
                                         AttachAccSection = gr.Max(p => p.b?.AttachAccSection ?? ""),
                                         AttachContract = gr.Max(p => p.b?.AttachContract ?? ""),
                                         AttachWorkPlace = gr.Max(p => p.b?.AttachWorkPlace ?? ""),
                                         AttachProductCost = gr.Max(p => p.b?.AttachProductCost ?? ""),
                                         IsBalanceSheetAcc = gr.Max(p => p.b?.IsBalanceSheetAcc ?? ""),
                                         AttachCurrency = gr.Max(p => p.b?.AttachCurrency ?? ""),
                                         AccPattern = gr.Max(p => p.b?.AccPattern ?? null),
                                     }
                                     ).ToList());
            foreach (var itemOpeningBalance in openingBalance)
            {
                if (itemOpeningBalance.PartnerCode == "C" && clearingIncurredFP == 1) itemOpeningBalance.FProductWorkCode = "";
                itemOpeningBalance.ContractCode = "";
                itemOpeningBalance.SectionCode = "";
                itemOpeningBalance.WorkPlaceCode = "";
            }
            var lstOpeningBalance = openingBalance.GroupBy(g => new
            {
                g.Year,
                g.AccCode,
                g.PartnerCode,
                g.ContractCode,
                g.SectionCode,
                g.WorkPlaceCode,
            }).Select(p => new AccOpeningBalanceReportDto
            {
                OrgCode = _webHelper.GetCurrentOrgUnit(),
                Year = p.Key.Year,
                AccCode = p.Key.AccCode,
                PartnerCode = p.Key.PartnerCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                SectionCode = p.Key.SectionCode,
                ContractCode = p.Key.ContractCode,
                DebitCur1 = p.Sum(p => p.DebitCur1 ?? 0),
                Debit1 = p.Sum(p => p.Debit1 ?? 0),
                CreditCur1 = p.Sum(p => p.CreditCur1 ?? 0),
                Credit1 = p.Sum(p => p.Credit1 ?? 0),
                _DebitCur1 = p.Sum(p => p.DebitCur1 ?? 0),
                _Debit1 = p.Sum(p => p.Debit1 ?? 0),
                _CreditCur1 = p.Sum(p => p.CreditCur1 ?? 0),
                _Credit1 = p.Sum(p => p.Credit1 ?? 0),
                DebitIncurredCur = p.Sum(p => p.DebitIncurredCur ?? 0),
                DebitIncurred = p.Sum(p => p.DebitIncurred ?? 0),
                CreditIncurredCur = p.Sum(p => p.CreditIncurredCur ?? 0),
                CreditIncurred = p.Sum(p => p.CreditIncurred ?? 0),
                DebitAccumulationCur = p.Sum(p => p.DebitIncurredCur ?? 0),
                DebitAccumulation = p.Sum(p => p.DebitIncurred ?? 0),
                CreditAccumulationCur = p.Sum(p => p.CreditIncurredCur ?? 0),
                CreditAccumulation = p.Sum(p => p.CreditIncurred ?? 0),
            }).ToList();
            foreach (var itemOpeningBalance in lstOpeningBalance)
            {
                var debitCur1 = itemOpeningBalance._DebitCur1;
                var creditCur1 = itemOpeningBalance._CreditCur1;
                var debit1 = itemOpeningBalance._Debit1;
                var credit1 = itemOpeningBalance._Credit1;
                var debitIncurredCur = itemOpeningBalance.DebitIncurredCur;
                var debitIncurred = itemOpeningBalance.DebitIncurred;
                var creditIncurredCur = itemOpeningBalance.CreditIncurredCur;
                var creditIncurred = itemOpeningBalance.CreditIncurred;

                // Update đầu kỳ
                itemOpeningBalance.DebitCur1 = (debitCur1 > creditCur1) ? debitCur1 - creditCur1 : 0;
                itemOpeningBalance.CreditCur1 = (creditCur1 > debitCur1) ? creditCur1 - debitCur1 : 0;
                itemOpeningBalance.Debit1 = (debit1 > credit1) ? debit1 - credit1 : 0;
                itemOpeningBalance.Credit1 = (credit1 > debit1) ? credit1 - debit1 : 0;

                // Update cuối kỳ
                itemOpeningBalance.DebitCur2 = (debitCur1 + debitIncurredCur > creditCur1 + creditIncurredCur) ? (debitCur1 + debitIncurredCur - creditCur1 - creditIncurredCur) : 0;
                itemOpeningBalance.CreditCur2 = (creditCur1 + creditIncurredCur > debitCur1 + debitIncurredCur) ? (creditCur1 + creditIncurredCur - debitCur1 - debitIncurredCur) : 0;
                itemOpeningBalance.Debit2 = (debit1 + debitIncurred > credit1 + creditIncurred) ? debit1 + debitIncurred - credit1 - creditIncurred : 0;
                itemOpeningBalance.Credit2 = (credit1 + creditIncurred > debit1 + debitIncurred) ? credit1 + creditIncurred - debit1 - debitIncurred : 0;
            }
            // Tạo bảng cân đối số phát sinh
            var balanceSheetAcc = lstOpeningBalance.OrderBy(p => p.AccCode).GroupBy(g => new { g.AccCode }).Select(p => new AccOpeningBalanceReportDto
            {
                Tag = 0,
                AccCode = p.Key.AccCode,
                DebitCur1 = p.Sum(p => p.DebitCur1 ?? 0),
                Debit1 = p.Sum(p => p.Debit1 ?? 0),
                CreditCur1 = p.Sum(p => p.CreditCur1 ?? 0),
                Credit1 = p.Sum(p => p.Credit1 ?? 0),
                _DebitCur1 = p.Sum(p => p.DebitCur1 ?? 0),
                _Debit1 = p.Sum(p => p.Debit1 ?? 0),
                _CreditCur1 = p.Sum(p => p.CreditCur1 ?? 0),
                _Credit1 = p.Sum(p => p.Credit1 ?? 0),
                DebitCur2 = p.Sum(p => p.DebitCur2 ?? 0),
                Debit2 = p.Sum(p => p.Debit2 ?? 0),
                CreditCur2 = p.Sum(p => p.CreditCur2 ?? 0),
                Credit2 = p.Sum(p => p.Credit2 ?? 0),
                DebitIncurredCur = p.Sum(p => p.DebitIncurredCur ?? 0),
                DebitIncurred = p.Sum(p => p.DebitIncurred ?? 0),
                CreditIncurredCur = p.Sum(p => p.CreditIncurredCur ?? 0),
                CreditIncurred = p.Sum(p => p.CreditIncurred ?? 0),
                DebitAccumulationCur = p.Sum(p => p.DebitIncurredCur ?? 0),
                DebitAccumulation = p.Sum(p => p.DebitIncurred ?? 0),
                CreditAccumulationCur = p.Sum(p => p.CreditIncurredCur ?? 0),
                CreditAccumulation = p.Sum(p => p.CreditIncurred ?? 0)
            }).ToList();
            balanceSheetAcc = balanceSheetAcc.Where(p => (p.DebitCur1 ?? 0) != 0 || (p.Debit1 ?? 0) != 0 || (p.CreditCur1 ?? 0) != 0 || (p.Credit1 ?? 0) != 0
                                                      || (p.DebitIncurredCur ?? 0) != 0 || (p.DebitIncurred ?? 0) != 0 || (p.CreditIncurredCur ?? 0) != 0
                                                      || (p.CreditIncurred ?? 0) != 0 || (p.DebitCur2 ?? 0) != 0 || (p.Debit2 ?? 0) != 0 || (p.CreditCur2 ?? 0) != 0 || (p.Credit2 ?? 0) != 0
                                                      || (p.DebitAccumulationCur ?? 0) != 0 || (p.DebitAccumulation ?? 0) != 0 || (p.CreditAccumulationCur ?? 0) != 0 || (p.CreditAccumulation ?? 0) != 0).ToList();
            if (clearingIncurred == 1)
            {
                foreach (var itemBalanceSheetAcc in balanceSheetAcc)
                {
                    var debitCur1 = itemBalanceSheetAcc._DebitCur1;
                    var creditCur1 = itemBalanceSheetAcc._CreditCur1;
                    var debit1 = itemBalanceSheetAcc._Debit1;
                    var credit1 = itemBalanceSheetAcc._Credit1;
                    var debitIncurredCur = itemBalanceSheetAcc.DebitIncurredCur;
                    var debitIncurred = itemBalanceSheetAcc.DebitIncurred;
                    var creditIncurredCur = itemBalanceSheetAcc.CreditIncurredCur;
                    var creditIncurred = itemBalanceSheetAcc.CreditIncurred;

                    // Update đầu kỳ
                    itemBalanceSheetAcc.DebitCur1 = (debitCur1 > creditCur1) ? debitCur1 - creditCur1 : 0;
                    itemBalanceSheetAcc.CreditCur1 = (creditCur1 > debitCur1) ? creditCur1 - debitCur1 : 0;
                    itemBalanceSheetAcc.Debit1 = (debit1 > credit1) ? debit1 - credit1 : 0;
                    itemBalanceSheetAcc.Credit1 = (credit1 > debit1) ? credit1 - debit1 : 0;

                    // Update cuối kỳ
                    itemBalanceSheetAcc.DebitCur2 = (debitCur1 + debitIncurredCur > creditCur1 + creditIncurredCur) ? (debitCur1 + debitIncurredCur - creditCur1 - creditIncurredCur) : 0;
                    itemBalanceSheetAcc.CreditCur2 = (creditCur1 + creditIncurredCur > debitCur1 + debitIncurredCur) ? (creditCur1 + creditIncurredCur - debitCur1 - debitIncurredCur) : 0;
                    itemBalanceSheetAcc.Debit2 = (debit1 + debitIncurred > credit1 + creditIncurred) ? debit1 + debitIncurred - credit1 - creditIncurred : 0;
                    itemBalanceSheetAcc.Credit2 = (credit1 + creditIncurred > debit1 + debitIncurred) ? credit1 + creditIncurred - debit1 - debitIncurred : 0;
                }
            }
            // LẤY DỮ LIỆU KHAI BÁO	
            var dataAccBalanceSheet = (tenantAccBalanceSheet.Count > 0) ?
                                      tenantAccBalanceSheet.OrderBy(p => p.Ord).Where(p => p.UsingDecision == usingDecision).Select(p => _objectMapper.Map<TenantAccBalanceSheet, AccBalanceSheet>(p)).ToList() :
                                      defaultAccBalanceSheet.OrderBy(p => p.Ord).Where(p => p.UsingDecision == usingDecision).Select(p => _objectMapper.Map<DefaultAccBalanceSheet, AccBalanceSheet>(p)).ToList();
            var rank = dataAccBalanceSheet.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataAccBalanceSheetByRank = dataAccBalanceSheet.Where(p => p.Rank == rank).ToList();
                foreach (var item in dataAccBalanceSheetByRank)
                {
                    decimal openingAmountCur = 0,
                            openingAmount = 0,
                            endingAmountCur = 0,
                            endingAmount = 0;
                    if ((item.Formular ?? "") == "")
                    {
                        if (item.Acc != "")
                        {
                            if (check == "C")
                            {
                                int TagError = 0;
                                if (item.DebitOrCredit == "N" && item.Type == "L") TagError = 1;
                                if (item.DebitOrCredit == "C" && item.Type == "L") TagError = 2;
                                if ((item.DebitOrCredit ?? "") != "L") TagError = 3;
                                if (!accountingSystems.Any(p => p.AccCode == item.Acc)) TagError = 4;
                                foreach (var itemBalanceSheetAcc in balanceSheetAcc)
                                {
                                    if (itemBalanceSheetAcc.Tag != 3 && itemBalanceSheetAcc.AccCode.StartsWith(item.Acc))
                                    {
                                        itemBalanceSheetAcc.Tag = ((itemBalanceSheetAcc.Tag == 1 && TagError == 2)
                                                                || (itemBalanceSheetAcc.Tag == 2 && TagError == 1)) ? 3 : (itemBalanceSheetAcc.Tag == 3 ? 4 : TagError);
                                    }
                                }
                            }

                            if (item.Type == "L")
                            {
                                if (item.DebitOrCredit == "N")
                                {
                                    openingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.DebitCur1 ?? 0);
                                    openingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.Debit1 ?? 0);
                                    endingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.DebitCur2 ?? 0);
                                    endingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.Debit2 ?? 0);
                                }
                                else if (item.DebitOrCredit == "C")
                                {
                                    openingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.CreditCur1 ?? 0);
                                    openingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.Credit1 ?? 0);
                                    endingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.CreditCur2 ?? 0);
                                    endingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => p.Credit2 ?? 0);
                                }
                            }
                            else
                            {
                                if (item.DebitOrCredit == "N")
                                {
                                    openingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.DebitCur1 ?? 0) - (p.CreditCur1 ?? 0));
                                    openingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.Debit1 ?? 0) - (p.Credit1 ?? 0));
                                    endingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.DebitCur2 ?? 0) - (p.CreditCur2 ?? 0));
                                    endingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.Debit2 ?? 0) - (p.Credit2 ?? 0));
                                }
                                else if (item.DebitOrCredit == "C")
                                {
                                    openingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.CreditCur1 ?? 0) - (p.DebitCur1 ?? 0));
                                    openingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.Credit1 ?? 0) - (p.Debit1 ?? 0));
                                    endingAmountCur = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.CreditCur2 ?? 0) - (p.DebitCur2 ?? 0));
                                    endingAmount = balanceSheetAcc.Where(p => (p.AccCode ?? "").StartsWith(item.Acc)).Sum(p => (p.Credit2 ?? 0) - (p.Debit2 ?? 0));
                                }
                                if (item.Type == "U")
                                {
                                    openingAmountCur = openingAmountCur < 0 ? 0 : openingAmountCur;
                                    openingAmount = openingAmount < 0 ? 0 : openingAmount;
                                    endingAmountCur = endingAmountCur < 0 ? 0 : endingAmountCur;
                                    endingAmount = endingAmount < 0 ? 0 : endingAmount;
                                }
                            }
                        }
                    }
                    else
                    {
                        var dataacc = dataAccBalanceSheet.Where(p => item.Formular.Contains(p.NumberCode) && p.NumberCode != "").ToList();
                        var lstFormular = GetFormular(item.Formular);
                        var data = (from a in lstFormular
                                    join b in dataAccBalanceSheet on a.Code equals b.NumberCode into ajb
                                    from b in ajb.DefaultIfEmpty()
                                    select new
                                    {
                                        OpeningAmountCur = (a.Math == "+") ? b?.OpeningAmountCur ?? 0 : -1 * b?.OpeningAmountCur ?? 0,
                                        OpeningAmount = (a.Math == "+") ? b?.OpeningAmount ?? 0 : -1 * b?.OpeningAmount ?? 0,
                                        EndingAmountCur = (a.Math == "+") ? b?.EndingAmountCur ?? 0 : -1 * b?.EndingAmountCur ?? 0,
                                        EndingAmount = (a.Math == "+") ? b?.EndingAmount ?? 0 : -1 * b?.EndingAmount ?? 0,
                                    }).ToList();
                        openingAmountCur = data.Sum(p => p.OpeningAmountCur);
                        openingAmount = data.Sum(p => p.OpeningAmount);
                        endingAmountCur = data.Sum(p => p.EndingAmountCur);
                        endingAmount = data.Sum(p => p.EndingAmount);
                    }
                    item.OpeningAmountCur = openingAmountCur;
                    item.OpeningAmount = openingAmount;
                    item.EndingAmountCur = endingAmountCur;
                    item.EndingAmount = endingAmount;
                }
                lst.AddRange(dataAccBalanceSheetByRank.OrderBy(p => p.Ord).Select(p => _objectMapper.Map<AccBalanceSheet, AccBalanceSheetDto>(p)).ToList());
                rank--;
            }
            //if (check != "K")
            //{
            //    lst = balanceSheetAcc.Where(p => (p.AccCode ?? "") != "" && p.Tag != 3
            //                             && (Math.Abs(p.Debit1 ?? 0) + Math.Abs(p.Credit1 ?? 0) + Math.Abs(p.Debit2 ?? 0) + Math.Abs(p.Credit2 ?? 0) != 0))
            //                         .OrderBy(p => p.AccCode).Select(p => new AccBalanceSheetDto 
            //                                                                 { 
            //                                                                    Acc = p.AccCode,
            //                                                                 }).ToList();
            //}
            var reportResponse = new ReportResponseDto<AccBalanceSheetDto>();
            reportResponse.Data = lst.OrderBy(p => p.Ord).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        #region Private
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
        private async Task<List<AccBalanceSheetDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.GroupBy(g => new
            {
                g.OrgCode,
                g.Year,
                g.VoucherCode,
                g.VoucherDate,
                g.VoucherNumber,
                g.VoucherGroup,
                g.VoucherId,
                g.Note,
                g.PartnerCode,
                g.FProductWorkCode,
                g.Representative,
                g.ReciprocalAcc,
                g.Acc,
                g.CurrencyCode,
                g.ExchangeRate,
                g.InvoicePartnerName,
                g.InvoiceNumber,
                g.InvoiceDate,
                g.ContractCode,
            }).Select(p => new AccBalanceSheetDto()
            {
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                VoucherCode = p.Key.VoucherCode,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                VoucherId = p.Key.VoucherId,
                Note = p.Key.Note,
                PartnerCode = p.Key.PartnerCode,
                FProductWorkCode = p.Key.FProductWorkCode,
                Representative = p.Key.Representative,
                ReciprocalAcc = p.Key.ReciprocalAcc,
                Acc = p.Key.Acc,
                CurrencyCode = p.Key.CurrencyCode,
                ContractCode = p.Key.ContractCode,
                DebitIncurredCur = p.Sum(g => g.DebitIncurredCur),
                DebitIncurred = p.Sum(g => g.DebitIncurred),
                CreditIncurredCur = p.Sum(g => g.CreditIncurredCur),
                CreditIncurred = p.Sum(g => g.CreditIncurred)
            }).OrderBy(p => p.PartnerCode)
                .ThenBy(p => p.VoucherDate)
                .ThenBy(p => p.VoucherNumber)
                .ThenBy(p => p.ReciprocalAcc)
                .ThenBy(p => p.VoucherId).ToList();
            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private Dictionary<string, object> GetLedgerParameter(AccBalanceSheetParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, "");
            return dic;
        }
        private async Task<List<AccOpeningBalanceReportDto>> GetOpeningBalance(AccBalanceSheetParameterDto dto,
                                List<AccountSystemDto> accountSystems)
        {
            var dic = this.GetLedgerParameter(dto);

            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, yearCategory.Year);
            }

            dic[LedgerParameterConst.Year] = yearCategory.Year;
            dic[LedgerParameterConst.FromDate] = yearCategory.BeginDate;
            dic[LedgerParameterConst.ToDate] = fromDate.AddDays(-1);

            var dtos = await _reportDataService.GetAccBalancesAsync(dic);
            var balances = (from a in dtos
                            join b in accountSystems on a.AccCode equals b.AccCode into ajb
                            from b in ajb.DefaultIfEmpty()
                            select new AccOpeningBalanceReportDto
                            {
                                OrgCode = orgCode,
                                Year = _webHelper.GetCurrentYear(),
                                AccCode = a.AccCode,
                                PartnerCode = a.PartnerCode,
                                FProductWorkCode = a.FProductCode,
                                WorkPlaceCode = a.WorkPlaceCode,
                                SectionCode = a.SectionCode,
                                CurrencyCode = a.CurrencyCode,
                                ContractCode = a.ContractCode,
                                DebitCur1 = a.DebitCur ?? 0,
                                Debit1 = a.Debit ?? 0,
                                CreditCur1 = a.CreditCur ?? 0,
                                Credit1 = a.Credit ?? 0,
                                DebitIncurredCur = 0,
                                DebitIncurred = 0,
                                CreditIncurredCur = 0,
                                CreditIncurred = 0,
                                DebitAccumulationCur = 0,
                                DebitAccumulation = 0,
                                CreditAccumulationCur = 0,
                                CreditAccumulation = 0,
                                AttachPartner = b?.AttachPartner ?? "",
                                AttachAccSection = b?.AttachAccSection ?? "",
                                AttachContract = b?.AttachContract ?? "",
                                AttachWorkPlace = b?.AttachWorkPlace ?? "",
                                AttachProductCost = b?.AttachProductCost ?? "",
                                IsBalanceSheetAcc = b?.IsBalanceSheetAcc ?? "",
                                AttachCurrency = b?.AttachCurrency ?? "",
                                AccPattern = b?.AccPattern ?? null,
                            }).ToList();
            return balances;
        }
        private async Task<List<AccountSystemDto>> GetAccountSystems(AccBalanceSheetParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, dto.FromDate.Value);
            return await _accountingCacheManager.GetAccountSystemsAsync(yearCategory.Year);
        }
        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return _objectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
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
            return _objectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        #endregion
    }
}
