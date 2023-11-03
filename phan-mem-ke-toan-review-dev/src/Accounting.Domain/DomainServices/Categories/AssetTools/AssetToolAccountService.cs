using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class AssetToolAccountService : BaseDomainService<AssetToolAccount, string>
    {
        public AssetToolAccountService(IRepository<AssetToolAccount, string> repository) : base(repository)
        {
        }
        public async Task<List<AssetToolAccount>> GetByAssetToolIdAsync(string assetToolId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AssetToolId == assetToolId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistCode(AssetToolAccount entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Year == entity.Year
                                && p.Id != entity.Id);
        }
        public async Task<List<AssetToolAccount>> GetListByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            var list = queryable.Where(p => p.OrgCode == orgCode).ToList();
            return list;
        }
        public override async Task CheckDuplicate(AssetToolAccount entity)
        {
            //bool isExist = await IsExistCode(entity);
            //if (isExist)
            //{
            //    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetToolAccount, ErrorCode.Duplicate),
            //            $"AssetToolAccount Code ['{entity.Code}'] already exist ");
            //}
        }
    }
}
