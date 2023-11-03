using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L06Service : BaseDomainService<DefaultFStatement200L06, string>
    {
        public DefaultFStatement200L06Service(IRepository<DefaultFStatement200L06, string> repository) : base(repository)
        {
        }
    }
}

