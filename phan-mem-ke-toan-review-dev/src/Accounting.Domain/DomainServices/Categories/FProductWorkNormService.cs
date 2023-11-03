using Accounting.Categories.Contracts;
using Accounting.Categories.Products;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories
{
    public class FProductWorkNormService : BaseDomainService<FProductWorkNorm, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public FProductWorkNormService(IRepository<FProductWorkNorm, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(FProductWorkNorm entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.FProductWorkCode == entity.FProductWorkCode
                                && p.Year == entity.Year
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(FProductWorkNorm entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.FProductWorkNorm, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.FProductWorkCode]);
            }
        }
    }
}
