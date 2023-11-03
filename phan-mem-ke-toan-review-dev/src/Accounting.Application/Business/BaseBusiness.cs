using Accounting.Localization;
using Microsoft.Extensions.Localization;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Accounting.Business
{
    public class BaseBusiness : ITransientDependency
    {
        #region Fields
        protected readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        #region Ctor
        public BaseBusiness(IStringLocalizer<AccountingResource> localizer)
        {
            _localizer = localizer;
        }
        #endregion
        protected string GetNewObjectId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
