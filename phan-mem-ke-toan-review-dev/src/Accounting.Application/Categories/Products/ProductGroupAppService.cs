using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Partners;
using Accounting.Catgories.Products;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
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
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Accounting.Categories.Products
{
    public class ProductGroupAppService : AccountingAppService, IProductGroupAppService
    {
        #region Fields
        private readonly ProductGroupService _productGroupService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IDistributedCache<ProductGroupDto> _cache;
        private readonly IDistributedCache<PageResultDto<ProductGroupDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public ProductGroupAppService(ProductGroupService productGroupService,
                                UserService userService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                IDistributedCache<ProductGroupDto> cache,
                                IDistributedCache<PageResultDto<ProductGroupDto>> pageCache,
                                CacheManager cacheManager,
                                LinkCodeBusiness linkCodeBusiness,
                                IStringLocalizer<AccountingResource> localizer
            )
        {
            _productGroupService = productGroupService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.ProductGroupManagerCreate)]
        public async Task<ProductGroupDto> CreateAsync(CrudProductGroupDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudProductGroupDto, ProductGroup>(dto);
            var result = await _productGroupService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<ProductGroup, ProductGroupDto>(result);
        }

        [Authorize(AccountingPermissions.ProductGroupManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _productGroupService.GetAsync(id);
            bool isUsing = await _productGroupService.IsParentGroup(entity.Id);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductGroup, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductGroupCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductGroup, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }

            var productGroup = await _productGroupService.GetQueryableAsync();
            var lstProductGroupChild = productGroup.Where(p => p.ParentId == id).ToList();
            if (lstProductGroupChild.Count > 0)
            {
                foreach (var item in lstProductGroupChild)
                {
                    item.ParentId = null;
                    await _productGroupService.UpdateAsync(item, true);
                }
            }
            await _productGroupService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.ProductGroupManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _productGroupService.GetAsync(item);
                bool isUsing = await _productGroupService.IsParentGroup(entity.Id);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductGroup, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
                isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductGroupCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductGroup, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _productGroupService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        public async Task<ProductGroupDto> GetByIdAsync(string productGroupId)
        {
            return await _cache.GetOrAddAsync(
                productGroupId, //Cache key
                async () =>
                {
                    var productGroup = await _productGroupService.GetAsync(productGroupId);
                    return ObjectMapper.Map<ProductGroup, ProductGroupDto>(productGroup);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );            
        }

        [Authorize(AccountingPermissions.ProductGroupManagerView)]
        public async Task<PageResultDto<ProductGroupDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ProductGroupDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductGroup, ProductGroupDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<BaseComboItemDto>> GetViewListAsync()
        {
            string key = string.Format(CacheKeyManager.ListByOrgCode, _webHelper.GetCurrentOrgUnit());
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<ProductGroupDto>(key);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var items = await _productGroupService.GetRepository()
                                    .GetListAsync(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                    return items.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Id,
                        Code = p.Code,
                        Name = p.Name,
                        ParentId = p.ParentId,
                        Value = p.Name
                    })
                    .OrderBy(p => p.Code).ToList();
                }
            );            
        }

        [Authorize(AccountingPermissions.ProductGroupManagerView)]
        public Task<PageResultDto<ProductGroupDto>> PagesAsync(PageRequestDto dto)
        {            
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.ProductGroupManagerUpdate)]
        public async Task UpdateAsync(string id, CrudProductGroupDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _productGroupService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _productGroupService.UpdateAsync(entity);
            await RemoveAllCache();
        }

        public async Task<List<ProductGroup>> GetChildGroup(string code)
        {
            var lstProductGroup = new List<ProductGroup>();
            var lstProductChildGroup = new List<ProductGroup>();
            var iQProductGroup = await _productGroupService.GetQueryableAsync();

            var lstProductGrouptemp = iQProductGroup.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var temp = lstProductGrouptemp.Where(x => x.Id == code || x.Code == code).Select(t => t.Code).FirstOrDefault();


            var productGroup = iQProductGroup.Where(p => ((code ?? "") == "" && (p.ParentId ?? "") == "" || p.Code == temp) && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();  //Chỗ này đang so sánh id với code(NHH)
            if (productGroup == null) return lstProductGroup;
            lstProductGroup.AddRange(productGroup);
            lstProductChildGroup.AddRange(productGroup);
            while (iQProductGroup.Any(p => lstProductChildGroup.Select(p => p.Id).Contains(p.ParentId)))
            {
                lstProductChildGroup = iQProductGroup.Where(p => lstProductChildGroup.Select(p => p.Id).Contains(p.ParentId)).ToList();
                lstProductGroup.AddRange(lstProductChildGroup);
            }
            return lstProductGroup;
        }

        public async Task<List<ProductGroupCustomineDto>> GetRankGroup(string id)
        {
            var lstProductGroup = new List<ProductGroupCustomineDto>();
            var lstProductChildGroup = new List<ProductGroupCustomineDto>();
            var iQProductGroup = await _productGroupService.GetQueryableAsync();
            var productGroup = ObjectMapper.Map(iQProductGroup.Where(p => (((id ?? "") == "" && (p.ParentId ?? "") == "") || p.Id == id) && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList(), new List<ProductGroupCustomineDto>());
            if (productGroup == null) return lstProductGroup;
            lstProductGroup.AddRange(productGroup);
            lstProductChildGroup.AddRange(productGroup);
            foreach (var item in lstProductChildGroup)
            {
                item.Rank = 1;
                item.OrdGroup = @"\" + item.Name + @"\";
            }
            int rank = 2;
            while (iQProductGroup.Any(p => lstProductChildGroup.Select(p => p.Id).Contains(p.ParentId)))
            {
                lstProductChildGroup = iQProductGroup.Where(p => lstProductChildGroup.Select(p => p.Id).Contains(p.ParentId)).Select(p => ObjectMapper.Map<ProductGroup, ProductGroupCustomineDto>(p)).ToList();
                foreach (var item in lstProductChildGroup)
                {
                    var ordGroupName = lstProductGroup.Where(p => p.Id == item.ParentId).FirstOrDefault();
                    item.OrdGroup = ordGroupName.OrdGroup + item.Name + @"\";
                    item.Rank = rank;
                }
                lstProductGroup.AddRange(lstProductChildGroup);
                rank++;
            }
            return lstProductGroup;
        }
        #region Private
        private async Task<IQueryable<ProductGroup>> Filter(PageRequestDto dto)
        {
            var queryable = await _productGroupService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            string key = string.Format(CacheKeyManager.ListByOrgCode, _webHelper.GetCurrentOrgUnit());
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<ProductGroupDto>(key);
            await _cacheManager.RemoveAsync(cacheKey);
        }
        #endregion
    }
}