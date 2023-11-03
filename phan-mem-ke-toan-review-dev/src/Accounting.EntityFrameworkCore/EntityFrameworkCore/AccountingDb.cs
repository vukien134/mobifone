using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;

namespace Accounting.EntityFrameworkCore
{
    public class AccountingDb : ITransientDependency
    {
        private readonly IDbContextProvider<AccountingDbContext> _dbContextProvider;
        public AccountingDb(IDbContextProvider<AccountingDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }
        public async Task ExecuteSQLAsync(string sql, params object[] parameters)
        {
            var context = await _dbContextProvider.GetDbContextAsync();
            await context.Database.ExecuteSqlRawAsync(sql, parameters);
        }
        public async Task ExecuteSQLAsync(string sql, Dictionary<string,object> dict)
        {
            var context = await _dbContextProvider.GetDbContextAsync();
            var conn = context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sql;
                if (dict != null)
                {
                    foreach (var item in dict)
                    {
                        var commandParameter = command.CreateParameter();
                        commandParameter.Value = item.Value;
                        commandParameter.ParameterName = item.Key;
                        command.Parameters.Add(commandParameter);
                    }
                }

                await command.ExecuteNonQueryAsync();
            }
        }
        public async Task<DataTable> GetDataTableAsync(string sql, params object[] parameters)
        {
            var dt = new DataTable();
            var context = await _dbContextProvider.GetDbContextAsync();
            var conn = context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sql;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var commandParameter = command.CreateParameter();
                        commandParameter.Value = parameters[i];
                        commandParameter.ParameterName = $"P_{i}";
                        command.Parameters.Add(commandParameter);
                    }
                }

                var reader = await command.ExecuteReaderAsync();
                dt.Load(reader);
            }
            return dt;
        }
        public async Task<DataTable> GetDataTableAsync(string sql, Dictionary<string, object> dict)
        {
            var dt = new DataTable();
            var context = await _dbContextProvider.GetDbContextAsync();
            var conn = context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sql;
                if (dict != null)
                {
                    foreach(var item in dict)
                    {
                        var commandParameter = command.CreateParameter();
                        commandParameter.Value = item.Value;
                        commandParameter.ParameterName = item.Key;
                        command.Parameters.Add(commandParameter);
                    }                    
                }

                var reader = await command.ExecuteReaderAsync();
                dt.Load(reader);
            }
            return dt;
        }

        public async Task<string> GetDataAsync(string sql, params object[] parameters)
        {
            var dt = new DataTable();
            string resul = "";
            var context = await _dbContextProvider.GetDbContextAsync();
            var conn = context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sql;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var commandParameter = command.CreateParameter();
                        commandParameter.Value = parameters[i];
                        commandParameter.ParameterName = $"P_{i}";
                        command.Parameters.Add(commandParameter);
                    }
                }

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    resul = (string)reader["id"];
                }
                dt.Load(reader);


            }
            return resul;
        }
        public async Task<string> GetDataNameAsync(string sql, params object[] parameters)
        {
            var dt = new DataTable();
            string resul = "";
            var context = await _dbContextProvider.GetDbContextAsync();
            var conn = context.Database.GetDbConnection();
            using (var command = conn.CreateCommand())
            {
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sql;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        var commandParameter = command.CreateParameter();
                        commandParameter.Value = parameters[i];
                        commandParameter.ParameterName = $"P_{i}";
                        command.Parameters.Add(commandParameter);
                    }
                }

                var reader = await command.ExecuteReaderAsync();
                while (reader.Read())
                {
                    resul = (string)reader["Name"];
                }
                dt.Load(reader);
            }
            return resul;
        }
        public async Task<string> GetDatabaseNameAsync()
        {
            var context = await _dbContextProvider.GetDbContextAsync();
            return context.Database.GetDbConnection().Database;
        }
    }
}
