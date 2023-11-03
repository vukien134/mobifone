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
    public class RecordingVoucherReportAppService : AccountingAppService
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
        public RecordingVoucherReportAppService(ReportDataService reportDataService,
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
        [Authorize(ReportPermissions.RecordingVoucherReportView)]
        public async Task<ReportResponseDto<RecordingVoucherReportDto>> CreateDataAsync(ReportRequestDto<RecordingVoucherReportParameterDto> dto)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var lst = new List<RecordingVoucherReportDto>();
            var accRank = await _accountSystemService.GetAccountByRank(dto.Parameters.RankAccCode, _webHelper.GetCurrentOrgUnit(), _webHelper.GetCurrentYear());
            var recordingVoucherBook = await _recordingVoucherBookService.GetQueryableAsync();
            var lstRecordingVoucherBook = recordingVoucherBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var ledger = await _ledgerService.GetQueryableAsync();
            ledger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                       && String.Compare(p.Status, "2") < 0
                                       && p.VoucherDate >= dto.Parameters.FromDate
                                       && p.VoucherDate <= dto.Parameters.ToDate
                                       && (p.RecordingVoucherNumber ?? "") != ""
                                       && ((dto.Parameters.FromNumber ?? "") == "" || String.Compare(p.RecordingVoucherNumber, dto.Parameters.FromNumber) >= 0)
                                       && ((dto.Parameters.ToNumber ?? "") == "" || String.Compare(p.RecordingVoucherNumber, dto.Parameters.ToNumber) <= 0)
                                       );
            var incurredData = (from a in ledger
                               group a by new
                               {
                                   a.OrgCode,
                                   a.RecordingVoucherNumber,
                                   a.DebitAcc,
                                   a.CreditAcc
                               } into gr
                               select new RecordingVoucherReportDto
                               {
                                   OrgCode = gr.Key.OrgCode,
                                   RecordingVoucherNumber = gr.Key.RecordingVoucherNumber,
                                   DebitAcc = gr.Key.DebitAcc,
                                   CreditAcc = gr.Key.CreditAcc,
                                   Description = "",
                                   Incurred = gr.Sum(p => p.Amount ?? 0),
                               }).ToList();
            var debitData = (from a in incurredData
                            join b in accRank on a.OrgCode equals b.OrgCode
                            where a.DebitAcc.StartsWith(b.AccCode)
                            group new { a, b} by new
                            {
                                a.OrgCode,
                                a.DebitAcc,
                                a.Year
                            } into gr
                            select new RecordingVoucherReportDto
                            {
                                OrgCode = gr.Key.OrgCode,
                                Year = gr.Key.Year,
                                DebitAcc = gr.Key.DebitAcc,
                                Acc = gr.Max(p => p.b.AccCode),
                            }).ToList();
            var creditData = (from a in incurredData
                             join b in accRank on a.OrgCode equals b.OrgCode
                             where a.CreditAcc.StartsWith(b.AccCode)
                             group new { a, b } by new
                             {
                                 a.OrgCode,
                                 a.CreditAcc,
                                 a.Year
                             } into gr
                             select new RecordingVoucherReportDto
                             {
                                 OrgCode = gr.Key.OrgCode,
                                 Year = gr.Key.Year,
                                 CreditAcc = gr.Key.CreditAcc,
                                 Acc = gr.Max(p => p.b.AccCode),
                             }).ToList();
            foreach (var item in incurredData)
            {
                var debitAcc = debitData.Where(p => p.DebitAcc == item.DebitAcc).FirstOrDefault();
                var creditAcc = creditData.Where(p => p.CreditAcc == item.CreditAcc).FirstOrDefault();
                if (debitAcc != null && creditAcc != null)
                {
                    var itemRecordingVoucherBook = lstRecordingVoucherBook.Where(p => p.VoucherNumber == item.RecordingVoucherNumber).FirstOrDefault();
                    var acc1 = accRank.Where(p => p.AccCode == debitAcc.Acc).FirstOrDefault();
                    var acc2 = accRank.Where(p => p.AccCode == creditAcc.Acc).FirstOrDefault();
                    item.DebitAcc = debitAcc.Acc;
                    item.CreditAcc = creditAcc.Acc;
                    item.Description = ((itemRecordingVoucherBook?.TypeDescription ?? 1) == 1) ? acc1.AccName : acc2.AccName;
                    item.DescriptionRV = itemRecordingVoucherBook?.Description ?? "";
                }
            }
            lst = incurredData.GroupBy(g => new 
                                            { 
                                                g.OrgCode, 
                                                g.Year,
                                                g.RecordingVoucherNumber,
                                                g.Description,
                                                g.DescriptionRV,
                                                g.DebitAcc,
                                                g.CreditAcc,
                                            }).Select(p => new RecordingVoucherReportDto 
                                            {
                                                Bold = "K",
                                                Sort0 = "A",
                                                Sort1 = "B",
                                                OrgCode = p.Key.OrgCode,
                                                RecordingVoucherNumber = p.Key.RecordingVoucherNumber,
                                                DebitAcc = p.Key.DebitAcc,
                                                CreditAcc = p.Key.CreditAcc,
                                                Description = p.Key.Description,
                                                DescriptionRV = p.Key.DescriptionRV,
                                                Incurred = p.Sum(p => p.Incurred ?? 0),
                                            }).ToList();
            var dataGroup = lst.GroupBy(g => new
                                                {
                                                    g.OrgCode,
                                                    g.Year,
                                                    g.RecordingVoucherNumber,
                                                    g.DescriptionRV,
                                                }).Select(p => new RecordingVoucherReportDto
                                                {
                                                    OrgCode = p.Key.OrgCode,
                                                    Year = p.Key.Year,
                                                    RecordingVoucherNumber = p.Key.RecordingVoucherNumber,
                                                    DescriptionRV = p.Key.DescriptionRV,
                                                    Incurred = p.Sum(p => p.Incurred ?? 0),
                                                }).ToList();
            lst.AddRange(dataGroup.Select(p => new RecordingVoucherReportDto 
                                                {
                                                    Bold = "C",
                                                    Sort0 = "A",
                                                    Sort1 = "A",
                                                    OrgCode = p.OrgCode,
                                                    RecordingVoucherNumber = p.RecordingVoucherNumber,
                                                    DebitAcc = p.DebitAcc,
                                                    CreditAcc = p.CreditAcc,
                                                    Description = p.DescriptionRV,
                                                    DescriptionRV = p.DescriptionRV,
                                                    Incurred = p.Incurred ?? 0,
                                                }).ToList());
            lst.AddRange(dataGroup.Select(p => new RecordingVoucherReportDto
                                                {
                                                    Bold = "C",
                                                    Sort0 = "A",
                                                    Sort1 = "C",
                                                    OrgCode = p.OrgCode,
                                                    Year = p.Year,
                                                    RecordingVoucherNumber = p.RecordingVoucherNumber,
                                                    DebitAcc = p.DebitAcc,
                                                    CreditAcc = p.CreditAcc,
                                                    Description = "Tổng cộng",
                                                    DescriptionRV = p.DescriptionRV,
                                                    Incurred = p.Incurred ?? 0,
                                                }).ToList());
            lst.AddRange(dataGroup.GroupBy(g => new
            {
                g.OrgCode,
                g.Year,
            }).Select(p => new RecordingVoucherReportDto
            {
                Bold = "C",
                Sort0 = "B",
                Sort1 = "A",
                OrgCode = p.Key.OrgCode,
                Year = p.Key.Year,
                Description = "TỔNG CỘNG",
                Incurred = p.Sum(p => p.Incurred ?? 0),
            }).ToList());
            var reportResponse = new ReportResponseDto<RecordingVoucherReportDto>();
            reportResponse.Data = lst.OrderBy(p => p.OrgCode).ThenBy(p => p.Year).ThenBy(p => p.Sort0)
                                                             .ThenBy(p => p.RecordingVoucherNumber).ThenBy(p => p.Sort1)
                                                             .ThenBy(p => p.DebitAcc).ThenBy(p => p.CreditAcc).ToList();
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.RecordingVoucherReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<RecordingVoucherReportParameterDto> dto)
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

