using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.WorkPlace
{
    public interface IWorkPlaceAppService
    {
        Task<PageResultDto<WorkPlaceDto>> PagesAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<PageResultDto<WorkPlaceDto>> GetListAsync(PageRequestDto dto);
        Task<WorkPlaceDto> CreateAsync(CrudWokPlaceDto dto);
        Task UpdateAsync(string id, CrudWokPlaceDto dto);
        Task DeleteAsync(string id);
    }
}
