using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.TaxCategories;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.Exceptions;
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
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.Others
{
    public class TaxCategoryAppService : AccountingAppService, ITaxCategoryAppService
    {
        #region Fields
        private readonly TaxCategoryService _taxCategoryService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly DefaultTaxCategoryService _defaultTaxCategoryService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly CacheManager _cacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public TaxCategoryAppService(TaxCategoryService taxCategoryService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            DefaultTaxCategoryService defaultTaxCategoryService,
                            AccountingCacheManager accountingCacheManager,
                            CacheManager cacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _taxCategoryService = taxCategoryService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _defaultTaxCategoryService = defaultTaxCategoryService;
            _accountingCacheManager = accountingCacheManager;
            _cacheManager = cacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.TaxCategoryManagerCreate)]
        public async Task<TaxCategoryDto> CreateAsync(CrudTaxCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudTaxCategoryDto, TaxCategory>(dto);
            var result = await _taxCategoryService.CreateAsync(entity);
            return ObjectMapper.Map<TaxCategory, TaxCategoryDto>(result);
        }

        [Authorize(AccountingPermissions.TaxCategoryManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _taxCategoryService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.TaxCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.TaxCategory, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _taxCategoryService.DeleteAsync(id);
            await RemoveAllCache();
        }

        [Authorize(AccountingPermissions.TaxCategoryManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }

            foreach (var item in dto.ListId)
            {
                var entity = await _taxCategoryService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.TaxCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.TaxCategory, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _taxCategoryService.DeleteManyAsync(deleteIds);
            await RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.TaxCategoryManagerView)]
        public Task<PageResultDto<TaxCategoryDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.TaxCategoryManagerView)]
        public async Task<PageResultDto<TaxCategoryDto>> GetListAsync(PageRequestDto dto)
        {
            await this.InsertDefaultAsync();
            var result = new PageResultDto<TaxCategoryDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<TaxCategory, TaxCategoryDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.TaxCategoryManagerUpdate)]
        public async Task UpdateAsync(string id, CrudTaxCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _taxCategoryService.GetAsync(id);
            string oldCode = entity.Code;
            bool isChangeCode = dto.Code == entity.Code ? false : true;
            ObjectMapper.Map(dto, entity);
            await _taxCategoryService.UpdateAsync(entity);
            if (isChangeCode)
            {
                await _linkCodeBusiness.UpdateCode(LinkCodeConst.TaxCode, dto.Code, oldCode, entity.OrgCode);
            }
            await RemoveAllCache();
        }
        public async Task<List<TaxCategoryComboItemDto>> GetDataReference(string outOrIn = null)
        {
            string key = string.Format(CacheKeyManager.ListTaxCategory, _webHelper.GetCurrentOrgUnit(),
                                                outOrIn);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<TaxCategoryComboItemDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var queryable = await _taxCategoryService.GetQueryableAsync();
                    queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                    if (!string.IsNullOrEmpty(outOrIn))
                    {
                        queryable = queryable.Where(p => p.OutOrIn == outOrIn);
                    }
                    queryable = queryable.OrderBy(p => p.Code);
                    var partnes = await AsyncExecuter.ToListAsync(queryable);
                    if (partnes.Count == 0)
                    {
                        var defaultTaxes = await _accountingCacheManager.GetDefaultTaxCategoryAsync();
                        return defaultTaxes.Select(p => new TaxCategoryComboItemDto()
                        {
                            Id = p.Code,
                            Value = p.Code,
                            Code = p.Code,
                            Name = p.Name,
                            DebitAcc = p.DebitAcc,
                            CreditAcc = p.CreditAcc,
                            Percentage = p.Percentage
                        }).ToList();
                    }
                    return partnes.Select(p => new TaxCategoryComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name,
                        DebitAcc = p.DebitAcc,
                        CreditAcc = p.CreditAcc,
                        Percentage = p.Percentage
                    }).ToList();
                }
            );            
        }
        public async Task<TaxCategoryDto> GetByIdAsync(string taxCategoryId)
        {
            var entity = await _taxCategoryService.GetAsync(taxCategoryId);
            return ObjectMapper.Map<TaxCategory, TaxCategoryDto>(entity);
        }
        #region Private
        private async Task<IQueryable<TaxCategory>> Filter(PageRequestDto dto)
        {
            var queryable = await _taxCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("deduct"))
                {
                    int value = Convert.ToInt32(item.Value.ToString());
                    queryable = queryable.Where(p => p.Deduct == value);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExist = await _taxCategoryService.IsExistListAsync(orgCode);
            if (isExist) return;
            var defaultExciseTaxs = await _accountingCacheManager.GetDefaultTaxCategoryAsync();
            var entities = defaultExciseTaxs.Select(p =>
            {
                p.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<DefaultTaxCategoryDto, TaxCategory>(p);
                entity.OrgCode = orgCode;
                return entity;
            }).ToList();
            await _taxCategoryService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<TaxCategoryComboItemDto>();
        }
        #endregion
    }
}
