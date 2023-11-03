using Accounting.Categories.Salaries;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Salaries
{
    public class DefaultSalaryCategoryService : BaseDomainService<DefaultSalaryCategory, string>
    {
        public DefaultSalaryCategoryService(IRepository<DefaultSalaryCategory, string> repository) : base(repository)
        {
        }
        public async Task<DefaultSalaryCategory> GetById(string Id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == Id);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<List<DefaultSalaryCategory>> GetList()
        {
            return await this.GetRepository().ToListAsync();
        }
    }
}
