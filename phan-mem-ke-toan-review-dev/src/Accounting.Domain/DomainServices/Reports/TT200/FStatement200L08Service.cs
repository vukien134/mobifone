using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L08Service : BaseDomainService<TenantFStatement200L08, string>
    {
        public FStatement200L08Service(IRepository<TenantFStatement200L08, string> repository) : base(repository)
        {
        }
    }
}

