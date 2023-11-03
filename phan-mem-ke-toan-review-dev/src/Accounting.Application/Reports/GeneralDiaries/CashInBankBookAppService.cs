using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Volo.Abp.Authorization;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports.GeneralDiaries
{
    public class CashInBankBookAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public CashInBankBookAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        VoucherCategoryService voucherCategoryService,
                        DefaultVoucherCategoryService defaultVoucherCategoryService,
                        AccountingCacheManager accountingCacheManager,
                        IStringLocalizer<AccountingResource> localizer)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _voucherCategoryService = voucherCategoryService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _accountingCacheManager = accountingCacheManager;
            _localizer = localizer;
        }
        #endregion

        #region Methods
        public async Task<ReportResponseDto<CashInBankBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstvoucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var defaultVoucherCategory = await _defaultVoucherCategoryService.GetQueryableAsync();

            var lst = new List<CashInBankBookDto>();

            decimal balance = openingBalance.Debit.GetValueOrDefault(0) - openingBalance.Credit.GetValueOrDefault(0);
            decimal balanceCur = openingBalance.DebitCur.GetValueOrDefault(0) - openingBalance.CreditCur.GetValueOrDefault(0);

            lst.Add(new CashInBankBookDto()
            {
                Bold = "C",
                Note = "Số dư đầu kỳ",
                Debit = balance,
                DebitIncurredCur = balanceCur,
                NOT_PRINT = "C"
            });

            decimal totalDebitIncurredCur = 0,
                    totalDebitIncurred = 0,
                    totalCreditIncurred = 0,
                    totalCreditIncurredCur = 0;
            if (lstvoucherCategory.Count > 0)
            {
                incurredData = (from a in incurredData
                                join b in lstvoucherCategory on a.VoucherCode equals b.Code
                                orderby a.VoucherDate, b.VoucherOrd, a.VoucherNumber, a.ReciprocalAcc
                                select new CashInBankBookDto
                                {
                                    OrgCode = a.OrgCode,
                                    ReciprocalAcc = a.ReciprocalAcc,
                                    Year = a.Year,
                                    VoucherCode = a.VoucherCode,
                                    VoucherDate = a.VoucherDate,
                                    VoucherNumber = a.VoucherNumber,
                                    Note = a.Note,
                                    PartnerCode = a.PartnerCode,
                                    Representative = a.Representative,
                                    DebitIncurredCur = a.DebitIncurredCur,
                                    DebitIncurred = a.DebitIncurred,
                                    CreditIncurredCur = a.CreditIncurredCur,
                                    CreditIncurred = a.CreditIncurred,
                                    VoucherId = a.VoucherId,
                                    Sort0 = b.VoucherOrd
                                }).ToList();
            }
            else
            {
                incurredData = (from a in incurredData
                                join b in defaultVoucherCategory on a.VoucherCode equals b.Code
                                orderby a.VoucherDate, b.VoucherOrd, a.VoucherNumber, a.ReciprocalAcc
                                select new CashInBankBookDto
                                {
                                    OrgCode = a.OrgCode,
                                    ReciprocalAcc = a.ReciprocalAcc,
                                    Year = a.Year,
                                    VoucherCode = a.VoucherCode,
                                    VoucherDate = a.VoucherDate,
                                    VoucherNumber = a.VoucherNumber,
                                    Note = a.Note,
                                    PartnerCode = a.PartnerCode,
                                    Representative = a.Representative,
                                    DebitIncurredCur = a.DebitIncurredCur,
                                    DebitIncurred = a.DebitIncurred,
                                    CreditIncurredCur = a.CreditIncurredCur,
                                    CreditIncurred = a.CreditIncurred,
                                    VoucherId = a.VoucherId,
                                    Sort0 = b.VoucherOrd
                                }).ToList();
            }

            foreach (var item in incurredData)
            {
                balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);
                item.Note = item.Note + " ( " + "theo chứng từ số " + item.VoucherNumber + " ngày " + item.VoucherDate + " )" + "( " + await GetPartnerName(orgCode, item.PartnerCode) + " )";
                item.PartnerName = await GetPartnerName(orgCode, item.PartnerCode);
                item.Debit = balance;
                item.DebitCur = balanceCur;
                item.NOT_PRINT = "C";

            }
            incurredData = incurredData.OrderBy(p => p.VoucherDate).ThenBy(p => p.Sort0).ThenBy(p => p.VoucherNumber).ToList();
            lst.AddRange(incurredData);
            lst.Add(new CashInBankBookDto()
            {
                Bold = "C",
                Note = "Tổng phát sinh",
                DebitIncurredCur = totalDebitIncurredCur,
                DebitIncurred = totalDebitIncurred,
                CreditIncurredCur = totalCreditIncurredCur,
                CreditIncurred = totalCreditIncurred,
                NOT_PRINT = "C"
            });
            lst.Add(new CashInBankBookDto()
            {
                Bold = "C",
                Note = "Số dư cuối kỳ",
                Debit = balance,
                DebitIncurredCur = balanceCur,
                NOT_PRINT = "C"
            });
            lst.Add(new CashInBankBookDto()
            {
                Bold = "C",
                Note = "-------------------------------"
            });

            var reciprocalAccs = await GetSumByReciprocalAcc(incurredData);
            lst.AddRange(reciprocalAccs);

            var reportResponse = new ReportResponseDto<CashInBankBookDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        #endregion
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            await this.CheckPermission(dto.ReportTemplateCode, ReportPermissions.ActionView);
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
        private async Task<List<CashInBankBookDto>> GetIncurredData(Dictionary<string, object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.GroupBy(g => new
            {
                g.OrgCode,
                g.Year,
                g.VoucherCode,
                g.VoucherDate,
                g.VoucherNumber,
                g.VoucherId,
                g.Note,
                g.PartnerCode,
                g.WorkPlaceCode,
                g.FProductWorkCode,
                g.SectionCode,
                g.Representative,
                g.ReciprocalAcc,
                g.PartnerCode0,

            }).Select(p => new CashInBankBookDto()
            {
                OrgCode = p.Key.OrgCode,
                ReciprocalAcc = p.Key.ReciprocalAcc,
                Year = p.Key.Year,
                VoucherCode = p.Key.VoucherCode,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                Note = p.Key.Note,
                PartnerCode = !string.IsNullOrEmpty(p.Key.PartnerCode) ? p.Key.PartnerCode : p.Key.PartnerCode0,
                Representative = p.Key.Representative,
                DebitIncurredCur = p.Sum(g => g.DebitIncurredCur),
                DebitIncurred = p.Sum(g => g.DebitIncurred),
                CreditIncurredCur = p.Sum(g => g.CreditIncurredCur),
                CreditIncurred = p.Sum(g => g.CreditIncurred),
                VoucherId = p.Key.VoucherId,
                Sort0 = null
            }).OrderBy(p => p.VoucherDate).ThenBy(p => p.VoucherNumber)
                .ToList();
            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string, object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private Dictionary<string, object> GetLedgerParameter(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
            if (!string.IsNullOrEmpty(dto.PartnerCode))
            {
                dic.Add(LedgerParameterConst.PartnerCode, dto.PartnerCode);
            }
            if (!string.IsNullOrEmpty(dto.WorkPlaceCode))
            {
                dic.Add(LedgerParameterConst.WorkPlaceCode, dto.WorkPlaceCode);
            }
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
            }
            if (!string.IsNullOrEmpty(dto.CurrencyCode))
            {
                dic.Add(LedgerParameterConst.CurrencyCode, dto.CurrencyCode);
            }

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
        private async Task<List<CashInBankBookDto>> GetSumByReciprocalAcc(List<CashInBankBookDto> accountDetailBooks)
        {
            if (accountDetailBooks.Count == 0) return new List<CashInBankBookDto>();
            var accounts = await _accountSystemService.GetAccounts(_webHelper.GetCurrentOrgUnit());

            var groupData = accountDetailBooks.GroupBy(g => new { g.ReciprocalAcc, g.Year })
                            .Select(p => new CashInBankBookDto()
                            {
                                Year = p.Key.Year,
                                ReciprocalAcc = p.Key.ReciprocalAcc,
                                DebitIncurredCur = p.Sum(i => i.DebitIncurredCur),
                                DebitIncurred = p.Sum(i => i.DebitIncurred),
                                CreditIncurredCur = p.Sum(i => i.CreditIncurredCur),
                                CreditIncurred = p.Sum(i => i.CreditIncurred)
                            }).ToList();

            var query = from d in groupData
                        join a in accounts on new { AccCode = d.ReciprocalAcc, d.Year } equals new { a.AccCode, a.Year } into ac
                        from n in ac.DefaultIfEmpty()
                        orderby d.ReciprocalAcc
                        select new CashInBankBookDto()
                        {
                            Bold = "C",
                            Year = d.Year,
                            ReciprocalAcc = d.ReciprocalAcc,
                            DebitIncurredCur = d.DebitIncurredCur,
                            DebitIncurred = d.DebitIncurred,
                            CreditIncurredCur = d.CreditIncurredCur,
                            CreditIncurred = d.CreditIncurred,
                            Note = n?.AccName ?? String.Empty
                        };
            return query.ToList();
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
        protected async Task CheckPermission(string reportCode, string action)
        {
            bool isGrant = await this.IsGrantPermission(reportCode, action);
            if (!isGrant)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
        }
        private async Task<bool> IsGrantPermission(string reportCode, string action)
        {
            string permissionName = $"{reportCode}_{action}";
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
    }
}

