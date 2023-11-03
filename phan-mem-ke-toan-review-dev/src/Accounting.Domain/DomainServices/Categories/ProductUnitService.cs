using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductUnitService : BaseDomainService<ProductUnit, string>
    {
        public ProductUnitService(IRepository<ProductUnit, string> repository) : base(repository)
        {
        }
        public async Task<List<ProductUnit>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductUnit>> GetByProductUnitAsync(string coDe, string ordCode, string unit)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductCode == coDe && p.OrgCode == ordCode && p.UnitCode == unit);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductUnit>> GetByProductCodeAsync(string coDe, string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductCode == coDe && p.OrgCode == ordCode && p.IsBasicUnit == true);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
