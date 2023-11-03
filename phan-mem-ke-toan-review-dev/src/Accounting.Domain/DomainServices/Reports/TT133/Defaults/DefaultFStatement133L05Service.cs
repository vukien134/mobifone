using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L05Service : BaseDomainService<DefaultFStatement133L05, string>
    {
        public DefaultFStatement133L05Service(IRepository<DefaultFStatement133L05, string> repository) : base(repository)
        {
        }
    }
}

