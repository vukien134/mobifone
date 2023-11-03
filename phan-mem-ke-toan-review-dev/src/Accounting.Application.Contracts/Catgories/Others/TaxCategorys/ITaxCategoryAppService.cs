using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.TaxCategorys
{
    public interface ITaxCategoryAppService
    {
        Task<PageResultDto<TaxCategoryDto>> GetListAsync(PageRequestDto dto);
        Task<TaxCategoryDto> CreateAsync(CrudTaxCategoryDto dto);
        Task UpdateAsync(string id, CrudTaxCategoryDto dto);
        Task DeleteAsync(string id);
    }
}
