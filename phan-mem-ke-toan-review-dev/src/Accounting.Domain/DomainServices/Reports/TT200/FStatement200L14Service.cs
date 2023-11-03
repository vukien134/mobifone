using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L14Service : BaseDomainService<TenantFStatement200L14, string>
    {
        public FStatement200L14Service(IRepository<TenantFStatement200L14, string> repository) : base(repository)
        {
        }
    }
}

