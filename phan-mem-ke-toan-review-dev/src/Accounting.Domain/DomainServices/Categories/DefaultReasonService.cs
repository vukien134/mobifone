using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultReasonService : BaseDomainService<DefaultReason, string>
    {
        public DefaultReasonService(IRepository<DefaultReason, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultReason>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
    }
}
