using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L11Service : BaseDomainService<TenantFStatement200L11, string>
    {
        public FStatement200L11Service(IRepository<TenantFStatement200L11, string> repository) : base(repository)
        {
        }
    }
}

