
using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L17Service : BaseDomainService<TenantFStatement200L17, string>
    {
        public FStatement200L17Service(IRepository<TenantFStatement200L17, string> repository) : base(repository)
        {
        }
    }
}

