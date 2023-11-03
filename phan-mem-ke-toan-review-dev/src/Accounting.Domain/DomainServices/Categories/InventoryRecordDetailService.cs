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

namespace Accounting.DomainServices.Categories
{
    public class InventoryRecordDetailService : BaseDomainService<InventoryRecordDetail, string>
    {
        public InventoryRecordDetailService(IRepository<InventoryRecordDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<InventoryRecordDetail>> GetByInventoryRecordIdAsync(string inventoryRecordId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.InventoryRecordId == inventoryRecordId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
