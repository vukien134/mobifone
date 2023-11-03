using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L13Service : BaseDomainService<DefaultFStatement200L13, string>
    {
        public DefaultFStatement200L13Service(IRepository<DefaultFStatement200L13, string> repository) : base(repository)
        {
        }
    }
}

