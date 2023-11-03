using Accounting.BaseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Windows
{
    public interface IReferenceAppService
    {
        Task<PageResultDto<ReferenceDto>> GetListAsync(PageRequestDto dto);
        Task<List<ComboItemDto>> GetValuesAsync(string referenceId);
        Task<ReferenceDto> CreateAsync(CrudReferenceDto dto);
        Task UpdateAsync(string id, CrudReferenceDto dto);
        Task DeleteAsync(string id);
        Task<ReferenceDto> GetByIdAsync(string referenceId);
    }
}
