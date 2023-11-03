using Accounting.DomainServices.BaseServices;
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
    public class DefaultAccBalanceSheetService : BaseDomainService<DefaultAccBalanceSheet, string>
    {
        public DefaultAccBalanceSheetService(IRepository<DefaultAccBalanceSheet, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultAccBalanceSheet>> GetAllAsync()
        {
            var queryable = await this.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<DefaultAccBalanceSheet>> GetAllAsync(int usingDecision)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.UsingDecision.Equals(usingDecision));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
