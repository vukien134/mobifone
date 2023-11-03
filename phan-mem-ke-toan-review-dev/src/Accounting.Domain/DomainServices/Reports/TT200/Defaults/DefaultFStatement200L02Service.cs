using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L02Service : BaseDomainService<DefaultFStatement200L02, string>
    {
        public DefaultFStatement200L02Service(IRepository<DefaultFStatement200L02, string> repository) : base(repository)
        {
        }
    }
}

