using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultVoucherCategoryService : BaseDomainService<DefaultVoucherCategory, string>
    {
        public DefaultVoucherCategoryService(IRepository<DefaultVoucherCategory, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultVoucherCategory>> GetListAsync()
        {
            return await this.GetRepository().ToListAsync();
        }
        public async Task<List<DefaultVoucherCategory>> GetListByTenantTypeAsync(int? tenantType)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.TenantType == tenantType);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<DefaultVoucherCategory> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code.Equals(code));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
