using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Sections
{
    public interface IAccSectionAppService
    {
        Task<PageResultDto<AccSectionDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AccSectionDto>> GetListAsync(PageRequestDto dto);
        Task<AccSectionDto> GetByIdAsync(string sectionId);
        Task<List<AccSectionComboItemDto>> DataReference(ComboRequestDto dto);
        Task<AccSectionDto> CreateAsync(CruAccSectionDto dto);
        Task UpdateAsync(string id, CruAccSectionDto dto);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
