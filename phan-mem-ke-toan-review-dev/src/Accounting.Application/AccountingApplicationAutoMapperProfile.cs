using Accounting.BaseDtos.Customines;
using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.CategoryDeletes;
using Accounting.Categories.Contracts;
using Accounting.Categories.CostProductions;
using Accounting.Categories.Menus;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Others.InvoiceBooks;
using Accounting.Categories.Others.PaymentTerms;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Categories.Salaries;
using Accounting.Catgories.AccCases;
using Accounting.Catgories.Accounts;
using Accounting.Catgories.Accounts.AccOpeningBalances;
using Accounting.Catgories.AdjustDepreciations;
using Accounting.Catgories.AssetTools;
using Accounting.Catgories.Capitals;
using Accounting.Catgories.CategoryDatas;
using Accounting.Catgories.Contracts;
using Accounting.Catgories.CostProductions;
using Accounting.Catgories.CostProductions.AllotmentForwardCategories;
using Accounting.Catgories.CostProductions.SoTHZs;
using Accounting.Catgories.FProductWorkNorms;
using Accounting.Catgories.FProductWorks;
using Accounting.Catgories.InventoryRecords;
using Accounting.Catgories.Menus;
using Accounting.Catgories.OrgUnits;
using Accounting.Catgories.Others.BusinessCategories;
using Accounting.Catgories.Others.Careers;
using Accounting.Catgories.Others.Circularses;
using Accounting.Catgories.Others.Currencies;
using Accounting.Catgories.Others.Departments;
using Accounting.Catgories.Others.ExciseTaxes;
using Accounting.Catgories.Others.FeeTypes;
using Accounting.Catgories.Others.Invoices;
using Accounting.Catgories.Others.Other;
using Accounting.Catgories.Others.PaymentTerms;
using Accounting.Catgories.Others.SaleChannels;
using Accounting.Catgories.Others.TaxCategories;
using Accounting.Catgories.Others.TenantSettings;
using Accounting.Catgories.Others.Warehouses;
using Accounting.Catgories.Partners;
using Accounting.Catgories.ProductOpeningBalances;
using Accounting.Catgories.ProductOthers;
using Accounting.Catgories.Products;
using Accounting.Catgories.ProductVouchers;
using Accounting.Catgories.Purposes;
using Accounting.Catgories.Reasons;
using Accounting.Catgories.Salaries.Employees;
using Accounting.Catgories.Salaries.Positions;
using Accounting.Catgories.Salaries.SalaryBooks;
using Accounting.Catgories.Salaries.SalaryCategories;
using Accounting.Catgories.Salaries.SalaryPeriods;
using Accounting.Catgories.Salaries.SalaryTypes;
using Accounting.Catgories.Sections;
using Accounting.Catgories.Units;
using Accounting.Catgories.VoucherCategories;
using Accounting.Catgories.VoucherTypes;
using Accounting.Catgories.WorkPlace;
using Accounting.Catgories.YearCategories;
using Accounting.Configs;
using Accounting.Excels;
using Accounting.Generals;
using Accounting.Generals.PriceStockOuts;
using Accounting.Invoices;
using Accounting.Invoices.Dtos;
using Accounting.Invoices.InvoiceAuths;
using Accounting.Invoices.InvoiceSuppliers;
using Accounting.Licenses;
using Accounting.Others;
using Accounting.Permissions;
using Accounting.Reports;
using Accounting.Reports.Cores;
using Accounting.Reports.DebitBooks;
using Accounting.Reports.Financials;
using Accounting.Reports.Financials.Tenant;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.Reports.Statements.T200.Tenants;
using Accounting.Reports.Tenants;
using Accounting.Reports.Tenants.TenantStatementTaxs;
using Accounting.Tenants;
using Accounting.Users;
using Accounting.Vouchers;
using Accounting.Vouchers.AccVouchers;
using Accounting.Vouchers.Ledgers;
using Accounting.Vouchers.Printings;
using Accounting.Vouchers.RecordingVoucherBooks;
using Accounting.Vouchers.RefVouchers;
using Accounting.Vouchers.VoucherExciseTaxs;
using Accounting.Vouchers.VoucherNumbers;
using Accounting.Vouchers.VoucherPaymentBeginnings;
using Accounting.Vouchers.WarehouseBooks;
using Accounting.Windows;
using AutoMapper;
using AutoMapper.Internal.Mappers;
using MongoDB.Bson;
using Volo.Abp.Identity;


namespace Accounting;

public class AccountingApplicationAutoMapperProfile : Profile
{
    public AccountingApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */
        CreateMap<AccountSystem, AccountSystemDto>();
        CreateMap<CruAccountSystemDto, AccountSystem>();
        CreateMap<AccountSystem, CruAccountSystemDto>();
        CreateMap<DefaultAccountSystem, AccountSystem>();
        CreateMap<CrudDefaultAccountSystemDto, AccountSystem>();
        CreateMap<DefaultAccountSystem, DefaultAccountSystemDto>();
        CreateMap<DefaultAccountSystemDto, CrudDefaultAccountSystemDto>();

        CreateMap<AccSection, AccSectionDto>();
        CreateMap<CruAccSectionDto, AccSection>();

        CreateMap<AccCase, AccCaseDto>();
        CreateMap<CrudAccCaseDto, AccCase>();

        CreateMap<AccOpeningBalance, AccOpeningBalanceDto>();
        CreateMap<CrudAccOpeningBalanceDto, AccOpeningBalance>();

        CreateMap<Career, CareerDto>();
        CreateMap<CrudCareerDto, Career>();

        CreateMap<FProductWork, FProductWorkDto>();
        CreateMap<CrudFProductWorkDto, FProductWork>();
        CreateMap<CrudFProductWorkNormDto, FProductWorkNormDto>();


