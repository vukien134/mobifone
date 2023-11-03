using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.TenantSettings
{
    public interface ITenantSettingAppService
    {
        Task<PageResultDto<TenantSettingDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<TenantSettingDto>> GetListAsync(PageRequestDto dto);
        Task<TenantSettingDto> GetByIdAsync(string tenantSettingId);
        Task<TenantSettingDto> CreateAsync(CrudTenantSettingDto dto);
        Task UpdateAsync(string id, CrudTenantSettingDto dto);
        Task DeleteAsync(string id);
    }
}
