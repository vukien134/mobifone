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
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports.RecordingVouchers
{
    public class RecordingVoucherRegisterBookAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly RecordingVoucherBookService _recordingVoucherBookService;
        private readonly LedgerService _ledgerService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public RecordingVoucherRegisterBookAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        RecordingVoucherBookService recordingVoucherBookService,
                        LedgerService ledgerService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _recordingVoucherBookService = recordingVoucherBookService;
            _ledgerService = ledgerService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion

        #region Methods
        [Authorize(ReportPermissions.RecordingVoucherRegisterBookReportView)]
        public async Task<ReportResponseDto<RecordingVoucherRegisterBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var dic = GetLedgerParameter(dto.Parameters);
            var incurredData = await GetIncurredData(dic, dto);
            var itemTotal = new RecordingVoucherRegisterBookDto
                            {
                                Sort = "C",
                                Bold = "C",
                                Description = "Tổng cộng",
                                Amount = incurredData.Sum(p => p.Amount ?? 0),
                                AmountCur = incurredData.Sum(p => p.AmountCur ?? 0)
                            };
            incurredData.Add(itemTotal);
            var reportResponse = new ReportResponseDto<RecordingVoucherRegisterBookDto>();
            reportResponse.Data = incurredData.OrderBy(p => p.Sort).ThenBy(p => p.Acc).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.RecordingVoucherRegisterBookReportPrint)]
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
        private async Task<List<RecordingVoucherRegisterBookDto>> GetIncurredData(Dictionary<string, object> dic, ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var recordingVoucherBook = await _recordingVoucherBookService.GetQueryableAsync();
            var lstRecordingVoucherBook = recordingVoucherBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var ledgerData = await GetDataledger(dic);
            var incurredData = (from a in ledgerData
                               join b in lstRecordingVoucherBook on a.RecordingVoucherNumber equals b.VoucherNumber into ajb
                               from b in ajb.DefaultIfEmpty()
                               group new { a, b } by new
                               {
                                   a.OrgCode,
                                   a.Year,
                                   a.Acc,
                                   RecordingVoucherDate = b?.VoucherDate ?? null,
                                   RecordingVoucherNumber = b?.VoucherNumber ?? null,
                                   Description = b?.Description ?? null,
                               } into gr
                               select new RecordingVoucherRegisterBookDto
                               {
                                   Sort = (gr.Key.Description == null) ? "A" : "B",
                                   Bold = "K",
                                   OrgCode = gr.Key.OrgCode,
                                   Year = gr.Key.Year,
                                   Acc = gr.Key.Acc,
                                   RecordingVoucherDate = gr.Key.RecordingVoucherDate ?? null,
                                   RecordingVoucherNumber = gr.Key.RecordingVoucherNumber ?? "",
                                   Description = gr.Key.Description ?? "Chứng từ chưa đăng ký CTGS",
                                   Amount = gr.Sum(p => p.a.Amount0 ?? 0),
                                   AmountCur = gr.Sum(p => p.a.AmountCur0 ?? 0),
                                   FromDate = dto.Parameters.FromDate,
                                   ToDate = dto.Parameters.ToDate,
                               }).ToList();
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
            return dic;
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
    }
}

