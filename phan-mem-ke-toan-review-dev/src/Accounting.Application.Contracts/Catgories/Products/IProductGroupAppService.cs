using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Products
{
    public interface IProductGroupAppService
    {
        Task<PageResultDto<ProductGroupDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ProductGroupDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetViewListAsync();
        Task<ProductGroupDto> GetByIdAsync(string productGroupId);
        Task<ProductGroupDto> CreateAsync(CrudProductGroupDto dto);
        Task UpdateAsync(string id, CrudProductGroupDto dto);
        Task DeleteAsync(string id);
    }
}
