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
    public class DepositSpendDiaryAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly OrgUnitService _orgUnitService;        
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public DepositSpendDiaryAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
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
            _voucherCategoryService = voucherCategoryService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.DepositSpendDiaryReportView)]
        public async Task<ReportResponseDto<CashSpendDiaryDto>> CreateDataAsync(ReportRequestDto<CashCollectionDiaryParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            var acc1 = (dto.Parameters.Acc1 != "" && dto.Parameters.Acc1 != null) ? dto.Parameters.Acc1 : "111";
            var acc2 = (dto.Parameters.Acc2 != "" && dto.Parameters.Acc2 != null) ? dto.Parameters.Acc2 : "131";
            var acc3 = (dto.Parameters.Acc3 != "" && dto.Parameters.Acc3 != null) ? dto.Parameters.Acc3 : "331";
            var acc4 = (dto.Parameters.Acc4 != "" && dto.Parameters.Acc4 != null) ? dto.Parameters.Acc4 : "627";
            var acc5 = (dto.Parameters.Acc5 != "" && dto.Parameters.Acc5 != null) ? dto.Parameters.Acc5 : "642";
            var lst = new List<CashSpendDiaryDto>();
            var incurredAcc = "";
            if (dto.Parameters.DebitCredit == "N")
            {
                incurredAcc = "Tổng phát sinh TK nợ " + dto.Parameters.AccCode;
            } else if (dto.Parameters.DebitCredit == "C")
            {
                incurredAcc = "Tổng phát sinh TK có " + dto.Parameters.AccCode;
            }
            else
            {
                incurredAcc = "Tổng phát sinh TK " + dto.Parameters.AccCode;
            }
            decimal balance = openingBalance.Debit.GetValueOrDefault(0) - openingBalance.Credit.GetValueOrDefault(0);
            decimal balanceCur = openingBalance.DebitCur.GetValueOrDefault(0) - openingBalance.CreditCur.GetValueOrDefault(0);
            var accSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccSystem = accSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();     
            decimal totalDebitIncurredCur = 0, 
                    totalDebitIncurred = 0,
                    totalCreditIncurred = 0,
                    totalCreditIncurredCur = 0,
                    tTotalCreditIncurredCur = 0,
                    tTotalCreditIncurred = 0,
                    totalReciprocalIncurredCur1 = 0,
                    totalReciprocalIncurred1 = 0,
                    totalReciprocalIncurredCur2 = 0,
                    totalReciprocalIncurred2 = 0,
                    totalReciprocalIncurredCur3 = 0,
                    totalReciprocalIncurred3 = 0,
                    totalReciprocalIncurredCur4 = 0,
                    totalReciprocalIncurred4 = 0,
                    totalReciprocalIncurredCur5 = 0,
                    totalReciprocalIncurred5 = 0,
                    totalReciprocalIncurredCurOther = 0,
                    totalReciprocalIncurredOther = 0;

            var iQIncurredData = from a in incurredData
                                 select new CashSpendDiaryDto
                                 {
                                     Sort = "A",
                                     Bold = "K",
                                     OrgCode = a.OrgCode,
                                     Year = a.Year,
                                     VoucherId = a.VoucherId,
                                     VoucherCode = a.VoucherCode,
                                     VoucherDate = a.VoucherDate,
                                     Note = a.Note,
                                     Acc = a.Acc,
                                     VoucherNumber = a.VoucherNumber,
                                     ReciprocalAcc = a.ReciprocalAcc ?? "",
                                     DebitIncurredCur = a.DebitIncurredCur,
                                     DebitIncurred = a.DebitIncurred,
                                     CreditIncurredCur = a.CreditIncurredCur,
                                     CreditIncurred = a.CreditIncurred,
                                     TotalCreditIncurredCur = a.CreditIncurredCur,
                                     TotalCreditIncurred = a.CreditIncurred,
                                     ReciprocalIncurredCur1 = (acc1 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc1)) ? a.CreditIncurredCur : null,
                                     ReciprocalIncurred1 = (acc1 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc1)) ? a.CreditIncurred : null,
                                     ReciprocalIncurredCur2 = (acc2 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc2)) ? a.CreditIncurredCur : null,
                                     ReciprocalIncurred2 = (acc2 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc2)) ? a.CreditIncurred : null,
                                     ReciprocalIncurredCur3 = (acc3 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc3)) ? a.CreditIncurredCur : null,
                                     ReciprocalIncurred3 = (acc3 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc3)) ? a.CreditIncurred : null,
                                     ReciprocalIncurredCur4 = (acc4 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc4)) ? a.CreditIncurredCur : null,
                                     ReciprocalIncurred4 = (acc4 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc4)) ? a.CreditIncurred : null,
                                     ReciprocalIncurredCur5 = (acc5 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc5)) ? a.CreditIncurredCur : null,
                                     ReciprocalIncurred5 = (acc5 != "" && (a.ReciprocalAcc ?? "").StartsWith(acc5)) ? a.CreditIncurred : null,
                                     ReciprocalAccOther = "",
                                     ReciprocalIncurredCurOther = 0,
                                     ReciprocalIncurredOther = 0,
                                     FromDate = dto.Parameters.FromDate,
                                     ToDate = dto.Parameters.ToDate,
                                     IncurredAcc = dto.Parameters.AccCode,
                                     Acc1 = acc1,
                                     Acc2 = acc2,
                                     Acc3 = acc3,
                                     Acc4 = acc4,
                                     Acc5 = acc5,
                                 };
            incurredData = iQIncurredData.ToList();
            foreach (var item in incurredData)
            {
                item.ReciprocalAccOther = (item.ReciprocalIncurred1 == null && item.ReciprocalIncurred2 == null && item.ReciprocalIncurred3 == null && item.ReciprocalIncurred4 == null && item.ReciprocalIncurred5 == null) ? item.ReciprocalAcc : null;
                item.ReciprocalIncurredCurOther = (item.ReciprocalIncurred1 == null && item.ReciprocalIncurred2 == null && item.ReciprocalIncurred3 == null && item.ReciprocalIncurred4 == null && item.ReciprocalIncurred5 == null) ? item.CreditIncurredCur : null;
                item.ReciprocalIncurredOther = (item.ReciprocalIncurred1 == null && item.ReciprocalIncurred2 == null && item.ReciprocalIncurred3 == null && item.ReciprocalIncurred4 == null && item.ReciprocalIncurred5 == null) ? item.CreditIncurred : null;
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.FromDate = dto.Parameters.FromDate;
                item.ToDate = dto.Parameters.ToDate;
                item.IncurredAcc = incurredAcc;
                item.Acc1 = "TK " + acc1;
                item.Acc2 = "TK " + acc2;
                item.Acc3 = "TK " + acc3;
                item.Acc4 = "TK " + acc4;
                item.Acc5 = "TK " + acc5;

                totalDebitIncurredCur += item.DebitIncurredCur ?? 0;
                totalDebitIncurred += item.DebitIncurred ?? 0;
                totalCreditIncurred += item.CreditIncurred ?? 0;
                totalCreditIncurredCur += item.CreditIncurredCur ?? 0;
                tTotalCreditIncurredCur += item.TotalCreditIncurredCur ?? 0;
                tTotalCreditIncurred += item.TotalCreditIncurred ?? 0;
                totalReciprocalIncurredCur1 += item.ReciprocalIncurredCur1 ?? 0;
                totalReciprocalIncurred1 += item.ReciprocalIncurred1 ?? 0;
                totalReciprocalIncurredCur2 += item.ReciprocalIncurredCur2 ?? 0;
                totalReciprocalIncurred2 += item.ReciprocalIncurred2 ?? 0;
                totalReciprocalIncurredCur3 += item.ReciprocalIncurredCur3 ?? 0;
                totalReciprocalIncurred3 += item.ReciprocalIncurred3 ?? 0;
                totalReciprocalIncurredCur4 += item.ReciprocalIncurredCur4 ?? 0;
                totalReciprocalIncurred4 += item.ReciprocalIncurred4 ?? 0;
                totalReciprocalIncurredCur5 += item.ReciprocalIncurredCur5 ?? 0;
                totalReciprocalIncurred5 += item.ReciprocalIncurred5 ?? 0;
                totalReciprocalIncurredCurOther += item.ReciprocalIncurredCurOther ?? 0;
                totalReciprocalIncurredOther += item.ReciprocalIncurredOther ?? 0;
                
            }
            lst.AddRange(incurredData);
            lst.Add(new CashSpendDiaryDto()
            {
                Sort = "B",
                Bold = "C",
                Note = "Phát sinh trong kỳ",
                DebitIncurredCur = totalDebitIncurredCur,
                DebitIncurred = totalDebitIncurred,
                CreditIncurredCur = totalCreditIncurredCur,
                CreditIncurred = totalCreditIncurred,
                TotalCreditIncurredCur = tTotalCreditIncurredCur,
                TotalCreditIncurred = tTotalCreditIncurred,
                ReciprocalIncurredCur1 = totalReciprocalIncurredCur1,
                ReciprocalIncurred1 = totalReciprocalIncurred1,
                ReciprocalIncurredCur2 = totalReciprocalIncurredCur2,
                ReciprocalIncurred2 = totalReciprocalIncurred2,
                ReciprocalIncurredCur3 = totalReciprocalIncurredCur3,
                ReciprocalIncurred3 = totalReciprocalIncurred3,
                ReciprocalIncurredCur4 = totalReciprocalIncurredCur4,
                ReciprocalIncurred4 = totalReciprocalIncurred4,
                ReciprocalIncurredCur5 = totalReciprocalIncurredCur5,
                ReciprocalIncurred5 = totalReciprocalIncurred5,
                ReciprocalIncurredOther = totalReciprocalIncurredOther,
            });
            var reportResponse = new ReportResponseDto<CashSpendDiaryDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.DepositSpendDiaryReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<CashCollectionDiaryParameterDto> dto)
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
        private async Task<List<CashSpendDiaryDto>> GetIncurredData(Dictionary<string,object> dic)
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
            }).Select(p => new CashSpendDiaryDto()
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
        private Dictionary<string,object> GetLedgerParameter(CashCollectionDiaryParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, dto.DebitCredit);
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);
            dic.Add(LedgerParameterConst.Acc, dto.AccCode);
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
