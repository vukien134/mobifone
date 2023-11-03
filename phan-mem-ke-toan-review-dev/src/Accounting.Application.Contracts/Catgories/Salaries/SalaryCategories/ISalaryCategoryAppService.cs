using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Salaries.SalaryCategories
{
    public interface ISalaryCategoryAppService
    {
        Task<PageResultDto<SalaryCategoryDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<SalaryCategoryDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<SalaryCategoryDto> GetByIdAsync(string caseId);
        Task<SalaryCategoryDto> CreateAsync(CrudSalaryCategoryDto dto);
        Task UpdateAsync(string id, CrudSalaryCategoryDto dto);
        Task DeleteAsync(string id);
    }
}
