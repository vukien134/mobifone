using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Salaries.SalaryCategories;
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
    public class SalaryCategoryAppService : AccountingAppService, ISalaryCategoryAppService
    {
        #region Fields
        private readonly SalaryCategoryService _salaryCategoryService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<SalaryCategoryDto> _cache;
        private readonly IDistributedCache<PageResultDto<SalaryCategoryDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly DefaultSalaryCategoryService _defaultCategory;
        #endregion
        #region Ctor
        public SalaryCategoryAppService(SalaryCategoryService salaryCategoryService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IDistributedCache<SalaryCategoryDto> cache,
                            IDistributedCache<PageResultDto<SalaryCategoryDto>> pageCache,
                            CacheManager cacheManager,
                            DefaultSalaryCategoryService defaultCategory
                            )
        {
            _salaryCategoryService = salaryCategoryService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _defaultCategory = defaultCategory;
        }
        #endregion

        [Authorize(AccountingPermissions.SalaryCategoryManagerCreate)]
        public async Task<SalaryCategoryDto> CreateAsync(CrudSalaryCategoryDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudSalaryCategoryDto, SalaryCategory>(dto);
            var result = await _salaryCategoryService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<SalaryCategory, SalaryCategoryDto>(result);
        }

        [Authorize(AccountingPermissions.SalaryCategoryManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _salaryCategoryService.DeleteAsync(id);
            await RemoveAllCache();
        }
        [Authorize(AccountingPermissions.SalaryCategoryManagerView)]
        public async Task<PageResultDto<SalaryCategoryDto>> PagesAsync(PageRequestDto dto)
        {
            await InsertDefaultData();     
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<SalaryCategoryDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );           
        }
        [Authorize(AccountingPermissions.SalaryCategoryManagerView)]
        public async Task<PageResultDto<SalaryCategoryDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<SalaryCategoryDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<SalaryCategory, SalaryCategoryDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.SalaryCategoryManagerUpdate)]
        public async Task UpdateAsync(string id, CrudSalaryCategoryDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _salaryCategoryService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _salaryCategoryService.UpdateAsync(entity);
            await RemoveAllCache();
        }

        public async Task<SalaryCategoryDto> GetByIdAsync(string caseId)
        {
            return await _cache.GetOrAddAsync(
                caseId, //Cache key
                async () =>
                {
                    var accCase = await _salaryCategoryService.GetAsync(caseId);
                    return ObjectMapper.Map<SalaryCategory, SalaryCategoryDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );

        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            await InsertDefaultData();
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<SalaryCategoryDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _salaryCategoryService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
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
        private async Task<IQueryable<SalaryCategory>> Filter(PageRequestDto dto)
        {
            var queryable = await _salaryCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(_webHelper.GetCurrentOrgUnit()));
            
            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _salaryCategoryService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<SalaryCategoryDto>();    
        }
        private async Task InsertDefaultData()
        {
            var orgCode = _webHelper.GetCurrentOrgUnit();             
            if (await IsDataCategoryExist(orgCode) == false)
            {
                var data = await this._defaultCategory.GetList();
                var dtos = data.Select(p => new CrudSalaryCategoryDto()
                {
                   Amount = p.Amount,
                   Code = p.Code,
                   Id = this.GetNewObjectId(),
                   OrgCode = orgCode,
                   Name = p.Name,
                   SalaryType = p.SalaryType,
                   Formular = p.Formular,
                   Status = p.Status,
                   Nature = p.Nature,
                   TaxType = p.TaxType         
                }).ToList();                     
                foreach (var item in dtos)
                {
                    var entity = ObjectMapper.Map<CrudSalaryCategoryDto,SalaryCategory>(item);           
                    await _salaryCategoryService.CreateAsync(entity);
                }
                await CurrentUnitOfWork.SaveChangesAsync();
                await this.RemoveAllCache();
            };
        }
        private async Task<bool> IsDataCategoryExist(string orgCode)
        {
            var queryable = await _salaryCategoryService.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode);
        }

        #endregion
    }
}
