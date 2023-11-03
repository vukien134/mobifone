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
    public class VoucherTemplateService : BaseDomainService<VoucherTemplate, string>
    {
        public VoucherTemplateService(IRepository<VoucherTemplate, string> repository) : base(repository)
        {
        }
        public async Task<List<VoucherTemplate>> GetByWindowIdAsync(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.WindowId == id);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<VoucherTemplate> GetByIdAsync(string id)
        {
            return await this.GetRepository().FindAsync(id);
        }
    }
}
