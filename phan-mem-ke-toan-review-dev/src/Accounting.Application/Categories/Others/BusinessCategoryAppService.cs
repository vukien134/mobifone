using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.BusinessCategories;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Configs;
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
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.Others
{
    public class BusinessCategoryAppService : AccountingAppService, IBusinessCategoryAppService
    {
        #region Fields
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;        
        private readonly CacheManager _cacheManager;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public BusinessCategoryAppService(BusinessCategoryService businessCategoryService,
                            UserService userService,
                            WebHelper webHelper,
                            CacheManager cacheManager,
                            TenantExtendInfoService tenantExtendInfoService,
                            AccountingCacheManager accountingCacheManager,
                            ICurrentTenant currentTenant,
                            LinkCodeBusiness linkCodeBusiness,
                            LicenseBusiness licenseBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _businessCategoryService = businessCategoryService;
            _userService = userService;
            _webHelper = webHelper;
            _cacheManager = cacheManager;
            _tenantExtendInfoService = tenantExtendInfoService;
            _accountingCacheManager = accountingCacheManager;
            _currentTenant = currentTenant;
            _linkCodeBusiness = linkCodeBusiness;
            _licenseBusiness = licenseBusiness;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.BusinessCategoryManagerCreate)]
        public async Task<BusinessCategoryDto> CreateAsync(CrudBusinessCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudBusinessCategoryDto, BusinessCategory>(dto);
            var result = await _businessCategoryService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<BusinessCategory, BusinessCategoryDto>(result);
        }

        [Authorize(AccountingPermissions.BusinessCategoryManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _businessCategoryService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.BusinessCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.BusinessCategory, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _businessCategoryService.DeleteAsync(id);
            await this.RemoveAllCache();
        }

        [Authorize(AccountingPermissions.BusinessCategoryManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }
            foreach (var item in dto.ListId)
            {
                var entity = await _businessCategoryService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.BusinessCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.BusinessCategory, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _businessCategoryService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.BusinessCategoryManagerView)]
        public Task<PageResultDto<BusinessCategoryDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.BusinessCategoryManagerView)]
        public async Task<PageResultDto<BusinessCategoryDto>> GetListAsync(PageRequestDto dto)
        {
            await this.InsertDefaultAsync();
            var result = new PageResultDto<BusinessCategoryDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<BusinessCategory, BusinessCategoryDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.BusinessCategoryManagerUpdate)]
        public async Task UpdateAsync(string id, CrudBusinessCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _businessCategoryService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _businessCategoryService.UpdateAsync(entity);
            await this.RemoveAllCache();
        }
        public async Task<List<BaseComboItemDto>> GetDataReference(string voucherCode=null)
        {
            await this.InsertDefaultAsync();
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListBusinessCategoryByVoucherCode, orgCode, voucherCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<BusinessCategoryDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var queryable = await _businessCategoryService.GetQueryableAsync();
                    queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                    if (!string.IsNullOrEmpty(voucherCode))
                    {
                        queryable = queryable.Where(p => p.VoucherCode == voucherCode);
                    }
                    queryable = queryable.OrderBy(p => p.Code);
                    var partnes = await AsyncExecuter.ToListAsync(queryable);
                    return partnes.Select(p => new BaseComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name,
                        CreditAcc = p.CreditAcc,
                        DebitAcc = p.DebitAcc
                    }).ToList();
                }
            );            
        }
        public async Task<BusinessCategoryDto> GetByIdAsync(string businessCategoryId)
        {
            var entity = await _businessCategoryService.GetAsync(businessCategoryId);
            return ObjectMapper.Map<BusinessCategory, BusinessCategoryDto>(entity);
        }

        #region Private
        private async Task<IQueryable<BusinessCategory>> Filter(PageRequestDto dto)
        {
            var queryable = await _businessCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<BusinessCategoryDto>();
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _businessCategoryService.IsExistListAsync(orgCode);
            if (isExists) return;

            var tenantInfo = await _tenantExtendInfoService.GetByTenantId(_currentTenant.Id);
            if (tenantInfo == null) return;

            var defaultCategories = await _accountingCacheManager.GetDefaultBusinessCategoryAsync(tenantInfo.TenantType);
            if (defaultCategories.Count == 0) return;
            var entities = defaultCategories.Select(p =>
            {
                p.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<DefaultBusinessCategoryDto, BusinessCategory>(p);
                entity.OrgCode = orgCode;
                return entity;
            }).ToList();
            await _businessCategoryService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        #endregion
    }
}
