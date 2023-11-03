using Accounting.DomainServices.BaseServices;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class InfoWindowService : BaseDomainService<InfoWindow, string>
    {
        public InfoWindowService(IRepository<InfoWindow, string> repository) : base(repository)
        {
        }
        public async Task<InfoWindow> GetWithDetailByIdAsync(string id)
        {
            var queryable = await this.GetRepository().WithDetailsAsync(p => p.InfoWindowDetails.OrderBy(t => t.Ord));
            queryable = queryable.Where(p => p.Id.Equals(id));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
