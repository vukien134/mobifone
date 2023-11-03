using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class FStatement133L05Service : BaseDomainService<TenantFStatement133L05, string>
    {
        public FStatement133L05Service(IRepository<TenantFStatement133L05, string> repository) : base(repository)
        {
        }
    }
}

