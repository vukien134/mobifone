using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class DefaultAssetToolGroupService : BaseDomainService<DefaultAssetToolGroup, string>
    {
        public DefaultAssetToolGroupService(IRepository<DefaultAssetToolGroup, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultAssetToolGroup>> GetByAssetOrToolAsync(string assetOrTool)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AssetOrTool.Equals(assetOrTool));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
