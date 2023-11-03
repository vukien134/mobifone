using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Reports.Statements.T133.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class FStatement133L07Service : BaseDomainService<TenantFStatement133L07, string>
    {
        public FStatement133L07Service(IRepository<TenantFStatement133L07, string> repository) : base(repository)
        {
        }
    }
}

