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
using Accounting.Report.Constants;
using Accounting.Reports.Cores;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Reports.GeneralDiaries
{
    public class GeneralDiaryReportAppService : AccountingAppService, IGeneralDiaryReportAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly AccountSystemService _accountSystemService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly TenantSettingService _tenantSettingService;
        private readonly OrgUnitService _orgUnitService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public GeneralDiaryReportAppService(ReportDataService reportDataService,
                        AccountSystemService accountSystemService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        TenantSettingService tenantSettingService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        YearCategoryService yearCategoryService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _accountSystemService = accountSystemService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _yearCategoryService = yearCategoryService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.GeneralDiaryBookReportView)]
        public async Task<ReportResponseDto<GeneralDiaryBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var ledgerData = await GetDataledger(dto.Parameters);
            var lstDetail = await GetDetailData(ledgerData);

            var lstHeaderRow = ledgerData.DistinctBy(g => new
            {
                g.OrgCode,
                g.VoucherId,
                g.VoucherNumber,
                g.VoucherDate,
                g.VoucherCode,
                g.VoucherGroup,
                g.PartnerCode0,
                g.PartnerName0,
                g.Representative,
                g.Description
            }).Select(p => new GeneralDiaryBookDto()
            {
                OrgCode = p.OrgCode,
                Sort0 = 1,
                Sort1 = 1,
                VoucherDate = p.VoucherDate,
                VoucherId = p.VoucherId,
                VoucherNumber = p.VoucherNumber,
                Note = p.Description,
                Representation = p.Representative,
                PartnerCode = p.PartnerCode0,
                PartnerName = p.PartnerName0,
                Bold = "C",
                VoucherCode = p.VoucherCode
            }).ToList();

            var unionData = lstHeaderRow.Union(lstDetail);
            var result = unionData.OrderBy(p => p.Sort0)
                                .ThenBy(p => p.VoucherDate)
                                .ThenBy(p => p.VoucherNumber)
                                .ThenBy(p => p.VoucherId)
                                .ThenBy(p => p.Sort1)
                                .ThenBy(p => p.Acc).ToList();
            int row = 0;
            foreach (var item in result)
            {
                if (string.IsNullOrEmpty(item.Acc)) continue;
                row++;
                item.RowOrd = row;
                item.VoucherDate = null;
                item.VoucherNumber = null;
            }

            decimal? debitIncurredCur = lstDetail.Sum(p => p.DebitIncurredCur);
            decimal? debitIncurred = lstDetail.Sum(p => p.DebitIncurred);
            decimal? creditIncurredCur = lstDetail.Sum(p => p.CreditIncurredCur);
            decimal? creditIncurred = lstDetail.Sum(p => p.CreditIncurred);

            result.Add(new GeneralDiaryBookDto()
            {
                Sort0 = 2,
                Note = "Tổng cộng",
                DebitIncurredCur = debitIncurredCur,
                DebitIncurred = debitIncurred,
                CreditIncurredCur = creditIncurredCur,
                CreditIncurred = creditIncurred,
                Bold = "C"
            });

            var reportResponse = new ReportResponseDto<GeneralDiaryBookDto>();
            reportResponse.Data = result;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.GeneralDiaryBookReportPrint)]
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
        private async Task<List<LedgerGeneralDto>> GetDataledger(ReportBaseParameterDto dto)
        {
            var dic = new Dictionary<string, object>();
            dic.Add(LedgerParameterConst.LstOrgCode, _webHelper.GetCurrentOrgUnit());
            dic.Add(LedgerParameterConst.DebitOrCredit, "*");
            dic.Add(LedgerParameterConst.FromDate, dto.FromDate);
            dic.Add(LedgerParameterConst.ToDate, dto.ToDate);

            var data = await _reportDataService.GetLedgerData(dic);
            return data;
        }
        private async Task<List<GeneralDiaryBookDto>> GetDetailData(List<LedgerGeneralDto> ledgerData)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            var groupDetailData = ledgerData.GroupBy(g => new { g.OrgCode, g.VoucherId, g.VoucherDate, g.VoucherNumber, g.Acc, g.ReciprocalAcc })
                                    .Select(p => new GeneralDiaryBookDto()
                                    {
                                        OrgCode = p.Key.OrgCode,
                                        VoucherId = p.Key.VoucherId,
                                        VoucherDate = p.Key.VoucherDate,
                                        VoucherNumber = p.Key.VoucherNumber,
                                        Acc = p.Key.Acc,
                                        ReciprocalAcc = p.Key.ReciprocalAcc,
                                        DebitIncurredCur = p.Sum(x => x.DebitIncurredCur),
                                        DebitIncurred = p.Sum(x => x.DebitIncurred),
                                        CreditIncurredCur = p.Sum(x => x.CreditIncurredCur),
                                        CreditIncurred = p.Sum(x => x.CreditIncurred),
                                        Sort0 = 1,
                                        Sort1 = 2
                                    });

            var lstAccount = await _accountSystemService.GetAccounts(orgCode, year);

            var lstDetail = groupDetailData.Join(lstAccount,
                    p => p.Acc,
                    s => s.AccCode,
                    (p, s) => new GeneralDiaryBookDto()
                    {
                        OrgCode = p.OrgCode,
                        VoucherId = p.VoucherId,
                        VoucherDate = p.VoucherDate,
                        VoucherNumber = p.VoucherNumber,
                        Acc = p.Acc,
                        ReciprocalAcc = p.ReciprocalAcc,
                        DebitIncurredCur = p.DebitIncurredCur,
                        DebitIncurred = p.DebitIncurred,
                        CreditIncurredCur = p.CreditIncurredCur,
                        CreditIncurred = p.CreditIncurred,
                        Sort0 = 1,
                        Sort1 = 2,
                        Note = s.AccName
                    }).ToList();
            return lstDetail;
        }
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
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
