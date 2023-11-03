using Accounting.EntityFrameworkCore;
using Volo.Abp.Modularity;

namespace Accounting;

[DependsOn(
    typeof(AccountingEntityFrameworkCoreTestModule)
    )]
public class AccountingDomainTestModule : AbpModule
{

}
