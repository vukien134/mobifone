using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Licenses;
using Accounting.Exceptions;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement;
using Volo.Abp.Users;

namespace Accounting.DomainServices.Users
{
    public class UserService : DomainService
    {
        #region Fields
        private readonly IRepository<IdentityUser, Guid> _userRepository;        
        private readonly ICurrentUser _currentUser;
        private readonly IdentityUserManager _identityUserManager;
        private readonly IPermissionManager _permissionManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly RegLicenseService _regLicenseService;
        #endregion
        #region Ctor
        public UserService(IRepository<IdentityUser,Guid> userRepository,                            
                            ICurrentUser currentUser,
                            IdentityUserManager identityUserManager,
                            IPermissionManager permissionManager,
                            ICurrentTenant currentTenant,
                            TenantExtendInfoService tenantExtendInfoService,
                            IStringLocalizer<AccountingResource> localizer,
                            RegLicenseService regLicenseService
            )
        {
            _userRepository = userRepository;
            _currentUser = currentUser;
            _identityUserManager = identityUserManager;
            _permissionManager = permissionManager;
            _currentTenant = currentTenant;
            _tenantExtendInfoService = tenantExtendInfoService;
            _localizer = localizer;
            _regLicenseService = regLicenseService;
        }
        #endregion
        #region Methods
        public async Task<string> GetUserNameByIdAsync(Guid? userId)
        {
            if (userId == null) return null;

            var user = await _userRepository.FindAsync(c => c.Id == userId);
            if (user != null)
            {
                return user.UserName;
            }

            return null;
        }
        public async Task<string> GetCurrentUserNameAsync()
        {
            var userId = _currentUser.Id;
            return await this.GetUserNameByIdAsync(userId);
        }
        public async Task<IdentityUser> GetByEmailAsync(string email)
        {
            var queryable = await _userRepository.GetQueryableAsync();
            queryable = queryable.Where(p => p.Email.Equals(email));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public Guid? GetCurrentUserId()
        {
            return _currentUser.Id;
        }
        public string[] GetRoles()
        {
            return _currentUser.Roles;
        }
        public async Task<string[]> GetViewPermissions()
        {
            var regLic = await _regLicenseService.GetByTenantId(_currentTenant.Id);
            var grantedPermission = await _permissionManager.GetAllForUserAsync(_currentUser.Id.Value);
            var permissions = grantedPermission.Where(p => p.Name.EndsWith(AccountingPermissions.ActionView)
                                        && p.IsGranted).Select(p => p.Name).ToArray();
            var result = new List<string>();
            foreach(var item in permissions)
            {
                string name = item.Replace("_View", "");
                if (!CompanyTypePermission.Detail.ContainsKey(name)) continue;
                if (!CompanyTypePermission.Detail[name].Contains(regLic.CompanyType)) continue;
                result.Add(item);
            }
            return result.ToArray();
        }
        public async Task<string[]> GetDashboard()
        {
            var grantedPermission = await _permissionManager.GetAllForUserAsync(_currentUser.Id.Value);
            return grantedPermission.Where(p => p.Name.EndsWith(AccountingPermissions.ActionDashboard) && p.IsGranted
                        && !p.Name.Equals("Dashboard"))
                .Select(p => p.Name).ToArray();
        }
        public async Task<IdentityUser> CreateAsync(IdentityUser entity,string password)
        {
            var result = await _identityUserManager.CreateAsync(entity, password);
            if (!result.Succeeded)
            {
                string detailError = "";
                foreach(var error in result.Errors)
                {
                    detailError = detailError + _localizer[error.Code];
                }
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.User, ErrorCode.Other),
                        _localizer[$"Err:{detailError}"]);
            }
            return await GetIdentityUserByIdAsync(entity.Id);
        }
        public async Task UpdateAsync(IdentityUser entity)
        {
            await _identityUserManager.UpdateAsync(entity);
        }
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetIdentityUserByIdAsync(id);
            await _identityUserManager.DeleteAsync(entity);
        }
        public async Task<IdentityUser> GetIdentityUserByIdAsync(Guid id)
        {
            return await _identityUserManager.GetByIdAsync(id);
        }        
        public Task<IQueryable<IdentityUser>> GetQueryableAsync()
        {
            return _userRepository.GetQueryableAsync();
        }
        public Task<int> Count()
        {
            return _userRepository.CountAsync();
        }
        public async Task UpdateRolesAsync(Guid userId,string[] roleNames)
        {
            var user = await _identityUserManager.GetByIdAsync(userId);
            await _identityUserManager.SetRolesAsync(user, roleNames);
        }
        public async Task<List<IdentityUser>> GetIdentityUsersAsync()
        {
            var queryable = await _userRepository.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task ResetPassWordAsync(string newPass,IdentityUser user)
        {
            string resetToken = await _identityUserManager.GeneratePasswordResetTokenAsync(user);
            await _identityUserManager.ResetPasswordAsync(user, resetToken, newPass);
        }
        public async Task<List<IdentityUser>> GetUsersByRoleIdAsync(Guid roleId)
        {
            var queryable = await _userRepository.WithDetailsAsync(p => p.Roles);
            queryable = queryable.Where(p => p.Roles.Any(c => c.RoleId == roleId));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        private async Task<int?> GetTenantType()
        {
            var tenantInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantInfo == null) return null;
            return tenantInfo.TenantType;
        }        
        #endregion
    }
}
