using Accounting.Categories.Accounts;
using Accounting.DomainServices.BaseServices;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;


namespace Accounting.DomainServices.Vouchers
{
    public class AccVoucherService : BaseDomainService<AccVoucher, string>
    {
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly AccountSystemService _accountSystemService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly IObjectMapper _objectMapper;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly AccPartnerService _accPartnerService;
        private readonly DepartmentService _departmentService;
        private readonly CurrencyService _currencyService;
        private readonly ContractService _contractService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
        private readonly AccCaseService _accCaseService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;

        public AccVoucherService(IRepository<AccVoucher, string> repository,
                                TenantSettingService tenantSettingService,
                                VoucherCategoryService voucherCategoryService,
                                IObjectMapper objectMapper,
                                DefaultVoucherCategoryService defaultVoucherCategoryService,
                                DefaultTenantSettingService defaultTenantSettingService,
                                AccountSystemService accountSystemService,
                                BusinessCategoryService businessCategoryService,
                                AccPartnerService accPartnerService,
                                DepartmentService departmentService,
                                CurrencyService currencyService,
                                ContractService contractService,
                                FProductWorkService fProductWorkService,
                                TaxCategoryService taxCategoryService,
                                AccSectionService accSectionService,
                                AccCaseService accCaseService,
                                WorkPlaceSevice workPlaceSevice,
                                IUnitOfWorkManager unitOfWorkManager,
                                IStringLocalizer<AccountingResource> localizer,
                                WebHelper webHelper) : base(repository)
        {
            _tenantSettingService = tenantSettingService;
            _voucherCategoryService = voucherCategoryService;
            _objectMapper = objectMapper;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _defaultTenantSettingService = defaultTenantSettingService;
            _accountSystemService = accountSystemService;
            _businessCategoryService = businessCategoryService;
            _accPartnerService = accPartnerService;
            _departmentService = departmentService;
            _currencyService = currencyService;
            _contractService = contractService;
            _fProductWorkService = fProductWorkService;
            _taxCategoryService = taxCategoryService;
            _accSectionService = accSectionService;
            _accCaseService = accCaseService;
            _workPlaceSevice = workPlaceSevice;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _localizer = localizer;
        }
        public async Task<bool> IsExistId(string id)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                && p.Id == id);
        }
        public async Task<bool> IsExistCode(AccVoucher entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.VoucherNumber == entity.VoucherNumber
                                && p.VoucherCode == entity.VoucherCode
                                && p.Year == entity.Year
                                && p.VoucherNumber != ""
                                && p.Id != entity.Id);
        }
        public async Task<bool> IsExistCode(CrudAccVoucherDto entity)
        {
            var queryable = await this.GetQueryableAsync();
            return queryable.Any(p => p.OrgCode == entity.OrgCode
                                && p.VoucherNumber == entity.VoucherNumber
                                && p.VoucherCode == entity.VoucherCode
                                && p.Year == entity.Year
                                && p.VoucherNumber != ""
                                && p.Id != entity.Id);
        }
        public override async Task CheckDuplicate(AccVoucher entity)
        {
            bool isExist = await IsExistCode(entity);
            await CheckValidate(entity);
            if (isExist)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Duplicate),
                        _localizer["Err:VoucherNumberAlreadyExist", entity.VoucherNumber]);
            }
        }

        public async Task CheckValidate(AccVoucher entity)
        {
            var errorMessage = "";
            // nếu có lỗi sẽ trả về
            if (errorMessage != "")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.Other), errorMessage);
            }
        }

        public async Task CheckLockVoucher(AccVoucher entity)
        {
            var errorMessage = "";
            // check khóa chứng từ
            var voucherCategory = await _voucherCategoryService.GetByCode(entity.VoucherCode, entity.OrgCode);
            if (voucherCategory == null)
            {
                var defaultVoucherCategory = await _defaultVoucherCategoryService.GetByCodeAsync(entity.VoucherCode);
                voucherCategory = _objectMapper.Map<DefaultVoucherCategory, VoucherCategory>(defaultVoucherCategory);
                voucherCategory.OrgCode = _webHelper.GetCurrentOrgUnit();
            }
            if (entity.Locked == true)
            {
                errorMessage = "Chứng từ [" + entity.VoucherCode + "] đã bị khóa";
            }
            if (entity.VoucherDate < voucherCategory.BookClosingDate)
            {
                errorMessage = "Chứng từ [" + entity.VoucherCode + "] đã bị khóa đến ngày " + voucherCategory.BookClosingDate;
            }
            // nếu có lỗi sẽ trả về
            if (errorMessage != "")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other), errorMessage);
            }
        }

        public async Task CheckAccVoucher(CrudAccVoucherDto dto)
        {
            var voucherCategory = await _voucherCategoryService.CheckIsSavingLedgerAsync(dto.VoucherCode, dto.OrgCode);
            var accPartner = await _accPartnerService.GetAccPartnerByCodeAsync(dto.PartnerCode0, dto.OrgCode);
            var department = await _departmentService.GetDepartmentByCodeAsync(dto.DepartmentCode, dto.OrgCode);
            var businessCategory = await _businessCategoryService.GetBusinessByCodeAsync(dto.DepartmentCode, dto.OrgCode);
            var currency = await _currencyService.GetCurrencyByCodeAsync(dto.CurrencyCode, dto.OrgCode);
            var errorMessage = "";
            // Check đầu phiếu
            if (voucherCategory != null && voucherCategory.BusinessBeginningDate != null
                && dto.VoucherDate >= voucherCategory.BusinessBeginningDate && dto.VoucherDate <= voucherCategory.BusinessEndingDate)
            {
                errorMessage = "Ngày chứng từ không được nằm ngoài " + (voucherCategory?.BusinessBeginningDate ?? System.DateTime.Now).ToString("yyyy-MM-dd") + " - " + (voucherCategory?.BusinessEndingDate ?? System.DateTime.Now).ToString("yyyy-MM-dd");
            }
            else if ((dto.PartnerCode0 ?? "") != "" && accPartner == null)
            {
                errorMessage = $"Mã đối tượng {dto.PartnerCode0} không tồn tại!";
            }
            else if ((dto.DepartmentCode ?? "") != "" && department == null)
            {
                errorMessage = $"Bộ phận {dto.DepartmentCode} không tồn tại!";
            }
            else if ((dto.DepartmentCode ?? "") != "" && department.ParentId == null)
            {
                errorMessage = $"Mã bộ phận {dto.DepartmentCode} là bộ phận mẹ!";
            }
            else if ((dto.CurrencyCode ?? "") == "")
            {
                errorMessage = $"Chưa nhập mã ngoại tệ";
            }
            else if ((dto.CurrencyCode ?? "") != "" && currency == null)
            {
                errorMessage = $"Mã ngoại tệ {dto.CurrencyCode} không tồn tại!";
            }
            else if ((dto.BusinessCode ?? "") != "" && businessCategory == null)
            {
                errorMessage = $"Hạch toán {dto.BusinessCode} không tồn tại!";
            }
            else if ((dto.BusinessCode ?? "") != "" && businessCategory.VoucherCode != dto.VoucherCode)
            {
                errorMessage = $"Mã hạch toán không khớp với mã chứng từ!";
            }
            if (errorMessage != "")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other), errorMessage);
            }
            // Kiểm tra chi tiết
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == dto.OrgCode && p.Year == dto.Year).ToList();
            var lstAccPartner = await _accPartnerService.GetAccPartnerAsync(dto.OrgCode);
            var lstContract = (await _contractService.GetQueryableAsync()).Where(p => p.OrgCode == dto.OrgCode).ToList();
            var lstFProductWork = (await _fProductWorkService.GetQueryableAsync()).Where(p => p.OrgCode == dto.OrgCode).ToList();
            var lstAccSection = (await _accSectionService.GetQueryableAsync()).Where(p => p.OrgCode == dto.OrgCode).ToList();
            var lstWorkPlace = (await _workPlaceSevice.GetQueryableAsync()).Where(p => p.OrgCode == dto.OrgCode).ToList();
            var lstAccCase = (await _accCaseService.GetQueryableAsync()).Where(p => p.OrgCode == dto.OrgCode).ToList();
            int i = 1;
            var dataDetail = new List<AccVoucherDetailDto>();
            if (dto.AccVoucherDetails == null || dto.AccVoucherDetails.Count == 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other), "Bạn chưa nhập chi tiết!");
            }
            dataDetail.AddRange((from a in dto.AccVoucherDetails
                                 select new AccVoucherDetailDto
                                 {
                                     Ord0 = i++.ToString(),
                                     DebitAcc = a.DebitAcc,
                                     PartnerCode = a.PartnerCode,
                                     PartnerName = a.PartnerName,
                                     ContractCode = a.ContractCode,
                                     FProductWorkCode = a.FProductWorkCode,
                                     SectionCode = a.SectionCode,
                                     WorkPlaceCode = a.WorkPlaceCode,
                                     CreditAcc = a.CreditAcc,
                                     ClearingPartnerCode = a.ClearingPartnerCode,
                                     ClearingContractCode = a.ClearingContractCode,
                                     ClearingFProductWorkCode = a.ClearingFProductWorkCode,
                                     ClearingSectionCode = a.ClearingSectionCode,
                                     ClearingWorkPlaceCode = a.ClearingWorkPlaceCode,
                                     CaseCode = a.CaseCode,
                                     Note = "Detail",
                                 }).ToList());
            i = 1;
            if (dto.AccTaxDetails != null)
                dataDetail.AddRange((from a in dto.AccTaxDetails
                                     select new AccVoucherDetailDto
                                     {
                                         Ord0 = i++.ToString(),
                                         DebitAcc = a.DebitAcc,
                                         PartnerCode = a.PartnerCode,
                                         ContractCode = a.ContractCode,
                                         FProductWorkCode = a.FProductWorkCode,
                                         SectionCode = a.SectionCode,
                                         WorkPlaceCode = a.WorkPlaceCode,
                                         CreditAcc = a.CreditAcc,
                                         ClearingPartnerCode = a.ClearingPartnerCode,
                                         ClearingContractCode = a.ClearingContractCode,
                                         ClearingFProductWorkCode = a.ClearingFProductWorkCode,
                                         ClearingSectionCode = a.ClearingSectionCode,
                                         ClearingWorkPlaceCode = a.ClearingWorkPlaceCode,
                                         CaseCode = a.CaseCode,
                                         Note = "Tax",
                                     }).ToList());
            foreach (var item in dataDetail)
            {
                var debitAcc = lstAccountSystem.Where(p => p.AccCode == item.DebitAcc).FirstOrDefault();
                var creditAcc = lstAccountSystem.Where(p => p.AccCode == item.CreditAcc).FirstOrDefault();
                var accPartnerDetail = lstAccPartner.Where(p => p.Code == item.PartnerCode).FirstOrDefault();
                var accPartnerDetailCl = lstAccPartner.Where(p => p.Code == item.ClearingPartnerCode).FirstOrDefault();
                var contractDetail = lstContract.Where(p => p.Code == item.ContractCode).FirstOrDefault();
                var contractDetailCl = lstContract.Where(p => p.Code == item.ClearingContractCode).FirstOrDefault();
                var fProductWorkDetail = lstFProductWork.Where(p => p.Code == item.FProductWorkCode).FirstOrDefault();
                var fProductWorkDetailCl = lstFProductWork.Where(p => p.Code == item.ClearingFProductWorkCode).FirstOrDefault();
                var accSectionDetail = lstAccSection.Where(p => p.Code == item.SectionCode).FirstOrDefault();
                var accSectionDetailCl = lstAccSection.Where(p => p.Code == item.ClearingSectionCode).FirstOrDefault();
                var workPlaceDetail = lstWorkPlace.Where(p => p.Code == item.WorkPlaceCode).FirstOrDefault();
                var workPlaceDetailCl = lstWorkPlace.Where(p => p.Code == item.ClearingWorkPlaceCode).FirstOrDefault();
                var accCase = lstAccCase.Where(p => p.Code == item.CaseCode).FirstOrDefault();

                if ((item.DebitAcc ?? "") == "" && (creditAcc?.IsBalanceSheetAcc ?? "") != "C")
                {
                    errorMessage = $"Chưa nhập mã tài khoản Nợ!";
                }
                else if ((item.DebitAcc ?? "") != "" && debitAcc == null)
                {
                    errorMessage = $"Mã tài khoản! {dto.PartnerCode0} không tồn tại!";
                }
                else if ((item.DebitAcc ?? "") != "" && debitAcc.AccType != "C")
                {
                    errorMessage = $"Mã tài khoản! {dto.PartnerCode0} là tài khoản mẹ!";
                }
                else if ((item.PartnerCode ?? "") == "" && (debitAcc?.AttachPartner ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã đối tượng!";
                }
                else if ((item.ContractCode ?? "") == "" && (debitAcc?.AttachContract ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã hợp đồng!";
                }
                else if ((item.FProductWorkCode ?? "") == "" && (debitAcc?.AttachProductCost ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã công trình, sản phẩm!";
                }
                else if ((item.SectionCode ?? "") == "" && (debitAcc?.AttachAccSection ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã khoản mục!";
                }
                else if ((item.WorkPlaceCode ?? "") == "" && (debitAcc?.AttachWorkPlace ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã phân xưởng!";
                }
                else if ((item.CreditAcc ?? "") == "" && debitAcc.IsBalanceSheetAcc != "C")
                {
                    errorMessage = $"Chưa nhập mã tài khoản Có!";
                }
                else if ((item.CreditAcc ?? "") != "" && creditAcc == null)
                {
                    errorMessage = $"Mã tài khoản! {dto.PartnerCode0} không tồn tại!";
                }
                else if ((item.CreditAcc ?? "") != "" && creditAcc.AccType != "C")
                {
                    errorMessage = $"Mã tài khoản! {dto.PartnerCode0} là tài khoản mẹ!";
                }
                else if ((item.PartnerCode ?? "") + (item.ClearingPartnerCode ?? "") == "" && (creditAcc?.AttachPartner ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã đối tượng!";
                }
                else if ((item.ContractCode ?? "") + (item.ClearingContractCode ?? "") == "" && (creditAcc?.AttachContract ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã hợp đồng!";
                }
                else if ((item.FProductWorkCode ?? "") + (item.ClearingFProductWorkCode ?? "") == "" && (creditAcc?.AttachProductCost ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã công trình, sản phẩm!";
                }
                else if ((item.SectionCode ?? "") + (item.ClearingSectionCode ?? "") == "" && (creditAcc?.AttachAccSection ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã khoản mục!";
                }
                else if ((item.WorkPlaceCode ?? "") + (item.ClearingWorkPlaceCode ?? "") == "" && (creditAcc?.AttachWorkPlace ?? "") == "C")
                {
                    errorMessage = $"Chưa nhập mã phân xưởng!";
                }
                else if ((item.PartnerCode ?? "") != "" && (accPartnerDetail == null))
                {
                    errorMessage = $"Mã đối tượng {item.PartnerCode} không tồn tại!";
                }
                else if ((item.ClearingPartnerCode ?? "") != "" && (accPartnerDetailCl == null))
                {
                    errorMessage = $"Mã đối tượng {item.ClearingPartnerCode} không tồn tại!";
                }
                else if ((item.ContractCode ?? "") != "" && (contractDetail == null))
                {
                    errorMessage = $"Mã hợp đồng {item.ContractCode} không tồn tại!";
                }
                else if ((item.ClearingContractCode ?? "") != "" && (contractDetailCl == null))
                {
                    errorMessage = $"Mã hợp đồng {item.ClearingContractCode} không tồn tại!";
                }
                else if ((item.FProductWorkCode ?? "") != "" && (fProductWorkDetail == null))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.FProductWorkCode} không tồn tại!";
                }
                else if ((item.FProductWorkCode ?? "") != "" && ((fProductWorkDetail.FPWType ?? "C") != "C"))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.FProductWorkCode} là mã mẹ!";
                }
                else if ((item.ClearingFProductWorkCode ?? "") != "" && (fProductWorkDetailCl == null))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.ClearingFProductWorkCode} không tồn tại!";
                }
                else if ((item.ClearingFProductWorkCode ?? "") != "" && ((fProductWorkDetailCl.FPWType ?? "C") != "C"))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.ClearingFProductWorkCode} là mã mẹ!";
                }
                else if ((item.SectionCode ?? "") != "" && (accSectionDetail == null))
                {
                    errorMessage = $"Mã khoản mục {item.SectionCode} không tồn tại!";
                }
                else if ((item.ClearingSectionCode ?? "") != "" && (accSectionDetailCl == null))
                {
                    errorMessage = $"Mã khoản mục {item.ClearingSectionCode} không tồn tại!";
                }
                else if ((item.WorkPlaceCode ?? "") != "" && (workPlaceDetail == null))
                {
                    errorMessage = $"Mã phân xưởng {item.WorkPlaceCode} không tồn tại!";
                }
                else if ((item.ClearingWorkPlaceCode ?? "") != "" && (workPlaceDetailCl == null))
                {
                    errorMessage = $"Mã phân xưởng {item.ClearingWorkPlaceCode} không tồn tại!";
                }
                else if ((item.CaseCode ?? "") != "" && (accCase == null))
                {
                    errorMessage = $"Mã vụ việc {item.CaseCode} không tồn tại!";
                }
                // --------------------------------------------
                if (errorMessage != "")
                {
                    if (item.Note == "Detail")
                    {
                        errorMessage += $" Lỗi tại dòng {item.Ord0} ở bảng chi tiết kế toán";
                    }
                    else
                    {
                        errorMessage += $" Lỗi tại dòng {item.Ord0} ở bảng chi tiết thuế";
                    }
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other), errorMessage);
                }
            }
        }

        public async Task ReturnError(string error)
        {
            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                        $"{error}");
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
