using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.ExciseTaxes
{
    public interface IExciseTaxAppService
    {
        Task<PageResultDto<ExciseTaxDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<ExciseTaxDto>> GetListAsync(PageRequestDto dto);
        Task<ExciseTaxDto> GetByIdAsync(string exciseId);
        Task<ExciseTaxDto> CreateAsync(CrudExciseTaxDto dto);
        Task UpdateAsync(string id, CrudExciseTaxDto dto);
        Task DeleteAsync(string id);
    }
}
