using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.Currencies
{
    public interface ICurrencyAppService
    {
        Task<PageResultDto<CurrencyDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<CurrencyDto>> GetListAsync(PageRequestDto dto);
        Task<List<CurrencyComboItemDto>> GetDataReference();
        Task<CurrencyDto> CreateAsync(CrudCurrencyDto dto);
        Task UpdateAsync(string id, CrudCurrencyDto dto);
        Task DeleteAsync(string id);
    }
}
