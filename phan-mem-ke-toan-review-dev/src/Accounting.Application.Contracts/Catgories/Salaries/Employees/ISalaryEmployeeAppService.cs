using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Salaries.Employees
{
    public interface ISalaryEmployeeAppService
    {
        Task<PageResultDto<SalaryEmployeeDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<SalaryEmployeeDto>> GetListAsync(PageRequestDto dto);        
        Task<SalaryEmployeeDto> GetByIdAsync(string caseId);
        Task<SalaryEmployeeDto> CreateAsync(CrudSalaryEmployeeDto dto);
        Task UpdateAsync(string id, CrudSalaryEmployeeDto dto);
        Task DeleteAsync(string id);
    }
}
