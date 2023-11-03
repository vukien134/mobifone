using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L03Service : BaseDomainService<DefaultFStatement133L03, string>
    {
        public DefaultFStatement133L03Service(IRepository<DefaultFStatement133L03, string> repository) : base(repository)
        {
        }
    }
}

