using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L05Service : BaseDomainService<DefaultFStatement200L05, string>
    {
        public DefaultFStatement200L05Service(IRepository<DefaultFStatement200L05, string> repository) : base(repository)
        {
        }

    }
}

