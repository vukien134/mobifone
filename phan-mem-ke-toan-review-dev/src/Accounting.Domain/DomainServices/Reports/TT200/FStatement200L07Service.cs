using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L07Service : BaseDomainService<TenantFStatement200L07, string>
    {
        public FStatement200L07Service(IRepository<TenantFStatement200L07, string> repository) : base(repository)
        {
        }
    }
}

