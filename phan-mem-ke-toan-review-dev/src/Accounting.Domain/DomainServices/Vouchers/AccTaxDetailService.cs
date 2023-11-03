using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Vouchers
{
    public class AccTaxDetailService : BaseDomainService<AccTaxDetail, string>
    {
        public AccTaxDetailService(IRepository<AccTaxDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<AccTaxDetail>> GetByAccVoucherIdAsync(string accVoucherId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccVoucherId == accVoucherId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<AccTaxDetail>> GetByProductIdAsync(string ProductVoucherId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductVoucherId == ProductVoucherId);
            return await AsyncExecuter.ToListAsync(queryable);
        }

    }
}
