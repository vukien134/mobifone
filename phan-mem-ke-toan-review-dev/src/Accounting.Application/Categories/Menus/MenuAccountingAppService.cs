using Accounting.BaseDtos;
using Accounting.Caching;
using Accounting.Catgories.Menus;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.MultiTenancy;

namespace Accounting.Categories.Menus
{
    public class MenuAccountingAppService : AccountingAppService, IMenuAccountingAppService
    {
        #region Fields
        private readonly MenuAccountingService _menuAccountingService;
        private readonly UserService _userService;        
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        private readonly CacheManager _cacheManager;
        private readonly WebHelper _webHelper;
        private readonly RegLicenseService _regLicenseService;
        #endregion
        #region Ctor
        public MenuAccountingAppService(MenuAccountingService menuAccountingService,
                            UserService userService,                            
                            IStringLocalizer<AccountingResource> localizer,
                            IPermissionDefinitionManager permissionDefinitionManager,
                            TenantExtendInfoService tenantExtendInfoService,
                            ICurrentTenant currentTenant,
                            CacheManager cacheManager,
                            WebHelper webHelper,
                            RegLicenseService regLicenseService
            )
        {
            _menuAccountingService = menuAccountingService;
            _userService = userService;            
            _localizer = localizer;
            _permissionDefinitionManager = permissionDefinitionManager;
            _tenantExtendInfoService = tenantExtendInfoService;
            _currentTenant = currentTenant;
            _cacheManager = cacheManager;
            _webHelper = webHelper;
            _regLicenseService = regLicenseService;
        }
        #endregion
        public async Task<MenuAccountingDto> CreateAsync(CruMenuAccountingDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            var entity = ObjectMapper.Map<CruMenuAccountingDto, MenuAccounting>(dto);
            var result = await _menuAccountingService.CreateAsync(entity);
            return ObjectMapper.Map<MenuAccounting, MenuAccountingDto>(result);
        }

        public async Task DeleteAsync(string id)
        {
            await _menuAccountingService.DeleteAsync(id);
        }

        public async Task<PageResultDto<MenuAccountingDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<MenuAccountingDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<MenuAccounting, MenuAccountingDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<string[]> GetDashboardByUserAsync()
        {
            string cacheKey = string.Format(CacheKeyManager.DashboardByUser, _currentTenant.Id,
                                                    _userService.GetCurrentUserId());
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    return await _userService.GetDashboard();
                }
            );            
        }
        public async Task<List<MenuWebixDto>> GetMenuWebixByUserAsync()
        {
            string languague = _webHelper.GetCurrentLanguage();
            string cacheKey = string.Format(CacheKeyManager.MenuByUser, _currentTenant.Id, languague, _userService.GetCurrentUserId().Value);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    int? tenantType = await this.GetTypeTenant();
                    var queryable = await _menuAccountingService.GetQueryableAsync();
                    queryable = queryable.Where(p => p.TenantType == tenantType || p.TenantType == null).OrderBy(p => p.Order);
                    List<MenuAccounting> menuAccountings = await AsyncExecuter.ToListAsync(queryable);
                    string[] viewPermissions = await _userService.GetViewPermissions();
                    var webixMenus = new List<MenuWebixDto>();
                    var menuWebix = new MenuWebixDto()
                    {
                        Id = this.GetNewObjectId(),
                        Value = _localizer["home"],
                        JavaScriptCode = "dash",
                        Icon = "mdi mdi-home"
                    };
                    webixMenus.Add(menuWebix);

                    GetMenuWebix(menuAccountings, null, webixMenus, viewPermissions);

                    return webixMenus;
                }
            );
        }

        public async Task UpdateAsync(string id, CruMenuAccountingDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            var entity = await _menuAccountingService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _menuAccountingService.UpdateAsync(entity);
        }
        public async Task<MenuAccountingDto> GetByIdAsync(string menuId)
        {
            var menuAccounting = await _menuAccountingService.GetAsync(menuId);
            return ObjectMapper.Map<MenuAccounting, MenuAccountingDto>(menuAccounting);
        }
        #region Private
        private async Task<IQueryable<MenuAccounting>> Filter(PageRequestDto dto)
        {
            var queryable = await _menuAccountingService.GetQueryableAsync();
            return queryable;
        }
        private void GetMenuWebix(List<MenuAccounting> menus, string parentId,List<MenuWebixDto> subMenu,
                                    string[] viewPermissions)
        {
            var menuAccountings = menus.Where(p => p.ParentId == parentId) 
                                .OrderBy(p => p.Order).ToList();

            if (menuAccountings.Count == 0) return;

            foreach(var item in menuAccountings)
            {
                if (!IsGrantedViewPermission(item, viewPermissions)) continue;

                var menuWebix = new MenuWebixDto()
                {
                    Id = item.Id,
                    Value = _localizer[item.Name],
                    Url = item.Url,
                    JavaScriptCode = item.JavaScriptCode,
                    Icon = item.Icon,
                    WindowId = item.windowId
                };
                menuWebix.Submenu = new List<MenuWebixDto>();
                GetMenuWebix(menus, item.Id, menuWebix.Submenu, viewPermissions);
                if (menuWebix.Submenu.Count == 0)
                {
                    if (string.IsNullOrEmpty(item.ViewPermission)) continue;
                    menuWebix.Submenu = null;
                }
                subMenu.Add(menuWebix);
            }
        }
        private bool IsGrantedViewPermission(MenuAccounting menu, string[] viewPermissions)
        {
            if (string.IsNullOrEmpty(menu.ViewPermission)) 
                return true;
            if (menu.JavaScriptCode.Equals(JavaScriptCodeType.Report)) 
                return IsGrantedReportGroup(menu, viewPermissions);
            return viewPermissions.Contains(menu.ViewPermission);
        }
        private bool IsGrantedReportGroup(MenuAccounting menu, string[] viewPermissions)
        {
            var reportGroup = _permissionDefinitionManager.GetGroups()
                                .Where(p => p.Name.Equals(PermissionGroup.Report))
                                .FirstOrDefault();

            var permissionDefines = reportGroup.Permissions.Where(p => p.Name.Equals(menu.ViewPermission))
                                    .FirstOrDefault();
            if (permissionDefines == null) return false;

            foreach(var reportPermission in permissionDefines.Children)
            {
                var viewPermission = reportPermission.Children
                                        .Where(p => p.Name.EndsWith($"_{ActionType.View}"))
                                        .FirstOrDefault();
                if (viewPermission == null) continue;
                if (viewPermissions.Contains(viewPermission.Name)) return true;
            }

            return false;
        }
        private async Task<int?> GetTypeTenant()
        {
            var tenant = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenant == null) return null;
            return tenant.TenantType;
        }
        #endregion
    }
}
