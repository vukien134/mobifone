using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Windows
{
    public interface IFieldAppService
    {
        Task<PageResultDto<FieldDto>> GetListAsync(PageRequestDto dto);
        Task<FieldDto> CreateAsync(CrudFieldDto dto);
        Task UpdateAsync(string id, CrudFieldDto dto);
        Task DeleteAsync(string id);
    }
}
