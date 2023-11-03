using Accounting.DomainServices.BaseServices;
using Accounting.Licenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Licenses
{
    public class TenantLicenseService : BaseDomainService<TenantLicense, string>
    {
        public TenantLicenseService(IRepository<TenantLicense, string> repository) : base(repository)
        {
        }
        public async Task<TenantLicense> GetByTenantIdAsync(Guid? tenantId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.TenantId == tenantId);
            return queryable.FirstOrDefault();
        }
        public async Task<bool> IsExitTenantId(Guid? tenantId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.TenantId == tenantId);
        }
    }
}
