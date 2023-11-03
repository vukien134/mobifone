using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L12Service : BaseDomainService<DefaultFStatement200L12, string>
    {
        public DefaultFStatement200L12Service(IRepository<DefaultFStatement200L12, string> repository) : base(repository)
        {
        }
    }
}

