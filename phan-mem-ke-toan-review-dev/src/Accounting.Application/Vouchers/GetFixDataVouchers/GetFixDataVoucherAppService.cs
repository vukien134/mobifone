using Accounting.BaseDtos;
using Accounting.BaseDtos.Customines;
using Accounting.Business;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.Contracts;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.Partners;
using Accounting.Common.Extensions;
using Accounting.Constants;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
using Accounting.DomainServices.Excels;
using Accounting.DomainServices.Ledgers;
using Accounting.DomainServices.Users;
using Accounting.DomainServices.Vouchers;
using Accounting.DomainServices.Windows;
using Accounting.Excels;
using Accounting.Helpers;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.VoucherNumbers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using Volo.Abp.Uow;

namespace Accounting.Vouchers.GetFixDataVouchers
{
    public class GetFixDataVoucherAppService : AccountingAppService
    {
        #region Field
        private readonly AccVoucherService _accVoucherService;
        private readonly AccVoucherDetailService _accVoucherDetailService;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly LedgerService _ledgerService;
        private readonly UserService _userService;
        private readonly ExcelService _excelService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly AccPartnerService _accPartnerService;
        private readonly AccountSystemService _accountSystemService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly FixErrorService _fixErrorService;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly ContractService _contractService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
        private readonly DepartmentService _departmentService;
        private readonly AccCaseService _accCaseService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _productService;
        private readonly ProductOriginService _productOriginService;
        private readonly ProductLotService _productLotService;
        private readonly UnitService _unitService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductVoucherService _productVoucherService;
        private readonly ProductVoucherDetailService _productVoucherDetailService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly WebHelper _webHelper;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly WindowService _windowService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly DefaultVoucherCategoryService _defaultVoucherCategoryService;
        private readonly DefaultFixErrorService _defaultFixErrorService;
        #endregion
        #region Ctor
        public GetFixDataVoucherAppService(AccVoucherService accVoucherService,
                                AccVoucherDetailService accVoucherDetailService,
                                AccTaxDetailService accTaxDetailService,
                                LedgerService ledgerService,
                                UserService userService,
                                ExcelService excelService,
                                PartnerGroupService partnerGroupService,
                                AccPartnerService accPartnerService,
                                AccountSystemService accountSystemService,
                                VoucherNumberBusiness voucherNumberBusiness,
                                VoucherCategoryService voucherCategoryService,
                                FixErrorService fixErrorService,
                                AccOpeningBalanceService accOpeningBalanceService,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                TaxCategoryService taxCategoryService,
                                ContractService contractService,
                                FProductWorkService fProductWorkService,
                                AccSectionService accSectionService,
                                DepartmentService departmentService,
                                AccCaseService accCaseService,
                                WarehouseService warehouseService,
                                ProductService productService,
                                ProductOriginService productOriginService,
                                ProductLotService productLotService,
                                UnitService unitService,
                                ProductUnitService productUnitService,
                                ProductVoucherService productVoucherService,
                                ProductVoucherDetailService productVoucherDetailService,
                                WarehouseBookService warehouseBookService,
                                WorkPlaceSevice workPlaceSevice,
                                IUnitOfWorkManager unitOfWorkManager,
                                WindowService windowService,
                                LicenseBusiness licenseBusiness,
                                WebHelper webHelper,
                                AccountingCacheManager accountingCacheManager,
                                TenantSettingService tenantSettingService,
                                DefaultVoucherCategoryService defaultVoucherCategoryService,
                                DefaultFixErrorService defaultFixErrorService
            )
        {
            _accVoucherService = accVoucherService;
            _accVoucherDetailService = accVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _ledgerService = ledgerService;
            _userService = userService;
            _excelService = excelService;
            _partnerGroupService = partnerGroupService;
            _accPartnerService = accPartnerService;
            _accountSystemService = accountSystemService;
            _voucherNumberBusiness = voucherNumberBusiness;
            _voucherCategoryService = voucherCategoryService;
            _fixErrorService = fixErrorService;
            _accOpeningBalanceService = accOpeningBalanceService;
            _productOpeningBalanceService = productOpeningBalanceService;
            _taxCategoryService = taxCategoryService;
            _contractService = contractService;
            _fProductWorkService = fProductWorkService;
            _accSectionService = accSectionService;
            _departmentService = departmentService;
            _accCaseService = accCaseService;
            _warehouseService = warehouseService;
            _productService = productService;
            _productOriginService = productOriginService;
            _productLotService = productLotService;
            _unitService = unitService;
            _productUnitService = productUnitService;
            _productVoucherService = productVoucherService;
            _productVoucherDetailService = productVoucherDetailService;
            _warehouseBookService = warehouseBookService;
            _workPlaceSevice = workPlaceSevice;
            _unitOfWorkManager = unitOfWorkManager;
            _licenseBusiness = licenseBusiness;
            _webHelper = webHelper;
            _accountingCacheManager = accountingCacheManager;
            _windowService = windowService;
            _tenantSettingService = tenantSettingService;
            _defaultVoucherCategoryService = defaultVoucherCategoryService;
            _defaultFixErrorService = defaultFixErrorService;
        }
        #endregion
        public async Task<ResultDto> FixData(FixDataParameterDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var curencyCode = await _tenantSettingService.GetTenantSettingByKeyAsync("M_MA_NT0", _webHelper.GetCurrentOrgUnit());
            var productUnit = await _productUnitService.GetQueryableAsync();
            productUnit = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucher = await _productVoucherService.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucherDetail = await _productVoucherDetailService.GetQueryableAsync();
            productVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var accTaxDetail = await _accTaxDetailService.GetQueryableAsync();
            accTaxDetail = accTaxDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var ledger = await _ledgerService.GetQueryableAsync();
            ledger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var warehouseBook = await _warehouseBookService.GetQueryableAsync();
            warehouseBook = warehouseBook.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            // update productUnit
            var productUnitUpdate = (from a in productUnit 
                                    join b in product on a.ProductId equals b.Id
                                    where a.OrgCode == _webHelper.GetCurrentOrgUnit()
                                    select a).ToList();
            foreach (var item in productUnitUpdate)
            {
                var itemProduct = product.Where(p => p.Id == item.ProductId).First();
                item.OrgCode = itemProduct.OrgCode;
                item.ProductCode = itemProduct.Code;
                await _productUnitService.UpdateAsync(item,true);
            }
            // xóa productUnit
            var productUnitDelete =  from a in productUnit
                                     join b in product on a.ProductId equals b.Id into ajb
                                     from b in ajb.DefaultIfEmpty()
                                     where a.OrgCode == _webHelper.GetCurrentOrgUnit() && b == null
                                     select a;
            await _productUnitService.DeleteManyAsync(productUnitDelete, true);

            // update product 
            var productUpdate = (from a in product
                                 join b in productUnit on a.Id equals b.ProductId
                                 where a.OrgCode == _webHelper.GetCurrentOrgUnit() && b.IsBasicUnit == true
                                 select a).ToList();
            foreach (var item in productUpdate)
            {
                var itemProductUnit = productUnit.Where(p => p.ProductId == item.Id && p.IsBasicUnit == true).First();
                item.UnitCode = itemProductUnit.UnitCode;
                await _productService.UpdateAsync(item, true);
            }

            //update phát sinh thuế
            var accTaxDetailUpdate = accTaxDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            foreach (var item in accTaxDetailUpdate)
            {
                item.InvoiceDate = item.InvoiceDate == null ? item.VoucherDate : item.InvoiceDate;
                await _accTaxDetailService.UpdateAsync(item, true);
            }

            //update hàng hóa
            var productVoucherDetailUpdate = (from a in productVoucherDetail
                                              join b in productVoucher on a.ProductVoucherId equals b.Id
                                              join c in product on a.ProductCode equals c.Code
                                              where b.VoucherDate >= dto.FromDate && b.VoucherDate <= dto.ToDate
                                              select a).ToList();
            foreach (var item in productVoucherDetailUpdate)
            {
                var itemProduct = product.Where(p => p.Code == item.ProductCode).First();
                item.ProductLotCode = itemProduct.AttachProductLot == "C" ? item.ProductLotCode : "";
                item.ProductOriginCode = itemProduct.AttachProductOrigin == "C" ? item.ProductOriginCode : "";
                await _productVoucherDetailService.UpdateAsync(item, true);
            }
            product = await _productService.GetQueryableAsync();
            lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            //update sokho
            var warehouseBookUpdate = (from a in warehouseBook
                                        join b in product on a.ProductCode equals b.Code
                                        where a.VoucherDate >= dto.FromDate && a.VoucherDate <= dto.ToDate
                                        select a).ToList();

            foreach (var item in warehouseBookUpdate)
            {
                var itemProduct = lstProduct.Where(p => p.Code == item.ProductCode).First();
                item.ProductLotCode = itemProduct.AttachProductLot == "C" ? item.ProductLotCode : "";
                item.ProductOriginCode = itemProduct.AttachProductOrigin == "C" ? item.ProductOriginCode : "";
            }
            await _warehouseBookService.DeleteManyAsync(warehouseBookUpdate, true);
            await _warehouseBookService.CreateManyAsync(warehouseBookUpdate, true);

            // update socai
            var ledgerUpdate = (from a in ledger
                                where a.VoucherDate >= dto.FromDate && a.VoucherDate <= dto.ToDate
                                select a).ToList();
            foreach (var item in ledgerUpdate)
            {
                var debitAcc = lstAccountSystem.Where(p => p.AccCode == item.DebitAcc).FirstOrDefault();
                var creditAcc = lstAccountSystem.Where(p => p.AccCode == item.CreditAcc).FirstOrDefault();
                var itemProduct = lstProduct.Where(p => p.Code == item.ProductCode).FirstOrDefault();

                item.DebitCurrencyCode = (debitAcc?.AttachCurrency ?? "") == "C" ? item.DebitCurrencyCode ?? "VND" : curencyCode.Value;
                item.DebitExchangeRate = (creditAcc?.AttachCurrency ?? "") == "C" ? item.DebitExchangeRate ?? 1 : 1;
                item.CreditCurrencyCode = (debitAcc?.AttachCurrency ?? "") == "C" ? item.CreditCurrencyCode ?? "VND" : curencyCode.Value;
                item.CreditExchangeRate = (creditAcc?.AttachCurrency ?? "") == "C" ? item.CreditExchangeRate ?? 1 : 1;
                item.DebitPartnerCode = (debitAcc?.AttachPartner ?? "") == "C" ? item.PartnerCode ?? "" : "";
                item.CreditPartnerCode = (creditAcc?.AttachPartner ?? "") == "C" ? ((item.ClearingPartnerCode ?? "") == "" ? (item.PartnerCode ?? "") : (item.ClearingPartnerCode)) : "";
                item.DebitContractCode = (debitAcc?.AttachContract ?? "") == "C" ? item.ContractCode ?? "" : "";
                item.CreditContractCode = (creditAcc?.AttachContract ?? "") == "C" ? ((item.ClearingContractCode ?? "") == "" ? (item.ContractCode ?? "") : (item.ClearingContractCode)) : "";
                item.DebitFProductWorkCode = (debitAcc?.AttachProductCost ?? "") == "C" ? item.FProductWorkCode ?? "" : "";
                item.CreditFProductWorkCode = (creditAcc?.AttachProductCost ?? "") == "C" ? ((item.ClearingFProductWorkCode ?? "") == "" ? (item.FProductWorkCode ?? "") : (item.ClearingFProductWorkCode)) : "";
                item.DebitSectionCode = (debitAcc?.AttachAccSection ?? "") == "C" ? item.SectionCode ?? "" : "";
                item.CreditSectionCode = (creditAcc?.AttachAccSection ?? "") == "C" ? ((item.ClearingSectionCode ?? "") == "" ? (item.SectionCode ?? "") : (item.ClearingSectionCode)) : "";
                item.DebitWorkPlaceCode = (debitAcc?.AttachWorkPlace ?? "") == "C" ? item.WorkPlaceCode ?? "" : "";
                item.CreditWorkPlaceCode = (creditAcc?.AttachWorkPlace ?? "") == "C" ? item.WorkPlaceCode ?? "" : "";
                item.ProductLotCode = (itemProduct?.AttachProductLot ?? "") == "C" ? item.ProductLotCode ?? "" : "";
                item.ProductOriginCode = (itemProduct?.AttachProductOrigin ?? "") == "C" ? item.ProductOriginCode ?? "" : "";
            }
            await _ledgerService.DeleteManyAsync(ledgerUpdate, true);
            await _ledgerService.CreateManyAsync(ledgerUpdate, true);

            var res = new ResultDto();
            res.Ok = true;
            res.Message = "Bảo trì số liệu hoàn thành";
            return res;
        }

