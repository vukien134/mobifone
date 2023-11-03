using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductVoucherAssemblyService : BaseDomainService<ProductVoucherAssembly, string>
    {
        public ProductVoucherAssemblyService(IRepository<ProductVoucherAssembly, string> repository) : base(repository)
        {
        }
        public async Task<List<ProductVoucherAssembly>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }

    }
}