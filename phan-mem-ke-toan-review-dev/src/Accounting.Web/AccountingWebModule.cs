using System;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Accounting.EntityFrameworkCore;
using Accounting.Localization;
using Accounting.MultiTenancy;
using Accounting.Web.Menus;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Authentication.JwtBearer;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.Localization;
using Volo.Abp.AspNetCore.Mvc.UI;
using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap;
using Volo.Abp.AspNetCore.Mvc.UI.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic.Bundling;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.Autofac;
using Volo.Abp.AutoMapper;
using Volo.Abp.FeatureManagement;
using Volo.Abp.Identity.Web;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.PermissionManagement.Web;
using Volo.Abp.SettingManagement.Web;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement.Web;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.UI;
using Volo.Abp.UI.Navigation;
using Volo.Abp.VirtualFileSystem;
using Volo.Abp.AspNetCore.ExceptionHandling;
using System.Linq;
using Microsoft.AspNetCore.Cors;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using System.Collections.Generic;
using Volo.Abp.AspNetCore.MultiTenancy;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Volo.Abp.Uow;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;

namespace Accounting.Web;

[DependsOn(
    typeof(AccountingHttpApiModule),
    typeof(AccountingApplicationModule),
    typeof(AccountingEntityFrameworkCoreModule),
    typeof(AbpAutofacModule),
    typeof(AbpIdentityWebModule),
    typeof(AbpSettingManagementWebModule),
    typeof(AbpAccountWebIdentityServerModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAspNetCoreAuthenticationJwtBearerModule),
    typeof(AbpTenantManagementWebModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpSwashbuckleModule)
    )]
