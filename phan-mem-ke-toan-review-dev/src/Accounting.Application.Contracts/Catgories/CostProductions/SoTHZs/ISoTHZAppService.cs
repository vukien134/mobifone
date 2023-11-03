using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.CostProductions.SoTHZs
{
    public interface ISoTHZAppService
    {
        Task<PageResultDto<SoTHZDto>> PagesAsync(string productOrWork, PageRequestDto dto);
        Task<PageResultDto<SoTHZDto>> GetListAsync(SoTHZRequestDto dto);        
        Task<SoTHZDto> GetByIdAsync(string caseId);
        Task<SoTHZDto> CreateAsync(CrudSoTHZDto dto);
        Task UpdateAsync(string id, CrudSoTHZDto dto);
        Task DeleteAsync(string id);
    }
}
