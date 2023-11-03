using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.FProductWorks
{
    public interface IFProductWorkAppService
    {
        Task<PageResultDto<FProductWorkDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<FProductWorkDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<FProductWorkDto> CreateAsync(CrudFProductWorkDto dto);
        Task UpdateAsync(string id, CrudFProductWorkDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile file, [FromForm] ExcelRequestDto dto);
    }
}
