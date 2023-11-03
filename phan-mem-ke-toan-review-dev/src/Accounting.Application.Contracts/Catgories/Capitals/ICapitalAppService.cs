using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Capitals
{
    public interface ICapitalAppService
    {
        Task<PageResultDto<CapitalDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<CapitalDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<CapitalDto> GetByIdAsync(string caseId);
        Task<CapitalDto> CreateAsync(CrudCapitalDto dto);
        Task UpdateAsync(string id, CrudCapitalDto dto);
        Task DeleteAsync(string id);
    }
}
