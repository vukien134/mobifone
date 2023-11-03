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
    public class DefaultStatementTaxService : BaseDomainService<DefaultStatementTax, string>
    {
        public DefaultStatementTaxService(IRepository<DefaultStatementTax, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultStatementTax>> GetAllAsync()
        {
            var queryable = await this.GetQueryableAsync();
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}

