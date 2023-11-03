using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines.AssetTool;
using Accounting.Catgories.AccCases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.AssetTools
{
    public interface IAssetToolDepreciationAppService
    {
        Task<AssetToolDepreciationDto> CreateAsync(CrudAssetToolDepreciationDto dto);
        Task DeleteAsync(string id);
        Task<PageResultDto<AssetToolDepreciationDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AssetToolDepreciationDto>> GetListAsync(PageRequestDto dto);
        Task UpdateAsync(string id, CrudAssetToolDepreciationDto dto);
        Task<AssetToolDepreciationDto> GetByIdAsync(string assetToolId);
    }
}
