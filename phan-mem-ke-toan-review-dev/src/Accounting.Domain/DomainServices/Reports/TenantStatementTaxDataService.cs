using System;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports
{
    public class TenantStatementTaxDataService : BaseDomainService<TenantStatementTaxData, string>
    {
        public TenantStatementTaxDataService(IRepository<TenantStatementTaxData, string> repository) : base(repository)
        {
        }
    }
}

