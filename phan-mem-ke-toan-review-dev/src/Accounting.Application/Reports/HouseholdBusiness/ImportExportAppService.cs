using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Products;
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
using Accounting.Reports.ImportExports;
using Accounting.Reports.ImportExports.Parameters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.HouseholdBusiness
{
    public class ImportExportAppService : AccountingAppService
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
        private readonly ProductGroupAppService _productGroupAppService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService;
        private readonly ProductLotService _productLotService;
        private readonly OrgUnitService _orgUnitService;        
        private readonly CircularsService _circularsService;
        private readonly AccPartnerService _accPartnerService;
        private readonly ImportExportBusiness _importExportBusiness;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ImportExportAppService(ReportDataService reportDataService,
                        WebHelper webHelper,
                        ReportTemplateService reportTemplateService,
                        IWebHostEnvironment hostingEnvironment,
                        YearCategoryService yearCategoryService,
                        AccountSystemService accountSystemService,
                        TenantSettingService tenantSettingService,
                        VoucherCategoryService voucherCategoryService,
                        ProductGroupAppService productGroupAppService,
                        WarehouseService warehouseService,
                        ProductService productService,
                        ProductLotService productLotService,
                        OrgUnitService orgUnitService,
                        CircularsService circularsService,
                        AccPartnerService accPartnerService,
                        ImportExportBusiness importExportBusiness,
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
            _productGroupAppService = productGroupAppService;
            _warehouseService = warehouseService;
            _productService = productService;
            _productLotService = productLotService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _accPartnerService = accPartnerService;
            _importExportBusiness = importExportBusiness;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.ImportExportHkdReportView)]
        public async Task<ReportResponseDto<InventorySummaryBookDto>> CreateDataAsync(ReportRequestDto<InventorySummaryBookParameterDto> dto)
        {
            return await _importExportBusiness.CreateDataAsync(dto);
        }
        [Authorize(ReportPermissions.ImportExportHkdReportPrint)]
        public async Task<FileContentResult> PrintAsync(ReportRequestDto<InventorySummaryBookParameterDto> dto)
        {
            var dataSource = await _importExportBusiness.CreateDataAsync(dto);
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
