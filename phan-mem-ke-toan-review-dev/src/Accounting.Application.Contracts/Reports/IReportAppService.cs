using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Reports
{
    public interface IReportAppService
    {
        Task<List<BaseComboItemDto>> GetByMenuId(string menuId);
        Task<ReportConfigDto> GetConfigAsync(string reportCode);
    }
}
