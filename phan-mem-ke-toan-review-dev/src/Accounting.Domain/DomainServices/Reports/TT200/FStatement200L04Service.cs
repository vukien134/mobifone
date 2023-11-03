using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L04Service : BaseDomainService<TenantFStatement200L04, string>
    {
        public FStatement200L04Service(IRepository<TenantFStatement200L04, string> repository) : base(repository)
        {
        }

    }
}

