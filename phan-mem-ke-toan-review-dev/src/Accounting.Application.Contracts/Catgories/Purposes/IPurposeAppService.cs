using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Purposes
{
    public interface IPurposeAppService
    {
        Task<PageResultDto<PurposeDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<PurposeDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<PurposeDto> GetByIdAsync(string caseId);
        Task<PurposeDto> CreateAsync(CrudPurposeDto dto);
        Task UpdateAsync(string id, CrudPurposeDto dto);
        Task DeleteAsync(string id);
    }
}
