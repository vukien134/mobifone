using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class VoucherPaymentBookService : BaseDomainService<VoucherPaymentBook, string>
    {
        public VoucherPaymentBookService(IRepository<VoucherPaymentBook, string> repository)
            : base(repository)
        {
        }
        public async Task<bool> IsExistCode(VoucherPaymentBook entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(VoucherPaymentBook entity)
        {

        }
        public async Task<List<VoucherPaymentBook>> GetByVoucherPaymentBookAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccVoucherId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }

        public async Task<List<VoucherPaymentBook>> GetByAccVoucherIdAsync(string accVoucherId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccVoucherId == accVoucherId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<VoucherPaymentBook>> GetByIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.DocumentId == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task DeleteByVoucherId(string id)
        {
            var payments = await this.GetRepository()
                                    .GetListAsync(p => p.DocumentId == id);
            if (payments.Count == 0) return;
            await this.GetRepository().DeleteManyAsync(payments);
        }
    }
}