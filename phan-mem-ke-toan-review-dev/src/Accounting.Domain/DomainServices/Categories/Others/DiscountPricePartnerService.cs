using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class DiscountPricePartnerService : BaseDomainService<DiscountPricePartner, string>
    {
        public DiscountPricePartnerService(IRepository<DiscountPricePartner, string> repository) 
            : base(repository)
        {
        }
        public async Task<List<DiscountPricePartner>> GetByDiscountPriceIdAsync(string discountPriceId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.DiscountPriceId == discountPriceId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
