using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.Ledgers
{
    public interface ILedgerAppService
    {
        Task<PageResultDto<LedgerDto>> GetListAsync(PageRequestDto dto);
        Task<LedgerDto> CreateAsync(CrudLedgerDto dto);
        Task UpdateAsync(string id, CrudLedgerDto dto);
        Task DeleteAsync(string id);
    }
}
