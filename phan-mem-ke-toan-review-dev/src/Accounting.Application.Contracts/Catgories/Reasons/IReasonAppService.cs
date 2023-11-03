using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Reasons
{
    public interface IReasonAppService
    {
        Task<PageResultDto<ReasonDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ReasonDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<ReasonDto> GetByIdAsync(string caseId);
        Task<ReasonDto> CreateAsync(CrudReasonDto dto);
        Task UpdateAsync(string id, CrudReasonDto dto);
        Task DeleteAsync(string id);
    }
}
