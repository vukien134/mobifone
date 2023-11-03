using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L18Service : BaseDomainService<DefaultFStatement200L18, string>
    {
        public DefaultFStatement200L18Service(IRepository<DefaultFStatement200L18, string> repository) : base(repository)
        {
        }
    }
}

