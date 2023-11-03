using Accounting.Categories.CostProductions;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class InfoZService : BaseDomainService<InfoZ, string>
    {
        public InfoZService(IRepository<InfoZ, string> repository) : base(repository)
        {
        }
    }
}
