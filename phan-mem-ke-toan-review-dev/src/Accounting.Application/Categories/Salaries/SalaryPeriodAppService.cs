using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Salaries.SalaryPeriods;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Salaries;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Accounting.Categories.Salaries
{
    public class SalaryPeriodAppService : AccountingAppService, ISalaryPeriodAppService
    {
        #region Fields
        private readonly SalaryPeriodService _salaryPeriodService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<SalaryPeriodDto> _cache;
        private readonly IDistributedCache<PageResultDto<SalaryPeriodDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public SalaryPeriodAppService(SalaryPeriodService salaryPeriodService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<SalaryPeriodDto> cache,
                            IDistributedCache<PageResultDto<SalaryPeriodDto>> pageCache,
                            CacheManager cacheManager
                            )
        {
            _salaryPeriodService = salaryPeriodService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
        }
        #endregion

        [Authorize(AccountingPermissions.SalaryPeriodManagerCreate)]
        public async Task<SalaryPeriodDto> CreateAsync(CrudSalaryPeriodDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudSalaryPeriodDto, SalaryPeriod>(dto);
            var result = await _salaryPeriodService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<SalaryPeriod, SalaryPeriodDto>(result);
        }

        [Authorize(AccountingPermissions.SalaryPeriodManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _salaryPeriodService.DeleteAsync(id);
            await RemoveAllCache();
        }
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            await RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }
        [Authorize(AccountingPermissions.SalaryPeriodManagerView)]
        public async Task<PageResultDto<SalaryPeriodDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<SalaryPeriodDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }
        [Authorize(AccountingPermissions.SalaryPeriodManagerView)]
        public async Task<PageResultDto<SalaryPeriodDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<SalaryPeriodDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<SalaryPeriod, SalaryPeriodDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.SalaryPeriodManagerUpdate)]
        public async Task UpdateAsync(string id, CrudSalaryPeriodDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _salaryPeriodService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _salaryPeriodService.UpdateAsync(entity);
            await RemoveAllCache();
        }

        public async Task<SalaryPeriodDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var accCase = await _salaryPeriodService.GetAsync(caseId);
                    return ObjectMapper.Map<SalaryPeriod, SalaryPeriodDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );

        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var partnes = await _salaryPeriodService.GetDataReference(orgCode);
            return partnes.Select(p => new BaseComboItemDto()
            {
                Id = p.Id,
                Value = p.Name,
                Code = p.Code,
                Name = p.Name
            }).ToList();
        }
        #region Private
        private async Task<IQueryable<SalaryPeriod>> Filter(PageRequestDto dto)
        {
            var queryable = await _salaryPeriodService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _salaryPeriodService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<SalaryPeriodDto>();
        }
        #endregion
    }
}
