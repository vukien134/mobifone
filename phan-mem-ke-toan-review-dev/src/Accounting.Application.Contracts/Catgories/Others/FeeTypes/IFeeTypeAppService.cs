using Accounting.BaseDtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.Others.FeeTypes
{
    public interface IFeeTypeAppService
    {
        Task<PageResultDto<FeeTypeDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<FeeTypeDto>> GetListAsync(PageRequestDto dto);
        Task<FeeTypeDto> GetByIdAsync(string caseId);
        Task<FeeTypeDto> CreateAsync(CrudFeeTypeDto dto);
        Task UpdateAsync(string id, CrudFeeTypeDto dto);
        Task DeleteAsync(string id);
    }
}
