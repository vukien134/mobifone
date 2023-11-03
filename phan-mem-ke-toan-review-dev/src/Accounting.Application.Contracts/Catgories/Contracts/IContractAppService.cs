using Accounting.BaseDtos;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.Contracts
{
    public interface IContractAppService
    {
        Task<PageResultDto<ContractDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ContractDto>> GetListAsync(PageRequestDto dto);
        Task<ContractDto> GetByIdAsync(string partnerId);
        Task<ContractDto> CreateAsync(CrudContractDto dto);
        Task<List<ContractDetailDto>> GetListContractDetailAsync(string contractId);
        Task UpdateAsync(string id, CrudContractDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto);
    }
}
