using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L08Service : BaseDomainService<DefaultFStatement200L08, string>
    {
        public DefaultFStatement200L08Service(IRepository<DefaultFStatement200L08, string> repository) : base(repository)
        {
        }
    }
}

