using Accounting.BaseDtos;
using Accounting.Categories.Accounts;
using Accounting.Categories.Others;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.Others.ParameterFillter;
using Accounting.Catgories.ProductVouchers;
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
using Accounting.JsonConverters;
using Accounting.Localization;
using Accounting.Permissions;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherExciseTaxs;
using Accounting.Vouchers.VoucherNumbers;
using Accounting.Vouchers.WarehouseBooks;
using Accounting.Windows;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.Formula.Functions;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Authorization;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Uow;
using static NPOI.HSSF.Util.HSSFColor;
using Accounting.Common.Extensions;
using System.Threading;
using Accounting.Migrations.TenantDbMigration;
using Accounting.Caching;
using System.Net.NetworkInformation;
using NPOI.Util;
using Org.BouncyCastle.Utilities;
using Accounting.Business;
using Accounting.Exceptions;
using Accounting.Catgories.VoucherCategories;
using Volo.Abp.Users;
using Volo.Abp;
using NPOI.HSSF.Record;
using static IdentityServer4.Models.IdentityResources;

namespace Accounting.Categories.ProductVouchers
{
    public class ProductVoucherAppService : AccountingAppService, IProductVoucherAppService
    {

        #region Field
        private readonly ProductVoucherService _productVoucher;
        private readonly ProductVoucherDetailService _productVoucherDetail;
        private readonly AccTaxDetailService _accTaxDetailService;
        private readonly ProductVoucherAssemblyService _productVoucherAssembly;
        private readonly ProductVoucherReceiptService _productVoucherReceipt;
        private readonly ProductVoucherVatService _productVoucherVat;
        private readonly UserService _userService;
        private readonly LedgerService _ledgerService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly WebHelper _webHelper;
        private readonly VoucherCategoryService _voucherCategoryService;
        private readonly WarehouseBookService _warehouseBookService;
        private readonly TenantSettingService _tenantSettingService;
        private readonly TaxCategoryService _taxCategoryService;
        private readonly ProductUnitService _productUnitService;
        private readonly ProductService _product;
        private readonly ProductOpeningBalanceService _productOpeningBalanceService;
        private readonly VoucherExciseTaxService _voucherExciseTaxService;
        private readonly PartnerGroupService _partnerGroupService;
        private readonly ProductGroupService _productGroupService;
        private readonly AccPartnerService _accPartnerService;
        private readonly WarehouseService _warehouseService;
        private readonly AccountSystemService _accountSystemService;
        private readonly ProductVoucherCostService _productVoucherCostService;
        private readonly VoucherNumberBusiness _voucherNumberBusiness;
        private readonly WindowService _windowService;
        private readonly ProductVoucherDetailReceiptService _productVoucherDetailReceiptService;
        private readonly ExcelService _excelService;
        private readonly VoucherPaymentBookService _voucherPaymentBookService;
        private readonly PaymentTermService _paymentTermService;
        private readonly PaymentTermDetailService _paymentTermDetailService;
        private readonly RefVoucherService _refVoucherService;
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
        private readonly MenuAccountingService _menuAccountingService;
        private readonly IStringLocalizer<AccountingResource> _localizer;
        private readonly AccPartnerAppService _accPartnerAppService;
        private readonly ProductAppService _productAppService;
        private readonly AccountingCacheManager _accountingCacheManager;
        private readonly ProductService _productService;
        private readonly WarehouseAppService _warehouseAppService;
        private readonly LicenseBusiness _licenseBusiness;
        private readonly ProductVoucherBusiness _productVoucherBusiness;
        private readonly AccOpeningBalanceService _accOpeningBalanceService;
        private readonly ICurrentUser _currentUser;
        private readonly SaleChannelService _saleChannelService;
        #endregion
        #region Ctor
        public ProductVoucherAppService(ProductVoucherService productVoucherService,
                                ProductVoucherDetailService productVoucherDetailService,
                                AccTaxDetailService accTaxDetailService,
                                ProductVoucherAssemblyService productVoucherAssemblyService,
                                ProductVoucherReceiptService productVoucherReceiptService,
                                ProductVoucherVatService productVoucherVatService,
                                UserService userService,
                                IUnitOfWorkManager unitOfWorkManager,
                                WebHelper webHelper,
                                LedgerService ledger,
                                VoucherCategoryService voucherCategoryService,
                                TenantSettingService tenantSettingService,
                                TaxCategoryService taxCategoryService,
                                ProductUnitService productUnitService,
                                ProductService product,
                                ProductOpeningBalanceService productOpeningBalanceService,
                                WarehouseBookService warehouseBookService,
                                VoucherExciseTaxService voucherExciseTaxService,
                                PartnerGroupService partnerGroupService,
                                ProductGroupService productGroupService,
                                AccPartnerService accPartnerService,
                                WarehouseService warehouseService,
                                AccountSystemService accountSystemService,
                                ProductVoucherCostService productVoucherCostService,
                                VoucherNumberBusiness voucherNumberBusiness,
                                WindowService windowService,
                                ProductVoucherDetailReceiptService productVoucherDetailReceiptService,
                                ExcelService excelService,
                                VoucherPaymentBookService voucherPaymentBookService,
                                PaymentTermService paymentTermService,
                                PaymentTermDetailService paymentTermDetailService,
                                RefVoucherService refVoucherService,
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
                                MenuAccountingService menuAccountingService,
                                IStringLocalizer<AccountingResource> localizer,
                                AccPartnerAppService accPartnerAppService,
                                ProductAppService productAppService,
                                AccountingCacheManager accountingCacheManager,
                                ProductService productService,
                                WarehouseAppService warehouseAppService,
                                LicenseBusiness licenseBusiness,
                                ProductVoucherBusiness productVoucherBusiness,
                                AccOpeningBalanceService accOpeningBalanceService,
                                SaleChannelService saleChannelService,
                                ICurrentUser currentUser
                                )
        {
            _productVoucher = productVoucherService;
            _productVoucherDetail = productVoucherDetailService;
            _accTaxDetailService = accTaxDetailService;
            _productVoucherAssembly = productVoucherAssemblyService;
            _productVoucherReceipt = productVoucherReceiptService;
            _productVoucherVat = productVoucherVatService;
            _userService = userService;
            _unitOfWorkManager = unitOfWorkManager;
            _webHelper = webHelper;
            _ledgerService = ledger;
            _voucherCategoryService = voucherCategoryService;
            _tenantSettingService = tenantSettingService;
            _taxCategoryService = taxCategoryService;
            _productUnitService = productUnitService;
            _product = product;
            _productOpeningBalanceService = productOpeningBalanceService;
            _warehouseBookService = warehouseBookService;
            _voucherExciseTaxService = voucherExciseTaxService;
            _partnerGroupService = partnerGroupService;
            _productGroupService = productGroupService;
            _accPartnerService = accPartnerService;
            _warehouseService = warehouseService;
            _accountSystemService = accountSystemService;
            _productVoucherCostService = productVoucherCostService;
            _voucherNumberBusiness = voucherNumberBusiness;
            _windowService = windowService;
            _productVoucherDetailReceiptService = productVoucherDetailReceiptService;
            _excelService = excelService;
            _voucherPaymentBookService = voucherPaymentBookService;
            _paymentTermService = paymentTermService;
            _paymentTermDetailService = paymentTermDetailService;
            _refVoucherService = refVoucherService;
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
            _menuAccountingService = menuAccountingService;
            _localizer = localizer;
            _accPartnerAppService = accPartnerAppService;
            _productAppService = productAppService;
            _accountingCacheManager = accountingCacheManager;
            _productService = productService;
            _warehouseAppService = warehouseAppService;
            _licenseBusiness = licenseBusiness;
            _productVoucherBusiness = productVoucherBusiness;
            _accOpeningBalanceService = accOpeningBalanceService;
            _currentUser = currentUser;
            _saleChannelService = saleChannelService;
        }
        #endregion 

