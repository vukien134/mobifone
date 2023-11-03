using Accounting.Categories.CostProductions;
using Accounting.DomainServices.BaseServices;
using Accounting.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class GroupCoefficientDetailService : BaseDomainService<GroupCoefficientDetail, string>
    {
        private readonly WebHelper _webHelper;
        public GroupCoefficientDetailService(IRepository<GroupCoefficientDetail, string> repository, WebHelper webHelper) : base(repository)
        {
            _webHelper = webHelper;
        }
        public async Task<List<GroupCoefficientDetail>> GetByGroupCoefficientIdAsync(string groupCoefficientId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.GroupCoefficientId == groupCoefficientId && p.Year == _webHelper.GetCurrentYear());

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
