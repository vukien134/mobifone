using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports
{
    public class ReportTemplateColumnService : BaseDomainService<ReportTemplateColumn, string>
    {
        public ReportTemplateColumnService(IRepository<ReportTemplateColumn, string> repository) : base(repository)
        {
        }
        public async Task<List<ReportTemplateColumn>> GetByReportTemplateIdAsync(string reportTemplateId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ReportTemplateId == reportTemplateId);
            queryable = queryable.OrderBy(p => p.Ord);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
