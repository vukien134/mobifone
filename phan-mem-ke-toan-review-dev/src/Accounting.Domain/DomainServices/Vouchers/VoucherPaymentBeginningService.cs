using Accounting.Categories.Partners;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Vouchers
{
    public class VoucherPaymentBeginningService : BaseDomainService<VoucherPaymentBeginning, string>
    {
        public VoucherPaymentBeginningService(IRepository<VoucherPaymentBeginning, string> repository) : base(repository)
        {
        }
        public async Task<bool> IsExistCode(AccPartner entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(VoucherPaymentBeginning entity)
        {
            //bool isExist = await IsExistCode(entity);
            //if (isExist)
            //{
            //    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.Duplicate),
            //            $"AccPartner Code ['{entity.Code}'] already exist ");
            //}
        }
        //public async Task<AccPartner> GetAccPartnerByCodeAsync(string Code, string OrdCode)
        //{
        //    var queryable = await this.GetQueryableAsync();
        //    queryable = queryable.Where(p => p.Code == Code && p.OrgCode == OrdCode);
        //    return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        //}
        //public async Task<List<AccPartner>> GetAccPartnerAsync(string OrdCode)
        //{
        //    var queryable = await this.GetQueryableAsync();
        //    queryable = queryable.Where(p => p.OrgCode == OrdCode);
        //    return await AsyncExecuter.ToListAsync(queryable);
        //}
    }
}
