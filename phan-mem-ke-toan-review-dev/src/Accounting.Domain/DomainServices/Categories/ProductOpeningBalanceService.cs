using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;
using Accounting.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ProductOpeningBalanceService : BaseDomainService<ProductOpeningBalance, string>
    {
        private readonly WebHelper _webHelper;
        public ProductOpeningBalanceService(IRepository<ProductOpeningBalance, string> repository, WebHelper webHelper)
            : base(repository)
        {
            _webHelper = webHelper;
        }
        public async Task<List<ProductOpeningBalance>> GetByProductOpeningBalanceAsync(string WarehouseCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.WarehouseCode == WarehouseCode &&
                                             p.OrgCode == _webHelper.GetCurrentOrgUnit() &&
                                             p.Year == _webHelper.GetCurrentYear());
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductOpeningBalance>> GetByProductOpeningBalanceAsync(string coDe, string ordCode, string ProductLotCode, string WarehouseCode, string ProductOriginCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ProductCode == coDe && p.OrgCode == ordCode && p.WarehouseCode == WarehouseCode && p.ProductLotCode == ProductLotCode && p.ProductOriginCode == ProductOriginCode);

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<ProductOpeningBalance>> GetByListProductOpeningBalanceAsync(string ordCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p =>  p.OrgCode == ordCode );

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}