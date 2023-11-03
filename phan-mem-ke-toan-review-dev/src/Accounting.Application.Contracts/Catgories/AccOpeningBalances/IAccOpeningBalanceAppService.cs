using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Accounts.AccOpeningBalances
{
    public interface IAccOpeningBalanceAppService
    {
        Task<PageResultDto<AccOpeningBalanceDto>> GetListAsync(PageRequestDto dto);
        Task<List<AccOpeningBalanceDto>> GetDetailByAccCodeAsync(string accCode);
        Task<List<AccOpeningBalanceCustomineDto>> GetDataAsync();
        Task<AccOpeningBalanceDto> CreateAsync(CrudAccOpeningBalanceDto dto);
        Task<List<AccOpeningBalanceDto>> CreateListAsync(AccOpeningBalanceParaDto listDto);
        Task<List<AccOpeningBalanceDto>> CreateListDetailAsync(AccOpeningBalanceDetailParaDto listDto);
        Task UpdateAsync(string id, CrudAccOpeningBalanceDto dto);
        Task DeleteAsync(string id);
        Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile file, [FromForm] ExcelRequestDto dto);
    }
}
