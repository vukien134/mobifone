using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Accounting.Licenses
{
    public interface ILicenseAppService
    {
        Task Register(CrudRegLicenseDto dto);
        Task<ResLicenseDto> GetRegisterByTenantIdAsync();
        Task<RegLicenseDto> ApprovalAsync(Guid tenantId);
        Task<Dictionary<string, string>> GetInfoLicenseAsync();
        Task<RegLicenseDto> UpdateAsync(string AccessCode, CrudRegLicenseDto dto);
        Task<JsonObject> CreateLicenseAsync(JsonObject dto);
    }
}
