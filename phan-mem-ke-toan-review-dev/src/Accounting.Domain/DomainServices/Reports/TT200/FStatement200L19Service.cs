﻿using System;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports.Statements.T200.Tenants;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports.TT200
{
    public class FStatement200L19Service : BaseDomainService<TenantFStatement200L19, string>
    {
        public FStatement200L19Service(IRepository<TenantFStatement200L19, string> repository) : base(repository)
        {
        }
    }
}

