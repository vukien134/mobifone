using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Identity;

namespace Accounting.DomainServices.Users
{
    public class UserRoleService : DomainService
    {
        #region Fields
        private readonly UserRoleFinder _userRoleFinder;
        private readonly IIdentityUserRepository _identityUserRepository;
        #endregion
        #region Ctor
        public UserRoleService(UserRoleFinder userRoleFinder,
                            IIdentityUserRepository identityUserRepository)
        {
            _userRoleFinder = userRoleFinder;
            _identityUserRepository = identityUserRepository;
        }
        #endregion
        #region Method
        public async Task<string[]> GetRolesAsync(Guid userId)
        {
            return await _userRoleFinder.GetRolesAsync(userId);
        }        
        #endregion
    }
}
