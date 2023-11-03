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
    public class ReportMenuShortcutService : BaseDomainService<ReportMenuShortcut, string>
    {
        public ReportMenuShortcutService(IRepository<ReportMenuShortcut, string> repository) : base(repository)
        {
        }
        public async Task<List<ReportMenuShortcut>> GetByReportIdAsync(string reportId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OriginReportId.Equals(reportId));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ReportMenuShortcut>> GetByWindowIdAsync(string windowId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ReferenceWindowId.Equals(windowId));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
