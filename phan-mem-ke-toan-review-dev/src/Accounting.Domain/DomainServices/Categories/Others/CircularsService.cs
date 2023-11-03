using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class CircularsService : BaseDomainService<Circulars, string>
    {
        public CircularsService(IRepository<Circulars, string> repository) : base(repository)
        {
        }
        public async Task<Circulars> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();   
            queryable = queryable.Where(p => p.Code.Equals(code));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
