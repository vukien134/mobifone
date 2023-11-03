using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L15Service : BaseDomainService<DefaultFStatement200L15, string>
    {
        public DefaultFStatement200L15Service(IRepository<DefaultFStatement200L15, string> repository) : base(repository)
        {
        }
    }
}

