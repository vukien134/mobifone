using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.Partners;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.Partners;
using Accounting.Catgories.VoucherCategories;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.DomainServices.Windows;
using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace Accounting.Vouchers.AccVouchers
{
    public class AccVoucherAppService : AccountingAppService, IAccVoucherAppService
    {
        #region Field
        private readonly AccVoucherService _accVoucherService;
        private readonly AccVoucherDetailService _accVoucherDetailService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly LedgerService _ledgerService;
        private readonly UserService _userService;
        private readonly ExcelService _excelService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountSystemService _accountSystemService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;
        private readonly WindowService _windowService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly MenuAccountingService _menuAccountingService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly RefVoucherService _refVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly AccVoucherBusiness _accVoucherBusiness;
        private readonly ICurrentUser _currentUser;
        private readonly AccountingCacheManager _accountingCacheManager;
        #endregion
        #region Ctor
        public AccVoucherAppService(AccVoucherService accVoucherService,
                                AccVoucherDetailService accVoucherDetailService,
                                AccTaxDetailService accTaxDetailService,
                                VoucherPaymentBookService voucherPaymentBookService,
                                LedgerService ledgerService,
                                UserService userService,
                                ExcelService excelService,
                                PartnerGroupService partnerGroupService,
                                AccPartnerService accPartnerService,
                                AccountSystemService accountSystemService,
                                VoucherNumberBusiness voucherNumberBusiness,
                                VoucherCategoryService voucherCategoryService,
                                IUnitOfWorkManager unitOfWorkManager,
                                WindowService windowService,
                                WebHelper webHelper,
                                TenantSettingService tenantSettingService,
                                MenuAccountingService menuAccountingService,
                                IStringLocalizer<AccountingResource> localizer,
                                RefVoucherService refVoucherService,
                                ProductVoucherService productVoucherService,
                                LicenseBusiness licenseBusiness,
                                AccVoucherBusiness accVoucherBusiness,
                                ICurrentUser currentUser,
                                AccountingCacheManager accountingCacheManager
            )
        {
            _accVoucherService = accVoucherService;
            _accVoucherDetailService = accVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _voucherPaymentBookService = voucherPaymentBookService;
            _ledgerService = ledgerService;
            _userService = userService;
            _excelService = excelService;
            _partnerGroupService = partnerGroupService;
            _accPartnerService = accPartnerService;
            _accountSystemService = accountSystemService;
            _voucherNumberBusiness = voucherNumberBusiness;
            _voucherCategoryService = voucherCategoryService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _windowService = windowService;
            _tenantSettingService = tenantSettingService;
            _menuAccountingService = menuAccountingService;
            _localizer = localizer;
            _refVoucherService = refVoucherService;
            _productVoucherService = productVoucherService;
            _licenseBusiness = licenseBusiness;
            _accVoucherBusiness = accVoucherBusiness;
            _currentUser = currentUser;
            _accountingCacheManager = accountingCacheManager;
        }
        #endregion
        public async Task<AccVoucherDto> GetByIdAsync(string accVoucherId)
        {
            var result = new AccVoucherDto();
            var query = await _accVoucherService.GetQueryableAsync();
            query = query.Where(p => p.Id == accVoucherId);
            result = query.Select(p => ObjectMapper.Map<AccVoucher, AccVoucherDto>(p)).FirstOrDefault();
            return result;
        }

        public async Task<AccVoucherDto> CreateAsync(CrudAccVoucherDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionCreate);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
            await _licenseBusiness.ValidLicAsync();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            var id = this.GetNewObjectId();
            dto.Id = id;
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = dto.Year != 0 ? dto.Year : _webHelper.GetCurrentYear();
            dto.Status = await this.GetVoucherStatus();
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

            dto = this.MapDetail(dto);
            await _accVoucherBusiness.CheckAccVoucher(dto);
            var entity = ObjectMapper.Map<CrudAccVoucherDto, AccVoucher>(dto);
            await _accVoucherBusiness.CheckLockVoucher(dto);
            try
            {
                var result = await _accVoucherService.CreateAsync(entity, true);
                //Post sổ cái
                var ledgers = await _accVoucherBusiness.MapLedger(dto);
                foreach (var ledger in ledgers)
                {
                    ledger.Id = this.GetNewObjectId();
                    var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                    ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                }
                await InsertRefVoucher(dto);
                await unitOfWork.CompleteAsync();
                return ObjectMapper.Map<AccVoucher, AccVoucherDto>(result);
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
        }

        public async Task<ResultDto> CreateListAsync(List<CrudAccVoucherDto> lstDto)
        {
            await _licenseBusiness.CheckExpired();
            var unitOfWork = _unitOfWorkManager.Begin();
            try
            {
                foreach (var item in lstDto)
                {
                    var dto = item;
                    dto.CreatorName = await _userService.GetCurrentUserNameAsync();
                    dto.Id = this.GetNewObjectId();
                    dto.OrgCode = _webHelper.GetCurrentOrgUnit();
                    dto.Year = dto.Year != 0 ? dto.Year : _webHelper.GetCurrentYear();
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
                    dto = this.MapDetail(dto);
                    await _accVoucherBusiness.CheckAccVoucher(dto);
                    var entity = ObjectMapper.Map<CrudAccVoucherDto, AccVoucher>(dto);
                    try
                    {
                        var result = await _accVoucherService.CreateAsync(entity);
                        //Post sổ cái
                        List<CrudLedgerDto> ledgers = await _accVoucherBusiness.MapLedger(dto);
                        foreach (var ledger in ledgers)
                        {
                            ledger.Id = this.GetNewObjectId();
                            var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                            ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity);
                        }
                        await unitOfWork.CompleteAsync();
                    }
                    catch (Exception)
                    {
                        await unitOfWork.RollbackAsync();
                        throw;
                    }
                }
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await unitOfWork.RollbackAsync();
                throw;
            }
            var res = new ResultDto();
            res.Ok = true;
            return res;
        }

        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _accVoucherService.GetAsync(id);
            await _accVoucherBusiness.CheckLockVoucher(entity);
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionDelete);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            try
            {
                var refVoucher = await _refVoucherService.GetQueryableAsync();
                var lstrefVoucher = refVoucher.Where(p => p.SrcId == id);
                if (lstrefVoucher.ToList().Count > 0)
                {
                    await _refVoucherService.DeleteManyAsync(lstrefVoucher.ToList());
                }
                var lstrefVouchers = refVoucher.Where(p => p.DestId == id);
                if (lstrefVouchers.ToList().Count > 0)
                {
                    await _refVoucherService.DeleteManyAsync(lstrefVouchers.ToList());
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

        public async Task<ResultDto> PostDeleteListAsync(ListDeleteDto dto)
        {
            await _licenseBusiness.CheckExpired();
            foreach (var item in dto.ListId)
            {
                await DeleteAsync(item);
            }
            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Thực hiện thành công";
            return res;
        }

        public async Task<PageResultDto<AccVoucherDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }

        public async Task<PageResultDto<AccVoucherDto>> GetListAsync(PageRequestDto dto)
        {
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionView);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            var result = new PageResultDto<AccVoucherDto>();
            var query = await Filter(dto);
            var querysort = query.OrderByDescending(p => p.VoucherDate).ThenByDescending(p => p.VoucherNumber).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccVoucher, AccVoucherDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<PageResultDto<AccVoucherDto>> PostFilterAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<AccVoucherDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.VoucherNumber).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<AccVoucher, AccVoucherDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        public async Task<List<AccVoucherDetailDto>> GetAccVoucherDetailAsync(string accVoucherId)
        {
            var result = new List<AccVoucherDetailDto>();
            var query = await _accVoucherDetailService.GetQueryableAsync();
            query = query.Where(p => p.AccVoucherId == accVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AccVoucherDetail, AccVoucherDetailDto>(p)).ToList();
            return result;
        }

        public async Task<List<AccTaxDetailDto>> GetAccTaxDetailAsync(string accVoucherId)
        {
            var result = new List<AccTaxDetailDto>();
            var query = await _accTaxDetailService.GetQueryableAsync();
            query = query.Where(p => p.AccVoucherId == accVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AccTaxDetail, AccTaxDetailDto>(p)).ToList();
            return result;
        }

        public async Task UpdateAsync(string id, CrudAccVoucherDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionUpdate);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            dto = this.MapDetail(dto);
            await _accVoucherBusiness.CheckAccVoucher(dto);
            var entity = await _accVoucherService.GetAsync(id);
            await _accVoucherBusiness.CheckLockVoucher(dto);
            ObjectMapper.Map(dto, entity);
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
                List<CrudLedgerDto> ledgersUpdate = await _accVoucherBusiness.MapLedger(dto);
                foreach (var ledger in ledgersUpdate)
                {
                    ledger.Id = this.GetNewObjectId();
                    var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
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
        public async Task<List<RefVoucherComboItemDto>> GetRefVoucherAsync(string refId)
        {
            var result = new List<RefVoucherComboItemDto>();
            var refVouchers = await _refVoucherService.GetByRefId(refId);
            if (refVouchers.Count == 0) return result;

            foreach(var item in refVouchers)
            {
                var accVoucher = await _accVoucherService.FindAsync(item.SrcId);
                if (accVoucher != null)
                {
                    result.Add(new RefVoucherComboItemDto()
                    {
                        Id = accVoucher.Id,
                        Value = accVoucher.VoucherNumber,
                        VoucherNumber = accVoucher.VoucherNumber,
                        VoucherCode = accVoucher.VoucherCode,
                        DataId = accVoucher.Id
                    });
                }
                var productVoucher = await _productVoucherService.FindAsync(item.SrcId);
                if (productVoucher != null)
                {
                    result.Add(new RefVoucherComboItemDto()
                    {
                        Id = productVoucher.Id,
                        Value = productVoucher.VoucherNumber,
                        VoucherNumber = productVoucher.VoucherNumber,
                        VoucherCode = productVoucher.VoucherCode,
                        DataId = productVoucher.Id
                    });
                }
            }
            return result;
        }
        #region Private
        private async Task<IQueryable<AccVoucher>> Filter(PageRequestDto dto)
        {
            var queryable = await _accVoucherService.GetQueryableAsync();
            var accVoucherDetails = await _accVoucherDetailService.GetQueryableAsync();
            var window = await _windowService.GetByIdAsync(dto.WindowId);
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && p.Year == _webHelper.GetCurrentYear()
                                        && p.VoucherCode == (window == null ? "" : window.VoucherCode)
                                        );
            var isViewByUserNew = await this.IsViewByUserNew();
            if (isViewByUserNew)
            {
                queryable = queryable.Where(p => p.CreatorId == _currentUser.Id);
            }
            queryable = await this.GetFilterRows(queryable, dto);
            if (dto.FilterAdvanced == null)
            {
                return queryable;
            }
            var voucherCode = dto.FilterAdvanced.Where(p => p.ColumnName == "voucherCode").FirstOrDefault()?.Value;
            var currencyCode = dto.FilterAdvanced.Where(p => p.ColumnName == "currencyCode").FirstOrDefault()?.Value;
            var fromDate = dto.FilterAdvanced.Where(p => p.ColumnName == "fromDate").FirstOrDefault()?.Value;
            var toDate = dto.FilterAdvanced.Where(p => p.ColumnName == "toDate").FirstOrDefault()?.Value;
            var beginVoucherNumber = dto.FilterAdvanced.Where(p => p.ColumnName == "beginVoucherNumber").FirstOrDefault()?.Value;
            var endVoucherNumber = dto.FilterAdvanced.Where(p => p.ColumnName == "endVoucherNumber").FirstOrDefault()?.Value;
            var beginAmount = dto.FilterAdvanced.Where(p => p.ColumnName == "beginAmount").FirstOrDefault()?.Value;
            var endAmount = dto.FilterAdvanced.Where(p => p.ColumnName == "endAmount").FirstOrDefault()?.Value;
            var beginAmountCur = dto.FilterAdvanced.Where(p => p.ColumnName == "beginAmountCur").FirstOrDefault()?.Value;
            var endAmountCur = dto.FilterAdvanced.Where(p => p.ColumnName == "endAmountCur").FirstOrDefault()?.Value;
            var partnerCode0 = dto.FilterAdvanced.Where(p => p.ColumnName == "partnerCode0").FirstOrDefault()?.Value;
            var parentPartnerCode = dto.FilterAdvanced.Where(p => p.ColumnName == "parentPartnerCode").FirstOrDefault()?.Value;
            var fProductWorkCode = dto.FilterAdvanced.Where(p => p.ColumnName == "fProductWorkCode").FirstOrDefault()?.Value;
            var sectionCode = dto.FilterAdvanced.Where(p => p.ColumnName == "sectionCode").FirstOrDefault()?.Value;
            var caseCode = dto.FilterAdvanced.Where(p => p.ColumnName == "caseCode").FirstOrDefault()?.Value;
            var accCode = dto.FilterAdvanced.Where(p => p.ColumnName == "accCode").FirstOrDefault()?.Value;
            var beginInvoiceNumber = dto.FilterAdvanced.Where(p => p.ColumnName == "beginInvoiceNumber").FirstOrDefault()?.Value;
            var endInvoiceNumber = dto.FilterAdvanced.Where(p => p.ColumnName == "endInvoiceNumber").FirstOrDefault()?.Value;
            var type = dto.FilterAdvanced.Where(p => p.ColumnName == "type").FirstOrDefault()?.Value;
            var creationTime = dto.FilterAdvanced.Where(p => p.ColumnName == "creationTime").FirstOrDefault()?.Value;
            var accReciprocal = dto.FilterAdvanced.Where(p => p.ColumnName == "accReciprocal").FirstOrDefault()?.Value;
            var status = dto.FilterAdvanced.Where(p => p.ColumnName == "status").FirstOrDefault()?.Value;
            // set điều kiện
            // Theo mã chứng từ
            if (voucherCode != null && voucherCode.ToString() != "")
            {
                queryable = queryable.Where(p => p.VoucherCode == voucherCode.ToString());
            }
            // Mã NT
            if (currencyCode != null && currencyCode.ToString() != "*" && currencyCode.ToString() != "")
            {
                queryable = queryable.Where(p => p.CurrencyCode == currencyCode.ToString());
            }
            // Từ ngày
            if (fromDate != null && fromDate.ToString() != "")
            {
                queryable = queryable.Where(p => p.VoucherDate >= DateTime.Parse(fromDate.ToString()));
            }
            // Đến ngày
            if (toDate != null && toDate.ToString() != "")
            {
                queryable = queryable.Where(p => p.VoucherDate <= DateTime.Parse(toDate.ToString()));
            }
            // Từ số chứng từ
            if (beginVoucherNumber != null && beginVoucherNumber.ToString() != "")
            {
                queryable = queryable.Where(p => String.Compare(p.VoucherNumber, beginVoucherNumber.ToString()) >= 0);
            }
            // Đến số chứng từ
            if (endVoucherNumber != null && endVoucherNumber.ToString() != "")
            {
                queryable = queryable.Where(p => String.Compare(p.VoucherNumber, endVoucherNumber.ToString()) <= 0);
            }
            // Từ tiền
            if (beginAmount != null && Decimal.Parse(beginAmount.ToString()) != 0)
            {
                queryable = queryable.Where(p => p.TotalAmount >= Decimal.Parse(beginAmount.ToString()));
            }
            // Đến tiền
            if (endAmount != null && Decimal.Parse(endAmount.ToString()) != 0)
            {
                queryable = queryable.Where(p => p.TotalAmount <= Decimal.Parse(endAmount.ToString()));
            }
            // Từ tiền NT
            if (beginAmountCur != null && Decimal.Parse(beginAmountCur.ToString()) != 0)
            {
                queryable = queryable.Where(p => p.TotalAmountCur >= Decimal.Parse(beginAmountCur.ToString()));
            }
            // Đến tiền NT
            if (endAmountCur != null && Decimal.Parse(endAmountCur.ToString()) != 0)
            {
                queryable = queryable.Where(p => p.TotalAmountCur <= Decimal.Parse(endAmountCur.ToString()));
            }
            // Mã đối tượng
            if (partnerCode0 != null && partnerCode0.ToString() != "")
            {
                queryable = queryable.Where(p => p.PartnerCode0 == partnerCode0.ToString());
            }
            // Mã công trình, sản phẩm
            if (fProductWorkCode != null && fProductWorkCode.ToString() != "")
            {
                queryable = queryable.Where(p => p.AccVoucherDetails.Any(e => e.FProductWorkCode == fProductWorkCode.ToString()));
            }
            // Mã khoản mục
            if (sectionCode != null && sectionCode.ToString() != "")
            {
                queryable = queryable.Where(p => p.AccVoucherDetails.Any(e => e.SectionCode == sectionCode.ToString()));
            }
            // Mã vụ việc
            if (caseCode != null && caseCode.ToString() != "")
            {
                queryable = queryable.Where(p => p.AccVoucherDetails.Any(e => e.CaseCode == caseCode.ToString()));
            }
            // Tài khoản
            if (accCode != null && accCode.ToString() != "")
            {
                var accountSystems = await _accountSystemService.GetQueryableAsync();
                var dataAccountSystem = accountSystems.Where(p => p.AccCode == accCode.ToString() && p.OrgCode == _webHelper.GetCurrentOrgUnit()).Select(p => ObjectMapper.Map<AccountSystem, AccountSystemDto>(p)).ToList();
                var dataAccountSystemChild = GetAccoutSystemCodeChild(dataAccountSystem).Result;
                while (dataAccountSystemChild.Count() > 0)
                {
                    foreach (var item in dataAccountSystemChild)
                    {
                        dataAccountSystem.Add(item);
                    }
                    dataAccountSystemChild = GetAccoutSystemCodeChild(dataAccountSystemChild).Result;
                }
                queryable = from q in queryable
                            join avdt in accVoucherDetails on q.Id equals avdt.AccVoucherId
                            where (from dtas in dataAccountSystem select dtas.AccCode).Contains(avdt.DebitAcc)
                               || (from dtas in dataAccountSystem select dtas.AccCode).Contains(avdt.CreditAcc)
                            select q;
            }
            // Từ số hóa đơn
            if (beginInvoiceNumber != null && beginInvoiceNumber.ToString() != "")
            {
                queryable = queryable.Where(p => decimal.Parse(p.InvoiceNumber) >= decimal.Parse(beginInvoiceNumber.ToString()));
            }
            // Đến số hóa đơn
            if (endInvoiceNumber != null && endInvoiceNumber.ToString() != "")
            {
                queryable = queryable.Where(p => decimal.Parse(p.InvoiceNumber) <= decimal.Parse(endInvoiceNumber.ToString()));
            }
            if (creationTime != null && creationTime.ToString() != "")
            {
                queryable = queryable.Where(p => p.CreationTime == DateTime.Parse(creationTime.ToString()));
            }
            // Kiểu tài khoản và tài khoản đối ứng
            var dataAccReciprocals = new List<AccountSystemDto>();
            if (accReciprocal != null && accReciprocal.ToString() != "")
            {
                var accountSystems = await _accountSystemService.GetQueryableAsync();
                dataAccReciprocals = accountSystems.Where(p => p.AccCode == accReciprocal.ToString() && p.OrgCode == _webHelper.GetCurrentOrgUnit()).Select(p => ObjectMapper.Map<AccountSystem, AccountSystemDto>(p)).ToList();
                var dataAccReciprocalChild = GetAccoutSystemCodeChild(dataAccReciprocals).Result;
                while (dataAccReciprocalChild.Count() > 0)
                {
                    foreach (var item in dataAccReciprocalChild)
                    {
                        dataAccReciprocals.Add(item);
                    }
                    dataAccReciprocalChild = GetAccoutSystemCodeChild(dataAccReciprocalChild).Result;
                }
            }
            if (type != null && type.ToString() == "*" && accReciprocal != null && accReciprocal.ToString() != "")
            {
                queryable = from q in queryable
                            join avdt in accVoucherDetails on q.Id equals avdt.AccVoucherId
                            where (from dtar in dataAccReciprocals select dtar.AccCode).Contains(avdt.DebitAcc)
                               || (from dtar in dataAccReciprocals select dtar.AccCode).Contains(avdt.CreditAcc)
                            select q;
            }
            else if (type != null && type.ToString() == "N" && accReciprocal != null && accReciprocal.ToString() != "")
            {
                queryable = from q in queryable
                            join avdt in accVoucherDetails on q.Id equals avdt.AccVoucherId
                            where (from dtar in dataAccReciprocals select dtar.AccCode).Contains(avdt.DebitAcc)
                            select q;
            }
            else if (type != null && type.ToString() == "C" && accReciprocal != null && accReciprocal.ToString() != "")
            {
                queryable = from q in queryable
                            join avdt in accVoucherDetails on q.Id equals avdt.AccVoucherId
                            where (from dtar in dataAccReciprocals select dtar.AccCode).Contains(avdt.CreditAcc)
                            select q;
            }

            // Trạng thái
            if (status != null && status.ToString() != "")
            {
                queryable = queryable.Where(p => p.Status == status.ToString());
            }
            // Nhóm đối tượng
            if (parentPartnerCode != null && parentPartnerCode.ToString() != "")
            {
                var partnerGroups = await _partnerGroupService.GetQueryableAsync();
                var accPartners = await _accPartnerService.GetQueryableAsync();
                var dataPartnerGroup = partnerGroups.Where(p => p.Code == parentPartnerCode.ToString()).Select(p => ObjectMapper.Map<PartnerGroup, PartnerGroupDto>(p)).ToList();
                var dataPartnerGroupNew = GetPartnerGroupCodeChild(dataPartnerGroup).Result;
                while (dataPartnerGroupNew.Count() > 0)
                {
                    foreach (var item in dataPartnerGroupNew)
                    {
                        dataPartnerGroup.Add(item);
                    }
                    dataPartnerGroupNew = GetPartnerGroupCodeChild(dataPartnerGroupNew).Result;
                }
                queryable = from q in queryable
                            join ap in accPartners on q.PartnerCode0 equals ap.Code
                            where q.OrgCode == ap.OrgCode && (from dtpg in dataPartnerGroup
                                                              select dtpg.Id).Contains(ap.PartnerGroupId)
                            select q;
            }
            // Khóa CT
            return queryable;
        }

        public CrudAccVoucherDto MapDetail(CrudAccVoucherDto dto)
        {
            if (dto.AccVoucherDetails != null)
            {
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
            }
                
            if (dto.AccTaxDetails != null)
            {
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
            }    
            return dto;
        }

        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.ValidLicAsync();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            var lstImport = await _excelService.ImportFileToList<ExcelAccVoucherDto>(bytes, dto.WindowId);
            lstImport = lstImport.Where(p => p.VoucherNumber != null && p.VoucherNumber != "" && p.VoucherCode != null && p.VoucherCode != "").ToList();           
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var accVouchers = from p in lstImport
                              group new { p } by new
                              {
                                  p.VoucherNumber,
                                  p.VoucherCode
                              } into gr
                              select new CrudAccVoucherDto
                              {
                                  Id = this.GetNewObjectId(),
                                  OrgCode = _webHelper.GetCurrentOrgUnit(),
                                  Year = _webHelper.GetCurrentYear(),
                                  DepartmentCode = gr.Max(a => a.p.DepartmentCode),
                                  VoucherCode = gr.Key.VoucherCode,
                                  VoucherNumber = gr.Key.VoucherNumber,
                                  InvoiceNumber = gr.Max(a => a.p.InvoiceNumber),
                                  VoucherDate = gr.Max(a => a.p.VoucherDate),
                                  BusinessCode = gr.Max(a => a.p.BusinessCode),
                                  BankNumber = gr.Max(a => a.p.BankNumber),
                                  BankName = gr.Max(a => a.p.BankName),
                                  PartnerCode0 = gr.Max(a => a.p.PartnerCode),
                                  PartnerName0 = gr.Max(a => a.p.PartnerName),
                                  Representative = gr.Max(a => a.p.Representative),
                                  Address = gr.Max(a => a.p.Address),
                                  Description = gr.Max(a => a.p.Description),
                                  OriginVoucher = gr.Max(a => a.p.OriginVoucher),
                                  CurrencyCode = gr.Max(a => a.p.CurrencyCode),
                                  ExchangeRate = gr.Max(a => a.p.ExchangeRate),
                                  TotalAmountWithoutVatCur = gr.Sum(a => a.p.AmountCur),
                                  TotalAmountWithoutVat = gr.Sum(a => a.p.Amount),
                                  TotalAmountVatCur = gr.Sum(a => a.p.TotalTaxAmountCur),
                                  TotalAmountVat = gr.Sum(a => a.p.TotalTaxAmount),
                                  TotalAmountCur = gr.Sum(a => a.p.AmountCur) + gr.Sum(a => a.p.TotalTaxAmountCur),
                                  TotalAmount = gr.Sum(a => a.p.Amount) + gr.Sum(a => a.p.TotalTaxAmount),
                                  Status = "1"
                              };
            var lstAccVouchers = accVouchers.ToList();
            var unitOfWork = _unitOfWorkManager.Begin();
            foreach (var accVoucher in lstAccVouchers)
            {
                var dataVoucherCategory = await _voucherCategoryService.GetByVoucherCategoryAsync(_webHelper.GetCurrentOrgUnit());
                if(dataVoucherCategory.Count() == 0)
                {
                    await InsertDefaultAsync();
                }
                var voucherCategory = await _voucherCategoryService.GetByCodeAsync(accVoucher.VoucherCode, accVoucher.OrgCode);
                var accPartner = await _accPartnerService.GetAccPartnerByCodeAsync(accVoucher.PartnerCode0, accVoucher.OrgCode); 
                var accPartners = await _accPartnerService.GetQueryableAsync();
                accPartners = accPartners.Where(p => p.OrgCode == accVoucher.OrgCode);
                if (voucherCategory == null)
                {
                    throw new Exception("Mã chứng từ " + accVoucher.VoucherCode + " không tồn tại!"); 
                }
                else if ((accVoucher.PartnerCode0 ?? "") != "" && accPartner == null)
                {
                    throw new Exception("Mã đối tượng " + accVoucher.PartnerCode0 + " không tồn tại!");
                }
                accVoucher.VoucherGroup = voucherCategory.VoucherGroup;
                if(accPartner != null)
                {
                    accVoucher.PartnerName0 = accPartner.Name;
                    accVoucher.Address = accPartner.Address;
                }             
                var accVoucherDetails = from p in lstImport
                                        where p.VoucherNumber == accVoucher.VoucherNumber
                                        select new CrudAccVoucherDetailDto
                                        {
                                            Id = this.GetNewObjectId(),
                                            AccVoucherId = accVoucher.Id,
                                            Year = accVoucher.Year,
                                            CreditAcc = p.CreditAcc,
                                            DebitAcc = p.DebitAcc,
                                            Note = accVoucher.Description,
                                            PartnerCode = p.PartnerCode,
                                            PartnerName = accVoucher.PartnerName0,
                                            ContractCode = p.ContractCode,
                                            SectionCode = p.SectionCode,
                                            WorkPlaceCode = p.WorkPlaceCode,
                                            FProductWorkCode = p.FProductWorkCode,                                          
                                            ClearingPartnerCode = p.ClearingPartnerCode,
                                            AmountCur = p.AmountCur,
                                            Amount = p.Amount,
                                            CaseCode = p.CaseCode
                                        };
                var accTaxDetails = from p in lstImport
                                    join ac in accPartners on p.PartnerCode equals ac.Code into ajp
                                    from ap in ajp.DefaultIfEmpty()
                                    where p.VoucherNumber == accVoucher.VoucherNumber && p.TaxCategoryCode != null && p.TaxCategoryCode != ""
                                    group new { p, ap } by new
                                    {
                                        p.TaxCategoryCode
                                    } into gr
                                    select new CrudAccTaxDetailDto
                                    {
                                        Id = this.GetNewObjectId(),
                                        AccVoucherId = accVoucher.Id,
                                        Year = accVoucher.Year,
                                        VoucherCode = accVoucher.VoucherCode,
                                        DepartmentCode = gr.Max(a => a.p.DepartmentCode),
                                        VoucherNumber = gr.Max(a => a.p.VoucherNumber),
                                        VoucherDate = gr.Max(a => a.p.VoucherDate),
                                        TaxCode = gr.Max(a => a.p.TaxCode),
                                        InvoiceDate = gr.Max(a => a.p.InvoiceDate),
                                        InvoiceGroup = "1",
                                        InvoiceSymbol = gr.Max(a => a.p.InvoiceSymbol),
                                        InvoiceNumber = gr.Max(a => a.p.InvoiceNumber),
                                        DebitAcc = gr.Max(a => a.p.DebitAccTax),
                                        PartnerCode = gr.Max(a => a.p.PartnerCode),
                                        ContractCode = gr.Max(a => a.p.ContractCode),
                                        SectionCode = gr.Max(a => a.p.SectionCode),
                                        WorkPlaceCode = gr.Max(a => a.p.WorkPlaceCode),
                                        FProductWorkCode = gr.Max(a => a.p.FProductWorkCode),
                                        CreditAcc = gr.Max(a => a.p.CreditAccTax),
                                        ClearingPartnerCode = gr.Max(a => a.p.ClearingPartnerCode),
                                        PartnerName = gr.Max(a => a.ap.Name),
                                        Address = gr.Max(a => a.p.Address),
                                        TaxCategoryCode = gr.Key.TaxCategoryCode,
                                        ProductName = gr.Max(a => a.p.ProductName),
                                        AmountWithoutVatCur = gr.Max(a => a.p.AmountCur),
                                        AmountWithoutVat = gr.Max(a => a.p.Amount),
                                        VatPercentage = gr.Max(a => a.p.VatPercentage),
                                        AmountCur = gr.Sum(a => a.p.TotalTaxAmountCur),
                                        Amount = gr.Sum(a => a.p.TotalTaxAmount),
                                        CaseCode = gr.Max(a => a.p.CaseCode),
                                        TotalAmountCur = gr.Sum(a => a.p.TotalTaxAmountCur) + gr.Sum(a => a.p.AmountCur),
                                        TotalAmount = gr.Sum(a => a.p.TotalTaxAmount) + gr.Sum(a => a.p.Amount)
                                    };
                accVoucher.AccVoucherDetails = accVoucherDetails.ToList();
                accVoucher.AccTaxDetails = accTaxDetails.ToList();
                await this.CreateAsync(accVoucher);
            }
            await unitOfWork.CompleteAsync();
            return new UploadFileResponseDto() { Ok = true };
        }

        private decimal GetFilterVoucherNumber(string VoucherNumber)
        {
            string[] numbers = Regex.Split(VoucherNumber, @"\D+");
            if (numbers.Length > 0)
            {
                return decimal.Parse(string.Join("", numbers));
            }
            else
            {
                return 0;
            }
        }

        private async Task InsertDefaultAsync()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            bool isExists = await _voucherCategoryService.IsExistListAsync(orgCode);
            if (isExists) return;

            var defaultCurrencies = await _accountingCacheManager.GetDefaultVoucherCategoryAsync();
            var entities = defaultCurrencies.Select(p =>
            {
                var dto = ObjectMapper.Map<DefaultVoucherCategoryDto, CruVoucherCategoryDto>(p);
                dto.OrgCode = orgCode;
                dto.Id = this.GetNewObjectId();
                var entity = ObjectMapper.Map<CruVoucherCategoryDto, VoucherCategory>(dto);
                return entity;
            }).ToList();
            await _voucherCategoryService.CreateManyAsync(entities);
            await CurrentUnitOfWork.SaveChangesAsync();
        }

        private async Task<List<PartnerGroupDto>> GetPartnerGroupCodeChild(List<PartnerGroupDto> listData)
        {
            var partnerGroups = await _partnerGroupService.GetQueryableAsync();
            var partnerGroupCodeChild = from p in partnerGroups
                                        where (from aS0 in listData
                                               select aS0.Id).Contains(p.ParentId) && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        select p;
            var res = partnerGroupCodeChild.Select(p => ObjectMapper.Map<PartnerGroup, PartnerGroupDto>(p)).ToList();
            return res;
        }

        private async Task<List<AccountSystemDto>> GetAccoutSystemCodeChild(List<AccountSystemDto> listData)
        {
            var accoutSystemCodeChilds = await _accountSystemService.GetQueryableAsync();
            var accoutSystemCodeChil = from p in accoutSystemCodeChilds
                                       where (from aS0 in listData
                                              select aS0.AccCode).Contains(p.ParentCode) && p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()
                                       select p;
            var res = accoutSystemCodeChil.Select(p => ObjectMapper.Map<AccountSystem, AccountSystemDto>(p)).ToList();
            return res;
        }

        private async Task<IQueryable<AccVoucher>> GetFilterRows(IQueryable<AccVoucher> queryable, PageRequestDto dto)
        {
            if (dto.FilterRows == null) return queryable;
            string tenantDecimalSymbol = await this.GetSymbolSeparateNumber();
            foreach (var item in dto.FilterRows)
            {
                if (item.ColumnType.Equals(ColumnType.Decimal) || item.ColumnType.Equals(ColumnType.Number))
                {
                    string value = item.Value.ToString();
                    if (value.StartsWith(OperatorFilter.GreaterThan))
                    {
                        value = value.Replace(OperatorFilter.GreaterThan, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.GreaterThan);
                    }
                    else if (value.StartsWith(OperatorFilter.GreaterThanOrEqual))
                    {
                        value = value.Replace(OperatorFilter.GreaterThanOrEqual, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.GreaterThanOrEqual);
                    }
                    else if (value.StartsWith(OperatorFilter.LessThan))
                    {
                        value = value.Replace(OperatorFilter.LessThan, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.LessThan);
                    }
                    else if (value.StartsWith(OperatorFilter.LessThanOrEqual))
                    {
                        value = value.Replace(OperatorFilter.LessThanOrEqual, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.LessThanOrEqual);
                    }
                    else if (value.StartsWith(OperatorFilter.NumEqual))
                    {
                        value = value.Replace(OperatorFilter.NumEqual, "");
                        queryable = queryable.Where(item.ColumnName, this.GetNumberDecimal(value, tenantDecimalSymbol), FilterOperator.Equal);
                    }
                }
                else if (item.ColumnType.Equals(ColumnType.Date))
                {
                    var obj = JsonObject.Parse(item.Value.ToString());
                    if (obj["start"] != null)
                    {
                        DateTime value = Convert.ToDateTime(obj["start"].ToString());
                        queryable = queryable.Where(item.ColumnName, value, FilterOperator.GreaterThanOrEqual);
                    }
                    if (obj["end"] != null)
                    {
                        DateTime value = Convert.ToDateTime(obj["end"].ToString());
                        queryable = queryable.Where(item.ColumnName, value, FilterOperator.LessThanOrEqual);
                    }
                }
                else
                {
                    queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.ILike);
                }
            }
            return queryable;
        }
        private decimal GetNumberDecimal(string value, string tenantDecimalSymbol)
        {
            string decimalSymbol = Thread.CurrentThread.CurrentCulture.NumberFormat.CurrencyDecimalSeparator;
            if (decimalSymbol.Equals(tenantDecimalSymbol)) return Convert.ToDecimal(value);
            value = value.Replace(tenantDecimalSymbol, decimalSymbol);
            return Convert.ToDecimal(value);
        }
        private async Task<string> GetSymbolSeparateNumber()
        {
            string orgCode = _webHelper.GetCurrentOrgUnit();
            var currencyFormats = await _tenantSettingService.GetNumberSeparateSymbol(orgCode);
            foreach (var item in currencyFormats)
            {
                if (item.Key.Equals(CurrencyConst.SymbolSeparateDecimal)) return item.Value;
            }
            return null;
        }
        private async Task<bool> IsGrantPermission(string menuAccountingId, string action)
        {
            var menuAccounting = await _menuAccountingService.FindAsync(menuAccountingId);
            if (menuAccounting == null) return false;
            string permissionName = menuAccounting.ViewPermission.Replace(AccountingPermissions.ActionView,
                                                    action);
            var result = await AuthorizationService.AuthorizeAsync(permissionName);
            return result.Succeeded;
        }
        private async Task InsertRefVoucher(CrudAccVoucherDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefVoucher)) return;
            
            var crudRefVoucherDto = new CrudRefVoucherDto();
            crudRefVoucherDto.Id = this.GetNewObjectId();
            crudRefVoucherDto.SrcId = dto.RefVoucher;
            crudRefVoucherDto.DestId = dto.Id;
            crudRefVoucherDto.OrgCode = _webHelper.GetCurrentOrgUnit();
            var refVoucher = ObjectMapper.Map<CrudRefVoucherDto, RefVoucher>(crudRefVoucherDto);
            await _refVoucherService.CreateAsync(refVoucher);

            var crudRefVoucherDtos = new CrudRefVoucherDto();
            crudRefVoucherDtos.Id = this.GetNewObjectId();
            crudRefVoucherDtos.SrcId = dto.Id;
            crudRefVoucherDtos.DestId = dto.RefVoucher;
            crudRefVoucherDtos.OrgCode = _webHelper.GetCurrentOrgUnit();
            var refVouchers = ObjectMapper.Map<CrudRefVoucherDto, RefVoucher>(crudRefVoucherDtos);
            await _refVoucherService.CreateAsync(refVouchers);            
        }
        private async Task<string> GetVoucherStatus()
        {
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionVoucherRecording);
            return isGranted ? "1" : "2";
        }
        private async Task<bool> IsViewByUserNew()
        {
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionViewByUserNew);
            return isGranted ? true : false;
        }
        #endregion
    }
}
