using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;

namespace Accounting.EntityFrameworkCore
{
    public class TenancyDbContextFactory : IDesignTimeDbContextFactory<TenancyDbContext>
    {
        public TenancyDbContext CreateDbContext(string[] args)
        {
            // https://www.npgsql.org/efcore/release-notes/6.0.html#opting-out-of-the-new-timestamp-mapping-logic
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            AccountingEfCoreEntityExtensionMappings.Configure();
            string connStr = GetDbConnectionString(args);            

            var builder = new DbContextOptionsBuilder<TenancyDbContext>()
                .UseNpgsql(connStr);

            return new TenancyDbContext(builder.Options);
        }
        private static string GetDbConnectionString(string[] args)
        {
            if (args != null)
            {
                string key = "ConnectionString=";
                var item = args.Where(c => c.StartsWith(key)).FirstOrDefault<string>();
                if (item != null)
                {
                    return item.Replace(key, "").Replace("\"", "");
                }
            }

            var configuration = BuildConfiguration();
            string connStr = configuration.GetConnectionString("TenantDb");
            return connStr;
        }
        private static IConfigurationRoot BuildConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../BookStore.DbMigrator/"))
                .AddJsonFile("appsettings.json", optional: false);

            return builder.Build();
        }
    }
}
