using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;

namespace Accounting.DomainServices.Categories
{
    public class TenantSettingService : BaseDomainService<TenantSetting, string>
    {
        #region Fields
        private readonly IObjectMapper _objectMapper;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        #endregion
        public TenantSettingService(IRepository<TenantSetting, string> repository,
                                    IObjectMapper objectMapper,
                                    DefaultTenantSettingService defaultTenantSettingService)

            : base(repository)
        {
            _objectMapper = objectMapper;
            _defaultTenantSettingService = defaultTenantSettingService;
        }
        public async Task<bool> IsExistCode(TenantSetting entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Key == entity.Key
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(TenantSetting entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.TenantSetting, ErrorCode.Duplicate),
                        $"TenantSetting Key ['{entity.Key}'] already exist ");
            }
        }

        public async Task<string> GetValue(string key, string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Key == key && p.OrgCode == orgCode);
            var sections = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (sections != null)
            {
                return sections.Value.ToString();
            }
            else 
            {
                var queryableDF = await _defaultTenantSettingService.GetQueryableAsync();
                queryableDF = queryableDF.Where(p => p.Key == key);
                var sectionDF = await AsyncExecuter.FirstOrDefaultAsync(queryableDF);
                if (sectionDF != null)
                {
                    return sectionDF.Value.ToString();
                }
                else
                {
                    return "";
                }
            }
        }
        public async Task<TenantSetting> GetTenantSettingByKeyAsync(string key, string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Key == key && p.OrgCode == orgCode);
            var tenantSetting = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (tenantSetting != null)
            {
                return tenantSetting;
            }
            else
            {
                var dFQueryable = await _defaultTenantSettingService.GetByKeyAsync(key);
                return _objectMapper.Map<DefaultTenantSetting, TenantSetting>(dFQueryable);
            }
        }
        public async Task<List<TenantSetting>> GetListFormatNumberFields(string orgCode) 
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                                && CurrencyConst.ListFormatNumberFields.Contains(p.Key));

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<Dictionary<string,string>> GetCurrencyFormats(string orgCode)
        {
            var currencyFormats = await this.GetListFormatNumberFields(orgCode);
            var dict = new Dictionary<string, string>();
            foreach (var item in currencyFormats)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        public async Task<List<TenantSetting>> GetNumberSeparateSymbol(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == orgCode
                            && (p.Key == CurrencyConst.SymbolSeparateGroupDigit || p.Key == CurrencyConst.SymbolSeparateDecimal));

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<string> CheckRemoveDuplicate(string orgCode, string voucherCode, string creditAcc, string debitAcc)
        {
            var accRemoveDuplicateCash = await this.GetValue("VHT_TK_KHU_TRUNG_TM", orgCode);
            if (string.IsNullOrEmpty(accRemoveDuplicateCash))
            {
                accRemoveDuplicateCash = await _defaultTenantSettingService.GetValue("VHT_TK_KHU_TRUNG_TM");
            }
            var accRemoveDuplicateDeposit = await this.GetValue("VHT_TK_KHU_TRUNG_TG", orgCode);
            if (string.IsNullOrEmpty(accRemoveDuplicateDeposit))
            {
                accRemoveDuplicateDeposit = await _defaultTenantSettingService.GetValue("VHT_TK_KHU_TRUNG_TG");
            }
            var RemoveDuplicateCashDeposit = await this.GetValue("VHT_KHU_TRUNG_TM_TG", orgCode);
            if (string.IsNullOrEmpty(RemoveDuplicateCashDeposit))
            {
                RemoveDuplicateCashDeposit = await _defaultTenantSettingService.GetValue("VHT_KHU_TRUNG_TM_TG");
            }
            if (voucherCode.Substring(0, 2) == "PT" && creditAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "PT" && creditAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3) && "13".Contains(RemoveDuplicateCashDeposit)
                || voucherCode.Substring(0, 2) == "BC" && creditAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "BC" && creditAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3) && "23".Contains(RemoveDuplicateCashDeposit)
                        )
            {
                return "N";
            }
            else if (
                   voucherCode.Substring(0, 2) == "PC" && debitAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "PC" && debitAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3) && "13".Contains(RemoveDuplicateCashDeposit)
                || voucherCode.Substring(0, 2) == "BN" && debitAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "BN" && debitAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3) && "23".Contains(RemoveDuplicateCashDeposit)
                )
            {
                return "C";
            }
            else
            {
                return "";
            }
        }
        public async Task<string> CheckRemoveDuplicateAccVoucher(string orgCode, string voucherCode, string creditAcc, string debitAcc)
        {
            var accRemoveDuplicateCash = await this.GetValue("VHT_TK_KHU_TRUNG_TM", orgCode);
            if (string.IsNullOrEmpty(accRemoveDuplicateCash))
            {
                accRemoveDuplicateCash = await _defaultTenantSettingService.GetValue("VHT_TK_KHU_TRUNG_TM");
            }
            var accRemoveDuplicateDeposit = await this.GetValue("VHT_TK_KHU_TRUNG_TG", orgCode);
            if (string.IsNullOrEmpty(accRemoveDuplicateDeposit))
            {
                accRemoveDuplicateDeposit = await _defaultTenantSettingService.GetValue("VHT_TK_KHU_TRUNG_TG");
            }
            var RemoveDuplicateCashDeposit = await this.GetValue("VHT_KHU_TRUNG_TM_TG", orgCode);
            if (string.IsNullOrEmpty(RemoveDuplicateCashDeposit))
            {
                RemoveDuplicateCashDeposit = await _defaultTenantSettingService.GetValue("VHT_KHU_TRUNG_TM_TG");
            }
            if (voucherCode.Substring(0, 2) == "PT" && creditAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "PT" && creditAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3) && "13".Contains(RemoveDuplicateCashDeposit)
                || voucherCode.Substring(0, 3) == "GBC" && creditAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3)
                || voucherCode.Substring(0, 3) == "GBC" && creditAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3) && "23".Contains(RemoveDuplicateCashDeposit)
                        )
            {
                return "N";
            }
            else if (
                   voucherCode.Substring(0, 2) == "PC" && debitAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "PC" && debitAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3) && "13".Contains(RemoveDuplicateCashDeposit)
                || voucherCode.Substring(0, 3) == "GBN" && debitAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3)
                || voucherCode.Substring(0, 3) == "GBN" && debitAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3) && "23".Contains(RemoveDuplicateCashDeposit)
                )
            {
                return "C";
            }
            else
            {
                return "";
            }
        }
        public async Task<List<TenantSetting>> GetBySettingTypeAsync(string orgCode,string settingType)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode)
                        && p.SettingType.Equals(settingType));
            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<bool> IsExistListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode.Equals(orgCode));
        }
        public async Task<List<TenantSetting>> GetListAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode.Equals(orgCode));
            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
