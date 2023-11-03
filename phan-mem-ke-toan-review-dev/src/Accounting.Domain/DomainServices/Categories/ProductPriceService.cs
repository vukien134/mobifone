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
    public class ProductPriceService : BaseDomainService<ProductPrice, string>
    {
        public ProductPriceService(IRepository<ProductPrice, string> repository) : base(repository)
        {
        }
        public async Task<List<ProductPrice>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
