using Accounting.Caching;
using Accounting.Exceptions;
using Accounting.Localization;
using Accounting.Vouchers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Accounting.Business
{
    public class ProductVoucherBusiness : BaseBusiness
    {
        #region Fields
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public ProductVoucherBusiness(IStringLocalizer<AccountingResource> localizer,
                AccountingCacheManager accountingCacheManager
            ) : base(localizer)
        {
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        #region Methods
        public async Task CheckLockVoucher(ProductVoucher entity)
        {
            var errorMessage = "";
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(entity.VoucherCode,
                                                                    entity.OrgCode);
            if (entity.Locked == true)
            {
                errorMessage = _localizer["Err:VoucherNumberLocked", entity.VoucherCode];
            }
            if (entity.VoucherDate <= voucherCategory.BookClosingDate)
            {
                errorMessage = _localizer["Err:VoucherNumberLockedToDate", entity.VoucherCode
                                                 , $"{voucherCategory.BookClosingDate:dd/MM/yyyy}"];
            }
            if (errorMessage != "")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                                                errorMessage);
            }
        }
        #endregion
    }
}
