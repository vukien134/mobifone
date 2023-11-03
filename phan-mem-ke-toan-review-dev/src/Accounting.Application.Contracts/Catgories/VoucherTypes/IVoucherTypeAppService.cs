using Accounting.BaseDtos;
using System.Threading.Tasks;

namespace Accounting.Catgories.VoucherTypes
{
    public interface IVoucherTypeAppService
    {
        Task<PageResultDto<VoucherTypeDto>> GetListAsync(PageRequestDto dto);
        Task<VoucherTypeDto> CreateAsync(CrudVoucherTypeDto dto);
        Task UpdateAsync(string id, CrudVoucherTypeDto dto);
        Task DeleteAsync(string id);
    }
}