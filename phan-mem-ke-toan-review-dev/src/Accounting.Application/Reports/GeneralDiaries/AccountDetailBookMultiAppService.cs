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
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.GeneralDiaries
{
    public class AccountDetailBookMultiAppService : AccountingAppService
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
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public AccountDetailBookMultiAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
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
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.MultiAccountDetailBookReportView)]
        public async Task<ReportResponseDto<AccountDetailBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var accCode = dto.Parameters.AccCode;
            var lstAccCode = new List<String>();
            var iQAccCount = await _accountSystemService.GetQueryableAsync();
            iQAccCount = iQAccCount.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            if (accCode == null || accCode == "")
            {
                dto.Parameters.AccCode = "";
                var dic0 = GetLedgerParameter(dto.Parameters);
                var incurredData0 = await GetIncurredData(dic0);
                //Lấy dữ liệu các tài khoản mẹ
                lstAccCode = iQAccCount.Where(p => p.ParentAccId == null || p.ParentAccId == "").Select(p => p.AccCode).ToList();
            }
            else
            {
                var lstAccCode0 = accCode.Split(',').ToList();
                var lstAccCountSystem = iQAccCount.Where(p => lstAccCode0.Contains(p.AccCode)).ToList();
                var lstAccCountParentNew = iQAccCount.Where(p => lstAccCountSystem.Select(a => a.ParentAccId).Contains(p.Id)).ToList();
                var lstAccCountParentDelete = lstAccCountParentNew;
                while (lstAccCountParentNew.Count != 0)
                {
                    lstAccCountParentNew = iQAccCount.Where(p => lstAccCountParentNew.Select(a => a.ParentAccId).Contains(p.Id)).ToList();
                    lstAccCountParentDelete.AddRange(lstAccCountParentNew);
                }
                foreach (var itemAccCode0 in lstAccCode0)
                {
                    if (!lstAccCountParentDelete.Any(p => p.AccCode == itemAccCode0))
                    {
                        lstAccCode.Add(itemAccCode0);
                    }
                }
            }

            var lst = new List<AccountDetailBookDto>();
            foreach (var itemAccCode in lstAccCode)
            {
                var accName = iQAccCount.Where(p => p.AccCode == itemAccCode).Select(p => p.AccName).FirstOrDefault();
                dto.Parameters.AccCode = itemAccCode;
                var dic = GetLedgerParameter(dto.Parameters);
                var openingBalance = await GetOpeningBalance(dic);
                var incurredData = await GetIncurredData(dic);
                decimal balance = openingBalance.Debit.GetValueOrDefault(0) - openingBalance.Credit.GetValueOrDefault(0);
                decimal balanceCur = openingBalance.DebitCur.GetValueOrDefault(0) - openingBalance.CreditCur.GetValueOrDefault(0);
                lst.Add(new AccountDetailBookDto()
                {
                    Bold = "C",
                    Note = itemAccCode + " - " + accName,
                    BalanceCur = balanceCur,
                    Balance = balance
                });
                lst.Add(new AccountDetailBookDto()
                {
                    Bold = "C",
                    Note = "Số dư đầu kỳ",
                    BalanceCur = balanceCur,
                    Balance = balance
                });

                decimal totalDebitIncurredCur = 0,
                        totalDebitIncurred = 0,
                        totalCreditIncurred = 0,
                        totalCreditIncurredCur = 0;

                foreach (var item in incurredData)
                {
                    balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                    balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                    totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                    totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                    totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                    totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);

                    item.PartnerName = await GetPartnerName(orgCode, item.PartnerCode);
                    item.Balance = balance;
                    item.BalanceCur = balanceCur;
                    item.Acc = itemAccCode;
                }

                lst.AddRange(incurredData);
                lst.Add(new AccountDetailBookDto()
                {
                    Bold = "C",
                    Note = "Phát sinh trong kỳ",
                    DebitIncurredCur = totalDebitIncurredCur,
                    DebitIncurred = totalDebitIncurred,
                    CreditIncurredCur = totalCreditIncurredCur,
                    CreditIncurred = totalCreditIncurred,
                    Acc = itemAccCode,
                });
                lst.Add(new AccountDetailBookDto()
                {
                    Bold = "C",
                    Note = "Số dư cuối kỳ",
                    BalanceCur = balanceCur,
                    Balance = balance,
                    Acc = itemAccCode
                });
            }
            dto.Parameters.AccCode = accCode;
            var reportResponse = new ReportResponseDto<AccountDetailBookDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.MultiAccountDetailBookReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<ReportBaseParameterDto> dto)
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
        #endregion
        #region Private
        private async Task<List<AccountDetailBookDto>> GetIncurredData(Dictionary<string,object> dic)
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
                g.ReciprocalAcc
            }).Select(p => new AccountDetailBookDto()
            {
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                VoucherCode = p.Key.VoucherCode,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                VoucherId = p.Key.VoucherId,
                Note = p.Key.Note,
                PartnerCode = p.Key.PartnerCode,
                WorkPlaceCode = p.Key.WorkPlaceCode,
                FProductWorkCode = p.Key.FProductWorkCode,
                SectionCode = p.Key.SectionCode,
                Representative = p.Key.Representative,
                ReciprocalAcc = p.Key.ReciprocalAcc,
                DebitIncurredCur = p.Sum(g => g.DebitIncurredCur),
                DebitIncurred = p.Sum(g => g.DebitIncurred),
                CreditIncurredCur = p.Sum(g => g.CreditIncurredCur),
                CreditIncurred = p.Sum(g => g.CreditIncurred)
            }).OrderBy(p => p.VoucherDate)
                .ThenBy(p => p.VoucherNumber)
                .ThenBy(p => p.VoucherId)
                .ThenBy(p => p.ReciprocalAcc).ToList();
            return incurredData;
        }
        private async Task<List<LedgerGeneralDto>> GetDataledger(Dictionary<string,object> dic)
        {
            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private Dictionary<string,object> GetLedgerParameter(ReportBaseParameterDto dto)
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
        private async Task<List<AccountDetailBookDto>> GetSumByReciprocalAcc(List<AccountDetailBookDto> accountDetailBooks)
        {
            if (accountDetailBooks.Count == 0) return new List<AccountDetailBookDto>();
            var accounts = await _accountSystemService.GetAccounts(_webHelper.GetCurrentOrgUnit());

            var groupData = accountDetailBooks.GroupBy(g => new { g.ReciprocalAcc,g.Year })
                            .Select(p => new AccountDetailBookDto()
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
                        select new AccountDetailBookDto()
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
        #endregion
    }
}
