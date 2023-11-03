using Accounting.Categories.Others.PaymentTerms;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class PaymentTermDetailService : BaseDomainService<PaymentTermDetail, string>
    {
        public PaymentTermDetailService(IRepository<PaymentTermDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<PaymentTermDetail>> GetByPamentTermId(string paymentTermId)
        {
            var queryable = await this.GetRepository().GetQueryableAsync();
            queryable = queryable.Where(p => p.PaymentTermId == paymentTermId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
