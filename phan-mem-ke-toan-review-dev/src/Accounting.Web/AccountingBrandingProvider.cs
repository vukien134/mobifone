using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace Accounting.Web;

[Dependency(ReplaceServices = true)]
public class AccountingBrandingProvider : DefaultBrandingProvider
{
    public override string AppName => "Accounting";
}
