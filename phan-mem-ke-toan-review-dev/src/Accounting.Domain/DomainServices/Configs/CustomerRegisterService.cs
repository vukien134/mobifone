using Accounting.Configs;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Configs
{
    public class CustomerRegisterService : BaseDomainService<CustomerRegister, string>
    {
        public CustomerRegisterService(IRepository<CustomerRegister, string> repository) : base(repository)
        {
        }
        public async Task<CustomerRegister> GetByAccessCode(string accessCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.AccessCode.Equals(accessCode));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<CustomerRegister> GetById(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id.Equals(id));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
