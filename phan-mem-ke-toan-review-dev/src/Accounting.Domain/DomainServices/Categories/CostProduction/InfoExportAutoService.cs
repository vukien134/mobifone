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
    public class InfoExportAutoService : BaseDomainService<InfoExportAuto, string>
    {
        public InfoExportAutoService(IRepository<InfoExportAuto, string> repository) : base(repository)
        {
        }
    }
}
