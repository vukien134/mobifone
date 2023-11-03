using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class ButtonService : BaseDomainService<Button, string>
    {
        public ButtonService(IRepository<Button, string> repository) : base(repository)
        {
        }

        public async Task<List<Button>> GetByWindowIdAsync(string windowId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.WindowId == windowId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<Button>> GetByReportIdAsync(string reportId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ReportTemplateId == reportId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
