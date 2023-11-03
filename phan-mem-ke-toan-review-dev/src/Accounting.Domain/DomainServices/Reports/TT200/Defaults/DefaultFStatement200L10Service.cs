
using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L10Service : BaseDomainService<DefaultFStatement200L10, string>
    {
        public DefaultFStatement200L10Service(IRepository<DefaultFStatement200L10, string> repository) : base(repository)
        {
        }
    }
}

