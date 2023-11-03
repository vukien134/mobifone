using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L01Service : BaseDomainService<DefaultFStatement200L01, string>
    {
        public DefaultFStatement200L01Service(IRepository<DefaultFStatement200L01, string> repository) : base(repository)
        {
        }
    }
}

