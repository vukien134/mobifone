using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.TenantSettings;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.Others
{
    public class TenantSettingAppService : AccountingAppService, ITenantSettingAppService
    {
        #region Fields
        private readonly TenantSettingService _tenantSettingService;
        private readonly CacheManager _cacheManager;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public TenantSettingAppService(TenantSettingService tenantSettingService,
                            CacheManager cacheManager,
                            UserService userService,
                            AccountingCacheManager accountingCacheManager,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper)
        {
            _tenantSettingService = tenantSettingService;
            _cacheManager = cacheManager;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        [Authorize(AccountingPermissions.TenantSettingManagerCreate)]
        public async Task<TenantSettingDto> CreateAsync(CrudTenantSettingDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudTenantSettingDto, TenantSetting>(dto);
            var result = await _tenantSettingService.CreateAsync(entity);
            await RemoveAllCache();
            return ObjectMapper.Map<TenantSetting, TenantSettingDto>(result);
        }
        [Authorize(AccountingPermissions.TenantSettingManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _tenantSettingService.DeleteAsync(id);
            await RemoveAllCache();
        }
        [Authorize(AccountingPermissions.TenantSettingManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            await RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }
        [Authorize(AccountingPermissions.TenantSettingManagerView)]
        public Task<PageResultDto<TenantSettingDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        [Authorize(AccountingPermissions.TenantSettingManagerView)]
        public async Task<PageResultDto<TenantSettingDto>> GetListAsync(PageRequestDto dto)
        {
            await InsertDefaultAsync();
            var result = new PageResultDto<TenantSettingDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Ord).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<TenantSetting, TenantSettingDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<JsonObject> GetMaxAmountAsync()
        {
            var tenantSetting = await _tenantSettingService.GetTenantSettingByKeyAsync("TIEN_MAX", _webHelper.GetCurrentOrgUnit());
            var res = new JsonObject();
            res.Add("key", "maxAmount");
            res.Add("value", tenantSetting.Value);
            return res;
        }

        public async Task<TenantSettingDto> GetByIdAsync(string tenantSettingId)
        {
            var tenantSetting = await _tenantSettingService.GetAsync(tenantSettingId);
            return ObjectMapper.Map<TenantSetting, TenantSettingDto>(tenantSetting);
        }
        [Authorize(AccountingPermissions.TenantSettingManagerUpdate)]
        public async Task UpdateAsync(string id, CrudTenantSettingDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _tenantSettingService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _tenantSettingService.UpdateAsync(entity);
            await RemoveAllCache();
        }
        #region Private
        private async Task<IQueryable<TenantSetting>> Filter(PageRequestDto dto)
        {
            var queryable = await _tenantSettingService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            return queryable;
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _tenantSettingService.IsExistListAsync(orgCode);
            if (isExists) return;

            var defaultCurrencies = await _accountingCacheManager.GetDefaultTenantSettingAsync();
            var entities = defaultCurrencies.Select(p =>
            {
                var dto = ObjectMapper.Map<DefaultTenantSettingDto, CrudTenantSettingDto>(p);
                dto.OrgCode = orgCode;
                dto.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<CrudTenantSettingDto, TenantSetting>(dto);
                return entity;
            }).ToList();
            await _tenantSettingService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task RemoveAllCache()
        {
            await _cacheManager.RemoveClassCache<TenantSettingDto>();
        }
        #endregion
    }
}
