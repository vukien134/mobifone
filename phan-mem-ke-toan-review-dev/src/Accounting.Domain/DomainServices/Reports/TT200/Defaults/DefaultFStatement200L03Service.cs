using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L03Service : BaseDomainService<DefaultFStatement200L03, string>
    {
        public DefaultFStatement200L03Service(IRepository<DefaultFStatement200L03, string> repository) : base(repository)
        {
        }
    }
}

