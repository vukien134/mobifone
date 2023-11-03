using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Reports;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Reports
{
    public class DefaultHtkkNumberCodeService : BaseDomainService<DefaultHtkkNumberCode, string>
    {
        public DefaultHtkkNumberCodeService(IRepository<DefaultHtkkNumberCode, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultHtkkNumberCode>> GetAllAsync()
        {
            var queryable = await this.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}

