using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.CostProductions
{
    public interface IConfigCostPriceAppService
    {
        Task<PageResultDto<ConfigCostPriceDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ConfigCostPriceDto>> GetListAsync(PageRequestDto dto);
        Task<ConfigCostPriceDto> GetByIdAsync(string caseId);
        Task<ConfigCostPriceDto> CreateAsync(CrudConfigCostPriceDto dto);
        Task UpdateAsync(string id, CrudConfigCostPriceDto dto);
        Task DeleteAsync(string id);
    }
}
