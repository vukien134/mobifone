using Accounting.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace Accounting.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class AccountingPageModel : AbpPageModel
{
    protected AccountingPageModel()
    {
        LocalizationResourceType = typeof(AccountingResource);
    }
}
