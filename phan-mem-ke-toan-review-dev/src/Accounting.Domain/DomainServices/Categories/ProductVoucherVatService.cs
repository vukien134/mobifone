using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductVoucherVatService : BaseDomainService<ProductVoucherVat, string>
    {
        public ProductVoucherVatService(IRepository<ProductVoucherVat, string> repository) : base(repository)
        {
        }
        public async Task<List<ProductVoucherVat>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }

    }
}