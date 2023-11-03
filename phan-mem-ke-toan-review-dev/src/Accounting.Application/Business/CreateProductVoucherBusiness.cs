using Accounting.BaseDtos.Customines;
using Accounting.Caching;
using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Catgories.ProductVouchers;
using Accounting.Catgories.VoucherCategories;
using Accounting.DomainServices.BusinessCategories;
using Accounting.DomainServices.Categories;
using Accounting.DomainServices.Categories.Others;
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
using Accounting.Vouchers.VoucherExciseTaxs;
using Accounting.Vouchers.VoucherNumbers;
using Accounting.Vouchers.WarehouseBooks;
using Accounting.Windows;
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
    public class CreateProductVoucherBusiness : BaseBusiness, IUnitOfWorkEnabled
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
        private readonly ProductVoucherService _productVoucher;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly DepartmentService _departmentService;
        private readonly CurrencyService _currencyService;
        private readonly BusinessCategoryService _businessCategoryService;
        private readonly ProductLotService _productLotService;
        private readonly VoucherTypeService _voucherTypeService;
        private readonly ContractService _contractService;
        private readonly FProductWorkService _fProductWorkService;
        private readonly AccSectionService _accSectionService;
        private readonly WorkPlaceSevice _workPlaceSevice;
        private readonly AccCaseService _accCaseService;
        private readonly WarehouseAppService _warehouseAppService;
        private readonly WarehouseService _warehouseService;
        private readonly ProductService _product;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductVoucherAssemblyService _productVoucherAssembly;
        private readonly ProductVoucherReceiptService _productVoucherReceipt;
        private readonly ProductVoucherVatService _productVoucherVat;
        private readonly ProductVoucherDetailService _productVoucherDetail;
        private readonly ProductVoucherCostService _productVoucherCostService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        #endregion
        #region Ctor
        public CreateProductVoucherBusiness(VoucherNumberService voucherNumberService,
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
                            ProductVoucherService productVoucherService,
                            VoucherExciseTaxService voucherExciseTaxService,
                            WarehouseBookService warehouseBookService,
                            DepartmentService departmentService,
                            CurrencyService currencyService,
                            BusinessCategoryService businessCategoryService,
                            ProductLotService productLotService,
                            VoucherTypeService voucherTypeService,
                            ContractService contractService,
                            FProductWorkService fProductWorkService,
                            AccSectionService accSectionService,
                            WorkPlaceSevice workPlaceSevice,
                            AccCaseService accCaseService,
                            WarehouseAppService warehouseAppService,
                            WarehouseService warehouseService,
                            ProductService product,
                            ProductUnitService productUnitService,
                            ProductVoucherAssemblyService productVoucherAssemblyService,
                            ProductVoucherReceiptService productVoucherReceiptService,
                            ProductVoucherVatService productVoucherVatService,
                            ProductVoucherDetailService productVoucherDetailService,
                            ProductVoucherCostService productVoucherCostService,
                            ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
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
            _productVoucher = productVoucherService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _warehouseBookService = warehouseBookService;
            _departmentService = departmentService;
            _currencyService = currencyService;
            _businessCategoryService = businessCategoryService;
            _productLotService = productLotService;
            _voucherTypeService = voucherTypeService;
            _contractService = contractService;
            _fProductWorkService = fProductWorkService;
            _accSectionService = accSectionService;
            _workPlaceSevice = workPlaceSevice;
            _accCaseService = accCaseService;
            _warehouseAppService = warehouseAppService;
            _warehouseService = warehouseService;
            _product = product;
            _productUnitService = productUnitService;
            _productVoucherAssembly = productVoucherAssemblyService;
            _productVoucherReceipt = productVoucherReceiptService;
            _productVoucherVat = productVoucherVatService;
            _productVoucherDetail = productVoucherDetailService;
            _productVoucherCostService = productVoucherCostService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
            _voucherNumberBusiness = voucherNumberBusiness;
        }
        #endregion
        #region Methods
        public async Task<ProductVoucherDto> CreateProductVoucherAsync(CrudProductVoucherDto dto)
        {
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            var idProductVoucher = this.GetNewObjectId();
            dto.Id = idProductVoucher;
            dto.Year = _webHelper.GetCurrentYear();
            ErrorDto errorDto = new ErrorDto();
            //Check


            var check = await CheckProductVoucherAsync(dto);
            if (check.Message != null)
            {

                throw new Exception(check.Message);
            }

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

            List<CrudProductVoucherDetailDto> productVoucherDetails = (List<CrudProductVoucherDetailDto>)dto.ProductVoucherDetails;
            if (productVoucherDetails.Count == 0)
            {

                throw new Exception("Chưa nhập chi tiết!");
            }
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();

            if (!string.IsNullOrEmpty(dto.RefVoucher))
            {
                CrudRefVoucherDto crudRefVoucherDto = new CrudRefVoucherDto();
                crudRefVoucherDto.Id = this.GetNewObjectId();
                crudRefVoucherDto.SrcId = dto.RefVoucher;
                crudRefVoucherDto.DestId = idProductVoucher;
                crudRefVoucherDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                var refVoucher = _objectMapper.Map<CrudRefVoucherDto, RefVoucher>(crudRefVoucherDto);
                await _refVoucherService.CreateAsync(refVoucher);

                CrudRefVoucherDto crudRefVoucherDtos = new CrudRefVoucherDto();
                crudRefVoucherDtos.Id = this.GetNewObjectId();
                crudRefVoucherDtos.SrcId = idProductVoucher;
                crudRefVoucherDtos.DestId = dto.RefVoucher;
                crudRefVoucherDtos.OrgCode = _webHelper.GetCurrentOrgUnit();
                var refVouchers = _objectMapper.Map<CrudRefVoucherDto, RefVoucher>(crudRefVoucherDtos);
                await _refVoucherService.CreateAsync(refVouchers);
            }

            List<CrudAccTaxDetailDto> AccTaxDetails = (List<CrudAccTaxDetailDto>)dto.AccTaxDetails;
            for (int i = 0; i < productVoucherDetails.Count; i++)
            {
                var id = this.GetNewObjectId();
                dto.ProductVoucherDetails[i].Id = id;
                dto.ProductVoucherDetails[i].OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.ProductVoucherDetails[i].Year = _webHelper.GetCurrentYear();
                dto.ProductVoucherDetails[i].Ord0 = productVoucherDetails[i].Ord0 = "A" + (i + 1).ToString().PadLeft(9, '0');
                List<CrudProductVoucherDetailReceiptDto> CrudProductVoucherDetailReceiptDto = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetails[0].ProductVoucherDetailReceipts;
                List<CrudProductVoucherDetailReceiptDto> CrudProductVoucherDetail = new List<CrudProductVoucherDetailReceiptDto>();
                CrudProductVoucherDetailReceiptDto = CrudProductVoucherDetail;
                CrudProductVoucherDetailReceiptDto crud = new CrudProductVoucherDetailReceiptDto();
                try
                {
                    crud.Id = this.GetNewObjectId();
                    crud.ProductVoucherDetailId = id;
                    crud.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crud.Year = _webHelper.GetCurrentYear();
                    crud.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                    if (AccTaxDetails != null)
                    {
                        if (AccTaxDetails.Count > 0)
                        {
                            crud.VatPercentage = AccTaxDetails[0].VatPercentage;
                        }

                    }

                    crud.VatAmountCur = productVoucherDetails[i].VatAmountCur;
                    crud.VatAmount = productVoucherDetails[i].VatAmount;
                    crud.VatPercentage = productVoucherDetails[i].VatPercentage;
                    crud.DiscountPercentage = productVoucherDetails[i].DiscountPercentage;
                    crud.DiscountAmount = productVoucherDetails[i].DiscountAmount;
                    crud.DiscountAmountCur = productVoucherDetails[i].DiscountAmountCur;
                    crud.ImportTaxPercentage = productVoucherDetails[i].ImportTaxPercentage;
                    crud.ImportTaxAmountCur = productVoucherDetails[i].ImportTaxAmountCur;
                    crud.ImportTaxAmount = productVoucherDetails[i].ImportTaxAmount;
                    crud.ExciseTaxPercentage = productVoucherDetails[i].ExciseTaxPercentage;
                    crud.ExciseTaxAmountCur = productVoucherDetails[i].ExciseTaxAmountCur;
                    crud.ExciseTaxAmount = productVoucherDetails[i].ExciseTaxAmount;
                    crud.ExpenseAmountCur0 = productVoucherDetails[i].ExpenseAmountCur0;
                    crud.ExpenseAmount0 = productVoucherDetails[i].ExpenseAmount0;
                    crud.ExpenseAmountCur1 = productVoucherDetails[i].ExpenseAmountCur1;
                    crud.ExpenseAmount1 = productVoucherDetails[i].ExpenseAmount1;
                    crud.ExpenseAmountCur = productVoucherDetails[i].ExpenseAmountCur;
                    crud.ExpenseAmount = productVoucherDetails[i].ExpenseAmount;

                    //crud.Ex = productVoucherDetails[i].expenseImportTaxAmount;
                    CrudProductVoucherDetailReceiptDto.Add(crud);
                    productVoucherDetails[i].ProductVoucherDetailReceipts = new List<CrudProductVoucherDetailReceiptDto>();
                    productVoucherDetails[i].ProductVoucherDetailReceipts.Add(crud);
                }
                catch (Exception ex)
                {

                    throw;
                }

            }


            if (AccTaxDetails != null)
            {
                for (int i = 0; i < AccTaxDetails.Count; i++)
                {
                    dto.AccTaxDetails[i].Ord0 = AccTaxDetails[i].Ord0 = "Z" + (i + 1).ToString().PadLeft(9, '0');
                    dto.AccTaxDetails[i].Id = this.GetNewObjectId();
                    dto.AccTaxDetails[i].Year = _webHelper.GetCurrentYear();
                    dto.AccTaxDetails[i].OrgCode = _webHelper.GetCurrentOrgUnit();
                    dto.AccTaxDetails[i].VoucherCode = dto.VoucherCode;
                    dto.AccTaxDetails[i].ProductVoucherId = idProductVoucher;
                    dto.AccTaxDetails[i].VoucherDate = dto.VoucherDate;
                    dto.InvoiceNumber = AccTaxDetails[i].InvoiceNumber;
                }
            }
            List<CrudProductVoucherAssemblyDto> productVoucherAssembly = (List<CrudProductVoucherAssemblyDto>)dto.ProductVoucherAssemblies; ;
            if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
            {
                CrudProductVoucherAssemblyDto crudProductVoucherAssemblyDto = new CrudProductVoucherAssemblyDto();
                crudProductVoucherAssemblyDto.Id = this.GetNewObjectId();
                crudProductVoucherAssemblyDto.Year = _webHelper.GetCurrentYear();
                crudProductVoucherAssemblyDto.ProductVoucherId = idProductVoucher;
                crudProductVoucherAssemblyDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                crudProductVoucherAssemblyDto.AssemblyWarehouseCode = dto.AssemblyWarehouseCode;
                crudProductVoucherAssemblyDto.AssemblyProductCode = dto.AssemblyProductCode;
                crudProductVoucherAssemblyDto.AssemblyUnitCode = dto.AssemblyUnitCode;
                crudProductVoucherAssemblyDto.Quantity = dto.Quantity;
                List<CrudProductVoucherAssemblyDto> cru = new List<CrudProductVoucherAssemblyDto>();
                cru.Add(crudProductVoucherAssemblyDto);
                productVoucherAssembly = cru;

            }
            if (productVoucherAssembly != null)
            {
                dto.ProductVoucherAssemblies = productVoucherAssembly;
            }
            List<CrudProductVoucherReceiptDto> productVoucherReceipts = (List<CrudProductVoucherReceiptDto>)dto.ProductVoucherReceipts;

            CrudProductVoucherReceiptDto crudProductVoucherReceipt = new CrudProductVoucherReceiptDto();
            crudProductVoucherReceipt.Id = this.GetNewObjectId();

            crudProductVoucherReceipt.Year = _webHelper.GetCurrentYear();
            crudProductVoucherReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
            crudProductVoucherReceipt.ProductVoucherId = idProductVoucher;
            crudProductVoucherReceipt.DiscountPercentage = dto.DiscountPercentage != null ? dto.DiscountPercentage : null;
            crudProductVoucherReceipt.DiscountCreditAcc = dto.DiscountCreditAcc != null ? dto.DiscountCreditAcc : null;
            crudProductVoucherReceipt.DiscountDebitAcc = dto.DiscountDebitAcc != null ? dto.DiscountDebitAcc : null;
            crudProductVoucherReceipt.DiscountCreditAcc0 = dto.DiscountCreditAcc0 != null ? dto.DiscountCreditAcc0 : null;
            crudProductVoucherReceipt.DiscountDebitAcc0 = dto.DiscountDebitAcc0 != null ? dto.DiscountDebitAcc0 : null;
            crudProductVoucherReceipt.DiscountAmount0 = dto.DiscountAmount0 != null ? dto.DiscountAmount0 : null;
            crudProductVoucherReceipt.DiscountAmountCur0 = dto.DiscountAmountCur0 != null ? dto.DiscountAmountCur0 : null;
            crudProductVoucherReceipt.DiscountDescription0 = dto.DiscountDescription0 != null ? dto.DiscountDescription0 : null;
            crudProductVoucherReceipt.ImportTaxPercentage = dto.ImportTaxPercentage != null ? dto.ImportTaxPercentage : null;
            crudProductVoucherReceipt.ImportCreditAcc = dto.ImportCreditAcc != null ? dto.ImportCreditAcc : null;
            crudProductVoucherReceipt.ImportDebitAcc = dto.ImportDebitAcc != null ? dto.ImportDebitAcc : null;
            crudProductVoucherReceipt.ImportDescription = dto.ImportDescription != null ? dto.ImportDescription : null;
            crudProductVoucherReceipt.ExciseTaxPercentage = dto.ExciseTaxPercentage != null ? dto.ExciseTaxPercentage : null;
            crudProductVoucherReceipt.ExciseTaxCreditAcc = dto.ExciseTaxCreditAcc != null ? dto.ExciseTaxCreditAcc : null;
            crudProductVoucherReceipt.ExciseTaxDebitAcc = dto.ExciseTaxDebitAcc != null ? dto.ExciseTaxDebitAcc : null;

            List<CrudProductVoucherReceiptDto> crus = new List<CrudProductVoucherReceiptDto>();
            crus.Add(crudProductVoucherReceipt);
            productVoucherReceipts = crus;
            if (productVoucherReceipts != null)
            {
                dto.ProductVoucherReceipts = productVoucherReceipts;
            }
            List<CrudProductVoucherCostDto> ProductVoucherCost = (List<CrudProductVoucherCostDto>)dto.ProductVoucherCostDetails;

            if (ProductVoucherCost != null)
            {
                for (int i = 0; i < ProductVoucherCost.Count; i++)
                {
                    dto.ProductVoucherCostDetails[i].Id = this.GetNewObjectId();
                    dto.ProductVoucherCostDetails[i].ProductVoucherId = idProductVoucher;
                    dto.ProductVoucherCostDetails[i].Year = _webHelper.GetCurrentYear();
                    dto.ProductVoucherCostDetails[i].OrgCode = _webHelper.GetCurrentOrgUnit();
                    dto.ProductVoucherCostDetails[i].Ord0 = ProductVoucherCost[i].Ord0 = "X" + (i + 1).ToString().PadLeft(9, '0');


                }

            }

            var entity = _objectMapper.Map<CrudProductVoucherDto, ProductVoucher>(dto);
            await _productVoucher.CheckLockVoucher(entity);
            try
            {

                var result = await _productVoucher.CreateAsync(entity, true);
                var voucherCategory = await _voucherCategoryService.CheckIsSavingLedgerAsync(entity.VoucherCode, entity.OrgCode);
                if (voucherCategory.IsSavingLedger.ToString() == "C")
                {



                    if (voucherCategory.IsSavingLedger.ToString() != "K")
                    {

                        List<CrudVoucherExciseTaxDto> VoucherExciseTax = await _ledgerService.VoucherExcitaxAsync(dto);
                        if (VoucherExciseTax != null && !VoucherExciseTax.Any())
                        {
                            foreach (var item in VoucherExciseTax)
                            {
                                item.Id = this.GetNewObjectId();

                                var excixeTax = _objectMapper.Map<CrudVoucherExciseTaxDto, VoucherExciseTax>(item);
                                excixeTax = await _voucherExciseTaxService.CreateAsync(excixeTax, true);
                            }
                        }

                        List<CrudVoucherPaymentBookDto> crudVoucherPaymentBookDto = await _ledgerService.CreateVoucherPaymentAsync(dto);
                        int i = 0;
                        foreach (var crudVoucherPaymentBook in crudVoucherPaymentBookDto)
                        {

                            crudVoucherPaymentBook.Id = this.GetNewObjectId();
                            crudVoucherPaymentBook.DocumentId = idProductVoucher;
                            crudVoucherPaymentBook.OrgCode = _webHelper.GetCurrentOrgUnit();
                            crudVoucherPaymentBook.PartnerCode = entity.PartnerCode0;
                            var crudVoucherPayment = _objectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(crudVoucherPaymentBook);
                            crudVoucherPayment = await _voucherPaymentBookService.CreateAsync(crudVoucherPayment, true);
                        }

                        try
                        {
                            if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
                            {
                                List<CrudLedgerDto> ledgers = await _ledgerService.CreateLedgerLrAsync(dto);
                                foreach (var ledger in ledgers)
                                {
                                    ledger.Id = this.GetNewObjectId();
                                    var ledgerEntity = _objectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                                    ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                                }
                            }
                            else
                            {
                                List<CrudLedgerDto> ledgers = await _ledgerService.CreateLedgerAsync(dto);
                                foreach (var ledger in ledgers)
                                {
                                    ledger.Id = this.GetNewObjectId();
                                    var ledgerEntity = _objectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                                    ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            await _unitOfWorkManager.Current.RollbackAsync();
                            throw;
                        }

                    }

                    if (voucherCategory.IsSavingWarehouseBook.ToString() != "K")
                    {
                        if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
                        {

                            List<CrudWarehouseBookDto> WarehouseBook = await _warehouseBookService.CreatewarehouseBookLrAsync(dto);
                            foreach (var WarehouseBooks in WarehouseBook)
                            {
                                WarehouseBooks.Id = this.GetNewObjectId();
                                var wareBookEntity = _objectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(WarehouseBooks);
                                wareBookEntity = await _warehouseBookService.CreateAsync(wareBookEntity, true);
                            }
                        }
                        else
                        {
                            List<CrudWarehouseBookDto> WarehouseBook = await _warehouseBookService.CreatewarehouseBookAsync(dto);
                            foreach (var WarehouseBooks in WarehouseBook)
                            {
                                WarehouseBooks.Id = this.GetNewObjectId();
                                var wareBookEntity = _objectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(WarehouseBooks);
                                wareBookEntity = await _warehouseBookService.CreateAsync(wareBookEntity, true);
                            }
                        }
                    }
                    return _objectMapper.Map<ProductVoucher, ProductVoucherDto>(result);


                }
            }
            catch (Exception ex)
            {

                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

            return _objectMapper.Map<ProductVoucher, ProductVoucherDto>(entity);
        }

        public async Task<ErrorDto> CheckProductVoucherAsync(CrudProductVoucherDto dto)
        {

            ErrorDto errorDto = new ErrorDto();
            if (string.IsNullOrEmpty(dto.VoucherDate.ToString()))
            {
                errorDto.Code = "101";
                errorDto.Message = "Bạn phải nhập ngày chứng từ!";
                return errorDto;
            }
            else
            {
                DateTime date = DateTime.Parse(dto.VoucherDate.ToString());
                int year = date.Year;
                if (year != _webHelper.GetCurrentYear())
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Ngày chứng từ không nằm trong năm làm việc!";
                    return errorDto;
                }
            }


            var voucher = await _accountingCacheManager.GetVoucherCategoryAsync();
            var lstVoucher = voucher.Where(p => p.Code == dto.VoucherCode).ToList().FirstOrDefault();
            if (string.IsNullOrEmpty(lstVoucher.BookClosingDate.ToString()) && dto.VoucherDate <= lstVoucher.BookClosingDate)
            {
                errorDto.Code = "101";
                errorDto.Message = "Chứng từ trước ngày khóa sổ " + dto.VoucherDate + "không thể thêm, sửa, xóa!";
                return errorDto;

            }
            if (string.IsNullOrEmpty(lstVoucher.BusinessBeginningDate.ToString()) && string.IsNullOrEmpty(lstVoucher.BusinessEndingDate.ToString()) && dto.VoucherDate > lstVoucher.BusinessBeginningDate && dto.VoucherDate < lstVoucher.BusinessEndingDate)
            {
                errorDto.Code = "101";
                errorDto.Message = "Ngày chứng từ không được nằm ngoài " + lstVoucher.BusinessBeginningDate.ToString() + " - " + lstVoucher.BusinessEndingDate.ToString();
                return errorDto;

            }

            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                var department = await _departmentService.GetQueryableAsync();
                var lstDepartment = department.Where(p => p.Code == dto.DepartmentCode && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                if (lstDepartment.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã bộ phận " + dto.DepartmentCode + " không tồn tại!";
                    return errorDto;
                }

            }
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {

                var department = await _departmentService.GetQueryableAsync();
                var lstDepartment = department.Where(p => p.Code == dto.DepartmentCode && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                var lstDepartmentPartner = department.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.DepartmentCode && p.DepartmentType != "C").ToList();
                if (lstDepartmentPartner.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã bộ phận  là bộ phận mẹ " + dto.DepartmentCode;
                    return errorDto;
                }
            }
            if (string.IsNullOrEmpty(dto.CurrencyCode) == true)
            {
                errorDto.Code = "101";
                errorDto.Message = "Chưa nhập mã ngoại tệ!";
                return errorDto;
            }
            else
            {
                var curency = await _currencyService.GetQueryableAsync();
                var lstcurency = curency.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.CurrencyCode).ToList();
                if (lstcurency.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã ngoại tệ " + dto.CurrencyCode + " không tồn tại!";
                    return errorDto;
                }
            }
            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {

                var businiss = await _businessCategoryService.GetQueryableAsync();
                var lstBusiniss = businiss.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.BusinessCode).ToList();

                if (lstBusiniss.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Hạch toán " + dto.BusinessCode + " không tồn tại!";
                    return errorDto;
                }

                if (lstBusiniss.Count > 0)
                {
                    if (lstBusiniss[0].VoucherCode != dto.VoucherCode)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Mã hạch toán không khớp với mã chứng từ!";
                        return errorDto;
                    }

                }
            }
            var accCode = await _accountSystemService.GetQueryableAsync();
            var lstAccCode = accCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            if (string.IsNullOrEmpty(dto.DevaluationDebitAcc) == false)
            {
                var acc = lstAccCode.Where(p => p.AccCode == dto.DevaluationDebitAcc).ToList();
                if (acc.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DevaluationDebitAcc + " không tồn tại!!";
                    return errorDto;
                }
                var accPartner = lstAccCode.Where(p => p.AccCode == dto.DevaluationDebitAcc && p.AccType != "C").ToList();
                if (accPartner.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DevaluationDebitAcc + " là tài khoản mẹ!";
                    return errorDto;
                }
                var accDevaCredit = lstAccCode.Where(p => p.AccCode == dto.DevaluationCreditAcc).ToList();
                if (accDevaCredit.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DevaluationCreditAcc + " không tồn tại!!";
                    return errorDto;
                }
                var accPartnerCredit = lstAccCode.Where(p => p.AccCode == dto.DevaluationCreditAcc && p.AccType != "C").ToList();
                if (accPartnerCredit.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DevaluationCreditAcc + " là tài khoản mẹ!";
                    return errorDto;
                }


                var accDisountDe = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc).ToList();
                if (accDisountDe.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountDebitAcc + " không tồn tại!!";
                    return errorDto;
                }
                var accDiscountDedit = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc && p.AccType != "C").ToList();
                if (accDiscountDedit.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountDebitAcc + " là tài khoản mẹ!";
                    return errorDto;
                }

                var discountCreditAcc = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc).ToList();
                if (discountCreditAcc.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountCreditAcc + " không tồn tại!!";
                    return errorDto;
                }
                var discountCreditAccPart = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc && p.AccType != "C").ToList();
                if (discountCreditAccPart.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountCreditAcc + " là tài khoản mẹ!";
                    return errorDto;
                }



                var accDisountDe0 = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc0).ToList();
                if (accDisountDe0.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountDebitAcc0 + " không tồn tại!!";
                    return errorDto;
                }
                var accDiscountDedit0 = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc0 && p.AccType != "C").ToList();
                if (accDiscountDedit.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountDebitAcc0 + " là tài khoản mẹ!";
                    return errorDto;
                }

                var discountCreditAcc0 = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc0).ToList();
                if (discountCreditAcc.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountCreditAcc0 + " không tồn tại!!";
                    return errorDto;
                }
                var discountCreditAccPart0 = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc0 && p.AccType != "C").ToList();
                if (discountCreditAccPart.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.DiscountCreditAcc0 + " là tài khoản mẹ!";
                    return errorDto;
                }
            }
            if (string.IsNullOrEmpty(dto.ImportCreditAcc) == false)
            {
                var importCreditAcc = lstAccCode.Where(p => p.AccCode == dto.ImportCreditAcc).ToList();
                if (importCreditAcc.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.ImportCreditAcc + " không tồn tại!!";
                    return errorDto;
                }
                var accountSystems = lstAccCode.Where(p => p.AccCode == dto.ImportCreditAcc && p.AccType != "C").ToList();
                if (accountSystems.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.ImportCreditAcc + " là tài khoản mẹ!";
                    return errorDto;
                }
            }
            if (!string.IsNullOrEmpty(dto.ExciseTaxDebitAcc))
            {
                var exciseTaxDebitAcc = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxDebitAcc).ToList();
                if (exciseTaxDebitAcc.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.ExciseTaxCreditAcc + " không tồn tại!!";
                    return errorDto;
                }
                var accountSystems = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxDebitAcc && p.AccType != "C").ToList();
                if (accountSystems.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.ExciseTaxDebitAcc + " là tài khoản mẹ!";
                    return errorDto;
                }
            }

            if (!string.IsNullOrEmpty(dto.ExciseTaxCreditAcc))
            {
                var exciseTaxCreditAcc = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxCreditAcc).ToList();
                if (exciseTaxCreditAcc.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.ExciseTaxCreditAcc + " không tồn tại!!";
                    return errorDto;
                }
                var accountSystems = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxCreditAcc && p.AccType != "C").ToList();
                if (accountSystems.Count > 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã tài khoản " + dto.ExciseTaxCreditAcc + " là tài khoản mẹ!";
                    return errorDto;
                }

            }
            if (lstVoucher.IsAssembly == "C")
            {
                List<CrudProductVoucherDetailDto> productVoucherDetails = dto.ProductVoucherDetails;
                foreach (var item in productVoucherDetails)
                {
                    if (string.IsNullOrEmpty(dto.AssemblyWarehouseCode))
                    {

                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã kho! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "AssemblyWarehouseCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    var wareHouse = await _warehouseService.GetQueryableAsync();
                    var lstWareHouse = wareHouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.AssemblyWarehouseCode).ToList();
                    if (lstWareHouse.Count == 0 && !string.IsNullOrEmpty(dto.AssemblyWarehouseCode))
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Mã kho " + dto.AssemblyWarehouseCode + " không tồn tại!";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "AssemblyWarehouseCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    var lstWareHousePartner = wareHouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.AssemblyWarehouseCode && p.WarehouseType == "K").ToList();
                    if (lstWareHousePartner.Count > 0)
                    {

                        errorDto.Code = "101";
                        errorDto.Message = "Mã kho " + dto.AssemblyWarehouseCode + " là kho mẹ!";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "TransWarehouseCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;




                    }
                    if (string.IsNullOrEmpty(dto.AssemblyProductCode))
                    {

                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã hàng! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "AssemblyProductCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (!string.IsNullOrEmpty(dto.AssemblyProductCode))
                    {
                        var product = await _product.GetQueryableAsync();
                        var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == dto.AssemblyProductCode).ToList();
                        if (lstProduct.Count == 0)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Mã hàng " + dto.AssemblyProductCode + "không tồn tại!";
                            InforDto inforDto = new InforDto();
                            inforDto.FieldName = "AssemblyProductCode";
                            inforDto.TabTable = "ProductVoucherDetail";
                            inforDto.RowNumber = item.Ord0;
                            List<InforDto> inforDtos = new List<InforDto>();
                            inforDtos.Add(inforDto);
                            errorDto.inforDtos = inforDtos;
                            return errorDto;

                        }
                    }
                    if (!string.IsNullOrEmpty(dto.AssemblyUnitCode))
                    {
                        var unit = await _productUnitService.GetQueryableAsync();
                        var lstUnit = unit.Where(p => p.Ord0 == _webHelper.GetCurrentOrgUnit() && dto.AssemblyProductCode == p.ProductCode && p.UnitCode == dto.AssemblyUnitCode).ToList().FirstOrDefault();
                        if (lstUnit != null)
                        {
                            if (string.IsNullOrEmpty(lstUnit.UnitCode) == true)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã DVT " + dto.AssemblyProductCode + "đã tồn tại!";
                                InforDto inforDto = new InforDto();
                                inforDto.FieldName = "TransUnitCode";
                                inforDto.TabTable = "ProductVoucherDetail";
                                inforDto.RowNumber = item.Ord0;
                                List<InforDto> inforDtos = new List<InforDto>();
                                inforDtos.Add(inforDto);
                                errorDto.inforDtos = inforDtos;
                                return errorDto;
                            }
                        }
                    }
                    //var productLotCode = await _productLotService.GetQueryableAsync();
                    //if (string.IsNullOrEmpty(dto.lot))
                    //{
                    //    var product = await _product.GetQueryableAsync();
                    //    var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.TransProductCode).ToList().FirstOrDefault();
                    //    if (lstProduct.AttachProductLot == "C")
                    //    {
                    //        errorDto.Code = "101";
                    //        errorDto.Message = "Chưa nhập mã lô hàng!";
                    //        InforDto inforDto = new InforDto();
                    //        inforDto.FieldName = "TransProductLotCode";
                    //        inforDto.TabTable = "ProductVoucherDetail";
                    //        inforDto.RowNumber = item.Ord0;
                    //        List<InforDto> inforDtos = new List<InforDto>();
                    //        inforDtos.Add(inforDto);
                    //        errorDto.inforDtos = inforDtos;
                    //        return errorDto;
                    //    }
                    //}

                    //if (!string.IsNullOrEmpty(item.TransProductLotCode))
                    //{
                    //    var lstproductLotCode = productLotCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.TransProductLotCode).FirstOrDefault();
                    //    var product = await _product.GetQueryableAsync();
                    //    var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.TransProductCode).ToList().FirstOrDefault();
                    //    if (lstProduct.AttachProductLot == "C" && string.IsNullOrEmpty(lstproductLotCode.Code) == true)
                    //    {
                    //        errorDto.Code = "101";
                    //        errorDto.Message = "Bạn không ghi được. Mã lô hàng " + item.TransProductLotCode + " đã tồn tại!";
                    //        InforDto inforDto = new InforDto();
                    //        inforDto.FieldName = "TransProductLotCode";
                    //        inforDto.TabTable = "ProductVoucherDetail";
                    //        inforDto.RowNumber = item.Ord0;
                    //        List<InforDto> inforDtos = new List<InforDto>();
                    //        inforDtos.Add(inforDto);
                    //        errorDto.inforDtos = inforDtos;
                    //        return errorDto;
                    //    }
                    //}
                    //if (!string.IsNullOrEmpty(item.TransProductLotCode))
                    //{
                    //    var lstproductLotCode = productLotCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.TransProductLotCode).FirstOrDefault();
                    //    var product = await _product.GetQueryableAsync();
                    //    var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.TransProductCode).ToList().FirstOrDefault();
                    //    if (lstProduct.AttachProductLot == "K")
                    //    {
                    //        errorDto.Code = "101";
                    //        errorDto.Message = "Bạn phải xóa mã lô hàng  " + item.TransProductLotCode + " (mã hàng ko theo dõi theo lô)!";
                    //        InforDto inforDto = new InforDto();
                    //        inforDto.FieldName = "TransProductLotCode";
                    //        inforDto.TabTable = "ProductVoucherDetail";
                    //        inforDto.RowNumber = item.Ord0;
                    //        List<InforDto> inforDtos = new List<InforDto>();
                    //        inforDtos.Add(inforDto);
                    //        errorDto.inforDtos = inforDtos;
                    //        return errorDto;
                    //    }
                    //}
                    if (item.Quantity == 0)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Bạn phải nhập số lượng lắp ráp, tháo dỡ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "Quantity";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                }

            }
            var products = await _product.GetQueryableAsync();
            var productUnit = await _productUnitService.GetQueryableAsync();
            var productLot = await _productLotService.GetQueryableAsync();
            //check detail
            if (dto.VoucherGroup < 4)
            {
                List<CrudProductVoucherDetailDto> crudProductVoucherDetailDtos = dto.ProductVoucherDetails;
                foreach (var item in crudProductVoucherDetailDtos)
                {
                    var wareHouse = await _warehouseService.GetQueryableAsync();
                    var lstWareHouse = wareHouse.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                    if (string.IsNullOrEmpty(item.WarehouseCode) == true)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã kho!";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "WarehouseCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;

                    }
                    if (!string.IsNullOrEmpty(item.WarehouseCode))
                    {
                        var lstWareHouse2 = lstWareHouse.Where(p => p.Code == item.WarehouseCode).ToList().FirstOrDefault();

                        if (lstWareHouse2 != null)
                        {
                            if (lstWareHouse2.Code == null)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã kho " + item.WarehouseCode + " không tồn tại!";
                                InforDto inforDto = new InforDto();
                                inforDto.FieldName = "WarehouseCode";
                                inforDto.TabTable = "ProductVoucherDetail";
                                inforDto.RowNumber = item.Ord0;
                                List<InforDto> inforDtos = new List<InforDto>();
                                inforDtos.Add(inforDto);
                                errorDto.inforDtos = inforDtos;
                                return errorDto;


                            }
                            var lstWareHousesss = await _warehouseAppService.GetChildGroup(item.WarehouseCode);

                            if (lstWareHousesss.Count > 1)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã kho " + item.WarehouseCode + " là kho mẹ!";
                                InforDto inforDto = new InforDto();
                                inforDto.FieldName = "WarehouseCode";
                                inforDto.TabTable = "ProductVoucherDetail";
                                inforDto.RowNumber = item.Ord0;
                                List<InforDto> inforDtos = new List<InforDto>();
                                inforDtos.Add(inforDto);
                                errorDto.inforDtos = inforDtos;
                                return errorDto;
                            }
                        }
                        var lstTran = lstWareHouse.Where(p => p.Code == item.TransWarehouseCode).ToList().FirstOrDefault();
                        if (lstTran != null)
                        {
                            if (lstTran.Code == null)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã kho " + item.WarehouseCode + " không tồn tại!";
                                InforDto inforDto = new InforDto();
                                inforDto.FieldName = "WarehouseCode";
                                inforDto.TabTable = "ProductVoucherDetail";
                                inforDto.RowNumber = item.Ord0;
                                List<InforDto> inforDtos = new List<InforDto>();
                                inforDtos.Add(inforDto);
                                errorDto.inforDtos = inforDtos;
                                return errorDto;
                            }

                        }
                        if (lstTran != null)
                        {
                            if (lstTran.WarehouseType == "K")
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã kho " + item.WarehouseCode + " là kho mẹ!";
                                InforDto inforDto = new InforDto();
                                inforDto.FieldName = "WarehouseCode";
                                inforDto.TabTable = "ProductVoucherDetail";
                                inforDto.RowNumber = item.Ord0;
                                List<InforDto> inforDtos = new List<InforDto>();
                                inforDtos.Add(inforDto);
                                errorDto.inforDtos = inforDtos;
                                return errorDto;
                            }
                        }

                        if (string.IsNullOrEmpty(item.ProductCode) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Chưa nhập mã hàng!";
                            InforDto inforDto = new InforDto();
                            inforDto.FieldName = "ProductCode";
                            inforDto.TabTable = "ProductVoucherDetail";
                            inforDto.RowNumber = item.Ord0;
                            List<InforDto> inforDtos = new List<InforDto>();
                            inforDtos.Add(inforDto);
                            errorDto.inforDtos = inforDtos;
                            return errorDto;
                        }

                        if (!string.IsNullOrEmpty(item.ProductCode))
                        {
                            var lstProduct = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ProductCode).ToList().FirstOrDefault();
                            if (lstProduct != null)
                            {
                                if (string.IsNullOrEmpty(lstProduct.Code) == true)
                                {
                                    errorDto.Code = "101";
                                    errorDto.Message = "Bạn không ghi được. Mã hàng hóa " + item.ProductCode + " không tồn tại!!";
                                    InforDto inforDto = new InforDto();
                                    inforDto.FieldName = "ProductCode";
                                    inforDto.TabTable = "ProductVoucherDetail";
                                    inforDto.RowNumber = item.Ord0;
                                    List<InforDto> inforDtos = new List<InforDto>();
                                    inforDtos.Add(inforDto);
                                    errorDto.inforDtos = inforDtos;
                                    return errorDto;

                                }
                            }

                        }
                        var lstProductTran = products.Where(p => p.Code == item.TransProductCode).ToList().FirstOrDefault();
                        if (!string.IsNullOrEmpty(item.TransProductCode))
                        {
                            if (lstProductTran != null)
                            {
                                if (string.IsNullOrEmpty(lstProductTran.Code) == true)
                                {

                                    errorDto.Code = "101";
                                    errorDto.Message = "  Bạn không ghi được. Mã hàng hóa " + item.ProductCode + " đã tồn tại!";
                                    InforDto inforDto = new InforDto();
                                    inforDto.FieldName = "ProductCode";
                                    inforDto.TabTable = "ProductVoucherDetail";
                                    inforDto.RowNumber = item.Ord0;
                                    List<InforDto> inforDtos = new List<InforDto>();
                                    inforDtos.Add(inforDto);
                                    errorDto.inforDtos = inforDtos;
                                    return errorDto;

                                }
                            }


                        }
                        //if (!string.IsNullOrEmpty(item.UnitCode))
                        //{
                        //    var lst = productUnit.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.ProductCode == item.ProductCode && p.UnitCode == item.UnitCode).ToList();
                        //    if (lst.Count > 0)
                        //    {
                        //        errorDto.Code = "101";
                        //        errorDto.Message = "Bạn không ghi được. Đơn vị tính " + item.UnitCode + " đã tồn tại!";
                        //        InforDto inforDto = new InforDto();
                        //        inforDto.FieldName = "UnitCode";
                        //        inforDto.TabTable = "ProductVoucherDetail";
                        //        inforDto.RowNumber = item.Ord0;
                        //        List<InforDto> inforDtos = new List<InforDto>();
                        //        inforDtos.Add(inforDto);
                        //        errorDto.inforDtos = inforDtos;
                        //        return errorDto;
                        //    }
                        //}
                        var lstPro = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ProductCode).ToList().FirstOrDefault();
                        if (string.IsNullOrEmpty(item.ProductLotCode) == true)
                        {
                            if (lstPro != null)
                            {
                                if (lstPro.AttachProductLot == "C")
                                {
                                    errorDto.Code = "101";
                                    errorDto.Message = "Chưa nhập mã lô hàng!";
                                    InforDto inforDto = new InforDto();
                                    inforDto.FieldName = "ProductLotCode";
                                    inforDto.TabTable = "ProductVoucherDetail";
                                    inforDto.RowNumber = item.Ord0;
                                    List<InforDto> inforDtos = new List<InforDto>();
                                    inforDtos.Add(inforDto);
                                    errorDto.inforDtos = inforDtos;
                                    return errorDto;

                                }
                            }


                        }
                        if (string.IsNullOrEmpty(item.ProductOriginCode) == true)
                        {
                            if (lstPro != null)
                            {
                                if (lstPro.AttachProductOrigin == "C")
                                {
                                    errorDto.Code = "101";
                                    errorDto.Message = "Chưa nhập mã nguồn hàng!";
                                    InforDto inforDto = new InforDto();
                                    inforDto.FieldName = "ProductOriginCode";
                                    inforDto.TabTable = "ProductVoucherDetail";
                                    inforDto.RowNumber = item.Ord0;
                                    List<InforDto> inforDtos = new List<InforDto>();
                                    inforDtos.Add(inforDto);
                                    errorDto.inforDtos = inforDtos;
                                    return errorDto;
                                }
                            }

                        }
                        if (!string.IsNullOrEmpty(item.TransProductCode))
                        {
                            var lstProdu = products.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.TransProductCode).ToList().FirstOrDefault();
                            if (lstProdu != null)
                            {
                                if (item.UnitCode != lstProdu.UnitCode)
                                {


                                    errorDto.Code = "101";
                                    errorDto.Message = " Đơn vị tính hàng chuyển đổi không khớp mã hàng: " + item.TransProductCode + "- đơn vị tính: " + item.UnitCode + "!";
                                    InforDto inforDto = new InforDto();
                                    inforDto.FieldName = "TransProductCode";
                                    inforDto.TabTable = "ProductVoucherDetail";
                                    inforDto.RowNumber = item.Ord0;
                                    List<InforDto> inforDtos = new List<InforDto>();
                                    inforDtos.Add(inforDto);
                                    errorDto.inforDtos = inforDtos;
                                    return errorDto;
                                }
                            }

                        }
                    }

                }
            }


            //check ht
            var voucherType = await _accountingCacheManager.GetVoucherTypeAsync();
            var lst = voucherType.ToList();
            var lstVoucherType1 = voucherType.Where(p => p.Code == "000").ToList().FirstOrDefault();
            var lstBoucherType2 = voucherType.Where(p => p.Code == "NO1").ToList().FirstOrDefault();
            //List<CrudAccVoucherDetailDto> crudAccVoucherDetailDtos = new List<CrudAccVoucherDetailDto>();

            List<CrudProductVoucherDetailDto> lstcrudProductVoucherDetailDtos = dto.ProductVoucherDetails;
            var crudAccVoucherDetailDtos = (from a in lstcrudProductVoucherDetailDtos
                                            where 1 == 0
                                            select new CrudProductvoucherCheckDto
                                            {
                                                Ord0 = a.Ord0,
                                                Year = a.Year,
                                                OrgCode = a.OrgCode,
                                                DebitAcc = a.DebitAcc,
                                                PartnerCode = a.PartnerCode,
                                                PartnerCode0 = "",
                                                ContractCode = a.ContractCode,
                                                FProductWorkCode = a.FProductWorkCode,
                                                CaseCode = a.CaseCode,
                                                WorkPlaceCode = a.WorkPlaceCode,
                                                CreditAcc = a.CreditAcc,
                                                ClearingPartnerCode = "",
                                                ClearingFProductWorkCode = "",
                                                ClearingContractCode = "",
                                                ClearingSectionCode = "",
                                                ClearingWorkPlaceCode = "",
                                                SectionCode = a.SectionCode,
                                                DebitAcc2 = a.DebitAcc2,
                                                CreditAcc2 = a.CreditAcc2,
                                            }).ToList();
            if (lstVoucherType1 != null)
            {
                if (lstVoucherType1.ListVoucher.Contains(dto.VoucherCode) == false)
                {
                    foreach (var item in lstcrudProductVoucherDetailDtos)
                    {
                        CrudProductvoucherCheckDto crud = new CrudProductvoucherCheckDto();
                        crud.Ord0 = item.Ord0;
                        crud.Year = item.Year;
                        crud.OrgCode = item.OrgCode;
                        crud.DebitAcc = item.DebitAcc;
                        crud.PartnerCode = string.IsNullOrEmpty(item.PartnerCode) == true ? dto.PartnerCode : item.PartnerCode;
                        crud.ContractCode = item.ContractCode;
                        crud.FProductWorkCode = item.FProductWorkCode;
                        crud.CaseCode = item.CaseCode;
                        crud.WorkPlaceCode = item.WorkPlaceCode;
                        crud.CreditAcc = item.CreditAcc;
                        crud.ClearingPartnerCode = "";
                        crud.ClearingFProductWorkCode = "";
                        crud.ClearingContractCode = "";
                        crud.ClearingSectionCode = "";
                        crud.ClearingWorkPlaceCode = "";
                        crud.SectionCode = item.SectionCode;
                        crud.DebitAcc2 = item.DebitAcc2;
                        crud.CreditAcc2 = item.CreditAcc2;
                        crudAccVoucherDetailDtos.Add(crud);
                    }

                }
            }

            if (lstBoucherType2.ListVoucher != null)
            {
                if (lstBoucherType2.ListVoucher.Contains(dto.VoucherCode) == true)
                {
                    foreach (var item in lstcrudProductVoucherDetailDtos)
                    {
                        CrudProductvoucherCheckDto crud = new CrudProductvoucherCheckDto();
                        crud.Ord0 = item.Ord0;
                        crud.Year = item.Year;
                        crud.OrgCode = item.OrgCode;
                        crud.DebitAcc = item.DebitAcc2;
                        crud.PartnerCode = string.IsNullOrEmpty(item.PartnerCode) == true ? dto.PartnerCode : item.PartnerCode;
                        crud.ContractCode = item.ContractCode;
                        crud.FProductWorkCode = item.FProductWorkCode;
                        crud.CaseCode = item.CaseCode;
                        crud.WorkPlaceCode = item.WorkPlaceCode;
                        crud.CreditAcc = item.CreditAcc2;
                        crud.ClearingPartnerCode = "";
                        crud.ClearingFProductWorkCode = "";
                        crud.ClearingContractCode = "";
                        crud.ClearingSectionCode = "";
                        crud.ClearingWorkPlaceCode = "";
                        crud.SectionCode = item.SectionCode;
                        crud.DebitAcc2 = item.DebitAcc2;
                        crud.CreditAcc2 = item.CreditAcc2;
                        crudAccVoucherDetailDtos.Add(crud);
                    }

                }
            }

            List<CrudProductVoucherCostDto> crudProductVoucherCostDtos = dto.ProductVoucherCostDetails;
            if (crudProductVoucherCostDtos != null)
            {
                foreach (var item in crudProductVoucherCostDtos)
                {
                    CrudProductvoucherCheckDto crud = new CrudProductvoucherCheckDto();
                    crud.Ord0 = item.Ord0;
                    crud.Year = item.Year;
                    crud.OrgCode = item.OrgCode;
                    crud.DebitAcc = item.DebitAcc;
                    crud.PartnerCode = item.PartnerCode;
                    crud.ContractCode = item.ContractCode;
                    crud.FProductWorkCode = item.FProductWorkCode;
                    crud.CaseCode = item.CaseCode;
                    crud.WorkPlaceCode = item.WorkPlaceCode;
                    crud.CreditAcc = item.CreditAcc;
                    crud.ClearingPartnerCode = item.ClearingPartnerCode;
                    crud.ClearingFProductWorkCode = item.ClearingFProductWorkCode;
                    crud.ClearingContractCode = item.ClearingContractCode;
                    crud.ClearingSectionCode = item.ClearingSectionCode;
                    crud.ClearingWorkPlaceCode = item.ClearingFProductWorkCode;
                    crud.SectionCode = item.SectionCode;

                    crudAccVoucherDetailDtos.Add(crud);
                }
            }

            var partner = await _accPartnerService.GetQueryableAsync();
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            var contract = await _contractService.GetQueryableAsync();
            var fpProduct = await _fProductWorkService.GetQueryableAsync();
            var sessionCode = await _accSectionService.GetQueryableAsync();
            var workplace = await _workPlaceSevice.GetQueryableAsync();
            var caseCode = await _accCaseService.GetQueryableAsync();
            foreach (var item in crudAccVoucherDetailDtos)
            {
                var acaseCode1 = caseCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.CaseCode).ToList();
                var workplace1 = workplace.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.WorkPlaceCode).ToList();
                var workplace2 = workplace.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ClearingWorkPlaceCode).ToList();
                var sessionCode1 = sessionCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.SectionCode).ToList();
                var sessionCode2 = sessionCode.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ClearingSectionCode).ToList();
                var fpProduct1 = fpProduct.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.FProductWorkCode).ToList();
                var fpProduct2 = fpProduct.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ClearingFProductWorkCode).ToList();
                var contract1 = contract.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ContractCode).ToList();
                var contrac2 = contract.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ClearingContractCode).ToList();
                var partner1 = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.PartnerCode).ToList();
                var partner2 = partner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Code == item.ClearingPartnerCode).ToList();
                var lstaccountSystem1 = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == item.DebitAcc).FirstOrDefault();
                var lstaccountSystem2 = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == item.CreditAcc).FirstOrDefault();


                var lstaccountSystemHKD1 = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == item.DebitAcc2).FirstOrDefault();
                var lstaccountSystemHKD2 = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear() && p.AccCode == item.CreditAcc2).FirstOrDefault();

                if (lstaccountSystem2 != null)
                {
                    if (lstaccountSystem2.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            var partners = await _accPartnerService.GetQueryableAsync();
                            var lstPartner = partners.Where(p => p.Code == dto.PartnerCode0 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                            if (lstPartner.Count == 0)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(item.DebitAcc) == true && lstaccountSystem2.IsBalanceSheetAcc != "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập tài khoản! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "DebitAcc";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.CreditAcc) == true && string.IsNullOrEmpty(lstaccountSystem2.AccCode) == true)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Mã tài khoản " + item.CreditAcc + " không tồn tại! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "CreditAcc";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    //if (!string.IsNullOrEmpty(item.PartnerCode0) && string.IsNullOrEmpty(item.ClearingPartnerCode) == true && lstaccountSystem2.AttachPartner == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã đối tượng ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "PartnerCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (!string.IsNullOrEmpty(item.PartnerCode) == true && string.IsNullOrEmpty(item.PartnerCode0) && string.IsNullOrEmpty(item.ClearingPartnerCode) == true && lstaccountSystem2.AttachPartner == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã đối tượng ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "PartnerCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    if (string.IsNullOrEmpty(item.ContractCode) == true && string.IsNullOrEmpty(item.ClearingContractCode) == true && lstaccountSystem2.AttachContract == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã hợp đồng ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "ContractCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.FProductWorkCode) == true && string.IsNullOrEmpty(item.ClearingFProductWorkCode) == true && lstaccountSystem2.AttachProductCost == "C" && dto.VoucherCode != "PX8")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã chứng từ sản phẩm ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "FProductWorkCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.WorkPlaceCode) == true && string.IsNullOrEmpty(item.WorkPlaceCode) == true && lstaccountSystem2.AttachWorkPlace == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã phân xưởng ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "WorkPlaceCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                }
                if (lstaccountSystem1 != null)
                {
                    if (lstaccountSystem1.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            var partners = await _accPartnerService.GetQueryableAsync();
                            var lstPartner = partners.Where(p => p.Code == dto.PartnerCode0 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                            if (lstPartner.Count == 0)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(item.DebitAcc) == true && string.IsNullOrEmpty(lstaccountSystem1.AccCode) == true)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Mã tài khoản " + item.DebitAcc + " không tồn tại! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "DebitAcc";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }

                    if (string.IsNullOrEmpty(dto.PartnerCode0) == true && lstaccountSystem1.AttachPartner == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã đối tượng! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "PartnerCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }

                    if (string.IsNullOrEmpty(item.ContractCode) == true && lstaccountSystem1.AttachContract == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã hợp đồng! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "ContractCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.ContractCode) == true && lstaccountSystem1.AttachContract == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã hợp đồng! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "ContractCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.FProductWorkCode) == true && lstaccountSystem1.AttachProductCost == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã công trình sản phẩm! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "ContractCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.CaseCode) == true && lstaccountSystem1.AccSectionCode == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã khoản mục! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "ContractCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.WorkPlaceCode) == true && lstaccountSystem1.AccSectionCode == "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập mã phân xưởng! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "WorkPlaceCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                    if (string.IsNullOrEmpty(item.CreditAcc) == true && lstaccountSystem1.IsBalanceSheetAcc != "C")
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Chưa nhập tài khoản! ";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "CreditAcc";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }

                }

                if (lstaccountSystemHKD2 != null)
                {
                    if (lstaccountSystemHKD2.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            var partners = await _accPartnerService.GetQueryableAsync();
                            var lstPartner = partners.Where(p => p.Code == dto.PartnerCode0 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                            if (lstPartner.Count == 0)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                    //if (string.IsNullOrEmpty(item.DebitAcc2) == true && lstaccountSystemHKD2.IsBalanceSheetAcc != "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập tài khoản! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "DebitAcc";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (string.IsNullOrEmpty(item.CreditAcc2) == true && string.IsNullOrEmpty(lstaccountSystemHKD2.AccCode) == true)
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Mã tài khoản " + item.CreditAcc2 + " không tồn tại! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "CreditAcc";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}

                }
                if (lstaccountSystemHKD1 != null)
                {
                    if (lstaccountSystemHKD1.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            var partners = await _accPartnerService.GetQueryableAsync();
                            var lstPartner = partners.Where(p => p.Code == dto.PartnerCode0 && p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
                            if (lstPartner.Count == 0)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                    //if (string.IsNullOrEmpty(item.DebitAcc2) == true && string.IsNullOrEmpty(lstaccountSystemHKD1.AccCode) == true)
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Mã tài khoản " + item.DebitAcc2 + " không tồn tại! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "DebitAcc";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}

                    //if (string.IsNullOrEmpty(item.PartnerCode) == true && lstaccountSystemHKD1.AttachPartner == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã đối tượng! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "PartnerCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}

                    //if (string.IsNullOrEmpty(item.ContractCode) == true && lstaccountSystemHKD1.AttachContract == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã hợp đồng! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "ContractCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (string.IsNullOrEmpty(item.ContractCode) == true && lstaccountSystemHKD1.AttachContract == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã hợp đồng! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "ContractCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (string.IsNullOrEmpty(item.FProductWorkCode) == true && lstaccountSystemHKD1.AttachProductCost == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã công trình sản phẩm! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "ContractCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (string.IsNullOrEmpty(item.CaseCode) == true && lstaccountSystemHKD1.AccSectionCode == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã khoản mục! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "ContractCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (string.IsNullOrEmpty(item.WorkPlaceCode) == true && lstaccountSystemHKD1.AccSectionCode == "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập mã phân xưởng! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "WorkPlaceCode";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}
                    //if (string.IsNullOrEmpty(item.CreditAcc2) == true && lstaccountSystemHKD1.IsBalanceSheetAcc != "C")
                    //{
                    //    errorDto.Code = "101";
                    //    errorDto.Message = "Chưa nhập tài khoản! ";
                    //    InforDto inforDto = new InforDto();
                    //    inforDto.FieldName = "CreditAcc";
                    //    inforDto.TabTable = "ProductVoucherDetail";
                    //    inforDto.RowNumber = item.Ord0;
                    //    List<InforDto> inforDtos = new List<InforDto>();
                    //    inforDtos.Add(inforDto);
                    //    errorDto.inforDtos = inforDtos;
                    //    return errorDto;
                    //}

                }


                if (!string.IsNullOrEmpty(item.PartnerCode) == true && partner1.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã đối tượng " + item.ClearingPartnerCode + " không tồn tại !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ClearingPartnerCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ClearingPartnerCode) == true && partner2.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã đối tượng " + item.ClearingPartnerCode + " không tồn tại !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ClearingPartnerCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ContractCode) == true && partner1.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã hợp đồng " + item.ContractCode + " không tồn tại !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ContractCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ClearingContractCode) == true && partner2.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã hợp đồng " + item.ClearingContractCode + " không tồn tại !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ClearingContractCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.FProductWorkCode) == true && fpProduct1.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã chứng từ sản phẩm " + item.FProductWorkCode + " không tồn tại !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "FProductWorkCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.FProductWorkCode) == true && fpProduct1[0].FPWType == "C")
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã chứng từ sản phẩm " + item.FProductWorkCode + " là mã mẹ !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "FProductWorkCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ClearingFProductWorkCode) == true && fpProduct2.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã chứng từ sản phẩm " + item.ClearingFProductWorkCode + " không tồn tại !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ClearingFProductWorkCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ClearingFProductWorkCode) == true && fpProduct2[0].FPWType == "C")
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã chứng từ sản phẩm " + item.ClearingFProductWorkCode + " là mã mẹ !";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ClearingFProductWorkCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.SectionCode) == true && sessionCode1.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã chứng Km " + item.SectionCode + " ko tồn tại!";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "SectionCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ClearingSectionCode) == true && sessionCode2.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã chứng Km " + item.ClearingSectionCode + " ko tồn tại!";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "SectionCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.WorkPlaceCode) == true && workplace1.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã phân xưởng " + item.WorkPlaceCode + " ko tồn tại!";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "WorkPlaceCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.ClearingWorkPlaceCode) == true && workplace2.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã phân xưởng " + item.ClearingWorkPlaceCode + " ko tồn tại!";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "ClearingWorkPlaceCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }
                if (!string.IsNullOrEmpty(item.CaseCode) == true && acaseCode1.Count == 0)
                {
                    errorDto.Code = "101";
                    errorDto.Message = "Mã vụ việc " + item.CaseCode + " ko tồn tại!";
                    InforDto inforDto = new InforDto();
                    inforDto.FieldName = "CaseCode";
                    inforDto.TabTable = "ProductVoucherDetail";
                    inforDto.RowNumber = item.Ord0;
                    List<InforDto> inforDtos = new List<InforDto>();
                    inforDtos.Add(inforDto);
                    errorDto.inforDtos = inforDtos;
                    return errorDto;
                }

            }

            return errorDto;
        }

        public async Task UpdateProductVoucherAsync(string id, CrudProductVoucherDto dto)
        {
            dto.LastModifierName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            List<CrudProductVoucherDetailDto> productVoucherDetail = (List<CrudProductVoucherDetailDto>)dto.ProductVoucherDetails;
            List<CrudAccTaxDetailDto> AccTaxDetails = (List<CrudAccTaxDetailDto>)dto.AccTaxDetails;
            var check = await CheckProductVoucherAsync(dto);
            if (check.Message != null)
            {

                throw new Exception(check.Message);
            }
            if (productVoucherDetail.Count == 0)
            {

                throw new Exception("Chưa nhập chi tiết!");
            }
            for (int i = 0; i < productVoucherDetail.Count; i++)
            {
                var ids = this.GetNewObjectId();
                dto.ProductVoucherDetails[i].Id = ids;
                dto.ProductVoucherDetails[i].OrgCode = _webHelper.GetCurrentOrgUnit();
                dto.ProductVoucherDetails[i].Year = _webHelper.GetCurrentYear();
                dto.ProductVoucherDetails[i].Ord0 = productVoucherDetail[i].Ord0 = "A" + (i + 1).ToString().PadLeft(9, '0');
                List<CrudProductVoucherDetailReceiptDto> CrudProductVoucherDetailReceiptDto = (List<CrudProductVoucherDetailReceiptDto>)productVoucherDetail[0].ProductVoucherDetailReceipts;
                List<CrudProductVoucherDetailReceiptDto> CrudProductVoucherDetail = new List<CrudProductVoucherDetailReceiptDto>();
                CrudProductVoucherDetailReceiptDto = CrudProductVoucherDetail;
                CrudProductVoucherDetailReceiptDto crud = new CrudProductVoucherDetailReceiptDto();
                try
                {
                    crud.Id = this.GetNewObjectId();
                    crud.ProductVoucherDetailId = ids;
                    crud.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crud.Year = _webHelper.GetCurrentYear();
                    crud.TaxCategoryCode = productVoucherDetail[i].TaxCategoryCode;
                    if (AccTaxDetails != null)
                    {
                        if (AccTaxDetails.Count > 0)
                        {
                            crud.VatPercentage = AccTaxDetails[0].VatPercentage;
                        }

                    }
                    crud.VatAmountCur = productVoucherDetail[i].VatAmountCur;
                    crud.VatAmount = productVoucherDetail[i].VatAmount;
                    crud.DiscountPercentage = productVoucherDetail[i].DiscountPercentage;
                    crud.DiscountAmount = productVoucherDetail[i].DiscountAmount;
                    crud.DiscountAmountCur = productVoucherDetail[i].DiscountAmountCur;
                    crud.ImportTaxPercentage = productVoucherDetail[i].ImportTaxPercentage;
                    crud.ImportTaxAmountCur = productVoucherDetail[i].ImportTaxAmountCur;
                    crud.ImportTaxAmount = productVoucherDetail[i].ImportTaxAmount;
                    crud.ExciseTaxPercentage = productVoucherDetail[i].ExciseTaxPercentage;
                    crud.ExciseTaxAmountCur = productVoucherDetail[i].ExciseTaxAmountCur;
                    crud.ExciseTaxAmount = productVoucherDetail[i].ExciseTaxAmount;
                    crud.ExpenseAmountCur0 = productVoucherDetail[i].ExpenseAmountCur0;
                    crud.ExpenseAmount0 = productVoucherDetail[i].ExpenseAmount0;
                    crud.ExpenseAmountCur1 = productVoucherDetail[i].ExpenseAmountCur1;
                    crud.ExpenseAmount1 = productVoucherDetail[i].ExpenseAmount1;
                    crud.ExpenseAmountCur = productVoucherDetail[i].ExpenseAmountCur;
                    crud.ExpenseAmount = productVoucherDetail[i].ExpenseAmount;


                    //crud.Ex = productVoucherDetails[i].expenseImportTaxAmount;
                    CrudProductVoucherDetailReceiptDto.Add(crud);
                    productVoucherDetail[i].ProductVoucherDetailReceipts = new List<CrudProductVoucherDetailReceiptDto>();
                    productVoucherDetail[i].ProductVoucherDetailReceipts.Add(crud);

                }
                catch (Exception ex)
                {

                    throw;
                }
                List<CrudProductVoucherReceiptDto> productVoucherReceipts = (List<CrudProductVoucherReceiptDto>)dto.ProductVoucherReceipts;

                CrudProductVoucherReceiptDto crudProductVoucherReceipt = new CrudProductVoucherReceiptDto();
                crudProductVoucherReceipt.Id = this.GetNewObjectId();

                crudProductVoucherReceipt.Year = _webHelper.GetCurrentYear();
                crudProductVoucherReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
                crudProductVoucherReceipt.ProductVoucherId = id;
                crudProductVoucherReceipt.DiscountPercentage = dto.DiscountPercentage != null ? dto.DiscountPercentage : null;
                crudProductVoucherReceipt.DiscountCreditAcc = dto.DiscountCreditAcc != null ? dto.DiscountCreditAcc : null;
                crudProductVoucherReceipt.DiscountDebitAcc = dto.DiscountDebitAcc != null ? dto.DiscountDebitAcc : null;
                crudProductVoucherReceipt.DiscountCreditAcc0 = dto.DiscountCreditAcc0 != null ? dto.DiscountCreditAcc0 : null;
                crudProductVoucherReceipt.DiscountDebitAcc0 = dto.DiscountDebitAcc0 != null ? dto.DiscountDebitAcc0 : null;
                crudProductVoucherReceipt.DiscountAmount0 = dto.DiscountAmount0 != null ? dto.DiscountAmount0 : null;
                crudProductVoucherReceipt.DiscountAmountCur0 = dto.DiscountAmountCur0 != null ? dto.DiscountAmountCur0 : null;
                crudProductVoucherReceipt.DiscountDescription0 = dto.DiscountDescription0 != null ? dto.DiscountDescription0 : null;
                crudProductVoucherReceipt.ImportTaxPercentage = dto.ImportTaxPercentage != null ? dto.ImportTaxPercentage : null;
                crudProductVoucherReceipt.ImportCreditAcc = dto.ImportCreditAcc != null ? dto.ImportCreditAcc : null;
                crudProductVoucherReceipt.ImportDebitAcc = dto.ImportDebitAcc != null ? dto.ImportDebitAcc : null;
                crudProductVoucherReceipt.ImportDescription = dto.ImportDescription != null ? dto.ImportDescription : null;
                crudProductVoucherReceipt.ExciseTaxPercentage = dto.ExciseTaxPercentage != null ? dto.ExciseTaxPercentage : null;
                crudProductVoucherReceipt.ExciseTaxCreditAcc = dto.ExciseTaxCreditAcc != null ? dto.ExciseTaxCreditAcc : null;
                crudProductVoucherReceipt.ExciseTaxDebitAcc = dto.ExciseTaxDebitAcc != null ? dto.ExciseTaxDebitAcc : null;

                List<CrudProductVoucherReceiptDto> crus = new List<CrudProductVoucherReceiptDto>();
                crus.Add(crudProductVoucherReceipt);
                productVoucherReceipts = crus;
                if (productVoucherReceipts != null)
                {
                    dto.ProductVoucherReceipts = productVoucherReceipts;
                }
                List<CrudProductVoucherAssemblyDto> productVoucherAssembly = (List<CrudProductVoucherAssemblyDto>)dto.ProductVoucherAssemblies; ;
                if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
                {
                    CrudProductVoucherAssemblyDto crudProductVoucherAssemblyDto = new CrudProductVoucherAssemblyDto();
                    crudProductVoucherAssemblyDto.Id = this.GetNewObjectId();
                    crudProductVoucherAssemblyDto.Year = _webHelper.GetCurrentYear();
                    crudProductVoucherAssemblyDto.ProductVoucherId = id;
                    crudProductVoucherAssemblyDto.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crudProductVoucherAssemblyDto.AssemblyWarehouseCode = dto.AssemblyWarehouseCode;
                    crudProductVoucherAssemblyDto.AssemblyProductCode = dto.AssemblyProductCode;
                    crudProductVoucherAssemblyDto.AssemblyUnitCode = dto.AssemblyUnitCode;
                    crudProductVoucherAssemblyDto.Quantity = dto.Quantity;

                    List<CrudProductVoucherAssemblyDto> cru = new List<CrudProductVoucherAssemblyDto>();
                    cru.Add(crudProductVoucherAssemblyDto);
                    productVoucherAssembly = cru;

                }
                if (productVoucherAssembly != null)
                {
                    dto.ProductVoucherAssemblies = productVoucherAssembly;
                }
            }
            if (AccTaxDetails != null)
            {
                for (int i = 0; i < AccTaxDetails.Count; i++)
                {
                    dto.AccTaxDetails[i].Ord0 = AccTaxDetails[i].Ord0 = "Z" + (i + 1).ToString().PadLeft(9, '0');
                    dto.AccTaxDetails[i].Id = this.GetNewObjectId();
                    dto.AccTaxDetails[i].Year = _webHelper.GetCurrentYear();
                    dto.AccTaxDetails[i].OrgCode = _webHelper.GetCurrentOrgUnit();
                    dto.AccTaxDetails[i].VoucherCode = dto.VoucherCode;
                    dto.AccTaxDetails[i].ProductVoucherId = id;
                    dto.AccTaxDetails[i].VoucherDate = dto.VoucherDate;
                    dto.InvoiceNumber = AccTaxDetails[i].InvoiceNumber;

                }
            }
            List<CrudProductVoucherCostDto> ProductVoucherCost = (List<CrudProductVoucherCostDto>)dto.ProductVoucherCostDetails;

            if (ProductVoucherCost != null)
            {
                for (int i = 0; i < ProductVoucherCost.Count; i++)
                {
                    dto.ProductVoucherCostDetails[i].Id = this.GetNewObjectId();
                    dto.ProductVoucherCostDetails[i].ProductVoucherId = id;
                    dto.ProductVoucherCostDetails[i].Year = _webHelper.GetCurrentYear();
                    dto.ProductVoucherCostDetails[i].OrgCode = _webHelper.GetCurrentOrgUnit();
                    dto.ProductVoucherCostDetails[i].Ord0 = ProductVoucherCost[i].Ord0 = "X" + (i + 1).ToString().PadLeft(9, '0');


                }

            }
            var entity = await _productVoucher.GetAsync(id);
            await _productVoucher.CheckLockVoucher(entity);
            _objectMapper.Map(dto, entity);
            try
            {
                var productVoucherAssemblys = await _productVoucherAssembly.GetByProductIdAsync(id);
                var productVoucherDetails = await _productVoucherDetail.GetByProductIdAsync(id);
                var productVoucherReceipts = await _productVoucherReceipt.GetByProductIdAsync(id);
                var voucherPayment = await _voucherPaymentBookService.GetByVoucherPaymentBookAsync(id);
                var productVoucherVats = await _productVoucherVat.GetByProductIdAsync(id);
                var accTaxDetails = await _accTaxDetailService.GetByProductIdAsync(id);
                var ledgers = await _ledgerService.GetByProductIdAsync(id);
                var warehouseBookS = await _warehouseBookService.GetByProductIdAsync(id);
                var productVoucherCost = await _productVoucherCostService.GetByProductIdAsync(id);


                using var unitOfWork = _unitOfWorkManager.Begin();
                if (voucherPayment != null)
                {
                    await _voucherPaymentBookService.DeleteManyAsync(voucherPayment);
                }
                if (productVoucherAssemblys != null)
                {
                    await _productVoucherAssembly.DeleteManyAsync(productVoucherAssemblys);
                }
                if (productVoucherDetails != null)
                {
                    await _productVoucherDetail.DeleteManyAsync(productVoucherDetails);
                    for (int i = 0; i < productVoucherDetails.Count(); i++)
                    {
                        var productVoucherdetailReceipts = await _productVoucherDetailReceiptService.GetByProductIdAsync(productVoucherDetails[i].Id);
                        if (productVoucherdetailReceipts != null)
                        {
                            await _productVoucherDetailReceiptService.DeleteManyAsync(productVoucherdetailReceipts);
                        }
                    }

                }
                if (productVoucherReceipts != null)
                {
                    await _productVoucherReceipt.DeleteManyAsync(productVoucherReceipts);
                }
                if (productVoucherVats != null)
                {
                    await _productVoucherVat.DeleteManyAsync(productVoucherVats);
                }
                if (accTaxDetails != null)
                {
                    await _accTaxDetailService.DeleteManyAsync(accTaxDetails);
                }
                if (ledgers != null)
                {
                    await _ledgerService.DeleteManyAsync(ledgers);
                }
                if (warehouseBookS != null)
                {
                    await _warehouseBookService.DeleteManyAsync(warehouseBookS);
                }
                if (productVoucherCost != null)
                {
                    await _productVoucherCostService.DeleteManyAsync(productVoucherCost);
                }


                await _productVoucher.UpdateAsync(entity, true);

                try
                {


                    var voucherCategory = await _voucherCategoryService.CheckIsSavingLedgerAsync(entity.VoucherCode, entity.OrgCode);


                    if (voucherCategory.IsSavingLedger.ToString() != "K")
                    {
                        List<CrudVoucherExciseTaxDto> VoucherExciseTax = await _ledgerService.VoucherExcitaxAsync(dto);
                        foreach (var item in VoucherExciseTax)
                        {
                            item.Id = this.GetNewObjectId();
                            var excixeTax = _objectMapper.Map<CrudVoucherExciseTaxDto, VoucherExciseTax>(item);
                            excixeTax = await _voucherExciseTaxService.CreateAsync(excixeTax, true);
                        }

                        List<CrudVoucherPaymentBookDto> crudVoucherPaymentBookDto = await _ledgerService.CreateVoucherPaymentAsync(dto);
                        foreach (var crudVoucherPaymentBook in crudVoucherPaymentBookDto)
                        {
                            crudVoucherPaymentBook.Id = this.GetNewObjectId();
                            crudVoucherPaymentBook.DocumentId = dto.Id;
                            var crudVoucherPayment = _objectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(crudVoucherPaymentBook);
                            crudVoucherPayment = await _voucherPaymentBookService.CreateAsync(crudVoucherPayment, true);
                        }

                        if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
                        {
                            List<CrudLedgerDto> ledgerss = await _ledgerService.CreateLedgerLrAsync(dto);
                            foreach (var ledger in ledgerss)
                            {
                                ledger.Id = this.GetNewObjectId();
                                var ledgerEntity = _objectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                                ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                            }
                        }
                        else
                        {
                            List<CrudLedgerDto> ledgersList = await _ledgerService.CreateLedgerAsync(dto);
                            foreach (var ledger in ledgersList)
                            {
                                ledger.Id = this.GetNewObjectId();
                                var ledgerEntity = _objectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                                ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity, true);
                            }
                        }


                    }

                    if (voucherCategory.IsSavingWarehouseBook.ToString() != "K")
                    {
                        if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
                        {
                            List<CrudWarehouseBookDto> WarehouseBook = await _warehouseBookService.CreatewarehouseBookLrAsync(dto);
                            foreach (var WarehouseBooks in WarehouseBook)
                            {
                                WarehouseBooks.Id = this.GetNewObjectId();
                                var wareBookEntity = _objectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(WarehouseBooks);
                                wareBookEntity = await _warehouseBookService.CreateAsync(wareBookEntity, true);
                            }
                        }
                        else
                        {
                            List<CrudWarehouseBookDto> WarehouseBook = await _warehouseBookService.CreatewarehouseBookAsync(dto);
                            foreach (var WarehouseBooks in WarehouseBook)
                            {
                                WarehouseBooks.Id = this.GetNewObjectId();
                                var wareBookEntity = _objectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(WarehouseBooks);
                                wareBookEntity = await _warehouseBookService.CreateAsync(wareBookEntity, true);
                            }
                        }

                    }



                }
                catch (Exception ex)
                {

                    await _unitOfWorkManager.Current.RollbackAsync();
                    throw;
                }

                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }

        public async Task DeleteProductVoucherAsync(string id)
        {
            var entity = await _productVoucher.GetAsync(id);
            await _productVoucher.CheckLockVoucher(entity);
            await _productVoucher.DeleteAsync(id);

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

            var crudVoucherPaymentBookDto = await _voucherPaymentBookService.GetByIdAsync(id);
            if (crudVoucherPaymentBookDto != null)
            {
                await _voucherPaymentBookService.DeleteManyAsync(crudVoucherPaymentBookDto);
            }
            var ledgers = await _ledgerService.GetByProductIdAsync(id);
            var warehouseBookS = await _warehouseBookService.GetByProductIdAsync(id);
            if (ledgers != null)
            {
                await _ledgerService.DeleteManyAsync(ledgers);
            }
            if (warehouseBookS != null)
            {
                await _warehouseBookService.DeleteManyAsync(warehouseBookS);
            }
        }

        public async Task RepostProductVoucherAsync(string id)
        {
            var entity = await _productVoucher.GetAsync(id);
            var dto = _objectMapper.Map<ProductVoucher, CrudProductVoucherDto>(entity);
            dto.ProductVoucherDetails = (await _productVoucherDetail.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<ProductVoucherDetail, CrudProductVoucherDetailDto>(p)).ToList();
            dto.AccTaxDetails = (await _accTaxDetailService.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<AccTaxDetail, CrudAccTaxDetailDto>(p)).ToList();
            dto.ProductVoucherAssemblies = (await _productVoucherAssembly.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<ProductVoucherAssembly, CrudProductVoucherAssemblyDto>(p)).ToList();
            dto.ProductVoucherReceipts = (await _productVoucherReceipt.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<ProductVoucherReceipt, CrudProductVoucherReceiptDto>(p)).ToList();
            dto.ProductVoucherVats = (await _productVoucherVat.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<ProductVoucherVat, CrudProductVoucherVatDto>(p)).ToList();
            dto.ProductVoucherCostDetails = (await _productVoucherCostService.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<ProductVoucherCost, CrudProductVoucherCostDto>(p)).ToList();
            dto.VoucherExciseTaxes = (await _voucherExciseTaxService.GetByProductIdAsync(id))
                                        .Select(p => _objectMapper.Map<VoucherExciseTax, CrudVoucherExciseTaxDto>(p)).ToList();
            await UpdateProductVoucherAsync(id, dto);
        }

        #endregion
        #region Privates
        #endregion
    }
}
