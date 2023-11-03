using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.VoucherNumbers
{
    public interface IVoucherNumberAppService
    {
        Task<PageResultDto<VoucherNumberDto>> GetListAsync(PageRequestDto dto);
        Task<VoucherNumberDto> CreateAsync(CrudVoucherNumberDto dto);
        Task<VoucherNumberCustomineDto> GetVoucherNumberAsync(string voucherCode, DateTime voucherDate);
        Task UpdateAsync(string id, CrudVoucherNumberDto dto);
        Task DeleteAsync(string id);
    }
}
