using Microsoft.Extensions.Configuration;
using Nito.AsyncEx;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Accounting.Caching.Redis
{
    public class RedisConnectionWrapper : ISingletonDependency
    {
        #region Fields
        private readonly IConfiguration _config;
        private ConnectionMultiplexer _connection;
        protected readonly AsyncLock _locker;
        #endregion
        #region Ctor
        public RedisConnectionWrapper(IConfiguration config)
        {
            _config = config;
            _locker = new AsyncLock();
        }
        #endregion
        #region Method
        public async Task<IDatabaseAsync> GetDatabaseAsync()
        {
            var conn = await this.GetConnection();
            return conn.GetDatabase();
        }
        public async Task<EndPoint[]> GetEndpoints()
        {
            var conn = await this.GetConnection();
            return conn.GetEndPoints();
        }
        public async Task<IServer> Server(EndPoint endPoint)
        {
            var conn = await this.GetConnection();
            return conn.GetServer(endPoint);
        }
        #endregion
        #region Private
        private async Task<ConnectionMultiplexer> GetConnection()
        {
            if (_connection != null)
            {
                if (_connection.IsConnected) return _connection;
            }
            using var _ = await _locker.LockAsync();
            if (_connection != null)
            {
                //Connection disconnected. Disposing connection...
                _connection.Dispose();
            }

            //Creating new instance of Redis Connection
            _connection = await ConnectionMultiplexer.ConnectAsync(this.GetConnectionString());
            return _connection;
        }
        private string GetConnectionString()
        {
            string connStr = _config.GetValue<string>("Redis:Configuration");
            return connStr;
        }
        #endregion
    }
}
