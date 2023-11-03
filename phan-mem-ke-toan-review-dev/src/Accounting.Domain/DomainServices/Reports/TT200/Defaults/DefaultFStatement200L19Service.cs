using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L19Service : BaseDomainService<DefaultFStatement200L19, string>
    {
        public DefaultFStatement200L19Service(IRepository<DefaultFStatement200L19, string> repository) : base(repository)
        {
        }
    }
}

