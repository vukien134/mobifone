using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Products
{
    public interface IDiscountPriceAppService
    {
        Task<PageResultDto<DiscountPriceDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<DiscountPriceDto>> GetListAsync(PageRequestDto dto);
        Task<DiscountPriceDto> GetByIdAsync(string discountPriceId);
        Task<DiscountPriceDto> CreateAsync(CrudDiscountPriceDto dto);
        Task<List<DiscountPriceDetailDto>> GetListDiscountPriceDetailAsync(string discountPriceId);
        Task<List<DiscountPricePartnerDto>> GetListDiscountPricePartnerAsync(string discountPriceId);
        Task UpdateAsync(string id, CrudDiscountPriceDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
