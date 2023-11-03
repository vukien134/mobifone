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
    public class SalaryBookService : BaseDomainService<SalaryBook, string>
    {
        public SalaryBookService(IRepository<SalaryBook, string> repository) : base(repository)
        {
        }
        public async Task<List<SalaryBook>> GetSalaryBooks(string salarySheetTypeId,string salaryPeriodId,string departmentCode = null)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.SalarySheetTypeId == salarySheetTypeId
                                        && p.SalaryPeriodId == salaryPeriodId);
            if (!string.IsNullOrEmpty(departmentCode))
            {
                queryable = queryable.Where(p => p.DepartmentCode == departmentCode);
            }
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
