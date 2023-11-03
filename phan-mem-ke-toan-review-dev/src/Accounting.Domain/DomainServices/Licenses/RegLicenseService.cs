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
    public class RegLicenseService : BaseDomainService<RegLicense, string>
    {
        public RegLicenseService(IRepository<RegLicense, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsApproval(Guid teantId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.CustomerTenantId == teantId && p.IsApproval == true);
        }
        public async Task<RegLicense> GetByTenantId(Guid? tenantId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.CustomerTenantId == tenantId);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<bool> IsRegisted(Guid? tenantId)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.CustomerTenantId == tenantId);
        }
    }
}
