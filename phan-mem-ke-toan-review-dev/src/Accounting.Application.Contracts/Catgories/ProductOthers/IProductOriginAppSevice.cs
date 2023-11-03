using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.ProductOthers
{
   public  interface IProductOriginAppSevice
    {
        Task<PageResultDto<ProductOriginDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ProductOriginDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<ProductOriginDto> GetByIdAsync(string productOriginId);
        Task<ProductOriginDto> CreateAsync(CrudProductOriginDto dto);
        Task UpdateAsync(string id, CrudProductOriginDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
