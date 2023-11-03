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
    public class DefaultFixErrorService : BaseDomainService<DefaultFixError, string>
    {
        public DefaultFixErrorService(IRepository<DefaultFixError, string> repository) : base(repository)
        {
        }
    }
}
