using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L02Service : BaseDomainService<DefaultFStatement133L02, string>
    {
        public DefaultFStatement133L02Service(IRepository<DefaultFStatement133L02, string> repository) : base(repository)
        {
        }

    }
}

