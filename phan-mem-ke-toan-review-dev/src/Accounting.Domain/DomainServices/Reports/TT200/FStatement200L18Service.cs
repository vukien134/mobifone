using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L18Service : BaseDomainService<TenantFStatement200L18, string>
    {
        public FStatement200L18Service(IRepository<TenantFStatement200L18, string> repository) : base(repository)
        {
        }
    }
}

