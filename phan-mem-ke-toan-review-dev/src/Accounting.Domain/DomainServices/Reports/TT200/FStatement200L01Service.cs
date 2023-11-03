using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L01Service : BaseDomainService<TenantFStatement200L01, string>
    {
        public FStatement200L01Service(IRepository<TenantFStatement200L01, string> repository) : base(repository)
        {
        }
    }
}

