using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class FStatement133L02Service : BaseDomainService<TenantFStatement133L02, string>
    {
        public FStatement133L02Service(IRepository<TenantFStatement133L02, string> repository) : base(repository)
        {
        }

    }
}

