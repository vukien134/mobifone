using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Partners;
using Accounting.Catgories.Partners;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class PersonAppService : AccountingAppService, IPersonAppService
    {
        #region Fields
        private readonly PersonService _personService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public PersonAppService(PersonService personService,
                        UserService userService,
                        LicenseBusiness licenseBusiness,
                        WebHelper webHelper,
                        IStringLocalizer<AccountingResource> localizer
            )
        {
            _personService = personService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.PersonManagerCreate)]
        public async Task<PersonDto> CreateAsync(CrudPersonDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();         
            
            var entity = ObjectMapper.Map<CrudPersonDto, Person>(dto);
            var result = await _personService.CreateAsync(entity);
            return ObjectMapper.Map<Person, PersonDto>(result);
        }
        [Authorize(AccountingPermissions.PersonManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _personService.DeleteAsync(id);
        }

        [Authorize(AccountingPermissions.PersonManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<PersonDto> GetByIdAsync(string personId)
        {
            var entity = await _personService.GetAsync(personId);
            return ObjectMapper.Map<Person, PersonDto>(entity);
        }
        [Authorize(AccountingPermissions.PersonManagerView)]
        public async Task<PageResultDto<PersonDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<PersonDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Name).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Person, PersonDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.PersonManagerView)]
        public async Task<PageResultDto<PersonDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.PersonManagerUpdate)]
        public async Task UpdateAsync(string id, CrudPersonDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _personService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _personService.UpdateAsync(entity);
        }
        public async Task<List<PersonComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            var partnes = await _personService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
            return partnes.Select(p => new PersonComboItemDto()
            {
                Id = p.Id,
                Value = p.Name,
                Code = p.Name,
                Name = p.Name,
                Address = p.Address,
                Display = p.Name
            }).ToList();
        }
        #region
        private async Task<IQueryable<Person>> Filter(PageRequestDto dto)
        {
            var queryable = await _personService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _personService.GetQueryableQuickSearch(queryable, filterValue);
            }
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        #endregion
    }
}
