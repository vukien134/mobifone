using Accounting.Exceptions;
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
    public class RoleService : DomainService
    {
        #region Fields
        private readonly IdentityRoleManager _identityRoleManager;
        private readonly IRepository<IdentityRole, Guid> _roleRepository;
        #endregion
        #region Ctor
        public RoleService(IdentityRoleManager identityRoleManager,
                            IRepository<IdentityRole, Guid> roleRepository)
        {
            _identityRoleManager = identityRoleManager;
            _roleRepository = roleRepository;
        }
        #endregion
        public Task<IQueryable<IdentityRole>> GetQueryableAsync()
        {
            return _roleRepository.GetQueryableAsync();
        }
        public async Task<IdentityRole> CreateAsync(IdentityRole entity)
        {
            var result = await _identityRoleManager.CreateAsync(entity);
            if (!result.Succeeded)
            {
                string detailError = "";
                foreach(var error in result.Errors)
                {
                    detailError = detailError + error.Description;
                }
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Role, ErrorCode.Other),
                        $"Create Role ['{entity.Name}'] has errors: '{detailError}' ");
            }
            return await GetIdentityRoleByIdAsync(entity.Id);
        }
        public async Task UpdateAsync(IdentityRole entity)
        {
            await _identityRoleManager.UpdateAsync(entity);
        }
        public async Task DeleteAsync(Guid id)
        {
            var entity = await GetIdentityRoleByIdAsync(id);
            await _identityRoleManager.DeleteAsync(entity);
        }
        public async Task<IdentityRole> GetIdentityRoleByIdAsync(Guid id)
        {
            return await _identityRoleManager.GetByIdAsync(id);
        }
        public Task<List<IdentityRole>> GetAllAsync()
        {
            return _roleRepository.ToListAsync();
        }
    }
}
