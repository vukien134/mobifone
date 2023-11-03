using Accounting.Categories.OrgUnits;
using Accounting.DomainServices.BaseServices;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using System.Linq;
using Accounting.Exceptions;
using System.Collections.Generic;
using Accounting.Permissions;
using System;
using Microsoft.Extensions.Localization;
using Accounting.Localization;

namespace Accounting.DomainServices.Categories
{
    public class OrgUnitService : BaseDomainService<OrgUnit, string>
    {
        #region Privates
        private readonly IRepository<OrgUnitPermission, string> _orgUnitPermissionRepository;        
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public OrgUnitService(IRepository<OrgUnit, string> repository,
                            IRepository<OrgUnitPermission, string> orgUnitPermissionRepository,
                            IStringLocalizer<AccountingResource> localizer) 
            : base(repository)
        {
            _orgUnitPermissionRepository = orgUnitPermissionRepository;
            _localizer = localizer;
        }
        public async Task<List<OrgUnit>> GetAllAsync()
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.OrderBy(p => p.Code);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<OrgUnit> GetByIdAsync(string id)
        {
            return await this.GetRepository().FindAsync(id);
        }
        public async Task<OrgUnit> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<OrgUnitPermission>> GetOrgUnitByUserIdAsync(Guid userId)
        {
            var queryable = await _orgUnitPermissionRepository.GetQueryableAsync();
            queryable = queryable.Where(p => p.UserId == userId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task DeleteOrgUnitByUserIdAsync(Guid userId)
        {
            var orgUnitPermissions = await this.GetOrgUnitByUserIdAsync(userId);
            await _orgUnitPermissionRepository.DeleteManyAsync(orgUnitPermissions);
        }
        public async Task SaveOrgUnitByUserIdAsync(List<OrgUnitPermission> orgUnitPermissions)
        {
            await _orgUnitPermissionRepository.InsertManyAsync(orgUnitPermissions);
        }
        public async Task<bool> IsExistCodeAsync(OrgUnit entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code && p.Id != entity.Id);
        }
        public async Task<List<OrgUnit>> GetOrgUnitForLogin(Guid userId)
        {
            var queryableOrgUnit = await this.GetQueryableAsync();
            var queryableOrgUnitPermission = await _orgUnitPermissionRepository.GetQueryableAsync();
            queryableOrgUnitPermission = queryableOrgUnitPermission.Where(p => p.UserId == userId);
            
            var query = from o in queryableOrgUnit
                        join p in queryableOrgUnitPermission on o.Id equals p.OrgUnitId
                        select o;

            return await AsyncExecuter.ToListAsync(query);
        }
        public async Task InsertOrgUnitPermissionAsync(OrgUnitPermission entity)
        {
            await _orgUnitPermissionRepository.InsertAsync(entity);
        }
        public async Task<bool> IsAssignOrgPermissionAsync(string id)
        {
            var queryable = await _orgUnitPermissionRepository.GetQueryableAsync();
            return queryable.Any(p => p.OrgUnitId == id);
        }
        public override async Task CheckDuplicate(OrgUnit entity)
        {
            bool isExistCode = await IsExistCodeAsync(entity);
            if (isExistCode)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.OrgUnit, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<OrgUnit> GetOrgUnitForRegistering()
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.OrderBy(p => p.Code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<int> CountAync()
        {
            return await this.GetRepository().CountAsync();
        }
    }
}
