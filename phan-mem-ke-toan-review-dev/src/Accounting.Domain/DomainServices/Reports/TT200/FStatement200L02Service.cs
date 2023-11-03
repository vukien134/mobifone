using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L02Service : BaseDomainService<TenantFStatement200L02, string>
    {
        public FStatement200L02Service(IRepository<TenantFStatement200L02, string> repository) : base(repository)
        {
        }
    }
}

