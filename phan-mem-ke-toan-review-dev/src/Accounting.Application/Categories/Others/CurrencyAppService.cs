using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Catgories.Others.Careers;
using Accounting.Catgories.Others.Currencies;
using Accounting.Catgories.Others.TenantSettings;
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
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;

namespace Accounting.Categories.Others
{
    public class CurrencyAppService : AccountingAppService, ICurrencyAppService
    {
        #region Fields
        private readonly CurrencyService _currencyService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly DefaultCurrencyService _defaultCurrencyService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly LinkCodeBusiness _linkCodeBusiness;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public CurrencyAppService(CurrencyService currencyService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            DefaultCurrencyService defaultCurrencyService,
                            AccountingCacheManager accountingCacheManager,
                            LinkCodeBusiness linkCodeBusiness,
                            IStringLocalizer<AccountingResource> localizer
            )
        {
            _currencyService = currencyService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _defaultCurrencyService = defaultCurrencyService;
            _accountingCacheManager = accountingCacheManager;
            _linkCodeBusiness = linkCodeBusiness;
            _localizer = localizer;
        }
        #endregion

        [Authorize(AccountingPermissions.CurrencyManagerCreate)]
        public async Task<CurrencyDto> CreateAsync(CrudCurrencyDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = ObjectMapper.Map<CrudCurrencyDto, Currency>(dto);
            var result = await _currencyService.CreateAsync(entity);
            await this.RemoveAllCache();
            return ObjectMapper.Map<Currency, CurrencyDto>(result);
        }

        [Authorize(AccountingPermissions.CurrencyManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _currencyService.GetAsync(id);
            bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.CurrencyCode, entity.Code, entity.OrgCode);
            if (isUsing)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Currency, ErrorCode.IsUsing),
                        _localizer["Err:CodeIsUsing", entity.Code]);
            }
            await _currencyService.DeleteAsync(id);
            await this.RemoveAllCache();
        }

        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.ListId == null)
            {
                throw new ArgumentNullException(nameof(dto.ListId));
            }
            foreach (var item in dto.ListId)
            {
                var entity = await _currencyService.GetAsync(item);
                bool isUsing = await _linkCodeBusiness.IsCodeUsing(LinkCodeConst.CurrencyCode, entity.Code, entity.OrgCode);
                if (isUsing)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Currency, ErrorCode.IsUsing),
                            _localizer["Err:CodeIsUsing", entity.Code]);
                }
            }
            string[] deleteIds = dto.ListId.ToArray();
            await _currencyService.DeleteManyAsync(deleteIds);
            await this.RemoveAllCache();
            var res = new ResultDto();
            res.Ok = true;
            res.Message = _localizer["success"];
            return res;
        }

        [Authorize(AccountingPermissions.CurrencyManagerView)]
        public Task<PageResultDto<CurrencyDto>> PagesAsync(PageRequestDto dto)
        {
            return GetListAsync(dto);
        }

        [Authorize(AccountingPermissions.CurrencyManagerView)]
        public async Task<PageResultDto<CurrencyDto>> GetListAsync(PageRequestDto dto)
        {
            await InsertDefaultAsync();
            var result = new PageResultDto<CurrencyDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.Code).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<Currency, CurrencyDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        [Authorize(AccountingPermissions.CurrencyManagerUpdate)]
        public async Task UpdateAsync(string id, CrudCurrencyDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var entity = await _currencyService.GetAsync(id);
            ObjectMapper.Map(dto, entity);
            await _currencyService.UpdateAsync(entity);
            await this.RemoveAllCache();
        }

        public async Task<CurrencyDto> GetByIdAsync(string currencyId)
        {
            var entity = await _currencyService.GetAsync(currencyId);
            return ObjectMapper.Map<Currency, CurrencyDto>(entity);
        }
        public async Task<List<CurrencyComboItemDto>> GetDataReference()
        {
            await InsertDefaultAsync();
            var partnes = await _accountingCacheManager.GetCurrenciesAsync();
            if (partnes.Count == 0)
            {
                var defaultCurrencies = await _accountingCacheManager.GetDefaultCurrenciesAsync();
                return defaultCurrencies.Select(p => new CurrencyComboItemDto()
                {
                    Id = p.Code,
                    Value = p.Code,
                    Code = p.Code,
                    Name = p.Name,
                    ExchangeRate = p.ExchangeRate,
                    Default = p.Default,
                    ExchangeMethod = p.ExchangeMethod
                }).ToList();
            }
            return partnes.Select(p => new CurrencyComboItemDto()
            {
                Id = p.Code,
                Value = p.Code,
                Code = p.Code,
                Name = p.Name,
                ExchangeRate = p.ExchangeRate,
                Default = p.Default,
                ExchangeMethod = p.ExchangeMethod
            }).ToList();
        }
        #region Private
        private async Task<IQueryable<Currency>> Filter(PageRequestDto dto)
        {
            var queryable = await _currencyService.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            if (dto.FilterRows == null) return queryable;
            string tenantDecimalSymbol = await this.GetSymbolSeparateNumber();
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnName.Equals("exchangeMethod"))
                {
                    bool value = Convert.ToBoolean(item.Value.ToString());
                    queryable = queryable.Where(p => p.ExchangeMethod == value);
                    continue;
                }
                if (item.ColumnName.Equals("exchangeRate"))
                {
                    string value = item.Value.ToString();
                    queryable = this.FilterByExchangeRate(queryable, value, item, tenantDecimalSymbol);
                    continue;
                }
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
            }
            return queryable;
        }
        private async Task RemoveAllCache()
        {
            await _accountingCacheManager.RemoveClassCache<CurrencyDto>();
        }
        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _currencyService.IsExistListAsync(orgCode);
            if (isExists) return;

            var defaultCurrencies = await _accountingCacheManager.GetDefaultCurrenciesAsync();
            var entities = defaultCurrencies.Select(p =>
            {
                var dto = ObjectMapper.Map<DefaultCurrencyDto, CrudCurrencyDto>(p);
                dto.OrgCode = orgCode;
                dto.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<CrudCurrencyDto, Currency>(dto);
                return entity;
            }).ToList();
            await _currencyService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }
        private IQueryable<Currency> FilterByExchangeRate(IQueryable<Currency> queryable,
                                            string value,FilterRowItemDto item,string tenantDecimalSymbol)
        {
            if (value.StartsWith(OperatorFilter.GreaterThan))
            {
                value = value.Replace(OperatorFilter.GreaterThan, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExchangeRate > _value);
            }
            else if (value.StartsWith(OperatorFilter.GreaterThanOrEqual))
            {
                value = value.Replace(OperatorFilter.GreaterThanOrEqual, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExchangeRate >= _value);
            }
            else if (value.StartsWith(OperatorFilter.LessThan))
            {
                value = value.Replace(OperatorFilter.LessThan, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExchangeRate < _value);
            }
            else if (value.StartsWith(OperatorFilter.LessThanOrEqual))
            {
                value = value.Replace(OperatorFilter.LessThanOrEqual, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExchangeRate <= _value);
            }
            else if (value.StartsWith(OperatorFilter.NumEqual))
            {
                value = value.Replace(OperatorFilter.NumEqual, "");
                decimal? _value = this.GetNumberDecimal(value, tenantDecimalSymbol);
                queryable = queryable.Where(p => p.ExchangeRate == _value);
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
        #endregion
    }
}
