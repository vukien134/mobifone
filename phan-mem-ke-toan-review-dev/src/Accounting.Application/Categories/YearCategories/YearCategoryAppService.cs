using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Catgories.YearCategories;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Accounting.Categories.YearCategories
{
    public class YearCategoryAppService : AccountingAppService, IYearCategoryAppService
    {
        #region Fields
        private readonly YearCategoryService _yearCategoryService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IDistributedCache<YearCategoryDto> _cache;
        private readonly IDistributedCache<PageResultDto<YearCategoryDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly TenantSettingService _tenantSettingService;
        #endregion
        #region Ctor
        public YearCategoryAppService(YearCategoryService yearCategoryService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            IDistributedCache<YearCategoryDto> cache,
                            IDistributedCache<PageResultDto<YearCategoryDto>> pageCache,
                            CacheManager cacheManager,
                            TenantSettingService tenantSettingService
            )
        {
            _yearCategoryService = yearCategoryService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _tenantSettingService = tenantSettingService;
        }
        #endregion
        [Authorize(AccountingPermissions.YearCategoryManagerCreate)]
        public async Task<YearCategoryDto> CreateAsync(CruYearCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CruYearCategoryDto, YearCategory>(dto);
            var result = await _yearCategoryService.CreateAsync(entity);
            await _cacheManager.RemoveClassCache<YearCategoryDto>();
            return ObjectMapper.Map<YearCategory, YearCategoryDto>(result);
        }
        [Authorize(AccountingPermissions.YearCategoryManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _yearCategoryService.DeleteAsync(id);
            await _cacheManager.RemoveClassCache<YearCategoryDto>();
        }
        [Authorize(AccountingPermissions.YearCategoryManagerView)]
        public Task<PageResultDto<YearCategoryDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.YearCategoryManagerView)]
        public async Task<PageResultDto<YearCategoryDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<YearCategoryDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Year).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<YearCategory, YearCategoryDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.YearCategoryManagerUpdate)]
        public async Task UpdateAsync(string id, CruYearCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _yearCategoryService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _yearCategoryService.UpdateAsync(entity);
            await _cacheManager.RemoveClassCache<YearCategoryDto>();
        }
        public async Task<YearCategoryDto> GetByIdAsync(string yearCategoryId)
        {
            return await _cache.GetOrAddAsync(
                yearCategoryId, //Cache key
                async () =>
                {
                    var yearCategory = await _yearCategoryService.GetAsync(yearCategoryId);
                    return ObjectMapper.Map<YearCategory, YearCategoryDto>(yearCategory);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );            
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var years = await _yearCategoryService.GetByOrgCode(orgCode);
            return years.Select(p => new BaseComboItemDto()
            {
                Id = p.Year,
                Value = p.Year.ToString(),
                Name = p.Year.ToString(),
                Code = p.Year.ToString()
            }).OrderByDescending(p => p.Value).ToList();
        }
        #region Private
        private async Task<IQueryable<YearCategory>> Filter(PageRequestDto dto)
        {
            var queryable = await _yearCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach(var item in dto.FilterRows)
            {
                if (item.Value == null) continue;
                string value = item.Value.ToString();
                if (string.IsNullOrEmpty(value)) continue;

                if (item.ColumnName.Equals("UsingDecision", StringComparison.InvariantCultureIgnoreCase))
                {
                    queryable = queryable.Where(p => p.UsingDecision == Convert.ToInt32(value));
                }
                else if (item.ColumnName.Equals("BeginDate", StringComparison.InvariantCultureIgnoreCase)
                    || item.ColumnName.Equals("EndDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    var obj = JsonObject.Parse(value);
                    if (obj["start"] != null)
                    {
                        DateTime date = Convert.ToDateTime(obj["start"].ToString());
                        queryable = queryable.Where(item.ColumnName, date, FilterOperator.GreaterThanOrEqual);
                    }
                    if (obj["end"] != null)
                    {
                        DateTime date = Convert.ToDateTime(obj["end"].ToString());
                        queryable = queryable.Where(item.ColumnName, date, FilterOperator.LessThanOrEqual);
                    }
                }
                else if (item.ColumnName.Equals("Year", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (value.StartsWith(OperatorFilter.GreaterThan))
                    {
                        value = value.Replace(OperatorFilter.GreaterThan, "");
                        queryable = queryable.Where(item.ColumnName, Convert.ToInt32(value), FilterOperator.GreaterThan);
                    }
                    else if (value.StartsWith(OperatorFilter.GreaterThanOrEqual))
                    {
                        value = value.Replace(OperatorFilter.GreaterThanOrEqual, "");
                        queryable = queryable.Where(item.ColumnName, Convert.ToInt32(value), FilterOperator.GreaterThanOrEqual);
                    }
                    else if (value.StartsWith(OperatorFilter.LessThan))
                    {
                        value = value.Replace(OperatorFilter.LessThan, "");
                        queryable = queryable.Where(item.ColumnName, Convert.ToInt32(value), FilterOperator.LessThan);
                    }
                    else if (value.StartsWith(OperatorFilter.LessThanOrEqual))
                    {
                        value = value.Replace(OperatorFilter.LessThanOrEqual, "");
                        queryable = queryable.Where(item.ColumnName, Convert.ToInt32(value), FilterOperator.LessThanOrEqual);
                    }
                    else if (value.StartsWith(OperatorFilter.NumEqual))
                    {
                        value = value.Replace(OperatorFilter.NumEqual, "");
                        queryable = queryable.Where(item.ColumnName, Convert.ToInt32(value), FilterOperator.Equal);
                    }
                    else
                    {
                        value = value.Replace(OperatorFilter.NumEqual, "");
                        queryable = queryable.Where(item.ColumnName, Convert.ToInt32(value), FilterOperator.Equal);
                    }
                }
            }
            return queryable;            
        }

        public async Task<List<YearComboItemDto>> GetYearByCurrentOrgUnitAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.YearCurrentUnit, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<YearCategoryDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var years = await _yearCategoryService.GetByOrgCode(_webHelper.GetCurrentOrgUnit());
                    return years.Select(p => new YearComboItemDto()
                    {
                        Id = p.Year,
                        Value = p.Year.ToString(),
                        BeginDate = p.BeginDate,
                        EndDate = p.EndDate
                    }).OrderByDescending(p => p.Value).ToList();
                }
            );            
        }
        private async Task<string> GetSymbolSeparateNumber()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await _tenantSettingService.GetNumberSeparateSymbol(orgCode);
            foreach (var item in currencyFormats)
            {
                if (item.Key.Equals(CurrencyConst.SymbolSeparateDecimal)) return item.Value;
            }
            return null;
        }      
        
        #endregion
    }
}
