using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Vouchers
{
    public class RefVoucherService : BaseDomainService<RefVoucher, string>
    {
        public RefVoucherService(IRepository<RefVoucher, string> repository) : base(repository)
        {
        }
        public async Task<List<RefVoucher>> GetByRefId(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.DestId.Equals(id));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task DeleteSrc(string srcId)
        {
            var vouchers = await this.GetRepository().GetListAsync(p => p.SrcId == srcId);
            if (vouchers.Count == 0) return;
            await this.GetRepository().DeleteManyAsync(vouchers);
        }
        public async Task DeleteDest(string destId)
        {
            var vouchers = await this.GetRepository().GetListAsync(p => p.DestId == destId);
            if (vouchers.Count == 0) return;
            await this.GetRepository().DeleteManyAsync(vouchers);
        }
        public async Task DeleteRefVoucher(string id)
        {
            var vouchers = await this.GetRepository()
                                .GetListAsync(p => p.DestId == id || p.SrcId == id);
            if (vouchers.Count == 0) return;
            await this.GetRepository().DeleteManyAsync(vouchers);
        }
    }
}

