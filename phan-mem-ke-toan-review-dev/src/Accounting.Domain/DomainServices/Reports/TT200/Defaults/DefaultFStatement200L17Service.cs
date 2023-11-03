
using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L17Service : BaseDomainService<DefaultFStatement200L17, string>
    {
        public DefaultFStatement200L17Service(IRepository<DefaultFStatement200L17, string> repository) : base(repository)
        {
        }
    }
}

