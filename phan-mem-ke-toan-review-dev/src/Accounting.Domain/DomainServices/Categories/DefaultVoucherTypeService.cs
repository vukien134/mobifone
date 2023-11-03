using Accounting.DomainServices.BaseServices;
using Accounting.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultVoucherTypeService : BaseDomainService<DefaultVoucherType, string>
    {
        public DefaultVoucherTypeService(IRepository<DefaultVoucherType, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultVoucherType>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
    }
}
