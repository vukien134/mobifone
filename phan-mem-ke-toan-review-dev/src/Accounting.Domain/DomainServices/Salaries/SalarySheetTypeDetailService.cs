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
    public class SalarySheetTypeDetailService : BaseDomainService<SalarySheetTypeDetail, string>
    {
        public SalarySheetTypeDetailService(IRepository<SalarySheetTypeDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<SalarySheetTypeDetail>> GetBySalarySheetTypeIdAsync(string salarySheetId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.SalarySheetTypeId == salarySheetId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
