using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Catgories.Others.Careers;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Ledgers;
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
    public class ResetVoucherNumberAppService : AccountingAppService, IResetVoucherNumberAppService, IUnitOfWorkEnabled
    {
        #region Fields
        private readonly VoucherNumberService _voucherNumberService;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly AccVoucherService _accVoucherService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly LedgerService _ledgerService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        private readonly UserService _userService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        #endregion
        #region Ctor
        public ResetVoucherNumberAppService(VoucherNumberService voucherNumberService,
                            UserService userService,
                            VoucherCategoryService voucherCategoryService,
                            AccVoucherService accVoucherService,
                            ProductVoucherService productVoucherService,
                            WarehouseBookService warehouseBookService,
                            VoucherNumberBusiness voucherNumberBusiness,
                            LedgerService ledgerService,
                            IUnitOfWorkManager unitOfWorkManager,
                            LicenseBusiness licenseBusiness,
                            WebHelper webHelper
                            )
        {
            _voucherNumberService = voucherNumberService;
            _userService = userService;
            _voucherCategoryService = voucherCategoryService;
            _accVoucherService = accVoucherService;
            _productVoucherService = productVoucherService;
            _warehouseBookService = warehouseBookService;
            _voucherNumberBusiness = voucherNumberBusiness;
            _ledgerService = ledgerService;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
        }
        #endregion
        public async Task<ResultDto> ResetVoucherNumber(CrudResetVoucherNumberDto dto)
        {
            await _licenseBusiness.CheckExpired();
            if (dto.Data.Any(p => p.BusinessBeginningDate == null || p.BusinessEndingDate == null))
            {
                throw new Exception("Hãy chọn thời gian đánh lại số chứng từ!");
            }
            var lstData = dto.Data;
            // khai báo phiếu kế toán
            var accVoucher = await _accVoucherService.GetQueryableAsync();
            accVoucher = accVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo phiếu hàng hóa
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo sổ cái
            var ledger = await _ledgerService.GetQueryableAsync();
            ledger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            // khai báo sổ kho
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            warehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            foreach (var item in lstData)
            {
                var voucherCategory = await _voucherCategoryService.GetByCode(item.Code, _webHelper.GetCurrentOrgUnit());
                if (voucherCategory == null)
                {
                    throw new Exception("Không tìm thấy mã chứng từ " + item.Code + " trong đơn vị cơ sở " + _webHelper.GetCurrentOrgUnit());
                }
                else if (voucherCategory.IncreaseNumberMethod != item.IncreaseNumberMethod && (item.IncreaseNumberMethod ?? "") != "")
                {
                    voucherCategory.IncreaseNumberMethod = item.IncreaseNumberMethod;
                    await _voucherCategoryService.UpdateAsync(voucherCategory, true);
                }
                await DeleteVoucherNumber(item);
                switch (voucherCategory.IncreaseNumberMethod)
                {
                    case "D":
                        if (item.VoucherKind == "KT")
                        {
                            var lstAccVoucher = accVoucher.Where(p => p.VoucherCode == item.Code
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                   && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.VoucherDate >= item.BusinessBeginningDate
                                                                   && p.VoucherDate <= item.BusinessEndingDate).OrderBy(P => P.VoucherDate).ToList();
                            await UpdateAccVoucherNumber(lstAccVoucher);
                        }
                        else
                        {
                            var lstProductVoucher = productVoucher.Where(p => p.VoucherCode == item.Code
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                   && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.VoucherDate >= item.BusinessBeginningDate
                                                                   && p.VoucherDate <= item.BusinessEndingDate).OrderBy(P => P.VoucherDate).ToList();
                            await UpdateProductVoucherNumber(lstProductVoucher);
                        }
                        break;
                    case "M":
                        if (item.VoucherKind == "KT")
                        {
                            var lstAccVoucher = accVoucher.Where(p => p.VoucherCode == item.Code
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                   && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.VoucherDate.Year >= item.BusinessBeginningDate.Value.Year
                                                                   && p.VoucherDate.Month >= item.BusinessBeginningDate.Value.Month
                                                                   && p.VoucherDate.Year <= item.BusinessEndingDate.Value.Year
                                                                   && p.VoucherDate.Month <= item.BusinessEndingDate.Value.Month).OrderBy(P => P.VoucherDate).ToList();
                            await UpdateAccVoucherNumber(lstAccVoucher);
                        }
                        else
                        {
                            var lstProductVoucher = productVoucher.Where(p => p.VoucherCode == item.Code
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                   && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.VoucherDate.Year >= item.BusinessBeginningDate.Value.Year
                                                                   && p.VoucherDate.Month >= item.BusinessBeginningDate.Value.Month
                                                                   && p.VoucherDate.Year <= item.BusinessEndingDate.Value.Year
                                                                   && p.VoucherDate.Month <= item.BusinessEndingDate.Value.Month).OrderBy(P => P.VoucherDate).ToList();
                            await UpdateProductVoucherNumber(lstProductVoucher);
                        }
                        break;
                    default:
                        if (item.VoucherKind == "KT")
                        {
                            var lstAccVoucher = accVoucher.Where(p => p.VoucherCode == item.Code
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                   && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.VoucherDate.Year >= item.BusinessBeginningDate.Value.Year
                                                                   && p.VoucherDate.Year <= item.BusinessEndingDate.Value.Year).OrderBy(P => P.VoucherDate).ToList();
                            await UpdateAccVoucherNumber(lstAccVoucher);
                        }
                        else
                        {
                            var lstProductVoucher = productVoucher.Where(p => p.VoucherCode == item.Code
                                                                   && p.Year == _webHelper.GetCurrentYear()
                                                                   && p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                                   && p.VoucherDate.Year >= item.BusinessBeginningDate.Value.Year
                                                                   && p.VoucherDate.Year <= item.BusinessEndingDate.Value.Year).OrderBy(P => P.VoucherDate).ToList();
                            await UpdateProductVoucherNumber(lstProductVoucher);
                        }
                        break;
                }
            }
            var result = new ResultDto();
            result.Ok = true;
            result.Message = "Hoàn thành";
            return result;
        }
        
        #region Private
        private async Task DeleteVoucherNumber(ResetVoucherNumberDataDto dto)
        {
            var voucherNumber = await _voucherNumberService.GetQueryableAsync();
            var dataDelete = voucherNumber;
            if (dto.IncreaseNumberMethod == "D")
            {
                dataDelete = voucherNumber.Where(p => p.OrgCode == dto.OrgCode
                                                   && p.VoucherCode == dto.Code
                                                   && p.Year >= dto.BusinessBeginningDate.Value.Year
                                                   && p.Month >= dto.BusinessBeginningDate.Value.Month
                                                   && ((p.Day >= dto.BusinessBeginningDate.Value.Day) || (p.Month > dto.BusinessBeginningDate.Value.Month))
                                                   && p.Year <= dto.BusinessEndingDate.Value.Year
                                                   && p.Month <= dto.BusinessEndingDate.Value.Month
                                                   && ((p.Day <= dto.BusinessEndingDate.Value.Day) || (p.Month < dto.BusinessBeginningDate.Value.Month)));
            } else if (dto.IncreaseNumberMethod == "M")
            {
                dataDelete = voucherNumber.Where(p => p.OrgCode == dto.OrgCode
                                                   && p.VoucherCode == dto.Code
                                                   && p.Year >= dto.BusinessBeginningDate.Value.Year
                                                   && p.Month >= dto.BusinessBeginningDate.Value.Month
                                                   && p.Year <= dto.BusinessEndingDate.Value.Year
                                                   && p.Month <= dto.BusinessEndingDate.Value.Month);
            }
            else
            {
                dataDelete = voucherNumber.Where(p => p.OrgCode == dto.OrgCode
                                                   && p.VoucherCode == dto.Code
                                                   && p.Year >= dto.BusinessBeginningDate.Value.Year
                                                   && p.Year <= dto.BusinessEndingDate.Value.Year);
            }
            await _voucherNumberService.DeleteManyAsync(dataDelete, true);
        }

        private async Task UpdateAccVoucherNumber(List<AccVoucher> lstAccVoucher)
        {
            foreach (var itemAccVoucher in lstAccVoucher)
            {
                // lấy số chứng từ
                var itemVoucherNumber = await _voucherNumberBusiness.AutoVoucherNumberAsync(itemAccVoucher.VoucherCode, itemAccVoucher.VoucherDate);
                itemAccVoucher.VoucherNumber = itemVoucherNumber.VoucherNumber;
                // update số chứng từ cho phiếu KT
                await _accVoucherService.UpdateNoCheckDuplicateAsync(itemAccVoucher);
                // update số chứng từ cho sổ cái
                var lstLedger = await _ledgerService.GetByAccVoucherIdAsync(itemAccVoucher.Id);
                foreach (var itemLedger in lstLedger)
                {
                    itemLedger.VoucherNumber = itemAccVoucher.VoucherNumber;
                    await _ledgerService.UpdateNoCheckDuplicateAsync(itemLedger);
                }
            }
        }

        private async Task UpdateProductVoucherNumber(List<ProductVoucher> lstProductVoucher)
        {
            foreach (var itemProductVoucher in lstProductVoucher)
            {
                // lấy số chứng từ
                var itemVoucherNumber = await _voucherNumberBusiness.AutoVoucherNumberAsync(itemProductVoucher.VoucherCode, itemProductVoucher.VoucherDate);
                itemProductVoucher.VoucherNumber = itemVoucherNumber.VoucherNumber;
                // update số chứng từ cho phiếu HV
                await _productVoucherService.UpdateNoCheckDuplicateAsync(itemProductVoucher);
                // update số chứng từ cho sổ cái
                var lstLedger = await _ledgerService.GetByProductIdAsync(itemProductVoucher.Id);
                foreach (var itemLedger in lstLedger)
                {
                    itemLedger.VoucherNumber = itemProductVoucher.VoucherNumber;
                    await _ledgerService.UpdateNoCheckDuplicateAsync(itemLedger);
                }
                // update số chứng từ cho sổ kho
                var lstWarehouseBook = await _warehouseBookService.GetByProductIdAsync(itemProductVoucher.Id);
                foreach (var itemWarehouseBook in lstWarehouseBook)
                {
                    itemWarehouseBook.VoucherNumber = itemProductVoucher.VoucherNumber;
                    await _warehouseBookService.UpdateNoCheckDuplicateAsync(itemWarehouseBook);
                }
            }
        }
        #endregion
    }
}
