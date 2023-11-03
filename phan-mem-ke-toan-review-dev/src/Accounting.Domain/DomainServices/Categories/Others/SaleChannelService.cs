using Accounting.Categories.Others;
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

namespace Accounting.DomainServices.Categories.Others
{
    public class SaleChannelService : BaseDomainService<SaleChannel, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public SaleChannelService(IRepository<SaleChannel, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(SaleChannel entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> Check(string orgCode, string code)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.Code == code
                                &&p.OrgCode == orgCode
                                );
        }
        public override async Task CheckDuplicate(SaleChannel entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.SaleChannel, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<bool> IsParentGroup(string id)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.ParentId == id);
        }
    }
}
