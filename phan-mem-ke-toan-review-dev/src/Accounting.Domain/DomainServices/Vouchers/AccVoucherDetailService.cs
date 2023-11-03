using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Vouchers
{
    public class AccVoucherDetailService : BaseDomainService<AccVoucherDetail, string>
    {
        public AccVoucherDetailService(IRepository<AccVoucherDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<AccVoucherDetail>> GetByAccVoucherIdAsync(string accVoucherId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccVoucherId == accVoucherId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
