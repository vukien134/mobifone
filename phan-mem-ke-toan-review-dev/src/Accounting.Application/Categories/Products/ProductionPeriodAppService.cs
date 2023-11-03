using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Others;
using Accounting.Catgories.ProductOthers;
using Accounting.Constants;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
using Accounting.Extensions;
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
using Volo.Abp.Uow;

namespace Accounting.Categories.Products
{
    public class ProductionPeriodAppService : AccountingAppService, IProductionPeriodAppService
    {
        #region Fields
        private readonly ProductionPeriodService _productionPeriodService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IDistributedCache<ProductionPeriodDto> _cache;
        private readonly IDistributedCache<PageResultDto<ProductionPeriodDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        #endregion
        #region Ctor
        public ProductionPeriodAppService(ProductionPeriodService accCaseService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            IDistributedCache<ProductionPeriodDto> cache,
                            IDistributedCache<PageResultDto<ProductionPeriodDto>> pageCache,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer,
                            IUnitOfWorkManager unitOfWorkManager
                            )
        {
            _productionPeriodService = accCaseService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
            _unitOfWorkManager = unitOfWorkManager;
        }
        #endregion

        [Authorize(AccountingPermissions.ProductionPeriodManagerCreate)]
        public async Task<ProductionPeriodDto> CreateAsync(CrudProductionPeriodDto dto)
        {
           
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudProductionPeriodDto, ProductionPeriod>(dto);          
            var result = await _productionPeriodService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<ProductionPeriod, ProductionPeriodDto>(result);
        }

        [Authorize(AccountingPermissions.ProductionPeriodManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _productionPeriodService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductPeriodCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductionPeriod, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }

            await _productionPeriodService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.ProductionPeriodManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }
            foreach (var item in dto.ListId)
            {
                var entity = await _productionPeriodService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductPeriodCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductionPeriod, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _productionPeriodService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.ProductionPeriodManagerView)]
        public async Task<PageResultDto<ProductionPeriodDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<ProductionPeriodDto>(dto);
            var data = _productionPeriodService.GetRepository();
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );            
        }
        [Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<ProductionPeriodDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ProductionPeriodDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductionPeriod, ProductionPeriodDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.CaseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudProductionPeriodDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _productionPeriodService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _productionPeriodService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.ProductPeriodCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }
        public async Task<ProductionPeriodDto> GetByIdAsync(string productionPeriodId)
        {
            return await _cache.GetOrAddAsync(
                productionPeriodId, //Cache key
                async () =>
                {
                    var entity = await _productionPeriodService.GetAsync(productionPeriodId);
                    return ObjectMapper.Map<ProductionPeriod, ProductionPeriodDto>(entity);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );            
        }
        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            string filterValue = $"%%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<ProductionPeriodDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var queryable = await _productionPeriodService.GetQueryableAsync();
                    queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                        .OrderBy(p => p.Code);
                    var sections = await AsyncExecuter.ToListAsync(queryable);
                    return sections.Select(p => new BaseComboItemDto()
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
        private async Task<IQueryable<ProductionPeriod>> Filter( PageRequestDto dto)
        {
            var queryable = await _productionPeriodService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _productionPeriodService.GetQueryableQuickSearch(queryable, filterValue);
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        public async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<ProductionPeriod>();
            await _cacheManager.RemoveClassCache<ProductionPeriodDto>();
        }
        #endregion
    }
}
