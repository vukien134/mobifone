using Accounting.Configs;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Configs
{
    public class DbServerService : BaseDomainService<DbServer, string>
    {
        public DbServerService(IRepository<DbServer, string> repository) : base(repository)
        {
        }
        public async Task<DbServer> GetActiveServer(bool? isDemo)
        {
            var queryable = await this.GetQueryableAsync();
            if ((isDemo ?? false) == true)
            {
                queryable = queryable.Where(p => p.IsActive.Value == true && p.IsDemo == true).OrderBy(p => p.Ord);
            }
            else
            {
                queryable = queryable.Where(p => p.IsActive.Value == true && p.IsDemo != true).OrderBy(p => p.Ord);
            }
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
