using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Vouchers.RecordingVoucherBooks
{
    public interface IRecordingVoucherBookAppService
    {
        Task<PageResultDto<RecordingVoucherBookDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<RecordingVoucherBookDto>> GetListAsync(PageRequestDto dto);        
        Task<RecordingVoucherBookDto> GetByIdAsync(string recordingId);
        Task<RecordingVoucherBookDto> CreateAsync(CrudRecordingVoucherBookDto dto);
        Task UpdateAsync(string id, CrudRecordingVoucherBookDto dto);
        Task DeleteAsync(string id);
    }
}
