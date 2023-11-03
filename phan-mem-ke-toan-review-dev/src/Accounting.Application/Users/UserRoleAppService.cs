using Accounting.BaseDtos;
using Accounting.Caching;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Tenants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Identity;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;

namespace Accounting.Users
{
    public class UserRoleAppService : AccountingAppService, IUserRoleAppService
    {
        #region Fields
        private readonly RoleService _userRoleService;
        private readonly UserService _userService;
        private readonly UserRoleService _roleUserService;
        private readonly ICurrentTenant _currentTenant;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;
        private readonly IPermissionManager _permissionManager;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public UserRoleAppService(RoleService userRoleService,ICurrentTenant currentTenant,
                        IPermissionDefinitionManager permissionDefinitionManager,
                        IPermissionManager permissionManager,
                        IStringLocalizer<AccountingResource> localizer,
                        TenantExtendInfoService tenantExtendInfoService,
                        CacheManager cacheManager,
                        UserService userService,
                        UserRoleService roleUserService
            )
        {
            _userRoleService = userRoleService;
            _currentTenant = currentTenant;
            _permissionDefinitionManager = permissionDefinitionManager;
            _permissionManager = permissionManager;
            _localizer = localizer;
            _tenantExtendInfoService = tenantExtendInfoService;
            _cacheManager = cacheManager;
            _userService = userService;
            _roleUserService = roleUserService;
        }
        #endregion
        [Authorize(AccountingPermissions.RoleManagerView)]
        public Task<PageResultDto<UserRoleDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.RoleManagerView)]
        public async Task<PageResultDto<UserRoleDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<UserRoleDto>();
            var query = await Filter(dto);
            int count = dto.Count == 0 ? 10 : dto.Count;
            var querysort = query.OrderBy(p => p.Name).Skip(dto.Start).Take(count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<IdentityRole, UserRoleDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<List<GroupPermissionDto>> GetGroupPermissionAsync()
        {
            var tenantExtendInfo = await this.GetTenantExtendInfo();
            var groups = _permissionDefinitionManager.GetGroups()
                            .Where(p => p.MultiTenancySide == MultiTenancySides.Tenant
                                    && TenantTypePermission.Group[tenantExtendInfo.TenantType.Value].Contains(p.Name)
                                    && CompanyTypePermission.Group[p.Name].Contains(tenantExtendInfo.CompanyType));

            var lst = groups.Select(p => new GroupPermissionDto()
            {
                Id = p.Name,
                Value = p.Name,
                Name = p.Name,
                IsSelect = false,
                DisplayName = _localizer[((LocalizableString)p.DisplayName).Name]
            }).ToList();

            return lst;
        }
        public async Task<UserRolePermissionDto> GetPermissionTreeByGroupAsync(Guid roleId,string name)
        {
            var tenantExtendInfo = await this.GetTenantExtendInfo();
            var role = await _userRoleService.GetIdentityRoleByIdAsync(roleId);
            var group = _permissionDefinitionManager.GetGroups().Where(p => p.Name == name
                                                && CompanyTypePermission.Group[p.Name].Contains(tenantExtendInfo.CompanyType))
                                    .FirstOrDefault();
            var permissions = group.Permissions.Where(p => TenantTypePermission.Detail[tenantExtendInfo.TenantType.Value].Contains(p.Name)
                                            && CompanyTypePermission.Detail[p.Name].Contains(tenantExtendInfo.CompanyType))
                                        .ToList();
            var tree = new List<PermissionTreeItemDto>();
            var needCheckPermissions = new List<string>();
            BuildTreePermissions(permissions, tree, needCheckPermissions);

            var multipleGrantInfo = await _permissionManager.GetAllForRoleAsync(role.Name);
            var grantNames = new List<string>();
            foreach(var item in needCheckPermissions)
            {
                var grantInfo = multipleGrantInfo.Where(p => p.Name == item).FirstOrDefault();
                if (grantInfo != null)
                {
                    if (grantInfo.IsGranted)
                    {
                        grantNames.Add(item);
                    }
                }
            }
            var userRolePermission = new UserRolePermissionDto()
            {
                PermissionTrees = tree,
                GrantNames = grantNames
            };
            return userRolePermission;
        }
        [Authorize(AccountingPermissions.RoleManagerCreate)]
        public async Task<UserRoleDto> CreateAsync(CrudUserRoleDto dto)
        {
            dto.Id = GuidGenerator.Create();
            dto.TenantId = _currentTenant.Id;
            var entity = ObjectMapper.Map<CrudUserRoleDto, IdentityRole>(dto);
            var identityUser = await _userRoleService.CreateAsync(entity);
            return ObjectMapper.Map<IdentityRole, UserRoleDto>(identityUser);
        }
        [Authorize(AccountingPermissions.RoleManagerUpdate)]
        public async Task UpdateAsync(Guid id, CrudUserRoleDto dto)
        {
            var entity = await _userRoleService.GetIdentityRoleByIdAsync(id);
            ObjectMapper.Map(dto, entity);
            await _userRoleService.UpdateAsync(entity);
        }
        [Authorize(AccountingPermissions.RoleManagerDelete)]
        public async Task DeleteAsync(Guid id)
        {
            await this.isUsed(id);
            await this.RemoveMenuCacheByUser(id);
            await _userRoleService.DeleteAsync(id);           
        }
        [Authorize(AccountingPermissions.RoleManagerAssign)]
        public async Task AssignPermission(AssignPermissionDto dto)
        {
            var role = await _userRoleService.GetIdentityRoleByIdAsync(dto.RoleId.Value);
            foreach(var item in dto.Items)
            {
                await _permissionManager.SetForRoleAsync(role.Name, item.Name, item.IsGranted);                
            }
            await this.RemoveMenuCacheByUser(dto.RoleId.Value);
        }        
        public async Task<UserRoleDto> GetByIdAsync(Guid userRoleId)
        {
            var identityRole = await _userRoleService.GetIdentityRoleByIdAsync(userRoleId);
            return ObjectMapper.Map<IdentityRole, UserRoleDto>(identityRole);
        }
        [Authorize(AccountingPermissions.RoleManagerAssign)]
        public async Task AssignOrRevokeGroupPermission(AssignPermissionDto dto)
        {
            var role = await _userRoleService.GetIdentityRoleByIdAsync(dto.RoleId.Value);
            string[] names = dto.Items.Select(p => p.Name).ToArray();
            var groups = _permissionDefinitionManager.GetGroups()
                                    .Where(p => p.MultiTenancySide == MultiTenancySides.Tenant
                                            && names.Contains(p.Name))
                                    .ToList();
            foreach (var group in groups)
            {
                var permissions = group.Permissions.ToList();
                bool value = dto.Items.Where(p => p.Name == group.Name)
                                        .Select(p => p.IsGranted).FirstOrDefault();
                await AssignGroupPermission(permissions, role.Name, value);
            }
            await this.RemoveMenuCacheByUser(dto.RoleId.Value);
        }
        #region Privates
        private async Task<IQueryable<IdentityRole>> Filter(PageRequestDto dto)
        {
            var queryable = await _userRoleService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private void BuildTreePermissions(List<PermissionDefinition> permissionDefinitions,
                                        List<PermissionTreeItemDto> tree,
                                        List<string> grantNames
                                        )
        {
            foreach (var item in permissionDefinitions)
            {
                var child = new PermissionTreeItemDto()
                {
                    Id = item.Name,
                    Value = item.Name,
                    Open = false,
                    Name = item.Name,
                    DisplayName = _localizer[((LocalizableString)item.DisplayName).Name] 
                };

                if (item.Children.Count > 0)
                {
                    child.Open = true;
                    child.Data = new List<PermissionTreeItemDto>();
                    BuildTreePermissions(item.Children.ToList(), child.Data, grantNames);
                }

                grantNames.Add(item.Name);
                tree.Add(child);
            }
        }
        private async Task AssignAllPermission(Guid roleId)
        {
            var role = await _userRoleService.GetIdentityRoleByIdAsync(roleId);
            var groups = _permissionDefinitionManager.GetGroups()
                                    .Where(p => p.MultiTenancySide == MultiTenancySides.Tenant)
                                    .ToList();
            foreach(var group in groups)
            {
                var permissions = group.Permissions.ToList();
                await AssignGroupPermission(permissions, role.Name, true);
            }
        }
        private async Task AssignGroupPermission(List<PermissionDefinition> permissions,
                                        string roleName, bool granted)
        {
            foreach(var item in permissions)
            {                
                if (item.Children.Count > 0)
                {
                    await AssignGroupPermission(item.Children.ToList(), roleName, granted);
                    continue;
                }
                await _permissionManager.SetForRoleAsync(roleName, item.Name, granted);
            }
        }
        private async Task<TenantExtendInfo> GetTenantExtendInfo()
        {
            var tenantInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            return tenantInfo;
        }
        
        private async Task RemoveMenuCacheByUser(Guid roleId)
        {
            var userRoles = await _userService.GetUsersByRoleIdAsync(roleId);
            foreach(var item in userRoles)
            {
                string cacheKey = string.Format(CacheKeyManager.MenuByUser, _currentTenant.Id, LangConst.EN, item.Id);
                await _cacheManager.RemoveAsync(cacheKey);
                cacheKey = string.Format(CacheKeyManager.MenuByUser, _currentTenant.Id, LangConst.VI, item.Id);
                await _cacheManager.RemoveAsync(cacheKey);
                cacheKey = string.Format(CacheKeyManager.DashboardByUser, _currentTenant.Id, item.Id);
                await _cacheManager.RemoveAsync(cacheKey);
            }            
        }
        private async Task<bool> isUsed(Guid roleId)
        {

            var userData = await _userService.GetUsersByRoleIdAsync(roleId);
            if (userData.Count() > 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Role, ErrorCode.IsUsing),
                        _localizer["Err:RoleIsUsing"]);
            }
            return true;
        }
        #endregion
    }
}
