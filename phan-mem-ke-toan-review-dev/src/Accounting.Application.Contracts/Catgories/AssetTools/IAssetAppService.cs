using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines.AssetTool;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.AssetTools
{
    public interface IAssetAppService
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
        Task<ResultDto> AssetToolAllocation(AssetToolAllocationDto dto);
        Task<ResultDto> CreateVoucherAssetTool(AssetToolAllocationDto dto);
        //Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile file, [FromForm] ExcelRequestDto dto);
    }
}
