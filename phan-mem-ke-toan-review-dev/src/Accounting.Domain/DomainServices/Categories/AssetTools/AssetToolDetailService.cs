using Accounting.Categories.AssetTools;
using Accounting.Categories.Contracts;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class AssetToolDetailService : BaseDomainService<AssetToolDetail, string>
    {
        public AssetToolDetailService(IRepository<AssetToolDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<AssetToolDetail>> GetByAssetToolIdAsync(string assetToolId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AssetToolId == assetToolId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AssetToolDetail>> GetListByOrgCode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            var list = queryable.Where(p => p.OrgCode == orgCode).ToList();
            return list;
        }
    }
}