        public async Task<List<JsonObject>> PostGetErrorData(GetErrorDataParameterDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var fixError = await _fixErrorService.GetData(_webHelper.GetCurrentOrgUnit());

            /*var lstFixError = fixError.Where(p => (dto.LstError ?? "") == "" || dto.LstError.Contains(p.KeyError) && p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                     .Select(p => new FixErrorDto
                                     {
                                         errorId = p.ErrorId,
                                         errorName = p.ErrorName,
                                         tag = p.Tag,
                                         keyError = p.KeyError,
                                         classify = p.Classify
                                     }).ToList();*/
            var lstFixError = await this.GetLstFixError(dto.LstError);
            var ledger = await _ledgerService.GetQueryableAsync();
            var lstLedger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()
                                           && p.VoucherDate >= dto.FromDate && p.VoucherDate <= dto.ToDate).ToList();
            var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
            var lstAccOpeningBalance = accOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var productOpeningBalance = await _productOpeningBalanceService.GetQueryableAsync();
            var lstProductOpeningBalance = productOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            // khai báo các dữ liệu liên quan
            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstVoucherCategory = voucherCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var taxCategory = await _taxCategoryService.GetQueryableAsync();
            var lstTaxCategory = taxCategory.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accPartner = await _accPartnerService.GetQueryableAsync();
            var lstAccPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var contract = await _contractService.GetQueryableAsync();
            var lstContract = contract.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var fProductWork = await _fProductWorkService.GetQueryableAsync();
            var lstFProductWork = fProductWork.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accSection = await _accSectionService.GetQueryableAsync();
            var lstAccSection = accSection.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var department = await _departmentService.GetQueryableAsync();
            var lstDepartment = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var accCase = await _accCaseService.GetQueryableAsync();
            var lstAccCase = accCase.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var warehouse = await _warehouseService.GetQueryableAsync();
            var lstWarehouse = warehouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var product = await _productService.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var workPlace = await _workPlaceSevice.GetQueryableAsync();
            var lstWorkPlace = workPlace.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productOrigin = await _productOriginService.GetQueryableAsync();
            var lstProductOrigin = productOrigin.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productLot = await _productLotService.GetQueryableAsync();
            var lstProductLot = productLot.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var unit = await _unitService.GetQueryableAsync();
            var lstUnit = unit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productUnit = await _productUnitService.GetQueryableAsync();
            var lstProductUnit = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            // Lấy dữ liệu
            var dataLedgerCheck = (from a in lstLedger
                                   join b in lstFixError on new { eq = "" } equals new { eq = "" }
                                   where b.classify == 1
                                   select new GetErrorDataLedgerDto
                                   {
                                       id = a.Id,
                                       errorId = b.errorId,
                                       errorName = b.errorName,
                                       keyError = b.keyError,
                                       voucherId = a.VoucherId,
                                       ord0 = a.Ord0,
                                       year = a.Year,
                                       ord0Extra = a.Ord0Extra,
                                       departmentCode = a.DepartmentCode,
                                       voucherCode = a.VoucherCode,
                                       voucherGroup = a.VoucherGroup,
                                       businessCode = a.BusinessCode,
                                       businessAcc = a.BusinessAcc,
                                       checkDuplicate = a.CheckDuplicate,
                                       voucherNumber = a.VoucherNumber,
                                       invoiceNbr = a.InvoiceNbr,
                                       recordingVoucherNumber = a.RecordingVoucherNumber,
                                       voucherDate = a.VoucherDate,
                                       days = a.Days,
                                       paymentTermsCode = a.PaymentTermsCode,
                                       contractCode = a.ContractCode,
                                       clearingContractCode = a.ClearingContractCode,
                                       currencyCode = a.CurrencyCode,
                                       exchangeRate = a.ExchangeRate,
                                       partnerCode0 = a.PartnerCode0,
                                       partnerName0 = a.PartnerName0,
                                       representative = a.Representative,
                                       address = a.Address,
                                       description = a.Description,
                                       descriptionE = a.DescriptionE,
                                       originVoucher = a.OriginVoucher,
                                       debitAcc = a.DebitAcc,
                                       debitExchangeRate = a.DebitExchangeRate,
                                       debitCurrencyCode = a.DebitCurrencyCode,
                                       debitPartnerCode = a.DebitPartnerCode,
                                       debitContractCode = a.DebitContractCode,
                                       debitFProductWorkCode = a.DebitFProductWorkCode,
                                       debitSectionCode = a.DebitSectionCode,
                                       debitWorkPlaceCode = a.DebitWorkPlaceCode,
                                       debitAmountCur = a.DebitAmountCur,
                                       creditAcc = a.CreditAcc,
                                       creditCurrencyCode = a.CreditCurrencyCode,
                                       creditExchangeRate = a.CreditExchangeRate,
                                       creditPartnerCode = a.CreditPartnerCode,
                                       creditContractCode = a.CreditContractCode,
                                       creditFProductWorkCode = a.CreditFProductWorkCode,
                                       creditSectionCode = a.CreditSectionCode,
                                       creditWorkPlaceCode = a.CreditWorkPlaceCode,
                                       creditAmountCur = a.CreditAmountCur,
                                       creditAmount = a.CreditAmount,
                                       amountCur = a.AmountCur,
                                       amount = a.Amount,
                                       note = a.Note,
                                       noteE = a.NoteE,
                                       fProductWorkCode = a.FProductWorkCode,
                                       partnerCode = a.PartnerCode,
                                       sectionCode = a.SectionCode,
                                       clearingFProductWorkCode = a.ClearingFProductWorkCode,
                                       clearingPartnerCode = a.ClearingPartnerCode,
                                       clearingSectionCode = a.ClearingSectionCode,
                                       workPlaceCode = a.WorkPlaceCode,
                                       caseCode = a.CaseCode,
                                       warehouseCode = a.WarehouseCode,
                                       transWarehouseCode = a.TransWarehouseCode,
                                       productCode = a.ProductCode,
                                       productLotCode = a.ProductLotCode,
                                       productOriginCode = a.ProductOriginCode,
                                       unitCode = a.UnitCode,
                                       trxQuantity = a.TrxQuantity,
                                       quantity = a.Quantity,
                                       trxPromotionQuantity = a.TrxPromotionQuantity,
                                       promotionQuantity = a.PromotionQuantity,
                                       priceCur = a.PriceCur,
                                       price = a.Price,
                                       taxCategoryCode = a.TaxCategoryCode,
                                       vatPercentage = a.VatPercentage,
                                       invoiceNumber = a.InvoiceNumber,
                                       invoiceSymbol = a.InvoiceSymbol,
                                       invoiceDate = a.InvoiceDate,
                                       invoicePartnerName = a.InvoicePartnerName,
                                       invoicePartnerAddress = a.InvoicePartnerAddress,
                                       taxCode = a.TaxCode,
                                       debitOrCredit = a.DebitOrCredit,
                                       checkDuplicate0 = a.CheckDuplicate0,
                                       salesChannelCode = a.SalesChannelCode,
                                       productName0 = a.ProductName0,
                                       securityNo = a.SecurityNo,
                                       status = a.Status,
                                   }).ToList();
            var dataAccCheck = (from a in lstAccOpeningBalance
                                where (dto.LstError ?? "") == "" || dto.LstError.Contains("BeginAcc")
                                select new GetErrorDataAccDto
                                {
                                    errorId = "13",
                                    errorName = "",
                                    keyError = "BeginAcc",
                                    id = a.Id,
                                    accCode = a.AccCode,
                                    year = a.Year,
                                    currencyCode = a.CurrencyCode,
                                    partnerCode = a.PartnerCode,
                                    contractCode = a.ContractCode,
                                    fProductWorkCode = a.FProductWorkCode,
                                    accSectionCode = a.AccSectionCode,
                                    workPlaceCode = a.WorkPlaceCode,
                                    debit = a.Debit,
                                    debitCur = a.DebitCur,
                                    credit = a.Credit,
                                    creditCur = a.CreditCur,
                                    debitCum = a.DebitCum,
                                    debitCumCur = a.DebitCumCur,
                                    creditCum = a.CreditCum,
                                    creditCumCur = a.CreditCumCur,
                                }).ToList();
            var dataProductCheck = (from a in lstProductOpeningBalance
                                    where (dto.LstError ?? "") == "" || dto.LstError.Contains("BeginProduct")
                                    select new GetErrorDataProductDto
                                    {
                                        errorId = "14",
                                        errorName = "",
                                        keyError = "BeginProduct",
                                        id = a.Id,
                                        ord0 = a.Ord0,
                                        year = a.Year,
                                        warehouseCode = a.WarehouseCode,
                                        accCode = a.AccCode,
                                        productCode = a.ProductCode,
                                        productLotCode = a.ProductLotCode,
                                        productOriginCode = a.ProductOriginCode,
                                        quantity = a.Quantity,
                                        price = a.Price,
                                        priceCur = a.PriceCur,
                                        amount = a.Amount,
                                        amountCur = a.AmountCur,
                                    }).ToList();
            var dataLedgerError = new List<GetErrorDataLedgerDto>();
            foreach (var item in dataLedgerCheck)
            {
                var itemVoucherCategory = lstVoucherCategory.Where(p => p.Code == item.voucherCode).FirstOrDefault();
                var debitAcc = lstAccountSystem.Where(p => p.AccCode == item.debitAcc).FirstOrDefault();
                var creditAcc = lstAccountSystem.Where(p => p.AccCode == item.creditAcc).FirstOrDefault();
                var itemTaxCategory = lstTaxCategory.Where(p => p.Code == item.taxCategoryCode).FirstOrDefault();
                var itemAccPartner = lstAccPartner.Where(p => p.Code == item.partnerCode).FirstOrDefault();
                var itemClearingPartner = lstAccPartner.Where(p => p.Code == item.clearingPartnerCode).FirstOrDefault();
                var itemContract = lstContract.Where(p => p.Code == item.contractCode).FirstOrDefault();
                var itemClearingContract = lstContract.Where(p => p.Code == item.clearingContractCode).FirstOrDefault();
                var itemFProductWork = lstFProductWork.Where(p => p.Code == item.fProductWorkCode).FirstOrDefault();
                var itemClearingFProductWork = lstFProductWork.Where(p => p.Code == item.clearingFProductWorkCode).FirstOrDefault();
                var itemSection = lstAccSection.Where(p => p.Code == item.sectionCode).FirstOrDefault();
                var itemClearingSection = lstAccSection.Where(p => p.Code == item.clearingSectionCode).FirstOrDefault();
                var itemDepartment = lstDepartment.Where(p => p.Code == item.departmentCode).FirstOrDefault();
                var itemCase = lstAccCase.Where(p => p.Code == item.caseCode).FirstOrDefault();
                var itemWarehouse = lstWarehouse.Where(p => p.Code == item.warehouseCode).FirstOrDefault();
                var itemProduct = lstProduct.Where(p => p.Code == item.productCode).FirstOrDefault();
                var itemWorkPlace = lstWorkPlace.Where(p => p.Code == item.workPlaceCode).FirstOrDefault();
                var itemProductOrigin = lstProductOrigin.Where(p => p.Code == item.productOriginCode).FirstOrDefault();
                var itemProductLot = lstProductLot.Where(p => p.Code == item.productLotCode).FirstOrDefault();
                var itemUnit = lstUnit.Where(p => p.Code == item.unitCode).FirstOrDefault();
                var itemProductUnit = lstProductUnit.Where(p => p.UnitCode == item.unitCode && p.ProductCode == item.productCode).FirstOrDefault();
                switch (item.keyError)
                {
                    case "Acc":
                        if ((creditAcc?.IsBalanceSheetAcc ?? "") != "C" && (debitAcc?.AccCode ?? "") == "")
                        {
                            item.errorName = "Không có mã tài khoản bên Nợ: " + item.debitAcc + " !";
                        }
                        else if ((debitAcc?.AccType ?? "") == "K")
                        {
                            item.errorName = "Tài khoản Nợ là tài khoản tổng hợp: " + item.debitAcc + " !";
                        }
                        else if ((debitAcc?.IsBalanceSheetAcc ?? "") != "C" && (creditAcc?.AccCode ?? "") == "")
                        {
                            item.errorName = "Không có mã tài khoản bên Có: " + item.creditAcc + " !";
                        }
                        else if ((creditAcc?.AccType ?? "") == "K")
                        {
                            item.errorName = "Tài khoản Có là tài khoản tổng hợp: " + item.creditAcc + " !";
                        }
                        else if (((item.ord0 ?? "").StartsWith("Z") && (itemTaxCategory?.OutOrIn ?? "") == "V" && !(item.debitAcc ?? "").StartsWith(itemTaxCategory?.DebitAcc ?? "zzzz")))
                        {
                            item.errorName = "Tài khoản Nợ: " + item.debitAcc + " không đúng khai báo mã thuế: " + item.taxCategoryCode + " !";
                        }
                        else if (((item.ord0 ?? "").StartsWith("Z") && (itemTaxCategory?.OutOrIn ?? "") == "R" && !(item.creditAcc ?? "").StartsWith(itemTaxCategory.CreditAcc ?? "zzzz")))
                        {
                            item.errorName = "Tài khoản Nợ: " + item.creditAcc + " không đúng khai báo mã thuế: " + item.taxCategoryCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Partner":
                        if ((item.partnerCode ?? "") != "" && (itemAccPartner?.Code ?? "") == "" && (this.CheckExistedPartner(item.creditAcc).Result == true || this.CheckExistedPartner(item.debitAcc).Result == true))
                        {
                            item.errorName = "Không có mã đối tượng: " + item.partnerCode + " !";
                        }
                        else if ((item.clearingPartnerCode ?? "") != "" && (itemClearingPartner?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã đối tượng bù trừ: " + item.clearingPartnerCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Contract":
                        if ((item.contractCode ?? "") != "" && (itemContract?.Code ?? "") == "" && (this.CheckExistedContract(item.creditAcc).Result == true || this.CheckExistedContract(item.debitAcc).Result == true))
                        {
                            item.errorName = "Không có mã hợp đồng: " + item.contractCode + " !";
                        }
                        else if ((item.clearingContractCode ?? "") != "" && (itemClearingContract?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã hợp đồng bù trừ: " + item.clearingContractCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "FProductWork":
                        if ((item.fProductWorkCode ?? "") == "" && (itemSection?.AttachProductCost ?? "") == "C")
                        {
                            item.errorName = "Bạn cần vào công trình,sp theo khoản mục: " + item.fProductWorkCode + " !";
                        }
                        else if ((item.fProductWorkCode ?? "") != "" && (itemFProductWork?.Code ?? "") == "" && (this.CheckExistedFProductWork(item.creditAcc).Result == true || this.CheckExistedFProductWork(item.debitAcc).Result == true))
                        {
                            item.errorName = "Không có mã công trình,sp: " + item.fProductWorkCode + " !";
                        }
                        else if ((item.fProductWorkCode ?? "") != "" && (itemFProductWork?.FPWType ?? "") == "K")
                        {
                            item.errorName = "Mã công trình,sp: " + item.fProductWorkCode + " là tổng hợp !";
                        }
                        else if ((item.clearingFProductWorkCode ?? "") != "" && (itemClearingFProductWork?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã công trình,sp bù trừ: " + item.clearingFProductWorkCode + " !";
                        }
                        else if ((item.clearingFProductWorkCode ?? "") != "" && (itemClearingFProductWork?.FPWType ?? "") == "K")
                        {
                            item.errorName = "Mã công trình,sp bù trừ: " + item.clearingFProductWorkCode + " là tổng hợp !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Section":
                        if ((item.sectionCode ?? "") != "" && (itemSection?.Code ?? "") == "" && (this.CheckExistedSection(item.creditAcc).Result == true || this.CheckExistedSection(item.debitAcc).Result == true))
                        {
                            item.errorName = "Không có mã khoản mục phí: " + item.sectionCode + " !";
                        }
                        else if ((item.clearingSectionCode ?? "") != "" && (itemClearingSection?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã khoản mục phí bù trừ: " + item.clearingSectionCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Department":
                        if ((item.departmentCode ?? "") != "" && (itemDepartment?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã bộ phận: " + item.departmentCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Case":
                        if ((item.caseCode ?? "") != "" && (itemCase?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã vụ việc: " + item.caseCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Warehouse":
                        if ((item.warehouseCode ?? "") == "" && (item.voucherGroup <= 2) && (item.ord0 ?? "").StartsWith("A") && itemVoucherCategory.VoucherKind == "HV")
                        {
                            item.errorName = "Bạn phải vào mã kho!";
                        }
                        else if ((item.warehouseCode ?? "") != "" && (itemWarehouse?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã Kho: " + item.warehouseCode + " !";
                        }
                        else if ((item.warehouseCode ?? "") != "" && (itemWarehouse?.WarehouseType ?? "") == "K")
                        {
                            item.errorName = "Mã Kho: " + item.warehouseCode + " là tổng hợp!";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Product":
                        if ((item.productCode ?? "") != "" && (itemProduct?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã hàng hóa: " + item.productCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "WorkPlace":
                        if ((item.workPlaceCode ?? "") != "" && (itemWorkPlace?.Code ?? "") == "" && (this.CheckExistedWorkPlace(item.creditAcc).Result == true || this.CheckExistedWorkPlace(item.debitAcc).Result == true))
                        {
                            item.errorName = "Không có mã phân xưởng: " + item.workPlaceCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "ProductOrigin":
                        if ((item.productOriginCode ?? "") == "" && (itemProduct?.AttachProductOrigin ?? "") == "C")
                        {
                            item.errorName = "Bạn phải nhập nguồn hàng cho mã hàng: " + item.productCode + " !";
                        }
                        else if ((item.productOriginCode ?? "") != "" && (itemProduct?.AttachProductOrigin ?? "") != "C")
                        {
                            item.errorName = "Bạn phải xóa nguồn hàng: " + item.productOriginCode + "cho mã hàng: " + item.productCode + " !";
                        }
                        else if ((item.productOriginCode ?? "") != "" && (itemProductOrigin?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã nguồn hàng: " + item.productOriginCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "ProductLot":
                        if ((item.productLotCode ?? "") == "" && (itemProduct?.AttachProductLot ?? "") == "C")
                        {
                            item.errorName = "Bạn phải nhập lô hàng cho mã hàng: " + item.productCode + " !";
                        }
                        else if ((item.productLotCode ?? "") != "" && (itemProduct?.AttachProductLot ?? "") != "C")
                        {
                            item.errorName = "Bạn phải xóa lô hàng: " + item.productLotCode + "cho mã hàng: " + item.productCode + " !";
                        }
                        else if ((item.productLotCode ?? "") != "" && (itemProductOrigin?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã lô hàng: " + item.productLotCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "Unit":
                        if ((item.unitCode ?? "") != "" && (item.productCode ?? "") != "" && (itemProduct?.ProductType ?? "") != "D" && (itemUnit?.Code ?? "") == "")
                        {
                            item.errorName = "Không có mã đơn vị tính: " + item.unitCode + " !";
                        }
                        else if ((item.unitCode ?? "") != "" && (item.productCode ?? "") != "" && (itemProduct?.ProductType ?? "") != "D" && (itemProductUnit?.UnitCode ?? "") == "")
                        {
                            item.errorName = "Mã hàng " + item.productCode + " không có mã đơn vị tính: " + item.unitCode + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    case "00":
                        if ((item.debitPartnerCode ?? "") == "" && (debitAcc?.AttachPartner ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào đối tượng bên Nợ cho tài khoản: " + item.debitAcc + " !";
                        }
                        else if ((item.creditAcc ?? "") == "" && (creditAcc?.AttachPartner ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào đối tượng bên Có cho tài khoản: " + item.creditAcc + " !";
                        }
                        else if ((item.debitContractCode ?? "") == "" && (debitAcc?.AttachContract ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào hợp đồng bên Nợ cho tài khoản: " + item.debitAcc + " !";
                        }
                        else if ((item.creditContractCode ?? "") == "" && (creditAcc?.AttachContract ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào hợp đồng bên Có cho tài khoản: " + item.creditAcc + " !";
                        }
                        else if ((item.debitFProductWorkCode ?? "") == "" && (debitAcc?.AttachProductCost ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào công trình, sp bên Nợ cho tài khoản: " + item.debitAcc + " !";
                        }
                        else if ((item.creditFProductWorkCode ?? "") == "" && (creditAcc?.AttachProductCost ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào công trình, sp bên Có cho tài khoản: " + item.creditAcc + " !";
                        }
                        else if ((item.debitSectionCode ?? "") == "" && (debitAcc?.AttachAccSection ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào khoản mục bên Nợ cho tài khoản: " + item.debitAcc + " !";
                        }
                        else if ((item.creditSectionCode ?? "") == "" && (creditAcc?.AttachAccSection ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào khoản mục bên Có cho tài khoản: " + item.creditAcc + " !";
                        }
                        else if ((item.debitWorkPlaceCode ?? "") == "" && (debitAcc?.AttachWorkPlace ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào phân xưởng bên Nợ cho tài khoản: " + item.debitAcc + " !";
                        }
                        else if ((item.creditWorkPlaceCode ?? "") == "" && (creditAcc?.AttachWorkPlace ?? "") == "C")
                        {
                            item.errorName = "Bạn phải vào phân xưởng bên Có cho tài khoản: " + item.creditAcc + " !";
                        }
                        else
                        {
                            item.errorName = "";
                        }
                        break;
                    default:
                        item.errorName = "";
                        break;
                }
                dataLedgerError.Add(item);
            }
            dataLedgerCheck = dataLedgerError;
            if ((dto.LstError ?? "") == "" || (dto.LstError ?? "").StartsWith("BeginAcc"))
            {
                foreach (var itemAccCheck in dataAccCheck)
                {
                    var itemAcc = lstAccountSystem.Where(p => p.AccCode == itemAccCheck.accCode).FirstOrDefault();
                    var itemAccPartner = lstAccPartner.Where(p => p.Code == itemAccCheck.partnerCode).FirstOrDefault();
                    var itemContract = lstContract.Where(p => p.Code == itemAccCheck.contractCode).FirstOrDefault();
                    var itemFProductWork = lstFProductWork.Where(p => p.Code == itemAccCheck.fProductWorkCode).FirstOrDefault();
                    var itemSection = lstAccSection.Where(p => p.Code == itemAccCheck.accSectionCode).FirstOrDefault();
                    var itemWorkPlace = lstWorkPlace.Where(p => p.Code == itemAccCheck.workPlaceCode).FirstOrDefault();

                    if ((itemAcc?.AccType ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Tài khoản: " + itemAccCheck.accCode + " là tài khoản tổng hợp!";
                    }
                    else if ((itemAcc?.AccCode ?? "") == "")
                    {
                        itemAccCheck.errorName = "Mã tài khoản: " + itemAccCheck.accCode + " không có!";
                    }
                    else if ((itemAccCheck.partnerCode ?? "") == "" && (itemAcc?.AttachPartner ?? "") == "C")
                    {
                        itemAccCheck.errorName = "Bạn phải vào mã đối tượng cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.partnerCode ?? "") != "" && (itemAcc?.AttachPartner ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Bạn phải xóa mã đối tượng: " + itemAccCheck.partnerCode + " cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.partnerCode ?? "") != "" && (itemAccPartner?.Code ?? "") == "")
                    {
                        itemAccCheck.errorName = "Mã đối tượng: " + itemAccCheck.partnerCode + " không có !";
                    }
                    else if ((itemAccCheck.contractCode ?? "") == "" && (itemAcc?.AttachContract ?? "") == "C")
                    {
                        itemAccCheck.errorName = "Bạn phải vào mã hợp đồng cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.contractCode ?? "") != "" && (itemAcc?.AttachContract ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Bạn phải xóa mã hợp đồng: " + itemAccCheck.contractCode + " cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.contractCode ?? "") != "" && (itemContract?.Code ?? "") == "")
                    {
                        itemAccCheck.errorName = "Mã hợp đồng: " + itemAccCheck.contractCode + " không có !";
                    }
                    else if ((itemAccCheck.fProductWorkCode ?? "") == "" && (itemAcc?.AttachProductCost ?? "") == "C")
                    {
                        itemAccCheck.errorName = "Bạn phải vào mã công trình,sp cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.fProductWorkCode ?? "") != "" && (itemAcc?.AttachProductCost ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Bạn phải xóa mã công trình,sp: " + itemAccCheck.fProductWorkCode + " cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.fProductWorkCode ?? "") != "" && (itemFProductWork?.FPWType ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Công trình,sp: " + itemAccCheck.fProductWorkCode + " là tổng hợp !";
                    }
                    else if ((itemAccCheck.fProductWorkCode ?? "") != "" && (itemFProductWork?.Code ?? "") == "")
                    {
                        itemAccCheck.errorName = "Mã công trình,sp: " + itemAccCheck.fProductWorkCode + " không có !";
                    }
                    else if ((itemAccCheck.accSectionCode ?? "") == "" && (itemAcc?.AttachAccSection ?? "") == "C")
                    {
                        itemAccCheck.errorName = "Bạn phải vào mã khoản mục phí cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.accSectionCode ?? "") != "" && (itemAcc?.AttachAccSection ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Bạn phải xóa mã khoản mục phí: " + itemAccCheck.accSectionCode + " cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.accSectionCode ?? "") != "" && (itemContract?.Code ?? "") == "")
                    {
                        itemAccCheck.errorName = "Mã khoản mục phí: " + itemAccCheck.accSectionCode + " không có !";
                    }
                    else if ((itemAccCheck.workPlaceCode ?? "") == "" && (itemAcc?.AttachWorkPlace ?? "") == "C")
                    {
                        itemAccCheck.errorName = "Bạn phải vào mã phân xưởng cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.workPlaceCode ?? "") != "" && (itemAcc?.AttachWorkPlace ?? "") != "C")
                    {
                        itemAccCheck.errorName = "Bạn phải xóa mã phân xưởng: " + itemAccCheck.workPlaceCode + " cho tài khoản: " + itemAccCheck.accCode;
                    }
                    else if ((itemAccCheck.workPlaceCode ?? "") != "" && (itemContract?.Code ?? "") == "")
                    {
                        itemAccCheck.errorName = "Mã phân xưởng: " + itemAccCheck.workPlaceCode + " không có !";
                    }
                    else
                    {
                        itemAccCheck.errorName = "";
                    }
                }
            }
            if ((dto.LstError ?? "") == "" || (dto.LstError ?? "").StartsWith("BeginProduct"))
            {
                foreach (var itemProductCheck in dataProductCheck)
                {
                    var itemWarehouse = lstWarehouse.Where(p => p.Code == itemProductCheck.warehouseCode).FirstOrDefault();
                    var itemProduct = lstProduct.Where(p => p.Code == itemProductCheck.productCode).FirstOrDefault();
                    var itemProductOrigin = lstProductOrigin.Where(p => p.Code == itemProductCheck.productOriginCode).FirstOrDefault();
                    var itemProductLot = lstProductLot.Where(p => p.Code == itemProductCheck.productLotCode).FirstOrDefault();
                    if ((itemProductCheck.warehouseCode ?? "") == "")
                    {
                        itemProductCheck.errorName = "Bạn phải vào mã kho!";
                    } 
                    else if ((itemProductCheck.warehouseCode ?? "") != "" && (itemWarehouse?.WarehouseType ?? "") != "C" )
                    {
                        itemProductCheck.errorName = "Mã kho " + itemProductCheck.warehouseCode + " là tổng hợp";
                    }
                    else if ((itemProductCheck.warehouseCode ?? "") != "" && (itemWarehouse?.Code ?? "") == "")
                    {
                        itemProductCheck.errorName = "Mã kho " + itemProductCheck.warehouseCode + " không có";
                    }
                    else if ((itemProductCheck.productCode ?? "") == "")
                    {
                        itemProductCheck.errorName = "Bạn phải vào mã hàng hóa!";
                    }
                    else if ((itemProductCheck.productCode ?? "") != "" && (itemProduct.Code ?? "") == "")
                    {
                        itemProductCheck.errorName = "Mã hàng hóa " + itemProductCheck.productCode + " không có";
                    }
                    else if ((itemProductCheck.productLotCode ?? "") == "" && (itemProduct?.AttachProductLot ?? "") == "C")
                    {
                        itemProductCheck.errorName = "Bạn phải vào mã lô cho mã hàng: " + itemProductCheck.productCode + " !";
                    }
                    else if ((itemProductCheck.productLotCode ?? "") == "" && (itemProduct?.AttachProductLot ?? "") != "C")
                    {
                        itemProductCheck.errorName = "Bạn phải xóa mã lô hàng " + itemProductCheck.productLotCode + " của mã hàng: " + itemProductCheck.productCode + " !";
                    }
                    else if ((itemProductCheck.productLotCode ?? "") != "" && (itemProductLot?.Code ?? "") == "")
                    {
                        itemProductCheck.errorName = "Mã lô hàng " + itemProductCheck.productLotCode + " không có";
                    }
                    else if ((itemProductCheck.productOriginCode ?? "") == "" && (itemProduct?.AttachProductOrigin ?? "") == "C")
                    {
                        itemProductCheck.errorName = "Bạn phải vào mã nguồn cho mã hàng: " + itemProductCheck.productCode + " !";
                    }
                    else if ((itemProductCheck.productOriginCode ?? "") == "" && (itemProduct?.AttachProductOrigin ?? "") != "C")
                    {
                        itemProductCheck.errorName = "Bạn phải xóa mã nguồn hàng " + itemProductCheck.productOriginCode + " của mã hàng: " + itemProductCheck.productCode + " !";
                    }
                    else if ((itemProductCheck.productOriginCode ?? "") != "" && (itemProductLot?.Code ?? "") == "")
                    {
                        itemProductCheck.errorName = "Mã nguồn hàng " + itemProductCheck.productOriginCode + " không có";
                    }
                    else
                    {
                        itemProductCheck.errorName = "";
                    }
                } 
            }
            var res = new List<JsonObject>();
            if(dto.Type == "G")
            {
                foreach (var itemFixError in lstFixError)
                {
                    var numErrorLedger = dataLedgerCheck.Where(p => p.errorId == itemFixError.errorId && p.errorName != "").Count();
                    var numErrorAcc = dataAccCheck.Where(p => p.errorId == itemFixError.errorId && p.errorName != "").Count();
                    var numErrorProduct = dataProductCheck.Where(p => p.errorId == itemFixError.errorId && p.errorName != "").Count();
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(itemFixError);
                    var job = (JsonObject)JsonObject.Parse(json);
                    job.Add("errorNumber", numErrorLedger + numErrorAcc + numErrorProduct);
                    res.Add((JsonObject)job);
                }
            } 
            else if (dto.LstError.Contains("BeginAcc"))
            {
                dataAccCheck = dataAccCheck.OrderBy(p => p.accCode).ToList();
                foreach (var itemAccCheckRes in dataAccCheck)
                {
                    if (itemAccCheckRes.errorName != "")
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(itemAccCheckRes);
                        var job = (JsonObject)JsonObject.Parse(json);
                        res.Add((JsonObject)job);
                    }
                }
            }
            else if (dto.LstError.Contains("BeginProduct"))
            {
                dataProductCheck = dataProductCheck.OrderBy(p => p.warehouseCode).ThenBy(p => p.productCode).ToList();
                foreach (var itemProductCheckRes in dataProductCheck)
                {
                    if (itemProductCheckRes.errorName != "")
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(itemProductCheckRes);
                        var job = (JsonObject)JsonObject.Parse(json);
                        res.Add((JsonObject)job);
                    }
                }
            }
            else 
            {
                dataLedgerCheck = dataLedgerCheck.OrderBy(p => p.voucherDate).ThenBy(p => p.voucherNumber).ToList();
                foreach (var item in dataLedgerCheck)
                {
                    if (item.errorName != "")
                    {
                        var json = Newtonsoft.Json.JsonConvert.SerializeObject(item);
                        var job = (JsonObject)JsonObject.Parse(json);
                        res.Add((JsonObject)job);
                    }
                }
            }
            return res;
        }

        public async Task<List<FixErrorDto>> GetList()
        {
            var fixError = await _defaultFixErrorService.GetQueryableAsync();
            var lstFixError = fixError.OrderBy(p => p.ErrorId)
                              .Select(p => new FixErrorDto
                              {
                                  errorId = p.ErrorId,
                                  errorName = p.ErrorName,
                                  tag = p.Tag,
                                  keyError = p.KeyError,
                                  classify = p.Classify,
                                  selectRow = false,
                              }).ToList();
            return lstFixError;
        }
        #region Private
        private async Task<bool> CheckExistedWorkPlace(string accCode)
        {
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var data = lstAccountSystem.Where(x => x.AccCode == accCode).FirstOrDefault();
            if(data.AttachWorkPlace == "C")
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckExistedPartner(string accCode)
        {
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var data = lstAccountSystem.Where(x => x.AccCode == accCode).FirstOrDefault();
            if (data.AttachPartner == "C")
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckExistedContract(string accCode)
        {
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var data = lstAccountSystem.Where(x => x.AccCode == accCode).FirstOrDefault();
            if (data.AttachContract == "C")
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckExistedSection(string accCode)
        {
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var data = lstAccountSystem.Where(x => x.AccCode == accCode).FirstOrDefault();
            if (data.AttachAccSection == "C")
            {
                return true;
            }
            return false;
        }
        private async Task<bool> CheckExistedFProductWork(string accCode)
        {
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var lstAccountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear()).ToList();
            var data = lstAccountSystem.Where(x => x.AccCode == accCode).FirstOrDefault();
            if (data.AttachProductCost == "C")
            {
                return true;
            }
            return false;
        }
        private async Task<List<FixErrorDto>> GetLstFixError(string ErrorKey)
        {
            var fixError = await _fixErrorService.GetData(_webHelper.GetCurrentOrgUnit());
            if (ErrorKey == "FProductWork")
            {
                var lstFixError = fixError.Where(p => (ErrorKey ?? "") == "" || p.KeyError == "FProductWork" && p.OrgCode == _webHelper.GetCurrentOrgUnit())
                .Select(p => new FixErrorDto
                 {
                     errorId = p.ErrorId,
                     errorName = p.ErrorName,
                     tag = p.Tag,
                     keyError = p.KeyError,
                     classify = p.Classify
                 }).ToList();
                return lstFixError;
            }
            else
            {
                var lstFixError = fixError.Where(p => (ErrorKey ?? "") == "" || ErrorKey.Contains(p.KeyError) && p.OrgCode == _webHelper.GetCurrentOrgUnit())
                                     .Select(p => new FixErrorDto
                                     {
                                         errorId = p.ErrorId,
                                         errorName = p.ErrorName,
                                         tag = p.Tag,
                                         keyError = p.KeyError,
                                         classify = p.Classify
                                     }).ToList();
                return lstFixError;
            }
        }
        #endregion
    }
}
