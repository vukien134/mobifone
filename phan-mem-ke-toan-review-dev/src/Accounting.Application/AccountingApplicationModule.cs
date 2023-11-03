using Volo.Abp.Account;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement;
using Volo.Abp.SettingManagement;
using Volo.Abp.TenantManagement;
using Volo.Abp.BackgroundJobs.RabbitMQ;
using Volo.Abp.Caching.StackExchangeRedis;
using Volo.Abp.Emailing;
using Volo.Abp.DistributedLocking;
using Microsoft.Extensions.DependencyInjection;
using Medallion.Threading;
using StackExchange.Redis;
using Medallion.Threading.Redis;

namespace Accounting;

[DependsOn(
    typeof(AccountingDomainModule),
    typeof(AbpAccountApplicationModule),
    typeof(AccountingApplicationContractsModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpTenantManagementApplicationModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpSettingManagementApplicationModule)
    )]
[DependsOn(typeof(AbpBackgroundJobsRabbitMqModule))]
    [DependsOn(typeof(AbpCachingStackExchangeRedisModule))]
    [DependsOn(typeof(AbpEmailingModule))]
    [DependsOn(typeof(AbpDistributedLockingModule))]
    public class AccountingApplicationModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var configuration = context.Services.GetConfiguration();
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AccountingApplicationModule>();
        });
        context.Services.AddSingleton<IDistributedLockProvider>(sp =>
        {
            var connection = ConnectionMultiplexer
                .Connect(configuration["Redis:Configuration"]);
            return new RedisDistributedSynchronizationProvider(connection.GetDatabase());
        });
    }
}
