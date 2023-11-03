using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Salaries.Employees;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Salaries;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Accounting.Categories.Salaries
{
    public class EmployeeAppService : AccountingAppService,IEmployeeAppService
    {
        #region Fields
        private readonly EmployeeService _employeeService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<EmployeeDto> _cache;
        private readonly IDistributedCache<PageResultDto<EmployeeDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public EmployeeAppService(EmployeeService employeeService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<EmployeeDto> cache,
                            IDistributedCache<PageResultDto<EmployeeDto>> pageCache,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
                            )
        {
            _employeeService = employeeService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.EmployeeManagerCreate)]
        public async Task<EmployeeDto> CreateAsync(CrudEmployeeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudEmployeeDto, Employee>(dto);
            var result = await _employeeService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<Employee, EmployeeDto>(result);
        }

        [Authorize(AccountingPermissions.EmployeeManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _employeeService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.EmployeeCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Employee, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _employeeService.DeleteAsync(id);
            await RemoveAllCache();
        }
        [Authorize(AccountingPermissions.EmployeeManagerView)]
        public async Task<PageResultDto<EmployeeDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<EmployeeDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }
        [Authorize(AccountingPermissions.EmployeeManagerView)]
        public async Task<PageResultDto<EmployeeDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<EmployeeDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Employee, EmployeeDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.EmployeeManagerUpdate)]
        public async Task UpdateAsync(string id, CrudEmployeeDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _employeeService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _employeeService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.EmployeeCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }

        public async Task<EmployeeDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var accCase = await _employeeService.GetAsync(caseId);
                    return ObjectMapper.Map<Employee, EmployeeDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );

        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<EmployeeDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _employeeService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
                    return partnes.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name
                    }).ToList();
                }
            );
        }
        #region Private
        private async Task<IQueryable<Employee>> Filter(PageRequestDto dto)
        {
            var queryable = await _employeeService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _employeeService.GetQueryableQuickSearch(queryable, filterValue);
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<EmployeeDto>();
        }
        #endregion
    }
}
