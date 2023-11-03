using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports
{
    public class TenantStatementTaxService : BaseDomainService<TenantStatementTax, string>
    {
        public TenantStatementTaxService(IRepository<TenantStatementTax, string> repository) : base(repository)
        {
        }
        public async Task<List<TenantStatementTax>> GetAllAsync(string orgCode, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode) && p.Year.Equals(year));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}

