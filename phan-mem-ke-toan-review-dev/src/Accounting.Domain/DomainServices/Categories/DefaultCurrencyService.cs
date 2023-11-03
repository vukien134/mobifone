using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultCurrencyService : BaseDomainService<DefaultCurrency, string>
    {
        public DefaultCurrencyService(IRepository<DefaultCurrency, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultCurrency>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
        public async Task<DefaultCurrency> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code.Equals(code));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
