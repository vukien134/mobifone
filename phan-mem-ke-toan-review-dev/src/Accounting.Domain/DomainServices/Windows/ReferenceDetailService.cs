using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class ReferenceDetailService : BaseDomainService<ReferenceDetail, string>
    {
        public ReferenceDetailService(IRepository<ReferenceDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<ReferenceDetail>> GetByReferenceIdAsync(string referenceId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ReferenceId == referenceId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
