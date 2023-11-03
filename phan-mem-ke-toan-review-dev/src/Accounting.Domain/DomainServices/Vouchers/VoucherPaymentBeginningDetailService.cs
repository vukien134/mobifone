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
    public class VoucherPaymentBeginningDetailService : BaseDomainService<VoucherPaymentBeginningDetail, string>
    {
        public VoucherPaymentBeginningDetailService(IRepository<VoucherPaymentBeginningDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<VoucherPaymentBeginningDetail>> GetByVoucherPaymentBeginningIdAsync(string voucherPaymentBeginningId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.VoucherPaymentBeginningId == voucherPaymentBeginningId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
