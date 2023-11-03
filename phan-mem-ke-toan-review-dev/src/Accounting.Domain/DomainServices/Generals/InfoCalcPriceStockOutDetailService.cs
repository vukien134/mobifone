using Accounting.DomainServices.BaseServices;
using Accounting.Generals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Generals
{
    public class InfoCalcPriceStockOutDetailService : BaseDomainService<InfoCalcPriceStockOutDetail, string>
    {
        public InfoCalcPriceStockOutDetailService(IRepository<InfoCalcPriceStockOutDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<InfoCalcPriceStockOutDetail>> GetByInfoCalcPriceStockOutAsync(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.InfoCalcPriceStockOutId == id);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
