using Accounting.BaseDtos;
using Accounting.Catgories.Salaries.SalaryTypes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Salaries.SalarySheetTypes
{
    public interface ISalarySheetTypeAppService
    {
        Task<PageResultDto<SalarySheetTypeDto>> GetListAsync(PageRequestDto dto);        
        Task<SalarySheetTypeDto> GetByIdAsync(string productId);
        Task<SalarySheetTypeDto> CreateAsync(CrudSalarySheetTypeDto dto);
        Task UpdateAsync(string id, CrudSalarySheetTypeDto dto);
        Task DeleteAsync(string id);
    }
}
