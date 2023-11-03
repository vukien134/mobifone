using Accounting.Categories.Contracts;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class ContractDetailService : BaseDomainService<ContractDetail, string>
    {
        public ContractDetailService(IRepository<ContractDetail, string> repository) : base(repository)
        {
        }
        public async Task<List<ContractDetail>> GetByContractIdAsync(string contractId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.ContractId == contractId);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
