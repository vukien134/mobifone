using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L09Service : BaseDomainService<DefaultFStatement200L09, string>
    {
        public DefaultFStatement200L09Service(IRepository<DefaultFStatement200L09, string> repository) : base(repository)
        {
        }
    }
}

