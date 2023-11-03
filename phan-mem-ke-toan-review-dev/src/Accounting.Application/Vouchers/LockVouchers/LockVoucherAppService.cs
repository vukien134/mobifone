using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Catgories.Others.Careers;
using Accounting.Catgories.VoucherCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Helpers;
using Accounting.Vouchers.ResetVoucherNumbers;
using Accounting.Vouchers.VoucherNumbers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Vouchers.VoucherNumbers
{
    public class LockVoucherAppService : AccountingAppService, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly VoucherNumberService _voucherNumberService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccVoucherService _accVoucherService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly ProductVoucherService _productVoucherService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        #endregion
        #region Ctor
        public LockVoucherAppService(VoucherNumberService voucherNumberService,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            ProductVoucherService productVoucherService,
                            AccVoucherService accVoucherService,
                            AccountingCacheManager accountingCacheManager,
                            IUnitOfWorkManager unitOfWorkManager,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper
                            )
        {
            _voucherNumberService = voucherNumberService;
            _userService = userService;
            _voucherCategoryService = voucherCategoryService;
            _productVoucherService = productVoucherService;
            _accVoucherService = accVoucherService;
            _accountingCacheManager = accountingCacheManager;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<ResultDto> LockAsync(LockVoucherDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var voucherCategory = await _voucherCategoryService.GetQueryableAsync();
            voucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var accVoucher = await _accVoucherService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucer = await _productVoucherService.GetQueryableAsync();
            productVoucer = productVoucer.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var lstKK = "KK0";
            foreach (var item in dto.Data)
            {
                var dataVoucherCategory = voucherCategory.Where(p => p.Code == item.VoucherCode).FirstOrDefault();
                if (dataVoucherCategory == null) throw new Exception("Không tìm thấy mã chứng từ " + item.VoucherCode);
                dataVoucherCategory.BookClosingDate = item.BookClosingDate;
                dataVoucherCategory.BusinessBeginningDate = item.BusinessBeginningDate;
                dataVoucherCategory.BusinessEndingDate = item.BusinessEndingDate;
                await _voucherCategoryService.UpdateAsync(dataVoucherCategory);

                if (lstKK.Contains(item.VoucherCode))
                {

                } 
                else
                {
                    if (dataVoucherCategory.VoucherKind == "KT")
                    {
                        var lstAccVoucher = accVoucher.Where(p => p.VoucherCode == item.VoucherCode).ToList();
                        foreach (var itemAccVoucher in lstAccVoucher)
                        {
                            itemAccVoucher.Locked = (item.BookClosingDate >= itemAccVoucher.VoucherDate) ? true : false;
                            await _accVoucherService.UpdateAsync(itemAccVoucher);
                        }
                    }
                    else
                    {
                        var lstProductVoucher = productVoucer.Where(p => p.VoucherCode == item.VoucherCode).ToList();
                        foreach (var itemProductVoucher in lstProductVoucher)
                        {
                            itemProductVoucher.Locked = (item.BookClosingDate >= itemProductVoucher.VoucherDate) ? true : false;
                            await _productVoucherService.UpdateAsync(itemProductVoucher);
                        }
                    }
                }
            }
            await _accountingCacheManager.RemoveClassCache<VoucherCategoryDto>();
            var res = new ResultDto();
            res.Ok = true;
            return res;
        }

        #region Private
        
        #endregion
    }
}
