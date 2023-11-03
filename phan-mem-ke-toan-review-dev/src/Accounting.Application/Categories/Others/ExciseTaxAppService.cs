using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.ExciseTaxes;
using Accounting.Catgories.Others.TenantSettings;
using Accounting.Constants;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Users;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Accounting.Categories.Others
{
    public class ExciseTaxAppService : AccountingAppService, IExciseTaxAppService
    {
        #region Fields
        private readonly ExciseTaxService _exciseTaxService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly CacheManager _cacheManager;
        #endregion
        #region Ctor
        public ExciseTaxAppService(ExciseTaxService exciseTaxService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            AccountingCacheManager accountingCacheManager,
                            CacheManager cacheManager
                            )
        {
            _exciseTaxService = exciseTaxService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _cacheManager = cacheManager;
        }
        #endregion

        [Authorize(AccountingPermissions.ExciseTaxManagerCreate)]
        public async Task<ExciseTaxDto> CreateAsync(CrudExciseTaxDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudExciseTaxDto, ExciseTax>(dto);
            var result = await _exciseTaxService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<ExciseTax, ExciseTaxDto>(result);
        }

        [Authorize(AccountingPermissions.ExciseTaxManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _exciseTaxService.DeleteAsync(id);
            await this.RemoveAllCache();
        }

        [Authorize(AccountingPermissions.ExciseTaxManagerDelete)]
        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }

        public async Task<ExciseTaxDto> GetByIdAsync(string exciseId)
        {
            var exciseTax = await _exciseTaxService.GetAsync(exciseId);
            return ObjectMapper.Map<ExciseTax, ExciseTaxDto>(exciseTax);
        }

        [Authorize(AccountingPermissions.ExciseTaxManagerView)]
        public async Task<PageResultDto<ExciseTaxDto>> GetListAsync(PageRequestDto dto)
        {
            await this.InsertDefaultAsync();
            var result = new PageResultDto<ExciseTaxDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ExciseTax, ExciseTaxDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.ExciseTaxManagerView)]
        public Task<PageResultDto<ExciseTaxDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }
        public async Task<List<ExciseTaxComboItemDto>> GetDataReference()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<ExciseTaxComboItemDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var queryable = await _exciseTaxService.GetQueryableAsync();
                    queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                        .OrderBy(p => p.Code);
                    var entities = await AsyncExecuter.ToListAsync(queryable);
                    if (entities.Count == 0)
                    {
                        var defaultTaxes = await _accountingCacheManager.GetDefaultExciseTaxAsync();
                        return defaultTaxes.Select(p => new ExciseTaxComboItemDto()
                        {
                            Id = p.Code,
                            Value = p.Code,
                            Code = p.Code,
                            Name = p.Name,
                            ExciseTaxPercentage = p.ExciseTaxPercentage
                        }).ToList();
                    }
                    return entities.Select(p => new ExciseTaxComboItemDto()
                    {
                        Id = p.Code,
                        Value = p.Code,
                        Code = p.Code,
                        Name = p.Name,
                        ExciseTaxPercentage = p.ExciseTaxPercentage
                    }).ToList();
                }
            );            
        }
        [Authorize(AccountingPermissions.ExciseTaxManagerUpdate)]
        public async Task UpdateAsync(string id, CrudExciseTaxDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _exciseTaxService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _exciseTaxService.UpdateAsync(entity);
            await this.RemoveAllCache();
        }
        #region Private
        private async Task<IQueryable<ExciseTax>> Filter(PageRequestDto dto)
        {
            var queryable = await _exciseTaxService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                queryable = queryable.Where(p => p.Code.Contains(dto.QuickSearch) || p.Name.Contains(dto.QuickSearch));
            }

            if (dto.FilterRows == null) return queryable;
            string tenantDecimalSymbol = await this.GetSymbolSeparateNumber();
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("exciseTaxPercentage"))
                {
                    string value = item.Value.ToString();
                    queryable = this.FilterByPercentage(queryable, value,  tenantDecimalSymbol);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
            }
            return queryable;
        }
        private IQueryable<ExciseTax> FilterByPercentage(IQueryable<ExciseTax> queryable,
                                            string value, string tenantDecimalSymbol)
        {
            if (value.StartsWith(OperatorFilter.GreaterThan))
            {
                value = value.Replace(OperatorFilter.GreaterThan, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExciseTaxPercentage > _value);
            }
            else if (value.StartsWith(OperatorFilter.GreaterThanOrEqual))
            {
                value = value.Replace(OperatorFilter.GreaterThanOrEqual, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExciseTaxPercentage >= _value);
            }
            else if (value.StartsWith(OperatorFilter.LessThan))
            {
                value = value.Replace(OperatorFilter.LessThan, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExciseTaxPercentage < _value);
            }
            else if (value.StartsWith(OperatorFilter.LessThanOrEqual))
            {
                value = value.Replace(OperatorFilter.LessThanOrEqual, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExciseTaxPercentage <= _value);
            }
            else if (value.StartsWith(OperatorFilter.NumEqual))
            {
                value = value.Replace(OperatorFilter.NumEqual, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExciseTaxPercentage == _value);
            }
            return queryable;
        }
        private decimal? GetNumberDecimal(string value, string tenantDecimalSymbol)
        {
            string decimalSymbol = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            if (decimalSymbol.Equals(tenantDecimalSymbol)) return Convert.ToDecimal(value);
            value = value.Replace(tenantDecimalSymbol, decimalSymbol);
            return Convert.ToDecimal(value);
        }
        private async Task<string> GetSymbolSeparateNumber()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await this.GetNumberSeparateSymbol();
            foreach (var item in currencyFormats)
            {
                if (item.Key.Equals(CurrencyConst.SymbolSeparateDecimal)) return item.Value;
            }
            return null;
        }
        private async Task<List<TenantSettingDto>> GetNumberSeparateSymbol()
        {
            var tenantSettings = await _accountingCacheManager.GetTenantSettingAsync();
            if (tenantSettings.Count > 0)
            {
                var dtos = tenantSettings.Where(p => p.Key == CurrencyConst.SymbolSeparateGroupDigit
                                || p.Key == CurrencyConst.SymbolSeparateDecimal)
                                .ToList();
                return dtos;
            }
            var defaultTenantSettings = await _accountingCacheManager.GetDefaultTenantSettingAsync();
            if (defaultTenantSettings.Count > 0)
            {
                var defaultDtos = defaultTenantSettings.Where(p => p.Key == CurrencyConst.SymbolSeparateGroupDigit
                                || p.Key == CurrencyConst.SymbolSeparateDecimal).ToList();
                var dtos = defaultDtos.Select(p => new TenantSettingDto()
                {
                    Id = p.Id,
                    Key = p.Key,
                    Ord = p.Ord,
                    SettingType = p.SettingType,
                    Type = p.Type,
                    Value = p.Value
                }).ToList();
                return dtos;
            }
            return null;
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExist = await _exciseTaxService.IsExistListAsync(orgCode);
            if (isExist) return;
            var defaultExciseTaxs = await _accountingCacheManager.GetDefaultExciseTaxAsync();
            var entities = defaultExciseTaxs.Select(p =>
            {
                p.Id = this.GetNewObjectId();                
                var entity = ObjectMapper.Map<DefaultExciseTaxDto, ExciseTax>(p);
                entity.OrgCode = orgCode;                
                return entity;
            }).ToList();
            await _exciseTaxService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        private async Task RemoveAllCache()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<ExciseTaxComboItemDto>(key);
            await _cacheManager.RemoveAsync(cacheKey);
        }
        #endregion
    }
}
