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
    public class AssetToolDepreciationService : BaseDomainService<AssetToolDepreciation, string>
    {
        public AssetToolDepreciationService(IRepository<AssetToolDepreciation, string> repository) : base(repository)
        {
        }
        public async Task<List<AssetToolDepreciation>> GetByAssetToolIdAsync(string assetToolId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AssetToolId == assetToolId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistCode(AssetToolDepreciation entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Year == entity.Year
                                && p.Id != entity.Id);
        }
        public async Task<List<AssetToolDepreciation>> GetListByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            var list = queryable.Where(p => p.OrgCode == orgCode).ToList();
            return list;
        }
        public override async Task CheckDuplicate(AssetToolDepreciation entity)
        {
            //bool isExist = await IsExistCode(entity);
            //if (isExist)
            //{
            //    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AssetToolDepreciation, ErrorCode.Duplicate),
            //            $"AssetToolDepreciation Code ['{entity.Code}'] already exist ");
            //}
        }
    }
}
