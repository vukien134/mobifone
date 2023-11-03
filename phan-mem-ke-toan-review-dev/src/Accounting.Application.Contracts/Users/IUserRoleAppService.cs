using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Users
{
    public interface IUserRoleAppService
    {
        Task<PageResultDto<UserRoleDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<UserRoleDto>> GetListAsync(PageRequestDto dto);
        Task<UserRoleDto> CreateAsync(CrudUserRoleDto dto);
        Task UpdateAsync(Guid id, CrudUserRoleDto dto);
        Task<UserRoleDto> GetByIdAsync(Guid userRoleId);
        Task DeleteAsync(Guid id);
        Task<List<GroupPermissionDto>> GetGroupPermissionAsync();
        Task<UserRolePermissionDto> GetPermissionTreeByGroupAsync(Guid roleId,string name);
        Task AssignPermission(AssignPermissionDto dto);
        Task AssignOrRevokeGroupPermission(AssignPermissionDto dto);
    }
}
