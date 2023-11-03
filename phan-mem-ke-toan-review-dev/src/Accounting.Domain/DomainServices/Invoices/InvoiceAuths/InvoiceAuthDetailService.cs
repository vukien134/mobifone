using Accounting.Categories.Contracts;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using Accounting.Invoices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Invoices.InvoiceAuths
{
    public class InvoiceAuthDetailService : BaseDomainService<InvoiceAuthDetail, string>
    {
        public InvoiceAuthDetailService(IRepository<InvoiceAuthDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<InvoiceAuthDetail>> GetByInvoiceAuthIdAsync(string invoiceAuthId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.InvoiceAuthId == invoiceAuthId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
