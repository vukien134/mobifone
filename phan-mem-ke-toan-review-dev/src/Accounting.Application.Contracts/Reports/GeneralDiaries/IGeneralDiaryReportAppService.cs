using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports.GeneralDiaries
{
    public interface IGeneralDiaryReportAppService
    {
        Task<ReportResponseDto<GeneralDiaryBookDto>> CreateDataAsync(ReportRequestDto<ReportBaseParameterDto> dto);
    }
}
