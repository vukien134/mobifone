using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L01Service : BaseDomainService<DefaultFStatement133L01, string>
    {
        public DefaultFStatement133L01Service(IRepository<DefaultFStatement133L01, string> repository) : base(repository)
        {
        }
    }
}

