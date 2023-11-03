using Volo.Abp.Modularity;

namespace Accounting;

[DependsOn(
    typeof(AccountingApplicationModule),
    typeof(AccountingDomainTestModule)
    )]
public class AccountingApplicationTestModule : AbpModule
{

}
