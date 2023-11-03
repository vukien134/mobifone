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
using Accounting.Reports.DebitBooks;
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
    public class BalanceSheetAccBusiness : BaseBusiness, IUnitOfWorkEnabled
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
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion
        public BalanceSheetAccBusiness(ReportDataService reportDataService,
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
                        AccountingCacheManager accountingCacheManager,
                        IStringLocalizer<AccountingResource> localizer
            ) : base(localizer)
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
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        public async Task<ReportResponseDto<BalanceSheetAccDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var accountingSystems = await this.GetAccountSystems(dto.Parameters);
            var balanceSheet = await this.GetBalaceSheet(dto.Parameters, accountingSystems);
            var treeAcc = this.GetAccTreeAsync(dto.Parameters, accountingSystems);
            var data = this.GetTreeBalanceSheet(balanceSheet, treeAcc,
                                            dto.Parameters.AccRank.Value, dto.Parameters.AccCode);
            var total = data.Where(p => p.Rank == 1)
                            .GroupBy(g => g.OrgCode)
                            .Select(p => new BalanceSheetAccDto()
                            {
                                Bold = "C",
                                AccNameTemp = "Tổng cộng",
                                Debit1 = p.Sum(s => s.Debit1),
                                DebitCur1 = p.Sum(s => s.DebitCur1),
                                Credit1 = p.Sum(s => s.Credit1),
                                CreditCur1 = p.Sum(s => s.CreditCur1),
                                Debit2 = p.Sum(s => s.Debit2),
                                DebitCur2 = p.Sum(s => s.DebitCur2),
                                Credit2 = p.Sum(s => s.Credit2),
                                CreditCur2 = p.Sum(s => s.CreditCur2),
                                DebitIncurred = p.Sum(s => s.DebitIncurred),
                                CreditIncurred = p.Sum(s => s.CreditIncurred),
                                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
                            }).FirstOrDefault();
            if (total != null)
            {
                data.Add(total);
            }

            var reportResponse = await this.CreateReportResponseDto<BalanceSheetAccDto>(data, dto);
            return reportResponse;
        }
        #endregion
        #region Privates
        private List<BalanceSheetAccDto> GetTreeBalanceSheet(List<BalanceSheetAccDto> balanceSheets,
                                        List<AccountSystemTreeItemDto> treeAcc, int rank, string acc)
        {
            int? maxRank = treeAcc.Max(p => p.Rank);

            var treeBalanceSheet = (from c in treeAcc
                                    join d in balanceSheets on c.Code equals d.AccCode into gj
                                    from df in gj.DefaultIfEmpty()
                                    orderby c.SortPath
                                    select new BalanceSheetAccDto()
                                    {
                                        Id = c.Id,
                                        ParentId = c.ParentId,
                                        AccCode = c.Code,
                                        AccNameTemp = c.Rank > 1 ? new string('-', (c.Rank.Value - 1) * 2) + c.Name : c.Name,
                                        AccName = c.Name,
                                        AccNameTempE = c.Name,
                                        AccType = df?.AccType ?? string.Empty,
                                        AccumulationCredit = df?.AccumulationCredit ?? null,
                                        AccumulationCreditCur = df?.AccumulationCreditCur ?? null,
                                        AccumulationDebit = df?.AccumulationDebit ?? null,
                                        AccumulationDebitCur = df?.AccumulationDebitCur ?? null,
                                        Bold = c.HasChild == true ? "C" : null,
                                        ContractCode = df?.ContractCode ?? null,
                                        Credit1 = df?.Credit1 ?? null,
                                        Credit2 = df?.Credit2 ?? null,
                                        CreditCur1 = df?.CreditCur1 ?? null,
                                        CreditCur2 = df?.CreditCur2 ?? null,
                                        CreditIncurred = df?.CreditIncurred ?? null,
                                        CreditIncurredCur = df?.CreditIncurredCur ?? null,
                                        CurrencyCode = df?.CurrencyCode ?? string.Empty,
                                        Debit1 = df?.Debit1 ?? null,
                                        Debit2 = df?.Debit2 ?? null,
                                        DebitCur1 = df?.DebitCur1 ?? null,
                                        DebitCur2 = df?.DebitCur2 ?? null,
                                        DebitIncurred = df?.DebitIncurred ?? null,
                                        DebitIncurredCur = df?.DebitIncurredCur ?? null,
                                        FProductWorkCode = df?.FProductWorkCode ?? null,
                                        PartnerCode = df?.PartnerCode ?? null,
                                        SectionCode = df?.SectionCode ?? null,
                                        SortPath = c.SortPath,
                                        Rank = c.Rank,
                                        AttachAccSection = df?.AttachAccSection ?? null,
                                        AttachContract = df?.AttachContract ?? null,
                                        AttachCurrency = df?.AttachCurrency ?? null,
                                        AttachPartner = df?.AttachPartner ?? null,
                                        AttachProductCost = df?.AttachProductCost ?? null,
                                        AttachWorkPlace = df?.AttachWorkPlace ?? null
                                    }).ToList();
            while (maxRank > 1)
            {
                var groupData = treeBalanceSheet.Where(p => p.Rank == maxRank)
                                            .GroupBy(g => new
                                            {
                                                g.ParentId
                                            }).Select(p => new BalanceSheetAccDto()
                                            {
                                                Id = p.Key.ParentId,
                                                Credit1 = p.Sum(s => s.Credit1),
                                                CreditCur1 = p.Sum(s => s.CreditCur1),
                                                Debit1 = p.Sum(s => s.Debit1),
                                                DebitCur1 = p.Sum(s => s.DebitCur1),
                                                CreditIncurred = p.Sum(s => s.CreditIncurred),
                                                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur),
                                                DebitIncurred = p.Sum(s => s.DebitIncurred),
                                                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                                                Credit2 = p.Sum(s => s.Credit2),
                                                CreditCur2 = p.Sum(s => s.CreditCur2),
                                                Debit2 = p.Sum(s => s.Debit2),
                                                DebitCur2 = p.Sum(s => s.DebitCur2)
                                            }).ToList();

                foreach (var item in groupData)
                {
                    var treeItem = treeBalanceSheet.Where(p => p.Id == item.Id).FirstOrDefault();
                    if (treeItem == null) continue;

                    treeItem.Credit1 = item.Credit1;
                    treeItem.CreditCur1 = item.CreditCur1;
                    treeItem.Debit1 = item.Debit1;
                    treeItem.DebitCur1 = item.DebitCur1;
                    treeItem.CreditIncurred = item.CreditIncurred;
                    treeItem.CreditIncurredCur = item.CreditIncurredCur;
                    treeItem.DebitIncurred = item.DebitIncurred;
                    treeItem.DebitIncurredCur = item.DebitIncurredCur;
                    treeItem.Credit2 = item.Credit2;
                    treeItem.CreditCur2 = item.CreditCur2;
                    treeItem.Debit2 = item.Debit2;
                    treeItem.DebitCur2 = item.DebitCur2;
                }

                maxRank--;
            }
            var queryable = treeBalanceSheet.AsQueryable();
            queryable = queryable.Where(p => p.Rank <= rank
                && (this.DefaultZeroIfNull(p.Debit2) != 0 || this.DefaultZeroIfNull(p.Credit2) != 0
                    || this.DefaultZeroIfNull(p.DebitCur2) != 0 || this.DefaultZeroIfNull(p.CreditCur2) != 0
                    || this.DefaultZeroIfNull(p.DebitIncurred) != 0 || this.DefaultZeroIfNull(p.DebitIncurredCur) != 0
                    || this.DefaultZeroIfNull(p.CreditIncurred) != 0 || this.DefaultZeroIfNull(p.CreditIncurredCur) != 0));
            if (!string.IsNullOrEmpty(acc))
            {
                var dto = treeBalanceSheet.Where(p => p.AccCode.Equals(acc)).FirstOrDefault();
                if (dto != null)
                {
                    string[] parts = dto.SortPath.Split('|');
                    int index = Array.IndexOf(parts, acc);
                    string[] filterParts = parts.Take(index + 1).ToArray();
                    queryable = queryable.Where(p => filterParts.Contains(p.AccCode));
                }
            }
            return queryable.ToList();
        }
        private async Task<List<AccountSystemDto>> GetAccountSystems(ReportBaseParameterDto dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, dto.FromDate.Value);
            return await _accountingCacheManager.GetAccountSystemsAsync(yearCategory.Year);
        }
        private async Task<List<BalanceSheetAccDto>> GetBalaceSheet(ReportBaseParameterDto dto,
                            List<AccountSystemDto> accountSystems)
        {
            var balanceSheet = new List<BalanceSheetAccDto>();

            var openings = await this.GetOpeningBalance(dto, accountSystems);
            balanceSheet.AddRange(openings);
            var incurreds = await this.GetIncurredData(dto, accountSystems);
            balanceSheet.AddRange(incurreds);

            var groupData = balanceSheet.AsParallel().GroupBy(g => new
            {
                g.AccCode,
                g.ContractCode,
                g.CurrencyCode,
                g.FProductWorkCode,
                g.PartnerCode,
                g.SectionCode,
                g.WorkPlaceCode,
                g.AttachAccSection,
                g.AttachContract,
                g.AttachCurrency,
                g.AttachPartner,
                g.AttachProductCost,
                g.AttachWorkPlace
            }).Select(p => new BalanceSheetAccDto()
            {
                AccCode = p.Key.AccCode,
                ContractCode = p.Key.ContractCode,
                CurrencyCode = p.Key.CurrencyCode,
                FProductWorkCode = p.Key.FProductWorkCode,
                PartnerCode = p.Key.PartnerCode,
                SectionCode = p.Key.SectionCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                AttachAccSection = p.Key.AttachAccSection,
                AttachContract = p.Key.AttachContract,
                AttachCurrency = p.Key.AttachCurrency,
                AttachPartner = p.Key.AttachPartner,
                AttachProductCost = p.Key.AttachProductCost,
                AttachWorkPlace = p.Key.AttachWorkPlace,
                Credit1 = p.Sum(s => s.Credit1),
                CreditCur1 = p.Sum(s => s.CreditCur1),
                Debit1 = p.Sum(s => s.Debit1),
                DebitCur1 = p.Sum(s => s.DebitCur1),
                Credit2 = p.Sum(s => s.Credit2),
                CreditCur2 = p.Sum(s => s.CreditCur2),
                Debit2 = p.Sum(s => s.Debit2),
                DebitCur2 = p.Sum(s => s.DebitCur2),
                DebitIncurred = p.Sum(s => s.DebitIncurred),
                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                CreditIncurred = p.Sum(s => s.CreditIncurred),
                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
            }).ToList();
            groupData = this.ProcessBalanceValue(groupData);

            return groupData.AsParallel().GroupBy(g => new
            {
                g.AccCode,
                g.AttachAccSection,
                g.AttachContract,
                g.AttachCurrency,
                g.AttachPartner,
                g.AttachProductCost,
                g.AttachWorkPlace

            }).Select(p => new BalanceSheetAccDto()
            {
                AccCode = p.Key.AccCode,
                AttachAccSection = p.Key.AttachAccSection,
                AttachContract = p.Key.AttachContract,
                AttachCurrency = p.Key.AttachCurrency,
                AttachPartner = p.Key.AttachPartner,
                AttachProductCost = p.Key.AttachProductCost,
                AttachWorkPlace = p.Key.AttachWorkPlace,
                Credit1 = p.Sum(s => s.Credit1),
                CreditCur1 = p.Sum(s => s.CreditCur1),
                Debit1 = p.Sum(s => s.Debit1),
                DebitCur1 = p.Sum(s => s.DebitCur1),
                Credit2 = p.Sum(s => s.Credit2),
                CreditCur2 = p.Sum(s => s.CreditCur2),
                Debit2 = p.Sum(s => s.Debit2),
                DebitCur2 = p.Sum(s => s.DebitCur2),
                DebitIncurred = p.Sum(s => s.DebitIncurred),
                DebitIncurredCur = p.Sum(s => s.DebitIncurredCur),
                CreditIncurred = p.Sum(s => s.CreditIncurred),
                CreditIncurredCur = p.Sum(s => s.CreditIncurredCur)
            }).ToList();

        }
        private async Task<List<BalanceSheetAccDto>> GetOpeningBalance(ReportBaseParameterDto dto,
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
            var query = from p in dtos
                        join d in accountSystems on p.AccCode equals d.AccCode into gj
                        from df in gj.DefaultIfEmpty()
                        select new BalanceSheetAccDto()
                        {
                            AccCode = p.AccCode,
                            ContractCode = null,
                            CurrencyCode = null,
                            FProductWorkCode = dto.CleaningAcc == true || dto.CleaningFProductWork == true
                                               || !this.IsAttachFProductWork(df) ? null : this.DefaultNullIfEmpty(p.FProductCode),
                            PartnerCode = dto.CleaningAcc == true || !this.IsAttachPartner(df) ? null : this.DefaultNullIfEmpty(p.PartnerCode),
                            SectionCode = null,

                            WorkPlaceCode = null,
                            Credit1 = p.Credit,
                            CreditCur1 = p.CreditCur,
                            Debit1 = p.Debit,
                            DebitCur1 = p.DebitCur,
                            Credit2 = p.Credit,
                            CreditCur2 = p.CreditCur,
                            Debit2 = p.Debit,
                            DebitCur2 = p.DebitCur,
                            AttachAccSection = this.IsAttachSection(df) == true ? "C" : null,
                            AttachContract = this.IsAttachContract(df) == true ? "C" : null,
                            AttachCurrency = this.IsAttachCurrency(df) == true ? "C" : null,
                            AttachPartner = this.IsAttachPartner(df) == true ? "C" : null,
                            AttachProductCost = this.IsAttachFProductWork(df) == true ? "C" : null,
                            AttachWorkPlace = this.IsAttachWorkPlace(df) == true ? "C" : null
                        };

            return query.ToList();
        }
        private async Task<List<BalanceSheetAccDto>> GetIncurredData(ReportBaseParameterDto dto,
                    List<AccountSystemDto> accountSystems)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = this.GetLedgerParameter(dto);
            DateTime fromDate = Convert.ToDateTime(dic[LedgerParameterConst.FromDate]);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, fromDate);
            var dtos = await _reportDataService.GetAccBalancesAsync(dic);
            var query = from p in dtos
                        join d in accountSystems on p.AccCode equals d.AccCode into gj
                        from df in gj.DefaultIfEmpty()
                        select new BalanceSheetAccDto()
                        {
                            AccCode = p.AccCode,
                            ContractCode = null,
                            CurrencyCode = null,
                            FProductWorkCode = dto.CleaningAcc == true || dto.CleaningFProductWork == true
                                              || !this.IsAttachFProductWork(df) ? null : this.DefaultNullIfEmpty(p.FProductCode),
                            PartnerCode = dto.CleaningAcc == true || !this.IsAttachPartner(df) ? null : this.DefaultNullIfEmpty(p.PartnerCode),
                            SectionCode = null,
                            WorkPlaceCode = null ,
                            CreditIncurred = p.CreditIncurred,
                            CreditIncurredCur = p.CreditIncurredCur,
                            DebitIncurred = p.DebitIncurred,
                            DebitIncurredCur = p.DebitIncurredCur,
                            Credit2 = p.CreditIncurred,
                            CreditCur2 = p.CreditIncurredCur,
                            Debit2 = p.DebitIncurred,
                            DebitCur2 = p.DebitIncurredCur,
                            AttachAccSection = this.IsAttachSection(df) == true ? "C" : null,
                            AttachContract = this.IsAttachContract(df) == true ? "C" : null,
                            AttachCurrency = this.IsAttachCurrency(df) == true ? "C" : null,
                            AttachPartner = this.IsAttachPartner(df) == true ? "C" : null,
                            AttachProductCost = this.IsAttachFProductWork(df) == true ? "C" : null,
                            AttachWorkPlace = this.IsAttachWorkPlace(df) == true ? "C" : null
                        };

            return query.ToList();
        }
        private Dictionary<string, object> GetLedgerParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            return dic;
        }
        private List<AccountSystemTreeItemDto> GetAccTreeAsync(ReportBaseParameterDto dto,
                                    List<AccountSystemDto> accountingSystems)
        {
            var tree = new List<AccountSystemTreeItemDto>();
            BuildTreeView(accountingSystems, "", tree, null, null);
            return tree;
        }
        private int BuildTreeView(List<AccountSystemDto> partnerGroups, string parentId, List<AccountSystemTreeItemDto> tree, int? rank, string sortPath)
        {
            var groups = partnerGroups.Where(p => p.ParentAccId == parentId)
                                .OrderBy(p => p.AccCode).ToList();
            if (groups.Count == 0) return 0;

            int count = 0;
            foreach (var item in groups)
            {
                var child = new AccountSystemTreeItemDto()
                {
                    Id = item.Id,
                    ParentId = item.ParentAccId,
                    Value = item.AccCode,
                    Code = item.AccCode,
                    Name = item.AccName,
                    ParentCode = item.ParentCode,
                    Rank = rank == null ? 1 : rank + 1,
                    SortPath = string.IsNullOrEmpty(sortPath) ? item.AccCode
                                : $"{sortPath}|{item.AccCode}",
                    HasChild = false
                };
                int treeCount = BuildTreeView(partnerGroups, item.Id, tree, child.Rank, child.SortPath);
                if (treeCount > 0)
                {
                    child.HasChild = true;
                }
                tree.Add(child);
                count++;
            }
            return count;
        }
        private List<BalanceSheetAccDto> ProcessBalanceValue(List<BalanceSheetAccDto> sheets)
        {
            Parallel.ForEach(sheets, item =>
            {
                decimal debit1 = item.Debit1 == null ? 0 : item.Debit1.Value;
                decimal credit1 = item.Credit1 == null ? 0 : item.Credit1.Value;
                decimal debit2 = item.Debit2 == null ? 0 : item.Debit2.Value;
                decimal credit2 = item.Credit2 == null ? 0 : item.Credit2.Value;
                decimal debitCur1 = item.DebitCur1 == null ? 0 : item.DebitCur1.Value;
                decimal creditCur1 = item.CreditCur1 == null ? 0 : item.CreditCur1.Value;
                decimal debitCur2 = item.DebitCur2 == null ? 0 : item.DebitCur2.Value;
                decimal creditCur2 = item.CreditCur2 == null ? 0 : item.CreditCur2.Value;

                item.Debit1 = debit1 >= credit1 ? debit1 - credit1 : null;
                item.Credit1 = credit1 > debit1 ? credit1 - debit1 : null;
                item.DebitCur1 = debitCur1 >= creditCur1 ? debitCur1 - creditCur1 : null;
                item.CreditCur1 = creditCur1 > debitCur1 ? creditCur1 - debitCur1 : null;
                item.Debit2 = debit2 >= credit2 ? debit2 - credit2 : null;
                item.Credit2 = credit2 > debit2 ? credit2 - debit2 : null;
                item.DebitCur2 = debitCur2 >= creditCur2 ? debitCur2 - creditCur2 : null;
                item.CreditCur2 = creditCur2 > debitCur2 ? creditCur2 - debitCur2 : null;
            });

            return sheets;
        }

        private async Task<ReportResponseDto<T>> CreateReportResponseDto<T>(List<T> data, ReportRequestDto<ReportBaseParameterDto> dto)
                            where T : class
        {
            var reportResponse = new ReportResponseDto<T>();
            reportResponse.Data = data;
            reportResponse.OrgUnit = await this.GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await this.GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }

        private async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return _objectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
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

        private bool IsAttachContract(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachContract == "C" ? true : false;
        }
        private bool IsAttachFProductWork(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachProductCost == "C" ? true : false;
        }
        private bool IsAttachSection(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachAccSection == "C" ? true : false;
        }
        private bool IsAttachPartner(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachPartner == "C" ? true : false;
        }
        private bool IsAttachCurrency(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachCurrency == "C" ? true : false;
        }
        private bool IsAttachWorkPlace(AccountSystemDto dto)
        {
            if (dto == null) return false;
            return dto.AttachWorkPlace == "C" ? true : false;
        }
        private string DefaultNullIfEmpty(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            return value;
        }
        private decimal? DefaultZeroIfNull(decimal? value)
        {
            if (value == null) return Decimal.Zero;
            return value;
        }
        #endregion
    }
}
