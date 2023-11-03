using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L05Service : BaseDomainService<TenantFStatement200L05, string>
    {
        public FStatement200L05Service(IRepository<TenantFStatement200L05, string> repository) : base(repository)
        {
        }

    }
}

