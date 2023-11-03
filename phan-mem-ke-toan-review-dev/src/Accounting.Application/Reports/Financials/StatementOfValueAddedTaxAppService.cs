using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Report;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.Financials;
using Accounting.Reports.Financials.StatementOfValueAddedTax;
using Accounting.Reports.GeneralDiaries;
using Accounting.Reports.Tenants;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports.Financials
{
    public class StatementOfValueAddedTaxAppService : AccountingAppService
    {
        #region Fields
        private readonly ReportDataService _reportDataService;
        private readonly WebHelper _webHelper;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly YearCategoryService _yearCategoryService;
        private readonly TenantStatementTaxDataService _tenantStatementTaxDataService;
        private readonly DefaultStatementTaxService _defaultStatementTaxService;
        private readonly TenantStatementTaxService _tenantStatementTaxService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly DefaultAccBalanceSheetService _defaultAccBalanceSheetService;
        private readonly DefaultCashFollowStatementService _defaultCashFollowStatementService;
        private readonly TenantAccBalanceSheetService _tenantAccBalanceSheetService;
        private readonly TenantCashFollowStatementService _tenantCashFollowStatementService;
        private readonly OrgUnitService _orgUnitService;
        private readonly StatementOfValueAddedBusiness _statementOfValueAddedBusiness;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountingCacheManager _accountingCacheManager;

        #endregion
        public StatementOfValueAddedTaxAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        TenantStatementTaxDataService tenantStatementTaxDataService,
                        DefaultStatementTaxService defaultStatementTaxService,
                        TenantStatementTaxService tenantStatementTaxService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        DefaultAccBalanceSheetService defaultAccBalanceSheetService,
                        DefaultCashFollowStatementService defaultCashFollowStatementService,
                        TenantAccBalanceSheetService tenantAccBalanceSheetService,
                        TenantCashFollowStatementService tenantCashFollowStatementService,
                        OrgUnitService orgUnitService,
                        StatementOfValueAddedBusiness statementOfValueAddedBusiness,
                        AccTaxDetailService accTaxDetailService,
                        TaxCategoryService taxCategoryService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        AccountingCacheManager accountingCacheManager)
        {
            _reportDataService = reportDataService;
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _tenantStatementTaxDataService = tenantStatementTaxDataService;
            _defaultStatementTaxService = defaultStatementTaxService;
            _tenantStatementTaxService = tenantStatementTaxService;
            _accountSystemService = accountSystemService;
            _tenantSettingService = tenantSettingService;
            _defaultAccBalanceSheetService = defaultAccBalanceSheetService;
            _defaultCashFollowStatementService = defaultCashFollowStatementService;
            _tenantAccBalanceSheetService = tenantAccBalanceSheetService;
            _tenantCashFollowStatementService = tenantCashFollowStatementService;
            _orgUnitService = orgUnitService;
            _statementOfValueAddedBusiness = statementOfValueAddedBusiness;
            _accTaxDetailService = accTaxDetailService;
            _taxCategoryService = taxCategoryService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _accountingCacheManager = accountingCacheManager;
        }
        #region Methods
        [Authorize(ReportPermissions.VatStatementReportView)]
        public async Task<ReportResponseDto<DataStatementTaxDto>> CreateDataAsync(ReportRequestDto<StatementOfValueAddedTaxParameterDto> dto)
        {
            return await _statementOfValueAddedBusiness.CreateDataAsync(dto);
        }
        [Authorize(ReportPermissions.VatStatementReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<StatementOfValueAddedTaxParameterDto> dto)
        {
            var dataSource = await _statementOfValueAddedBusiness.CreateDataAsync(dto);
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

        public async Task<TenantStatementTaxDataDto> PostGetFilter(StatementOfValueAddedTaxParameterDto dto)
        {
            var statementTaxData = await _tenantStatementTaxDataService.GetQueryableAsync();
            var lstStatementTaxData = statementTaxData.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var itemStatementTaxDataPre = lstStatementTaxData.Where(p => p.BeginDate == null && p.EndDate == dto.FromDate).Select(p => ObjectMapper.Map<TenantStatementTaxData, TenantStatementTaxDataDto>(p)).FirstOrDefault();
            var itemStatementTaxData = lstStatementTaxData.Where(p => p.BeginDate == dto.FromDate && p.EndDate == dto.ToDate).Select(p => ObjectMapper.Map<TenantStatementTaxData, TenantStatementTaxDataDto>(p)).FirstOrDefault();
            if (itemStatementTaxData == null)
            {
                itemStatementTaxData = new TenantStatementTaxDataDto()
                {
                    BeginDate = dto.FromDate,
                    EndDate = dto.ToDate,
                    DeductPre = itemStatementTaxDataPre?.DeductPre ?? 0,
                    IncreasePre = 0,
                    ReducePre = 0,
                    SuggestionReturn = 0,
                    OrgCode = _webHelper.GetCurrentOrgUnit(),
                    Id = GetNewObjectId(),
                };
            }
            return itemStatementTaxData;
        }
        #endregion
        #region Private
        private string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate,lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        #endregion
    }
}

