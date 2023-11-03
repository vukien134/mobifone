using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Salaries.SalaryPeriods
{
    public interface ISalaryPeriodAppService
    {
        Task<PageResultDto<SalaryPeriodDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<SalaryPeriodDto>> GetListAsync(PageRequestDto dto);        
        Task<SalaryPeriodDto> GetByIdAsync(string caseId);
        Task<SalaryPeriodDto> CreateAsync(CrudSalaryPeriodDto dto);
        Task UpdateAsync(string id, CrudSalaryPeriodDto dto);
        Task DeleteAsync(string id);
    }
}
