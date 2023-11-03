using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L09Service : BaseDomainService<TenantFStatement200L09, string>
    {
        public FStatement200L09Service(IRepository<TenantFStatement200L09, string> repository) : base(repository)
        {
        }
    }
}

