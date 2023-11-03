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
    public class SalarySheetTypeService : BaseDomainService<SalarySheetType, string>
    {
        public SalarySheetTypeService(IRepository<SalarySheetType, string> repository) : base(repository)
        {
        }
        public IQueryable<SalarySheetType> GetQueryableQuickSearch(IQueryable<SalarySheetType> queryable, string filterValue)
        {
            queryable = queryable.Where(p => EF.Functions.ILike(p.Code, filterValue) || EF.Functions.ILike(p.Name, filterValue));
            return queryable;
        }
        public async Task<List<SalarySheetType>> GetDataReference(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode)
                                .OrderBy(p => p.Code);
            var partnes = await AsyncExecuter.ToListAsync(queryable);
            return partnes;
        }
    }
}
