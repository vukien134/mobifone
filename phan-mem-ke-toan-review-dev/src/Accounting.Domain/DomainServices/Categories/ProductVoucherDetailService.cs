using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductVoucherDetailService : BaseDomainService<ProductVoucherDetail, string>
    {
        public ProductVoucherDetailService(IRepository<ProductVoucherDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<ProductVoucherDetail>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductVoucherDetail>> GetByProductDetailIdAsync(string Id, string Ord0)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == Id && p.Ord0 == Ord0);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}