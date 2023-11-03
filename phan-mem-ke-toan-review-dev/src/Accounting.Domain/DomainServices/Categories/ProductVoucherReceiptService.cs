using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductVoucherReceiptService : BaseDomainService<ProductVoucherReceipt, string>
    {
        public ProductVoucherReceiptService(IRepository<ProductVoucherReceipt, string> repository) : base(repository)
        {
        }
        public async Task<List<ProductVoucherReceipt>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}