using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L03Service : BaseDomainService<TenantFStatement200L03, string>
    {
        public FStatement200L03Service(IRepository<TenantFStatement200L03, string> repository) : base(repository)
        {
        }
    }
}

