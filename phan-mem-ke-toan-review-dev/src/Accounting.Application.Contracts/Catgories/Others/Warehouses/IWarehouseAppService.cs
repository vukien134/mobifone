using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.Warehouses
{
    public interface IWarehouseAppService
    {
        Task<PageResultDto<WarehouseDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<WarehouseDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<WarehouseDto> GetByIdAsync(string caseId);
        Task<WarehouseDto> CreateAsync(CrudWarehouseDto dto);
        Task UpdateAsync(string id, CrudWarehouseDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
