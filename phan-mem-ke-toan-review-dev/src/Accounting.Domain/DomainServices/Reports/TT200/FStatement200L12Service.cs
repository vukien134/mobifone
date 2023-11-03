using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L12Service : BaseDomainService<TenantFStatement200L12, string>
    {
        public FStatement200L12Service(IRepository<TenantFStatement200L12, string> repository) : base(repository)
        {
        }
    }
}

