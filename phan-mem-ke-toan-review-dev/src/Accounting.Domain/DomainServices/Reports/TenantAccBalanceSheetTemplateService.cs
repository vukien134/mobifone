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
    public class TenantAccBalanceSheetService : BaseDomainService<TenantAccBalanceSheet, string>
    {
        public TenantAccBalanceSheetService(IRepository<TenantAccBalanceSheet, string> repository) : base(repository)
        {
        }
        public async Task<List<TenantAccBalanceSheet>> GetAllAsync(string orgCode, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode) && p.Year.Equals(year));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<TenantAccBalanceSheet>> GetAllAsync(string orgCode, int year,int usingDecision)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode) && p.Year.Equals(year) && p.UsingDecision.Equals(usingDecision));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
