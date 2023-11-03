using Accounting.BaseDtos;
using Accounting.Caching;
using Accounting.Common;
using Accounting.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Caching;
using Volo.Abp.DependencyInjection;
using Volo.Abp.MultiTenancy;

namespace Accounting.Caching
{
    [Dependency(ServiceLifetime.Transient)]
    public class CacheManager
    {
        #region Fields        
        private readonly RedisCacheManager _redisCacheManager;
        private readonly ICurrentTenant _currentTenant;
        private readonly WebHelper _webHelper;
        #endregion
        #region Ctor
        public CacheManager(RedisCacheManager redisCacheManager,
                    ICurrentTenant currentTenant,
                    WebHelper webHelper
            )
        {
            _redisCacheManager = redisCacheManager;
            _currentTenant = currentTenant;
            _webHelper = webHelper;
        }
        #endregion
        #region Methods
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, bool includeTenant = true, int cacheTime = 60)
        {
            return await _redisCacheManager.GetOrAddAsync<T>(key, factory, cacheTime);
        }
        public async Task RemoveByPatternAsync(string key)
        {
            await _redisCacheManager.RemoveByPatternAsync(key);
        }
        public async Task RemoveAsync(string key)
        {
            await _redisCacheManager.RemoveAsync(key);
        }
        public async Task RemoveClassCache<T>(bool includeTenantId = true)
        {
            string cacheKey = GetCacheKeyPrefix<T>(includeTenantId);
            await _redisCacheManager.RemoveByPatternAsync($"*{cacheKey}*");
        }
        public string GetCacheKeyByPageRequest(PageRequestDto dto)
        {
            string key = $"{dto.Start}{dto.Count}{dto.QuickSearch}";
            if (dto.FilterRows != null)
            {
                foreach(var item in dto.FilterRows)
                {
                    key = key + $"{item.ColumnName}{item.ColumnType}{item.Value}";
                }
            }
            if (dto.FilterAdvanced != null)
            {
                foreach (var item in dto.FilterAdvanced)
                {
                    key = key + $"{item.ColumnName}{item.ColumnType}{item.Value}";
                }
            }

            string hashKey = Util.MD5Hash(key);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string cacheKey = $"Pages_{orgCode}_{hashKey}";

            return cacheKey;
        }
        public string GetCacheKeyByPageRequest<T>(PageRequestDto dto, bool includeTenant = true)
        {
            string key = $"{dto.Start}{dto.Count}{dto.QuickSearch}";
            if (dto.FilterRows != null)
            {
                foreach (var item in dto.FilterRows)
                {
                    key = key + $"{item.ColumnName}{item.ColumnType}{item.Value}";
                }
            }
            if (dto.FilterAdvanced != null)
            {
                foreach (var item in dto.FilterAdvanced)
                {
                    key = key + $"{item.ColumnName}{item.ColumnType}{item.Value}";
                }
            }

            string prefix = this.GetCacheKeyPrefix<T>(includeTenant);
            string hashKey = Util.MD5Hash(key);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string cacheKey = $"Pages_{orgCode}_{hashKey}";
            cacheKey = prefix + $"k:{cacheKey}" ;

            return cacheKey;
        }
        public string GetCacheKeyByFilterValue<T>(string filterValue,bool includeTenant = true)
        {
            string prefix = this.GetCacheKeyPrefix<T>(includeTenant);
            string hashKey = Util.MD5Hash(filterValue);
            string orgCode = _webHelper.GetCurrentOrgUnit();
            string cacheKey = prefix + $"k:FilterValue_{orgCode}_{hashKey}";
            return cacheKey;
        }
        public string GetCacheKeyWithPrefixClass<T>(string key, bool includeTenant = true)
        {
            string prefix = this.GetCacheKeyPrefix<T>(includeTenant);
            string cacheKey = prefix + $"k:{key}";
            return cacheKey;
        }
        #endregion
        #region Privates
        private string GetCacheKeyPrefix<T>(bool includeTenantId)
        {
            string fullName = typeof(T).FullName;

            string tenantPrefix = includeTenantId == true ? $"t:{_currentTenant.Id}," : "";
            string classPrefix = $"c:{fullName},";

            return tenantPrefix + classPrefix;
        }
        #endregion
    }
}
