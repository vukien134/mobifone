using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class AssetToolService : BaseDomainService<AssetTool, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public AssetToolService(IRepository<AssetTool, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(AssetTool entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Year == entity.Year
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(string orgCode, string code)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode
                                && p.Code == code);
        }
        public override async Task CheckDuplicate(AssetTool entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetTool, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<AssetTool>> GetDataReference(string orgCode, string filterValue, string assetOrTool)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                        && (EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue))
                                        && p.AssetOrTool.Equals(assetOrTool))
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
        public async Task<List<AssetTool>> GetListByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            var list = queryable.Where(p => p.OrgCode == orgCode).ToList();
            return list;
        }
        public async Task<bool> IsParentGroup(string groupId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.AssetToolGroupId == groupId);
        }
    }
}
