using Accounting.Caching;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
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
    public class LedgerRecordingVoucherAppService : AccountingAppService
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
        private readonly RecordingVoucherBookService _recordingVoucherBookService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public LedgerRecordingVoucherAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        RecordingVoucherBookService recordingVoucherBookService,
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
            _recordingVoucherBookService = recordingVoucherBookService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.LedgerRecordingVoucherReportView)]
        public async Task<ReportResponseDto<LedgerRecordingVoucherDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var openingBalance = await GetOpeningBalance(dic);
            var incurredData = await GetIncurredData(dic);
            var recordingVoucherBook = await _recordingVoucherBookService.GetQueryableAsync();
            var lstRecordingVoucherBook = recordingVoucherBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var lst = new List<LedgerRecordingVoucherDto>();

            decimal balance = openingBalance.Debit.GetValueOrDefault(0) - openingBalance.Credit.GetValueOrDefault(0);
            decimal balanceCur = openingBalance.DebitCur.GetValueOrDefault(0) - openingBalance.CreditCur.GetValueOrDefault(0);
            var debit = openingBalance.Debit.GetValueOrDefault(0);
            var credit = openingBalance.Credit.GetValueOrDefault(0);
            var debitCur = openingBalance.DebitCur.GetValueOrDefault(0);
            var creditCur = openingBalance.CreditCur.GetValueOrDefault(0);

            lst.Add(new LedgerRecordingVoucherDto()
            {
                Bold = "C",
                Note = "Dư đầu",
                DebitIncurred = openingBalance.Debit.GetValueOrDefault(0),
                DebitIncurredCur = openingBalance.DebitCur.GetValueOrDefault(0),
                CreditIncurred = openingBalance.Credit.GetValueOrDefault(0),
                CreditIncurredCur = openingBalance.CreditCur.GetValueOrDefault(0),
            });

            decimal totalDebitIncurredCur = 0,
                    totalDebitIncurred = 0,
                    totalCreditIncurred = 0,
                    totalCreditIncurredCur = 0;
            //Lấy dữ liệu tổng phát sinh của từng tháng
            var iQTotalIncurredDataMonth = from a in incurredData
                                           group new { a } by new
                                           {
                                               Year = a.VoucherDate.Value.Year,
                                               a.Month
                                           } into gr
                                           orderby gr.Key.Month 
                                           select new
                                           {
                                               Year = gr.Key.Year,
                                               Month = gr.Key.Month,
                                               Note = "Phát sinh tháng " + gr.Key.Month,
                                               DebitIncurred = gr.Sum(p => p.a.DebitIncurred),
                                               CreditIncurred = gr.Sum(p => p.a.CreditIncurred),
                                               DebitIncurredCur = gr.Sum(p => p.a.DebitIncurredCur),
                                               CreditIncurredCur = gr.Sum(p => p.a.CreditIncurredCur)
                                           };
            var lstTotalIncurredDataMonth = iQTotalIncurredDataMonth.ToList();

            foreach (var item in lstTotalIncurredDataMonth)
            {
                lst.Add(new LedgerRecordingVoucherDto()
                {
                    Bold = "C",
                    Note = item.Note,
                    DebitIncurred = item.DebitIncurred,
                    CreditIncurred = item.CreditIncurred,
                    DebitIncurredCur = item.DebitIncurredCur,
                    CreditIncurredCur = item.CreditIncurredCur,
                });
                var incurredDataMonth = incurredData.Where(p => p.Month == item.Month && p.Year == item.Year)
                                                    .OrderBy(p => p.VoucherDate)
                                                    .ThenBy(p => p.VoucherNumber)
                                                    .ThenBy(p => p.VoucherId)
                                                    .ThenBy(p => p.ReciprocalAcc).ToList();
                foreach (var itemData in incurredDataMonth) {
                    var recordingVoucher = lstRecordingVoucherBook.Where(p => p.VoucherNumber == itemData.RecordingVoucherNumber).FirstOrDefault();
                    itemData.RecordingVoucherDate = recordingVoucher?.VoucherDate ?? null;
                    itemData.VoucherNumber = recordingVoucher?.VoucherNumber ?? "";
                }
                lst.AddRange(incurredDataMonth);
                lst.Add(new LedgerRecordingVoucherDto()
                {
                    Bold = "C",
                    Note = "Tổng phát sinh tháng " + item.Month,
                    DebitIncurred = item.DebitIncurred,
                    CreditIncurred = item.CreditIncurred,
                    DebitIncurredCur = item.DebitIncurredCur,
                    CreditIncurredCur = item.CreditIncurredCur,
                });

                balance = balance + item.DebitIncurred.GetValueOrDefault(0) - item.CreditIncurred.GetValueOrDefault(0);
                balanceCur = balanceCur + item.DebitIncurredCur.GetValueOrDefault(0) - item.CreditIncurredCur.GetValueOrDefault(0);
                totalDebitIncurredCur = totalDebitIncurredCur + item.DebitIncurredCur.GetValueOrDefault(0);
                totalDebitIncurred = totalDebitIncurred + item.DebitIncurred.GetValueOrDefault(0);
                totalCreditIncurredCur = totalCreditIncurredCur + item.CreditIncurredCur.GetValueOrDefault(0);
                totalCreditIncurred = totalCreditIncurred + item.CreditIncurred.GetValueOrDefault(0);

                debit += item.DebitIncurred ?? 0;
                credit += item.CreditIncurred ?? 0;
                debitCur += item.DebitIncurredCur ?? 0;
                creditCur += item.CreditIncurredCur ?? 0;
                lst.Add(new LedgerRecordingVoucherDto()
                {
                    Bold = "C",
                    Note = "Dư cuối tháng " + item.Month,
                    DebitIncurred = debit - credit > 0 ? debit - credit : 0,
                    CreditIncurred = debit - credit > 0 ? 0 : credit - debit,
                    DebitIncurredCur = debitCur - creditCur > 0 ? debitCur - creditCur : 0,
                    CreditIncurredCur = debitCur - creditCur > 0 ? 0 : creditCur - debitCur,
                });
            }

            lst.Add(new LedgerRecordingVoucherDto()
            {
                Bold = "C",
                Note = "Tổng phát sinh",
                DebitIncurredCur = totalDebitIncurredCur,
                DebitIncurred = totalDebitIncurred
            });
            lst.Add(new LedgerRecordingVoucherDto()
            {
                Bold = "C",
                Note = "Dư cuối kỳ",
                DebitIncurred = debit - credit > 0 ? debit - credit : 0,
                CreditIncurred = debit - credit > 0 ? 0 : credit - debit,
                DebitIncurredCur = debitCur - creditCur > 0 ? debitCur - creditCur : 0,
                CreditIncurredCur = debitCur - creditCur > 0 ? 0 : creditCur - debitCur,
            });
            var reportResponse = new ReportResponseDto<LedgerRecordingVoucherDto>();
            reportResponse.Data = lst;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());            
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.LedgerRecordingVoucherReportPrint)]
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
        private async Task<List<LedgerRecordingVoucherDto>> GetIncurredData(Dictionary<string,object> dic)
        {
            var ledgerData = await GetDataledger(dic);
            var incurredData = ledgerData.GroupBy(g => new
            {
                g.OrgCode,
                g.Year,
                g.VoucherCode,
                g.VoucherDate,
                g.VoucherNumber,
                g.RecordingVoucherNumber,
                g.VoucherGroup,
                g.VoucherId,
                g.Note,
                g.PartnerCode,
                g.WorkPlaceCode,
                g.FProductWorkCode,
                g.SectionCode,
                g.Representative,
                g.ReciprocalAcc
            }).Select(p => new LedgerRecordingVoucherDto()
            {
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                VoucherCode = p.Key.VoucherCode,
                VoucherDate = p.Key.VoucherDate,
                VoucherNumber = p.Key.VoucherNumber,
                RecordingVoucherNumber = p.Key.RecordingVoucherNumber,
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
                CreditIncurred = p.Sum(g => g.CreditIncurred),
                Month = p.Key.VoucherDate.Value.Month
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
