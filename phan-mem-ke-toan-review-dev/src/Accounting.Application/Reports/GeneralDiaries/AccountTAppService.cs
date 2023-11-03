using Accounting.Caching;
using Accounting.Categories.Accounts;
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
    public class AccountTAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly AccountSystemAppService _accountSystemAppService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly OrgUnitService _orgUnitService;        
        private readonly CircularsService _circularsService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly AccPartnerService _accPartnerService;
        #endregion
        #region Ctor
        public AccountTAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        AccountSystemAppService accountSystemAppService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccountingCacheManager accountingCacheManager,
                        AccPartnerService accPartnerService
                        )
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _accountSystemAppService = accountSystemAppService;
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accountingCacheManager = accountingCacheManager;
            _accPartnerService = accPartnerService;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.AccountingTReportView)]
        public async Task<ReportResponseDto<AccountTDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            var yearCategory = await _yearCategoryService.GetLatestFromDateAsync(orgCode, dto.Parameters.FromDate.Value);
            var lst = new List<AccountTDto>();

            decimal balance = openingBalance.Debit.GetValueOrDefault(0) - openingBalance.Credit.GetValueOrDefault(0);
            decimal balanceCur = openingBalance.DebitCur.GetValueOrDefault(0) - openingBalance.CreditCur.GetValueOrDefault(0);
            var debit = openingBalance.Debit.GetValueOrDefault(0);
            var credit = openingBalance.Credit.GetValueOrDefault(0);
            var debitCur = openingBalance.DebitCur.GetValueOrDefault(0);
            var creditCur = openingBalance.CreditCur.GetValueOrDefault(0);
            var accSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccSystem = await _accountingCacheManager.GetAccountSystemsAsync(yearCategory.Year);
            var accountSystems = await _accountSystemService.GetAccountByRank(dto.Parameters.AccRank ?? 1, _webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            incurredData = incurredData.Where(p => accountSystems.Select(p => p.AccCode).Contains(p.ReciprocalAcc)).ToList();    

            decimal totalDebitIncurredCur = 0,
                    totalDebitIncurred = 0,
                    totalCreditIncurred = 0,
                    totalCreditIncurredCur = 0;

            var iQGroupByReciprocalAcc = from a in incurredData
                                         join b in lstAccSystem on a.ReciprocalAcc equals b.AccCode into ajb
                                         from b in ajb.DefaultIfEmpty()
                                         group new { a, b } by new
                                         {
                                             a.ReciprocalAcc
                                         } into gr
                                         orderby gr.Key.ReciprocalAcc
                                         select new AccountTDto
                                         {
                                             Sort = "B",
                                             Bold = "K",
                                             ReciprocalAcc = gr.Key.ReciprocalAcc,
                                             AccName = gr.Max(p => p.b?.AccName ?? "<font color=red>[Không có tên tài khoản]</font>"),
                                             DebitIncurred = gr.Sum(p => p.a.DebitIncurred),
                                             DebitIncurredCur = gr.Sum(p => p.a.DebitIncurredCur),
                                             CreditIncurred = gr.Sum(p => p.a.CreditIncurred),
                                             CreditIncurredCur = gr.Sum(p => p.a.CreditIncurredCur),
                                         };
            var lstGroupByReciprocalAcc = iQGroupByReciprocalAcc.ToList();
            foreach (var item in lstGroupByReciprocalAcc)
            {
                balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);

                item.Balance = balance;
                item.BalanceCur = balanceCur;
            }
            lst.AddRange(lstGroupByReciprocalAcc);
            lst.AddRange((from a in lst
                         join b in accountSystems on 1 equals 1
                         where a.ReciprocalAcc.StartsWith(b.AccCode)
                            && (a.ReciprocalAcc ?? "") != ""
                            && a.ReciprocalAcc != b.AccCode
                          group new { a, b} by new
                         {
                             b.AccCode,
                             b.AccName
                         } into gr
                         select new AccountTDto
                         {
                             Sort = "B",
                             Bold = "C",
                             ReciprocalAcc = gr.Key.AccCode,
                             AccName = gr.Key.AccName,
                             DebitIncurred = gr.Sum(p => p.a.DebitIncurred),
                             DebitIncurredCur = gr.Sum(p => p.a.DebitIncurredCur),
                             CreditIncurred = gr.Sum(p => p.a.CreditIncurred),
                             CreditIncurredCur = gr.Sum(p => p.a.CreditIncurredCur),
                         }).ToList());
            // thêm dư đầu
            lst.Add(new AccountTDto()
            {
                Sort = "A",
                Bold = "C",
                AccName = "Dư đầu",
                DebitIncurred = openingBalance.Debit.GetValueOrDefault(0),
                DebitIncurredCur = openingBalance.DebitCur.GetValueOrDefault(0),
                CreditIncurred = openingBalance.Credit.GetValueOrDefault(0),
                CreditIncurredCur = openingBalance.CreditCur.GetValueOrDefault(0),
            });

            lst.Add(new AccountTDto()
            {
                Sort = "C",
                Bold = "C",
                AccName = "Tổng phát sinh",
                DebitIncurredCur = totalDebitIncurredCur,
                DebitIncurred = totalDebitIncurred,
                CreditIncurredCur = totalCreditIncurredCur,
                CreditIncurred = totalCreditIncurred
            });

            debit += totalDebitIncurred;
            credit += totalCreditIncurred;
            debitCur += totalDebitIncurredCur;
            creditCur += totalCreditIncurredCur;
            lst.Add(new AccountTDto()
            {
                Sort = "D",
                Bold = "C",
                AccName = "Dư cuối kỳ",
                DebitIncurred = debit - credit > 0 ? debit - credit : 0,
                CreditIncurred = debit - credit > 0 ? 0 : credit - debit,
                DebitIncurredCur = debitCur - creditCur > 0 ? debitCur - creditCur : 0,
                CreditIncurredCur = debitCur - creditCur > 0 ? 0 : creditCur - debitCur,
            });

            var reportResponse = new ReportResponseDto<AccountTDto>();
            reportResponse.Data = lst.OrderBy(p => p.Sort).ThenBy(p => p.ReciprocalAcc).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.AccountingTReportPrint)]
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
        private async Task<List<AccountTDto>> GetIncurredData(Dictionary<string,object> dic)
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
                g.WorkPlaceCode,
                g.FProductWorkCode,
                g.SectionCode,
                g.Representative,
                g.ReciprocalAcc
            }).Select(p => new AccountTDto()
            {
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                ReciprocalAcc = p.Key.ReciprocalAcc,
                DebitIncurredCur = p.Sum(g => g.DebitIncurredCur),
                DebitIncurred = p.Sum(g => g.DebitIncurred),
                CreditIncurredCur = p.Sum(g => g.CreditIncurredCur),
                CreditIncurred = p.Sum(g => g.CreditIncurred),
            }).OrderBy(p => p.ReciprocalAcc).ToList();
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
            if (!string.IsNullOrEmpty(dto.FProductWorkCode))
            {
                dic.Add(LedgerParameterConst.FProductWorkCode, dto.FProductWorkCode);
            }
            if (!string.IsNullOrEmpty(dto.SectionCode))
            {
                dic.Add(LedgerParameterConst.SectionCode, dto.SectionCode);
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
