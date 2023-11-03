using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Catgories.VoucherCategories;
using Accounting.Common.Extensions;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.VoucherCategories
{
    public class VoucherCategoryAppService : AccountingAppService, IVoucherCategoryAppService
    {
        #region Fields
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly TenantExtendInfoService _tenantExtendInfoService;
        private readonly ICurrentTenant _currentTenant;
        #endregion
        #region Ctor
        public VoucherCategoryAppService(VoucherCategoryService voucherCategoryService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            DefaultVoucherCategoryService defaultVoucherCategoryService,
                            AccountingCacheManager accountingCacheManager,
                            TenantExtendInfoService tenantExtendInfoService,
                            ICurrentTenant currentTenant
            )
        {
            _voucherCategoryService = voucherCategoryService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _accountingCacheManager = accountingCacheManager;
            _tenantExtendInfoService = tenantExtendInfoService;
            _currentTenant = currentTenant;
        }
        #endregion
        [Authorize(AccountingPermissions.VoucherCategoryManagerCreate)]
        public async Task<VoucherCategoryDto> CreateAsync(CruVoucherCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.TenantType = await _tenantExtendInfoService.GetTenantType(_currentTenant.Id);
            var entity = ObjectMapper.Map<CruVoucherCategoryDto, VoucherCategory>(dto);
            var result = await _voucherCategoryService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<VoucherCategory, VoucherCategoryDto>(result);
        }
        [Authorize(AccountingPermissions.VoucherCategoryManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _voucherCategoryService.DeleteAsync(id);
            await this.RemoveAllCache();
        }
        [Authorize(AccountingPermissions.VoucherCategoryManagerView)]
        public Task<PageResultDto<VoucherCategoryDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.VoucherCategoryManagerView)]
        public async Task<PageResultDto<VoucherCategoryDto>> GetListAsync(PageRequestDto dto)
        {
            await InsertDefaultAsync();
            var result = new PageResultDto<VoucherCategoryDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<VoucherCategory, VoucherCategoryDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        [Authorize(AccountingPermissions.VoucherCategoryManagerUpdate)]
        public async Task UpdateAsync(string id, CruVoucherCategoryDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _voucherCategoryService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _voucherCategoryService.UpdateAsync(entity);
            await this.RemoveAllCache();
        }

        public async Task<List<BaseComboItemDto>> GetDataReference()
        {
            var sections = await _accountingCacheManager.GetVoucherCategoryAsync();
            if (sections.Count == 0)
            {
                var defaultVouchers = await _accountingCacheManager.GetDefaultVoucherCategoryAsync();
                return defaultVouchers.Select(p => new BaseComboItemDto()
                {
                    Id = p.Code,
                    Value = p.Code,
                    Code = p.Code,
                    Name = p.Name
                }).OrderBy(p => p.Code).ToList();
            }

            return sections.Select(p => new BaseComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name
            }).OrderBy(p => p.Code).ToList();
        }
        public async Task<VoucherCategoryDto> GetByIdAsync(string voucherCategoryId)
        {
            var entity = await _voucherCategoryService.GetAsync(voucherCategoryId);
            return ObjectMapper.Map<VoucherCategory, VoucherCategoryDto>(entity);
        }
        #region Private
        private async Task<IQueryable<VoucherCategory>> Filter(PageRequestDto dto)
        {
            int? tenantType = await _tenantExtendInfoService.GetTenantType(_currentTenant.Id);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var queryable = await _voucherCategoryService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode) && p.TenantType == tenantType);
            if (dto.FilterRows == null) return queryable;

            foreach(var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("voucherGroup"))
                {
                    int voucherGroup = Convert.ToInt32(item.Value.ToString());
                    queryable = queryable.Where(p => p.VoucherGroup == voucherGroup);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _voucherCategoryService.IsExistListAsync(orgCode);
            if (isExists) return;

            var defaultCurrencies = await _accountingCacheManager.GetDefaultVoucherCategoryAsync();
            var entities = defaultCurrencies.Select(p =>
            {
                var dto = ObjectMapper.Map<DefaultVoucherCategoryDto, CruVoucherCategoryDto>(p);
                dto.OrgCode = orgCode;
                dto.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<CruVoucherCategoryDto, VoucherCategory>(dto);
                return entity;
            }).ToList();
            await _voucherCategoryService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        private async Task RemoveAllCache()
        {
            await _accountingCacheManager.RemoveClassCache<VoucherCategoryDto>();
        }
        #endregion
    }
}
