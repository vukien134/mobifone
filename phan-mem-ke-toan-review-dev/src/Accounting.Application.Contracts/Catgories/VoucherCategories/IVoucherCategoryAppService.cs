using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.VoucherCategories
{
    public interface IVoucherCategoryAppService
    {
        Task<List<BaseComboItemDto>> GetDataReference();
        Task<PageResultDto<VoucherCategoryDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<VoucherCategoryDto>> GetListAsync(PageRequestDto dto);
        Task<VoucherCategoryDto> CreateAsync(CruVoucherCategoryDto dto);
        Task UpdateAsync(string id, CruVoucherCategoryDto dto);
        Task DeleteAsync(string id);
    }
}