public class AccountingWebModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        context.Services.PreConfigure<AbpMvcDataAnnotationsLocalizationOptions>(options =>
        {
            options.AddAssemblyResource(
                typeof(AccountingResource),
                typeof(AccountingDomainModule).Assembly,
                typeof(AccountingDomainSharedModule).Assembly,
                typeof(AccountingApplicationModule).Assembly,
                typeof(AccountingApplicationContractsModule).Assembly,
                typeof(AccountingWebModule).Assembly
            );
        });

        //PreConfigure<IIdentityServerBuilder>(builder =>
        //{
        //    builder.AddSigningCredential(GetCertSign());
        //    builder.AddValidationKey(GetCertSign());
        //});
    }

    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();
        //context.Services.AddDataProtection((option) =>
        //{
        //    option.ApplicationDiscriminator = "AccountingApp";            
        //});

        ConfigureUrls(configuration);
        ConfigureBundles();
        ConfigureAuthentication(context, configuration);
        ConfigureAutoMapper();
        ConfigureVirtualFileSystem(hostingEnvironment);
        ConfigureLocalizationServices();
        ConfigureNavigationServices();
        ConfigureAutoApiControllers();
        ConfigureSwaggerServices(context.Services);
        ConfigureCors(context, configuration);
        Configure<AbpExceptionHandlingOptions>(options =>
        {
            options.SendExceptionsDetailsToClients = true;
            options.SendStackTraceToClients = true;
        });
        //Configure<AbpUnitOfWorkDefaultOptions>(options =>
        //{
        //    options.TransactionBehavior = UnitOfWorkTransactionBehavior.Disabled;
        //});
    }
    private void ConfigureCors(ServiceConfigurationContext context, IConfiguration configuration)
    {
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();

            });
        });
    }
    private void ConfigureUrls(IConfiguration configuration)
    {
        Configure<AppUrlOptions>(options =>
        {
            options.Applications["MVC"].RootUrl = configuration["App:SelfUrl"];
        });
    }

    private void ConfigureBundles()
    {
        Configure<AbpBundlingOptions>(options =>
        {
            options.StyleBundles.Configure(
                BasicThemeBundles.Styles.Global,
                bundle =>
                {
                    bundle.AddFiles("/global-styles.css");
                }
            );
        });
    }

    private void ConfigureAuthentication(ServiceConfigurationContext context, IConfiguration configuration)
    {
               
        context.Services.AddAuthentication()            
            .AddJwtBearer(options =>
            {
                options.Authority = configuration["AuthServer:Authority"];
                options.RequireHttpsMetadata = Convert.ToBoolean(configuration["AuthServer:RequireHttpsMetadata"]);
                options.Audience = "Accounting";                
                //options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.ValidateIssuer = false;
            });
        
        context.Services.ForwardIdentityAuthenticationForBearer();
    }

    private void ConfigureAutoMapper()
    {
        Configure<AbpAutoMapperOptions>(options =>
        {
            options.AddMaps<AccountingWebModule>();
        });
    }

    private void ConfigureVirtualFileSystem(IWebHostEnvironment hostingEnvironment)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                    options.FileSets.ReplaceEmbeddedByPhysical<AccountingDomainSharedModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Accounting.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<AccountingDomainModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Accounting.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<AccountingApplicationContractsModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Accounting.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<AccountingApplicationModule>(Path.Combine(hostingEnvironment.ContentRootPath, $"..{Path.DirectorySeparatorChar}Accounting.Application"));
                options.FileSets.ReplaceEmbeddedByPhysical<AccountingWebModule>(hostingEnvironment.ContentRootPath);
            });
        }
    }

    private void ConfigureLocalizationServices()
    {
        Configure<AbpLocalizationOptions>(options =>
        {            
            options.Languages.Add(new LanguageInfo("en", "en", "English"));            
            options.Languages.Add(new LanguageInfo("vi", "vi", "Việt Nam"));
        });
    }

    private void ConfigureNavigationServices()
    {
        Configure<AbpNavigationOptions>(options =>
        {
            options.MenuContributors.Add(new AccountingMenuContributor());
        });
    }

    private void ConfigureAutoApiControllers()
    {
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers.Create(typeof(AccountingApplicationModule).Assembly);
        });
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddAbpSwaggerGen(
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo { Title = "Accounting API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
            }
        );
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        var supportedCultures = new[]
        {
            new CultureInfo("vi"),
            new CultureInfo("en"),
        };
        app.UseAbpRequestLocalization(options =>
        {
            options.DefaultRequestCulture = new RequestCulture("vi");
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new List<IRequestCultureProvider>
            {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider(),
                new AcceptLanguageHeaderRequestCultureProvider()
            };
        });        

        app.UseCorrelationId();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseJwtTokenMiddleware();

        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }

        app.UseUnitOfWork();
        app.UseIdentityServer();
        
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Accounting API");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
    }

    private X509Certificate2 GetCertSign()
    {
        string base64String = "MIIJ7wIBAzCCCasGCSqGSIb3DQEHAaCCCZwEggmYMIIJlDCCBgsGCSqGSIb3DQEHAaCCBfwEggX4MIIF9DCCBfAGCyqGSIb3DQEMCgECoIIE / jCCBPowHAYKKoZIhvcNAQwBAzAOBAhnV9GbOFkcKAICB9AEggTY9XbzTWT4QqRkUDuCm2jsfLrGUKM0GZSHT3USXmEEwFMVStfsZWMDxH8ahe5MlN14niVCHKX7lXYGUxDimwBVSeTUqOH5BJ1AMIQ9nGex + 9yMvP0W1OeUKKCWETUixu1dKz3mrzSouVvL9eyYL8hmtmDh4IqsCi6ec6Z7UfEWrM4afjhOFeAfyzd71D9nLfj6FTbBRs0cIQr6vL5LXyiYvm2SuiZgFvfEbOeq9R9hZ / s0owx88xO5s1zTVm / r0YCwAhpfPmWysowvrJdzV8aCgNbfjasStkfcrP42H20ECFh5ExkLfQqM05QO4l8 + EqacTbz2LxPetV9HsCYPQIg7HnOfHN6ZQliOyYWqIbOORbDPYe5sPWQd3yprqHL046Z3uaIVtma8 / obrN9xy2AhF1ViCWiUwaMUcYyVIwN8KTsGN4jg6bEjrK8zH6K1nGR / NXkMG7Kbx07wUOOojCb2umJAxWM2Ljk + G79lPBG4zoCvmWQqu99E1TlC1dqnHRUi5PWwLhR1YapHOkuJZwJbu / zLBsoMCYDS3KbtAL5ePHIkuzUtrT5yj9RHMxi07NwiA3zChS2lTYZrDalFx4YzNI9Ggnk + 0hsEWMZJCid8omytl9L7tzId06Gim5RS8t9str1akvmL60ii1ze3DTDB8YS1caBtH1vqwEdP0Rt44rfK1GC / La4qQmN1ucoWCOrqDmghI53T3WnuFvny + 3LtsTDne8 + 4yTRAjRiOXZ0YDv8qlFjurhgL2DDLa0anw9nht1VI1aIhLSEOX5tgmQbvUGH68AjGb19rByMKmKqps8EweU30mnmZmy0D9AaMw8XtdmqoHFs7ytJozNJe9TeoXNPL9 + eWP1G66LQcC + CugIKgPDP5HbRdRq0snB2134dByfH9 / vkpSmzz45LeqLRWgfKdoecZt6kTfHrCN6EiLMf7jx4r9Y9GIbjMkftPWdAUqf9b9vtb7CAVbV5GOEcqTGUAhLoXSNclyYWO0sGbESDO73JI0WCBs + gDKdTPMQWF336enyBth5RUdIxdWzzdv9m6Kgo1v + vdZ3TVjpkS2DUreS30ReZGrtiFA4PmY8IdHItxLC / jx + dn / RYZuT / PSBSu453BeGsfoz9vkLXZC1G56xjobPcPbhZpjRbPaLKV44XKzZ + 5HxbySaGuyjAbjrOFvjMd8iDKka0OLaObBEEMXeVuUq3NR6PJ5nA / uTX4GbTU1iNUsYdXZVsf6ZCOpbItYsxysVqEQaE3Q8nz04VLhcJmZc9cO02maUKQGMflvSCW19SNzq / 3czLuyWL63xFnkmHffL0ws / ztmYP0wJGrIS1 + kd1TcIxR9Cd8IHfLc24fslpIcBmosI77LWuh5vn7IBVNGtFkxWVZgaIo1sTUV//5Xp1cugTgDf5N855eoXqKDQY9nCmcVPXO1mTkfokpvN6IurVmyo4yzjllOSGIRasjdAzp1O8TlFYW8ZghTRFslyDZk1CcvrDFGjf1Qzeap7687mnwEU+2IyOXv6vjg7GhmzKMmBZW649p/aTFZvmaX93kutDVllAm3iM4qayzEs3YOfp2wl21eQ71BcG3N3uynu98IvbMD6YwpMSeIp0vIswEmxmBh9Sj7PyKy6pgRJJJGGwePD0trgkp0tJ8ZYHT67OmyHDGB3jANBgkrBgEEAYI3EQIxADATBgkqhkiG9w0BCRUxBgQEAQAAADBNBgkqhkiG9w0BCRQxQB4+AFcATQBTAHYAYwAgAEMAZQByAHQAaQBmAGkAYwBhAHQAZQAgAEsAZQB5ACAAQwBvAG4AdABhAGkAbgBlAHIwaQYJKwYBBAGCNxEBMVweWgBNAGkAYwByAG8AcwBvAGYAdAAgAFIAUwBBACAAUwBDAGgAYQBuAG4AZQBsACAAQwByAHkAcAB0AG8AZwByAGEAcABoAGkAYwAgAFAAcgBvAHYAaQBkAGUAcjCCA4EGCSqGSIb3DQEHAaCCA3IEggNuMIIDajCCA2YGCyqGSIb3DQEMCgEDoIIDFzCCAxMGCiqGSIb3DQEJFgGgggMDBIIC/zCCAvswggHjoAMCAQICEGg5e72zSVSAS8c5m/zOFscwDQYJKoZIhvcNAQELBQAwJTEjMCEGA1UEAxMaV01TdmMtU0hBMi1XSU4tUEQwUFRUUUdLNE0wHhcNMTkwOTA5MTAzMzE1WhcNMjkwOTA2MTAzMzE1WjAlMSMwIQYDVQQDExpXTVN2Yy1TSEEyLVdJTi1QRDBQVFRRR0s0TTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAKD7DA1peIismlqZPD4xD3cICmh7mWmmWR4gBeMDvlaz8ccWCpAqY9OFrIPP7kqfd54pgMxvoDmRDNdcVUQuMgL/tJQ4yjm1YKP1bLKYES/gWlKCnIumvBoCtTtMVil0mqvXROmTy+MoHRhybK423+j5X7pA6ZwAo5hGyFDSF4banf+GusunCc3luCFRH4rtN2lBp5X1DwzfeKRUXjP/o65n/FWn0ezEQXLys6HWdVp3naElL2YdwSYzXFrMe2J5lspnFOWRK+1DCdsuG0vHYq5itRJ1FG3bYDjbbs941gyEYRkmhxb78O0jA7Sp6SbHKrVl+3ODPrsUy64skWRsMnUCAwEAAaMnMCUwEwYDVR0lBAwwCgYIKwYBBQUHAwEwDgYDVR0PBAcDBQCwAAAAMA0GCSqGSIb3DQEBCwUAA4IBAQBvbveHhoM4bk3Sc5YZ+XUcmATViMkHNgSujINtP9ldvHfrn0yiTGWUvQIBkGzbCnYyWZDfKnwI2aBAU66VGzm9s3s03qyHFIkRI0vz8DOE6prAfE317qdQH4Ggd7/gKpVXXp+F4/ej0xvoUFiszxalSy8s6CoMwkE4KE6hvj4K7l82DDj0o86i3aujMTH7OPZMtzQG/S0gvzn85KmfDrn4CZkj27nRp0ehJ9zgt8Ddpy3O5jdciVKqJrYooWetPiSR7HOj9CbxCim3JZLYP+FHZe6PsPbUWqNHIyLbvVGXA9UyuEv8bbC3DrRhOu4zELYoFMlbebkc91OrEQC0eXZKMTwwEwYJKoZIhvcNAQkVMQYEBAEAAAAwJQYJKoZIhvcNAQkUMRgeFgBXAE0AUwBWAEMALQBTAEgAQQAyAAAwOzAfMAcGBSsOAwIaBBTRWAaawooWs/SBTRpGTZU3KO9LHQQUmaucOm0/gBnt5tPcnffEoZNLjTwCAgfQ";
        byte[] bytes = Convert.FromBase64String(base64String);
        return new X509Certificate2(bytes, "12345678");
    }
}
