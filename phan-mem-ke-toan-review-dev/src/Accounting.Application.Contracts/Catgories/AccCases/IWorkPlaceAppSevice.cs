using Accounting.BaseDtos;
using Accounting.Catgories.WorkPlace;
using System.Threading.Tasks;

namespace Accounting.Catgories.AccCases
{
    public interface IWorkPlaceAppSevice
    {
        Task<PageResultDto<WorkPlaceDto>> GetListAsync(PageRequestDto dto);
        Task<WorkPlaceDto> CreateAsync(CurdWokPlaceDto dto);
        Task UpdateAsync(string id, CurdWokPlaceDto dto);
        Task DeleteAsync(string id);
    }
}