        CreateMap<VoucherCategory, VoucherCategoryDto>();
        CreateMap<CruVoucherCategoryDto, VoucherCategory>();
        CreateMap<DefaultVoucherCategory, DefaultVoucherCategoryDto>();
        CreateMap<DefaultVoucherCategoryDto, CruVoucherCategoryDto>();
        CreateMap<DefaultVoucherCategory, VoucherCategoryDto>();
        CreateMap<DefaultVoucherCategory, VoucherCategory>();

        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<CrudWarehouseDto, Warehouse>();

        CreateMap<WorkPlace, WorkPlaceDto>();
        CreateMap<CrudWokPlaceDto, WorkPlace>();

        CreateMap<YearCategory, YearCategoryDto>();
        CreateMap<CruYearCategoryDto, YearCategory>();

        CreateMap<CrudOrgUnitPermissionDto, OrgUnitPermission>();
        CreateMap<OrgUnit, OrgUnitDto>();
        CreateMap<CrudOrgUnitDto, OrgUnit>();

        CreateMap<MenuAccounting, MenuAccountingDto>();
        CreateMap<CruMenuAccountingDto, MenuAccounting>();

        CreateMap<BankPartner, BankPartnerDto>();
        CreateMap<CrudBankPartnerDto, BankPartner>();

        CreateMap<ProductOrigin, ProductOriginDto>();
        CreateMap<CrudProductOriginDto, ProductOrigin>();


        CreateMap<ProductOpeningBalance, ProductOpeningBalanceDto>();
        CreateMap<CrudProductOpeningBalanceDto, ProductOpeningBalance>();

