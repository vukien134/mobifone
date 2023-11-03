using Accounting.BaseDtos;
using System.Threading.Tasks;

namespace Accounting.Generals.PriceStockOuts
{
    public interface IInfoCalcPriceStockOutAppService
    {
        Task<PageResultDto<InfoCalcPriceStockOutDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<InfoCalcPriceStockOutDto>> GetListAsync(PageRequestDto dto);
        Task<PageResultDto<InfoCalcPriceStockOutDetailDto>> ProductsAsync(string id, PageRequestDto dto);
        Task<InfoCalcPriceStockOutDto> GetByIdAsync(string caseId);
        Task<InfoCalcPriceStockOutDto> GetStart(string id);
        Task<InfoCalcPriceStockOutDto> CreateAsync(CrudInfoCalcPriceStockOutDto dto);
        Task DeleteAsync(string id);
        Task UpdateAsync(string id, CrudInfoCalcPriceStockOutDetailDto dto);
    }
}
