using Accounting.BaseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Menus
{
    public interface IMenuAccountingAppService
    {
        Task<PageResultDto<MenuAccountingDto>> GetListAsync(PageRequestDto dto);
        Task<List<MenuWebixDto>> GetMenuWebixByUserAsync();
        Task<MenuAccountingDto> CreateAsync(CruMenuAccountingDto dto);
        Task<MenuAccountingDto> GetByIdAsync(string menuId);
        Task UpdateAsync(string id, CruMenuAccountingDto dto);
        Task DeleteAsync(string id);
    }
}