        CreateMap<AccPartner, AccPartnerDto>();
        CreateMap<CrudAccPartnerDto, AccPartner>().BeforeMap((source, dest) =>
        {
            if (source.BankPartners == null) return;
            int ord = 1;
            foreach (var item in source.BankPartners)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.PartnerId = source.Id;
                item.PartnerCode = source.Code;
                item.OrgCode = source.OrgCode;
                item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                ord++;
            }
        });

        CreateMap<WindowDto, CrudWindowDto>();
        CreateMap<Window, WindowDto>();
        CreateMap<CrudWindowDto, Window>();

        CreateMap<Tab, TabDto>();
        CreateMap<CrudTabDto, Tab>();

        CreateMap<Field, FieldDto>();
        CreateMap<CrudFieldDto, Field>();

        CreateMap<TenantAccBalanceSheet, AccBalanceSheet>();
        CreateMap<DefaultAccBalanceSheet, AccBalanceSheet>();
        CreateMap<AccBalanceSheet, AccBalanceSheetDto>();

        CreateMap<TenantCashFollowStatement, CashFollowStatement>();
        CreateMap<DefaultCashFollowStatement, CashFollowStatement>();
        CreateMap<CashFollowStatement, CashFlowDto>();

        CreateMap<CrudAccBalanceSheetDto, TenantAccBalanceSheet>();
        CreateMap<CrudTenantBusinessResultDto, TenantBusinessResult>();

        CreateMap<CrudSoTHZRpDto, SoTHZ>();
        CreateMap<CrudSoTHZDto, SoTHZ>();
        CreateMap<SoTHZ, SoTHZDto>();

        CreateMap<TenantBusinessResult, BusinessResult>();
        CreateMap<DefaultBusinessResult, BusinessResult>();
        CreateMap<BusinessResult, BusinessResultDto>();

        CreateMap<ProductPrice, ProductPriceDto>();
        CreateMap<CrudProductPriceDto, ProductPrice>();

        CreateMap<ProductUnit, ProductUnitDto>();
        CreateMap<CrudProductUnitDto, ProductUnit>();


        CreateMap<ProductVoucherVat, ProductVoucherVatDto>();
        CreateMap<ProductVoucherVat, CrudProductVoucherVatDto>();
        CreateMap<CrudProductVoucherVatDto, ProductVoucherVat>();

        CreateMap<ProductVoucherAssembly, ProductVoucherAssemblyDto>();
        CreateMap<ProductVoucherAssembly, CrudProductVoucherAssemblyDto>();
        CreateMap<CrudProductVoucherAssemblyDto, ProductVoucherAssembly>();

        CreateMap<ProductVoucherDetail, ProductVoucherDetailDto>();
        CreateMap<ProductVoucherDetail, CrudProductVoucherDetailDto>();
        CreateMap<CrudProductVoucherDetailDto, ProductVoucherDetail>();

        CreateMap<ProductVoucherDetailReceipt, ProductVoucherDetailReceiptDto>();

        CreateMap<ProductVoucherDetailReceipt, ProductVoucherDetailReceiptDto>();
        CreateMap<ProductVoucherDetailReceipt, CrudProductVoucherDetailReceiptDto>();
        CreateMap<CrudProductVoucherDetailReceiptDto, ProductVoucherDetailReceipt>();

        CreateMap<ProductVoucherReceipt, ProductVoucherReceiptDto>();
        CreateMap<ProductVoucherReceipt, CrudProductVoucherReceiptDto>();
        CreateMap<CrudProductVoucherReceiptDto, ProductVoucherReceipt>();

        CreateMap<ProductVoucherCost, ProductVoucherCostDto>();
        CreateMap<ProductVoucherCost, CrudProductVoucherCostDto>();
        CreateMap<CrudProductVoucherCostDto, ProductVoucherCost>();

        CreateMap<AccTaxDetail, AccTaxDetailDto>();
        CreateMap<AccTaxDetail, CrudAccTaxDetailDto>();
        CreateMap<CrudAccTaxDetailDto, AccTaxDetail>();

        CreateMap<VoucherExciseTax, CrudVoucherExciseTaxDto>();
        CreateMap<CrudVoucherExciseTaxDto, VoucherExciseTax>();
        CreateMap<Ledger, LedgerDto>();
        CreateMap<CrudLedgerDto, Ledger>();

        CreateMap<VoucherNumber, VoucherNumberDto>();
        CreateMap<CrudVoucherNumberDto, VoucherNumber>();

        CreateMap<WarehouseBook, WarehouseBookDto>();
        CreateMap<CrudWarehouseBookDto, WarehouseBook>();
        CreateMap<WarehouseBookDto, WarehouseBook>();



        CreateMap<TaxCategory, TaxCategoryDto>();
        CreateMap<CrudTaxCategoryDto, TaxCategory>();

        CreateMap<DefaultBusinessCategory, DefaultBusinessCategoryDto>();
        CreateMap<DefaultBusinessCategoryDto, BusinessCategory>();
        CreateMap<DefaultBusinessCategory, BusinessCategory>();
        CreateMap<BusinessCategory, BusinessCategoryDto>();
        CreateMap<CrudBusinessCategoryDto, BusinessCategory>();


        CreateMap<TenantSetting, TenantSettingDto>();
        CreateMap<CrudTenantSettingDto, TenantSetting>();
        CreateMap<DefaultTenantSetting, DefaultTenantSettingDto>();
        CreateMap<DefaultTenantSettingDto, CrudTenantSettingDto>();
        CreateMap<DefaultTenantSetting, TenantSetting>();

        CreateMap<ProductOpeningBalance, ProductOpeningBalanceDto>();
        CreateMap<CrudProductOpeningBalanceDto, ProductOpeningBalance>();

        CreateMap<ProductVoucher, ProductVoucherDto>();
        CreateMap<ProductVoucher, CrudProductVoucherDto>();
        CreateMap<AccVoucher, CrudAccVoucherDto>();
        CreateMap<CrudProductVoucherDto, ProductVoucher>().BeforeMap((source, dest) =>
        {
            if (source.ProductVoucherVats != null)
            {
                for (int i = 0; i < source.ProductVoucherVats.Count; i++)
                {
                    var ProductVoucherVats = source.ProductVoucherVats[i];
                    ProductVoucherVats.Id = ObjectId.GenerateNewId().ToString();
                    ProductVoucherVats.ProductVoucherId = source.Id;
                    ProductVoucherVats.OrgCode = source.OrgCode;
                }
            }
            if (source.ProductVoucherAssemblies != null)
            {
                for (int i = 0; i < source.ProductVoucherAssemblies.Count; i++)
                {
                    var ProductVoucherVats = source.ProductVoucherAssemblies[i];
                    ProductVoucherVats.Id = ObjectId.GenerateNewId().ToString();
                    ProductVoucherVats.ProductVoucherId = source.Id;
                    ProductVoucherVats.OrgCode = source.OrgCode;

                }
            }

            if (source.ProductVoucherDetails != null)
            {
                for (int i = 0; i < source.ProductVoucherDetails.Count; i++)
                {
                    var ProductVoucherDetails = source.ProductVoucherDetails[i];
                    ProductVoucherDetails.Id = ObjectId.GenerateNewId().ToString();
                    ProductVoucherDetails.ProductVoucherId = source.Id;
                    ProductVoucherDetails.OrgCode = source.OrgCode;


                    if (source.ProductVoucherDetails[i].ProductVoucherDetailReceipts == null) return;
                    foreach (var item in source.ProductVoucherDetails[i].ProductVoucherDetailReceipts)
                    {
                        item.Id = ObjectId.GenerateNewId().ToString();
                        item.ProductVoucherDetailId = source.ProductVoucherDetails[i].Id;
                        item.OrgCode = source.OrgCode;
                    }
                }
            }
            if (source.ProductVoucherReceipts != null)
            {
                foreach (var item in source.ProductVoucherReceipts)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.ProductVoucherId = source.Id;
                    item.OrgCode = source.OrgCode;
                }
            }
            if (source.ProductVoucherCostDetails != null)
            {
                foreach (var item in source.ProductVoucherCostDetails)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.ProductVoucherId = source.Id;
                    item.OrgCode = source.OrgCode;
                }
            }
            if (source.AccTaxDetails != null)
            {
                for (int i = 0; i < source.AccTaxDetails.Count; i++)
                {
                    var AccTaxDetails = source.AccTaxDetails[i];
                    AccTaxDetails.Id = ObjectId.GenerateNewId().ToString();
                    AccTaxDetails.ProductVoucherId = source.Id;
                    AccTaxDetails.OrgCode = source.OrgCode;
                }
            }
        });

        CreateMap<ProductVoucher, CrudProductVoucherDto>();
        CreateMap<ProductVoucherDetail, CrudProductVoucherDetailDto>();

        CreateMap<Product, ProductDto>();
        CreateMap<CrudProductDto, Product>().BeforeMap((source, dest) =>
        {
            if (source.ProductPrices != null)
            {
                int ordProductPrice = 1;
                foreach (var item in source.ProductPrices)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.ProductId = source.Id;
                    item.ProductCode = source.Code;
                    item.OrgCode = source.OrgCode;
                    item.Ord0 = "A" + ordProductPrice.ToString().PadLeft(9, '0');
                    ordProductPrice++;
                }
            }


            if (source.ProductUnits != null)
            {
                int ordProductUnit = 1;
                foreach (var item in source.ProductUnits)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.ProductId = source.Id;
                    item.ProductCode = source.Code;
                    item.OrgCode = source.OrgCode;
                    item.Ord0 = "A" + ordProductUnit.ToString().PadLeft(9, '0');
                    ordProductUnit++;
                }
            }
        });

        CreateMap<ContractDetail, ContractDetailDto>();
        CreateMap<CrudContractDetailDto, ContractDetail>();

        CreateMap<Contract, ContractDto>();
        CreateMap<CrudContractDto, Contract>().BeforeMap((source, dest) =>
        {
            if (source.ContractDetails == null) return;
            int ord = 1;
            foreach (var item in source.ContractDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.ContractId = source.Id;
                item.OrgCode = source.OrgCode;
                item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                ord++;
            }
        });

        CreateMap<InvoiceAuthDetail, InvoiceAuthDetailDto>();
        CreateMap<CrudInvoiceAuthDetailDto, InvoiceAuthDetail>();

        CreateMap<InvoiceAuth, InvoiceAuthDto>();
        CreateMap<CrudInvoiceAuthDto, InvoiceAuth>().BeforeMap((source, dest) =>
        {
            if (source.InvoiceAuthDetails == null) return;
            int ord = 1;
            foreach (var item in source.InvoiceAuthDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.InvoiceAuthId = source.Id;
                item.OrgCode = source.OrgCode;
                item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                ord++;
            }
        });

        CreateMap<InvoiceSupplier, InvoiceSupplierDto>();
        CreateMap<CrudInvoiceSupplierDto, InvoiceSupplier>();

        CreateMap<AccVoucherDetail, AccVoucherDetailDto>();
        CreateMap<CrudAccVoucherDetailDto, AccVoucherDetail>();

        CreateMap<AccTaxDetail, AccTaxDetailDto>();
        CreateMap<CrudAccTaxDetailDto, AccTaxDetail>();



        CreateMap<AccVoucher, AccVoucherDto>();
        CreateMap<CrudAccVoucherDto, AccVoucher>();

        CreateMap<ReferenceDetail, ReferenceDetailDto>();
        CreateMap<CrudReferenceDetailDto, ReferenceDetail>();

        CreateMap<Reference, ReferenceDto>();
        CreateMap<CrudReferenceDto, Reference>().BeforeMap((source, dest) =>
        {
            if (source.ReferenceDetails == null) return;
            foreach (var item in source.ReferenceDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.ReferenceId = source.Id;
            }
        });

        CreateMap<PartnerGroup, PartnerGroupDto>();
        CreateMap<CrudPartnerGroupDto, PartnerGroup>();
        CreateMap<PartnerGroup, PartnerGroupCustomineDto>();

        CreateMap<TenantSetting, TenantSettingDto>();
        CreateMap<CrudTenantSettingDto, TenantSetting>();

        CreateMap<IdentityUser, UserDto>();
        CreateMap<CrudUserDto, IdentityUser>();

        CreateMap<IdentityRole, UserRoleDto>();
        CreateMap<CrudUserRoleDto, IdentityRole>();

        CreateMap<BusinessCategory, BusinessCategoryDto>();
        CreateMap<CrudBusinessCategoryDto, BusinessCategory>();

        CreateMap<DefaultTaxCategory, DefaultTaxCategoryDto>();
        CreateMap<DefaultTaxCategoryDto, TaxCategory>();
        CreateMap<TaxCategory, TaxCategoryDto>();
        CreateMap<CrudTaxCategoryDto, TaxCategory>();

        CreateMap<Department, DepartmentDto>();
        CreateMap<CrudDepartmentDto, Department>();

        CreateMap<Currency, CurrencyDto>();
        CreateMap<CrudCurrencyDto, Currency>();
        CreateMap<DefaultCurrency, DefaultCurrencyDto>();
        CreateMap<DefaultCurrencyDto, CrudCurrencyDto>();

        CreateMap<SelectOrgUnitDto, OrgUnitPermission>();

        CreateMap<VoucherTemplate, VoucherTemplateDto>();

        CreateMap<Department, DepartmentDto>();
        CreateMap<CrudDepartmentDto, Department>();

        CreateMap<SaleChannel, SaleChannelDto>();
        CreateMap<CrudSaleChannelDto, SaleChannel>();

        CreateMap<ProductGroup, ProductGroupDto>();
        CreateMap<CrudProductGroupDto, ProductGroup>();
        CreateMap<ProductGroup, ProductGroupCustomineDto>();

        CreateMap<DefaultExciseTax, DefaultExciseTaxDto>();
        CreateMap<DefaultExciseTaxDto, ExciseTax>();
        CreateMap<ExciseTax, ExciseTaxDto>();
        CreateMap<CrudExciseTaxDto, ExciseTax>();

        CreateMap<ProductLot, ProductLotDto>();
        CreateMap<CrudProductLotDto, ProductLot>();

        CreateMap<DiscountPricePartner, DiscountPricePartnerDto>();
        CreateMap<CrudDiscountPricePartnerDto, DiscountPricePartner>();
        CreateMap<DiscountPriceDetail, DiscountPriceDetailDto>();
        CreateMap<CrudDiscountPriceDetailDto, DiscountPriceDetail>();
        CreateMap<DiscountPrice, DiscountPriceDto>();
        CreateMap<CrudDiscountPriceDto, DiscountPrice>().BeforeMap((source, dest) =>
        {
            if (source.DiscountPricePartners != null)
            {
                foreach (var item in source.DiscountPricePartners)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.DiscountPriceId = source.Id;
                    item.OrgCode = source.OrgCode;
                }
            }


            if (source.DiscountPriceDetails != null)
            {
                foreach (var item in source.DiscountPriceDetails)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.DiscountPriceId = source.Id;
                    item.OrgCode = source.OrgCode;
                }
            }
        });

        CreateMap<ProductionPeriod, ProductionPeriodDto>();
        CreateMap<CrudProductionPeriodDto, ProductionPeriod>();

        CreateMap<Unit, UnitDto>();
        CreateMap<CrudUnitDto, Unit>();

        CreateMap<InvoiceBook, InvoiceBookDto>();
        CreateMap<CrudInvoiceBookDto, InvoiceBook>();

        CreateMap<InfoWindowDetail, InfoWindowDetailDto>();
        CreateMap<InfoWindow, InfoWindowDto>();

        CreateMap<AccVoucher, PrintingAccVoucherDto>();

        CreateMap<CrudPaymentTermDetailDto, PaymentTermDetail>();
        CreateMap<PaymentTermDetail, PaymentTermDetailDto>();
        CreateMap<CrudPaymentTermDto, PaymentTerm>().BeforeMap((source, dest) =>
        {
            if (source.PaymentTermDetails == null) return;
            foreach (var item in source.PaymentTermDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.PaymentTermId = source.Id;
                item.OrgCode = source.OrgCode;
            }
        });
        CreateMap<PaymentTerm, PaymentTermDto>();

        CreateMap<InfoCalcPriceStockOut, InfoCalcPriceStockOutDto>();
        CreateMap<InfoCalcPriceStockOut, CrudInfoCalcPriceStockOutDto>();

        CreateMap<InfoCalcPriceStockOutDetail, CrudInfoCalcPriceStockOutDetailDto>();

        CreateMap<CrudInfoCalcPriceStockOutDto, InfoCalcPriceStockOut>().BeforeMap((source, dest) =>
        {
            if (source.InfoCalcPriceStockOutDetails == null) return;
            foreach (var item in source.InfoCalcPriceStockOutDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.InfoCalcPriceStockOutId = source.Id;
                item.OrgCode = source.OrgCode;
            }
        });
        CreateMap<CrudInfoCalcPriceStockOutDetailDto, InfoCalcPriceStockOutDetail>();

        CreateMap<VoucherPaymentBook, VoucherPaymentBookDto>();
        CreateMap<CrudVoucherPaymentBookDto, VoucherPaymentBook>();

        CreateMap<VoucherPaymentBeginning, VoucherPaymentBeginningDto>();
        CreateMap<CrudVoucherPaymentBeginningDto, VoucherPaymentBeginning>().BeforeMap((source, dest) =>
        {
            if (source.VoucherPaymentBeginningDetails == null) return;
            foreach (var item in source.VoucherPaymentBeginningDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.VoucherPaymentBeginningId = source.Id;
                item.OrgCode = source.OrgCode;
            }
        });

        CreateMap<VoucherPaymentBeginningDetail, VoucherPaymentBeginningDetailDto>();
        CreateMap<CrudVoucherPaymentBeginningDetailDto, VoucherPaymentBeginningDetail>();

        CreateMap<Button, ButtonDto>();

        CreateMap<DefaultAssetToolGroup, DefaultAssetToolGroupDto>();
        CreateMap<DefaultAssetToolGroupDto, AssetToolGroup>();
        CreateMap<AssetToolGroup, AssetToolGroupDto>();
        CreateMap<CrudAssetToolGroupDto, AssetToolGroup>();
        CreateMap<AssetToolGroup, AssetToolGroupCustomineDto>();

        CreateMap<AssetTool, AssetToolDto>();
        CreateMap<CrudAssetToolDto, AssetTool>().BeforeMap((source, dest) =>
        {
            var ord = 1;
            if (source.AssetToolDetails != null)
            {
                foreach (var item in source.AssetToolDetails)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.AssetToolId = source.Id;
                    item.OrgCode = source.OrgCode;
                    item.AssetOrTool = source.AssetOrTool;
                    item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                    item.Year = source.Year;
                    ord++;
                }
            };
            if (source.AssetToolStoppingDepreciations != null)
            {
                ord = 1;
                foreach (var item in source.AssetToolStoppingDepreciations)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.AssetToolId = source.Id;
                    item.OrgCode = source.OrgCode;
                    item.AssetOrTool = source.AssetOrTool;
                    item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                    ord++;
                }
            }
            if (source.AssetToolAccessories != null)
            {
                ord = 1;
                foreach (var item in source.AssetToolAccessories)
                {
                    item.Id = ObjectId.GenerateNewId().ToString();
                    item.AssetToolId = source.Id;
                    item.OrgCode = source.OrgCode;
                    item.AssetOrTool = source.AssetOrTool;
                    item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                    item.Year = source.Year;
                    ord++;
                }
            }
        });
        CreateMap<AssetToolDetail, AssetToolDetailDto>();
        CreateMap<CrudAssetToolDetailDto, AssetToolDetail>();
        CreateMap<AssetToolAccessory, AssetToolAccessoryDto>();
        CreateMap<CrudAssetToolAccessoryDto, AssetToolAccessory>();
        CreateMap<AssetToolStoppingDepreciationDto, AssetToolStoppingDepreciation>();
        CreateMap<CrudAssetToolStoppingDepreciationDto, AssetToolStoppingDepreciation>();
        CreateMap<AssetToolAccount, AssetToolAccountDto>();
        CreateMap<CrudAssetToolAccountDto, AssetToolAccount>();
        CreateMap<AssetToolDepreciation, AssetToolDepreciationDto>();
        CreateMap<CrudAssetToolDepreciationDto, AssetToolDepreciation>();

        CreateMap<AdjustDepreciation, AdjustDepreciationDto>();
        CreateMap<CrudAdjustDepreciationDto, AdjustDepreciation>().BeforeMap((source, dest) =>
        {
            if (source.AdjustDepreciationDetails == null) return;
            foreach (var item in source.AdjustDepreciationDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.AdjustDepreciationId = source.Id;
                item.OrgCode = source.OrgCode;
            }
        });

        CreateMap<AdjustDepreciationDetail, AdjustDepreciationDetailDto>();
        CreateMap<CrudAdjustDepreciationDetailDto, AdjustDepreciationDetail>();

        CreateMap<Purpose, PurposeDto>();
        CreateMap<CrudPurposeDto, Purpose>();

        CreateMap<Reason, ReasonDto>();
        CreateMap<CrudReasonDto, Reason>();
        CreateMap<DefaultReason, DefaultReasonDto>();
        CreateMap<DefaultReason, ReasonDto>();

        CreateMap<Capital, CapitalDto>();
        CreateMap<CrudCapitalDto, Capital>();
        CreateMap<DefaultCapital, DefaultCapitalDto>();
        CreateMap<DefaultCapital, CapitalDto>();

        CreateMap<AssetToolAccount, AssetToolAccountDto>();
        CreateMap<CrudAssetToolAccountDto, AssetToolAccount>();

        CreateMap<AssetToolDepreciation, AssetToolDepreciationDto>();
        CreateMap<CrudAssetToolDepreciationDto, AssetToolDepreciation>();

        CreateMap<Circulars, CircularsDto>();

        CreateMap<AllotmentForwardCategory, AllotmentForwardCategoryDto>();
        CreateMap<CrudAllotmentForwardCategoryDto, AllotmentForwardCategory>();
        CreateMap<DefaultAllotmentForwardCategory, CrudAllotmentForwardCategoryDto>();
        CreateMap<DefaultAllotmentForwardCategory, AllotmentForwardCategory>();

        CreateMap<ConfigCostPrice, ConfigCostPriceDto>();
        CreateMap<CrudConfigCostPriceDto, ConfigCostPrice>();

        CreateMap<GroupCoefficient, GroupCoefficientDto>();
        CreateMap<CrudGroupCoefficientDto, GroupCoefficient>().BeforeMap((source, dest) =>
        {
            if (source.GroupCoefficientDetails == null) return;
            foreach (var item in source.GroupCoefficientDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.GroupCoefficientId = source.Id;
                item.GroupCoefficientCode = source.Code;
                item.OrgCode = source.OrgCode;
                item.FProductWork = source.FProductWork;
            }
        });

        CreateMap<GroupCoefficientDetail, GroupCoefficientDetailDto>();
        CreateMap<CrudGroupCoefficientDetailDto, GroupCoefficientDetail>();

        CreateMap<InfoZ, InfoZDto>();
        CreateMap<InfoZ, InfoZ>();
        CreateMap<CrudInfoZDto, InfoZ>();

        CreateMap<FProductWorkNorm, FProductWorkNormDto>();
        CreateMap<CrudFProductWorkNormDto, FProductWorkNorm>().BeforeMap((source, dest) =>
        {
            if (source.FProductWorkNormDetails == null) return;
            foreach (var item in source.FProductWorkNormDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.FProductWorkNormId = source.Id;
                item.OrgCode = source.OrgCode;
                item.Year = source.Year;
            }
        });

        CreateMap<FProductWorkNormDetail, FProductWorkNormDetailDto>();
        CreateMap<CrudFProductWorkNormDetailDto, FProductWorkNormDetail>();

        CreateMap<InfoCalcPriceStockOutDetail, InfoCalcPriceStockOutDetailDto>();

        CreateMap<ProductVoucher, PrintingProductVoucherDto>();

        CreateMap<ReportTemplateColumn, ReportTemplateColumnDto>();
        CreateMap<ReportTemplate, ReportTemplateDto>();

        CreateMap<InfoExportAuto, InfoExportAutoDto>();
        CreateMap<CrudInfoExportAutoDto, InfoExportAuto>();

        CreateMap<ImportExcelTemplateColumn, ImportExcelTemplateColumnDto>();
        CreateMap<ImportExcelTemplate, ImportExcelTemplateDto>();

        CreateMap<ReportMenuShortcut, ReportMenuShortcutDto>();

        CreateMap<RecordingVoucherBook, RecordingVoucherBookDto>();
        CreateMap<CrudRecordingVoucherBookDto, RecordingVoucherBook>();

        CreateMap<Person, PersonDto>();
        CreateMap<CrudPersonDto, Person>();

        CreateMap<RegLicense, RegLicenseDto>();
        CreateMap<CrudRegLicenseDto, RegLicense>();
        CreateMap<RegLicenceInfoDto, RegLicenseInfo>();
        // CreateMap<RegLicenseInfo, RegLicenceInfoDto>();
        CreateMap<CrudReglicenseInfoDto, RegLicenseInfo>();
        CreateMap<RegLicenseInfo, CrudReglicenseInfoDto>();

        CreateMap<CrudTenantLicenseDto, TenantLicense>();
        CreateMap<TenantLicense, TenantLicenseDto>();

        CreateMap<TenantAccBalanceSheet, TenantAccBalanceSheetDto>();
        CreateMap<DefaultAccBalanceSheet, TenantAccBalanceSheetDto>();
        CreateMap<CrudTenantAccBalanceSheetDto, TenantAccBalanceSheet>();

        CreateMap<DebtBalanceSheetDto, DebtBalanceSheetDto>();
        CreateMap<IncreaseReducedAssetToolDto, IncreaseReducedAssetToolDto>();
        CreateMap<AssetBookDto, AssetBookDto>();
        CreateMap<SpreadsheetDepreciationDto, SpreadsheetDepreciationDto>();
        CreateMap<SpreadsheetDepreciationAMDto, SpreadsheetDepreciationAMDto>();

        CreateMap<TenantCashFollowStatement, TenantCashFollowStatementDto>();
        CreateMap<DefaultCashFollowStatement, TenantCashFollowStatementDto>();
        CreateMap<CrudTenantCashFollowStatementDto, TenantCashFollowStatement>();

        CreateMap<TenantBusinessResult, TenantBusinessResultDto>();
        CreateMap<DefaultBusinessResult, TenantBusinessResultDto>();
        CreateMap<CrudTenantBusinessResultDto, TenantBusinessResult>();

        CreateMap<TenantStatementTax, TenantStatementTaxDto>();
        CreateMap<DefaultStatementTax, TenantStatementTaxDto>();
        CreateMap<CrudTenantStatementTaxDto, TenantStatementTax>();

        CreateMap<TenantStatementTaxDataDto, TenantStatementTaxData>();

        CreateMap<CrudTenantDto, CustomerRegister>();
        CreateMap<CrudTenantExtendInfoDto, TenantExtendInfo>();

        CreateMap<CrudRefVoucherDto, RefVoucher>();

        CreateMap<CrudInvoiceStatusDto, InvoiceStatus>();
        CreateMap<InvoiceStatus, InvoiceStatusDto>();

        CreateMap<CrudPositionDto, Position>();
        CreateMap<Position, PositionDto>();

        CreateMap<CrudEmployeeDto, Employee>();
        CreateMap<Employee, EmployeeDto>();

        CreateMap<CrudSalaryPeriodDto, SalaryPeriod>();
        CreateMap<SalaryPeriod, SalaryPeriodDto>();

        CreateMap<CrudSalaryCategoryDto, SalaryCategory>();
        CreateMap<SalaryCategory, SalaryCategoryDto>();

        CreateMap<SalaryEmployee, SalaryEmployeeDto>();
        CreateMap<CrudSalaryEmployeeDto, SalaryEmployee>();
        CreateMap<ExcelSalaryEmployee, SalaryEmployee>();

        CreateMap<CrudSalarySheetTypeDto, SalarySheetType>().BeforeMap((source, dest) =>
        {
            if (source.SalarySheetTypeDetails == null) return;
            foreach (var item in source.SalarySheetTypeDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.SalarySheetTypeId = source.Id;
                item.OrgCode = source.OrgCode;
            }
        });
        CreateMap<SalarySheetType, SalarySheetTypeDto>();
        CreateMap<CrudSalarySheetTypeDetailDto, SalarySheetTypeDetail>();
        CreateMap<SalarySheetTypeDetail, SalarySheetTypeDetailDto>();

        CreateMap<CrudSalaryBookDto, SalaryBook>();


        CreateMap<InventoryRecord, InventoryRecordDto>();
        CreateMap<CrudInventoryRecordDto, InventoryRecord>().BeforeMap((source, dest) =>
        {
            if (source.InventoryRecordDetails == null) return;
            int ord = 1;
            foreach (var item in source.InventoryRecordDetails)
            {
                item.Id = ObjectId.GenerateNewId().ToString();
                item.InventoryRecordId = source.Id;
                item.OrgCode = source.OrgCode;
                item.Ord0 = "A" + ord.ToString().PadLeft(9, '0');
                item.Year = source.Year;
                ord++;
            }
        });

        CreateMap<InventoryRecordDetail, InventoryRecordDetailDto>();
        CreateMap<CrudInventoryRecordDetailDto, InventoryRecordDetail>();

        CreateMap<DefaultFStatement200L01, TenantFStatement200L01>();
        CreateMap<DefaultFStatement200L02, TenantFStatement200L02>();
        CreateMap<DefaultFStatement200L03, TenantFStatement200L03>();
        CreateMap<DefaultFStatement200L04, TenantFStatement200L04>();
        CreateMap<DefaultFStatement200L05, TenantFStatement200L05>();
        CreateMap<DefaultFStatement200L06, TenantFStatement200L06>();
        CreateMap<DefaultFStatement200L07, TenantFStatement200L07>();
        CreateMap<DefaultFStatement200L08, TenantFStatement200L08>();
        CreateMap<DefaultFStatement200L09, TenantFStatement200L09>();
        CreateMap<DefaultFStatement200L10, TenantFStatement200L10>();
        CreateMap<DefaultFStatement200L11, TenantFStatement200L11>();
        CreateMap<DefaultFStatement200L12, TenantFStatement200L12>();
        CreateMap<DefaultFStatement200L13, TenantFStatement200L13>();
        CreateMap<DefaultFStatement200L14, TenantFStatement200L14>();
        CreateMap<DefaultFStatement200L15, TenantFStatement200L15>();
        CreateMap<DefaultFStatement200L16, TenantFStatement200L16>();
        CreateMap<DefaultFStatement200L17, TenantFStatement200L17>();
        CreateMap<DefaultFStatement200L18, TenantFStatement200L18>();
        CreateMap<DefaultFStatement200L19, TenantFStatement200L19>();

        CreateMap<DefaultFStatement133L01, TenantFStatement133L01>();
        CreateMap<DefaultFStatement133L02, TenantFStatement133L02>();
        CreateMap<DefaultFStatement133L03, TenantFStatement133L03>();
        CreateMap<DefaultFStatement133L04, TenantFStatement133L04>();
        CreateMap<DefaultFStatement133L05, TenantFStatement133L05>();
        CreateMap<DefaultFStatement133L06, TenantFStatement133L06>();
        CreateMap<DefaultFStatement133L07, TenantFStatement133L07>();

        CreateMap<DefaultFStatement200L01, FStatement200L01Dto>();
        CreateMap<DefaultFStatement200L02, FStatement200L02Dto>();
        CreateMap<DefaultFStatement200L03, FStatement200L03Dto>();
        CreateMap<DefaultFStatement200L04, FStatement200L04Dto>();
        CreateMap<DefaultFStatement200L05, FStatement200L05Dto>();
        CreateMap<DefaultFStatement200L06, FStatement200L06Dto>();
        CreateMap<DefaultFStatement200L07, FStatement200L07Dto>();
        CreateMap<DefaultFStatement200L08, FStatement200L08Dto>();
        CreateMap<DefaultFStatement200L09, FStatement200L09Dto>();
        CreateMap<DefaultFStatement200L10, FStatement200L10Dto>();
        CreateMap<DefaultFStatement200L11, FStatement200L11Dto>();
        CreateMap<DefaultFStatement200L12, FStatement200L12Dto>();
        CreateMap<DefaultFStatement200L13, FStatement200L13Dto>();
        CreateMap<DefaultFStatement200L14, FStatement200L14Dto>();
        CreateMap<DefaultFStatement200L15, FStatement200L15Dto>();
        CreateMap<DefaultFStatement200L16, FStatement200L16Dto>();
        CreateMap<DefaultFStatement200L17, FStatement200L17Dto>();
        CreateMap<DefaultFStatement200L18, FStatement200L18Dto>();
        CreateMap<DefaultFStatement200L19, FStatement200L19Dto>();

        CreateMap<TenantFStatement200L01, FStatement200L01Dto>();
        CreateMap<TenantFStatement200L02, FStatement200L02Dto>();
        CreateMap<TenantFStatement200L03, FStatement200L03Dto>();
        CreateMap<TenantFStatement200L04, FStatement200L04Dto>();
        CreateMap<TenantFStatement200L05, FStatement200L05Dto>();
        CreateMap<TenantFStatement200L06, FStatement200L06Dto>();
        CreateMap<TenantFStatement200L07, FStatement200L07Dto>();
        CreateMap<TenantFStatement200L08, FStatement200L08Dto>();
        CreateMap<TenantFStatement200L09, FStatement200L09Dto>();
        CreateMap<TenantFStatement200L10, FStatement200L10Dto>();
        CreateMap<TenantFStatement200L11, FStatement200L11Dto>();
        CreateMap<TenantFStatement200L12, FStatement200L12Dto>();
        CreateMap<TenantFStatement200L13, FStatement200L13Dto>();
        CreateMap<TenantFStatement200L14, FStatement200L14Dto>();
        CreateMap<TenantFStatement200L15, FStatement200L15Dto>();
        CreateMap<TenantFStatement200L16, FStatement200L16Dto>();
        CreateMap<TenantFStatement200L17, FStatement200L17Dto>();
        CreateMap<TenantFStatement200L18, FStatement200L18Dto>();
        CreateMap<TenantFStatement200L19, FStatement200L19Dto>();

        CreateMap<CrudFStatement200L01Dto, TenantFStatement200L01>();
        CreateMap<CrudFStatement200L02Dto, TenantFStatement200L02>();
        CreateMap<CrudFStatement200L03Dto, TenantFStatement200L03>();
        CreateMap<CrudFStatement200L04Dto, TenantFStatement200L04>();
        CreateMap<CrudFStatement200L05Dto, TenantFStatement200L05>();
        CreateMap<CrudFStatement200L06Dto, TenantFStatement200L06>();
        CreateMap<CrudFStatement200L07Dto, TenantFStatement200L07>();
        CreateMap<CrudFStatement200L08Dto, TenantFStatement200L08>();
        CreateMap<CrudFStatement200L09Dto, TenantFStatement200L09>();
        CreateMap<CrudFStatement200L10Dto, TenantFStatement200L10>();
        CreateMap<CrudFStatement200L11Dto, TenantFStatement200L11>();
        CreateMap<CrudFStatement200L12Dto, TenantFStatement200L12>();
        CreateMap<CrudFStatement200L13Dto, TenantFStatement200L13>();
        CreateMap<CrudFStatement200L14Dto, TenantFStatement200L14>();
        CreateMap<CrudFStatement200L15Dto, TenantFStatement200L15>();
        CreateMap<CrudFStatement200L16Dto, TenantFStatement200L16>();
        CreateMap<CrudFStatement200L17Dto, TenantFStatement200L17>();
        CreateMap<CrudFStatement200L18Dto, TenantFStatement200L18>();
        CreateMap<CrudFStatement200L19Dto, TenantFStatement200L19>();

        CreateMap<DefaultFStatement133L01, FStatement133L01Dto>();
        CreateMap<DefaultFStatement133L02, FStatement133L02Dto>();
        CreateMap<DefaultFStatement133L03, FStatement133L03Dto>();
        CreateMap<DefaultFStatement133L04, FStatement133L04Dto>();
        CreateMap<DefaultFStatement133L05, FStatement133L05Dto>();
        CreateMap<DefaultFStatement133L06, FStatement133L06Dto>();
        CreateMap<DefaultFStatement133L07, FStatement133L07Dto>();

        CreateMap<TenantFStatement133L01, FStatement133L01Dto>();
        CreateMap<TenantFStatement133L02, FStatement133L02Dto>();
        CreateMap<TenantFStatement133L03, FStatement133L03Dto>();
        CreateMap<TenantFStatement133L04, FStatement133L04Dto>();
        CreateMap<TenantFStatement133L05, FStatement133L05Dto>();
        CreateMap<TenantFStatement133L06, FStatement133L06Dto>();
        CreateMap<TenantFStatement133L07, FStatement133L07Dto>();

        CreateMap<CrudFStatement133L01Dto, TenantFStatement133L01>();
        CreateMap<CrudFStatement133L02Dto, TenantFStatement133L02>();
        CreateMap<CrudFStatement133L03Dto, TenantFStatement133L03>();
        CreateMap<CrudFStatement133L04Dto, TenantFStatement133L04>();
        CreateMap<CrudFStatement133L05Dto, TenantFStatement133L05>();
        CreateMap<CrudFStatement133L06Dto, TenantFStatement133L06>();
        CreateMap<CrudFStatement133L07Dto, TenantFStatement133L07>();

        CreateMap<TenantStatementTaxData, TenantStatementTaxDataDto>();

        CreateMap<CrudFeeTypeDto, FeeType>();
        CreateMap<FeeType, FeeTypeDto>();

        CreateMap<LinkCode, LinkCodeDto>();

        CreateMap<VoucherType, VoucherTypeDto>();
        CreateMap<DefaultVoucherType, VoucherTypeDto>();

        CreateMap<CategoryDelete, CategoryDataDto>();
        CreateMap<CrudVoucherPaymentBookDto, VoucherPaymentBook>();

        CreateMap<CrudAssetToolDetailDepreciationDto, AssetToolDetailDepreciation>();
        CreateMap<AssetToolDetailDepreciationDto, AssetToolDetailDepreciation>();

        CreateMap<DefaultFixError, FixError>();
        CreateMap<DefaultCareer, Career>();
    }
}