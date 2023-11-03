using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.Careers
{
    public interface ICareerAppService
    {
        Task<PageResultDto<CareerDto>> GetListAsync(PageRequestDto dto);
        Task<CareerDto> CreateAsync(CrudCareerDto dto);
        Task<CareerDto> GetById(string careerId);
        Task UpdateAsync(string id, CrudCareerDto dto);
        Task DeleteAsync(string id);
    }
}
