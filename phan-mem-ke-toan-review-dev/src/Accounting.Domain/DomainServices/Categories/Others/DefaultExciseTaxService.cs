using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class DefaultExciseTaxService : BaseDomainService<DefaultExciseTax, string>
    {
        public DefaultExciseTaxService(IRepository<DefaultExciseTax, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultExciseTax>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
    }
}
