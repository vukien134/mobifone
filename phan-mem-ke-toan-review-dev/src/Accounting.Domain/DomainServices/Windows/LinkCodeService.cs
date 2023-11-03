using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class LinkCodeService : BaseDomainService<LinkCode, string>
    {
        public LinkCodeService(IRepository<LinkCode, string> repository) : base(repository)
        {
        }
        public async Task<List<LinkCode>> GetListAsync(string fieldCode)
        {
            return await this.GetRepository().GetListAsync(p => p.FieldCode.Equals(fieldCode));
        }
    }
}

