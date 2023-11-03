using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.DomainServices.BaseServices;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Vouchers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;

namespace Accounting.DomainServices.Vouchers
{
    public class VoucherNumberService : BaseDomainService<VoucherNumber, string>
    {
        private readonly WebHelper _webHelper;
        public VoucherNumberService(IRepository<VoucherNumber, string> repository,
                WebHelper webHelper
            ): base(repository)
        {
            _webHelper = webHelper;
        }
        public async Task<bool> IsExistVoucherNumber(string voucherCode, int day, int month, int year)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.VoucherCode == voucherCode
                                && p.Day == day
                                && p.Month == month
                                && p.Year == year);
        }
        public async Task<bool> IsExistBusinessVoucherNumber(string businessCode, int day, int month, int year)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.BusinessCode == businessCode
                                && p.Day == day
                                && p.Month == month
                                && p.Year == year);
        }
        public async Task<VoucherNumber> GetDataVoucherNumber(string voucherCode, int day, int month, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.VoucherCode == voucherCode
                                && p.Day == day
                                && p.Month == month
                                && p.Year == year);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }

        public async Task<VoucherNumber> GetDataBusinessVoucherNumber(string businessCode, int day, int month, int year)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.BusinessCode == businessCode
                                && p.Day == day
                                && p.Month == month
                                && p.Year == year);
            return await AsyncExecuter.FirstOrDefaultAsync(queryable);
        }
    }
}
