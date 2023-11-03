using Accounting.BaseDtos;
using Accounting.Caching;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Windows;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Caching;
using Volo.Abp.ObjectMapping;

namespace Accounting.Reports
{
    public class ReportAppService : AccountingAppService, IReportAppService
    {
        #region Fields
        private readonly MenuAccountingService _menuAccountingService;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly ReportTemplateService _reportTemplateService;
        private readonly ReportTemplateColumnService _reportTemplateColumnService;
        private readonly InfoWindowService _infoWindowService;
        private readonly WebHelper _webHelper;
        private readonly TenantSettingService _tenantSettingService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly ReferenceService _referenceService;
        private readonly ReportMenuShortcutService _reportMenuShortcutService;
        private readonly UserService _userService;
        private readonly ButtonService _buttonService;
        private readonly IDistributedCache<ReportConfigDto> _cacheReportConfig;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public ReportAppService(MenuAccountingService menuAccountingService,
                        IPermissionDefinitionManager permissionDefinitionManager,
                        IStringLocalizer<AccountingResource> localizer,
                        ReportTemplateService reportTemplateService,
                        ReportTemplateColumnService reportTemplateColumnService,
                        InfoWindowService infoWindowService,
                        WebHelper webHelper,
                        TenantSettingService tenantSettingService,
                        DefaultTenantSettingService defaultTenantSettingService,
                        ReferenceService referenceService,
                        ReportMenuShortcutService reportMenuShortcutService,
                        UserService userService,
                        ButtonService buttonService,
                        IDistributedCache<ReportConfigDto> cacheReportConfig,
                        CacheManager cacheManager
            )
        {
            _menuAccountingService = menuAccountingService;
            _permissionDefinitionManager = permissionDefinitionManager;
            _localizer = localizer;
            _reportTemplateService = reportTemplateService;
            _reportTemplateColumnService = reportTemplateColumnService;
            _infoWindowService = infoWindowService;
            _webHelper = webHelper;
            _tenantSettingService = tenantSettingService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _referenceService = referenceService;
            _reportMenuShortcutService = reportMenuShortcutService;
            _userService = userService;
            _buttonService = buttonService;
            _cacheReportConfig = cacheReportConfig;
            _cacheManager = cacheManager;
        }
        #endregion
        #region Method Public
        public async Task<List<BaseComboItemDto>> GetByMenuId(string menuId)
        {
            var lstReport = new List<BaseComboItemDto>();

            var menu = await _menuAccountingService.GetAsync(menuId);
            var reportGroup = _permissionDefinitionManager.GetGroups()
                                .Where(p => p.Name.Equals(PermissionGroup.Report))
                                .FirstOrDefault();

            var permissionDefines = reportGroup.Permissions.Where(p => p.Name.Equals(menu.ViewPermission))
                                    .FirstOrDefault();
            if (permissionDefines == null) return lstReport;
            string[] viewPermissions = await _userService.GetViewPermissions();
            foreach (var reportPermission in permissionDefines.Children)
            {
                var viewPermission = reportPermission.Children
                                        .Where(p => p.Name.EndsWith($"_{ActionType.View}"))
                                        .FirstOrDefault();
                if (viewPermission == null) continue;
                if (!viewPermissions.Contains(viewPermission.Name)) continue;
                lstReport.Add(new BaseComboItemDto()
                {
                    Id = reportPermission.Name,
                    Value = _localizer[reportPermission.Name],
                    Code = reportPermission.Name,
                    Display = _localizer[reportPermission.Name],
                    Name = _localizer[reportPermission.Name]
                });
            }
            return lstReport;
        }
        public async Task<ReportConfigDto> GetConfigAsync(string reportCode)
        {
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<ReportConfigDto>(reportCode,false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var reportTemplate = await _reportTemplateService.GetByCodeAsync(reportCode);
                    var reportColumns = await _reportTemplateColumnService.GetByReportTemplateIdAsync(reportTemplate.Id);
                    var config = new ReportConfigDto();
                    config.ReportTemplateColumns = reportColumns.Select(p => ObjectMapper.Map<ReportTemplateColumn, ReportTemplateColumnDto>(p))
                                                    .ToList();
                    config.ReportTemplate = ObjectMapper.Map<ReportTemplate, ReportTemplateDto>(reportTemplate);
                    config.InfoWindow = await GetInfoWindow(reportTemplate.InfoWindowId);
                    config.SymbolSeparateNumber = await GetSymbolSeparateNumber();
                    config.CurrencyFormats = await GetCurrencyFormats();
                    config.References = await GetReferences(config.InfoWindow);
                    config.ReportMenuShortcuts = await GetMenuShortcutDtos(reportTemplate.Id);
                    config.Buttons = await GetButtons(reportTemplate.Id);
                    return config;
                }
            );                       
        }
        public async Task<ReportConfigDto> GetConfigByIdAsync(string reportId)
        {
            var reportTemplate = await _reportTemplateService.GetAsync(reportId);
            return await GetConfigAsync(reportTemplate.Code);
        }
        public async Task<ResultDto> CheckPermission(JsonObject parameter)
        {
            string reportCode = parameter["reportCode"].ToString();
            string action = parameter["action"].ToString();
            var resultDto = new ResultDto();
            var reportTemplate = await _reportTemplateService.GetByCodeAsync(reportCode);
            if (reportTemplate == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ReportTemplate, ErrorCode.NotFoundEntity),
                        $"ReportTeamplate Code = ['{reportCode}'] already exist ");
            }
            string permissionName = action switch
            {
                "view" => $"{reportCode}_View",
                "print" => $"{reportCode}_Print",
                "exportExcel" => $"{reportCode}_ExportExcel",
                _ => null
            };
            if (string.IsNullOrEmpty(permissionName))
            {
                throw new ArgumentNullException(_localizer["actionNotValid"]);
            }
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            if (result.Succeeded == false)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
            resultDto.Ok = true;
            return resultDto;
        }
        #endregion
        #region Private
        private async Task<InfoWindowDto> GetInfoWindow(string infoWindowId)
        {
            var infoWindow = await _infoWindowService.GetWithDetailByIdAsync(infoWindowId);
            if (infoWindow == null) return null;
            return ObjectMapper.Map<InfoWindow, InfoWindowDto>(infoWindow);
        }
        private async Task<Dictionary<string, string>> GetSymbolSeparateNumber()
        {
            var dict = new Dictionary<string, string>();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await _tenantSettingService.GetNumberSeparateSymbol(orgCode);
            if (currencyFormats.Count == 0)
            {
                return await this.GetDefaultNumberSeparateSymbol();
            }            
            foreach (var item in currencyFormats)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        private async Task<Dictionary<string, string>> GetCurrencyFormats()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await _tenantSettingService.GetListFormatNumberFields(orgCode);
            if (currencyFormats.Count == 0)
            {
                return await this.GetDefaultCurrencyFormat();
            }
            var dict = new Dictionary<string, string>();
            foreach (var item in currencyFormats)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        private async Task<Dictionary<string, ReferenceDto>> GetReferences(InfoWindowDto window)
        {
            var dic = new Dictionary<string, ReferenceDto>();
            var referenceIds = window.InfoWindowDetails.Where(p => !string.IsNullOrEmpty(p.ReferenceId))
                                        .Select(p => p.ReferenceId).ToArray();
            foreach (var id in referenceIds)
            {
                if (dic.ContainsKey(id)) continue;
                var reference = await _referenceService.GetWithDetailAsync(id);
                reference.ReferenceDetails = reference.ReferenceDetails.OrderBy(p => p.Ord).ToList();
                dic.Add(id, ObjectMapper.Map<Reference, ReferenceDto>(reference));
            }
            return dic;
        }
        private async Task<List<ReportMenuShortcutDto>> GetMenuShortcutDtos(string reportId)
        {
            var menus = await _reportMenuShortcutService.GetByReportIdAsync(reportId);
            return menus.Select(p => ObjectMapper.Map<ReportMenuShortcut, ReportMenuShortcutDto>(p))
                        .ToList();
        }
        private async Task<Dictionary<string, string>> GetDefaultNumberSeparateSymbol()
        {
            var dict = new Dictionary<string, string>();
            var defaultTenantSettings = await _defaultTenantSettingService.GetNumberSeparateSymbol();
            foreach (var item in defaultTenantSettings)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        private async Task<Dictionary<string, string>> GetDefaultCurrencyFormat()
        {
            var dict = new Dictionary<string, string>();
            var defaultTenantSettings = await _defaultTenantSettingService.GetListFormatNumberFields();
            foreach (var item in defaultTenantSettings)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        private async Task<List<ButtonDto>> GetButtons(string reportId)
        {
            var buttons = await _buttonService.GetByReportIdAsync(reportId);
            return buttons.Select(p => ObjectMapper.Map<Button, ButtonDto>(p)).ToList();
        }
        #endregion
    }
}
