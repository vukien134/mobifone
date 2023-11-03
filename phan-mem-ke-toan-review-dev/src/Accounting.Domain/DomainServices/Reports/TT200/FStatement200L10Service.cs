
using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L10Service : BaseDomainService<TenantFStatement200L10, string>
    {
        public FStatement200L10Service(IRepository<TenantFStatement200L10, string> repository) : base(repository)
        {
        }
    }
}

