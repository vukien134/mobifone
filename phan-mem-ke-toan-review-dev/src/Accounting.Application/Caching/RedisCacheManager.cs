using Accounting.Caching.Redis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Accounting.Caching
{
    public class RedisCacheManager : ISingletonDependency
    {
        #region Fields
        private readonly RedisConnectionWrapper _redisConnectionWrapper;        
        #endregion
        #region Ctor
        public RedisCacheManager(RedisConnectionWrapper redisConnectionWrapper)
        {
            _redisConnectionWrapper = redisConnectionWrapper;            
        }
        #endregion
        #region Methods
        public async Task RemoveByPatternAsync(string pattern)
        {
            var db = await _redisConnectionWrapper.GetDatabaseAsync();
            var endPoints = await _redisConnectionWrapper.GetEndpoints();

            foreach (var ep in endPoints)
            {
                var server = await _redisConnectionWrapper.Server(ep);
                //var keys = server.Keys(pattern: "*" + pattern + "*");
                var keys = server.Keys(pattern: pattern).ToArray();
                await db.KeyDeleteAsync(keys);
            }
        }
        public async Task RemoveAsync(string key)
        {
            var db = await _redisConnectionWrapper.GetDatabaseAsync();
            await db.KeyDeleteAsync(key);            
        }
        public async Task<T> GetOrAddAsync<T>(string key, Func<Task<T>> factory, int cacheTime = 60)
        {
            var value = await GetAsync<T>(key);
            if (value != null)
            {
                return value;
            }

            value = await factory();
            await SetAsync(key, value, cacheTime);

            return value;

        }
        #endregion
        #region Privates
        private async Task<T> GetAsync<T>(string key)
        {
            var db = await _redisConnectionWrapper.GetDatabaseAsync();
            var rValue = await db.StringGetAsync(key);
            if (!rValue.HasValue)
                return default(T);
            var result = Deserialize<T>(rValue);            
            return result;
        }        

        private async Task SetAsync(string key, object data, int cacheTime)
        {
            if (data == null)
                return;

            var entryBytes = Serialize(data);
            var expiresIn = TimeSpan.FromMinutes(cacheTime);
            var db = await _redisConnectionWrapper.GetDatabaseAsync();
            await db.StringSetAsync(key, entryBytes, expiresIn);
        }
        private byte[] Serialize(object item)
        {
            var jsonString = JsonSerializer.Serialize(item);
            return Encoding.UTF8.GetBytes(jsonString);
        }
        private T Deserialize<T>(byte[] serializedObject)
        {
            if (serializedObject == null)
                return default(T);

            var jsonString = Encoding.UTF8.GetString(serializedObject);
            return JsonSerializer.Deserialize<T>(jsonString);
        }
        #endregion
    }
}
