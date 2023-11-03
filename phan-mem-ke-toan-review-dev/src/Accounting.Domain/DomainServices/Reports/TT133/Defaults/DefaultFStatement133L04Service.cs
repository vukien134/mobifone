using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Reports.Statements.T133.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT133
{
    public class DefaultFStatement133L04Service : BaseDomainService<DefaultFStatement133L04, string>
    {
        public DefaultFStatement133L04Service(IRepository<DefaultFStatement133L04, string> repository) : base(repository)
        {
        }
    }
}

