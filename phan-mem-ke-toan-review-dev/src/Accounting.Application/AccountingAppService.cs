using Accounting.Localization;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using Volo.Abp.Application.Services;

namespace Accounting;

/* Inherit your application services from this class.
 */
[Authorize]
public abstract class AccountingAppService : ApplicationService
{
    protected AccountingAppService()
    {
        LocalizationResource = typeof(AccountingResource);
    }
    protected string GetNewObjectId()
    {
        return ObjectId.GenerateNewId().ToString();
    }
}
