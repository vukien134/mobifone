using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.ProductOthers;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;

namespace Accounting.Categories.Others
{
    public class ProductOriginAppService : AccountingAppService, IProductOriginAppSevice
    {
        #region Fields
        private readonly ProductOriginService _productOriginService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IDistributedCache<ProductOriginDto> _cache;
        private readonly IDistributedCache<PageResultDto<ProductOriginDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public ProductOriginAppService(ProductOriginService productOriginService,
                                   UserService userService,
                                   ExcelService excelService,
                                   LicenseBusiness licenseBusiness,
                                   WebHelper webHelper,
                                   IDistributedCache<ProductOriginDto> cache,
                                   IDistributedCache<PageResultDto<ProductOriginDto>> pageCache,
                                   CacheManager cacheManager,
                                   LinkCodeBusiness linkCodeBusiness,
                                   IStringLocalizer<AccountingResource> localizer
            )
        {
            _productOriginService = productOriginService;
            _userService = userService;
            _excelService = excelService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _cache = cache;
            _pageCache = pageCache;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion
        [Authorize(AccountingPermissions.ProductOriginManagerCreate)]
        public async Task<ProductOriginDto> CreateAsync(CrudProductOriginDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudProductOriginDto, ProductOrigin>(dto);
            var result = await _productOriginService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<ProductOrigin, ProductOriginDto>(result);
        }
        [Authorize(AccountingPermissions.ProductOriginManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _productOriginService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductOriginCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductOrigin, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _productOriginService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.ProductOriginManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _productOriginService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductOriginCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductOrigin, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _productOriginService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.ProductOriginManagerView)]
        public Task<PageResultDto<ProductOriginDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.ProductOriginManagerView)]
        public async Task<PageResultDto<ProductOriginDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ProductOriginDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductOrigin, ProductOriginDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<List<BaseComboItemDto>> DataReference(ComboRequestDto dto)
        {
            string filterValue = $"%{dto.FilterValue}%";
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<ProductOriginDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var products = await _productOriginService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
                    return products.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name
                    }).ToList();
                }
            );            
        }
        public async Task<ProductOriginDto> GetByIdAsync(string productOriginId)
        {
            return await _cache.GetOrAddAsync(
                productOriginId, //Cache key
                async () =>
                {
                    var accCase = await _productOriginService.GetAsync(productOriginId);
                    return ObjectMapper.Map<ProductOrigin, ProductOriginDto>(accCase);
                },
                () => new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = DateTimeOffset.Now.AddHours(1)
                }
            );            
        }
        [Authorize(AccountingPermissions.ProductOriginManagerUpdate)]
        public async Task UpdateAsync(string id, CrudProductOriginDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _productOriginService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _productOriginService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.ProductOriginCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }
        [Authorize(AccountingPermissions.ProductOriginManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudProductOriginDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {
                var accOpeningBalances = await _productOriginService.GetByAccCodeAsync(item.Code);
                if (accOpeningBalances.Count() != 0)
                {
                    throw new Exception("Mã nguồn hàng " + item.Code + " đã tồn tại!");
                }
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
            }

            var lstProductLot = lstImport.Select(p => ObjectMapper.Map<CrudProductOriginDto, ProductOrigin>(p)).ToList();
            await _productOriginService.CreateManyAsync(lstProductLot);
            await RemoveAllCache();
            return new UploadFileResponseDto() { Ok = true };
        }
        #region Private
        private async Task<IQueryable<ProductOrigin>> Filter(PageRequestDto dto)
        {
            var queryable = await _productOriginService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _productOriginService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<ProductOriginDto>();
        }
        #endregion
    }
}

