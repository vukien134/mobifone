using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.AssetTools
{
    public interface IAssetGroupAppService
    {
        Task<PageResultDto<AssetToolGroupDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AssetToolGroupDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetViewListByCodeAsync();
        Task<List<BaseComboItemDto>> GetViewListAsync();
        Task<List<BaseComboItemDto>> GetViewTreeByCodeAsync();
        Task<AssetToolGroupDto> CreateAsync(CrudAssetToolGroupDto dto);
        Task UpdateAsync(string id, CrudAssetToolGroupDto dto);
        Task DeleteAsync(string id);
    }
}
