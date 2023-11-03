using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.Departments
{
    public interface IDepartmentAppService
    {
        Task<PageResultDto<DepartmentDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<DepartmentDto>> GetListAsync(PageRequestDto dto);
        Task<DepartmentDto> GetByIdAsync(string partnerGroupId);
        Task<List<BaseComboItemDto>> GetViewListAsync();
        Task<List<BaseComboItemDto>> GetViewListCodeAsync();
        Task<DepartmentDto> CreateAsync(CrudDepartmentDto dto);
        Task UpdateAsync(string id, CrudDepartmentDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
