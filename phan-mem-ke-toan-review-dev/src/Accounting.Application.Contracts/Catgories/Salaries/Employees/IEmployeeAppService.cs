using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Salaries.Employees
{
    public interface IEmployeeAppService
    {
        Task<PageResultDto<EmployeeDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<EmployeeDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<EmployeeDto> GetByIdAsync(string caseId);
        Task<EmployeeDto> CreateAsync(CrudEmployeeDto dto);
        Task UpdateAsync(string id, CrudEmployeeDto dto);
        Task DeleteAsync(string id);
    }
}
