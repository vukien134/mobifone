using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultAccountSystemService : BaseDomainService<DefaultAccountSystem, string>
    {
        public DefaultAccountSystemService(IRepository<DefaultAccountSystem, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultAccountSystem>> GetListAsync(int usingDecision)
        {
            var accountingSystem = await this.GetRepository()
                                .GetListAsync(p => p.UsingDecision == usingDecision);
            return accountingSystem;
        }
    }
}
