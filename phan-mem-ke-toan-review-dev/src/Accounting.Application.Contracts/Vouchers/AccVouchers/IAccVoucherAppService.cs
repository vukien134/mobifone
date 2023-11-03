using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.AccVouchers
{
    public interface IAccVoucherAppService
    {
        Task<PageResultDto<AccVoucherDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AccVoucherDto>> GetListAsync(PageRequestDto dto);
        Task<AccVoucherDto> GetByIdAsync(string accVoucherId);
        Task<List<AccVoucherDetailDto>> GetAccVoucherDetailAsync(string accVoucherId);
        Task<List<AccTaxDetailDto>> GetAccTaxDetailAsync(string accVoucherId);
        Task<PageResultDto<AccVoucherDto>> PostFilterAsync(PageRequestDto dto);
        Task<AccVoucherDto> CreateAsync(CrudAccVoucherDto dto);
        Task UpdateAsync(string id, CrudAccVoucherDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
