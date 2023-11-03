﻿using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports
{
    public class DefaultCashFollowStatementService : BaseDomainService<DefaultCashFollowStatement, string>
    {
        public DefaultCashFollowStatementService(IRepository<DefaultCashFollowStatement, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultCashFollowStatement>> GetAllAsync()
        {
            var queryable = await this.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<DefaultCashFollowStatement>> GetAllAsync(int usingDecision)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.UsingDecision.Equals(usingDecision));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
