using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L07Service : BaseDomainService<DefaultFStatement200L07, string>
    {
        public DefaultFStatement200L07Service(IRepository<DefaultFStatement200L07, string> repository) : base(repository)
        {
        }
    }
}

