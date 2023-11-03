using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L06Service : BaseDomainService<DefaultFStatement133L06, string>
    {
        public DefaultFStatement133L06Service(IRepository<DefaultFStatement133L06, string> repository) : base(repository)
        {
        }
    }
}

