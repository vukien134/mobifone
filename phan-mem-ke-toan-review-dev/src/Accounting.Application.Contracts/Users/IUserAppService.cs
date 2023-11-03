using Accounting.BaseDtos;
using Accounting.Catgories.OrgUnits;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Users
{
    public interface IUserAppService
    {
        Task<PageResultDto<UserDto>> GetListAsync(PageRequestDto dto);
        Task<UserDto> CreateAsync(CrudUserDto dto);
        Task UpdateAsync(Guid id, CrudUserDto dto);
        Task DeleteAsync(Guid id);
        Task<PageResultDto<UserDto>> PagesAsync(PageRequestDto dto);
        Task<UserDto> GetByIdAsync(Guid id);
        Task<UrlPermissionDto> CheckClientPath(UrlPermissionDto dto);
        Task<List<SelectRoleDto>> GetSelectRolesAsync(Guid userId);
        Task<List<SelectOrgUnitDto>> GetSelectOrgUnitsAsync(Guid userId);
        Task<List<SelectRoleDto>> GetRolesAsync(Guid userId);
        Task<List<SelectOrgUnitDto>> GetOrgUnitByUserIdAsync(Guid userId);
        Task SaveRolesAsync(SaveRoleDto dto);
    }
}
