using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultTaxCategoryService : BaseDomainService<DefaultTaxCategory, string>
    {
        public DefaultTaxCategoryService(IRepository<DefaultTaxCategory, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultTaxCategory>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
        public async Task<DefaultTaxCategory> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
