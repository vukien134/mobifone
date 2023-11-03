using Accounting.Categories.Others;
using Accounting.Categories.Salaries;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultAccSectionService : BaseDomainService<DefaultAccSection, string>
    {
        public DefaultAccSectionService(IRepository<DefaultAccSection, string> repository) : base(repository)
        {
        }
        public async Task<DefaultAccSection> GetById(string Id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == Id);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<List<DefaultAccSection>> GetList()
        {
            return await this.GetRepository().ToListAsync();
        }
    }
}
