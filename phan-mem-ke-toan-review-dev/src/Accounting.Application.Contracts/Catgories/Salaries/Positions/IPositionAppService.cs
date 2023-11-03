using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Salaries.Positions
{
    public interface IPositionAppService
    {
        Task<PageResultDto<PositionDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<PositionDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<PositionDto> GetByIdAsync(string caseId);
        Task<PositionDto> CreateAsync(CrudPositionDto dto);
        Task UpdateAsync(string id, CrudPositionDto dto);
        Task DeleteAsync(string id);
    }
}
