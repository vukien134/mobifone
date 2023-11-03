using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.BusinessCategories
{
    public interface IBusinessCategoryAppService
    {
        Task<PageResultDto<BusinessCategoryDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<BusinessCategoryDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetDataReference(string voucherCode=null);
        Task<BusinessCategoryDto> CreateAsync(CrudBusinessCategoryDto dto);
        Task UpdateAsync(string id, CrudBusinessCategoryDto dto);
        Task DeleteAsync(string id);
    }
}
