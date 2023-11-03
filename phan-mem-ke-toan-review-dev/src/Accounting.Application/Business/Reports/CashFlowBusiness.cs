using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Common.Extensions;
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
    public class CashFlowBusiness : BaseBusiness, IUnitOfWorkEnabled
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
        private readonly DefaultCashFollowStatementService _defaultCashFollowStatementService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly TenantCashFollowStatementService _tenantCashFollowStatementService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly IObjectMapper _objectMapper;

        #endregion
        public CashFlowBusiness(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                        DefaultCashFollowStatementService defaultCashFollowStatementService,
                        TenantAccBalanceSheetService tenantAccBalanceSheetService,
                        TenantCashFollowStatementService tenantCashFollowStatementService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
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
            _defaultCashFollowStatementService = defaultCashFollowStatementService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _tenantCashFollowStatementService = tenantCashFollowStatementService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _objectMapper = objectMapper;
        }
        #region Methods
        public async Task<ReportResponseDto<CashFlowDto>> CreateDataAsync(ReportRequestDto<CashFlowBResultParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lst = new List<CashFlowDto>();
            var yearThis = dto.Parameters.FromDate.Value.Year;
            var yearLast = dto.Parameters.FromDateLast.Value.Year;
            var yearCategory = (await _yearCategoryService.GetQueryableAsync()).Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).FirstOrDefault();
            int? usingDecision = (dto.Parameters.UsingDecision == null) ? yearCategory?.UsingDecision ?? 200 : dto.Parameters.UsingDecision;
            var tenantCashFollowStatement = await _tenantCashFollowStatementService.GetAllAsync(_webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            var defaulCashFollowStatement = await _defaultCashFollowStatementService.GetAllAsync();
            var beginDateThis = new DateTime(yearThis, 1, 1);
            var beginDateLast = new DateTime(yearLast, 1, 1);
            var thisParameters = new CashFlowBResultParameterDto
            {
                FromDate = beginDateThis,
                ToDate = dto.Parameters.ToDate,
                LstOrgCode = dto.Parameters.LstOrgCode,
                IsSummary = dto.Parameters.IsSummary,
                UsingDecision = dto.Parameters.UsingDecision,
            };
            var lastParameters = new CashFlowBResultParameterDto
            {
                FromDate = beginDateLast,
                ToDate = dto.Parameters.ToDateLast,
                LstOrgCode = dto.Parameters.LstOrgCode,
                IsSummary = dto.Parameters.IsSummary,
                UsingDecision = dto.Parameters.UsingDecision,
            };
            var dicThis = GetLedgerParameter(thisParameters);
            var dicLast = GetLedgerParameter(lastParameters);
            var thisIncurredData = await GetIncurredData(dicThis);
            var lastIncurredData = await GetIncurredData(dicLast);
            var incurredData = lastIncurredData.GroupBy(g => new
            {
                g.DebitAcc,
                g.CreditAcc,
                g.SectionCode
            }).Select(p => new CashFlowDto()
            {
                DebitAcc = p.Key.DebitAcc,
                CreditAcc = p.Key.CreditAcc,
                SectionCode0 = p.Key.SectionCode,
                AccumulatedLastPeriod = p.Sum(p => p.Amount0),
                AccumulatedLastPeriodCur = p.Sum(p => p.AmountCur0),
                LastPeriod = p.Sum(p => (dto.Parameters.FromDateLast <= p.VoucherDate && dto.Parameters.ToDateLast >= p.VoucherDate) ? p.Amount0 : 0),
                LastPeriodCur = p.Sum(p => (dto.Parameters.FromDateLast <= p.VoucherDate && dto.Parameters.ToDateLast >= p.VoucherDate) ? p.AmountCur0 : 0),
                ThisPeriod = 0,
                ThisPeriodCur = 0,
                AccumulatedThisPeriod = 0,
                AccumulatedThisPeriodCur = 0,
            }).ToList();
            incurredData.AddRange(thisIncurredData.GroupBy(g => new
            {
                g.DebitAcc,
                g.CreditAcc,
                g.SectionCode
            }).Select(p => new CashFlowDto()
            {
                DebitAcc = p.Key.DebitAcc,
                CreditAcc = p.Key.CreditAcc,
                SectionCode0 = p.Key.SectionCode,
                AccumulatedLastPeriod = 0,
                AccumulatedLastPeriodCur = 0,
                LastPeriod = 0,
                LastPeriodCur = 0,
                AccumulatedThisPeriod = p.Sum(p => p.Amount0),
                AccumulatedThisPeriodCur = p.Sum(p => p.AmountCur0),
                ThisPeriod = p.Sum(p => (dto.Parameters.FromDate <= p.VoucherDate && dto.Parameters.ToDate >= p.VoucherDate) ? p.Amount0 : 0),
                ThisPeriodCur = p.Sum(p => (dto.Parameters.FromDate <= p.VoucherDate && dto.Parameters.ToDate >= p.VoucherDate) ? p.AmountCur0 : 0),
            }).ToList());
            // LẤY DỮ LIỆU KHAI BÁO	
            var dataCashFlow = (tenantCashFollowStatement.Count > 0) ?
                                      tenantCashFollowStatement.OrderBy(p => p.Ord).Where(p => p.UsingDecision == usingDecision).Select(p => _objectMapper.Map<TenantCashFollowStatement, CashFollowStatement>(p)).ToList() :
                                      defaulCashFollowStatement.OrderBy(p => p.Ord).Where(p => p.UsingDecision == usingDecision).Select(p => _objectMapper.Map<DefaultCashFollowStatement, CashFollowStatement>(p)).ToList();
            var rank = dataCashFlow.Max(p => p.Rank);
            while (rank > 0)
            {
                var dataCashFlowByRank = dataCashFlow.Where(p => p.Rank == rank).ToList();
                foreach (var item in dataCashFlowByRank)
                {
                    decimal accumulatedLastPeriod = 0,
                            accumulatedLastPeriodCur = 0,
                            lastPeriod = 0,
                            lastPeriodCur = 0,
                            accumulatedThisPeriod = 0,
                            accumulatedThisPeriodCur = 0,
                            thisPeriod = 0,
                            thisPeriodCur = 0;

                    if ((item.Formular ?? "") == "")
                    {
                        if (item.Method == "incurred")
                        {
                            if ((item.DebitAcc ?? "") != "" || (item.CreditAcc ?? "") != "")
                            {
                                var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                                var lstCreditAcc = GetSplit(item.CreditAcc, ',');
                                var lstSectionCode = GetSplit(item.SectionCode, ',');
                                var dataJoin = (from a in incurredData
                                                join b in lstDebitAcc on 0 equals 0
                                                where (a.DebitAcc ?? "").StartsWith(b.Data)
                                                join c in lstCreditAcc on 0 equals 0
                                                where (a.CreditAcc ?? "").StartsWith(c.Data)
                                                join d in lstSectionCode on a.SectionCode equals d.Data into ajd
                                                from d in ajd.DefaultIfEmpty()
                                                where (item.SectionCode ?? "") == "" || (d?.Data ?? "") != ""
                                                select new
                                                {
                                                    AccumulatedLastPeriod = a.AccumulatedLastPeriod,
                                                    AccumulatedLastPeriodCur = a.AccumulatedLastPeriodCur,
                                                    LastPeriod = a.LastPeriod,
                                                    LastPeriodCur = a.LastPeriodCur,
                                                    AccumulatedThisPeriod = a.AccumulatedThisPeriod,
                                                    AccumulatedThisPeriodCur = a.AccumulatedThisPeriodCur,
                                                    ThisPeriod = a.ThisPeriod,
                                                    ThisPeriodCur = a.ThisPeriodCur,
                                                }).ToList();
                                accumulatedLastPeriod = dataJoin.Sum(p => p.AccumulatedLastPeriod ?? 0);
                                accumulatedLastPeriodCur = dataJoin.Sum(p => p.AccumulatedLastPeriodCur ?? 0);
                                lastPeriod = dataJoin.Sum(p => p.LastPeriod ?? 0);
                                lastPeriodCur = dataJoin.Sum(p => p.LastPeriodCur ?? 0);
                                thisPeriod = dataJoin.Sum(p => p.ThisPeriod ?? 0);
                                thisPeriodCur = dataJoin.Sum(p => p.ThisPeriodCur ?? 0);
                                accumulatedThisPeriod = dataJoin.Sum(p => p.AccumulatedThisPeriod ?? 0);
                                accumulatedThisPeriodCur = dataJoin.Sum(p => p.AccumulatedThisPeriodCur ?? 0);
                            }
                        }
                        else if (item.Method == "beginningBalance")
                        {
                            if ((item.DebitAcc ?? "") != "")
                            {
                                lastPeriod = 0;
                                lastPeriodCur = 0;
                                thisPeriod = 0;
                                thisPeriodCur = 0;
                                var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                                foreach (var itemDebitAcc in lstDebitAcc)
                                {
                                    // kỳ trước
                                    dicLast[LedgerParameterConst.ToDate] = beginDateLast;
                                    dicLast[LedgerParameterConst.Acc] = itemDebitAcc.Data;
                                    dicLast[LedgerParameterConst.DebitOrCredit] = "*";
                                    var openingBalanceLast = await GetOpeningBalance(dicLast);
                                    lastPeriod += (openingBalanceLast.Debit ?? 0) - (openingBalanceLast.Credit ?? 0);
                                    lastPeriodCur += (openingBalanceLast.DebitCur ?? 0) - (openingBalanceLast.DebitCur ?? 0);
                                    // kỳ này
                                    dicThis[LedgerParameterConst.ToDate] = beginDateThis;
                                    dicThis[LedgerParameterConst.Acc] = itemDebitAcc.Data;
                                    dicThis[LedgerParameterConst.DebitOrCredit] = "*";
                                    var openingBalanceThis = await GetOpeningBalance(dicThis);
                                    thisPeriod += (openingBalanceThis.Debit ?? 0) - (openingBalanceThis.Credit ?? 0);
                                    thisPeriodCur += (openingBalanceThis.DebitCur ?? 0) - (openingBalanceThis.DebitCur ?? 0);
                                }
                            }
                        }
                        else if (item.Method == "endBalance")
                        {
                            if ((item.DebitAcc ?? "") != "")
                            {
                                lastPeriod = 0;
                                lastPeriodCur = 0;
                                thisPeriod = 0;
                                thisPeriodCur = 0;
                                var lstDebitAcc = GetSplit(item.DebitAcc, ',');
                                foreach (var itemDebitAcc in lstDebitAcc)
                                {
                                    // kỳ trước
                                    dicLast[LedgerParameterConst.FromDate] = beginDateLast;
                                    dicLast[LedgerParameterConst.ToDate] = dto.Parameters.ToDateLast.Value.AddDays(-1);
                                    dicLast[LedgerParameterConst.Acc] = itemDebitAcc.Data;
                                    dicLast[LedgerParameterConst.DebitOrCredit] = "*";
                                    var openingBalanceLast = await GetAccBalance(dicLast);
                                    lastPeriod += (openingBalanceLast.Debit ?? 0) - (openingBalanceLast.Credit ?? 0);
                                    lastPeriodCur += (openingBalanceLast.DebitCur ?? 0) - (openingBalanceLast.DebitCur ?? 0);
                                    // kỳ này
                                    dicThis[LedgerParameterConst.FromDate] = beginDateThis;
                                    dicThis[LedgerParameterConst.ToDate] = dto.Parameters.ToDate.Value.AddDays(-1);
                                    dicThis[LedgerParameterConst.Acc] = itemDebitAcc.Data;
                                    dicThis[LedgerParameterConst.DebitOrCredit] = "*";
                                    var openingBalanceThis = await GetAccBalance(dicThis);
                                    thisPeriod += (openingBalanceThis.Debit ?? 0) - (openingBalanceThis.Credit ?? 0);
                                    thisPeriodCur += (openingBalanceThis.DebitCur ?? 0) - (openingBalanceThis.DebitCur ?? 0);
                                }
                            }
                        }
                    }
                    else
                    {
                        var lstFormular = GetFormular(item.Formular);
                        var dataJoin = (from a in lstFormular
                                        join b in dataCashFlow on a.Code equals b.NumberCode into ajb
                                        from b in ajb.DefaultIfEmpty()
                                        select new
                                        {
                                            AccumulatedLastPeriod = (a.Math == "+") ? b?.AccumulatedLastPeriod ?? 0 : -1 * b?.AccumulatedLastPeriod ?? 0,
                                            LastPeriod = (a.Math == "+") ? b?.LastPeriod ?? 0 : -1 * b?.LastPeriod ?? 0,
                                            AccumulatedThisPeriod = (a.Math == "+") ? b?.AccumulatedThisPeriod ?? 0 : -1 * b?.AccumulatedThisPeriod ?? 0,
                                            ThisPeriod = (a.Math == "+") ? b?.ThisPeriod ?? 0 : -1 * b?.ThisPeriod ?? 0,
                                        }).ToList();
                        accumulatedLastPeriod = dataJoin.Sum(p => p.AccumulatedLastPeriod);
                        lastPeriod = dataJoin.Sum(p => p.LastPeriod);
                        accumulatedThisPeriod = dataJoin.Sum(p => p.AccumulatedThisPeriod);
                        thisPeriod = dataJoin.Sum(p => p.ThisPeriod);
                    }
                    item.AccumulatedLastPeriod = accumulatedLastPeriod.GetDefaultNullIfZero();
                    item.LastPeriod = lastPeriod.GetDefaultNullIfZero();
                    item.AccumulatedThisPeriod = accumulatedThisPeriod.GetDefaultNullIfZero();
                    item.ThisPeriod = thisPeriod.GetDefaultNullIfZero();
                }
                lst.AddRange(dataCashFlowByRank.OrderBy(p => p.Ord).Select(p => _objectMapper.Map<CashFollowStatement, CashFlowDto>(p)).ToList());
                rank--;
            }
            var reportResponse = new ReportResponseDto<CashFlowDto>();
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
        private List<FormularDto> GetFormular(string formular) // lấy list công thức
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

        private List<LstAccDto> GetSplit(string str, char spt) // lấy list
        {
            var lst = new List<LstAccDto>();
            var lstAcc = str.Split(spt).ToList();
            for (var i = 0; i < lstAcc.Count; i++)
            {
                lst.Add(new LstAccDto
                {
                    Id = i + 1,
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
            DateTime ToDate = Convert.ToDateTime(dic[LedgerParameterConst.ToDate]);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, ToDate.Year);
            }
            dic[LedgerParameterConst.Year] = ToDate.Year;
            dic[LedgerParameterConst.ToDate] = fromDate.AddDays(-1);

            var openingBalances = await _reportDataService.GetAccBalancesAsync(dic);
            var balances = new AccountBalanceDto()
            {
                Debit = openingBalances.Sum(p => p.Debit),
                Credit = openingBalances.Sum(p => p.Credit),
                DebitCur = openingBalances.Sum(p => p.DebitCur),
                CreditCur = openingBalances.Sum(p => p.CreditCur)
            };
            return balances;
        }

        private async Task<AccountBalanceDto> GetAccBalance(Dictionary<string, object> dic)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            DateTime ToDate = Convert.ToDateTime(dic[LedgerParameterConst.ToDate]);
            if (!dic.ContainsKey(LedgerParameterConst.Year))
            {
                dic.Add(LedgerParameterConst.Year, ToDate.Year);
            }
            dic[LedgerParameterConst.Year] = ToDate.Year;

            var openingBalances = await _reportDataService.GetAccBalancesAsync(dic);
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
