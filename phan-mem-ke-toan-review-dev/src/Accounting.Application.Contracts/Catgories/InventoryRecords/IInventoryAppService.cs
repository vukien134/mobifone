using Accounting.BaseDtos;
using Accounting.Catgories.Contracts;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Accounting.Catgories.InventoryRecords
{
    public interface IInventoryRecordAppService
    {
        Task<PageResultDto<InventoryRecordDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<InventoryRecordDto>> GetListAsync(PageRequestDto dto);
        Task<InventoryRecordDto> GetByIdAsync(string partnerId);
        Task<InventoryRecordDto> CreateAsync(CrudInventoryRecordDto dto);
        Task<List<InventoryRecordDetailResDto>> GetListInventoryRecordDetailAsync(string InventoryRecordId);
        Task UpdateAsync(string id, CrudInventoryRecordDto dto);
        Task DeleteAsync(string id);
    }
}
