using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts;
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
using Accounting.Reports.DebitBooks;
using Accounting.Reports.Financials;
using Accounting.Reports.GeneralDiaries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports.Financials
{
    public class AccBalanceSheetReportAppService : AccountingAppService
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
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly OrgUnitService _orgUnitService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccBalanceSheetReportBusiness _accBalanceSheetReportBusiness;
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion
        public AccBalanceSheetReportAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                        TenantAccBalanceSheetService tenantAccBalanceSheetService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccBalanceSheetReportBusiness accBalanceSheetReportBusiness,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accBalanceSheetReportBusiness = accBalanceSheetReportBusiness;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.AccBalanceSheetReportView)]
        public async Task<ReportResponseDto<AccBalanceSheetDto>> CreateDataAsync(ReportRequestDto<AccBalanceSheetParameterDto> dto)
        {
            return await _accBalanceSheetReportBusiness.CreateDataAsync(dto);
        }
        [Authorize(ReportPermissions.AccBalanceSheetReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<AccBalanceSheetParameterDto> dto)
        {
            var dataSource = await _accBalanceSheetReportBusiness.CreateDataAsync(dto);
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
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        #endregion
    }
}

