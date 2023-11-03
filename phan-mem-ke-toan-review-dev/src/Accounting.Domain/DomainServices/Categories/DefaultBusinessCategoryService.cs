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
    public class DefaultBusinessCategoryService : BaseDomainService<DefaultBusinessCategory, string>
    {
        public DefaultBusinessCategoryService(IRepository<DefaultBusinessCategory, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultBusinessCategory>> GetListAsync(int? tenantType)
        {
            var queryable = await this.GetQueryableAsync();
            if (tenantType != null)
            {
                queryable = queryable.Where(p => p.TenantType == tenantType);
            }
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<DefaultBusinessCategory> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code.Equals(code));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
