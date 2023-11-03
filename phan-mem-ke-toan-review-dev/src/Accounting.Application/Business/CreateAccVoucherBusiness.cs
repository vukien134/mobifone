using Accounting.BaseDtos.Customines;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Catgories.VoucherCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Exceptions;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;

namespace Accounting.Business
{
    public class CreateAccVoucherBusiness : BaseBusiness, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly VoucherNumberService _voucherNumberService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly UserService _userService;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IObjectMapper _objectMapper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly AccVoucherService _accVoucherService;
        private readonly AccVoucherDetailService _accVoucherDetailService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly LedgerService _ledgerService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountSystemService _accountSystemService;
        private readonly RefVoucherService _refVoucherService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        #endregion
        #region Ctor
        public CreateAccVoucherBusiness(VoucherNumberService voucherNumberService,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            IUnitOfWorkManager unitOfWorkManager,
                            WebHelper webHelper,
                            IObjectMapper objectMapper,
                            AccountingCacheManager accountingCacheManager,
                            AccVoucherService accVoucherService,
                            AccVoucherDetailService accVoucherDetailService,
                            AccTaxDetailService accTaxDetailService,
                            VoucherPaymentBookService voucherPaymentBookService,
                            LedgerService ledgerService,
                            PartnerGroupService partnerGroupService,
                            AccPartnerService accPartnerService,
                            AccountSystemService accountSystemService,
                            RefVoucherService refVoucherService,
                            VoucherNumberBusiness voucherNumberBusiness,
                            IStringLocalizer<AccountingResource> localizer
                            ) : base(localizer)
        {
            _voucherNumberService = voucherNumberService;
            _userService = userService;
            _voucherCategoryService = voucherCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _objectMapper = objectMapper;
            _accountingCacheManager = accountingCacheManager;
            _accVoucherService = accVoucherService;
            _accVoucherDetailService = accVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _voucherPaymentBookService = voucherPaymentBookService;
            _ledgerService = ledgerService;
            _partnerGroupService = partnerGroupService;
            _accPartnerService = accPartnerService;
            _accountSystemService = accountSystemService;
            _refVoucherService = refVoucherService;
            _voucherNumberBusiness = voucherNumberBusiness;
        }
        #endregion
        #region Methods
        public async Task<AccVoucherDto> CreateAccVoucherAsync(CrudAccVoucherDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            var id = this.GetNewObjectId();
            dto.Id = id;
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = dto.Year != 0 ? dto.Year : _webHelper.GetCurrentYear();
            var unitOfWork = _unitOfWorkManager.Begin();
            if ((dto.BusinessCode ?? "") == "")
            {
                if ((dto.VoucherNumber ?? "") == "")
                {
                    var voucherNumber = await _voucherNumberBusiness.AutoVoucherNumberAsync(dto.VoucherCode, dto.VoucherDate);
                    dto.VoucherNumber = voucherNumber.VoucherNumber;
                }
                else
                {
                    await _voucherNumberBusiness.UpdateVoucherNumberAsync(dto.VoucherCode, dto.VoucherNumber, dto.VoucherDate);
                }
            }
            else
            {
                if ((dto.VoucherNumber ?? "") == "")
                {
                    var voucherNumber = await _voucherNumberBusiness.AutoBusinessVoucherNumberAsync(dto.VoucherCode, dto.BusinessCode, dto.VoucherDate);
                    dto.VoucherNumber = voucherNumber.VoucherNumber;
                }
                else
                {
                    await _voucherNumberBusiness.UpdateBusinessVoucherNumberAsync(dto.VoucherCode, dto.BusinessCode, dto.VoucherNumber, dto.VoucherDate);
                }
            }
            
            if (!string.IsNullOrEmpty(dto.RefVoucher))
            {
                CrudRefVoucherDto crudRefVoucherDto = new CrudRefVoucherDto();
                crudRefVoucherDto.Id = this.GetNewObjectId();
                crudRefVoucherDto.SrcId = dto.RefVoucher;
                crudRefVoucherDto.DestId = id;
                crudRefVoucherDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                var refVoucher = _objectMapper.Map<CrudRefVoucherDto, RefVoucher>(crudRefVoucherDto);
                await _refVoucherService.CreateAsync(refVoucher);

                CrudRefVoucherDto crudRefVoucherDtos = new CrudRefVoucherDto();
                crudRefVoucherDtos.Id = this.GetNewObjectId();
                crudRefVoucherDtos.SrcId = id;
                crudRefVoucherDtos.DestId = dto.RefVoucher;
                crudRefVoucherDtos.OrgCode = _webHelper.GetCurrentOrgUnit();
                var refVouchers = _objectMapper.Map<CrudRefVoucherDto, RefVoucher>(crudRefVoucherDtos);
                await _refVoucherService.CreateAsync(refVouchers);
            }
            dto = this.MapDetailAccVoucher(dto);
            await _accVoucherService.CheckAccVoucher(dto);
            var entity = _objectMapper.Map<CrudAccVoucherDto, AccVoucher>(dto);
            await _accVoucherService.CheckLockVoucher(entity);
            try
            {
                var result = await _accVoucherService.CreateAsync(entity, true);
                //Post sổ cái
                List<CrudLedgerDto> ledgers = await _ledgerService.MapLedger(dto);
                foreach (var ledger in ledgers)
                {
                    ledger.Id = this.GetNewObjectId();
                    var ledgerEntity = _objectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                    ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                }
                await unitOfWork.CompleteAsync();
                return _objectMapper.Map<AccVoucher, AccVoucherDto>(result);
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task UpdateAccVoucherAsync(string id, CrudAccVoucherDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto = this.MapDetailAccVoucher(dto);
            await _accVoucherService.CheckAccVoucher(dto);
            var entity = await _accVoucherService.GetAsync(id);
            await _accVoucherService.CheckLockVoucher(entity);
            _objectMapper.Map(dto, entity);
            try
            {
                var accVoucherDetails = await _accVoucherDetailService.GetByAccVoucherIdAsync(id);
                var accTaxDetails = await _accTaxDetailService.GetByAccVoucherIdAsync(id);
                var ledgers = await _ledgerService.GetByAccVoucherIdAsync(id);



                using var unitOfWork = _unitOfWorkManager.Begin();
                if (accVoucherDetails != null)
                {
                    await _accVoucherDetailService.DeleteManyAsync(accVoucherDetails);
                }
                if (accTaxDetails != null)
                {
                    await _accTaxDetailService.DeleteManyAsync(accTaxDetails);
                }
                if (ledgers != null)
                {
                    await _ledgerService.DeleteManyAsync(ledgers);
                }
                await _accVoucherService.UpdateAsync(entity);
                // post sổ cái
                List<CrudLedgerDto> ledgersUpdate = await _ledgerService.MapLedger(dto);
                foreach (var ledger in ledgersUpdate)
                {
                    ledger.Id = this.GetNewObjectId();
                    var ledgerEntity = _objectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                    ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity);
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }

        public async Task DeleteAccVoucherAsync(string id)
        {
            var entity = await _accVoucherService.GetAsync(id);
            await _accVoucherService.CheckLockVoucher(entity);
            try
            {
                var refVoucher = await _refVoucherService.GetQueryableAsync();
                var lstrefVoucher = refVoucher.Where(p => p.SrcId == id);
                if (lstrefVoucher.ToList().Count > 0)
                {
                    await _refVoucherService.DeleteManyAsync(lstrefVoucher.ToList(), true);
                }
                var lstrefVouchers = refVoucher.Where(p => p.DestId == id);
                if (lstrefVouchers.ToList().Count > 0)
                {
                    await _refVoucherService.DeleteManyAsync(lstrefVouchers.ToList(), true);
                }
                var ledgers = await _ledgerService.GetByAccVoucherIdAsync(id);
                var accVoucherDetails = await _accVoucherDetailService.GetByAccVoucherIdAsync(id);
                var accTaxDetails = await _accTaxDetailService.GetByAccVoucherIdAsync(id);
                var voucherPaymentBooks = await _voucherPaymentBookService.GetByAccVoucherIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (ledgers != null)
                {
                    await _ledgerService.DeleteManyAsync(ledgers, true);
                }
                if (accVoucherDetails != null)
                {
                    await _accVoucherDetailService.DeleteManyAsync(accVoucherDetails, true);
                }
                if (accTaxDetails != null)
                {
                    await _accTaxDetailService.DeleteManyAsync(accTaxDetails, true);
                }
                if (voucherPaymentBooks != null)
                {
                    await _voucherPaymentBookService.DeleteManyAsync(voucherPaymentBooks, true);
                }
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
            await _accVoucherService.DeleteAsync(id, true);
        }
        #endregion
        #region Privates
        public CrudAccVoucherDto MapDetailAccVoucher(CrudAccVoucherDto dto)
        {
            if (dto.AccVoucherDetails != null)
                for (int i = 0; i < dto.AccVoucherDetails.Count; i++)
                {
                    int ord = i + 1;
                    var accVoucherDetail = dto.AccVoucherDetails[i];
                    accVoucherDetail.Id = this.GetNewObjectId();
                    accVoucherDetail.AccVoucherId = dto.Id;
                    accVoucherDetail.OrgCode = dto.OrgCode;
                    accVoucherDetail.Year = dto.Year;
                    accVoucherDetail.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                }
            if (dto.AccTaxDetails != null)
                dto.InvoiceNumber = dto.AccTaxDetails.Max(p => p.InvoiceNumber);
            for (int i = 0; i < dto.AccTaxDetails.Count; i++)
            {
                int ord = i + 1;
                var accTaxDetail = dto.AccTaxDetails[i];
                accTaxDetail.Id = this.GetNewObjectId();
                accTaxDetail.AccVoucherId = dto.Id;
                accTaxDetail.OrgCode = dto.OrgCode;
                accTaxDetail.VoucherDate = dto.VoucherDate;
                accTaxDetail.VoucherCode = dto.VoucherCode;
                accTaxDetail.Year = dto.Year;
                accTaxDetail.Ord0 = "Z" + ord.ToString().PadLeft(9, '0');
            }
            return dto;
        }
        #endregion
    }
}
