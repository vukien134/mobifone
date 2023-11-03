using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class VoucherExciseTaxService : BaseDomainService<VoucherExciseTax, string>
    {
        public VoucherExciseTaxService(IRepository<VoucherExciseTax, string> repository) : base(repository)
        {
        }
        public async Task<List<VoucherExciseTax>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
