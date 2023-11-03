using Accounting.Categories.CostProductions;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.CostProduction
{
    public class DefaultAllotmentForwardCategoryService : BaseDomainService<DefaultAllotmentForwardCategory, string>
    {
        public DefaultAllotmentForwardCategoryService(IRepository<DefaultAllotmentForwardCategory, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultAllotmentForwardCategory>> GetListAsync(string type, string productOrWork, int usingDecision,
                                                        string ordGrp)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Type == type && p.FProductWork == productOrWork
                                && p.DecideApply == usingDecision);
            if (!string.IsNullOrEmpty(ordGrp))
            {
                queryable = queryable.Where(p => p.OrdGrp == ordGrp);
            }
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
