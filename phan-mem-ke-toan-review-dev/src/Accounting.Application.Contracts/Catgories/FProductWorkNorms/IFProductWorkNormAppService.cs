using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Catgories.Customines;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.FProductWorkNorms
{
    public interface IFProductWorkNormAppService
    {
        Task<PageResultDto<FProductWorkNormCustomineDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<FProductWorkNormCustomineDto>> GetListAsync(PageRequestDto dto);
        Task<FProductWorkNormDto> CreateAsync(CrudFProductWorkNormDto dto);
        Task UpdateAsync(string id, CrudFProductWorkNormDto dto);
        Task DeleteAsync(string id);
        Task<FProductWorkNormCustomineDto> GetByIdAsync(string fProductWorkNormId);
        Task<List<FProductWorkNormDetailCustomineDto>> GetFProductWorkNormDetailAsync(string fProductWorkNormId);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile file, [FromForm] ExcelRequestDto dto);
    }
}
