using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Products
{
    public interface IProductLotAppService
    {
        Task<PageResultDto<ProductLotDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ProductLotDto>> GetListAsync(PageRequestDto dto);
        Task<ProductLotDto> GetByIdAsync(string caseId);
        Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto);
        Task<ProductLotDto> CreateAsync(CrudProductLotDto dto);
        Task UpdateAsync(string id, CrudProductLotDto dto);
        Task DeleteAsync(string id);
    }
}
