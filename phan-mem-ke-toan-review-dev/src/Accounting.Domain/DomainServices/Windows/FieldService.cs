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
    public class FieldService : BaseDomainService<Field, string>
    {
        #region Fields
        private readonly IRepository<RegisterEvent, string> _registerEventRepository;
        #endregion
        public FieldService(IRepository<Field, string> repository,
                        IRepository<RegisterEvent, string> registerEventRepository) 
            : base(repository)
        {
            _registerEventRepository = registerEventRepository;
        }
        public async Task<List<Field>> GetByTabIdAsync(string tabId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.TabId == tabId)
                            .OrderBy(p => p.Ord);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<RegisterEvent>> GetRegisterEventAsync(string fieldId)
        {
            var queryable = await _registerEventRepository.GetQueryableAsync();
            queryable = queryable.Where(p => p.FieldId == fieldId);
            return await AsyncExecuter.ToListAsync(queryable);
        }        
    }
}
