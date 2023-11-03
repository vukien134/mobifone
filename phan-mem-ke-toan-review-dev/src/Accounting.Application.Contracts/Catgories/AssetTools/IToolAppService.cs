using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.AssetTools
{
    public interface IToolAppService
    {
        Task<AssetToolDto> CreateAsync(CrudAssetToolDto dto);
        Task DeleteAsync(string id);
        Task<PageResultDto<AssetToolDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AssetToolDto>> GetListAsync(PageRequestDto dto);
        Task UpdateAsync(string id, CrudAssetToolDto dto);
        Task<AssetToolDto> GetByIdAsync(string assetToolId);
        Task<List<AssetToolDetailDto>> GetAssetToolDetailAsync(string productId);
        Task<List<AssetToolAccessoryDto>> GetAssetToolAccessoryAsync(string productId);
        Task<List<AssetToolStoppingDepreciationDto>> GetAssetToolStoppingDepreciationAsync(string productId);
    }
}
