using Accounting.Categories.AssetTools;
using Accounting.DomainServices.BaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.AssetTools
{
    public class DefaultCapitalService : BaseDomainService<DefaultCapital, string>
    {
        public DefaultCapitalService(IRepository<DefaultCapital, string> repository) : base(repository)
        {
        }
        public async Task<List<DefaultCapital>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
    }
}
