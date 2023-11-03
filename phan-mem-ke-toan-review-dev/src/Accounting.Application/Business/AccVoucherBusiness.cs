using Accounting.Caching;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.Others.TenantSettings;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.Ledgers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;

namespace Accounting.Business
{
    public class AccVoucherBusiness : BaseBusiness
    {
        #region Fields
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly DefaultTenantSettingService _defaultTenantSettingService;
        private readonly AccPartnerService _accPartnerService;
        private readonly DepartmentService _departmentService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly ContractService _contractService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly AccCaseService _accCaseService;
        private readonly WebHelper _webHelper;
        private readonly ICurrentTenant _currentTenant;
        private readonly YearCategoryService _yearCategoryService;
        #endregion
        #region Ctor
        public AccVoucherBusiness(IStringLocalizer<AccountingResource> localizer,
                AccountingCacheManager accountingCacheManager,
                DefaultTenantSettingService defaultTenantSettingService,
                AccPartnerService accPartnerService,
                DepartmentService departmentService,
                BusinessCategoryService businessCategoryService,
                ContractService contractService,
                FProductWorkService fProductWorkService,
                AccSectionService accSectionService,
                WorkPlaceSevice workPlaceSevice,
                AccCaseService accCaseService,
                WebHelper webHelper,
                ICurrentTenant currentTenant,
                YearCategoryService yearCategoryService
                ) : base(localizer)
        {
            _accountingCacheManager = accountingCacheManager;
            _defaultTenantSettingService = defaultTenantSettingService;
            _accPartnerService = accPartnerService;
            _departmentService = departmentService;
            _businessCategoryService = businessCategoryService;
            _contractService = contractService;
            _fProductWorkService = fProductWorkService;
            _accSectionService = accSectionService;
            _workPlaceSevice = workPlaceSevice;
            _accCaseService = accCaseService;
            _webHelper = webHelper;
            _currentTenant = currentTenant;
            _yearCategoryService = yearCategoryService;
        }
        #endregion
        #region Methods
        public async Task CheckLockVoucher(CrudAccVoucherDto dto)
        {
            var errorMessage = "";            
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(dto.VoucherCode,
                                                                    dto.OrgCode);
            if (dto.Locked == true)
            {
                errorMessage = _localizer["Err:VoucherNumberLocked", dto.VoucherCode];
            }
            if (dto.VoucherDate <= voucherCategory.BookClosingDate)
            {
               errorMessage = _localizer["Err:VoucherNumberLockedToDate", dto.VoucherCode
                                                ,$"{voucherCategory.BookClosingDate:dd/MM/yyyy}"];
            }            
            if (errorMessage != "")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other), 
                                                errorMessage);
            }
        }
        public async Task CheckLockVoucher(AccVoucher entity)
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
        public async Task CheckAccVoucher(CrudAccVoucherDto dto)
        {
            var tenantSettings = await this.GetTenantSettings(dto.OrgCode);
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(dto.VoucherCode,
                                                            dto.OrgCode);
            var currency = await _accountingCacheManager.GetCurrenciesAsync(dto.CurrencyCode, dto.OrgCode);
            var amountMax = decimal.Parse(tenantSettings.Where(p => p.Key == "TIEN_MAX").Select(p => p.Value).FirstOrDefault());
            string errorMessage = "";

            // Check đầu phiếu
            if (dto.VoucherDate.Year != _webHelper.GetCurrentYear())
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                    "Ngày chứng từ không thuộc năm làm việc " + _webHelper.GetCurrentYear());
            }
            if (
                (!string.IsNullOrEmpty(voucherCategory.BusinessBeginningDate.ToString())
                 && dto.VoucherDate < voucherCategory.BusinessBeginningDate) 
                 || 
                (!string.IsNullOrEmpty(voucherCategory.BusinessEndingDate.ToString())
                  && dto.VoucherDate > voucherCategory.BusinessEndingDate)
               )
            {
                DateTime beginningDate = voucherCategory?.BusinessBeginningDate ?? System.DateTime.Now;
                DateTime endingDate = voucherCategory?.BusinessEndingDate ?? System.DateTime.Now;

                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                    _localizer["Err:VoucherDateNotInRangeAllow", $"{beginningDate:dd/MM/yyyy}", $"{endingDate:dd/MM/yyyy}"]);
            }
            if (!string.IsNullOrEmpty(dto.PartnerCode0))
            {
                bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, dto.PartnerCode0);
                if (!isExistPartner)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Partner, ErrorCode.NotFoundEntity),
                    _localizer["Err:PartnerCodeNotExist", dto.PartnerCode0]);
                }                
            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                bool isExistDepartment = await _departmentService.IsExistCode(dto.OrgCode, dto.DepartmentCode);
                if (!isExistDepartment)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Department, ErrorCode.NotFoundEntity),
                    _localizer["Err:DepartmentCodeNotExist", dto.DepartmentCode]);
                }                
            }            
            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                bool isExist = await _businessCategoryService.IsExistCode(dto.OrgCode, dto.BusinessCode);
                if (!isExist)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.BusinessCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:BusinessCodeNotExist", dto.BusinessCode]);
                }
                var businessCategory = await _businessCategoryService.GetBusinessByCodeAsync(dto.BusinessCode, dto.OrgCode);
                if ((businessCategory?.VoucherCode ?? "") != dto.VoucherCode)
                {
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                        _localizer["Err:BusinessCodeDifferentVoucherCode", dto.BusinessCode]);
                }
            }
            if (dto.TotalAmount > amountMax && dto.VoucherCode == "PCT")
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.BusinessCategory, ErrorCode.NotFoundEntity),
                        _localizer["Err:ExceedTotalAmountMax", amountMax]);
            }

            // Kiểm tra chi tiết
            if (dto.VoucherCode == "VAT") return;

            if (dto.AccVoucherDetails == null || dto.AccVoucherDetails.Count == 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other),
                                _localizer["Err:EmptyDetail"]);
            }

            var lstAccountSystem = await GetListAccountAsync(dto.OrgCode, dto.Year ); 
            
            var lstAccCase = (await _accCaseService.GetQueryableAsync()).Where(p => p.OrgCode == dto.OrgCode).ToList();
            int i = 1;
            var dataDetail = new List<AccVoucherDetailDto>();
            
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
                bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, item.PartnerCode);
                bool isExistPartnerCl = await _accPartnerService.IsExistCode(dto.OrgCode, item.ClearingPartnerCode);
                bool isExistContract = await _contractService.IsExistCode(dto.OrgCode, item.ContractCode);
                bool isExistContractCl = await _contractService.IsExistCode(dto.OrgCode, item.ClearingContractCode);
                var fProductWork = await _accountingCacheManager.GetFProductWorkByCodeAsync(item.FProductWorkCode, dto.OrgCode);
                var fProductWorkCl = await _accountingCacheManager.GetFProductWorkByCodeAsync(item.ClearingFProductWorkCode, dto.OrgCode);
                bool isExistsSection = await _accSectionService.IsExistCode(dto.OrgCode, item.SectionCode);
                bool isExistsSectionCl = await _accSectionService.IsExistCode(dto.OrgCode, item.ClearingSectionCode);
                bool isExistsWorkPlace = await _workPlaceSevice.IsExistCode(dto.OrgCode, item.WorkPlaceCode);
                bool isExistsWorkPlaceCl = await _workPlaceSevice.IsExistCode(dto.OrgCode, item.ClearingWorkPlaceCode);
                bool isExistsAccCase = await _accCaseService.IsExistCode(dto.OrgCode, item.CaseCode);

                if ((item.DebitAcc ?? "") == "" && (creditAcc?.IsBalanceSheetAcc ?? "") != "C")
                {
                    errorMessage = $"Chưa nhập mã tài khoản Nợ!";
                }
                else if ((item.DebitAcc ?? "") != "" && debitAcc == null)
                {
                    errorMessage = $"Mã tài khoản! {item.DebitAcc} không tồn tại!";
                }
                else if ((item.DebitAcc ?? "") != "" && debitAcc.AccType != "C")
                {
                    errorMessage = $"Mã tài khoản! {item.DebitAcc} là tài khoản mẹ!";
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
                    errorMessage = $"Mã tài khoản! {item.CreditAcc} không tồn tại!";
                }
                else if (creditAcc != null && creditAcc.AccType == "K")
                {
                    errorMessage = $"Mã tài khoản! {item.CreditAcc} là tài khoản mẹ!";
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
                else if ((item.PartnerCode ?? "") != "" && (isExistPartner == false))
                {
                    errorMessage = $"Mã đối tượng {item.PartnerCode} không tồn tại!";
                }
                else if ((item.ClearingPartnerCode ?? "") != "" && (isExistPartnerCl == false))
                {
                    errorMessage = $"Mã đối tượng {item.ClearingPartnerCode} không tồn tại!";
                }
                else if ((item.ContractCode ?? "") != "" && (isExistContract == false))
                {
                    errorMessage = $"Mã hợp đồng {item.ContractCode} không tồn tại!";
                }
                else if ((item.ClearingContractCode ?? "") != "" && (isExistContractCl == false))
                {
                    errorMessage = $"Mã hợp đồng {item.ClearingContractCode} không tồn tại!";
                }
                else if ((item.FProductWorkCode ?? "") != "" && (fProductWork == null))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.FProductWorkCode} không tồn tại!";
                }
                else if ((item.FProductWorkCode ?? "") != "" && ((fProductWork.FPWType ?? "C") != "C"))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.FProductWorkCode} là mã mẹ!";
                }
                else if ((item.ClearingFProductWorkCode ?? "") != "" && (fProductWorkCl == null))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.ClearingFProductWorkCode} không tồn tại!";
                }
                else if ((item.ClearingFProductWorkCode ?? "") != "" && ((fProductWorkCl.FPWType ?? "C") != "C"))
                {
                    errorMessage = $"Mã công trình, sản phẩm {item.ClearingFProductWorkCode} là mã mẹ!";
                }
                else if ((item.SectionCode ?? "") != "" && (isExistsSection == false))
                {
                    errorMessage = $"Mã khoản mục {item.SectionCode} không tồn tại!";
                }
                else if ((item.ClearingSectionCode ?? "") != "" && (isExistsSectionCl == false))
                {
                    errorMessage = $"Mã khoản mục {item.ClearingSectionCode} không tồn tại!";
                }
                else if ((item.WorkPlaceCode ?? "") != "" && (isExistsWorkPlace == false))
                {
                    errorMessage = $"Mã phân xưởng {item.WorkPlaceCode} không tồn tại!";
                }
                else if ((item.ClearingWorkPlaceCode ?? "") != "" && (isExistsWorkPlaceCl == false))
                {
                    errorMessage = $"Mã phân xưởng {item.ClearingWorkPlaceCode} không tồn tại!";
                }
                else if ((item.CaseCode ?? "") != "" && (isExistsAccCase == false))
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
                    throw new AccountingException(ErrorCode.Get(GroupErrorCodes.AccVoucher, ErrorCode.Other), 
                        errorMessage);
                }
            }
        }
        public async Task<List<CrudLedgerDto>> MapLedger(CrudAccVoucherDto dto)
        {
            var ledgers = new List<CrudLedgerDto>();
            var tenantSettings = await this.GetTenantSettings(dto.OrgCode);
            var accountSystems = await _accountingCacheManager.GetAccountSystemsAsync(dto.Year, dto.OrgCode);
            var curencyCode = tenantSettings.Where(p => p.Key == "M_MA_NT0").Select(p => p.Value).FirstOrDefault();
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(dto.VoucherCode, dto.OrgCode);
            var businessCategory = await _businessCategoryService.GetBusinessByCodeAsync(dto.BusinessCode, dto.OrgCode);
            if (voucherCategory.IsSavingLedger == "C" || (businessCategory != null && businessCategory.IsAccVoucher))
            {
                if (dto.AccVoucherDetails != null)
                    for (int i = 0; i < dto.AccVoucherDetails.Count; i++)
                    {
                        int STT = i + 1;
                        var accVoucherDetail = dto.AccVoucherDetails[i];
                        var accountSystemDebitAcc = accountSystems.Where(p => p.AccCode == accVoucherDetail.DebitAcc).FirstOrDefault();
                        var accountSystemCreditAcc = accountSystems.Where(p => p.AccCode == accVoucherDetail.CreditAcc).FirstOrDefault();
                        var ledger = new CrudLedgerDto();
                        ledger.VoucherId = dto.Id;
                        ledger.Year = dto.Year;
                        ledger.OrgCode = dto.OrgCode;
                        ledger.Ord0 = accVoucherDetail.Ord0;
                        ledger.Ord0Extra = accVoucherDetail.Ord0;
                        ledger.DepartmentCode = dto.DepartmentCode;
                        ledger.VoucherCode = dto.VoucherCode;
                        ledger.VoucherGroup = dto.VoucherGroup;
                        ledger.BusinessCode = dto.BusinessCode;
                        ledger.BusinessAcc = dto.BusinessAcc;
                        ledger.VoucherNumber = dto.VoucherNumber;
                        ledger.InvoiceNbr = dto.InvoiceNumber;
                        ledger.RecordingVoucherNumber = "";
                        ledger.VoucherDate = dto.VoucherDate;
                        ledger.PaymentTermsCode = dto.PaymentTermsCode;
                        ledger.CurrencyCode = dto.CurrencyCode;
                        ledger.ExchangeRate = dto.ExchangeRate;
                        ledger.PartnerCode0 = dto.PartnerCode0;
                        ledger.PartnerName0 = dto.PartnerName0;
                        ledger.Representative = dto.Representative;
                        ledger.Address = dto.Address;
                        ledger.Description = dto.Description;
                        ledger.DescriptionE = "";
                        ledger.OriginVoucher = dto.OriginVoucher;
                        ledger.DebitAcc = accVoucherDetail.DebitAcc;
                        ledger.ContractCode = accVoucherDetail.ContractCode;
                        ledger.DebitExchangeRate = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                        ledger.DebitCurrencyCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                        ledger.DebitPartnerCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachPartner == "C") ? accVoucherDetail.PartnerCode : "";
                        ledger.DebitContractCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachContract == "C") ? accVoucherDetail.ContractCode : "";
                        ledger.DebitFProductWorkCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachProductCost == "C") ? accVoucherDetail.FProductWorkCode : "";
                        ledger.DebitSectionCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachAccSection == "C") ? accVoucherDetail.SectionCode : "";
                        ledger.DebitWorkPlaceCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachWorkPlace == "C") ? accVoucherDetail.WorkPlaceCode : "";
                        ledger.DebitAmountCur = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? accVoucherDetail.AmountCur : 0;
                        ledger.CreditAcc = accVoucherDetail.CreditAcc;
                        ledger.CreditExchangeRate = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                        ledger.CreditCurrencyCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                        ledger.CreditPartnerCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachPartner == "C") ? ((accVoucherDetail.ClearingPartnerCode != "" && accVoucherDetail.ClearingPartnerCode != null) ? accVoucherDetail.ClearingPartnerCode : accVoucherDetail.PartnerCode) : "";
                        ledger.CreditContractCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachContract == "C") ? ((accVoucherDetail.ClearingContractCode != "" && accVoucherDetail.ClearingContractCode != null) ? accVoucherDetail.ClearingContractCode : accVoucherDetail.ContractCode) : "";
                        ledger.CreditFProductWorkCode = accVoucherDetail.FProductWorkCode;
                        ledger.CreditSectionCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachAccSection == "C") ? ((accVoucherDetail.ClearingSectionCode != "" && accVoucherDetail.ClearingSectionCode != null) ? accVoucherDetail.ClearingSectionCode : accVoucherDetail.SectionCode) : "";
                        ledger.CreditWorkPlaceCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachWorkPlace == "C") ? ((accVoucherDetail.ClearingWorkPlaceCode != "" && accVoucherDetail.ClearingWorkPlaceCode != null) ? accVoucherDetail.ClearingWorkPlaceCode : accVoucherDetail.WorkPlaceCode) : "";
                        ledger.CreditAmountCur = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accVoucherDetail.AmountCur : 0;
                        ledger.CreditAmount = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accVoucherDetail.AmountCur : 0;
                        ledger.AmountCur = accVoucherDetail.AmountCur;
                        ledger.Amount = accVoucherDetail.Amount;
                        ledger.Note = dto.Description;
                        ledger.NoteE = accVoucherDetail.Note;
                        ledger.FProductWorkCode = accVoucherDetail.FProductWorkCode;
                        ledger.PartnerCode = accVoucherDetail.PartnerCode;
                        ledger.SectionCode = accVoucherDetail.SectionCode;
                        ledger.ClearingFProductWorkCode = accVoucherDetail.ClearingFProductWorkCode;
                        ledger.ClearingPartnerCode = accVoucherDetail.ClearingPartnerCode;
                        ledger.ClearingSectionCode = accVoucherDetail.ClearingSectionCode;
                        ledger.WorkPlaceCode = accVoucherDetail.WorkPlaceCode;
                        ledger.CaseCode = accVoucherDetail.CaseCode;
                        ledger.DebitOrCredit = dto.DebitOrCredit;
                        ledger.Status = dto.Status;
                        ledger.InvoiceDate = (dto.AccTaxDetails != null && dto.AccTaxDetails.Count > 0) ? dto.AccTaxDetails[0].InvoiceDate : null;
                        //Check khử trùng KT
                        ledger.CheckDuplicate = await this.CheckRemoveDuplicateAccVoucher(tenantSettings, dto.VoucherCode, accVoucherDetail.CreditAcc, accVoucherDetail.DebitAcc);
                        ledger.CheckDuplicate0 = ledger.CheckDuplicate;
                        ledgers.Add(ledger);
                    }

                if (dto.AccTaxDetails != null)
                    for (int i = 0; i < dto.AccTaxDetails.Count; i++)
                    {
                        int STT = i + 1;
                        var accTaxDetail = dto.AccTaxDetails[i];
                        var accountSystemDebitAcc = accountSystems.Where(p => p.AccCode == accTaxDetail.DebitAcc).FirstOrDefault();
                        var accountSystemCreditAcc = accountSystems.Where(p => p.AccCode == accTaxDetail.CreditAcc).FirstOrDefault();
                        var taxCategory = await _accountingCacheManager.GetTaxCategoryByCodeAsync(accTaxDetail.TaxCategoryCode, dto.OrgCode);
                        if (taxCategory != null && taxCategory.IsDirect == false)
                        {
                            var Ledger = new CrudLedgerDto();
                            Ledger.VoucherId = dto.Id;
                            Ledger.Year = dto.Year;
                            Ledger.OrgCode = dto.OrgCode;
                            Ledger.Ord0 = accTaxDetail.Ord0;
                            Ledger.Ord0Extra = accTaxDetail.Ord0;
                            Ledger.DepartmentCode = dto.DepartmentCode;
                            Ledger.VoucherCode = dto.VoucherCode;
                            Ledger.VoucherGroup = dto.VoucherGroup;
                            Ledger.BusinessCode = dto.BusinessCode;
                            Ledger.BusinessAcc = dto.BusinessAcc;
                            Ledger.VoucherNumber = dto.VoucherNumber;
                            Ledger.InvoiceNbr = dto.InvoiceNumber;
                            Ledger.RecordingVoucherNumber = "";
                            Ledger.VoucherDate = dto.VoucherDate;
                            Ledger.PaymentTermsCode = dto.PaymentTermsCode;
                            Ledger.CurrencyCode = dto.CurrencyCode;
                            Ledger.ExchangeRate = dto.ExchangeRate;
                            Ledger.PartnerCode0 = dto.PartnerCode0;
                            Ledger.PartnerName0 = dto.PartnerName0;
                            Ledger.Representative = dto.Representative;
                            Ledger.Address = dto.Address;
                            Ledger.Description = dto.Description;
                            Ledger.DescriptionE = accTaxDetail.Note;
                            Ledger.OriginVoucher = dto.OriginVoucher;
                            Ledger.DebitAcc = accTaxDetail.DebitAcc;
                            Ledger.ContractCode = accTaxDetail.ContractCode;
                            Ledger.DebitExchangeRate = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                            Ledger.DebitCurrencyCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                            Ledger.DebitPartnerCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachPartner == "C") ? accTaxDetail.PartnerCode : "";
                            Ledger.DebitContractCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachContract == "C") ? accTaxDetail.ContractCode : "";
                            Ledger.DebitFProductWorkCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachProductCost == "C") ? accTaxDetail.FProductWorkCode : "";
                            Ledger.DebitSectionCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachAccSection == "C") ? accTaxDetail.SectionCode : "";
                            Ledger.DebitWorkPlaceCode = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachWorkPlace == "C") ? accTaxDetail.WorkPlaceCode : "";
                            Ledger.DebitAmountCur = (accountSystemDebitAcc != null && accountSystemDebitAcc.AttachCurrency == "C") ? accTaxDetail.AmountCur : 0;
                            Ledger.CreditAcc = accTaxDetail.CreditAcc;
                            Ledger.CreditExchangeRate = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.ExchangeRate : 1;
                            Ledger.CreditCurrencyCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? dto.CurrencyCode : curencyCode;
                            Ledger.CreditPartnerCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachPartner == "C") ? ((accTaxDetail.ClearingPartnerCode != "" && accTaxDetail.ClearingPartnerCode != null) ? accTaxDetail.ClearingPartnerCode : accTaxDetail.PartnerCode) : "";
                            Ledger.CreditContractCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachContract == "C") ? ((accTaxDetail.ClearingContractCode != "" && accTaxDetail.ClearingContractCode != null) ? accTaxDetail.ClearingContractCode : accTaxDetail.ContractCode) : "";
                            Ledger.CreditFProductWorkCode = accTaxDetail.FProductWorkCode;
                            Ledger.CreditSectionCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachAccSection == "C") ? ((accTaxDetail.ClearingSectionCode != "" && accTaxDetail.ClearingSectionCode != null) ? accTaxDetail.ClearingSectionCode : accTaxDetail.SectionCode) : "";
                            Ledger.CreditWorkPlaceCode = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachWorkPlace == "C") ? ((accTaxDetail.ClearingWorkPlaceCode != "" && accTaxDetail.ClearingWorkPlaceCode != null) ? accTaxDetail.ClearingWorkPlaceCode : accTaxDetail.WorkPlaceCode) : "";
                            Ledger.CreditAmountCur = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accTaxDetail.AmountCur : 0;
                            Ledger.CreditAmount = (accountSystemCreditAcc != null && accountSystemCreditAcc.AttachCurrency == "C") ? accTaxDetail.AmountCur : 0;
                            Ledger.AmountCur = accTaxDetail.AmountCur;
                            Ledger.Amount = accTaxDetail.Amount;
                            Ledger.Note = accTaxDetail.Note;
                            Ledger.NoteE = accTaxDetail.NoteE;
                            Ledger.FProductWorkCode = accTaxDetail.FProductWorkCode;
                            Ledger.PartnerCode = accTaxDetail.PartnerCode;
                            Ledger.SectionCode = accTaxDetail.SectionCode;
                            Ledger.ClearingFProductWorkCode = accTaxDetail.ClearingFProductWorkCode;
                            Ledger.ClearingPartnerCode = accTaxDetail.ClearingPartnerCode;
                            Ledger.ClearingSectionCode = accTaxDetail.ClearingSectionCode;
                            Ledger.WorkPlaceCode = accTaxDetail.WorkPlaceCode;
                            Ledger.CaseCode = accTaxDetail.CaseCode;
                            Ledger.DebitOrCredit = dto.DebitOrCredit;
                            Ledger.InvoiceDate = dto.AccTaxDetails[i].InvoiceDate;
                            Ledger.Status = dto.Status;
                            //Check khử trùng KT
                            Ledger.CheckDuplicate = await this.CheckRemoveDuplicateAccVoucher(tenantSettings,dto.VoucherCode, accTaxDetail.CreditAcc, accTaxDetail.DebitAcc);
                            Ledger.CheckDuplicate0 = Ledger.CheckDuplicate;
                            ledgers.Add(Ledger);
                        }
                    }
            }
            return ledgers;
        }
        public async Task<string> CheckRemoveDuplicateAccVoucher(List<TenantSettingDto> dtos, string voucherCode,
                                                    string creditAcc, string debitAcc)
        {
            var accRemoveDuplicateCash = dtos.Where(p => p.Key == "VHT_TK_KHU_TRUNG_TM")
                                            .Select(p => p.Value).FirstOrDefault();
            var accRemoveDuplicateDeposit = dtos.Where(p => p.Key == "VHT_TK_KHU_TRUNG_TG")
                                            .Select(p => p.Value).FirstOrDefault();
            var RemoveDuplicateCashDeposit = dtos.Where(p => p.Key == "VHT_KHU_TRUNG_TM_TG")
                                            .Select(p => p.Value).FirstOrDefault();
            var RemoveDuplicateType = dtos.Where(p => p.Key == "VHT_CO_KHU_TRUNG")
                                            .Select(p => p.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(accRemoveDuplicateCash))
            {
                accRemoveDuplicateCash = await _defaultTenantSettingService.GetValue("VHT_TK_KHU_TRUNG_TM");
            }
            if (string.IsNullOrEmpty(accRemoveDuplicateDeposit))
            {
                accRemoveDuplicateDeposit = await _defaultTenantSettingService.GetValue("VHT_TK_KHU_TRUNG_TG");
            }
            if (string.IsNullOrEmpty(RemoveDuplicateCashDeposit))
            {
                RemoveDuplicateCashDeposit = await _defaultTenantSettingService.GetValue("VHT_KHU_TRUNG_TM_TG");
            }
            if (RemoveDuplicateType == "K")
            {
                return "";
            }
            if (voucherCode.Substring(0, 2) == "PT" && creditAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "PT" && creditAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3) && "13".Contains(RemoveDuplicateCashDeposit)
                || voucherCode.Substring(0, 3) == "GBC" && creditAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3)
                || voucherCode.Substring(0, 3) == "GBC" && creditAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3) && "23".Contains(RemoveDuplicateCashDeposit)
                        )
            {
                return "N";
            }
            else if (
                   voucherCode.Substring(0, 2) == "PC" && debitAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3)
                || voucherCode.Substring(0, 2) == "PC" && debitAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3) && "13".Contains(RemoveDuplicateCashDeposit)
                || voucherCode.Substring(0, 3) == "GBN" && debitAcc.Substring(0, 3) == accRemoveDuplicateCash.Substring(0, 3)
                || voucherCode.Substring(0, 3) == "GBN" && debitAcc.Substring(0, 3) == accRemoveDuplicateDeposit.Substring(0, 3) && "23".Contains(RemoveDuplicateCashDeposit)
                )
            {
                return "C";
            }
            else
            {
                return "";
            }
        }
        #endregion
        #region Privates
        private async Task<List<TenantSettingDto>> GetTenantSettings(string orgCode)
        {
            var tenantSettings = await _accountingCacheManager.GetTenantSettingAsync(orgCode);
            if (tenantSettings.Count == 0)
            {
                var defaultTenantSettings = await _accountingCacheManager.GetDefaultTenantSettingAsync();
                return defaultTenantSettings.Select(p =>
                {
                    var dto = new TenantSettingDto()
                    {
                        Id = p.Id,
                        Key = p.Key,
                        Ord = p.Ord,
                        OrgCode = orgCode,
                        SettingType = p.SettingType,
                        Type = p.Type,
                        Value = p.Value,
                        TenantId = _currentTenant.Id
                    };
                    return dto;
                }).ToList();
            }
            return tenantSettings;
        }
        private async Task<List<AccountSystemDto>> GetListAccountAsync(string orgCode,int year)
        {

            var lstAccountSystem = await _accountingCacheManager.GetAccountSystemsAsync(year); 
            
            if (lstAccountSystem.Count != 0) { return lstAccountSystem; }
            var yearCategory = await _yearCategoryService.GetByYearAsync(orgCode, year);

            var defaultAccountSystems= await _accountingCacheManager.GetDefaultAccountSystemsAsync(yearCategory.UsingDecision.Value);
            return defaultAccountSystems.Select(p => new AccountSystemDto()
            {
                AccCode = p.AccCode,
                AccName = p.AccName,
                AccNameEn = p.AccNameEn,
                AccNameTemp = p.AccNameTemp,
                AccNameTempE = p.AccNameTempE,
                AccPattern = p.AccPattern,
                AccRank = p.AccRank,
                AccSectionCode = p.AccSectionCode,
                AccType = p.AccType,
                AssetOrEquity = p.AssetOrEquity,
                AttachAccSection = p.AttachAccSection,
                AttachContract = p.AttachContract,
                AttachCurrency = p.AttachCurrency,
                AttachPartner = p.AttachPartner,
                AttachProductCost = p.AttachProductCost,
                AttachVoucher = p.AttachVoucher,
                AttachWorkPlace = p.AttachWorkPlace,
                BankAccountNumber = p.BankAccountNumber,
                BankName = p.BankName,
                CreationTime = p.CreationTime,
                CreatorId = p.CreatorId,
                Id = p.Id,
                IsBalanceSheetAcc = p.IsBalanceSheetAcc,
                LastModificationTime = p.LastModificationTime,
                LastModifierId = p.LastModifierId,
                OrgCode = orgCode,
                ParentAccId = p.ParentAccId,
                Year = p.Year,
                SortPath = p.SortPath,
                Province = p.Province,
                ParentCode = p.ParentCode
            }).ToList();
        }
        #endregion
    }
}
