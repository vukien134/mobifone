using Accounting.Categories.Others;
using Accounting.Categories.Partners;
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
    public class PartnerGroupService : BaseDomainService<PartnerGroup, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public PartnerGroupService(IRepository<PartnerGroup, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(PartnerGroup entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(PartnerGroup entity)
        {
            bool isExist = await IsExistCode(entity);            
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.PartnerGroup, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }       

        public async Task<PartnerGroup> GetByCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<List<PartnerGroup>> GetByProductCodeAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<PartnerGroup>> GetByProductVoucherGroupParnerAsync(string code, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code && p.OrgCode == ordCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<PartnerGroup>> GetByProductVoucherGroupAsync(string Id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ParentId == Id);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsParentGroup(string groupId)
        {
            if (string.IsNullOrEmpty(groupId)) return false;
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentId == groupId);
        }
        public async Task<List<PartnerGroup>> GetListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
