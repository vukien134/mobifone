using Accounting.BaseDtos;
using Accounting.Caching;
using Accounting.Categories.Menus;
using Accounting.Catgories.Menus;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Reports;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Windows;
using Accounting.EntityFrameworkCore;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Reports;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Caching;
using Microsoft.Extensions.Configuration;

namespace Accounting.Windows
{
    public class WindowAppService : AccountingAppService, IWindowAppService
    {
        #region Fields
        private readonly ILogger<WindowAppService> _logger;
        private readonly WindowService _windowService;
        private readonly UserService _userService;
        private readonly MenuAccountingService _menuAccountingService;
        private readonly ReferenceService _referenceService;
        private readonly EventSettingService _eventSettingService;
        private readonly VoucherTemplateService _voucherTemplateService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly TenantSettingService _tenantSettingService;
        private readonly IConfiguration _config;
        private readonly WebHelper _webHelper;
        private readonly TabService _tabService;
        private readonly FieldService _fieldService;
        private readonly InfoWindowService _infoWindowService;
        private readonly ButtonService _buttonService;
        private readonly AccountingDb _accountingDb;
        private readonly ReportMenuShortcutService _reportMenuShortcutService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly IDistributedCache<WindowConfigDto> _cacheWinConfig;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public WindowAppService(ILogger<WindowAppService> logger,
                        WindowService windowService,
                        UserService userService,
                        MenuAccountingService menuAccountingService,
                        ReferenceService referenceService,
                        EventSettingService eventSettingService,
                        VoucherTemplateService voucherTemplateService,                        
                        IStringLocalizer<AccountingResource> localizer,
                        TenantSettingService tenantSettingService,
                        IConfiguration config,
                        WebHelper webHelper,
                        TabService tabService,
                        FieldService fieldService,
                        InfoWindowService infoWindowService,
                        ButtonService buttonService,
                        AccountingDb accountingDb,
                        ReportMenuShortcutService reportMenuShortcutService,
                        DefaultTenantSettingService defaultTenantSettingService,
                        IDistributedCache<WindowConfigDto> cacheWinConfig,
                        CacheManager cacheManager
                        )
        {
            _logger = logger;
            _windowService = windowService;
            _userService = userService;
            _menuAccountingService = menuAccountingService;
            _referenceService = referenceService;
            _eventSettingService = eventSettingService;
            _voucherTemplateService = voucherTemplateService;
            _localizer = localizer;
            _tenantSettingService = tenantSettingService;
            _config = config;
            _webHelper = webHelper;
            _tabService = tabService;
            _fieldService = fieldService;
            _infoWindowService = infoWindowService;
            _buttonService = buttonService;
            _accountingDb = accountingDb;
            _reportMenuShortcutService = reportMenuShortcutService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _cacheWinConfig = cacheWinConfig;
            _cacheManager = cacheManager;
        }
        #endregion
        public async Task<WindowDto> CreateAsync(CrudWindowDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            var entity = ObjectMapper.Map<CrudWindowDto, Window>(dto);
            var result = await _windowService.CreateAsync(entity);
            return ObjectMapper.Map<Window, WindowDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _windowService.DeleteAsync(id);
        }
        
        public async Task<PagedResultDto<WindowDto>> GetListAsync(PagedAndSortedResultRequestDto dto)
        {
            var result = new PagedResultDto<WindowDto>();
            var query = await _windowService.GetQueryableAsync();
            
            var querysort = query.OrderBy(p => p.Code).Skip(dto.SkipCount).Take(dto.MaxResultCount);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Items = sections.Select(p => ObjectMapper.Map<Window, WindowDto>(p)).ToList();
            
            return result;
        }

        public async Task UpdateAsync(string id, CrudWindowDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();            
            var entity = await _windowService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _windowService.UpdateAsync(entity);
        }
        public async Task<WindowConfigDto> GetConfigByCodeAsync(string code)
        {
            var window = await _windowService.GetByCodeAsync(code);
            if (window == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Window, ErrorCode.NotFoundEntity),
                        $"Window Code ['{code}'] not found ");
            }

