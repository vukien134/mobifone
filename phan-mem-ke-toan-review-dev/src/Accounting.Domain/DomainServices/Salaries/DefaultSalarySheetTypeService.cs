using Accounting.Categories.Salaries;
using Accounting.DomainServices.BaseServices;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Salaries
{
    public class DefaultSalarySheetTypeService : BaseDomainService<DefaultSalarySheetType, string>
    {
        public DefaultSalarySheetTypeService(IRepository<DefaultSalarySheetType, string> repository) : base(repository)
        {
        }

        public async Task<DefaultSalarySheetType> GetById(string Id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == Id);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<List<DefaultSalarySheetType>> GetList()
        {
           return await this.GetRepository().ToListAsync();
        }
    }
}
