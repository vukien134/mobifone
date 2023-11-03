using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Accounts.AccOpeningBalances;
using Accounting.Catgories.Products;
using Accounting.Constants;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Extensions;
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

namespace Accounting.Categories.Products
{
    public class ProductLotAppService : AccountingAppService, IProductLotAppService
    {
        #region Fields
        private readonly ProductLotService _productLotService;
        private readonly UserService _userService;
        private readonly ExcelService _excelService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IDistributedCache<ProductLotDto> _cache;
        private readonly IDistributedCache<PageResultDto<ProductLotDto>> _pageCache;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public ProductLotAppService(ProductLotService productLotService,
                            UserService userService,
                            ExcelService excelService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            IDistributedCache<ProductLotDto> cache,
                            IDistributedCache<PageResultDto<ProductLotDto>> pageCache,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
                            )
        {
            _productLotService = productLotService;
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

        [Authorize(AccountingPermissions.ProductLotManagerCreate)]
        public async Task<ProductLotDto> CreateAsync(CrudProductLotDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudProductLotDto, ProductLot>(dto);
            var result = await _productLotService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<ProductLot, ProductLotDto>(result);
        }

        [Authorize(AccountingPermissions.ProductLotManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _productLotService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductLotCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductLot, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _productLotService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.ProductLotManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }
            foreach (var item in dto.ListId)
            {
                var entity = await _productLotService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.ProductLotCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductLot, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _productLotService.DeleteManyAsync(deleteIds);
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.ProductLotManagerView)]
        public async Task<PageResultDto<ProductLotDto>> PagesAsync(PageRequestDto dto)
        {
            string cacheKey = _cacheManager.GetCacheKeyByPageRequest<ProductLotDto>(dto);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () => await GetListAsync(dto)
            );            
        }
        [Authorize(AccountingPermissions.ProductLotManagerView)]
        public async Task<PageResultDto<ProductLotDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<ProductLotDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductLot, ProductLotDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.ProductLotManagerUpdate)]
        public async Task UpdateAsync(string id, CrudProductLotDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _productLotService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _productLotService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.ProductLotCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }
        public async Task<ProductLotDto> GetByIdAsync(string productLotId)
        {
            return await _cache.GetOrAddAsync(
                productLotId, //Cache key
                async () =>
                {
                    var productLot = await _productLotService.GetAsync(productLotId);
                    return ObjectMapper.Map<ProductLot, ProductLotDto>(productLot);
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
            string cacheKey = _cacheManager.GetCacheKeyByFilterValue<ProductLotDto>(filterValue);

            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var products = await _productLotService.GetDataReference(_webHelper.GetCurrentOrgUnit(), filterValue);
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
        [Authorize(AccountingPermissions.ProductLotManagerCreate)]
        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();

            var lstImport = await _excelService.ImportFileToList<CrudProductLotDto>(bytes, dto.WindowId);
            foreach (var item in lstImport)
            {
                var productLots = await _productLotService.GetByAccCodeAsync(item.Code);
                if (productLots.Count() != 0)
                {
                    throw new Exception("Mã lô hàng " + item.Code + " đã tồn tại!");
                }
                item.Id = this.GetNewObjectId();
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
            }

            var lstProductLot = lstImport.Select(p => ObjectMapper.Map<CrudProductLotDto, ProductLot>(p)).ToList();
            await _productLotService.CreateManyAsync(lstProductLot);
            await RemoveAllCache();
            return new UploadFileResponseDto() { Ok = true };
        }
        #region Private
        private async Task<IQueryable<ProductLot>> Filter(PageRequestDto dto)
        {
            var queryable = await _productLotService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                string filterValue = $"%{dto.QuickSearch}%";
                queryable = _productLotService.GetQueryableQuickSearch(queryable, filterValue);
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
            await _cacheManager.RemoveClassCache<ProductLotDto>();
        }
        #endregion
    }
}
