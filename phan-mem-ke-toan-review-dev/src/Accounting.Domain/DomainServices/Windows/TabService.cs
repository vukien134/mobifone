using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Windows
{
    public class TabService : BaseDomainService<Tab, string>
    {
        #region Fields
        private readonly IRepository<RegisterEvent,string> _registerEventRepository;
        #endregion
        public TabService(IRepository<Tab, string> repository,
                        IRepository<RegisterEvent, string> registerEventRepository) 
            : base(repository)
        {
            _registerEventRepository = registerEventRepository;
        }
        public async Task<bool> IsExistCode(Tab entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(Tab entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Tab, ErrorCode.Duplicate),
                        $"Tab Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<List<RegisterEvent>> GetRegisterEventAsync(string tabId)
        {
            var queryable = await _registerEventRepository.GetQueryableAsync();
            queryable = queryable.Where(p => p.TabId == tabId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<List<Tab>> GetByWindowIdAsync(string windowId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.WindowId == windowId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
