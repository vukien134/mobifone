using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Accounting.DomainServices.Windows
{
    public class WindowService : BaseDomainService<Window, string>
    {
        #region Fields
        private readonly FieldService _fieldService;
        private readonly IRepository<RegisterEvent> _registerEventRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly TabService _tabService;
        #endregion
        public WindowService(IRepository<Window, string> repository,
            FieldService fieldService,
            IRepository<RegisterEvent> registerEventRepository,
            IUnitOfWorkManager unitOfWorkManager,
            TabService tabService) 
            : base(repository)
        {
            _fieldService = fieldService;
            _registerEventRepository = registerEventRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _tabService = tabService;
        }
        public async Task<bool> IsExistCode(Window entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(Window entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Window, ErrorCode.Duplicate),
                        $"Window Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<Window> GetWithDetailAsync(string id)
        {
            var queryable = await  this.GetRepository().WithDetailsAsync(p => p.Tabs.OrderBy(t => t.Ord));
            queryable = queryable.Where(p => p.Id.Equals(id));
                                
            var window = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (window == null) return null;

            foreach(var tab in window.Tabs)
            {
                tab.Fields = await _fieldService.GetByTabIdAsync(tab.Id);
            }

            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<List<RegisterEvent>> GetRegisterEventAsync(string windowId)
        {
            var queryable = await _registerEventRepository.GetQueryableAsync();
            queryable = queryable.Where(p => p.WindowId == windowId);
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task Copy(string windowId,string code,string name)
        {
            var window = await this.GetAsync(windowId);
            try
            {
                window.SetId(ObjectHelper.NewId());
                window.Code = code;
                window.Name = name;
                using var unitOfWork = _unitOfWorkManager.Begin();
                await this.CreateAsync(window);
                await CopyTab(windowId, window.Id);
                //await CopyEventWindow(windowId, window.Id);
                await unitOfWork.CompleteAsync();
            }
            catch(Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }

        public async Task<Window> GetByIdAsync(string id)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == id);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<Window> GetByCodeAsync(string code)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == code);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        public async Task<Window> GetByVoucherCodeAsync(string voucherCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.VoucherCode.Equals(voucherCode));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
        private async Task CopyTab(string windowOldId,string windowNewId)
        {
            var tabs = await _tabService.GetByWindowIdAsync(windowOldId);
            foreach (Tab tab in tabs)
            {
                string tabId = tab.Id;
                tab.SetId(ObjectHelper.NewId());
                tab.WindowId = windowNewId;
                tab.Code = tab.Code + string.Format("{0:yyyyMMdd}", DateTime.Now);
                await _tabService.CreateAsync(tab);
                await CopyField(tabId, tab.Id);
                //await CopyEventTab(tabId, tab.Id);
            }
        }
        private async Task CopyField(string tabOldId,string tabNewId)
        {
            var fields = await _fieldService.GetByTabIdAsync(tabOldId);
            foreach (Field field in fields)
            {
                string fieldId = field.Id;
                field.SetId(ObjectHelper.NewId());
                field.TabId = tabNewId;
                await _fieldService.CreateAsync(field);
                //await CopyEventField(fieldId, field.Id);
            }
        }
        private async Task CopyEventWindow(string windowOldId,string windowNewId)
        {
            var registerEvents = await this.GetRegisterEventAsync(windowOldId);
            foreach (RegisterEvent item in registerEvents)
            {
                item.WindowId = windowNewId;
                item.SetId(ObjectHelper.NewId());
            }
        }
        private async Task CopyEventTab(string tabOldId,string tabNewId)
        {
            var registerEvents = await _tabService.GetRegisterEventAsync(tabOldId);
            foreach (RegisterEvent item in registerEvents)
            {
                item.TabId = tabNewId;
                item.SetId(ObjectHelper.NewId());
            }
        }
        private async Task CopyEventField(string fieldOldId, string fieldNewId)
        {
            var registerEvents = await _fieldService.GetRegisterEventAsync(fieldOldId);
            foreach (RegisterEvent item in registerEvents)
            {
                item.FieldId = fieldNewId;
                item.SetId(ObjectHelper.NewId());
            }
        }
    }
}
