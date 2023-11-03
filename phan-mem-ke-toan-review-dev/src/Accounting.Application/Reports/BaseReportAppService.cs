using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.Circularses;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Reports;
using Accounting.Helpers;
using Accounting.Localization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;

namespace Accounting.Reports
{
    public class BaseReportAppService : AccountingAppService
    {
        #region Privates        
        protected readonly WebHelper _webHelper;
        protected readonly ReportTemplateService _reportTemplateService;
        protected readonly IWebHostEnvironment _hostingEnvironment;
        protected readonly YearCategoryService _yearCategoryService;
        protected readonly TenantSettingService _tenantSettingService;
        protected readonly OrgUnitService _orgUnitService;
        protected readonly CircularsService _circularsService;
        protected readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctors
        public BaseReportAppService(
                    WebHelper webHelper,
                    ReportTemplateService reportTemplateService,
                    IWebHostEnvironment hostingEnvironment,
                    YearCategoryService yearCategoryService,
                    TenantSettingService tenantSettingService,
                    OrgUnitService orgUnitService,
                    CircularsService circularsService,
                    IStringLocalizer<AccountingResource> localizer
            )
        {
            _webHelper = webHelper;
            _reportTemplateService = reportTemplateService;
            _hostingEnvironment = hostingEnvironment;
            _yearCategoryService = yearCategoryService;
            _tenantSettingService = tenantSettingService;
            _orgUnitService = orgUnitService;
            _circularsService = circularsService;
            _localizer = localizer;
        }
        #endregion
        #region Privates       
        protected async Task<dynamic> GetTenantSetting(string orgCode)
        {
            dynamic exo = new System.Dynamic.ExpandoObject();

            var tenantSettings = await _tenantSettingService.GetBySettingTypeAsync(orgCode, TenantSettingType.Report);
            foreach (var setting in tenantSettings)
            {
                ((IDictionary<String, Object>)exo).Add(setting.Key, setting.Value);
            }
            return exo;
        }
        protected string GetFileTemplatePath(string templateFile)
        {
            string rootPath = _hostingEnvironment.WebRootPath;
            string lang = _webHelper.GetCurrentLanguage();
            string filePath = Path.Combine(rootPath, FolderConst.ReportTemplate, lang, FolderConst.Report,
                                        templateFile);
            return filePath;
        }
        protected async Task<CircularsDto> GetCircularsDto(string orgCode, int year)
        {
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);
            if (yearCategory == null) return null;

            var usingDecision = yearCategory.UsingDecision;
            if (usingDecision == null) return null;

            var circulars = await _circularsService.GetByCodeAsync(usingDecision.Value.ToString());
            return ObjectMapper.Map<Circulars, CircularsDto>(circulars);
        }
        protected async Task<OrgUnitDto> GetOrgUnit(string code)
        {
            var orgUnit = await _orgUnitService.GetByCodeAsync(code);
            return ObjectMapper.Map<OrgUnit, OrgUnitDto>(orgUnit);
        }
        protected async Task<ReportResponseDto<T>> CreateReportResponseDto<T>(List<T> data, ReportRequestDto<ReportBaseParameterDto> dto) 
                            where T : class
        {
            var reportResponse = new ReportResponseDto<T>();
            reportResponse.Data = data;
            reportResponse.OrgUnit = await this.GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await this.GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        protected async Task<ReportResponseDto<T>> CreateReportResponseDto<T,V>(List<T> data, ReportRequestDto<V> dto)
                            where T : class
                            where V : class
        {
            var reportResponse = new ReportResponseDto<T>();
            reportResponse.Data = data;
            reportResponse.OrgUnit = await this.GetOrgUnit(_webHelper.GetCurrentOrgUnit());
            reportResponse.RequestParameter = dto.Parameters;
            reportResponse.TenantSetting = await this.GetTenantSetting(_webHelper.GetCurrentOrgUnit());
            reportResponse.Circulars = await GetCircularsDto(_webHelper.GetCurrentOrgUnit(),
                                    _webHelper.GetCurrentYear());
            return reportResponse;
        }
        protected async Task CheckPermission(string reportCode,string action)
        {
            bool isGrant = await this.IsGrantPermission(reportCode, action);
            if (!isGrant)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
        }
        private async Task<bool> IsGrantPermission(string reportCode, string action)
        {
            string permissionName = $"{reportCode}_{action}";
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
        #endregion
    }
}
