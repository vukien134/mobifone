using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.AdjustDepreciations
{
    public interface IAdjustDepreciationAppService
    {
        Task<PageResultDto<AdjustDepreciationDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<AdjustDepreciationDto>> GetListAsync(PageRequestDto dto);
        Task<AdjustDepreciationDto> GetByIdAsync(string caseId);
        Task<AdjustDepreciationDto> CreateAsync(CrudAdjustDepreciationDto dto);
        Task UpdateAsync(string id, CrudAdjustDepreciationDto dto);
        Task DeleteAsync(string id);
        Task<List<DataAdjustDepreciationDetailDto>> GetAdjustDepreciationDetailAsync(string productId);
    }
}
