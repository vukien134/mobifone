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
    public class AssetToolAccessoryService : BaseDomainService<AssetToolAccessory, string>
    {
        public AssetToolAccessoryService(IRepository<AssetToolAccessory, string> repository) : base(repository)
        {
        }
        public async Task<List<AssetToolAccessory>> GetByAssetToolIdAsync(string assetToolId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AssetToolId == assetToolId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
