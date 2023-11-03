using Accounting.BaseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace Accounting.Windows
{
    public interface IWindowAppService
    {
        //Task<PageResultDto<WindowDto>> GetListAsync(PageRequestDto dto);
        Task<PagedResultDto<WindowDto>> GetListAsync(PagedAndSortedResultRequestDto dto);
        Task<WindowDto> GetByIdAsync(string id);
        Task<WindowConfigDto> GetConfigByCodeAsync(string code);
        Task<WindowConfigDto> GetConfigByVoucherCodeAsync(string code);
        Task<WindowDto> GetByVoucherCodeAsync(string code);
        Task<WindowConfigDto> GetFormatNumber();
        Task<WindowDto> CreateAsync(CrudWindowDto dto);
        Task UpdateAsync(string id, CrudWindowDto dto);
        Task DeleteAsync(string id);
        Task Copy(string windowId, string code, string name);
    }
}
