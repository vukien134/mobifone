using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class FStatement133L03Service : BaseDomainService<TenantFStatement133L03, string>
    {
        public FStatement133L03Service(IRepository<TenantFStatement133L03, string> repository) : base(repository)
        {
        }
    }
}

