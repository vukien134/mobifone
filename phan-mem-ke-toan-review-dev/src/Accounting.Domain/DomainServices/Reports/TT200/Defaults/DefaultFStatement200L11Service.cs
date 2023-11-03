using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L11Service : BaseDomainService<DefaultFStatement200L11, string>
    {
        public DefaultFStatement200L11Service(IRepository<DefaultFStatement200L11, string> repository) : base(repository)
        {
        }
    }
}

