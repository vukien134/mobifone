using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Accounting.Data;
using Volo.Abp.DependencyInjection;

namespace Accounting.EntityFrameworkCore;

public class EntityFrameworkCoreAccountingDbSchemaMigrator
    : IAccountingDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreAccountingDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the AccountingDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<AccountingDbContext>()
            .Database
            .MigrateAsync();
    }

    public async Task MigrateAsync(string connStr)
    {
        var dbContext = _serviceProvider.GetRequiredService<TenancyDbContext>();
        dbContext.Database.SetConnectionString(connStr);

        await dbContext.Database.MigrateAsync();
    }
}