        public async Task<CrudProductVoucherDto> CreateAsync(CrudProductVoucherDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            //bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionCreate);
            //if (!isGranted)
            //{
            //    throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            //}
            await _licenseBusiness.ValidLicAsync();
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            var idProductVoucher = this.GetNewObjectId();
            dto.Id = idProductVoucher;
            dto.Year = _webHelper.GetCurrentYear();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();
            dto.Status = await this.GetVoucherStatus();

            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(dto.VoucherCode,
                                                                dto.OrgCode);
            ErrorDto errorDto = new ErrorDto();

            if (dto.VoucherCode != "KKD")
            {
                var check = await CheckAsync(dto, voucherCategory);
                if (check.Message != null)
                {
                    throw new Exception(check.Message);
                }
            }
            var productVoucherDetails = dto.ProductVoucherDetails;
            if (productVoucherDetails.Count == 0)
            {
                throw new AccountingException(ErrorCode.Get(GroupErrorCodes.ProductVoucher, ErrorCode.Other),
                                _localizer["Err:EmptyDetail"]);
            }

            await this.IncrementVoucherNumber(dto);
            await this.InsertRefVoucher(dto);

            var accTaxDetails = dto.AccTaxDetails;
            for (int i = 0; i < productVoucherDetails.Count; i++)
            {
                string ord0 = "A" + (i + 1).ToString().PadLeft(9, '0');
                productVoucherDetails[i].Ord0 = ord0;
                var id = this.GetNewObjectId();
                dto.ProductVoucherDetails[i].Id = id;
                dto.ProductVoucherDetails[i].OrgCode = dto.OrgCode;
                dto.ProductVoucherDetails[i].Year = dto.Year;
                dto.ProductVoucherDetails[i].Ord0 = ord0;
                var crudProductVoucherDetailReceiptDto = productVoucherDetails[0].ProductVoucherDetailReceipts;
                var crudProductVoucherDetail = new List<CrudProductVoucherDetailReceiptDto>();
                crudProductVoucherDetailReceiptDto = crudProductVoucherDetail;
                var crud = new CrudProductVoucherDetailReceiptDto();
                try
                {
                    crud.Id = this.GetNewObjectId();
                    crud.ProductVoucherDetailId = id;
                    crud.OrgCode = _webHelper.GetCurrentOrgUnit();
                    crud.Year = _webHelper.GetCurrentYear();
                    crud.TaxCategoryCode = productVoucherDetails[i].TaxCategoryCode;
                    if (accTaxDetails != null)
                    {
                        if (accTaxDetails.Count > 0)
                        {
                            crud.VatPercentage = accTaxDetails[0].VatPercentage;
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
                    crudProductVoucherDetailReceiptDto.Add(crud);
                    productVoucherDetails[i].ProductVoucherDetailReceipts = new List<CrudProductVoucherDetailReceiptDto>();
                    productVoucherDetails[i].ProductVoucherDetailReceipts.Add(crud);
                }
                catch (Exception)
                {
                    throw;
                }
            }


            if (accTaxDetails != null)
            {
                for (int i = 0; i < accTaxDetails.Count; i++)
                {
                    var item = accTaxDetails[i];
                    item.Ord0 = "Z" + (i + 1).ToString().PadLeft(9, '0');
                    item.Id = this.GetNewObjectId();
                    item.Year = dto.Year;
                    item.OrgCode = dto.OrgCode;
                    item.VoucherCode = dto.VoucherCode;
                    item.ProductVoucherId = dto.Id;
                    item.VoucherDate = dto.VoucherDate;

                    dto.InvoiceNumber = item.InvoiceNumber;
                }
            }

            if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
            {
                var crudAssemblyDto = new CrudProductVoucherAssemblyDto();
                crudAssemblyDto.Id = this.GetNewObjectId();
                crudAssemblyDto.Year = dto.Year;
                crudAssemblyDto.ProductVoucherId = dto.Id;
                crudAssemblyDto.OrgCode = dto.OrgCode;
                crudAssemblyDto.AssemblyWarehouseCode = dto.AssemblyWarehouseCode;
                crudAssemblyDto.AssemblyProductCode = dto.AssemblyProductCode;
                crudAssemblyDto.AssemblyUnitCode = dto.AssemblyUnitCode;
                crudAssemblyDto.Quantity = dto.Quantity;

                var lstAssemblies = new List<CrudProductVoucherAssemblyDto>();
                lstAssemblies.Add(crudAssemblyDto);
                dto.ProductVoucherAssemblies = lstAssemblies;
            }


            var crudReceipt = new CrudProductVoucherReceiptDto();
            crudReceipt.Id = this.GetNewObjectId();
            crudReceipt.Year = _webHelper.GetCurrentYear();
            crudReceipt.OrgCode = _webHelper.GetCurrentOrgUnit();
            crudReceipt.ProductVoucherId = idProductVoucher;
            crudReceipt.DiscountPercentage = dto.DiscountPercentage != null ? dto.DiscountPercentage : null;
            crudReceipt.DiscountCreditAcc = dto.DiscountCreditAcc != null ? dto.DiscountCreditAcc : null;
            crudReceipt.DiscountDebitAcc = dto.DiscountDebitAcc != null ? dto.DiscountDebitAcc : null;
            crudReceipt.DiscountCreditAcc0 = dto.DiscountCreditAcc0 != null ? dto.DiscountCreditAcc0 : null;
            crudReceipt.DiscountDebitAcc0 = dto.DiscountDebitAcc0 != null ? dto.DiscountDebitAcc0 : null;
            crudReceipt.DiscountAmount0 = dto.DiscountAmount0 != null ? dto.DiscountAmount0 : null;
            crudReceipt.DiscountAmountCur0 = dto.DiscountAmountCur0 != null ? dto.DiscountAmountCur0 : null;
            crudReceipt.DiscountDescription0 = dto.DiscountDescription0 != null ? dto.DiscountDescription0 : null;
            crudReceipt.ImportTaxPercentage = dto.ImportTaxPercentage != null ? dto.ImportTaxPercentage : null;
            crudReceipt.ImportCreditAcc = dto.ImportCreditAcc != null ? dto.ImportCreditAcc : null;
            crudReceipt.ImportDebitAcc = dto.ImportDebitAcc != null ? dto.ImportDebitAcc : null;
            crudReceipt.ImportDescription = dto.ImportDescription != null ? dto.ImportDescription : null;
            crudReceipt.ExciseTaxPercentage = dto.ExciseTaxPercentage != null ? dto.ExciseTaxPercentage : null;
            crudReceipt.ExciseTaxCreditAcc = dto.ExciseTaxCreditAcc != null ? dto.ExciseTaxCreditAcc : null;
            crudReceipt.ExciseTaxDebitAcc = dto.ExciseTaxDebitAcc != null ? dto.ExciseTaxDebitAcc : null;

            var lstReceipt = new List<CrudProductVoucherReceiptDto>();
            lstReceipt.Add(crudReceipt);
            dto.ProductVoucherReceipts = lstReceipt;

            var productVoucherCost = dto.ProductVoucherCostDetails;
            if (productVoucherCost != null)
            {
                for (int i = 0; i < productVoucherCost.Count; i++)
                {
                    var item = productVoucherCost[i];
                    item.Id = this.GetNewObjectId();
                    item.ProductVoucherId = dto.Id;
                    item.Year = dto.Year;
                    item.OrgCode = dto.OrgCode;
                    item.Ord0 = "X" + (i + 1).ToString().PadLeft(9, '0');
                }
            }

            var entity = ObjectMapper.Map<CrudProductVoucherDto, ProductVoucher>(dto);
            await _productVoucherBusiness.CheckLockVoucher(entity);
            try
            {
                using var unitOfWork = _unitOfWorkManager.Begin();
                var result = await _productVoucher.CreateAsync(entity);

                await this.SavingLedger(voucherCategory, dto);
                await this.SavingWarehouseBook(voucherCategory, dto);
                await unitOfWork.CompleteAsync();
                return dto;
            }
            catch (Exception)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }
        }

        public async Task<ErrorDto> CheckAsync(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            ErrorDto errorDto = new ErrorDto();
            await CheckDateProductVoucher(dto, voucherCategory);
            await CheckDepartment(dto, voucherCategory);
            await CheckSalesChannelCode(dto, voucherCategory);
            await CheckCodeProductVoucher(dto, voucherCategory);
            await CheckAccCodeProductVoucher(dto, voucherCategory);
            errorDto = await CheckIsAssembly(dto, voucherCategory);
            if (errorDto.Message != null)
            {
                return errorDto;
            }
            //check Detail
            errorDto = await CheckDetail(dto, voucherCategory);
            if (errorDto.Message != null)
            {
                return errorDto;
            }
            //check ht
            errorDto = await CheckAccounting(dto, voucherCategory);
            if (errorDto.Message != null)
            {
                return errorDto;
            }
            return errorDto;
        }

        public async Task<CrudProductVoucherDto> GetByIdAsync(string productVoucherId)
        {
            var queryable = await _productVoucher.GetQueryableAsync();
            var productAproductVoucherReceipt = await _productVoucherReceipt.GetQueryableAsync();
            var productVoucherAssembly = await _productVoucherAssembly.GetQueryableAsync();
            var lstproductVoucherAssembly = productVoucherAssembly.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstqueryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstproductAproductVoucherReceipt = productAproductVoucherReceipt.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var reusl = (from a in lstqueryable
                         join b in lstproductAproductVoucherReceipt on new { a.OrgCode, ProductVoucherId = a.Id } equals new { b.OrgCode, b.ProductVoucherId } into c
                         from pr in c.DefaultIfEmpty()
                         join d in lstproductVoucherAssembly on a.Id equals d.ProductVoucherId into e
                         from lst in e.DefaultIfEmpty()
                         where a.Id == productVoucherId
                         select new CrudProductVoucherDto
                         {
                             Id = a.Id,
                             Year = a.Year,
                             DepartmentCode = a.DepartmentCode,
                             VoucherCode = a.VoucherCode,
                             VoucherGroup = a.VoucherGroup,
                             BusinessCode = a.BusinessCode,
                             BusinessAcc = a.BusinessAcc,
                             VoucherNumber = a.VoucherNumber,
                             InvoiceNumber = a.InvoiceNumber,
                             VoucherDate = a.VoucherDate,
                             PaymentTermsCode = a.PaymentTermsCode,
                             PartnerCode0 = a.PartnerCode0,
                             PartnerName0 = a.PartnerName0,
                             Representative = a.Representative,
                             Address = a.Address,
                             Tel = a.Tel,
                             Description = a.Description,
                             DescriptionE = a.DescriptionE,
                             Place = a.Place,
                             OriginVoucher = a.OriginVoucher,
                             CurrencyCode = a.CurrencyCode,
                             ExchangeRate = (decimal)a.ExchangeRate,
                             TotalAmountWithoutVatCur = a.TotalAmountWithoutVatCur,
                             TotalAmountWithoutVat = a.TotalAmountWithoutVat,
                             TotalDiscountAmountCur = a.TotalDiscountAmountCur,
                             TotalDiscountAmount = a.TotalDiscountAmount,
                             TotalVatAmountCur = a.TotalVatAmountCur,
                             TotalVatAmount = a.TotalVatAmount,
                             DebitOrCredit = a.DebitOrCredit,
                             TotalAmountCur = a.TotalAmountCur,
                             TotalAmount = a.TotalAmount,
                             TotalProductAmountCur = a.TotalProductAmountCur,
                             TotalProductAmount = a.TotalProductAmount,
                             TotalExciseTaxAmountCur = a.TotalExciseTaxAmountCur,
                             TotalExciseTaxAmount = a.TotalExciseTaxAmount,
                             TotalQuantity = a.TotalQuantity,
                             ExportNumber = a.ExportNumber,
                             TotalExpenseAmountCur0 = a.TotalExpenseAmountCur0,
                             TotalExpenseAmount0 = a.TotalExpenseAmount0,
                             TotalImportTaxAmountCur = a.TotalImportTaxAmountCur,
                             TotalImportTaxAmount = a.TotalImportTaxAmount,
                             TotalExpenseAmountCur = a.TotalExpenseAmountCur,
                             TotalExpenseAmount = a.TotalExpenseAmount,
                             EmployeeCode = a.EmployeeCode,
                             Status = a.Status,
                             PaymentTermsId = a.PaymentTermsId,
                             SalesChannelCode = a.SalesChannelCode,
                             BillNumber = a.BillNumber,
                             DevaluationPercentage = a.DevaluationPercentage,
                             TotalDevaluationAmountCur = a.TotalDevaluationAmountCur,
                             TotalDevaluationAmount = a.TotalDevaluationAmount,
                             PriceDebitAcc = a.PriceDebitAcc,
                             PriceCreditAcc = a.PriceCreditAcc,
                             PriceDecreasingDescription = a.PriceDecreasingDescription,
                             IsCreatedEInvoice = a.IsCreatedEInvoice,
                             InfoFilter = a.InfoFilter,
                             RefType = a.RefType,
                             Vehicle = a.Vehicle,
                             OtherDepartment = a.OtherDepartment,
                             CommandNumber = a.CommandNumber,
                             TotalExpenseAmountCur1 = a.TotalExpenseAmountCur1,
                             TotalExpenseAmount1 = a.TotalExpenseAmount1,
                             PaymentDebitAcc = a.PaymentDebitAcc,
                             PaymentCreditAcc = a.PaymentCreditAcc,
                             ExcutionStatus = a.ExcutionStatus,
                             DiscountPercentage = pr != null ? pr.DiscountPercentage : null,
                             DiscountCreditAcc = pr != null ? pr.DiscountCreditAcc : "",
                             DiscountDebitAcc = pr != null ? pr.DiscountDebitAcc : "",
                             DiscountAmountCur = pr != null ? pr.DiscountAmountCur0 : null,
                             DiscountAmount = pr != null ? pr.DiscountAmount0 : null,
                             ImportTaxPercentage = pr != null ? pr.ImportTaxPercentage : null,
                             ImportCreditAcc = pr != null ? pr.ImportCreditAcc : null,
                             ImportDebitAcc = pr != null ? pr.ImportDebitAcc : null,
                             ExciseTaxPercentage = pr != null ? pr.ExciseTaxPercentage : null,
                             ExciseTaxCreditAcc = pr != null ? pr.ExciseTaxCreditAcc : null,
                             ExciseTaxDebitAcc = pr != null ? pr.ExciseTaxDebitAcc : null,
                             DiscountAmount0 = pr != null ? pr.DiscountAmount0 : null,
                             DiscountAmountCur0 = pr != null ? pr.DiscountAmountCur0 : null,
                             DiscountCreditAcc0 = pr != null ? pr.DiscountCreditAcc0 : null,
                             DiscountDebitAcc0 = pr != null ? pr.DiscountDebitAcc0 : null,
                             DiscountDescription0 = pr != null ? pr.DiscountDescription0 : null,
                             AssemblyProductCode = lst != null ? lst.AssemblyProductCode : null,
                             AssemblyUnitCode = lst != null ? lst.AssemblyUnitCode : null,
                             AssemblyWarehouseCode = lst != null ? lst.AssemblyWarehouseCode : null,
                             Quantity = lst != null ? (decimal)lst.Quantity : 0

                         }).ToList();
            CrudProductVoucherDto crudProductVoucherDto = new CrudProductVoucherDto();
            foreach (var item in reusl)
            {
                crudProductVoucherDto = item;

            }


            return crudProductVoucherDto;
        }
        public async Task DeleteAsync(string id)
        {
            await _licenseBusiness.CheckExpired();
            var entity = await _productVoucher.GetAsync(id);
            await _productVoucherBusiness.CheckLockVoucher(entity);
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionDelete);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            await _productVoucher.DeleteAsync(id);
            await _refVoucherService.DeleteRefVoucher(id);
            await _voucherPaymentBookService.DeleteByVoucherId(id);
            await _ledgerService.DeleteByVoucherId(id);
            await _warehouseBookService.DeleteByVoucherId(id);
        }

