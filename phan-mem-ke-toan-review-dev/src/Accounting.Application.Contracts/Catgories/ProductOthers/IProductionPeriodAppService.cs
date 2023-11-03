using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.ProductOthers
{
    public interface IProductionPeriodAppService
    {
        Task<PageResultDto<ProductionPeriodDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ProductionPeriodDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetDataReference();
        Task<ProductionPeriodDto> GetByIdAsync(string caseId);
        Task<ProductionPeriodDto> CreateAsync(CrudProductionPeriodDto dto);
        Task UpdateAsync(string id, CrudProductionPeriodDto dto);
        Task DeleteAsync(string id);
    }
}
