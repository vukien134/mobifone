using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Catgories.CostProductions.AllotmentForwardCategories;
using Accounting.Excels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Catgories.CostProductions
{
    public interface IAllotmentForwardCategoryAppService
    {
        Task<PageResultDto<AllotmentForwardCategoryDto>> PagesAsync(string type, string productOrWork,string ordGrp,PageRequestDto dto);
        Task<PageResultDto<AllotmentForwardCategoryDto>> GetListAsync(AllotmentForwardRequestDto dto);
        Task<AllotmentForwardCategoryDto> GetByIdAsync(string caseId);
        Task<AllotmentForwardCategoryDto> CreateAsync(CrudAllotmentForwardCategoryDto dto);
        Task UpdateAsync(string id, CrudAllotmentForwardCategoryDto dto);
        Task DeleteAsync(string id);
        Task<List<BaseComboItemDto>> GetDataReference(string productOrWork, string lstType);
    }
}
