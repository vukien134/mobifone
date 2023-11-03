using Accounting.BaseDtos;
using Accounting.Catgories.Others.ParameterFillter;
using Accounting.Excels;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.VoucherExciseTaxs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.ProductVouchers
{
    public interface IProductVoucherAppService
    {
        Task<PageResultDto<ProductVoucherDto>> GetListAsync(PageRequestDto dto);
        Task<CrudProductVoucherDto> CreateAsync(CrudProductVoucherDto dto);
        Task UpdateAsync(string id, CrudProductVoucherDto dto);
        Task DeleteAsync(string id);
        Task<PageResultDto<ProductVoucherDto>> PostProductListAsync(PageRequestDto dto);
        Task<List<CrudProductVoucherDetailDto>> GetProductVoucherDetailAsync(string productVoucherId);
        Task<List<CrudVoucherExciseTaxDto>> GetVoucherExciseTaxAsync(string productVoucherId);
        Task<List<CrudProductVoucherCostDto>> GetProductVoucherCostDetailAsync(string productVoucherId);
        Task<List<CrudProductVoucherVatDto>> GetProductVoucherVatAsync(string productVoucherId);
        Task<List<CrudProductVoucherReceiptDto>> GetProductVoucherReceiptAsync(string productVoucherId);
        Task<List<CrudProductVoucherAssemblyDto>> GetProductVoucherAssemblyAsync(string productVoucherId);
        Task<List<CrudAccTaxDetailDto>> GetAccTaxDetailAsync(string productVoucherId);
        Task<PageResultDto<ProductVoucherDto>> PagesAsync(PageRequestDto dto);

        Task<List<ProductVoucherCustomineDto>> ListProductVoucherDetailAsync(ParameterFillters dto);
        Task<CrudProductVoucherDto> GetByIdAsync(string ProductVoucherId);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
