using Accounting.BaseDtos;
using Accounting.Business;
using Accounting.Categories.Others;
using Accounting.Catgories.AccCases;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.Excels;
using Accounting.Exceptions;
using Accounting.Extensions;
using Accounting.Helpers;
using Accounting.Permissions;
using Accounting.Vouchers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Accounting.Vouchers.VoucherPaymentBeginnings
{
    public class VoucherPaymentBeginningAppService : AccountingAppService, IVoucherPaymentBeginningAppService
    {
        #region Fields
        private readonly VoucherPaymentBeginningService _voucherPaymentBeginningService;
        private readonly VoucherPaymentBeginningDetailService _voucherPaymentBeginningDetailService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly ExcelService _excelService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        #endregion
        #region Ctor
        public VoucherPaymentBeginningAppService(
                            VoucherPaymentBeginningService voucherPaymentBeginningService,
                            VoucherPaymentBeginningDetailService voucherPaymentBeginningDetailService,
                            VoucherPaymentBookService voucherPaymentBookService,
                            UserService userService,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper,
                            ExcelService excelService,
                            IUnitOfWorkManager unitOfWorkManager
                            )
        {
            _voucherPaymentBeginningService = voucherPaymentBeginningService;
            _voucherPaymentBeginningDetailService = voucherPaymentBeginningDetailService;
            _voucherPaymentBookService = voucherPaymentBookService;
            _userService = userService;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _excelService = excelService;
            _unitOfWorkManager = unitOfWorkManager;
        }
        #endregion

        //[Authorize(AccountingPermissions.CaseManagerCreate)]
        public async Task<VoucherPaymentBeginningDto> CreateAsync(CrudVoucherPaymentBeginningDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.Id = this.GetNewObjectId();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            var entity = ObjectMapper.Map<CrudVoucherPaymentBeginningDto, VoucherPaymentBeginning>(dto);
            var result = await _voucherPaymentBeginningService.CreateAsync(entity, true);
            var lstVoucherPaymentBook = new List<CrudVoucherPaymentBookDto>();
            foreach (var item in dto.VoucherPaymentBeginningDetails)
            {
                lstVoucherPaymentBook.Add(new CrudVoucherPaymentBookDto
                {
                    Id = this.GetNewObjectId(),
                    DocumentId = dto.Id,
                    VoucherDate = dto.VoucherDate,
                    VoucherNumber = dto.VoucherNumber,
                    AccCode = dto.AccCode,
                    PartnerCode = dto.PartnerCode,
                    Amount = dto.TotalAmountWithoutVat,
                    VatAmount = dto.TotalAmountVat,
                    DiscountAmount = dto.TotalAmountDiscount,
                    TotalAmount = dto.TotalAmount,
                    DeadlinePayment = item.DeadlinePayment,
                    AmountReceivable = item.Amount,
                    Times = item.Times,
                    CreatorName = dto.CreatorName,
                    AmountReceived = 0,
                    OrgCode = dto.OrgCode,
                    Year = dto.Year,
                    AccType = dto.PaymentType,
                });
            }
            var entityLstVoucherPaymentBook = lstVoucherPaymentBook.Select(p => ObjectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(p)).ToList();
            await _voucherPaymentBookService.CreateManyAsync(entityLstVoucherPaymentBook, true); 
            return ObjectMapper.Map<VoucherPaymentBeginning, VoucherPaymentBeginningDto>(result);
        }

        //[Authorize(AccountingPermissions.CaseManagerDelete)]
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            await _voucherPaymentBeginningService.DeleteAsync(id);
        }
        //[Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<VoucherPaymentBeginningDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        //[Authorize(AccountingPermissions.CaseManagerView)]
        public async Task<PageResultDto<VoucherPaymentBeginningDto>> GetListAsync(PageRequestDto dto)
        {
            var result = new PageResultDto<VoucherPaymentBeginningDto>();
            var query = await Filter(dto);
            var querysort = query.OrderBy(p => p.VoucherDate).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<VoucherPaymentBeginning, VoucherPaymentBeginningDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }

        //[Authorize(AccountingPermissions.CaseManagerUpdate)]
        public async Task UpdateAsync(string id, CrudVoucherPaymentBeginningDto dto)
        {
            await _licenseBusiness.CheckExpired();
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Year = _webHelper.GetCurrentYear();
            try
            {
                var voucherPaymentBeginningDetails = await _voucherPaymentBeginningDetailService.GetByVoucherPaymentBeginningIdAsync(id);
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (voucherPaymentBeginningDetails != null)
                {
                    await _voucherPaymentBeginningDetailService.DeleteManyAsync(voucherPaymentBeginningDetails);
                }
                var entity = await _voucherPaymentBeginningService.GetAsync(id);
                ObjectMapper.Map(dto, entity);
                await _voucherPaymentBeginningService.UpdateAsync(entity);
                // xóa dữ liệu ở voucherPaymentBook
                var lstDelete = await _voucherPaymentBookService.GetQueryableAsync();
                lstDelete = lstDelete.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
                lstDelete = lstDelete.Where(p => p.DocumentId == dto.Id);
                if (lstDelete.Any(p => p.AccVoucherId != null)) throw new Exception("Chứng từ đã có chứng từ thanh toán");
                await _voucherPaymentBookService.DeleteManyAsync(lstDelete, true);
                // - - - - -
                var lstVoucherPaymentBook = new List<CrudVoucherPaymentBookDto>();
                foreach (var item in dto.VoucherPaymentBeginningDetails)
                {
                    lstVoucherPaymentBook.Add(new CrudVoucherPaymentBookDto
                    {
                        Id = this.GetNewObjectId(),
                        DocumentId = dto.Id,
                        VoucherDate = dto.VoucherDate,
                        VoucherNumber = dto.VoucherNumber,
                        AccCode = dto.AccCode,
                        PartnerCode = dto.PartnerCode,
                        Amount = dto.TotalAmountWithoutVat,
                        VatAmount = dto.TotalAmountVat,
                        DiscountAmount = dto.TotalAmountDiscount,
                        TotalAmount = dto.TotalAmount,
                        DeadlinePayment = item.DeadlinePayment,
                        AmountReceivable = item.Amount,
                        Times = item.Times,
                        AmountReceived = 0,
                        OrgCode = dto.OrgCode,
                        Year = dto.Year,
                        AccType = dto.PaymentType,
                    });
                }
                var entityLstVoucherPaymentBook = lstVoucherPaymentBook.Select(p => ObjectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(p)).ToList();
                await _voucherPaymentBookService.CreateManyAsync(entityLstVoucherPaymentBook, true);
                await unitOfWork.CompleteAsync();
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }
        
        public async Task<VoucherPaymentBeginningDto> GetByIdAsync(string voucherPaymentBeginningId)
        {
            var voucherPaymentBeginning = await _voucherPaymentBeginningService.GetAsync(voucherPaymentBeginningId);
            return ObjectMapper.Map<VoucherPaymentBeginning, VoucherPaymentBeginningDto>(voucherPaymentBeginning);
        }

        public async Task<List<VoucherPaymentBeginningDetailDto>> GetDetailAsync(string voucherPaymentBeginningId)
        {
            var result = new List<VoucherPaymentBeginningDetailDto>();
            var query = await _voucherPaymentBeginningDetailService.GetQueryableAsync();
            query = query.Where(p => p.VoucherPaymentBeginningId == voucherPaymentBeginningId).OrderBy(p => p.Times);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<VoucherPaymentBeginningDetail, VoucherPaymentBeginningDetailDto>(p)).ToList();
            return result;
        }
        #region Private
        private async Task<ResultDto> PostVoucherPaymentBook(CrudVoucherPaymentBeginningDto dto)
        {
            var voucherPaymentBeginning = await _voucherPaymentBeginningService.GetQueryableAsync();
            var voucherPaymentBeginningDetail = await _voucherPaymentBeginningDetailService.GetQueryableAsync();
            var voucherPaymentBook = await _voucherPaymentBookService.GetQueryableAsync();
            if (voucherPaymentBook.Any(p => p.DocumentId == dto.Id && p.AccVoucherId != null))
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.VoucherPaymentBeginning, ErrorCode.Other), "Chứng từ đã có chứng từ thanh toán");
            }
            var voucherPaymentBookDelete = voucherPaymentBook.Where(p => p.DocumentId == dto.Id);
            await _voucherPaymentBookService.DeleteManyAsync(voucherPaymentBookDelete, true);

            var data = (from a in dto.VoucherPaymentBeginningDetails
                       select new CrudVoucherPaymentBookDto
                       {
                           Id = GetNewObjectId(),
                           DocumentId = dto.Id,
                           VoucherDate = dto.VoucherDate,
                           VoucherNumber = dto.VoucherNumber,
                           AccCode = dto.AccCode,
                           PartnerCode = dto.PartnerCode,
                           Amount = dto.TotalAmountWithoutVat,
                           VatAmount = dto.TotalAmountVat,
                           DiscountAmount = dto.TotalAmountDiscount,
                           TotalAmount = dto.TotalAmount,
                           DeadlinePayment = a.DeadlinePayment,
                           AmountReceivable = a.Amount,
                           Times = a.Times,
                           AmountReceived = null,
                           OrgCode = dto.OrgCode,
                           Year = dto.Year,
                           AccVoucherId = null,
                           AccType = dto.PaymentType,
                       }).ToList();
            var lstVPB = data.Select(p => ObjectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(p)).ToList();
            await _voucherPaymentBookService.CreateManyAsync(lstVPB, true);
            var res = new ResultDto();
            return res;
        }

            private async Task<IQueryable<VoucherPaymentBeginning>> Filter(PageRequestDto dto)
        {
            var queryable = await _voucherPaymentBeginningService.GetQueryableAsync();

            if (dto.FilterRows == null) return queryable;

            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());


            if (!string.IsNullOrEmpty(dto.QuickSearch))
            {
                queryable = queryable.Where(p => p.AccCode.Contains(dto.QuickSearch) || p.PartnerCode.Contains(dto.QuickSearch));
            }

            if (dto.FilterRows == null) return queryable;
            foreach (var item in dto.FilterRows)
            {
                queryable = queryable.Where(item.ColumnName, item.Value, FilterOperator.Contains);
            }
            return queryable;
        }
        #endregion
    }
}
