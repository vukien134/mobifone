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
    public class DefaultSalarySheetTypeDetailService : BaseDomainService<DefaultSalarySheetTypeDetail, string>
    {
        public DefaultSalarySheetTypeDetailService(IRepository<DefaultSalarySheetTypeDetail, string> repository) : base(repository)
        {
        }
        public async Task<DefaultSalarySheetTypeDetail> GetById(string Id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == Id);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<DefaultSalarySheetTypeDetail>> GetList()
        {
            return await this.GetRepository().ToListAsync();
        }
        public async Task<List<DefaultSalarySheetTypeDetail>> GetByDetailId(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.SalarySheetTypeId == id);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
