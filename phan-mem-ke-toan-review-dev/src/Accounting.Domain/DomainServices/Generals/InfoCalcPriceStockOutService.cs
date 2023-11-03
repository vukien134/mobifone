using Accounting.DomainServices.BaseServices;
using Accounting.Generals;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Generals
{
    public class InfoCalcPriceStockOutService : BaseDomainService<InfoCalcPriceStockOut, string>
    {
        public InfoCalcPriceStockOutService(IRepository<InfoCalcPriceStockOut, string> repository) : base(repository)
        {
        }
        public async Task<List<InfoCalcPriceStockOut>> GetByInfoCalcPriceStockOutAsync(string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == ordCode && p.Status == "Chờ thực hiện");

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<InfoCalcPriceStockOut>> GetByInfoCalcPriceStockOutIdAsync(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id== id );

            return await AsyncExecuter.ToListAsync(queryable);
        }

    }
}
