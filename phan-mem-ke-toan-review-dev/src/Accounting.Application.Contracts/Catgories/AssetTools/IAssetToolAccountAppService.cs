using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines.AssetTool;
using Accounting.Catgories.AccCases;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.AssetTools
{
    public interface IAssetToolAccountAppService
    {
        Task<AssetToolAccountDto> CreateAsync(CrudAssetToolAccountDto dto);
        Task DeleteAsync(string id);
        Task<PageResultDto<AssetToolAccountDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AssetToolAccountDto>> GetListAsync(PageRequestDto dto);
        Task UpdateAsync(string id, CrudAssetToolAccountDto dto);
        Task<AssetToolAccountDto> GetByIdAsync(string assetToolId);
        Task<List<AssetToolAccountDto>> CreateListAsync(ObjectListAssetToolAccountDto listDto);
        Task<List<AssetToolAccountDto>> GetListByAssetToolIdAsync(string assetToolId);
    }
}
