using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L13Service : BaseDomainService<TenantFStatement200L13, string>
    {
        public FStatement200L13Service(IRepository<TenantFStatement200L13, string> repository) : base(repository)
        {
        }
    }
}

