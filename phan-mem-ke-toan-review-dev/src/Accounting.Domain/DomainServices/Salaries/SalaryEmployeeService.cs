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
    public class SalaryEmployeeService : BaseDomainService<SalaryEmployee, string>
    {
        public SalaryEmployeeService(IRepository<SalaryEmployee, string> repository) : base(repository)
        {
        }
        public IQueryable<SalaryEmployee> GetQueryableQuickSearch(IQueryable<SalaryEmployee> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.EmployeeCode, filterValue) || EF.Functions.ILike(p.SalaryCode, filterValue));
            return queryable;
        }
        public async Task<SalaryEmployee> GetSalary(string employeeCode,string salaryCode,string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode && p.EmployeeCode == employeeCode
                                    && p.SalaryCode == salaryCode);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<SalaryEmployee>> GetallbyOrgcode(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable= queryable.Where(p => p.OrgCode == orgCode);
            return queryable.ToList();
        }
        public async Task<List<SalaryEmployee>> getAll()
        {
            return await this.GetRepository().ToListAsync();
        }   
    }
}
