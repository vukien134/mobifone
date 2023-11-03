using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.YearCategories
{
    public interface IYearCategoryAppService
    {
        Task<PageResultDto<YearCategoryDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<YearCategoryDto>> GetListAsync(PageRequestDto dto);
        Task<YearCategoryDto> GetByIdAsync(string yearCategoryId);
        Task<List<YearComboItemDto>> GetYearByCurrentOrgUnitAsync();
        Task<YearCategoryDto> CreateAsync(CruYearCategoryDto dto);
        Task UpdateAsync(string id, CruYearCategoryDto dto);
        Task DeleteAsync(string id);
    }
}
