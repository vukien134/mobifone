using Accounting.Catgories.Others.CostOfGoods;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.Exceptions;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Uow;


namespace Accounting.DomainServices.Vouchers
{
    public class CalculatePriceFifoService : DomainService
    {
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccPartnerService _accPartnerService;
        private readonly DepartmentService _departmentService;
        private readonly CurrencyService _currencyService;
        private readonly Ledger _ledgerService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public CalculatePriceFifoService(
                                TenantSettingService tenantSettingService,
                                VoucherCategoryService voucherCategoryService,
                                AccountSystemService accountSystemService,
                                BusinessCategoryService businessCategoryService,
                                AccPartnerService accPartnerService,
                                DepartmentService departmentService,
                                CurrencyService currencyService,
                                Ledger ledgerService,
                                TaxCategoryService taxCategoryService,
                                IUnitOfWorkManager unitOfWorkManager)
        {
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _accountSystemService = accountSystemService;
            _businessCategoryService = businessCategoryService;
            _accPartnerService = accPartnerService;
            _departmentService = departmentService;
            _currencyService = currencyService;
            _ledgerService = ledgerService;
            _taxCategoryService = taxCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
        }

        public async Task ReturnError(string error)
        {
            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                        $"{error}");
        }
    }
}
