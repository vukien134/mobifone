using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Salaries.Positions;
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
    public class PositionAppService : AccountingAppService, IPositionAppService
    {
        #region Fields
        private readonly PositionService _positionService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<PositionDto> _cache;
        private readonly IDistributedCache<PageResultDto<PositionDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public PositionAppService(PositionService positionService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<PositionDto> cache,
                            IDistributedCache<PageResultDto<PositionDto>> pageCache,
                            CacheManager cacheManager
                            )
        {
            _positionService = positionService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
        }
        #endregion

        [Authorize(AccountingPermissions.PositionManagerCreate)]
        public async Task<PositionDto> CreateAsync(CrudPositionDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudPositionDto, Position>(dto);
            var result = await _positionService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<Position, PositionDto>(result);
        }

        [Authorize(AccountingPermissions.PositionManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _positionService.DeleteAsync(id);
            await RemoveAllCache();
        }
        [Authorize(AccountingPermissions.PositionManagerView)]
        public async Task<PageResultDto<PositionDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<PositionDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );
        }
        [Authorize(AccountingPermissions.PositionManagerView)]
        public async Task<PageResultDto<PositionDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<PositionDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Position, PositionDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.PositionManagerUpdate)]
        public async Task UpdateAsync(string id, CrudPositionDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _positionService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _positionService.UpdateAsync(entity);
            await RemoveAllCache();
        }
        
        public async Task<PositionDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var accCase = await _positionService.GetAsync(caseId);
                    return ObjectMapper.Map<Position, PositionDto>(accCase);
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
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<PositionDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _positionService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
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
        private async Task<IQueryable<Position>> Filter(PageRequestDto dto)
        {
            var queryable = await _positionService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _positionService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<PositionDto>();
        }
        #endregion

    }
}
