using Accounting.Categories.Partners;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class BankPartnerService : BaseDomainService<BankPartner, string>
    {
        public BankPartnerService(IRepository<BankPartner, string> repository) : base(repository)
        {
        }
        public async Task<List<BankPartner>> GetByAccPartnerIdAsync(string partnerId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.PartnerId == partnerId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
