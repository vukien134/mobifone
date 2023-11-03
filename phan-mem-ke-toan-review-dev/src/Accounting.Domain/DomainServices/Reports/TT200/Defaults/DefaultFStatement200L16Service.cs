using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L16Service : BaseDomainService<DefaultFStatement200L16, string>
    {
        public DefaultFStatement200L16Service(IRepository<DefaultFStatement200L16, string> repository) : base(repository)
        {
        }
    }
}

