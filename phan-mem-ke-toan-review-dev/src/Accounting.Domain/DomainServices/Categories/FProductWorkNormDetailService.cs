using Accounting.Categories.Contracts;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class FProductWorkNormDetailService : BaseDomainService<FProductWorkNormDetail, string>
    {
        public FProductWorkNormDetailService(IRepository<FProductWorkNormDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<FProductWorkNormDetail>> GetByFProductWorkNormIdAsync(string fProductWorkNormId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.FProductWorkNormId == fProductWorkNormId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
