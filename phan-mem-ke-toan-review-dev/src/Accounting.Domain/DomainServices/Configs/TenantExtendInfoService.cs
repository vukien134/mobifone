using Accounting.DomainServices.BaseServices;
using Accounting.Tenants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Configs
{
    public class TenantExtendInfoService : BaseDomainService<TenantExtendInfo, string>
    {
        public TenantExtendInfoService(IRepository<TenantExtendInfo, string> repository) : base(repository)
        {
        }
        public async Task<TenantExtendInfo> GetByTenantId(Guid? tenantId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.TenantId == tenantId);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<int?> GetTenantType(Guid? tenantId)
        {
            var tenantExtendInfo = await this.GetByTenantId(tenantId);
            return tenantExtendInfo?.TenantType;
        }
    }
}
