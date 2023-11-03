using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L16Service : BaseDomainService<TenantFStatement200L16, string>
    {
        public FStatement200L16Service(IRepository<TenantFStatement200L16, string> repository) : base(repository)
        {
        }
    }
}

