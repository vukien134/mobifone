using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class AssetToolGroupService : BaseDomainService<AssetToolGroup, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public AssetToolGroupService(IRepository<AssetToolGroup, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(AssetToolGroup entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(string orgCode, string code)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode
                                && p.Code == code);
        }
        public override async Task CheckDuplicate(AssetToolGroup entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetToolGroup, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<AssetToolGroup>> GetByAssetToolAsync(string assetToolGroup, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AssetOrTool == assetToolGroup && p.OrgCode == ordCode);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistListAsync(string orgCode,string assetOrTool)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode.Equals(orgCode) 
                                && p.AssetOrTool.Equals(assetOrTool));
        }
        public async Task<bool> IsParentGroup(string groupId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentId == groupId);
        }
    }
}
