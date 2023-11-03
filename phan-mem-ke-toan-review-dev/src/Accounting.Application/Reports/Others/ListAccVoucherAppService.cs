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
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.Others
{
    public class ListAccVoucherAppService : AccountingAppService
    {
        #region Privates
        private readonly AccVoucherService _accVoucherService;
        private readonly WebHelper _webHelper;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CircularsService _circularsService;
        private readonly OrgUnitService _orgUnitService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ListAccVoucherAppService(AccVoucherService accVoucherService,
                            WebHelper webHelper,
                            YearCategoryService yearCategoryService,
                            CircularsService circularsService,
                            OrgUnitService orgUnitService,
                            TenantSettingService tenantSettingService,
                            IWebHostEnvironment hostingEnvironment,
                            ReportTemplateService reportTemplateService,
                            AccountingCacheManager accountingCacheManager
                )
        {
            _accVoucherService = accVoucherService;
            _webHelper = webHelper;
            _yearCategoryService = yearCategoryService;
            _circularsService = circularsService;
            _orgUnitService = orgUnitService;
            _tenantSettingService = tenantSettingService;
            _hostingEnvironment = hostingEnvironment;
            _reportTemplateService = reportTemplateService;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        [Authorize(ReportPermissions.ListAccVoucherReportView)]
        public async Task<ReportResponseDto<AccVoucherDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var queryable = await Filter(dto);
            var vouchers = await AsyncExecuter.ToListAsync(queryable);
            var dtos = vouchers.Select(p => ObjectMapper.Map<AccVoucher, AccVoucherDto>(p)).ToList();
            var reportResponse = new ReportResponseDto<AccVoucherDto>();
            reportResponse.Data = dtos;
            reportResponse.OrgUnit = await GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        [Authorize(ReportPermissions.ListAccVoucherReportPrint)]
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
        #region Privates
        private async Task<IQueryable<AccVoucher>> Filter(ReportRequestDto<ReportBaseParameterDto> dto)
        {
            var queryable = await _accVoucherService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit())
                                    && p.VoucherDate >= dto.Parameters.FromDate
                                    && p.VoucherDate <= dto.Parameters.ToDate);
            if (!string.IsNullOrEmpty(dto.Parameters.VoucherCode))
            {
                queryable = queryable.Where(p => dto.Parameters.VoucherCode.Contains(p.VoucherCode));
            }
            return queryable;
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
