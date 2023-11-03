using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.AssetTools;
using Accounting.Catgories.Capitals;
using Accounting.Catgories.FProductWorks;
using Accounting.Catgories.Others.BusinessCategories;
using Accounting.Catgories.Others.Currencies;
using Accounting.Catgories.Others.Departments;
using Accounting.Catgories.Others.ExciseTaxes;
using Accounting.Catgories.Others.TaxCategories;
using Accounting.Catgories.Others.TenantSettings;
using Accounting.Catgories.Partners;
using Accounting.Catgories.Reasons;
using Accounting.Catgories.VoucherCategories;
using Accounting.Catgories.VoucherTypes;
using Accounting.Catgories.YearCategories;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.AssetTools;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Configs;
using Accounting.DomainServices.Windows;
using Accounting.Helpers;
using Accounting.Others;
using Accounting.Vouchers;
using Accounting.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;
using Volo.Abp.ObjectMapping;

namespace Accounting.Caching
{
    public class AccountingCacheManager : ITransientDependency
    {
        #region Privates
        private readonly AccountSystemService _accountSystemService;
        private readonly DefaultAccountSystemService _defaultAccountSystemService;
        private readonly YearCategoryService _yearCategoryService;
        private readonly CurrencyService _currencyService;
        private readonly DefaultCurrencyService _defaultCurrencyService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly AssetToolGroupService _assetToolGroupService;
        private readonly DefaultAssetToolGroupService _defaultAssetToolGroupService;
        private readonly DefaultExciseTaxService _defaultExciseTaxService;
        private readonly DefaultTaxCategoryService _defaultTaxCategoryService;
        private readonly DefaultBusinessCategoryService _defaultBusinessCategoryService;
        private readonly LinkCodeService _linkCodeService;
        private readonly WindowService _windowService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly WebHelper _webHelper;
        private readonly CacheManager _cacheManager;
        private readonly IObjectMapper _objectMapper;
        private readonly ICurrentTenant _currentTenant;
        private readonly AccPartnerService _partnerService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly DepartmentService _departmentService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly DefaultVoucherTypeService _defaultVoucherTypeService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly DefaultCapitalService _defaultCapitalService;
        private readonly CapitalService _capitalService;
        private readonly DefaultReasonService _defaultReasonService;
        private readonly ReasonService _reasonService;
        private readonly TenantExtendInfoService _tenantExtendInfoService;        
        #endregion
        #region Ctor
        public AccountingCacheManager(AccountSystemService accountSystemService,
                DefaultAccountSystemService defaultAccountSystemService,
                YearCategoryService yearCategoryService,
                WebHelper webHelper,
                CacheManager cacheManager,
                CurrencyService currencyService,
                DefaultCurrencyService defaultCurrencyService,
                TenantSettingService tenantSettingService,
                DefaultTenantSettingService defaultTenantSettingService,
                VoucherCategoryService voucherCategoryService,
                DefaultVoucherCategoryService defaultVoucherCategoryService,
                AssetToolGroupService assetToolGroupService,
                DefaultAssetToolGroupService defaultAssetToolGroupService,
                DefaultExciseTaxService defaultExciseTaxService,
                DefaultTaxCategoryService defaultTaxCategoryService,
                DefaultBusinessCategoryService defaultBusinessCategoryService,
                PartnerGroupService partnerGroupService,
                LinkCodeService linkCodeService,
                WindowService windowService,
                IObjectMapper objectMapper,
                ICurrentTenant currentTenant,
                AccPartnerService partnerService,
                FProductWorkService fProductWorkService,
                TaxCategoryService taxCategoryService,
                DepartmentService departmentService,
                BusinessCategoryService businessCategoryService,
                DefaultVoucherTypeService defaultVoucherTypeService,
                VoucherTypeService voucherTypeService,
                DefaultCapitalService defaultCapitalService,
                CapitalService capitalService,
                DefaultReasonService defaultReasonService,
                ReasonService reasonService,
                TenantExtendInfoService tenantExtendInfoService
            )
        {
            _accountSystemService = accountSystemService;
            _defaultAccountSystemService = defaultAccountSystemService;
            _webHelper = webHelper;
            _cacheManager = cacheManager;
            _yearCategoryService = yearCategoryService;
            _currencyService = currencyService;
            _defaultCurrencyService = defaultCurrencyService;
            _objectMapper = objectMapper;
            _tenantSettingService = tenantSettingService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _defaultAssetToolGroupService = defaultAssetToolGroupService;
            _assetToolGroupService = assetToolGroupService;
            _defaultExciseTaxService = defaultExciseTaxService;
            _defaultTaxCategoryService = defaultTaxCategoryService;
            _defaultBusinessCategoryService = defaultBusinessCategoryService;
            _linkCodeService = linkCodeService;
            _windowService = windowService;
            _partnerGroupService = partnerGroupService;
            _currentTenant = currentTenant;
            _partnerService = partnerService;
            _fProductWorkService = fProductWorkService;
            _taxCategoryService = taxCategoryService;
            _departmentService = departmentService;
            _businessCategoryService = businessCategoryService;
            _defaultVoucherTypeService = defaultVoucherTypeService;
            _voucherTypeService = voucherTypeService;
            _defaultCapitalService = defaultCapitalService;
            _capitalService = capitalService;
            _defaultReasonService = defaultReasonService;
            _reasonService = reasonService;
            _tenantExtendInfoService = tenantExtendInfoService;
        }
        #endregion
        #region Methods
        public async Task<Dictionary<string, string>> GetCurrencyFormats(string orgCode)
        {
            string key = string.Format(CacheKeyManager.ListCurrency, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<Dictionary<string, string>>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var currencyFormats = await _tenantSettingService.GetListFormatNumberFields(orgCode);
                    var dict = new Dictionary<string, string>();
                    foreach (var item in currencyFormats)
                    {
                        if (dict.ContainsKey(item.Key)) continue;
                        dict.Add(item.Key, item.Value);
                    }
                    return dict;
                }
            );
        }
        public async Task<List<VoucherTypeDto>> GetVoucherTypeAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<VoucherTypeDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _voucherTypeService.GetRepository().GetListAsync();
                    if (partnes.Count == 0)
                    {
                        var defaultVoucherType = await _defaultVoucherTypeService.GetListAsync();                        
                        return defaultVoucherType.Select(p => _objectMapper.Map<DefaultVoucherType,VoucherTypeDto>(p))
                                    .ToList();
                    }
                    return partnes.Select(p => _objectMapper.Map<VoucherType,VoucherTypeDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<BusinessCategoryDto> GetBusinessCategoryByCodeAsync(string code, string orgCode = null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<BusinessCategoryDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _businessCategoryService.GetBusinessByCodeAsync(code, orgCode);
                    return _objectMapper.Map<BusinessCategory, BusinessCategoryDto>(partnes);
                }
            );
        }
        public async Task<DepartmentDto> GetDepartmentByCodeAsync(string code, string orgCode = null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DepartmentDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _departmentService.GetDepartmentByCodeAsync(code, orgCode);
                    return _objectMapper.Map<Department, DepartmentDto>(partnes);
                }
            );
        }
        public async Task<TaxCategoryDto> GetTaxCategoryByCodeAsync(string code, string orgCode = null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<TaxCategoryDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _taxCategoryService.GetTaxByCodeAsync(code, orgCode);
                    if (partnes == null)
                    {
                        var defaultTaxCategory = await _defaultTaxCategoryService.GetByCodeAsync(code);
                        var dto = new TaxCategoryDto()
                        {
                            Code = defaultTaxCategory.Code,
                            CreditAcc = defaultTaxCategory.CreditAcc,
                            DebitAcc = defaultTaxCategory.DebitAcc,
                            Deduct = defaultTaxCategory.Deduct,
                            Id = defaultTaxCategory.Id,
                            IsDirect = defaultTaxCategory.IsDirect,
                            Name = defaultTaxCategory.Name,
                            OrgCode = orgCode,
                            OutOrIn = defaultTaxCategory.OutOrIn,
                            Percentage = defaultTaxCategory.Percentage,
                            Percetage0 = defaultTaxCategory.Percetage0,
                            TenantId = _currentTenant.Id
                        };
                        return dto;
                    }
                    return _objectMapper.Map<TaxCategory, TaxCategoryDto>(partnes);
                }
            );
        }
        public async Task<FProductWorkDto> GetFProductWorkByCodeAsync(string code, string orgCode = null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<FProductWorkDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _fProductWorkService.GetByFProductWorkAsync(code, orgCode);
                    return _objectMapper.Map<FProductWork, FProductWorkDto>(partnes);
                }
            );
        }
        public async Task<AccPartnerDto> GetPartnerByCodeAsync(string code,string orgCode = null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<AccPartnerDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _partnerService.GetAccPartnerByCodeAsync(code, orgCode);
                    return _objectMapper.Map<AccPartner,AccPartnerDto>(partnes);
                }
            );
        }
        public async Task<List<AccountSystemDto>> GetAccountSystemsAsync(int year,string orgCode=null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }            
            string key = string.Format(CacheKeyManager.AccountSystemByYear, orgCode, year);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<AccountSystemDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _accountSystemService.GetAccounts(orgCode, year);
                    return partnes.Select(p => _objectMapper.Map<AccountSystem,AccountSystemDto>(p)).ToList();
                }
            );
        }
        public async Task<List<DefaultAccountSystemDto>> GetDefaultAccountSystemsAsync(int usingDecision)
        {
            string key = string.Format(CacheKeyManager.DefaultAccountSystemByUsingDecision, usingDecision);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultAccountSystemDto>(key,false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultAccountSystemService.GetListAsync(usingDecision);
                    return partnes.Select(p => _objectMapper.Map<DefaultAccountSystem, DefaultAccountSystemDto>(p)).ToList();
                }
            );
        }
        public async Task<List<LinkCodeDto>> GetLinkCodeAsync(string fieldCode)
        {
            string key = string.Format(CacheKeyManager.ListByCode,fieldCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<LinkCodeDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _linkCodeService.GetListAsync(fieldCode);
                    return partnes.Select(p => _objectMapper.Map<LinkCode, LinkCodeDto>(p))
                        .OrderBy(p => p.Ord).ThenBy(p => p.RefTableName).ToList();
                }
            );
        }
        public async Task<List<CurrencyDto>> GetCurrenciesAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListCurrency, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<CurrencyDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _currencyService.GetListAsync(orgCode);
                    return partnes.Select(p => _objectMapper.Map<Currency, CurrencyDto>(p)).ToList();
                }
            );
        }
        public async Task<CurrencyDto> GetCurrenciesAsync(string code,string orgCode=null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }            
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<CurrencyDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var currency = await _currencyService.GetCurrencyByCodeAsync(code, orgCode);
                    if (currency == null)
                    {
                        var defaultCurrency = await _defaultCurrencyService.GetByCodeAsync(code);
                        var dto = new CurrencyDto()
                        {
                            Code = defaultCurrency.Code,
                            Name = defaultCurrency.Name,
                            NameE = defaultCurrency.NameE,
                            Default = defaultCurrency.Default,
                            ExchangeMethod = defaultCurrency.ExchangeMethod,
                            ExchangeRate = defaultCurrency.ExchangeRate,
                            ExchangeRateDate = defaultCurrency.ExchangeRateDate,
                            OddCurrencyEN = defaultCurrency.OddCurrencyEN,
                            OddCurrencyVN = defaultCurrency.OddCurrencyVN,
                            Id = defaultCurrency.Id,
                            OrgCode = orgCode,
                            TenantId = _currentTenant.Id
                        };
                    }
                    return _objectMapper.Map<Currency, CurrencyDto>(currency);
                }
            );
        }
        public async Task<List<DefaultCurrencyDto>> GetDefaultCurrenciesAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultCurrencyDto>(key,false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultCurrencyService.GetListAsync();
                    return partnes.Select(p => _objectMapper.Map<DefaultCurrency, DefaultCurrencyDto>(p)).ToList();
                }
            );
        }
        public async Task<List<TenantSettingDto>> GetTenantSettingAsync(string orgCode=null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }            
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<TenantSettingDto>(key, true);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _tenantSettingService.GetListAsync(orgCode);
                    return partnes.Select(p => _objectMapper.Map<TenantSetting, TenantSettingDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<List<DefaultTenantSettingDto>> GetDefaultTenantSettingAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultTenantSettingDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultTenantSettingService.GetListAsync();
                    return partnes.Select(p => _objectMapper.Map<DefaultTenantSetting, DefaultTenantSettingDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<List<DefaultBusinessCategoryDto>> GetDefaultBusinessCategoryAsync(int? tenantType)
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultBusinessCategoryDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultBusinessCategoryService.GetListAsync(tenantType);
                    return partnes.Select(p => _objectMapper.Map<DefaultBusinessCategory, DefaultBusinessCategoryDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<List<DefaultExciseTaxDto>> GetDefaultExciseTaxAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultExciseTaxDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultExciseTaxService.GetListAsync();
                    return partnes.Select(p => _objectMapper.Map<DefaultExciseTax, DefaultExciseTaxDto>(p)).ToList();
                }
            );
        }
        public async Task<List<DefaultTaxCategoryDto>> GetDefaultTaxCategoryAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultTaxCategoryDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultTaxCategoryService.GetListAsync();
                    return partnes.Select(p => _objectMapper.Map<DefaultTaxCategory, DefaultTaxCategoryDto>(p)).ToList();
                }
            );
        }
        public async Task<List<VoucherCategoryDto>> GetVoucherCategoryAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<VoucherCategoryDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _voucherCategoryService.GetRepository().GetListAsync();
                    if (partnes.Count == 0)
                    {
                        var defaultVoucherType = await _defaultVoucherCategoryService.GetListAsync();
                        return defaultVoucherType.Select(p => _objectMapper.Map<DefaultVoucherCategory, VoucherCategoryDto>(p))
                                    .ToList();
                    }
                    return partnes.Select(p => _objectMapper.Map<VoucherCategory, VoucherCategoryDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<VoucherCategoryDto> GetVoucherCategoryAsync(string code,string orgCode=null)
        {
            if (string.IsNullOrEmpty(orgCode))
            {
                orgCode = _webHelper.GetCurrentOrgUnit();
            }
            string key = string.Format(CacheKeyManager.CodeWithOrgCode, orgCode, code);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<VoucherCategoryDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var voucherCategory = await _voucherCategoryService.GetByCodeAsync(code,orgCode);
                    if (voucherCategory == null)
                    {
                        var defaultVoucherCategory = await _defaultVoucherCategoryService.GetByCodeAsync(code);
                        var dto = _objectMapper.Map<DefaultVoucherCategory, VoucherCategoryDto>(defaultVoucherCategory);
                        dto.OrgCode = orgCode;
                        dto.TenantId = _currentTenant.Id;
                        return dto;
                    }
                    return _objectMapper.Map<VoucherCategory, VoucherCategoryDto>(voucherCategory);
                }
            );
        }
        public async Task<List<DefaultVoucherCategoryDto>> GetDefaultVoucherCategoryAsync()
        {
            string key = CacheKeyManager.ListDefault;
            int? tenantType = await _tenantExtendInfoService.GetTenantType(_currentTenant.Id);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultVoucherCategoryDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultVoucherCategoryService.GetListByTenantTypeAsync(tenantType);
                    return partnes.Select(p => _objectMapper.Map<DefaultVoucherCategory, DefaultVoucherCategoryDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<YearCategoryDto> GetYearCategoryByYearAsync(int year)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.YearCategoryByYear, orgCode, year);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<YearCategory>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _yearCategoryService.GetByYearAsync(orgCode,year);
                    return _objectMapper.Map<YearCategory,YearCategoryDto>(partnes);
                }
            );
        }
        public async Task<List<AssetToolGroupDto>> GetAssetToolGroupAsync(string assetOrTool)
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListAssetToolGroup, orgCode, assetOrTool);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<AssetToolGroupDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _assetToolGroupService.GetByAssetToolAsync(assetOrTool,orgCode);
                    return partnes.Select(p => _objectMapper.Map<AssetToolGroup, AssetToolGroupDto>(p)).ToList();
                }
            );
        }
        public async Task<List<DefaultAssetToolGroupDto>> GetDefaultAssetToolGroupAsync(string assetOrTool)
        {
            string key = string.Format(CacheKeyManager.ListDefaultAssetToolGroup,assetOrTool);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultAssetToolGroupDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultAssetToolGroupService.GetByAssetOrToolAsync(assetOrTool);
                    return partnes.Select(p => _objectMapper.Map<DefaultAssetToolGroup, DefaultAssetToolGroupDto>(p)).ToList();
                }
            );
        }
        public async Task<List<PartnerGroupDto>> GetPartnerGroupAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<PartnerGroupDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _partnerGroupService.GetListAsync(orgCode);
                    return partnes.Select(p => _objectMapper.Map<PartnerGroup, PartnerGroupDto>(p)).ToList();
                }
            );
        }
        public async Task<List<CapitalDto>> GetCapitalsAsync()
        {            
            string orgCode = _webHelper.GetCurrentOrgUnit();            
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<CapitalDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var capitals = await _capitalService.GetListAsync(orgCode);
                    if (capitals.Count == 0)
                    {
                        var defaultCapitals = await _defaultCapitalService.GetListAsync();
                        return defaultCapitals.Select(p =>
                        {
                            var dto = _objectMapper.Map<DefaultCapital, CapitalDto>(p);
                            dto.OrgCode = orgCode;
                            dto.TenantId = _currentTenant.Id;
                            return dto;
                        }).ToList();
                    }
                    return capitals.Select(p => _objectMapper.Map<Capital, CapitalDto>(p)).ToList();
                }
            );
        }
        public async Task<List<DefaultCapitalDto>> GetDefaultCapitalAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultCapitalDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultCapitalService.GetListAsync();
                    return partnes.Select(p => _objectMapper.Map<DefaultCapital, DefaultCapitalDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<List<ReasonDto>> GetReasonsAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string key = string.Format(CacheKeyManager.ListByOrgCode, orgCode);
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<ReasonDto>(key);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var reasons = await _reasonService.GetListAsync(orgCode);
                    if (reasons.Count == 0)
                    {
                        var defaultReasons = await _defaultReasonService.GetListAsync();
                        return defaultReasons.Select(p =>
                        {
                            var dto = _objectMapper.Map<DefaultReason, ReasonDto>(p);
                            dto.OrgCode = orgCode;
                            dto.TenantId = _currentTenant.Id;
                            return dto;
                        }).ToList();
                    }
                    return reasons.Select(p => _objectMapper.Map<Reason, ReasonDto>(p)).ToList();
                }
            );
        }
        public async Task<List<DefaultReasonDto>> GetDefaultReasonAsync()
        {
            string key = CacheKeyManager.ListDefault;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<DefaultReasonDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var partnes = await _defaultReasonService.GetListAsync();
                    return partnes.Select(p => _objectMapper.Map<DefaultReason, DefaultReasonDto>(p))
                                .ToList();
                }
            );
        }
        public async Task<WindowDto> GetWindowAsync(string windowId)
        {
            string key = windowId;
            string cacheKey = _cacheManager.GetCacheKeyWithPrefixClass<WindowDto>(key, false);
            return await _cacheManager.GetOrAddAsync(
                cacheKey,
                async () =>
                {
                    var window = await _windowService.GetAsync(windowId);
                    return _objectMapper.Map<Window, WindowDto>(window);
                }
            );
        }
        public async Task RemoveClassCache<T>()
        {
            await _cacheManager.RemoveClassCache<T>();
        }
        #endregion
    }
}
