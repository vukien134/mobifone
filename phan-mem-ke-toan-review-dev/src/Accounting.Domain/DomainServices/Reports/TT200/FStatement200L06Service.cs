using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L06Service : BaseDomainService<TenantFStatement200L06, string>
    {
        public FStatement200L06Service(IRepository<TenantFStatement200L06, string> repository) : base(repository)
        {
        }
    }
}

