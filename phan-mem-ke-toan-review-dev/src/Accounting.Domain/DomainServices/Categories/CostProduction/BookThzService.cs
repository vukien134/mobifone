using Accounting.Categories.CostProductions;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.CostProduction
{
    public class BookThzService : BaseDomainService<BookThz, string>
    {
        public BookThzService(IRepository<BookThz, string> repository) : base(repository)
        {
        }
    }
}
