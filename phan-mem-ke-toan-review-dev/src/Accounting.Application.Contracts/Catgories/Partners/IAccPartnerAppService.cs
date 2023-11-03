using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Partners
{
    public interface IAccPartnerAppService
    {
        Task<PageResultDto<AccPartnerDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AccPartnerDto>> GetListAsync(PageRequestDto dto);
        Task<AccPartnerDto> GetByIdAsync(string partnerId);
        Task<List<PartnerComboItemDto>> GetTypePartnerAsync();
        Task<List<PartnerComboItemDto>> DataReference(ComboRequestDto dto);
        Task<AccPartnerDto> CreateAsync(CrudAccPartnerDto dto);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
        Task<List<BankPartnerDto>> GetListBankPartnerAsync(string partnerId);
        Task UpdateAsync(string id, CrudAccPartnerDto dto);
        Task DeleteAsync(string id);
    }
}