        public async Task<List<CheckDebtCeilingDto>> GetCheckDebtCeilingAsync(string productVoucherId)
        {
            var productVoucher = await _productVoucher.GetQueryableAsync();
            productVoucher = productVoucher.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Id == productVoucherId);
            var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
            productVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var accountSystem = await _accountSystemService.GetQueryableAsync();
            accountSystem = accountSystem.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit() && p.Year == _webHelper.GetCurrentYear());
            var accPartner = await _accPartnerService.GetQueryableAsync();
            accPartner = accPartner.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var data = (from a in productVoucher
                        join b in productVoucherDetail on a.Id equals b.ProductVoucherId
                        join c in accountSystem on (a.VoucherGroup == 2 ? b.DebitAcc2 : b.DebitAcc) equals c.AccCode
                        join d in accPartner on a.PartnerCode0 equals d.Code
                        where d.DebtCeiling != 0 && c.AttachPartner == "C"
                        group new { a, b, c, d } by new
                        {
                            OrgCode = a.OrgCode,
                            AccCode = (a.VoucherGroup == 2 ? b.DebitAcc2 : b.DebitAcc),
                            PartnerCode = ((b.PartnerCode ?? "") == "" ? a.PartnerCode0 : b.PartnerCode),
                            PartnerName = d.Name,
                            DebtCeiling = d.DebtCeiling,
                        } into gr
                        select new CheckDebtCeilingDto
                        {
                            OrgCode = gr.Key.OrgCode,
                            AccCode = gr.Key.AccCode,
                            PartnerCode = gr.Key.PartnerCode,
                            PartnerName = gr.Key.PartnerName,
                            DebtCeiling = gr.Key.DebtCeiling,
                            Balance = 0
                        }).ToList();
            foreach (var item in data)
            {
                var accOpeningBalance = await _accOpeningBalanceService.GetQueryableAsync();
                accOpeningBalance = accOpeningBalance.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                                              && p.Year == _webHelper.GetCurrentYear()
                                                              && p.AccCode == item.AccCode
                                                              && p.PartnerCode == item.PartnerCode);
                var ledger = await _ledgerService.GetQueryableAsync();
                ledger = ledger.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                        && p.Year == _webHelper.GetCurrentYear()
                                        && (p.DebitAcc == item.AccCode || p.CreditAcc == item.AccCode)
                                        && (p.DebitPartnerCode == item.PartnerCode || p.CreditPartnerCode == item.PartnerCode)
                                        && String.Compare(p.Status, "2") < 0);
                item.Balance = accOpeningBalance.Sum(p => p.Debit - p.Credit) + (ledger.Sum(p => (p.DebitAcc == item.AccCode ? p.Amount : -1 * p.Amount)) ?? 0);
            }
            return data;
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

        public async Task<PageResultDto<ProductVoucherDto>> GetListAsync(PageRequestDto dto)
        {
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionView);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            var result = new PageResultDto<ProductVoucherDto>();
            var query = await FilterProductVoucher(dto);

            var querysort = query.OrderBy(p => p.VoucherDate).OrderByDescending(p => p.VoucherDate).ThenByDescending(p => p.VoucherNumber).Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductVoucherDto, ProductVoucherDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }
        public async Task<PageResultDto<ProductVoucherDto>> PostProductListAsync(PageRequestDto dto)
        {
            await _licenseBusiness.CheckExpired();
            var result = new PageResultDto<ProductVoucherDto>();
            var query = await FilterProductVoucher(dto);
            var querysort = query.Skip(dto.Start).Take(dto.Count);
            var sections = await AsyncExecuter.ToListAsync(querysort);
            result.Data = sections.Select(p => ObjectMapper.Map<ProductVoucherDto, ProductVoucherDto>(p)).ToList();
            result.Pos = dto.Start;
            if (dto.Start == 0)
            {
                result.TotalCount = await AsyncExecuter.CountAsync(query);
            }
            return result;
        }


        public async Task UpdateAsync(string id, CrudProductVoucherDto dto)
        {
            await _licenseBusiness.CheckExpired();
            string menuId = _webHelper.GetMenuId();
            bool isGranted = await IsGrantPermission(menuId, AccountingPermissions.ActionUpdate);
            if (!isGranted)
            {
                throw new AbpAuthorizationException(_localizer["notAuthorize"]);
            }

            dto.SalesChannelCode = dto.SalesChannelCode;
            dto.CreatorName = await _userService.GetCurrentUserNameAsync();
            dto.OrgCode = _webHelper.GetCurrentOrgUnit();

            var voucherCategory = await _accountingCacheManager.GetVoucherCategoryAsync(dto.VoucherCode,
                                                                dto.OrgCode);

            List<CrudProductVoucherDetailDto> productVoucherDetail = (List<CrudProductVoucherDetailDto>)dto.ProductVoucherDetails;
            List<CrudAccTaxDetailDto> AccTaxDetails = (List<CrudAccTaxDetailDto>)dto.AccTaxDetails;
            if (dto.VoucherCode != "KKD")
            {
                var check = await CheckAsync(dto, voucherCategory);
                if (check.Message != null)
                {

                    throw new Exception(check.Message);
                }

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
                    crud.VatPercentage = productVoucherDetail[i].VatPercentage;
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
            await _productVoucherBusiness.CheckLockVoucher(entity);
            ObjectMapper.Map(dto, entity);
            try
            {
                var productVoucherAssemblys = await _productVoucherAssembly.GetByProductIdAsync(id);
                var productVoucherDetails = await _productVoucherDetail.GetByProductIdAsync(id);
                var productVoucherReceipts = await _productVoucherReceipt.GetByProductIdAsync(id);
                var voucherPayment = await _voucherPaymentBookService.GetByIdAsync(id);
                var productVoucherVats = await _productVoucherVat.GetByProductIdAsync(id);
                var accTaxDetails = await _accTaxDetailService.GetByProductIdAsync(id);
                var ledgers = await _ledgerService.GetByProductIdAsync(id);
                var warehouseBookS = await _warehouseBookService.GetByProductIdAsync(id);
                var productVoucherCost = await _productVoucherCostService.GetByProductIdAsync(id);
                var voucherExciseTax = await _voucherExciseTaxService.GetByProductIdAsync(id);

                if (warehouseBookS.Count > 0)
                {
                    DateTime? creationTime = warehouseBookS.FirstOrDefault().CreationTime;
                    dto.CreationTime = creationTime;
                }
                using var unitOfWork = _unitOfWorkManager.Begin();
                if (voucherExciseTax != null)
                {
                    await _voucherExciseTaxService.DeleteManyAsync(voucherExciseTax);
                }
                var voucherPaymentNull = voucherPayment.Where(p => p.AccVoucherId == null).ToList();
                if (voucherPaymentNull != null)
                {
                    await _voucherPaymentBookService.DeleteManyAsync(voucherPaymentNull);
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


                await _productVoucher.UpdateAsync(entity);
                var error = await _warehouseBookService.CheckSoundOutput(dto);

                if (error.Message != null)
                {
                    throw new Exception(error.Message);
                }


                if (voucherCategory.IsSavingLedger.ToString() != "K")
                {
                    List<CrudVoucherExciseTaxDto> VoucherExciseTax = await _ledgerService.VoucherExcitaxAsync(dto);
                    foreach (var item in VoucherExciseTax)
                    {
                        item.Id = this.GetNewObjectId();
                        var excixeTax = ObjectMapper.Map<CrudVoucherExciseTaxDto, VoucherExciseTax>(item);
                        excixeTax = await _voucherExciseTaxService.CreateAsync(excixeTax);
                    }

                    List<CrudVoucherPaymentBookDto> crudVoucherPaymentBookDto = await _ledgerService.CreateVoucherPaymentAsync(dto);
                    foreach (var crudVoucherPaymentBook in crudVoucherPaymentBookDto)
                    {
                        crudVoucherPaymentBook.Id = this.GetNewObjectId();
                        crudVoucherPaymentBook.DocumentId = dto.Id;
                        crudVoucherPaymentBook.OrgCode = _webHelper.GetCurrentOrgUnit();
                        var crudVoucherPayment = ObjectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(crudVoucherPaymentBook);
                        crudVoucherPayment = await _voucherPaymentBookService.CreateAsync(crudVoucherPayment);
                    }
                    var voucherPaymentUpdate = voucherPayment.Where(p => p.AccVoucherId != null).ToList();
                    foreach (var item in voucherPaymentUpdate)
                    {
                        foreach (var crudVoucherPaymentBook in crudVoucherPaymentBookDto)
                        {
                            if (item.Times == crudVoucherPaymentBook.Times)
                            {
                                item.AmountReceivable = crudVoucherPaymentBook.AmountReceivable;
                                item.Amount = (decimal)crudVoucherPaymentBook.Amount;
                                item.TotalAmount = (decimal)crudVoucherPaymentBook.TotalAmount;
                            }
                        }
                    }

                    if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
                    {
                        List<CrudLedgerDto> ledgerss = await _ledgerService.CreateLedgerLrAsync(dto);
                        foreach (var ledger in ledgerss)
                        {
                            ledger.Id = this.GetNewObjectId();
                            ledger.CreationTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                            ledger.CreatorName = await _userService.GetCurrentUserNameAsync();
                            var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                            ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity);
                        }
                    }
                    else
                    {
                        List<CrudLedgerDto> ledgersList = await _ledgerService.CreateLedgerAsync(dto);
                        foreach (var ledger in ledgersList)
                        {
                            ledger.Id = this.GetNewObjectId();
                            ledger.CreationTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));
                            ledger.CreatorName = await _userService.GetCurrentUserNameAsync();
                            var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                            ledgerEntity = await _ledgerService.CreateAsync(ledgerEntity);
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
                            var wareBookEntity = ObjectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(WarehouseBooks);
                            wareBookEntity = await _warehouseBookService.CreateAsync(wareBookEntity);
                        }
                    }
                    else
                    {

                        List<CrudWarehouseBookDto> WarehouseBook = await _warehouseBookService.CreatewarehouseBookAsync(dto);
                        foreach (var WarehouseBooks in WarehouseBook)
                        {
                            WarehouseBooks.Id = this.GetNewObjectId();

                            var wareBookEntity = ObjectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(WarehouseBooks);
                            wareBookEntity = await _warehouseBookService.CreateAsync(wareBookEntity);
                        }
                    }

                }

                await unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWorkManager.Current.RollbackAsync();
                throw;
            }

        }
        #region Private
        private async Task CheckDateProductVoucher(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            ErrorDto errorDto = new ErrorDto();
            DateTime date = DateTime.Parse(dto.VoucherDate.ToString());
            int year = date.Year;
            if (year != _webHelper.GetCurrentYear())
            {
                throw new Exception("Ngày chứng từ không nằm trong năm làm việc!");
            }

            if (string.IsNullOrEmpty(voucherCategory.BookClosingDate.ToString())
                                    && dto.VoucherDate <= voucherCategory.BookClosingDate)
            {
                throw new Exception("Chứng từ trước ngày khóa sổ " + dto.VoucherDate
                                        + "không thể thêm, sửa, xóa!");
            }
            if (
                (!string.IsNullOrEmpty(voucherCategory.BusinessBeginningDate.ToString())
                 && dto.VoucherDate < voucherCategory.BusinessBeginningDate)
                 ||
                (!string.IsNullOrEmpty(voucherCategory.BusinessEndingDate.ToString())
                  && dto.VoucherDate > voucherCategory.BusinessEndingDate)
               )
            {
                throw new Exception("Ngày chứng từ không được nằm ngoài " + voucherCategory.BusinessBeginningDate.ToString()
                                    + " - " + voucherCategory.BusinessEndingDate.ToString());
            }
        }

        private async Task CheckDepartment(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            if (!string.IsNullOrEmpty(dto.DepartmentCode))
            {
                var department = await _accountingCacheManager.GetDepartmentByCodeAsync(dto.DepartmentCode, dto.OrgCode);
                if (department == null)
                {
                    throw new Exception("Mã bộ phận " + dto.DepartmentCode + " không tồn tại!");
                }

                bool isParentGroup = await _departmentService.IsParentGroup(department.Id);
                if (isParentGroup)
                {
                    throw new Exception("Mã bộ phận  là bộ phận mẹ " + dto.DepartmentCode);
                }
            }
        }

        private async Task CheckSalesChannelCode(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            if (!string.IsNullOrEmpty(dto.SalesChannelCode))
            {
                var dataSale = await _saleChannelService.Check(_webHelper.GetCurrentOrgUnit(), dto.SalesChannelCode);
                if (dataSale == false)
                {
                    throw new Exception("Kênh bán hàng " + dto.SalesChannelCode + "Không tồn tại");
                }
            }
        }

        private async Task CheckCodeProductVoucher(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            if (!string.IsNullOrEmpty(dto.PaymentTermsCode))
            {
                var isDataPaymentTerms = await _paymentTermService.Check(dto.PaymentTermsCode, _webHelper.GetCurrentOrgUnit());
                if (isDataPaymentTerms == false)
                {
                    throw new Exception("Mã điều kiện thanh toán " + dto.PaymentTermsCode + "Không tồn tại");
                }
            }

            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                var isDataBusiness = await _businessCategoryService.GetBusinessByCodeAsync(dto.BusinessCode, _webHelper.GetCurrentOrgUnit());
                if (isDataBusiness == null)
                {
                    throw new Exception("Mã hạch toán " + dto.BusinessCode + "Không tồn tại");
                }
            }

            var curency = await _accountingCacheManager.GetCurrenciesAsync(dto.CurrencyCode, dto.OrgCode);
            if (curency == null)
            {
                throw new Exception("Mã ngoại tệ " + dto.CurrencyCode + " không tồn tại!");
            }

            if (!string.IsNullOrEmpty(dto.BusinessCode))
            {
                var businessCategory = await _accountingCacheManager.GetBusinessCategoryByCodeAsync(dto.BusinessCode, dto.OrgCode);
                if (businessCategory == null)
                {
                    throw new Exception("Hạch toán " + dto.BusinessCode + " không tồn tại!");
                }

                if (businessCategory.VoucherCode != dto.VoucherCode)
                {
                    throw new Exception("Mã hạch toán không khớp với mã chứng từ!");
                }
            }
        }

        private async Task CheckAccCodeProductVoucher(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            var lstAccCode = await _accountingCacheManager.GetAccountSystemsAsync(dto.Year, dto.OrgCode);
            if (string.IsNullOrEmpty(dto.DevaluationDebitAcc) == false)
            {
                var acc = lstAccCode.Where(p => p.AccCode == dto.DevaluationDebitAcc).ToList();
                if (acc.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DevaluationDebitAcc + " không tồn tại!");
                }
                var accPartner = lstAccCode.Where(p => p.AccCode == dto.DevaluationDebitAcc && p.AccType != "C").ToList();
                if (accPartner.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DevaluationDebitAcc + " là tài khoản mẹ!");
                }
                var accDevaCredit = lstAccCode.Where(p => p.AccCode == dto.DevaluationCreditAcc).ToList();
                if (accDevaCredit.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DevaluationCreditAcc + " không tồn tại!");
                }
                var accPartnerCredit = lstAccCode.Where(p => p.AccCode == dto.DevaluationCreditAcc && p.AccType != "C").ToList();
                if (accPartnerCredit.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DevaluationCreditAcc + " là tài khoản mẹ!");
                }


                var accDisountDe = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc).ToList();
                if (accDisountDe.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountDebitAcc + " không tồn tại!");
                }
                var accDiscountDedit = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc && p.AccType != "C").ToList();
                if (accDiscountDedit.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountDebitAcc + " là tài khoản mẹ!");
                }

                var discountCreditAcc = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc).ToList();
                if (discountCreditAcc.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountCreditAcc + " không tồn tại!");
                }
                var discountCreditAccPart = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc && p.AccType != "C").ToList();
                if (discountCreditAccPart.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountCreditAcc + " là tài khoản mẹ!");
                }



                var accDisountDe0 = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc0).ToList();
                if (accDisountDe0.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountDebitAcc0 + " không tồn tại!");
                }
                var accDiscountDedit0 = lstAccCode.Where(p => p.AccCode == dto.DiscountDebitAcc0 && p.AccType != "C").ToList();
                if (accDiscountDedit.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountDebitAcc0 + " là tài khoản mẹ!");
                }

                var discountCreditAcc0 = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc0).ToList();
                if (discountCreditAcc.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountCreditAcc0 + " không tồn tại!");
                }
                var discountCreditAccPart0 = lstAccCode.Where(p => p.AccCode == dto.DiscountCreditAcc0 && p.AccType != "C").ToList();
                if (discountCreditAccPart.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.DiscountCreditAcc0 + " là tài khoản mẹ!");
                }
            }
            if (string.IsNullOrEmpty(dto.ImportCreditAcc) == false)
            {
                var importCreditAcc = lstAccCode.Where(p => p.AccCode == dto.ImportCreditAcc).ToList();
                if (importCreditAcc.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.ImportCreditAcc + " không tồn tại!");
                }
                var accountSystems = lstAccCode.Where(p => p.AccCode == dto.ImportCreditAcc && p.AccType != "C").ToList();
                if (accountSystems.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.ImportCreditAcc + " là tài khoản mẹ!");
                }
            }
            if (!string.IsNullOrEmpty(dto.ExciseTaxDebitAcc))
            {
                var exciseTaxDebitAcc = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxDebitAcc).ToList();
                if (exciseTaxDebitAcc.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.ExciseTaxCreditAcc + " không tồn tại!");
                }
                var accountSystems = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxDebitAcc && p.AccType != "C").ToList();
                if (accountSystems.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.ExciseTaxDebitAcc + " là tài khoản mẹ!");
                }
            }

            if (!string.IsNullOrEmpty(dto.ExciseTaxCreditAcc))
            {
                var exciseTaxCreditAcc = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxCreditAcc).ToList();
                if (exciseTaxCreditAcc.Count == 0)
                {
                    throw new Exception("Mã tài khoản " + dto.ExciseTaxCreditAcc + " không tồn tại!!");
                }
                var accountSystems = lstAccCode.Where(p => p.AccCode == dto.ExciseTaxCreditAcc && p.AccType != "C").ToList();
                if (accountSystems.Count > 0)
                {
                    throw new Exception("Mã tài khoản " + dto.ExciseTaxCreditAcc + " là tài khoản mẹ!");
                }

            }
        }

        private async Task<ErrorDto> CheckIsAssembly(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            var errorDto = new ErrorDto();
            if (voucherCategory.IsAssembly == "C")
            {
                var productVoucherDetails = dto.ProductVoucherDetails;
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
            return errorDto;
        }

        private async Task<ErrorDto> CheckDetail(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            var errorDto = new ErrorDto();
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
                        if (lstWareHouse2 == null)
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
                        if (lstWareHouse2 != null)
                        {

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
                            if (lstProduct == null)
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
            return errorDto;
        }

        private async Task<ErrorDto> CheckAccounting(CrudProductVoucherDto dto, VoucherCategoryDto voucherCategory)
        {
            var errorDto = new ErrorDto();
            var voucherTypes = await _accountingCacheManager.GetVoucherTypeAsync();
            var voucherType1 = voucherTypes.Where(p => p.Code == "000").FirstOrDefault();
            var voucherType2 = voucherTypes.Where(p => p.Code == "NO1").FirstOrDefault();

            var lstcrudProductVoucherDetailDtos = dto.ProductVoucherDetails;
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
            if (voucherType1 != null)
            {
                if (voucherType1.ListVoucher.Contains(dto.VoucherCode) == false)
                {
                    foreach (var item in lstcrudProductVoucherDetailDtos)
                    {
                        var crud = new CrudProductvoucherCheckDto();
                        crud.Ord0 = item.Ord0;
                        crud.Year = item.Year;
                        crud.OrgCode = item.OrgCode;
                        crud.DebitAcc = item.DebitAcc;
                        crud.PartnerCode = string.IsNullOrEmpty(item.PartnerCode) == true ? dto.PartnerCode
                                                    : item.PartnerCode;
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

            if (voucherType2.ListVoucher == null)
            {
                if ("PBH,PTL".Contains(dto.VoucherCode) == true)
                {
                    foreach (var item in lstcrudProductVoucherDetailDtos)
                    {
                        var crud = new CrudProductvoucherCheckDto();
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


            var crudProductVoucherCostDtos = dto.ProductVoucherCostDetails;
            if (crudProductVoucherCostDtos != null)
            {
                foreach (var item in crudProductVoucherCostDtos)
                {
                    var crud = new CrudProductvoucherCheckDto();
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
            errorDto = await CheckAccountingAccVoucherDetail(dto, crudAccVoucherDetailDtos);
            return errorDto;
        }

        private async Task<ErrorDto> CheckAccountingAccVoucherDetail(CrudProductVoucherDto dto, List<CrudProductvoucherCheckDto> crudAccVoucherDetailDtos)
        {
            var errorDto = new ErrorDto();
            var partner = await _accPartnerService.GetQueryableAsync();
            var accountSystem = await _accountingCacheManager.GetAccountSystemsAsync(dto.Year, dto.OrgCode);
            var contract = await _contractService.GetQueryableAsync();
            var fpProduct = await _fProductWorkService.GetQueryableAsync();
            var sessionCode = await _accSectionService.GetQueryableAsync();
            var workplace = await _workPlaceSevice.GetQueryableAsync();
            var caseCode = await _accCaseService.GetQueryableAsync();
            foreach (var item in crudAccVoucherDetailDtos)
            {
                if (!string.IsNullOrEmpty(item.CaseCode))
                {
                    bool isExistCaseCode = await _accCaseService.IsExistCode(dto.OrgCode, item.CaseCode);
                    if (!isExistCaseCode)
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
                if (!string.IsNullOrEmpty(item.WorkPlaceCode))
                {
                    bool isExistWorkPlace1 = await _workPlaceSevice.IsExistCode(dto.OrgCode, item.WorkPlaceCode);
                    if (!isExistWorkPlace1)
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
                }
                if (!string.IsNullOrEmpty(item.ClearingWorkPlaceCode) == true)
                {
                    bool isExistWorkPlace2 = await _workPlaceSevice.IsExistCode(dto.OrgCode, item.ClearingWorkPlaceCode);
                    if (!isExistWorkPlace2)
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
                }
                if (!string.IsNullOrEmpty(item.SectionCode) == true)
                {
                    bool isExistSectionCode1 = await _accSectionService.IsExistCode(dto.OrgCode, item.SectionCode);
                    if (!isExistSectionCode1)
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
                }
                if (!string.IsNullOrEmpty(item.ClearingSectionCode) == true)
                {
                    bool isExistSectionCode2 = await _accSectionService.IsExistCode(dto.OrgCode, item.ClearingSectionCode);
                    if (!isExistSectionCode2)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Mã khoản mục " + item.ClearingSectionCode + " ko tồn tại!";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "SectionCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                }
                if (!string.IsNullOrEmpty(item.PartnerCode) == true)
                {
                    bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, item.PartnerCode);
                    if (!isExistPartner)
                    {
                        errorDto.Code = "101";
                        errorDto.Message = "Mã đối tượng " + item.PartnerCode + " không tồn tại !";
                        InforDto inforDto = new InforDto();
                        inforDto.FieldName = "PartnerCode";
                        inforDto.TabTable = "ProductVoucherDetail";
                        inforDto.RowNumber = item.Ord0;
                        List<InforDto> inforDtos = new List<InforDto>();
                        inforDtos.Add(inforDto);
                        errorDto.inforDtos = inforDtos;
                        return errorDto;
                    }
                }

                var accountSystem1 = accountSystem.Where(p => p.OrgCode == dto.OrgCode && p.Year == dto.Year
                                                && p.AccCode == item.DebitAcc).FirstOrDefault();
                if (accountSystem1 != null)
                {
                    if (accountSystem1.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, dto.PartnerCode0);
                            if (!isExistPartner)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(item.DebitAcc) == true && string.IsNullOrEmpty(accountSystem1.AccCode) == true)
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

                    if (string.IsNullOrEmpty(dto.PartnerCode0) == true && accountSystem1.AttachPartner == "C")
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
                    if (string.IsNullOrEmpty(item.ContractCode) == true && accountSystem1.AttachContract == "C")
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
                    if (string.IsNullOrEmpty(item.FProductWorkCode) == true && accountSystem1.AttachProductCost == "C")
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
                    if (string.IsNullOrEmpty(item.CaseCode) == true && accountSystem1.AccSectionCode == "C")
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
                    if (string.IsNullOrEmpty(item.WorkPlaceCode) == true && accountSystem1.AccSectionCode == "C")
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
                    if (string.IsNullOrEmpty(item.CreditAcc) == true && accountSystem1.IsBalanceSheetAcc != "C")
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
                var accountSystem2 = accountSystem.Where(p => p.OrgCode == dto.OrgCode && p.Year == dto.Year
                                    && p.AccCode == item.CreditAcc).FirstOrDefault();
                if (accountSystem2 != null)
                {
                    if (accountSystem2.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, dto.PartnerCode0);
                            if (!isExistPartner)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(item.DebitAcc) == true && accountSystem2.IsBalanceSheetAcc != "C")
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
                    if (string.IsNullOrEmpty(item.CreditAcc) == true && string.IsNullOrEmpty(accountSystem2.AccCode) == true)
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

                    if (string.IsNullOrEmpty(item.ContractCode) == true && string.IsNullOrEmpty(item.ClearingContractCode) == true && accountSystem2.AttachContract == "C")
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
                    if (string.IsNullOrEmpty(item.FProductWorkCode) == true && string.IsNullOrEmpty(item.ClearingFProductWorkCode) == true && accountSystem2.AttachProductCost == "C" && dto.VoucherCode != "PX8")
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
                    if (string.IsNullOrEmpty(item.WorkPlaceCode) == true && string.IsNullOrEmpty(item.WorkPlaceCode) == true && accountSystem2.AttachWorkPlace == "C")
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

                var accountSystemHKD1 = accountSystem.Where(p => p.OrgCode == dto.OrgCode && p.Year == dto.Year
                                    && p.AccCode == item.DebitAcc2).FirstOrDefault();
                if (accountSystemHKD1 != null)
                {
                    if (accountSystemHKD1.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;
                        }
                        else
                        {
                            bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, dto.PartnerCode0);
                            if (!isExistPartner)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                }
                var accountSystemHKD2 = accountSystem.Where(p => p.OrgCode == dto.OrgCode && p.Year == dto.Year
                                                && p.AccCode == item.CreditAcc2).FirstOrDefault();
                if (accountSystemHKD2 != null)
                {
                    if (accountSystemHKD2.AttachPartner == "C")
                    {
                        if (string.IsNullOrEmpty(dto.PartnerCode0) == true)
                        {
                            errorDto.Code = "101";
                            errorDto.Message = "Bạn phải nhập mã đối tượng!";
                            return errorDto;

                        }
                        else
                        {
                            bool isExistPartner = await _accPartnerService.IsExistCode(dto.OrgCode, dto.PartnerCode0);
                            if (!isExistPartner)
                            {
                                errorDto.Code = "101";
                                errorDto.Message = "Mã đối tượng " + dto.PartnerCode0 + " không tồn tại!";
                                return errorDto;
                            }
                        }
                    }
                }
                if (!string.IsNullOrEmpty(item.ClearingPartnerCode) == true)
                {
                    bool isExists = await _accPartnerService.IsExistCode(dto.OrgCode, item.ClearingPartnerCode);
                    if (!isExists)
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
                }
                if (!string.IsNullOrEmpty(item.ContractCode) == true)
                {
                    bool isExist = await _contractService.IsExistCode(dto.OrgCode, item.ContractCode);
                    if (!isExist)
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
                }
                if (!string.IsNullOrEmpty(item.ClearingContractCode) == true)
                {
                    bool isExists = await _contractService.IsExistCode(dto.OrgCode, item.ClearingContractCode);
                    if (!isExists)
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
                }
                if (!string.IsNullOrEmpty(item.FProductWorkCode) == true)
                {
                    var fpProduct1 = fpProduct.Where(p => p.OrgCode == dto.OrgCode && p.Code == item.FProductWorkCode).FirstOrDefault();
                    if (fpProduct1 == null)
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
                    if (fpProduct1.FPWType == "C")
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
                }

                if (!string.IsNullOrEmpty(item.ClearingFProductWorkCode) == true)
                {
                    var fpProduct2 = fpProduct.Where(p => p.OrgCode == dto.OrgCode && p.Code == item.ClearingFProductWorkCode).FirstOrDefault();
                    if (fpProduct2 == null)
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
                    if (fpProduct2.FPWType == "C")
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
                }
            }
            return errorDto;
        }

        private async Task<IQueryable<CrudProductVoucherDto>> Filter(PageRequestDto dto)
        {
            var queryable = await _productVoucher.GetQueryableAsync();
            var productAproductVoucherReceipt = await _productVoucherReceipt.GetQueryableAsync();
            var productVoucherAssembly = await _productVoucherAssembly.GetQueryableAsync();
            var lstproductVoucherAssembly = productVoucherAssembly.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstqueryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var lstproductAproductVoucherReceipt = productAproductVoucherReceipt.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();
            var productVoucherDetail = await _productVoucherDetail.GetQueryableAsync();
            var lstProductVoucherDetail = productVoucherDetail.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()).ToList();

            var reusl = from a in lstqueryable
                        join b in lstproductAproductVoucherReceipt on new { a.OrgCode, ProductVoucherId = a.Id } equals new { b.OrgCode, b.ProductVoucherId } into c
                        from pr in c.DefaultIfEmpty()
                        join d in lstproductVoucherAssembly on a.Id equals d.ProductVoucherId into e
                        from lst in e.DefaultIfEmpty()

                        select new CrudProductVoucherDto
                        {
                            Id = a.Id,
                            Year = a.Year,
                            DepartmentCode = a.DepartmentCode,
                            VoucherCode = a.VoucherCode,
                            VoucherGroup = a.VoucherGroup,
                            BusinessCode = a.BusinessCode,
                            BusinessAcc = a.BusinessAcc,
                            VoucherNumber = a.VoucherNumber,
                            InvoiceNumber = a.InvoiceNumber,
                            VoucherDate = a.VoucherDate,
                            PaymentTermsCode = a.PaymentTermsCode,
                            PartnerCode0 = a.PartnerCode0,
                            PartnerName0 = a.PartnerName0,
                            Representative = a.Representative,
                            Address = a.Address,
                            Tel = a.Tel,
                            Description = a.Description,
                            DescriptionE = a.DescriptionE,
                            Place = a.Place,
                            OriginVoucher = a.OriginVoucher,
                            CurrencyCode = a.CurrencyCode,
                            ExchangeRate = (decimal)a.ExchangeRate,
                            TotalAmountWithoutVatCur = a.TotalAmountWithoutVatCur,
                            TotalAmountWithoutVat = a.TotalAmountWithoutVat,
                            TotalDiscountAmountCur = a.TotalDiscountAmountCur,
                            TotalDiscountAmount = a.TotalDiscountAmount,
                            TotalVatAmountCur = a.TotalVatAmountCur,
                            TotalVatAmount = a.TotalVatAmount,
                            DebitOrCredit = a.DebitOrCredit,
                            TotalAmountCur = a.TotalAmountCur,
                            TotalAmount = a.TotalAmount,
                            TotalProductAmountCur = a.TotalProductAmountCur,
                            TotalProductAmount = a.TotalProductAmount,
                            TotalExciseTaxAmountCur = a.TotalExciseTaxAmountCur,
                            TotalExciseTaxAmount = a.TotalExciseTaxAmount,
                            TotalQuantity = a.TotalQuantity,
                            ExportNumber = a.ExportNumber,
                            TotalExpenseAmountCur0 = a.TotalExpenseAmountCur0,
                            TotalExpenseAmount0 = a.TotalExpenseAmount0,
                            TotalImportTaxAmountCur = a.TotalImportTaxAmountCur,
                            TotalImportTaxAmount = a.TotalImportTaxAmount,
                            TotalExpenseAmountCur = a.TotalExpenseAmountCur,
                            TotalExpenseAmount = a.TotalExpenseAmount,
                            EmployeeCode = a.EmployeeCode,
                            Status = a.Status,
                            PaymentTermsId = a.PaymentTermsId,
                            SalesChannelCode = a.SalesChannelCode,
                            BillNumber = a.BillNumber,
                            DevaluationPercentage = a.DevaluationPercentage,
                            TotalDevaluationAmountCur = a.TotalDevaluationAmountCur,
                            TotalDevaluationAmount = a.TotalDevaluationAmount,
                            PriceDebitAcc = a.PriceDebitAcc,
                            PriceCreditAcc = a.PriceCreditAcc,
                            PriceDecreasingDescription = a.PriceDecreasingDescription,
                            IsCreatedEInvoice = a.IsCreatedEInvoice,
                            InfoFilter = a.InfoFilter,
                            RefType = a.RefType,
                            Vehicle = a.Vehicle,
                            OtherDepartment = a.OtherDepartment,
                            CommandNumber = a.CommandNumber,
                            TotalExpenseAmountCur1 = a.TotalExpenseAmountCur1,
                            TotalExpenseAmount1 = a.TotalExpenseAmount1,
                            PaymentDebitAcc = a.PaymentDebitAcc,
                            PaymentCreditAcc = a.PaymentCreditAcc,
                            ExcutionStatus = a.ExcutionStatus,
                            DiscountPercentage = pr != null ? pr.DiscountPercentage : null,
                            DiscountCreditAcc = pr != null ? pr.DiscountCreditAcc : "",
                            DiscountDebitAcc = pr != null ? pr.DiscountDebitAcc : "",
                            DiscountAmountCur = pr != null ? pr.DiscountAmountCur0 : null,
                            DiscountAmount = pr != null ? pr.DiscountAmount0 : null,
                            ImportTaxPercentage = pr != null ? pr.ImportTaxPercentage : null,
                            ImportCreditAcc = pr != null ? pr.ImportCreditAcc : null,
                            ImportDebitAcc = pr != null ? pr.ImportDebitAcc : null,
                            ExciseTaxPercentage = pr != null ? pr.ExciseTaxPercentage : null,
                            ExciseTaxCreditAcc = pr != null ? pr.ExciseTaxCreditAcc : null,
                            ExciseTaxDebitAcc = pr != null ? pr.ExciseTaxDebitAcc : null,
                            DiscountAmount0 = pr != null ? pr.DiscountAmount0 : null,
                            DiscountAmountCur0 = pr != null ? pr.DiscountAmountCur0 : null,
                            DiscountCreditAcc0 = pr != null ? pr.DiscountCreditAcc0 : null,
                            DiscountDebitAcc0 = pr != null ? pr.DiscountDebitAcc0 : null,
                            DiscountDescription0 = pr != null ? pr.DiscountDescription0 : null,
                            AssemblyProductCode = lst != null ? lst.AssemblyProductCode : null,
                            AssemblyUnitCode = lst != null ? lst.AssemblyUnitCode : null,
                            AssemblyWarehouseCode = lst != null ? lst.AssemblyWarehouseCode : null,
                            Quantity = lst != null ? (decimal)lst.Quantity : 0

                        };
            var window = await _windowService.GetByIdAsync(dto.WindowId);
            reusl = reusl.Where(p => p.Year == _webHelper.GetCurrentYear()
                                               && p.VoucherCode == (window == null ? "" : window.VoucherCode)
                                               );

            return reusl.AsQueryable();
        }
        private decimal GetVoucherNumber(string VoucherNumber)
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

        private async Task<IQueryable<ProductVoucherDto>> FilterProductVoucher(PageRequestDto dto)
        {
            var queryable = await _productVoucher.GetQueryableAsync();
            queryable = queryable.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit()
                                  && p.Year == _webHelper.GetCurrentYear());
            var isViewByUserNew = await this.IsViewByUserNew();
            if (isViewByUserNew)
            {
                queryable = queryable.Where(p => p.CreatorId == _currentUser.Id);
            }
            var window = await _accountingCacheManager.GetWindowAsync(dto.WindowId);
            queryable = queryable.Where(p => p.VoucherCode == window.VoucherCode);
            queryable = await this.GetFilterRows(queryable, dto);
            queryable = await this.GetFilterAdvanced(queryable, dto);
            var productAproductVoucherReceipt = await _productVoucherReceipt.GetQueryableAsync();
            productAproductVoucherReceipt = productAproductVoucherReceipt.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucherAssembly = await _productVoucherAssembly.GetQueryableAsync();
            productVoucherAssembly = productVoucherAssembly.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());

            var reuslt = from a in queryable
                         join b in productAproductVoucherReceipt on a.Id equals b.ProductVoucherId into c
                         from pr in c.DefaultIfEmpty()
                         join d in productVoucherAssembly on a.Id equals d.ProductVoucherId into e
                         from lst in e.DefaultIfEmpty()
                         select new ProductVoucherDto
                         {
                             Id = a.Id,
                             Year = a.Year,
                             DepartmentCode = a.DepartmentCode,
                             VoucherCode = a.VoucherCode,
                             VoucherGroup = a.VoucherGroup,
                             BusinessCode = a.BusinessCode,
                             BusinessAcc = a.BusinessAcc,
                             VoucherNumber = a.VoucherNumber,
                             InvoiceNumber = a.InvoiceNumber,
                             VoucherDate = a.VoucherDate,
                             PaymentTermsCode = a.PaymentTermsCode,
                             PartnerCode0 = a.PartnerCode0,
                             PartnerName0 = a.PartnerName0,
                             Representative = a.Representative,
                             Address = a.Address,
                             Tel = a.Tel,
                             Description = a.Description,
                             DescriptionE = a.DescriptionE,
                             Place = a.Place,
                             OriginVoucher = a.OriginVoucher,
                             CurrencyCode = a.CurrencyCode,
                             ExchangeRate = (decimal)a.ExchangeRate,
                             TotalAmountWithoutVatCur = a.TotalAmountWithoutVatCur,
                             TotalAmountWithoutVat = a.TotalAmountWithoutVat,
                             TotalDiscountAmountCur = a.TotalDiscountAmountCur,
                             TotalDiscountAmount = a.TotalDiscountAmount,
                             TotalVatAmountCur = a.TotalVatAmountCur,
                             TotalVatAmount = a.TotalVatAmount,
                             DebitOrCredit = a.DebitOrCredit,
                             TotalAmountCur = a.TotalAmountCur,
                             TotalAmount = a.TotalAmount,
                             TotalProductAmountCur = a.TotalProductAmountCur,
                             TotalProductAmount = a.TotalProductAmount,
                             TotalExciseTaxAmountCur = a.TotalExciseTaxAmountCur,
                             TotalExciseTaxAmount = a.TotalExciseTaxAmount,
                             TotalQuantity = a.TotalQuantity,
                             ExportNumber = a.ExportNumber,
                             TotalExpenseAmountCur0 = a.TotalExpenseAmountCur0,
                             TotalExpenseAmount0 = a.TotalExpenseAmount0,
                             TotalImportTaxAmountCur = a.TotalImportTaxAmountCur,
                             TotalImportTaxAmount = a.TotalImportTaxAmount,
                             TotalExpenseAmountCur = a.TotalExpenseAmountCur,
                             TotalExpenseAmount = a.TotalExpenseAmount,
                             EmployeeCode = a.EmployeeCode,
                             Status = a.Status,
                             PaymentTermsId = a.PaymentTermsId,
                             SalesChannelCode = a.SalesChannelCode,
                             BillNumber = a.BillNumber,
                             DevaluationPercentage = a.DevaluationPercentage,
                             TotalDevaluationAmountCur = a.TotalDevaluationAmountCur,
                             TotalDevaluationAmount = a.TotalDevaluationAmount,
                             PriceDebitAcc = a.PriceDebitAcc,
                             PriceCreditAcc = a.PriceCreditAcc,
                             PriceDecreasingDescription = a.PriceDecreasingDescription,
                             IsCreatedEInvoice = a.IsCreatedEInvoice,
                             InfoFilter = a.InfoFilter,
                             RefType = a.RefType,
                             Vehicle = a.Vehicle,
                             OtherDepartment = a.OtherDepartment,
                             CommandNumber = a.CommandNumber,
                             TotalExpenseAmountCur1 = a.TotalExpenseAmountCur1,
                             TotalExpenseAmount1 = a.TotalExpenseAmount1,
                             PaymentDebitAcc = a.PaymentDebitAcc,
                             PaymentCreditAcc = a.PaymentCreditAcc,
                             ExcutionStatus = a.ExcutionStatus,
                             DiscountPercentage = pr != null ? pr.DiscountPercentage : null,
                             DiscountCreditAcc = pr != null ? pr.DiscountCreditAcc : "",
                             DiscountDebitAcc = pr != null ? pr.DiscountDebitAcc : "",
                             DiscountAmountCur = pr != null ? pr.DiscountAmountCur0 : null,
                             DiscountAmount = pr != null ? pr.DiscountAmount0 : null,
                             ImportTaxPercentage = pr != null ? pr.ImportTaxPercentage : null,
                             ImportCreditAcc = pr != null ? pr.ImportCreditAcc : null,
                             ImportDebitAcc = pr != null ? pr.ImportDebitAcc : null,
                             ExciseTaxPercentage = pr != null ? pr.ExciseTaxPercentage : null,
                             ExciseTaxCreditAcc = pr != null ? pr.ExciseTaxCreditAcc : null,
                             ExciseTaxDebitAcc = pr != null ? pr.ExciseTaxDebitAcc : null,
                             DiscountAmount0 = pr != null ? pr.DiscountAmount0 : null,
                             DiscountAmountCur0 = pr != null ? pr.DiscountAmountCur0 : null,
                             DiscountCreditAcc0 = pr != null ? pr.DiscountCreditAcc0 : null,
                             DiscountDebitAcc0 = pr != null ? pr.DiscountDebitAcc0 : null,
                             DiscountDescription0 = pr != null ? pr.DiscountDescription0 : null,
                             AssemblyProductCode = lst != null ? lst.AssemblyProductCode : null,
                             AssemblyUnitCode = lst != null ? lst.AssemblyUnitCode : null,
                             AssemblyWarehouseCode = lst != null ? lst.AssemblyWarehouseCode : null,
                             Quantity = lst != null ? (decimal)lst.Quantity : 0

                         };

            return reuslt.AsQueryable();
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
        public async Task<PageResultDto<ProductVoucherDto>> PagesAsync(PageRequestDto dto)
        {
            return await GetListAsync(dto);
        }
        public async Task<List<CrudAccTaxDetailDto>> GetAccTaxDetailAsync(string productVoucherId)
        {
            var result = new List<CrudAccTaxDetailDto>();
            var query = await _accTaxDetailService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<AccTaxDetail, CrudAccTaxDetailDto>(p)).ToList();
            return result;
        }

        public async Task<List<CrudProductVoucherAssemblyDto>> GetProductVoucherAssemblyAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherAssemblyDto>();
            var query = await _productVoucherAssembly.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherAssembly, CrudProductVoucherAssemblyDto>(p)).ToList();
            return result;
        }

        public async Task<List<CrudProductVoucherReceiptDto>> GetProductVoucherReceiptAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherReceiptDto>();
            var query = await _productVoucherReceipt.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherReceipt, CrudProductVoucherReceiptDto>(p)).ToList();
            return result;
        }

        public async Task<List<CrudProductVoucherVatDto>> GetProductVoucherVatAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherVatDto>();
            var query = await _productVoucherVat.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherVat, CrudProductVoucherVatDto>(p)).ToList();
            return result;
        }

        public async Task<List<CrudProductVoucherCostDto>> GetProductVoucherCostDetailAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherCostDto>();
            var query = await _productVoucherCostService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<ProductVoucherCost, CrudProductVoucherCostDto>(p)).ToList();
            return result;
        }

        public async Task<List<CrudVoucherExciseTaxDto>> GetVoucherExciseTaxAsync(string productVoucherId)
        {
            var result = new List<CrudVoucherExciseTaxDto>();
            var query = await _voucherExciseTaxService.GetQueryableAsync();
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var sections = await AsyncExecuter.ToListAsync(query);
            result = sections.Select(p => ObjectMapper.Map<VoucherExciseTax, CrudVoucherExciseTaxDto>(p)).ToList();
            return result;
        }

        public async Task<List<CrudProductVoucherDetailDto>> GetProductVoucherDetailAsync(string productVoucherId)
        {
            var result = new List<CrudProductVoucherDetailDto>();
            var query = await _productVoucherDetail.GetQueryableAsync();
            var querry = await _productVoucherDetailReceiptService.GetQueryableAsync();
            var product = await _product.GetQueryableAsync();
            var lstProduct = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            query = query.Where(p => p.ProductVoucherId == productVoucherId);
            var resultProduct = from i in query
                                join j in querry on i.Id equals j.ProductVoucherDetailId into ajp
                                from ap in ajp.DefaultIfEmpty()
                                join a in lstProduct on i.ProductCode equals a.Code into c
                                from pr in c.DefaultIfEmpty()
                                select new CrudProductVoucherDetailDto
                                {
                                    Id = i.Id,
                                    ProductVoucherId = i.ProductVoucherId,
                                    Ord0 = i.Ord0,
                                    Year = i.Year,
                                    ProductCode = i.ProductCode,
                                    TransProductCode = i.TransProductCode,
                                    ProductName = i.ProductName,
                                    ProductName0 = i.ProductName0,
                                    UnitCode = i.UnitCode,
                                    TransUnitCode = i.TransUnitCode,
                                    WarehouseCode = i.WarehouseCode,
                                    TransWarehouseCode = i.TransWarehouseCode,
                                    TrxQuantity = i.TrxQuantity,
                                    Quantity = i.Quantity,
                                    PriceCur = i.PriceCur,
                                    Price = i.Price,
                                    TrxAmountCur = i.TrxAmountCur,
                                    TrxAmount = i.TrxAmount,
                                    AmountCur = i.AmountCur,
                                    Amount = i.Amount,
                                    FixedPrice = i.FixedPrice,
                                    DebitAcc = i.DebitAcc,
                                    CreditAcc = i.CreditAcc,
                                    ProductLotCode = i.ProductLotCode,
                                    TransProductLotCode = i.TransProductLotCode,
                                    ProductOriginCode = i.ProductOriginCode,
                                    TransProductOriginCode = i.TransProductOriginCode,
                                    PartnerCode = i.PartnerCode,
                                    FProductWorkCode = i.FProductWorkCode,
                                    ContractCode = i.ContractCode,
                                    WorkPlaceCode = i.WorkPlaceCode,
                                    SectionCode = i.SectionCode,
                                    CaseCode = i.CaseCode,
                                    PriceCur2 = i.PriceCur2,
                                    Price2 = i.Price2,
                                    AmountCur2 = i.AmountCur2,
                                    Amount2 = i.Amount2,
                                    DebitAcc2 = i.DebitAcc2,
                                    CreditAcc2 = i.CreditAcc2,
                                    TaxCategoryCode = i.TaxCategoryCode,
                                    InsuranceDate = i.InsuranceDate,
                                    Note = i.Note,
                                    NoteE = i.NoteE,
                                    HTPercentage = i.HTPercentage,
                                    RevenueAmount = i.RevenueAmount,
                                    VatPriceCur = i.VatPriceCur,
                                    VatPrice = i.VatPrice,
                                    DevaluationPercentage = i.DevaluationPercentage,
                                    DevaluationPriceCur = i.DevaluationPriceCur,
                                    DevaluationPrice = i.DevaluationPrice,
                                    DevaluationAmountCur = i.DevaluationAmountCur,
                                    DevaluationAmount = i.DevaluationAmount,
                                    VarianceAmount = i.VarianceAmount,
                                    RefId = i.RefId,
                                    ProductVoucherDetailId = ap.ProductVoucherDetailId,
                                    VatTaxAmount = i.VatTaxAmount,
                                    DecreasePercentage = i.DecreasePercentage,
                                    DecreaseAmount = i.DecreaseAmount,
                                    AmountWithVat = i.AmountWithVat,
                                    AmountAfterDecrease = i.AmountAfterDecrease,
                                    VatAmount = i.VatAmount,
                                    VatPercentage = ap.VatPercentage,
                                    VatAmountCur = ap.VatAmountCur,
                                    DiscountPercentage = ap.DiscountPercentage,
                                    DiscountAmount = ap.DiscountAmount,
                                    DiscountAmountCur = ap.DiscountAmountCur,
                                    ImportTaxPercentage = ap.ImportTaxPercentage,
                                    ImportTaxAmountCur = ap.ImportTaxAmountCur,
                                    ImportTaxAmount = ap.ImportTaxAmount,
                                    ExpenseAmount0 = ap.ExpenseAmount0,
                                    ExpenseAmountCur0 = ap.ExpenseAmountCur0,
                                    ExciseTaxPercentage = ap.ExciseTaxPercentage,
                                    ExciseTaxAmountCur = ap.ExciseTaxAmountCur,
                                    ExciseTaxAmount = ap.ExciseTaxAmount,
                                    ExpenseImportTaxAmountCur = ap.ExpenseAmountCur0,
                                    ExpenseImportTaxAmount = ap.ExpenseAmount0,
                                    ExpenseAmountCur1 = ap.ExpenseAmountCur1,
                                    ExpenseAmount1 = ap.ExpenseAmount1,
                                    ExpenseAmountCur = ap.ExpenseAmountCur,
                                    ExpenseAmount = ap.ExpenseAmount,
                                    AttachProductLot = pr != null ? pr.AttachProductLot : null,
                                    AttachProductOrigin = pr != null ? pr.AttachProductOrigin : null,
                                    ProductAcc = pr != null ? pr.ProductAcc : null

                                };
            var sections = await AsyncExecuter.ToListAsync(resultProduct);

            //result = sections.Select(p => ObjectMapper.Map<ProductVoucherDetail, CrudProductVoucherDetailDto>(p)).ToList();
            return sections;
        }

        public async Task<List<ProductVoucherCustomineDto>> ListProductVoucherDetailAsync(ParameterFillters dto)
        {
            var result = new List<ProductVoucherCustomineDto>();
            var query = await _productVoucher.GetQueryableAsync();
            var queryDetail = await _productVoucherDetail.GetQueryableAsync();
            query = query.Where(p => p.VoucherDate >= dto.FromDate && p.VoucherDate <= dto.ToDate && p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var product = await _product.GetQueryableAsync();
            product = product.Where(p => p.OrgCode == _webHelper.GetCurrentOrgUnit());
            var productVoucherReceipt = await _productVoucherReceipt.GetQueryableAsync();
            var query2 = (from a in query
                          join b in queryDetail on a.Id equals b.ProductVoucherId
                          join c in product on b.ProductCode equals c.Code into d
                          from pro in d.DefaultIfEmpty()
                          join e in productVoucherReceipt on a.Id equals e.ProductVoucherId into f
                          from g in f.DefaultIfEmpty()
                          where dto.VoucherCode.Contains(a.VoucherCode) == true
                          select new ProductVoucherCustomineDto
                          {
                              Year = a.Year,
                              DepartmentCode = a.DepartmentCode,
                              VoucherCode = a.VoucherCode,
                              VoucherGroup = a.VoucherGroup,
                              BusinessCode = a.BusinessCode,
                              BusinessAcc = a.BusinessAcc,
                              VoucherNumber = a.VoucherNumber,
                              InvoiceNumber = a.InvoiceNumber,
                              VoucherDate = a.VoucherDate,
                              PaymentTermsCode = a.PaymentTermsCode,
                              PartnerCode0 = a.PartnerCode0,
                              PartnerName0 = a.PartnerName0,
                              Representative = a.Representative,
                              Address = a.Address,
                              Tel = a.Tel,
                              Description = a.Description,
                              DescriptionE = a.DescriptionE,
                              Place = a.Place,
                              OriginVoucher = a.OriginVoucher,
                              CurrencyCode = a.CurrencyCode,
                              ExchangeRate = a.ExchangeRate,
                              TotalAmountWithoutVatCur = a.TotalAmountWithoutVatCur,
                              TotalAmountWithoutVat = a.TotalAmountWithoutVat,
                              TotalDiscountAmountCur = a.TotalDiscountAmountCur,
                              TotalDiscountAmount = a.TotalDiscountAmount,
                              TotalVatAmountCur = a.TotalVatAmountCur,
                              TotalVatAmount = a.TotalVatAmount,
                              DebitOrCredit = a.DebitOrCredit,
                              TotalAmountCur = a.TotalAmountCur,
                              TotalAmount = a.TotalAmount,
                              TotalProductAmountCur = a.TotalProductAmountCur,
                              TotalProductAmount = a.TotalProductAmount,
                              TotalExciseTaxAmountCur = a.TotalExciseTaxAmountCur,
                              TotalExciseTaxAmount = a.TotalExciseTaxAmount,
                              TotalQuantity = a.TotalQuantity,
                              ExportNumber = a.ExportNumber,
                              TotalExpenseAmountCur0 = a.TotalExpenseAmountCur0,
                              TotalExpenseAmount0 = a.TotalExpenseAmount0,
                              TotalImportTaxAmountCur = a.TotalImportTaxAmountCur,
                              TotalImportTaxAmount = a.TotalImportTaxAmount,
                              TotalExpenseAmountCur = a.TotalExpenseAmountCur,
                              TotalExpenseAmount = a.TotalExpenseAmount,
                              EmployeeCode = a.EmployeeCode,
                              DeliveryDate = a.DeliveryDate,
                              Status = a.Status,
                              PaymentTermsId = a.PaymentTermsId,
                              SalesChannelCode = a.SalesChannelCode,
                              BillNumber = a.BillNumber,
                              DevaluationPercentage = a.DevaluationPercentage,
                              TotalDevaluationAmountCur = a.TotalDevaluationAmountCur,
                              TotalDevaluationAmount = a.TotalDevaluationAmount,
                              PriceDebitAcc = a.PriceDebitAcc,
                              PriceCreditAcc = a.PriceCreditAcc,
                              PriceDecreasingDescription = a.PriceDecreasingDescription,
                              IsCreatedEInvoice = a.IsCreatedEInvoice,
                              InfoFilter = a.InfoFilter,
                              RefType = a.RefType,
                              ImportDate = a.ImportDate,
                              ExportDate = a.ExportDate,
                              ProductVoucherId = b.ProductVoucherId,
                              Ord0 = b.Ord0,
                              ProductCode = b.ProductCode,
                              TransProductCode = b.TransProductCode,
                              ProductName = b.ProductName,
                              ProductName0 = b.ProductName0,
                              UnitCode = b.UnitCode,
                              TransUnitCode = b.TransUnitCode,
                              WarehouseCode = b.WarehouseCode,
                              TransWarehouseCode = b.TransWarehouseCode,
                              TrxQuantity = b.TrxQuantity,
                              Quantity = b.Quantity,
                              PriceCur = b.PriceCur,
                              Price = b.Price,
                              TrxAmountCur = b.TrxAmountCur,
                              TrxAmount = b.TrxAmount,
                              AmountCur = b.AmountCur,
                              Amount = b.Amount,
                              FixedPrice = b.FixedPrice,
                              DebitAcc = b.DebitAcc,
                              CreditAcc = b.CreditAcc,
                              ProductLotCode = b.ProductLotCode,
                              TransProductLotCode = b.TransProductLotCode,
                              ProductOriginCode = b.ProductOriginCode,
                              TransProductOriginCode = b.TransProductOriginCode,
                              PartnerCode = b.PartnerCode,
                              FProductWorkCode = b.FProductWorkCode,
                              ContractCode = b.ContractCode,
                              WorkPlaceCode = b.WorkPlaceCode,
                              SectionCode = b.SectionCode,
                              CaseCode = b.CaseCode,
                              PriceCur2 = b.PriceCur2,
                              Price2 = b.Price2,
                              AmountCur2 = b.AmountCur2,
                              Amount2 = b.Amount2,
                              DebitAcc2 = b.DebitAcc2,
                              CreditAcc2 = b.CreditAcc2,
                              TaxCategoryCode = b.TaxCategoryCode,
                              InsuranceDate = b.InsuranceDate,
                              Note = b.Note,
                              NoteE = b.NoteE,
                              HTPercentage = b.HTPercentage,
                              RevenueAmount = b.RevenueAmount,
                              VatPriceCur = b.VatPriceCur,
                              VatPrice = b.VatPrice,
                              DevaluationPriceCur = b.DevaluationPriceCur,
                              DevaluationPrice = b.DevaluationPrice,
                              DevaluationAmountCur = b.DevaluationAmountCur,
                              DevaluationAmount = b.DevaluationAmount,
                              VarianceAmount = b.VarianceAmount,
                              RefId = b.RefId,
                              ProductVoucherDetailId = b.ProductVoucherDetailId,
                              VatTaxAmount = b.VatTaxAmount,
                              DecreasePercentage = b.DecreasePercentage,
                              DecreaseAmount = b.DecreaseAmount,
                              AmountWithVat = b.AmountWithVat,
                              AmountAfterDecrease = b.AmountAfterDecrease,
                              VatAmount = b.VatAmount,
                              AttachProductLot = pro != null ? pro.AttachProductLot : null,
                              AttachProductOrigin = pro != null ? pro.AttachProductOrigin : null,
                              DiscountPercentage = g != null ? g.DiscountPercentage : null,
                              DiscountAmountCur = g != null ? g.DiscountAmountCur0 : 0,
                              DiscountAmount = g != null ? g.DiscountAmount0 : 0

                          }).ToList();

            return query2;
        }

        public async Task<UploadFileResponseDto> ImportExcel([FromForm] IFormFile upload, [FromForm] ExcelRequestDto dto)
        {
            await _licenseBusiness.ValidLicAsync();
            using var ms = new MemoryStream();
            await upload.CopyToAsync(ms);
            byte[] bytes = ms.ToArray();
            var currencyCode = await _tenantSettingService.GetTenantSettingByKeyAsync("M_MA_NT0", _webHelper.GetCurrentOrgUnit());

            var lstCrudProductVoucher = await _excelService.ImportFileToList<CrudProductVoucherImportDto>(bytes, dto.WindowId);


            var listProductvouchers = (from a in lstCrudProductVoucher
                                       where string.IsNullOrEmpty(a.VoucherNumber) == false
                                       group new { a } by new
                                       {
                                           a.VoucherNumber,
                                           a.VoucherDate,
                                           a.VoucherCode,
                                           a.InvoiceSerial,
                                           a.InvoiceNumber,
                                           a.InvoiceDate,
                                           a.CurrencyCode,
                                           a.ExchangeRate,
                                           a.PartnerCode0,
                                           a.PartnerName0,
                                           a.TaxCode,
                                           a.Representative,
                                           a.Address,
                                           a.Description,
                                           a.InvoiceGroup,
                                           a.VoucherGroup,
                                           a.BusinessCode

                                       } into gr
                                       select new CrudProductVoucherDto
                                       {
                                           VoucherGroup = gr.Key.VoucherGroup,
                                           VoucherCode = gr.Key.VoucherCode,
                                           VoucherNumber = gr.Key.VoucherNumber,
                                           VoucherDate = (DateTime)gr.Key.VoucherDate,
                                           InvoiceSerial = gr.Key.InvoiceSerial,
                                           InvoiceNumber = gr.Key.InvoiceNumber,
                                           InvoiceDate = gr.Key.InvoiceDate,
                                           CurrencyCode = gr.Key.CurrencyCode,
                                           ExchangeRate = gr.Key.ExchangeRate,
                                           PartnerCode0 = gr.Key.PartnerCode0,
                                           PartnerName0 = gr.Key.PartnerName0,
                                           DepartmentCode = gr.Max(p => p.a.DepartmentCode),
                                           PaymentTermsCode = gr.Max(p => p.a.PaymentTermsCode),
                                           SalesChannelCode = gr.Max(p => p.a.SalesChannelCode),
                                           ProductCode = gr.Max(p => p.a.ProductCode),
                                           TaxCode = gr.Key.TaxCode,
                                           Representative = gr.Key.Representative,
                                           Address = gr.Key.Address,
                                           ContractCode = gr.Max(p => p.a.ContractCode),
                                           FProductWorkCode = gr.Max(p => p.a.FProductWorkCode),
                                           WorkPlaceCode = gr.Max(p => p.a.WorkPlaceCode),
                                           SectionCode = gr.Max(p => p.a.SectionCode),
                                           CaseCode = gr.Max(p => p.a.CaseCode),
                                           TotalAmount =gr.Max(p=>p.a.VoucherCode)=="BH8"? gr.Sum(p=>p.a.AmountAfterDecrease): gr.Sum(p => p.a.Amount) + gr.Sum(p => p.a.TotalVatAmount),
                                           TotalAmountWithoutVat = gr.Sum(p => p.a.Amount),
                                           TotalAmountWithoutVatCur = gr.Sum(p => p.a.AmountCur),
                                           TotalQuantity = gr.Sum(p => p.a.Quantity),
                                           TotalVatAmountCur = gr.Sum((p) => p.a.TotalVatAmountCur),
                                           TotalVatAmount = gr.Sum(p => p.a.TotalVatAmount),
                                           TotalDiscountAmountCur = gr.Sum(p => p.a.DiscountAmountCur),
                                           TotalDiscountAmount = gr.Sum(p => p.a.DiscountAmount),
                                           TotalExciseTaxAmountCur = gr.Sum(p => p.a.ExciseTaxAmountCur),
                                           TotalExciseTaxAmount = gr.Sum(p => p.a.ExciseTaxAmount),
                                           TotalImportTaxAmount = gr.Sum(p => p.a.ImportTaxAmount),
                                           TotalImportTaxAmountCur = gr.Sum(p => p.a.ImportTaxAmountCur),
                                           Description = gr.Key.Description,
                                           InvoiceGroup = gr.Key.InvoiceGroup,
                                           CreditAccAtax = gr.Max(p => p.a.CreditAccAtax),
                                           DebitAccAtax = gr.Max(p => p.a.DebitAccAtax),
                                           DiscountPercentage = gr.Max(p => p.a.DiscountPercentage),
                                           DiscountAmountCur = gr.Max(p => p.a.DiscountAmountCur),
                                           DiscountAmount = gr.Max(p => p.a.DiscountAmount),
                                           DiscountDebitAcc = gr.Max(p => p.a.DiscountDebitAcc),
                                           DiscountCreditAcc = gr.Max(p => p.a.DiscountCreditAcc),
                                           ExciseTaxPercentage = gr.Max(p => p.a.ExciseTaxPercentage),
                                           ExciseTaxAmountCur = gr.Max(p => p.a.ExciseTaxAmountCur),
                                           ExciseTaxAmount = gr.Max(p => p.a.ExciseTaxAmount),
                                           ExciseTaxCreditAcc = gr.Max(p => p.a.ExciseTaxCreditAcc),
                                           ExciseTaxDebitAcc = gr.Max(p => p.a.ExciseTaxDebitAcc),
                                           ExciseTaxDescription = gr.Max(p => p.a.ExciseTaxDescription),
                                           VatPercentage = gr.Max(p => p.a.VatPercentage),
                                           Ord0Tax = gr.Max(p => p.a.Ord0Tax),
                                           ProductAcount = gr.Max(p => p.a.ProductAcount),
                                           ImportCreditAcc = gr.Max(p => p.a.ImportCreditAcc),
                                           ImportTaxPercentage = gr.Max(p => p.a.ImportTaxPercentage),
                                           ImportTaxAmount = gr.Max(p => p.a.ImportTaxAmount),
                                           ImportTaxAmountCur = gr.Max(p => p.a.ImportTaxAmountCur),
                                           BusinessCode = gr.Max(p => p.a.BusinessCode),
                                           Status = "1",
                                           TaxCategoryCode = gr.Max(p => p.a.TaxCategoryCode),
                                           DebitAcc= gr.Max(p=>p.a.DebitAcc),
                                           CreditAcc = gr.Max(p=>p.a.CreditAcc),
                                           DecreasePercentage = gr.Max(p=>p.a.DecreasePercentage),
                                           DecreaseAmount = gr.Max(p=>p.a.DecreaseAmount),
                                           AmountAfterDecrease = gr.Max(p=>p.a.AmountAfterDecrease)
                                       }).ToList();
            var i = 0;
            var orgCode = _webHelper.GetCurrentOrgUnit();
            var resulPartner = await _accPartnerService.GetQueryableAsync();
            resulPartner = resulPartner.Where(p => p.OrgCode == orgCode);
            var resulWimdow = await _windowService.GetQueryableAsync();
            resulWimdow = resulWimdow.Where(p => p.Id == dto.WindowId);
            foreach (var item in listProductvouchers)
            {
                i += 1;
                if (string.IsNullOrEmpty(item.PartnerCode0) == false)
                {
                    var resulPartners = resulPartner.Where(p => p.Code == item.PartnerCode0);
                    if (resulPartners.Count() <= 0)
                    {

                        throw new Exception("Mã đối tượng " + item.PartnerCode0 + " không tồn tại , tại dòng " + i + " vui lòng kiểm tra và thử lại!");
                    }
                }
                if (resulWimdow.FirstOrDefault().VoucherCode != item.VoucherCode)
                {
                    throw new Exception("Mã chứng từ không giống với phiếu hiện tại đang thực hiện tại dòng " + i + " vui lòng kiểm tra và thử lại!");

                }
                if (item.InvoiceDate != null)
                {
                    if (item.VoucherDate != item.InvoiceDate)
                    {
                        throw new Exception("Ngày chứng từ không trùng với ngày hoá đơn tại dòng " + i + " vui lòng kiểm tra và thử lại!");

                    }
                }
                var voucherGroup = await _voucherCategoryService.GetByCodeAsync(item.VoucherCode, _webHelper.GetCurrentOrgUnit());
                if ((item.BusinessCode ?? "") == "")
                {
                    if ((item.VoucherNumber ?? "") == "")
                    {
                        var voucherNumber = await _voucherNumberBusiness.AutoVoucherNumberAsync(item.VoucherCode, item.VoucherDate);
                        item.VoucherNumber = voucherNumber.VoucherNumber;
                    }
                    else
                    {
                        await _voucherNumberBusiness.UpdateVoucherNumberAsync(item.VoucherCode, item.VoucherNumber, item.VoucherDate);
                    }
                }
                else
                {
                    if ((item.VoucherNumber ?? "") == "")
                    {
                        var voucherNumber = await _voucherNumberBusiness.AutoBusinessVoucherNumberAsync(item.VoucherCode, item.BusinessCode, item.VoucherDate);
                        item.VoucherNumber = voucherNumber.VoucherNumber;
                    }
                    else
                    {
                        await _voucherNumberBusiness.UpdateBusinessVoucherNumberAsync(item.VoucherCode, item.BusinessCode, item.VoucherNumber, item.VoucherDate);
                    }
                }
                var productVoucherId = this.GetNewObjectId();
                item.Id = productVoucherId;
                item.OrgCode = _webHelper.GetCurrentOrgUnit();
                item.CreatorName = await _userService.GetCurrentUserNameAsync();
                item.Year = _webHelper.GetCurrentYear();
                item.VoucherCode = item.VoucherCode;
                item.VoucherGroup = voucherGroup.VoucherGroup;
                var partnerData = await _accPartnerService.GetAccPartnerByCodeAsync(item.PartnerCode0, orgCode);
                if (partnerData != null)
                {
                    item.PartnerName0 = partnerData.Name;
                    item.Address = partnerData.Address;
                }
                var productVoucherDetail = (from a in lstCrudProductVoucher
                                            where a.VoucherNumber == item.VoucherNumber
                                            && a.VoucherDate == item.VoucherDate
                                            && a.InvoiceSerial == item.InvoiceSerial
                                            && a.InvoiceNumber == item.InvoiceNumber
                                            && a.InvoiceDate == item.InvoiceDate
                                            && a.CurrencyCode == item.CurrencyCode
                                            && a.ExchangeRate == item.ExchangeRate
                                            && a.PartnerCode0 == item.PartnerCode0
                                            && a.TaxCode == item.TaxCode
                                            && a.Representative == item.Representative
                                            select new CrudProductVoucherDetailDto
                                            {
                                                Id = this.GetNewObjectId(),
                                                Year = _webHelper.GetCurrentYear(),
                                                OrgCode = _webHelper.GetCurrentOrgUnit(),
                                                Ord0 = "A" + (i).ToString().PadLeft(9, '0'),
                                                ProductVoucherId = productVoucherId,
                                                ProductCode = a.ProductCode,
                                                /*ProductName = productData.Name,*/
                                                WarehouseCode = a.WarehouseCode,
                                                TransWarehouseCode = a.TransWarehouseCode,
                                                ProductLotCode = a.ProductLotCode,
                                                ProductOriginCode = a.ProductOriginCode,
                                                Price = item.VoucherCode == "PBH"|| item.VoucherCode=="BH8" ? 0 : a.Price,
                                                PriceCur = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? 0 : a.PriceCur,
                                                Quantity = a.Quantity,
                                                Amount = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? 0 : a.Amount,
                                                AmountCur = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? 0 : a.AmountCur,
                                                Price2 = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? a.Price : 0,
                                                PriceCur2 = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? a.PriceCur : 0,
                                                Amount2 = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? a.Amount : 0,
                                                AmountCur2 = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? a.AmountCur : 0,
                                                DebitAcc2 = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? a.DebitAcc : null,
                                                CreditAcc2 = item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? a.CreditAcc : null,
                                                ContractCode = a.ContractCode,
                                                FProductWorkCode = a.FProductWorkCode,
                                                WorkPlaceCode = a.WorkPlaceCode,
                                                SectionCode = a.SectionCode,
                                                CaseCode = a.CaseCode,
                                                DiscountPercentage = a.DiscountPercentage,
                                                DiscountAmountCur = a.DiscountAmountCur,
                                                DiscountAmount = a.DiscountAmount,
                                                ExciseTaxPercentage = a.ExciseTaxPercentage,
                                                ExciseTaxAmountCur = a.ExciseTaxAmountCur,
                                                ExciseTaxAmount = a.ExciseTaxAmount,
                                                VatPercentage = a.VatPercentage,
                                                ImportTaxPercentage = a.ImportTaxPercentage,
                                                ImportTaxAmount = a.ImportTaxAmount,
                                                ImportTaxAmountCur = a.ImportTaxAmountCur,
                                                DecreasePercentage =a.DecreasePercentage,
                                                DecreaseAmount =a.DecreaseAmount,
                                                AmountAfterDecrease = a.AmountAfterDecrease,
                                                VatAmount = a.VatAmount
                                            }).ToList();

                List<CrudProductVoucherDetailDto> productDetail = new List<CrudProductVoucherDetailDto>();
                foreach (var detail in productVoucherDetail)
                {
                    if (item.CostAcount == null && item.ProductAcount == null)
                    {

                        var dataAcc = await _productService.GetByCodeAsync(detail.ProductCode, orgCode);
                        if (dataAcc == null)
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.Product, ErrorCode.NotFoundEntity),
                                _localizer["Err:ProductCodeNotExist", detail.ProductCode]);
                        }
                        detail.CreditAcc =item.VoucherCode== "PTL"|| item.VoucherCode=="PBH"|| item.VoucherCode == "BH8" ? dataAcc.ProductAcc: item.CreditAcc;
                        detail.DebitAcc = item.VoucherCode == "PTL" || item.VoucherCode == "PBH" || item.VoucherCode == "BH8" ? dataAcc.ProductCostAcc : item.DebitAcc;
                        detail.UnitCode = dataAcc.UnitCode;
                        detail.ProductName = dataAcc.Name;
                    }
                    productDetail.Add(detail);
                }
                item.ProductVoucherDetails = productDetail;

                var accTax = (from a in lstCrudProductVoucher
                              where a.AccTaxDetails != null
                              && a.VoucherNumber == item.VoucherNumber
                              && a.VoucherDate == item.VoucherDate
                              && a.InvoiceSerial == item.InvoiceSerial
                              && a.InvoiceNumber == item.InvoiceNumber
                              && a.InvoiceDate == item.InvoiceDate
                              && a.CurrencyCode == item.CurrencyCode
                              && a.ExchangeRate == item.ExchangeRate
                              && a.PartnerCode0 == item.PartnerCode0
                              && a.TaxCode == item.TaxCode
                              && a.Representative == item.Representative
                              group new { a } by new
                              {
                                  a.VoucherNumber,
                                  a.VoucherDate,
                                  a.VoucherCode,
                                  a.InvoiceSerial,
                                  a.InvoiceNumber,
                                  a.InvoiceDate,
                                  a.CurrencyCode,
                                  a.ExchangeRate,
                                  a.PartnerCode0,
                                  a.PartnerName0,
                                  a.TaxCode,
                                  a.Representative,
                                  a.Address,
                                  a.Description,
                                  a.VatPercentage,
                                  a.CreditAccAtax,
                                  a.DebitAccAtax
                              } into gr
                              select new CrudAccTaxDetailDto
                              {
                                  Id = this.GetNewObjectId(),
                                  Year = _webHelper.GetCurrentYear(),
                                  Ord0 = "Z" + (i).ToString().PadLeft(9, '0'),
                                  OrgCode = _webHelper.GetCurrentOrgUnit(),
                                  ProductVoucherId = productVoucherId,
                                  InvoiceNumber = gr.Key.InvoiceNumber,
                                  InvoiceSymbol = gr.Key.InvoiceSerial,
                                  Note = item.Note,
                                  NoteE = item.NoteE,
                                  CreditAcc = item.CreditAccAtax,
                                  DebitAcc = item.DebitAccAtax,
                                  VoucherDate = item.VoucherDate,
                                  InvoiceDate = item.VoucherDate,
                                  VoucherCode = item.VoucherCode,
                                  TaxCode = gr.Key.TaxCode,
                                  VatPercentage = item.VatPercentage,
                                  AmountWithoutVat = item.TotalAmountWithoutVat,
                                  AmountWithoutVatCur = item.TotalAmountWithoutVatCur,
                                  Amount = item.TotalVatAmount,
                                  AmountCur = item.TotalAmountWithoutVatCur,
                                  TotalAmount = item.TotalVatAmount + item.TotalAmountWithoutVat,
                                  TotalAmountCur = item.TotalVatAmountCur,
                                  TaxCategoryCode = item.TaxCategoryCode,
                                  PartnerCode = item.PartnerCode0,
                                  PartnerName = item.PartnerName0,
                                  Address = item.Address,

                              }).ToList();
                List<CrudAccTaxDetailDto> list = new List<CrudAccTaxDetailDto>();
                foreach (var accTaxs in accTax)
                {
                    if (accTaxs.TaxCategoryCode.IsNullOrEmpty() == false)
                    {
                        var dataAccTax = await _taxCategoryService.GetTaxByCodeAsync(accTaxs.TaxCategoryCode, _webHelper.GetCurrentOrgUnit());
                        if (dataAccTax == null)
                        {
                            throw new AccountingException(ErrorCode.Get(GroupErrorCodes.TaxCategory, ErrorCode.NotFoundEntity),
                                _localizer["Err:TaxCodeNotExist", accTaxs.TaxCategoryCode]);
                        }
                        accTaxs.Note = dataAccTax.Name;
                    }
                    list.Add(accTaxs);
                }
                item.AccTaxDetails = list;
            }

            foreach (var item in listProductvouchers)
            {
                await this.CreateAsync(item);
            }

            return new UploadFileResponseDto() { Ok = true };
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
        private async Task<IQueryable<ProductVoucher>> GetFilterAdvanced(IQueryable<ProductVoucher> queryable, PageRequestDto dto)
        {
            if (dto.FilterAdvanced == null) return queryable;
            string orgCode = _webHelper.GetCurrentOrgUnit();

            bool hasJoinPartner = false,
                 hasJoinProduct = false;

            var queryablePartner = await _accPartnerService.GetQueryableAsync();
            queryablePartner = queryablePartner.Where(p => p.OrgCode.Equals(orgCode));
            var queryableProduct = await _productService.GetQueryableAsync();
            queryableProduct = queryableProduct.Where(p => p.OrgCode.Equals(orgCode));
            var queryableProductDetail = await _productVoucherDetail.GetQueryableAsync();
            queryableProductDetail = queryableProductDetail.Where(p => p.OrgCode == orgCode);

            foreach (var item in dto.FilterAdvanced)
            {
                if (item.Value == null) continue;
                if (string.IsNullOrEmpty(item.Value.ToString())) continue;

                if (item.ColumnName.Equals("fromDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime fromDate = Convert.ToDateTime(item.Value.ToString());
                    queryable = queryable.Where(p => p.VoucherDate >= fromDate);
                }
                if (item.ColumnName.Equals("toDate", StringComparison.InvariantCultureIgnoreCase))
                {
                    DateTime toDate = Convert.ToDateTime(item.Value.ToString());
                    queryable = queryable.Where(p => p.VoucherDate <= toDate);
                }
                if (item.ColumnName.Equals("partnerGroupId"))
                {
                    hasJoinPartner = true;
                    queryablePartner = queryablePartner.Where(p => p.PartnerGroupId.Equals(item.Value.ToString())); ;
                }
                if (item.ColumnName.Equals("productGroupId"))
                {
                    hasJoinProduct = true;
                    queryableProduct = queryableProduct.Where(p => p.ProductGroupId.Equals(item.Value.ToString()));
                }

                if (item.ColumnName.Equals("beginVoucherNumber"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        foreach (var items in queryable.OrderBy(p => p.VoucherNumber))
                        {
                            items.VoucherNumber = GetVoucherNumber(items.VoucherNumber).ToString();

                        }
                        var para = GetVoucherNumber(item.Value.ToString());

                        queryable = queryable.Where(p => Convert.ToDecimal(p.VoucherNumber) >= para);


                    }

                }
                if (item.ColumnName.Equals("endVoucherNumber"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        foreach (var items in queryable.OrderBy(p => p.VoucherNumber))
                        {
                            items.VoucherNumber = GetVoucherNumber(items.VoucherNumber).ToString();

                        }
                        var para = GetVoucherNumber(item.Value.ToString());
                        queryable = queryable.Where(p => Convert.ToDecimal(p.VoucherNumber) <= para);
                    }
                }
                if (item.ColumnName.Equals("beginAmount"))
                {
                    if (decimal.Parse(item.Value.ToString()) != 0)
                    {
                        queryable = queryable.Where(p => p.TotalAmount >= decimal.Parse(item.Value.ToString()));
                    }
                }
                if (item.ColumnName.Equals("columnName"))
                {
                    if (decimal.Parse(item.Value.ToString()) != 0)
                    {
                        queryable = queryable.Where(p => p.TotalAmount <= decimal.Parse(item.Value.ToString()));
                    }
                }
                if (item.ColumnName.Equals("beginAmountCur"))
                {
                    if (decimal.Parse(item.Value.ToString()) != 0)
                    {
                        queryable = queryable.Where(p => p.TotalAmountCur <= decimal.Parse(item.Value.ToString()));
                    }
                }
                if (item.ColumnName.Equals("endAmountCur"))
                {
                    if (decimal.Parse(item.Value.ToString()) != 0)
                    {
                        queryable = queryable.Where(p => p.TotalAmountCur <= decimal.Parse(item.Value.ToString()));
                    }
                }

                if (item.ColumnName.Equals("partnerCode0"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where c.PartnerCode0 == item.Value.ToString()
                                    select c;
                    }

                }
                if (item.ColumnName.Equals("fProductWorkCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    join d in queryableProductDetail on c.Id equals d.ProductVoucherId
                                    where d.FProductWorkCode == item.Value.ToString()
                                    select c;
                    }

                }
                if (item.ColumnName.Equals("sectionCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    join d in queryableProductDetail on c.Id equals d.ProductVoucherId
                                    where d.SectionCode == item.Value.ToString()
                                    select c;
                    }
                }
                if (item.ColumnName.Equals("caseCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    join d in queryableProductDetail on c.Id equals d.ProductVoucherId
                                    where d.CaseCode == item.Value.ToString()
                                    select c;
                    }
                }
                if (item.ColumnName.Equals("currencyCode"))
                {
                    queryable = from c in queryable
                                where c.CurrencyCode == item.Value.ToString()
                                select c;

                }

                if (item.ColumnName.Equals("accCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        var result = (from a in queryableProductDetail
                                      where a.CreditAcc == item.Value.ToString() || a.DebitAcc == item.Value.ToString() || a.CreditAcc2 == item.Value.ToString() || a.DebitAcc2 == item.Value.ToString()
                                      select new ProductVoucherDetail
                                      {
                                          ProductVoucherId = a.ProductVoucherId
                                      }).Distinct();

                        queryable = from c in queryable
                                    join d in result on c.Id equals d.ProductVoucherId

                                    select c;

                    }

                }

                if (item.ColumnName.Equals("debitCredit"))
                {
                    if (item.Value.ToString() == "*")
                    {
                        queryable = from c in queryable
                                    select c;
                    }
                    if (item.Value.ToString() == "N")
                    {
                        var queryableProductDetails = (from a in queryableProductDetail
                                                       where a.DebitAcc == item.Value.ToString() || a.DebitAcc2 == item.Value.ToString()
                                                       select new ProductVoucherDetail
                                                       {
                                                           ProductVoucherId = a.ProductVoucherId
                                                       }).Distinct();
                        queryable = from c in queryable
                                    join d in queryableProductDetails on c.Id equals d.ProductVoucherId
                                    select c;
                    }
                    if (item.Value.ToString() == "C")
                    {
                        var queryableProductDetails = (from a in queryableProductDetail
                                                       where a.DebitAcc == item.Value.ToString() || a.DebitAcc2 == item.Value.ToString()
                                                       select new ProductVoucherDetail
                                                       {
                                                           ProductVoucherId = a.ProductVoucherId
                                                       }).Distinct();
                        queryable = from c in queryable
                                    join d in queryableProductDetails on c.Id equals d.ProductVoucherId
                                    select c;
                    }


                }
                if (item.ColumnName.Equals("reciprocalAcc"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        var result = (from a in queryableProductDetail
                                      where a.CreditAcc == item.Value.ToString() || a.CreditAcc2 == item.Value.ToString()
                                      select new ProductVoucherDetail
                                      {
                                          ProductVoucherId = a.ProductVoucherId
                                      }).Distinct();

                        queryable = from c in queryable
                                    join d in result on c.Id equals d.ProductVoucherId

                                    select c;

                    }
                }
                if (item.ColumnName.Equals("warehouseCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        var result = (from a in queryableProductDetail
                                      where a.WarehouseCode == item.Value.ToString()
                                      select new ProductVoucherDetail
                                      {
                                          ProductVoucherId = a.ProductVoucherId
                                      }).Distinct();

                        queryable = from c in queryable
                                    join d in result on c.Id equals d.ProductVoucherId

                                    select c;

                    }
                }
                if (item.ColumnName.Equals("productCode"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {

                        var result = (from a in queryableProductDetail
                                      where a.ProductCode == item.Value.ToString()
                                      select new ProductVoucherDetail
                                      {
                                          ProductVoucherId = a.ProductVoucherId
                                      }).Distinct();

                        queryable = from c in queryable
                                    join d in result on c.Id equals d.ProductVoucherId

                                    select c;

                    }
                }
                if (item.ColumnName.Equals("creationTime"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where c.CreationTime == DateTime.Parse(item.Value.ToString())
                                    select c;

                    }
                }

                if (item.ColumnName.Equals("status"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where c.Status == item.Value.ToString()
                                    select c;

                    }
                }

                if (item.ColumnName.Equals("beginInvoiceNumber"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where GetVoucherNumber(c.VoucherNumber) <= GetVoucherNumber(item.Value.ToString())
                                    select c;

                    }
                }
                if (item.ColumnName.Equals("endInvoiceNumber"))
                {
                    if (!string.IsNullOrEmpty(item.Value.ToString()))
                    {
                        queryable = from c in queryable
                                    where GetVoucherNumber(c.VoucherNumber) >= GetVoucherNumber(item.Value.ToString())
                                    select c;

                    }
                }
            }

            if (hasJoinPartner == true && hasJoinProduct == true)
            {
                return queryable;
            }
            else
            {
                if (hasJoinPartner)
                {
                    var test = queryable.ToList();
                    var queryablePartners = queryablePartner.ToList();
                    return from c in queryable
                           join d in queryablePartner on c.PartnerCode0 equals d.Code
                           select c;
                }
                if (hasJoinProduct)
                {
                    return from c in queryable
                           join d in queryableProductDetail on c.Id equals d.ProductVoucherId
                           join e in queryableProduct on d.ProductCode equals e.Code
                           select c;
                }
                return queryable;
            }
        }
        private async Task<IQueryable<ProductVoucher>> GetFilterRows(IQueryable<ProductVoucher> queryable, PageRequestDto dto)
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
        private async Task InsertRefVoucher(CrudProductVoucherDto dto)
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
        private async Task<CrudProductVoucherDto> IncrementVoucherNumber(CrudProductVoucherDto dto)
        {
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
            return dto;
        }
        private async Task SavingLedger(VoucherCategoryDto voucherCategory, CrudProductVoucherDto dto)
        {
            if (voucherCategory.IsSavingLedger != "C") return;

            List<CrudVoucherExciseTaxDto> VoucherExciseTax = await _ledgerService.VoucherExcitaxAsync(dto);
            if (VoucherExciseTax != null && !VoucherExciseTax.Any())
            {
                foreach (var item in VoucherExciseTax)
                {
                    item.Id = this.GetNewObjectId();

                    var excixeTax = ObjectMapper.Map<CrudVoucherExciseTaxDto, VoucherExciseTax>(item);
                    await _voucherExciseTaxService.CreateAsync(excixeTax);
                }
            }

            var crudVoucherPaymentBookDto = await _ledgerService.CreateVoucherPaymentAsync(dto);
            foreach (var crudVoucherPaymentBook in crudVoucherPaymentBookDto)
            {

                crudVoucherPaymentBook.Id = this.GetNewObjectId();
                crudVoucherPaymentBook.DocumentId = dto.Id;
                crudVoucherPaymentBook.OrgCode = dto.OrgCode;
                crudVoucherPaymentBook.PartnerCode = dto.PartnerCode0;
                var crudVoucherPayment = ObjectMapper.Map<CrudVoucherPaymentBookDto, VoucherPaymentBook>(crudVoucherPaymentBook);
                await _voucherPaymentBookService.CreateAsync(crudVoucherPayment);
            }


            if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
            {
                var ledgers = await _ledgerService.CreateLedgerLrAsync(dto);
                foreach (var ledger in ledgers)
                {
                    ledger.Id = this.GetNewObjectId();
                    ledger.CreationTime = DateTime.Now;
                    ledger.CreatorName = await _userService.GetCurrentUserNameAsync();
                    var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                    await _ledgerService.CreateAsync(ledgerEntity);
                }
            }
            else
            {
                var ledgers = await _ledgerService.CreateLedgerAsync(dto);
                foreach (var ledger in ledgers)
                {
                    ledger.Id = this.GetNewObjectId();
                    ledger.CreationTime = DateTime.Now;
                    ledger.CreatorName = await _userService.GetCurrentUserNameAsync();
                    var ledgerEntity = ObjectMapper.Map<CrudLedgerDto, Ledger>(ledger);
                    await _ledgerService.CreateAsync(ledgerEntity);
                }
            }
        }
        private async Task SavingWarehouseBook(VoucherCategoryDto voucherCategory, CrudProductVoucherDto dto)
        {
            if (voucherCategory.IsSavingWarehouseBook.ToString() != "C") return;
            var error = await _warehouseBookService.CheckSoundOutput(dto);

            if (error.Message != null)
            {
                throw new Exception(error.Message);
            }
            if (dto.VoucherCode == "PLR" || dto.VoucherCode == "PTR")
            {
                var warehouseBook = await _warehouseBookService.CreatewarehouseBookLrAsync(dto);
                foreach (var warehouseBooks in warehouseBook)
                {
                    warehouseBooks.Id = this.GetNewObjectId();
                    var wareBookEntity = ObjectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(warehouseBooks);
                    await _warehouseBookService.CreateAsync(wareBookEntity);
                }
            }
            else
            {
                var warehouseBook = await _warehouseBookService.CreatewarehouseBookAsync(dto);
                foreach (var warehouseBooks in warehouseBook)
                {
                    warehouseBooks.Id = this.GetNewObjectId();
                    var wareBookEntity = ObjectMapper.Map<CrudWarehouseBookDto, WarehouseBook>(warehouseBooks);
                    await _warehouseBookService.CreateAsync(wareBookEntity);
                }
            }
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
