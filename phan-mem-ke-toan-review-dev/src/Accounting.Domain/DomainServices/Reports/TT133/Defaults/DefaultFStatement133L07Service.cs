using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Reports.Statements.T133.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L07Service : BaseDomainService<DefaultFStatement133L07, string>
    {
        public DefaultFStatement133L07Service(IRepository<DefaultFStatement133L07, string> repository) : base(repository)
        {
        }
    }
}