            return await GetConfigByIdAsync(window.Id);
        }
        public async Task<WindowConfigDto> GetConfigByVoucherCodeAsync(string code)
        {
            var window = await _windowService.GetByVoucherCodeAsync(code);
            if (window == null)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Window, ErrorCode.NotFoundEntity),
                        $"Window with VoucherCode = ['{code}'] not found ");
            }

            return await GetConfigByIdAsync(window.Id);
        }
        public async Task<WindowConfigDto> GetConfigByIdAsync(string windowId)
        {
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<WindowConfigDto>(windowId,false);
            var configDto = await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var config = new WindowConfigDto();
                    var menu = await _menuAccountingService.GetByWindowIdAsync(windowId);
                    var window = await _windowService.GetWithDetailAsync(windowId);

                    if (menu != null)
                    {
                        config.Menu = ObjectMapper.Map<MenuAccounting, MenuAccountingDto>(menu);
                        config.Menu.Name = _localizer[config.Menu.Name];
                        config.Menu.Detail = _localizer[config.Menu.Detail];
                    }

                    config.Window = ObjectMapper.Map<Window, WindowDto>(window);
                    config.References = await GetReferences(window);
                    config.WindowEvents = await GetWindowEvents(windowId);
                    config.VoucherTemplates = await GetVoucherTemplateDtos(windowId);
                    config.CurrencyFormats = await GetCurrencyFormats();
                    config.SymbolSeparateNumber = await GetSymbolSeparateNumber();
                    config.TabEvents = await GetTabEvents(config.Window);
                    config.FieldEvents = await GetFieldEvents(config.Window);
                    config.InfoWindow = await GetInfoWindow(config.Window.InfoWindowId);
                    config.InfoWindowReferences = await GetInfoWindowReferences(config.InfoWindow);
                    config.Buttons = await GetButtons(windowId);
                    config.ReportMenuShortcutDtos = await this.GetReportMenuShortcutDtos(windowId);
                    return config;
                }
            );
            configDto = await LoadDataReference(configDto);
            return configDto;
        }
        public async Task<WindowDto> GetByIdAsync(string windowId)
        {
            var window = await _windowService.GetAsync(windowId);
            return ObjectMapper.Map<Window, WindowDto>(window);
        }
        public async Task<WindowConfigDto> GetFormatNumber()
        {
            var config = new WindowConfigDto();
            config.CurrencyFormats = await GetCurrencyFormats();
            config.SymbolSeparateNumber = await GetSymbolSeparateNumber();

            return config;
        }
        public async Task Copy(string windowId,string code,string name)
        {
            await _windowService.Copy(windowId, code, name);
        }
        public async Task<WindowDto> GetByVoucherCodeAsync(string code)
        {
            var queryable = await _windowService.GetQueryableAsync();
            queryable = queryable.Where(p => p.VoucherCode.Equals(code));
            var window = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (window != null)
            {
                return ObjectMapper.Map<Window, WindowDto>(window);
            }
            return null;
        }
        #region Private
        private async Task<IQueryable<Window>> Filter(PageRequestDto dto)
        {
            var queryable = await _windowService.GetQueryableAsync();
            return queryable;
        }
        private async Task<Dictionary<string,ReferenceDto>> GetReferences(Window window)
        {
            var dic = new Dictionary<string, ReferenceDto>();
            foreach (var tab in window.Tabs)
            {
                var referenceIds = tab.Fields.Where(p => p.ReferenceId != null)
                                        .Select(p => p.ReferenceId).ToArray();
                foreach (var id in referenceIds)
                {
                    if (dic.ContainsKey(id)) continue;
                    var reference = await _referenceService.GetWithDetailAsync(id);
                    reference.ReferenceDetails = reference.ReferenceDetails.OrderBy(p => p.Ord).ToList();
                    var dto = ObjectMapper.Map<Reference, ReferenceDto>(reference);                    
                    dic.Add(id, dto);
                    
                }
            }
            return dic;
        }
        private async Task<Dictionary<string,string>> GetWindowEvents(string windowId)
        {
            var registerEvents = await _windowService.GetRegisterEventAsync(windowId);
            if (registerEvents.Count == 0) return null;

            var dict = new Dictionary<string, string>();
            foreach(var item in registerEvents)
            {
                var eventSetting = await _eventSettingService.GetAsync(item.EventSettingId);
                if (dict.ContainsKey(eventSetting.TypeEvent)) continue;

                dict.Add(eventSetting.TypeEvent, eventSetting.Content);
            }
            return dict;
        }
        private async Task<Dictionary<string, Dictionary<string, string>>> GetTabEvents(WindowDto window)
        {
            var dict = new Dictionary<string, Dictionary<string,string>>();

            foreach(var tab in window.Tabs)
            {
                var registerEvents = await _tabService.GetRegisterEventAsync(tab.Id);
                if (registerEvents.Count == 0)  continue;

                var events = new Dictionary<string, string>();
                foreach (var item in registerEvents)
                {
                    var eventSetting = await _eventSettingService.GetAsync(item.EventSettingId);
                    if (events.ContainsKey(eventSetting.TypeEvent)) continue;

                    events.Add(eventSetting.TypeEvent, eventSetting.Content);
                }

                dict.Add(tab.Id, events);
            }            
            
            return dict;
        }
        private async Task<Dictionary<string, Dictionary<string, string>>> GetFieldEvents(WindowDto window)
        {
            var dict = new Dictionary<string, Dictionary<string, string>>();

            foreach (var tab in window.Tabs)
            {
                foreach(var field in tab.Fields)
                {
                    var registerEvents = await _fieldService.GetRegisterEventAsync(field.Id);
                    if (registerEvents.Count == 0) continue;

                    var events = new Dictionary<string, string>();
                    foreach (var item in registerEvents)
                    {
                        var eventSetting = await _eventSettingService.GetAsync(item.EventSettingId);
                        if (events.ContainsKey(eventSetting.TypeEvent)) continue;

                        events.Add(eventSetting.TypeEvent, eventSetting.Content);
                    }

                    dict.Add(field.Id, events);
                }
                
            }

            return dict;
        }
        private async Task<List<VoucherTemplateDto>> GetVoucherTemplateDtos(string windowId)
        {
            var voucherTemplates = await _voucherTemplateService.GetByWindowIdAsync(windowId);
            return voucherTemplates.Select(p => ObjectMapper.Map<VoucherTemplate, VoucherTemplateDto>(p))
                                .ToList();
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
        private async Task<Dictionary<string, string>> GetSymbolSeparateNumber()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await _tenantSettingService.GetNumberSeparateSymbol(orgCode);
            if (currencyFormats.Count == 0)
            {
                return await this.GetDefaultNumberSeparateSymbol();
            }
            var dict = new Dictionary<string, string>();
            foreach (var item in currencyFormats)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        private async Task<InfoWindowDto> GetInfoWindow(string infoWindowId)
        {
            var infoWindow = await _infoWindowService.GetWithDetailByIdAsync(infoWindowId);
            if (infoWindow == null) return null;
            return ObjectMapper.Map<InfoWindow, InfoWindowDto>(infoWindow);
        }
        private async Task<List<ButtonDto>> GetButtons(string windowId)
        {
            var buttons = await _buttonService.GetByWindowIdAsync(windowId);
            return buttons.Select(p => ObjectMapper.Map<Button, ButtonDto>(p)).ToList();
        }
        private async Task<JsonArray> GetDataReference(ReferenceDto reference)
        {
            if (reference.FetchData != true) return null;
            string url = _config.GetValue<string>("App:SelfUrl") + reference.UrlApiData;
            //string url = _webHelper.GetUrlApi() + reference.UrlApiData;
            _logger.LogInformation("GetDataReference: " + url);
            string token = _webHelper.GetBearerToken();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            int year = _webHelper.GetCurrentYear();
            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", token);
            httpClient.DefaultRequestHeaders.Add("x-orgcode", orgCode);
            httpClient.DefaultRequestHeaders.Add("x-year", year.ToString());
            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;
            var result = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(result)) return null;
            return (JsonArray)JsonArray.Parse(result);
        }
        private async Task<Dictionary<string, ReferenceDto>> GetInfoWindowReferences(InfoWindowDto window)
        {
            if (window == null) return null;
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
        private async Task<List<ReportMenuShortcutDto>> GetReportMenuShortcutDtos(string windowId)
        {
            var entities = await _reportMenuShortcutService.GetByWindowIdAsync(windowId);
            return entities.Select(p => ObjectMapper.Map<ReportMenuShortcut, ReportMenuShortcutDto>(p))
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
        private async Task<WindowConfigDto> LoadDataReference(WindowConfigDto configDto)
        {
            if (configDto.References == null) return configDto;
            foreach(var item in configDto.References)
            {
                var referenceDto = item.Value;
                referenceDto.RefData = await GetDataReference(referenceDto);
            }
            return configDto;
        }
        #endregion
    }
}
