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
    public class ReportTemplateService : BaseDomainService<ReportTemplate, string>
    {
        public ReportTemplateService(IRepository<ReportTemplate, string> repository) : base(repository)
        {
        }
        public async Task<ReportTemplate> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code.Equals(code));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
