using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Units
{
    public interface IUnitAppService
    {
        Task<PageResultDto<UnitDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetDataReference();
        Task<UnitDto> CreateAsync(CrudUnitDto dto);
        Task UpdateAsync(string id, CrudUnitDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}