using Accounting.BaseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.ProductOpeningBalances
{
    public interface IProductOpeningBalanceAppService
    {
        Task<PageResultDto<ProductOpeningBalanceDto>> GetListAsync(PageRequestDto dto);
        Task<ProductOpeningBalanceDto> CreateAsync(CrudProductOpeningBalanceDto dto);
        Task<List<ProductOpeningBalanceDto>> CreateListAsync(ObjectProductOpeningBalanceDto listDto);
        Task<List<ProductOpeningBalanceCustomerDto>> GetDataAsync();
        Task UpdateAsync(string id, CrudProductOpeningBalanceDto dto);
        Task DeleteAsync(string id);
    }
}
