using Accounting.Catgories.ProductVouchers;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.BusinessCategories;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Vouchers;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;

namespace Accounting.DomainServices.Categories
{
    public class ProductVoucherService : BaseDomainService<ProductVoucher, string>
    {
        private readonly PartnerGroupService _partnerGroupService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccPartnerService _accPartnerService;
        private readonly DepartmentService _departmentService;
        private readonly CurrencyService _currencyService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        public ProductVoucherService(PartnerGroupService partnerGroupService,
                                    TenantSettingService tenantSettingService,
                                    VoucherCategoryService voucherCategoryService,
                                    AccountSystemService accountSystemService,
                                    BusinessCategoryService businessCategoryService,
                                    AccPartnerService accPartnerService,
                                    DepartmentService departmentService,
                                    CurrencyService currencyService,
                                    TaxCategoryService taxCategoryService,
                                    IUnitOfWorkManager unitOfWorkManager,
                                    WebHelper webHelper,
                                    IStringLocalizer<AccountingResource> localizer,
                                    IRepository<ProductVoucher, string> repository) : base(repository)
        {
            _partnerGroupService = partnerGroupService;
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _accountSystemService = accountSystemService;
            _businessCategoryService = businessCategoryService;
            _accPartnerService = accPartnerService;
            _departmentService = departmentService;
            _currencyService = currencyService;
            _taxCategoryService = taxCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _localizer = localizer;
        }
        public async Task<List<ProductVoucher>> GetByProductIdAsync(string productId)
        {
            var queryable = await this.GetQueryableAsync();
            queryable = queryable.Where(p => p.Id == productId);

            return await AsyncExecuter.ToListAsync(queryable);
        }

        public async Task<bool> IsExistId(string id)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.Id == id);
        }

        public async Task<bool> IsExistCode(ProductVoucher entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.VoucherNumber == entity.VoucherNumber
                                && p.VoucherCode == entity.VoucherCode
                                && p.Year == entity.Year
                                && p.VoucherNumber != ""
                                && p.Id != entity.Id);
        }

        public async Task<bool> IsExistCode(CrudProductVoucherDto entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.VoucherNumber == entity.VoucherNumber
                                && p.VoucherCode == entity.VoucherCode
                                && p.Year == entity.Year
                                && p.VoucherNumber != ""
                                && p.Id != entity.Id);
        }

        public override async Task CheckDuplicate(ProductVoucher entity)
        {
            bool isExist = await IsExistCode(entity);            
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductVoucher, ErrorCode.Duplicate),
                        _localizer["Err:VoucherNumberAlreadyExist", entity.VoucherNumber]);
            }
        }
        public async Task CheckLockVoucher(ProductVoucher entity)
        {
            var errorMessage = "";
            // check khóa chứng từ
            var voucherCategory = await _voucherCategoryService.GetByCode(entity.VoucherCode, entity.OrgCode);
            if (entity.Locked == true)
            {
                errorMessage = _localizer["Err:VoucherNumberLocked", entity.VoucherNumber];
            }
            if (entity.VoucherDate < voucherCategory.BookClosingDate)
            {
                errorMessage = _localizer["Err:VoucherNumberLockedToDate", 
                        entity.VoucherNumber,$"{voucherCategory.BookClosingDate:dd/MM/yyyy}"];                
            }
            // nếu có lỗi sẽ trả về
            if (errorMessage != "")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductVoucher, ErrorCode.Other), errorMessage);
            }
        }
        public async Task<int> CountVoucherAsync(string orgCode)
        {
            var queryable = await this.GetQueryableAsync();
            if (string.IsNullOrEmpty(orgCode))
            {
                queryable = queryable.Where(p => p.OrgCode == orgCode);
            }
            return await AsyncExecuter.CountAsync(queryable);
        }
    }
}