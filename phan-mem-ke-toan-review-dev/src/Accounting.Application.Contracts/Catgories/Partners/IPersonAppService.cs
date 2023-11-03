using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Partners
{
    public interface IPersonAppService
    {
        Task<PageResultDto<PersonDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<PersonDto>> GetListAsync(PageRequestDto dto);
        Task<PersonDto> GetByIdAsync(string partnerId);        
        Task<PersonDto> CreateAsync(CrudPersonDto dto);        
        Task UpdateAsync(string id, CrudPersonDto dto);
        Task DeleteAsync(string id);
    }
}
