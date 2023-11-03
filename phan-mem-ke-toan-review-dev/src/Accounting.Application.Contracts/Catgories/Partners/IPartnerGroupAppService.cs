using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Partners
{
    public interface IPartnerGroupAppService
    {
        Task<PageResultDto<PartnerGroupDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<PartnerGroupDto>> GetListAsync(PageRequestDto dto);
        Task<List<PartnerGroupTreeItemDto>> GetViewTreeAsync();
        Task<List<PartnerGroupComboItemDto>> GetViewListAsync();
        Task<PartnerGroupDto> GetByIdAsync(string partnerGroupId);        
        Task<PartnerGroupDto> CreateAsync(CrudPartnerGroupDto dto);        
        Task UpdateAsync(string id, CrudPartnerGroupDto dto);
        Task DeleteAsync(string id);
    }
}
