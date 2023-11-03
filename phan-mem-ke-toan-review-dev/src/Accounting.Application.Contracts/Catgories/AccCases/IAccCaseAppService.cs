using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.AccCases
{
    public interface IAccCaseAppService
    {
        Task<PageResultDto<AccCaseDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AccCaseDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<AccCaseDto> GetByIdAsync(string caseId);
        Task<AccCaseDto> CreateAsync(CrudAccCaseDto dto);
        Task UpdateAsync(string id, CrudAccCaseDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile file, [FromForm] ExcelRequestDto dto);
    }
}
