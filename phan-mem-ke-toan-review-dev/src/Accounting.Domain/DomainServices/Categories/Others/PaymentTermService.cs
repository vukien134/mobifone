using Accounting.Categories.Others.PaymentTerms;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Localization;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Categories.Others
{
    public class PaymentTermService : BaseDomainService<PaymentTerm, string>
    {
        #region Privates
        private readonly IStringLocalizer<AccountingResource> _localizer;
        #endregion
        public PaymentTermService(IRepository<PaymentTerm, string> repository,
                IStringLocalizer<AccountingResource> localizer
            ) : base(repository)
        {
            _localizer = localizer;
        }
        public async Task<bool> IsExistCode(PaymentTerm entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Code == entity.Code
                                && p.Id != entity.Id);
        }
        public async Task<bool> Check (string code, string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == orgCode
                                && p.Code == code
                                );
        }
        public override async Task CheckDuplicate(PaymentTerm entity)
        {
            bool isExist = await IsExistCode(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.PaymentTerm, ErrorCode.Duplicate),
                        _localizer["Err:CodeAlreadyExist", entity.Code]);
            }
        }
        public async Task<List<PaymentTerm>> GetByPaymentTermAsync(string voucherPaymentBook, 
                                            string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Code == voucherPaymentBook && p.OrgCode == orgCode);

            return await AsyncExecuter.ToListAsync(queryable);
        }
    }
}
