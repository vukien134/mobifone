using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Products
{
    public interface IProductAppService
    {
        Task<PageResultDto<ProductDto>> GetListAsync(PageRequestDto dto);
        Task<List<ProductComboItemDto>> DataReference(ComboRequestDto dto);
        Task<ProductDto> GetByIdAsync(string productId);
        Task<ProductDto> CreateAsync(CrudProductDto dto);
        Task UpdateAsync(string id, CrudProductDto dto);
        Task DeleteAsync(string id);
    }
}
