﻿using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Defaults;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class DefaultFStatement200L14Service : BaseDomainService<DefaultFStatement200L14, string>
    {
        public DefaultFStatement200L14Service(IRepository<DefaultFStatement200L14, string> repository) : base(repository)
        {
        }
    }
}

