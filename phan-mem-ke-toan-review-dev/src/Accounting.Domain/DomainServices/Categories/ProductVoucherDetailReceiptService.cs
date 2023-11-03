using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductVoucherDetailReceiptService : BaseDomainService<ProductVoucherDetailReceipt, string>
    {
        public ProductVoucherDetailReceiptService(IRepository<ProductVoucherDetailReceipt, string> repository) : base(repository)
        {
        }

        public async Task<List<ProductVoucherDetailReceipt>> GetByProductIdAsync(string productVoucherDetailId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherDetailId == productVoucherDetailId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
