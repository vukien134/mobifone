using Accounting.BaseDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.OrgUnits
{
    public interface IOrgUnitAppService
    {
        Task<PageResultDto<OrgUnitDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<OrgUnitDto>> GetListAsync(PageRequestDto dto);
        Task<List<OrgUnitDto>> GetByCurrentUser();
        Task<List<SelectOrgUnitLoginDto>> GetSelectToLoginAsync();
        Task<OrgUnitDto> CreateAsync(CrudOrgUnitDto dto);
        Task UpdateAsync(string id, CrudOrgUnitDto dto);
        Task DeleteAsync(string id);
    }
}
