using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.SaleChannels
{
    public interface ISaleChannelAppService
    {
        Task<PageResultDto<SaleChannelDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<SaleChannelDto>> GetListAsync(PageRequestDto dto);
        Task<List<BaseComboItemDto>> GetViewListAsync();
        Task<SaleChannelDto> GetByIdAsync(string partnerGroupId);
        Task<SaleChannelDto> CreateAsync(CrudSaleChannelDto dto);
        Task UpdateAsync(string id, CrudSaleChannelDto dto);
        Task DeleteAsync(string id);
    }
}
