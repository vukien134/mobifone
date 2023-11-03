using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class FStatement133L01Service : BaseDomainService<TenantFStatement133L01, string>
    {
        public FStatement133L01Service(IRepository<TenantFStatement133L01, string> repository) : base(repository)
        {
        }
    }
}

