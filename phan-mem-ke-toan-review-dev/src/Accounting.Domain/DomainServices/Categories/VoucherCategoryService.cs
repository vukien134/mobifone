using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class VoucherCategoryService : BaseDomainService<VoucherCategory, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public VoucherCategoryService(IRepository<VoucherCategory, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ): base(repository)
        {
            _localizer = localizer;
        }

        public async Task<VoucherCategory> CheckIsSavingLedgerAsync(string Code, string OrdCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == Code && p.OrgCode == OrdCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<VoucherCategory> GetByCode(string Code, string OrdCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == Code && p.OrgCode == OrdCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<bool> IsExistCode(VoucherCategory entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(VoucherCategory entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherCategory, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<VoucherCategory> GetByCodeAsync(string code, string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<VoucherCategory>> GetByVoucherCategoryAsync( string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<VoucherCategory>> GetByTenantTypeAsync(string orgCode,int? tenantType)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.TenantType == tenantType);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode.Equals(orgCode));
        }
    }
}