using Accounting.BaseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Accounts
{
    public interface IAccountSystemAppService
    {
        Task<PageResultDto<AccountSystemDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AccountSystemDto>> GetListAsync(PageRequestDto dto);
        Task<List<AccountSystemComboItemDto>> GetViewListByCodeAsync();
        Task<List<AccountSystemComboItemDto>> GetViewListAsync();
        Task<List<AccountSystemTreeItemDto>> GetViewTreeByCodeAsync();
        Task<AccountSystemDto> CreateAsync(CruAccountSystemDto dto);
        Task UpdateAsync(string id, CruAccountSystemDto dto);
        Task DeleteAsync(string id);
    }
}
