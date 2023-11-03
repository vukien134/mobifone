using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.TaxCategories
{
    public interface ITaxCategoryAppService
    {
        Task<List<TaxCategoryComboItemDto>> GetDataReference(string outOrIn = null);
        Task<PageResultDto<TaxCategoryDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<TaxCategoryDto>> GetListAsync(PageRequestDto dto);
        Task<TaxCategoryDto> CreateAsync(CrudTaxCategoryDto dto);
        Task UpdateAsync(string id, CrudTaxCategoryDto dto);
        Task DeleteAsync(string id);
    }
}
