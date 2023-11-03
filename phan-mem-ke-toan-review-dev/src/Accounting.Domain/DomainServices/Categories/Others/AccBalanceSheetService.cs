using System;
using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class AccBalanceSheetService : BaseDomainService<AccBalanceSheet, string>
    {
        public AccBalanceSheetService(IRepository<AccBalanceSheet, string> repository) : base(repository)
        {
        }
    }
}

