using Accounting.Categories.Menus;
using Accounting.Constants;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class MenuAccountingService : BaseDomainService<MenuAccounting, string>
    {
        public MenuAccountingService(IRepository<MenuAccounting, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(MenuAccounting entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p =>  p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(MenuAccounting entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.MenuAccounting, ErrorCode.Duplicate),
                        $"Menu Code ['{entity.Code}'] already exist ");
            }
        }
        public async Task<MenuAccounting> GetByWindowIdAsync(string windowId)
        {
            var queyable = await this.GetQueryableAsync();
            queyable = queyable.Where(p => p.windowId == windowId);

            return await AsyncExecuter.FirstOrDefaultAsync(queyable);
        }
        public async Task<MenuAccounting> GetMenuAccountingByUrl(string url)
        {
            var menus = await this.GetRepository().ToListAsync();
            return menus.Where(p => GetClientUrlPath(p) == url)
                        .FirstOrDefault();
        }
        private string GetClientUrlPath(MenuAccounting menu)
        {
            if (string.IsNullOrEmpty(menu.JavaScriptCode)) return null;
            string url = $"/main/{menu.JavaScriptCode}";
            if (!string.IsNullOrEmpty(menu.windowId))
            {
                url = url + $"?windowid={menu.windowId}";
                return url;
            }
            url = url + $"?id={menu.Id}";            
            return url;
        }
    }
}
