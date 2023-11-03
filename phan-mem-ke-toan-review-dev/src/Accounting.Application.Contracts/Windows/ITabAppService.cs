using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Windows
{
    public interface ITabAppService
    {
        Task<PageResultDto<TabDto>> GetListAsync(PageRequestDto dto);
        Task<TabDto> CreateAsync(CrudTabDto dto);
        Task UpdateAsync(string id, CrudTabDto dto);
        Task DeleteAsync(string id);
    }
}
