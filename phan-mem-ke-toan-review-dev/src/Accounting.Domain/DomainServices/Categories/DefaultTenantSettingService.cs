using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Constants;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class DefaultTenantSettingService : BaseDomainService<DefaultTenantSetting, string>
    {

        public DefaultTenantSettingService(IRepository<DefaultTenantSetting, string> repository) 

            : base(repository)
        {
        }
        
        public async Task<List<DefaultTenantSetting>> GetListFormatNumberFields()
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => CurrencyConst.ListFormatNumberFields.Contains(p.Key));

            return await AsyncExecuter.ToListAsync(queryable);
        }
        public async Task<Dictionary<string,string>> GetCurrencyFormats()
        {
            var currencyFormats = await this.GetListFormatNumberFields();
            var dict = new Dictionary<string, string>();
            foreach (var item in currencyFormats)
            {
                if (dict.ContainsKey(item.Key)) continue;
                dict.Add(item.Key, item.Value);
            }
            return dict;
        }
        public async Task<List<DefaultTenantSetting>> GetNumberSeparateSymbol()
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Key == CurrencyConst.SymbolSeparateGroupDigit || p.Key == CurrencyConst.SymbolSeparateDecimal);

            return await AsyncExecuter.ToListAsync(queryable);
        }

        public async Task<DefaultTenantSetting> GetByKeyAsync(string key)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Key.Equals(key));
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<string> GetValue(string key)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Key == key);
            var sections = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (sections != null)
            {
                return sections.Value.ToString();
            }
            else return "";
        }
        public async Task<List<DefaultTenantSetting>> GetListAsync()
        {
            return await this.GetRepository().GetListAsync();
        }
    }
}
