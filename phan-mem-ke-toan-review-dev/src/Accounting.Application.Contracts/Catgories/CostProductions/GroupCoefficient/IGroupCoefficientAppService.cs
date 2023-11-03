using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Catgories.Customines;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.CostProductions
{
    public interface IGroupCoefficientAppService
    {
        Task<PageResultDto<GroupCoefficientDto>> PagesAsync(PageRequestDto dto);
        Task<PageResultDto<GroupCoefficientDto>> GetListAsync(PageRequestDto dto);
        Task<GroupCoefficientDto> GetByIdAsync(string caseId);
        Task<GroupCoefficientDto> CreateAsync(CrudGroupCoefficientDto dto);
        Task UpdateAsync(string id, CrudGroupCoefficientDto dto);
        Task DeleteAsync(string id);
        Task<List<GroupCoefficientDetailCustomineDto>> GetGroupCoefficientDetailAsync(string productId);
    }
}
