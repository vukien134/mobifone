using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class FStatement133L06Service : BaseDomainService<TenantFStatement133L06, string>
    {
        public FStatement133L06Service(IRepository<TenantFStatement133L06, string> repository) : base(repository)
        {
        }
    }
}

