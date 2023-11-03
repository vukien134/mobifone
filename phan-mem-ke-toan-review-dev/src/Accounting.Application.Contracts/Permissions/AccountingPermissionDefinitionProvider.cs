using Accounting.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace Accounting.Permissions;

public class AccountingPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        ConfigCategoryGroup(context);
        ConfigBalanceGroup(context);
        ConfigAccVoucherGroup(context);
        ConfigProductVoucherGroup(context);
        ConfigGeneralAccVoucherGroup(context);
        ConfigInvoiceGroup(context);
        ConfigFixedAssetGroup(context);
        ConfigAllocateToolGroup(context);
        ConfigProductCostGroup(context);
        ConfigDashboardGroup(context);
        ConfigSalaryGroup(context);
        ConfigSystemGroup(context);
    }
    private void ConfigSystemGroup(IPermissionDefinitionContext context)
    {
        var systemGroup = context.AddGroup(AccountingPermissions.SystemGroup,
                                        G(AccountingPermissions.SystemGroup),
                                        MultiTenancySides.Tenant);

        var roleManager = systemGroup.AddPermission(AccountingPermissions.RoleManager,
                G(AccountingPermissions.RoleManager), MultiTenancySides.Tenant);
        roleManager.AddChild(AccountingPermissions.RoleManagerView,
                G(AccountingPermissions.RoleManagerView), MultiTenancySides.Tenant);
        roleManager.AddChild(AccountingPermissions.RoleManagerCreate,
                G(AccountingPermissions.RoleManagerCreate), MultiTenancySides.Tenant);
        roleManager.AddChild(AccountingPermissions.RoleManagerUpdate,
                G(AccountingPermissions.RoleManagerUpdate), MultiTenancySides.Tenant);
        roleManager.AddChild(AccountingPermissions.RoleManagerDelete,
                G(AccountingPermissions.RoleManagerDelete), MultiTenancySides.Tenant);
        roleManager.AddChild(AccountingPermissions.RoleManagerAssign,
                G(AccountingPermissions.RoleManagerAssign), MultiTenancySides.Tenant);

        var userManager = systemGroup.AddPermission(AccountingPermissions.UserManager,
                G(AccountingPermissions.UserManager), MultiTenancySides.Tenant);
        userManager.AddChild(AccountingPermissions.UserManagerView,
                G(AccountingPermissions.UserManagerView), MultiTenancySides.Tenant);
        userManager.AddChild(AccountingPermissions.UserManagerCreate,
                G(AccountingPermissions.UserManagerCreate), MultiTenancySides.Tenant);
        userManager.AddChild(AccountingPermissions.UserManagerUpdate,
                G(AccountingPermissions.UserManagerUpdate), MultiTenancySides.Tenant);
        userManager.AddChild(AccountingPermissions.UserManagerDelete,
                G(AccountingPermissions.UserManagerDelete), MultiTenancySides.Tenant);

        var orgUnitManager = systemGroup.AddPermission(AccountingPermissions.OrgUnitManager,
                G(AccountingPermissions.OrgUnitManager), MultiTenancySides.Tenant);
        orgUnitManager.AddChild(AccountingPermissions.OrgUnitManagerView,
                G(AccountingPermissions.OrgUnitManagerView), MultiTenancySides.Tenant);
        orgUnitManager.AddChild(AccountingPermissions.OrgUnitManagerCreate,
                G(AccountingPermissions.OrgUnitManagerCreate), MultiTenancySides.Tenant);
        orgUnitManager.AddChild(AccountingPermissions.OrgUnitManagerUpdate,
                G(AccountingPermissions.OrgUnitManagerUpdate), MultiTenancySides.Tenant);
        orgUnitManager.AddChild(AccountingPermissions.OrgUnitManagerDelete,
                G(AccountingPermissions.OrgUnitManagerDelete), MultiTenancySides.Tenant);

        var tenantSettingManager = systemGroup.AddPermission(AccountingPermissions.TenantSettingManager,
                G(AccountingPermissions.TenantSettingManager), MultiTenancySides.Tenant);
        tenantSettingManager.AddChild(AccountingPermissions.TenantSettingManagerView,
                G(AccountingPermissions.TenantSettingManagerView), MultiTenancySides.Tenant);
        tenantSettingManager.AddChild(AccountingPermissions.TenantSettingManagerCreate,
                G(AccountingPermissions.TenantSettingManagerCreate), MultiTenancySides.Tenant);
        tenantSettingManager.AddChild(AccountingPermissions.TenantSettingManagerUpdate,
                G(AccountingPermissions.TenantSettingManagerUpdate), MultiTenancySides.Tenant);
        tenantSettingManager.AddChild(AccountingPermissions.TenantSettingManagerDelete,
                G(AccountingPermissions.TenantSettingManagerDelete), MultiTenancySides.Tenant);

        var voucherCategoryManager = systemGroup.AddPermission(AccountingPermissions.VoucherCategoryManager,
                G(AccountingPermissions.VoucherCategoryManager), MultiTenancySides.Tenant);
        voucherCategoryManager.AddChild(AccountingPermissions.VoucherCategoryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        voucherCategoryManager.AddChild(AccountingPermissions.VoucherCategoryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        voucherCategoryManager.AddChild(AccountingPermissions.VoucherCategoryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        voucherCategoryManager.AddChild(AccountingPermissions.VoucherCategoryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var systemDiaryManager = systemGroup.AddPermission(AccountingPermissions.SystemDiaryManager,
                G(AccountingPermissions.SystemDiaryManager), MultiTenancySides.Tenant);
        systemDiaryManager.AddChild(AccountingPermissions.SystemDiaryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);

        var dataDeletingManager = systemGroup.AddPermission(AccountingPermissions.DataDeletingManager,
                G(AccountingPermissions.DataDeletingManager), MultiTenancySides.Tenant);
        dataDeletingManager.AddChild(AccountingPermissions.DataDeletingManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        dataDeletingManager.AddChild(AccountingPermissions.DataDeletingManagerExecute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);
    }
    private void ConfigCategoryGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.CategoryGroup,
                                G(AccountingPermissions.CategoryGroup),
                                MultiTenancySides.Tenant);

        var accountSystemManager = group.AddPermission(AccountingPermissions.AccountSystemManager,
                G(AccountingPermissions.AccountSystemManager), MultiTenancySides.Tenant);
        accountSystemManager.AddChild(AccountingPermissions.AccountSystemManagerView,
                G(AccountingPermissions.AccountSystemManagerView), MultiTenancySides.Tenant);
        accountSystemManager.AddChild(AccountingPermissions.AccountSystemManagerCreate,
                G(AccountingPermissions.AccountSystemManagerCreate), MultiTenancySides.Tenant);
        accountSystemManager.AddChild(AccountingPermissions.AccountSystemManagerUpdate,
                G(AccountingPermissions.AccountSystemManagerUpdate), MultiTenancySides.Tenant);
        accountSystemManager.AddChild(AccountingPermissions.AccountSystemManagerDelete,
                G(AccountingPermissions.AccountSystemManagerDelete), MultiTenancySides.Tenant);

        var caseManager = group.AddPermission(AccountingPermissions.CaseManager,
                G(AccountingPermissions.CaseManager), MultiTenancySides.Tenant);
        caseManager.AddChild(AccountingPermissions.CaseManagerView,
                G(AccountingPermissions.CaseManagerView), MultiTenancySides.Tenant);
        caseManager.AddChild(AccountingPermissions.CaseManagerCreate,
                G(AccountingPermissions.CaseManagerCreate), MultiTenancySides.Tenant);
        caseManager.AddChild(AccountingPermissions.CaseManagerUpdate,
                G(AccountingPermissions.CaseManagerUpdate), MultiTenancySides.Tenant);
        caseManager.AddChild(AccountingPermissions.CaseManagerDelete,
                G(AccountingPermissions.CaseManagerDelete), MultiTenancySides.Tenant);

        var personManager = group.AddPermission(AccountingPermissions.PersonManager,
                G(AccountingPermissions.PersonManager), MultiTenancySides.Tenant);
        personManager.AddChild(AccountingPermissions.PersonManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        personManager.AddChild(AccountingPermissions.PersonManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        personManager.AddChild(AccountingPermissions.PersonManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        personManager.AddChild(AccountingPermissions.PersonManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var sectionManager = group.AddPermission(AccountingPermissions.SectionManager,
                G(AccountingPermissions.SectionManager), MultiTenancySides.Tenant);
        sectionManager.AddChild(AccountingPermissions.SectionManagerView,
                G(AccountingPermissions.SectionManagerView), MultiTenancySides.Tenant);
        sectionManager.AddChild(AccountingPermissions.SectionManagerCreate,
                G(AccountingPermissions.SectionManagerCreate), MultiTenancySides.Tenant);
        sectionManager.AddChild(AccountingPermissions.SectionManagerUpdate,
                G(AccountingPermissions.SectionManagerUpdate), MultiTenancySides.Tenant);
        sectionManager.AddChild(AccountingPermissions.SectionManagerDelete,
                G(AccountingPermissions.SectionManagerDelete), MultiTenancySides.Tenant);


        var partnerGroupManager = group.AddPermission(AccountingPermissions.PartnerGroupManager,
                G(AccountingPermissions.PartnerGroupManager), MultiTenancySides.Tenant);
        partnerGroupManager.AddChild(AccountingPermissions.PartnerGroupManagerView,
                G(AccountingPermissions.PartnerGroupManagerView), MultiTenancySides.Tenant);
        partnerGroupManager.AddChild(AccountingPermissions.PartnerGroupManagerCreate,
                G(AccountingPermissions.PartnerGroupManagerCreate), MultiTenancySides.Tenant);
        partnerGroupManager.AddChild(AccountingPermissions.PartnerGroupManagerUpdate,
                G(AccountingPermissions.PartnerGroupManagerUpdate), MultiTenancySides.Tenant);
        partnerGroupManager.AddChild(AccountingPermissions.PartnerGroupManagerDelete,
                G(AccountingPermissions.PartnerGroupManagerDelete), MultiTenancySides.Tenant);

        var partnerManager = group.AddPermission(AccountingPermissions.PartnerManager,
                G(AccountingPermissions.PartnerManager), MultiTenancySides.Tenant);
        partnerManager.AddChild(AccountingPermissions.PartnerManagerView,
                G(AccountingPermissions.PartnerManagerView), MultiTenancySides.Tenant);
        partnerManager.AddChild(AccountingPermissions.PartnerManagerCreate,
                G(AccountingPermissions.PartnerManagerCreate), MultiTenancySides.Tenant);
        partnerManager.AddChild(AccountingPermissions.PartnerManagerUpdate,
                G(AccountingPermissions.PartnerManagerUpdate), MultiTenancySides.Tenant);
        partnerManager.AddChild(AccountingPermissions.PartnerManagerDelete,
                G(AccountingPermissions.PartnerManagerDelete), MultiTenancySides.Tenant);

        var yearCategoryManager = group.AddPermission(AccountingPermissions.YearCategoryManager,
                G(AccountingPermissions.YearCategoryManager), MultiTenancySides.Tenant);
        yearCategoryManager.AddChild(AccountingPermissions.YearCategoryManagerView,
                G(AccountingPermissions.YearCategoryManagerView), MultiTenancySides.Tenant);
        yearCategoryManager.AddChild(AccountingPermissions.YearCategoryManagerCreate,
                G(AccountingPermissions.YearCategoryManagerCreate), MultiTenancySides.Tenant);
        yearCategoryManager.AddChild(AccountingPermissions.YearCategoryManagerUpdate,
                G(AccountingPermissions.YearCategoryManagerUpdate), MultiTenancySides.Tenant);
        yearCategoryManager.AddChild(AccountingPermissions.YearCategoryManagerDelete,
                G(AccountingPermissions.YearCategoryManagerDelete), MultiTenancySides.Tenant);

        var departmentManager = group.AddPermission(AccountingPermissions.DepartmentManager,
                G(AccountingPermissions.DepartmentManager), MultiTenancySides.Tenant);
        departmentManager.AddChild(AccountingPermissions.DepartmentManagerView,
                G(AccountingPermissions.DepartmentManagerView), MultiTenancySides.Tenant);
        departmentManager.AddChild(AccountingPermissions.DepartmentManagerCreate,
                G(AccountingPermissions.DepartmentManagerCreate), MultiTenancySides.Tenant);
        departmentManager.AddChild(AccountingPermissions.DepartmentManagerUpdate,
                G(AccountingPermissions.DepartmentManagerUpdate), MultiTenancySides.Tenant);
        departmentManager.AddChild(AccountingPermissions.DepartmentManagerDelete,
                G(AccountingPermissions.DepartmentManagerDelete), MultiTenancySides.Tenant);

        var workPlaceManager = group.AddPermission(AccountingPermissions.WorkPlaceManager,
                G(AccountingPermissions.WorkPlaceManager), MultiTenancySides.Tenant);
        workPlaceManager.AddChild(AccountingPermissions.WorkPlaceManagerView,
                G(AccountingPermissions.WorkPlaceManagerView), MultiTenancySides.Tenant);
        workPlaceManager.AddChild(AccountingPermissions.WorkPlaceManagerCreate,
                G(AccountingPermissions.WorkPlaceManagerCreate), MultiTenancySides.Tenant);
        workPlaceManager.AddChild(AccountingPermissions.WorkPlaceManagerUpdate,
                G(AccountingPermissions.WorkPlaceManagerUpdate), MultiTenancySides.Tenant);
        workPlaceManager.AddChild(AccountingPermissions.WorkPlaceManagerDelete,
                G(AccountingPermissions.WorkPlaceManagerDelete), MultiTenancySides.Tenant);

        var saleChannelManager = group.AddPermission(AccountingPermissions.SaleChannelManager,
                G(AccountingPermissions.SaleChannelManager), MultiTenancySides.Tenant);
        saleChannelManager.AddChild(AccountingPermissions.SaleChannelManagerView,
                G(AccountingPermissions.SaleChannelManagerView), MultiTenancySides.Tenant);
        saleChannelManager.AddChild(AccountingPermissions.SaleChannelManagerCreate,
                G(AccountingPermissions.SaleChannelManagerCreate), MultiTenancySides.Tenant);
        saleChannelManager.AddChild(AccountingPermissions.SaleChannelManagerUpdate,
                G(AccountingPermissions.SaleChannelManagerUpdate), MultiTenancySides.Tenant);
        saleChannelManager.AddChild(AccountingPermissions.SaleChannelManagerDelete,
                G(AccountingPermissions.SaleChannelManagerDelete), MultiTenancySides.Tenant);

        var productGroupManager = group.AddPermission(AccountingPermissions.ProductGroupManager,
                G(AccountingPermissions.ProductGroupManager), MultiTenancySides.Tenant);
        productGroupManager.AddChild(AccountingPermissions.ProductGroupManagerView,
                G(AccountingPermissions.ProductGroupManagerView), MultiTenancySides.Tenant);
        productGroupManager.AddChild(AccountingPermissions.ProductGroupManagerCreate,
                G(AccountingPermissions.ProductGroupManagerCreate), MultiTenancySides.Tenant);
        productGroupManager.AddChild(AccountingPermissions.ProductGroupManagerUpdate,
                G(AccountingPermissions.ProductGroupManagerUpdate), MultiTenancySides.Tenant);
        productGroupManager.AddChild(AccountingPermissions.ProductGroupManagerDelete,
                G(AccountingPermissions.ProductGroupManagerDelete), MultiTenancySides.Tenant);

        var productManager = group.AddPermission(AccountingPermissions.ProductManager,
                G(AccountingPermissions.ProductManager), MultiTenancySides.Tenant);
        productManager.AddChild(AccountingPermissions.ProductManagerView,
                G(AccountingPermissions.ProductManagerView), MultiTenancySides.Tenant);
        productManager.AddChild(AccountingPermissions.ProductManagerCreate,
                G(AccountingPermissions.ProductManagerCreate), MultiTenancySides.Tenant);
        productManager.AddChild(AccountingPermissions.ProductManagerUpdate,
                G(AccountingPermissions.ProductManagerUpdate), MultiTenancySides.Tenant);
        productManager.AddChild(AccountingPermissions.ProductManagerDelete,
                G(AccountingPermissions.ProductManagerDelete), MultiTenancySides.Tenant);

        var exciseTaxManager = group.AddPermission(AccountingPermissions.ExciseTaxManager,
                G(AccountingPermissions.ExciseTaxManager), MultiTenancySides.Tenant);
        exciseTaxManager.AddChild(AccountingPermissions.ExciseTaxManagerView,
                G(AccountingPermissions.ExciseTaxManagerView), MultiTenancySides.Tenant);
        exciseTaxManager.AddChild(AccountingPermissions.ExciseTaxManagerCreate,
                G(AccountingPermissions.ExciseTaxManagerCreate), MultiTenancySides.Tenant);
        exciseTaxManager.AddChild(AccountingPermissions.ExciseTaxManagerUpdate,
                G(AccountingPermissions.ExciseTaxManagerUpdate), MultiTenancySides.Tenant);
        exciseTaxManager.AddChild(AccountingPermissions.ExciseTaxManagerDelete,
                G(AccountingPermissions.ExciseTaxManagerDelete), MultiTenancySides.Tenant);

        var contractManager = group.AddPermission(AccountingPermissions.ContractManager,
                G(AccountingPermissions.ContractManager), MultiTenancySides.Tenant);
        contractManager.AddChild(AccountingPermissions.ContractManagerView,
                G(AccountingPermissions.ContractManagerView), MultiTenancySides.Tenant);
        contractManager.AddChild(AccountingPermissions.ContractManagerCreate,
                G(AccountingPermissions.ContractManagerCreate), MultiTenancySides.Tenant);
        contractManager.AddChild(AccountingPermissions.ContractManagerUpdate,
                G(AccountingPermissions.ContractManagerUpdate), MultiTenancySides.Tenant);
        contractManager.AddChild(AccountingPermissions.ContractManagerDelete,
                G(AccountingPermissions.ContractManagerDelete), MultiTenancySides.Tenant);

        var productLotManager = group.AddPermission(AccountingPermissions.ProductLotManager,
                G(AccountingPermissions.ProductLotManager), MultiTenancySides.Tenant);
        productLotManager.AddChild(AccountingPermissions.ProductLotManagerView,
                G(AccountingPermissions.ProductLotManagerView), MultiTenancySides.Tenant);
        productLotManager.AddChild(AccountingPermissions.ProductLotManagerCreate,
                G(AccountingPermissions.ProductLotManagerCreate), MultiTenancySides.Tenant);
        productLotManager.AddChild(AccountingPermissions.ProductLotManagerUpdate,
                G(AccountingPermissions.ProductLotManagerUpdate), MultiTenancySides.Tenant);
        productLotManager.AddChild(AccountingPermissions.ProductLotManagerDelete,
                G(AccountingPermissions.ProductLotManagerDelete), MultiTenancySides.Tenant);

        var productOriginManager = group.AddPermission(AccountingPermissions.ProductOriginManager,
                G(AccountingPermissions.ProductOriginManager), MultiTenancySides.Tenant);
        productOriginManager.AddChild(AccountingPermissions.ProductOriginManagerView,
                G(AccountingPermissions.ProductOriginManagerView), MultiTenancySides.Tenant);
        productOriginManager.AddChild(AccountingPermissions.ProductOriginManagerCreate,
                G(AccountingPermissions.ProductOriginManagerCreate), MultiTenancySides.Tenant);
        productOriginManager.AddChild(AccountingPermissions.ProductOriginManagerUpdate,
                G(AccountingPermissions.ProductOriginManagerUpdate), MultiTenancySides.Tenant);
        productOriginManager.AddChild(AccountingPermissions.ProductOriginManagerDelete,
                G(AccountingPermissions.ProductOriginManagerDelete), MultiTenancySides.Tenant);

        var productionPeriodManager = group.AddPermission(AccountingPermissions.ProductionPeriodManager,
                G(AccountingPermissions.ProductionPeriodManager), MultiTenancySides.Tenant);
        productionPeriodManager.AddChild(AccountingPermissions.ProductionPeriodManagerView,
                G(AccountingPermissions.ProductionPeriodManagerView), MultiTenancySides.Tenant);
        productionPeriodManager.AddChild(AccountingPermissions.ProductionPeriodManagerCreate,
                G(AccountingPermissions.ProductionPeriodManagerCreate), MultiTenancySides.Tenant);
        productionPeriodManager.AddChild(AccountingPermissions.ProductionPeriodManagerUpdate,
                G(AccountingPermissions.ProductionPeriodManagerUpdate), MultiTenancySides.Tenant);
        productionPeriodManager.AddChild(AccountingPermissions.ProductionPeriodManagerDelete,
                G(AccountingPermissions.ProductionPeriodManagerDelete), MultiTenancySides.Tenant);

        var unitManager = group.AddPermission(AccountingPermissions.UnitManager,
                G(AccountingPermissions.UnitManager), MultiTenancySides.Tenant);
        unitManager.AddChild(AccountingPermissions.UnitManagerView,
                G(AccountingPermissions.UnitManagerView), MultiTenancySides.Tenant);
        unitManager.AddChild(AccountingPermissions.UnitManagerCreate,
                G(AccountingPermissions.UnitManagerCreate), MultiTenancySides.Tenant);
        unitManager.AddChild(AccountingPermissions.UnitManagerUpdate,
                G(AccountingPermissions.UnitManagerUpdate), MultiTenancySides.Tenant);
        unitManager.AddChild(AccountingPermissions.UnitManagerDelete,
                G(AccountingPermissions.UnitManagerDelete), MultiTenancySides.Tenant);

        var warehouseManager = group.AddPermission(AccountingPermissions.WarehouseManager,
                G(AccountingPermissions.WarehouseManager), MultiTenancySides.Tenant);
        warehouseManager.AddChild(AccountingPermissions.WarehouseManagerView,
                G(AccountingPermissions.WarehouseManagerView), MultiTenancySides.Tenant);
        warehouseManager.AddChild(AccountingPermissions.WarehouseManagerCreate,
                G(AccountingPermissions.WarehouseManagerCreate), MultiTenancySides.Tenant);
        warehouseManager.AddChild(AccountingPermissions.WarehouseManagerUpdate,
                G(AccountingPermissions.WarehouseManagerUpdate), MultiTenancySides.Tenant);
        warehouseManager.AddChild(AccountingPermissions.WarehouseManagerDelete,
                G(AccountingPermissions.WarehouseManagerDelete), MultiTenancySides.Tenant);

        var taxCategoryManager = group.AddPermission(AccountingPermissions.TaxCategoryManager,
                G(AccountingPermissions.TaxCategoryManager), MultiTenancySides.Tenant);
        taxCategoryManager.AddChild(AccountingPermissions.TaxCategoryManagerView,
                G(AccountingPermissions.TaxCategoryManagerView), MultiTenancySides.Tenant);
        taxCategoryManager.AddChild(AccountingPermissions.TaxCategoryManagerCreate,
                G(AccountingPermissions.TaxCategoryManagerCreate), MultiTenancySides.Tenant);
        taxCategoryManager.AddChild(AccountingPermissions.TaxCategoryManagerUpdate,
                G(AccountingPermissions.TaxCategoryManagerUpdate), MultiTenancySides.Tenant);
        taxCategoryManager.AddChild(AccountingPermissions.TaxCategoryManagerDelete,
                G(AccountingPermissions.TaxCategoryManagerDelete), MultiTenancySides.Tenant);

        var paymentTermManager = group.AddPermission(AccountingPermissions.PaymentTermManager,
                G(AccountingPermissions.PaymentTermManager), MultiTenancySides.Tenant);
        paymentTermManager.AddChild(AccountingPermissions.PaymentTermManagerView,
                G(AccountingPermissions.PaymentTermManagerView), MultiTenancySides.Tenant);
        paymentTermManager.AddChild(AccountingPermissions.PaymentTermManagerCreate,
                G(AccountingPermissions.PaymentTermManagerCreate), MultiTenancySides.Tenant);
        paymentTermManager.AddChild(AccountingPermissions.PaymentTermManagerUpdate,
                G(AccountingPermissions.PaymentTermManagerUpdate), MultiTenancySides.Tenant);
        paymentTermManager.AddChild(AccountingPermissions.PaymentTermManagerDelete,
                G(AccountingPermissions.PaymentTermManagerDelete), MultiTenancySides.Tenant);

        var fProductWorkManager = group.AddPermission(AccountingPermissions.FProductWorkManager,
                G(AccountingPermissions.FProductWorkManager), MultiTenancySides.Tenant);
        fProductWorkManager.AddChild(AccountingPermissions.FProductWorkManagerView,
                G(AccountingPermissions.FProductWorkManagerView), MultiTenancySides.Tenant);
        fProductWorkManager.AddChild(AccountingPermissions.FProductWorkManagerCreate,
                G(AccountingPermissions.FProductWorkManagerCreate), MultiTenancySides.Tenant);
        fProductWorkManager.AddChild(AccountingPermissions.FProductWorkManagerUpdate,
                G(AccountingPermissions.FProductWorkManagerUpdate), MultiTenancySides.Tenant);
        fProductWorkManager.AddChild(AccountingPermissions.FProductWorkManagerDelete,
                G(AccountingPermissions.FProductWorkManagerDelete), MultiTenancySides.Tenant);

        var businessCategoryManager = group.AddPermission(AccountingPermissions.BusinessCategoryManager,
                G(AccountingPermissions.BusinessCategoryManager), MultiTenancySides.Tenant);
        businessCategoryManager.AddChild(AccountingPermissions.BusinessCategoryManagerView,
                G(AccountingPermissions.BusinessCategoryManagerView), MultiTenancySides.Tenant);
        businessCategoryManager.AddChild(AccountingPermissions.BusinessCategoryManagerCreate,
                G(AccountingPermissions.BusinessCategoryManagerCreate), MultiTenancySides.Tenant);
        businessCategoryManager.AddChild(AccountingPermissions.BusinessCategoryManagerUpdate,
                G(AccountingPermissions.BusinessCategoryManagerUpdate), MultiTenancySides.Tenant);
        businessCategoryManager.AddChild(AccountingPermissions.BusinessCategoryManagerDelete,
                G(AccountingPermissions.FProductWorkManagerDelete), MultiTenancySides.Tenant);

        var invoiceBookManager = group.AddPermission(AccountingPermissions.InvoiceBookManager,
                G(AccountingPermissions.InvoiceBookManager), MultiTenancySides.Tenant);
        invoiceBookManager.AddChild(AccountingPermissions.InvoiceBookManagerView,
                G(AccountingPermissions.InvoiceBookManagerView), MultiTenancySides.Tenant);
        invoiceBookManager.AddChild(AccountingPermissions.InvoiceBookManagerCreate,
                G(AccountingPermissions.InvoiceBookManagerCreate), MultiTenancySides.Tenant);
        invoiceBookManager.AddChild(AccountingPermissions.InvoiceBookManagerUpdate,
                G(AccountingPermissions.InvoiceBookManagerUpdate), MultiTenancySides.Tenant);
        invoiceBookManager.AddChild(AccountingPermissions.InvoiceBookManagerDelete,
                G(AccountingPermissions.InvoiceBookManagerDelete), MultiTenancySides.Tenant);

        var discountPriceManager = group.AddPermission(AccountingPermissions.DiscountPriceManager,
                G(AccountingPermissions.DiscountPriceManager), MultiTenancySides.Tenant);
        discountPriceManager.AddChild(AccountingPermissions.DiscountPriceManagerView,
                G(AccountingPermissions.DiscountPriceManagerView), MultiTenancySides.Tenant);
        discountPriceManager.AddChild(AccountingPermissions.DiscountPriceManagerCreate,
                G(AccountingPermissions.DiscountPriceManagerCreate), MultiTenancySides.Tenant);
        discountPriceManager.AddChild(AccountingPermissions.DiscountPriceManagerUpdate,
                G(AccountingPermissions.DiscountPriceManagerUpdate), MultiTenancySides.Tenant);
        discountPriceManager.AddChild(AccountingPermissions.DiscountPriceManagerDelete,
                G(AccountingPermissions.DiscountPriceManagerDelete), MultiTenancySides.Tenant);

        var currencyManager = group.AddPermission(AccountingPermissions.CurrencyManager,
                G(AccountingPermissions.CurrencyManager), MultiTenancySides.Tenant);
        currencyManager.AddChild(AccountingPermissions.CurrencyManagerView,
                G(AccountingPermissions.CurrencyManagerView), MultiTenancySides.Tenant);
        currencyManager.AddChild(AccountingPermissions.CurrencyManagerCreate,
                G(AccountingPermissions.CurrencyManagerCreate), MultiTenancySides.Tenant);
        currencyManager.AddChild(AccountingPermissions.CurrencyManagerUpdate,
                G(AccountingPermissions.CurrencyManagerUpdate), MultiTenancySides.Tenant);
        currencyManager.AddChild(AccountingPermissions.CurrencyManagerDelete,
                G(AccountingPermissions.CurrencyManagerDelete), MultiTenancySides.Tenant);

        var feeTypeManager = group.AddPermission(AccountingPermissions.FeeTypeManager,
                G(AccountingPermissions.FeeTypeManager), MultiTenancySides.Tenant);
        feeTypeManager.AddChild(AccountingPermissions.FeeTypeManagerView,
                G(AccountingPermissions.FeeTypeManagerView), MultiTenancySides.Tenant);
        feeTypeManager.AddChild(AccountingPermissions.FeeTypeManagerCreate,
                G(AccountingPermissions.FeeTypeManagerCreate), MultiTenancySides.Tenant);
        feeTypeManager.AddChild(AccountingPermissions.FeeTypeManagerUpdate,
                G(AccountingPermissions.FeeTypeManagerUpdate), MultiTenancySides.Tenant);
        feeTypeManager.AddChild(AccountingPermissions.FeeTypeManagerDelete,
                G(AccountingPermissions.FeeTypeManagerDelete), MultiTenancySides.Tenant);

        var barcodeManager = group.AddPermission(AccountingPermissions.BarcodeManager,
                G(AccountingPermissions.BarcodeManager), MultiTenancySides.Tenant);
        barcodeManager.AddChild(AccountingPermissions.BarcodeManagerView,
                G(AccountingPermissions.BarcodeManagerView), MultiTenancySides.Tenant);
        barcodeManager.AddChild(AccountingPermissions.BarcodeManagerPrint,
                G(AccountingPermissions.BarcodeManagerPrint), MultiTenancySides.Tenant);
    }

    private void ConfigBalanceGroup(IPermissionDefinitionContext context)
    {
        var balanceGroup = context.AddGroup(AccountingPermissions.BalanceGroup,
                                        G(AccountingPermissions.BalanceGroup),
                                        MultiTenancySides.Tenant);

        var accountOpeningBalanceManager = balanceGroup.AddPermission(AccountingPermissions.AccountOpeningBalanceManager,
                G(AccountingPermissions.AccountOpeningBalanceManager), MultiTenancySides.Tenant);
        accountOpeningBalanceManager.AddChild(AccountingPermissions.AccountOpeningBalanceManagerView,
                G(AccountingPermissions.AccountOpeningBalanceManagerView), MultiTenancySides.Tenant);
        accountOpeningBalanceManager.AddChild(AccountingPermissions.AccountOpeningBalanceManagerCreate,
                G(AccountingPermissions.AccountOpeningBalanceManagerCreate), MultiTenancySides.Tenant);
        accountOpeningBalanceManager.AddChild(AccountingPermissions.AccountOpeningBalanceManagerUpdate,
                G(AccountingPermissions.AccountOpeningBalanceManagerUpdate), MultiTenancySides.Tenant);
        accountOpeningBalanceManager.AddChild(AccountingPermissions.AccountOpeningBalanceManagerDelete,
                G(AccountingPermissions.AccountOpeningBalanceManagerDelete), MultiTenancySides.Tenant);

        var productOpeningBalanceManager = balanceGroup.AddPermission(AccountingPermissions.ProductOpeningBalanceManager,
                G(AccountingPermissions.ProductOpeningBalanceManager), MultiTenancySides.Tenant);
        productOpeningBalanceManager.AddChild(AccountingPermissions.ProductOpeningBalanceManagerView,
                G(AccountingPermissions.ProductOpeningBalanceManagerView), MultiTenancySides.Tenant);
        productOpeningBalanceManager.AddChild(AccountingPermissions.ProductOpeningBalanceManagerCreate,
                G(AccountingPermissions.ProductOpeningBalanceManagerCreate), MultiTenancySides.Tenant);
        productOpeningBalanceManager.AddChild(AccountingPermissions.ProductOpeningBalanceManagerUpdate,
                G(AccountingPermissions.ProductOpeningBalanceManagerUpdate), MultiTenancySides.Tenant);
        productOpeningBalanceManager.AddChild(AccountingPermissions.ProductOpeningBalanceManagerDelete,
                G(AccountingPermissions.ProductOpeningBalanceManagerDelete), MultiTenancySides.Tenant);
    }

    private void ConfigAccVoucherGroup(IPermissionDefinitionContext context)
    {
        var accVoucherGroup = context.AddGroup(AccountingPermissions.AccVoucherGroup,
                                        G(AccountingPermissions.AccVoucherGroup),
                                        MultiTenancySides.Tenant);

        var receiptVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.ReceiptVoucherManager,
                G(AccountingPermissions.ReceiptVoucherManager), MultiTenancySides.Tenant);
        receiptVoucherManager.AddChild(AccountingPermissions.ReceiptVoucherManagerView,
                G(AccountingPermissions.ReceiptVoucherManagerView), MultiTenancySides.Tenant);
        receiptVoucherManager.AddChild(AccountingPermissions.ReceiptVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        receiptVoucherManager.AddChild(AccountingPermissions.ReceiptVoucherManagerCreate,
                G(AccountingPermissions.ReceiptVoucherManagerCreate), MultiTenancySides.Tenant);
        receiptVoucherManager.AddChild(AccountingPermissions.ReceiptVoucherManagerUpdate,
                G(AccountingPermissions.ReceiptVoucherManagerUpdate), MultiTenancySides.Tenant);
        receiptVoucherManager.AddChild(AccountingPermissions.ReceiptVoucherManagerDelete,
                G(AccountingPermissions.ReceiptVoucherManagerDelete), MultiTenancySides.Tenant);
        receiptVoucherManager.AddChild(AccountingPermissions.ReceiptVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var paymentVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.PaymentVoucherManager,
                G(AccountingPermissions.PaymentVoucherManager), MultiTenancySides.Tenant);
        paymentVoucherManager.AddChild(AccountingPermissions.PaymentVoucherManagerView,
                G(AccountingPermissions.PaymentVoucherManagerView), MultiTenancySides.Tenant);
        paymentVoucherManager.AddChild(AccountingPermissions.PaymentVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        paymentVoucherManager.AddChild(AccountingPermissions.PaymentVoucherManagerCreate,
                G(AccountingPermissions.PaymentVoucherManagerCreate), MultiTenancySides.Tenant);
        paymentVoucherManager.AddChild(AccountingPermissions.PaymentVoucherManagerUpdate,
                G(AccountingPermissions.PaymentVoucherManagerUpdate), MultiTenancySides.Tenant);
        paymentVoucherManager.AddChild(AccountingPermissions.PaymentVoucherManagerDelete,
                G(AccountingPermissions.PaymentVoucherManagerDelete), MultiTenancySides.Tenant);
        paymentVoucherManager.AddChild(AccountingPermissions.PaymentVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var creditNoteVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.CreditNoteVoucherManager,
                G(AccountingPermissions.CreditNoteVoucherManager), MultiTenancySides.Tenant);
        creditNoteVoucherManager.AddChild(AccountingPermissions.CreditNoteVoucherManagerView,
                G(AccountingPermissions.CreditNoteVoucherManagerView), MultiTenancySides.Tenant);
        creditNoteVoucherManager.AddChild(AccountingPermissions.CreditNoteVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        creditNoteVoucherManager.AddChild(AccountingPermissions.CreditNoteVoucherManagerCreate,
                G(AccountingPermissions.CreditNoteVoucherManagerCreate), MultiTenancySides.Tenant);
        creditNoteVoucherManager.AddChild(AccountingPermissions.CreditNoteVoucherManagerUpdate,
                G(AccountingPermissions.CreditNoteVoucherManagerUpdate), MultiTenancySides.Tenant);
        creditNoteVoucherManager.AddChild(AccountingPermissions.CreditNoteVoucherManagerDelete,
                G(AccountingPermissions.CreditNoteVoucherManagerDelete), MultiTenancySides.Tenant);
        creditNoteVoucherManager.AddChild(AccountingPermissions.CreditNoteVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var debitNoteVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.DebitNoteVoucherManager,
                G(AccountingPermissions.DebitNoteVoucherManager), MultiTenancySides.Tenant);
        debitNoteVoucherManager.AddChild(AccountingPermissions.DebitNoteVoucherManagerView,
                G(AccountingPermissions.DebitNoteVoucherManagerView), MultiTenancySides.Tenant);
        debitNoteVoucherManager.AddChild(AccountingPermissions.DebitNoteVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        debitNoteVoucherManager.AddChild(AccountingPermissions.DebitNoteVoucherManagerCreate,
                G(AccountingPermissions.DebitNoteVoucherManagerCreate), MultiTenancySides.Tenant);
        debitNoteVoucherManager.AddChild(AccountingPermissions.DebitNoteVoucherManagerUpdate,
                G(AccountingPermissions.DebitNoteVoucherManagerUpdate), MultiTenancySides.Tenant);
        debitNoteVoucherManager.AddChild(AccountingPermissions.DebitNoteVoucherManagerDelete,
                G(AccountingPermissions.DebitNoteVoucherManagerDelete), MultiTenancySides.Tenant);
        debitNoteVoucherManager.AddChild(AccountingPermissions.DebitNoteVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var ortherVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.OtherVoucherManager,
                G(AccountingPermissions.OtherVoucherManager), MultiTenancySides.Tenant);
        ortherVoucherManager.AddChild(AccountingPermissions.OtherVoucherManagerView,
                G(AccountingPermissions.OtherVoucherManagerView), MultiTenancySides.Tenant);
        ortherVoucherManager.AddChild(AccountingPermissions.OtherVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        ortherVoucherManager.AddChild(AccountingPermissions.OtherVoucherManagerCreate,
                G(AccountingPermissions.OtherVoucherManagerCreate), MultiTenancySides.Tenant);
        ortherVoucherManager.AddChild(AccountingPermissions.OtherVoucherManagerUpdate,
                G(AccountingPermissions.OtherVoucherManagerUpdate), MultiTenancySides.Tenant);
        ortherVoucherManager.AddChild(AccountingPermissions.OtherVoucherManagerDelete,
                G(AccountingPermissions.OtherVoucherManagerDelete), MultiTenancySides.Tenant);
        ortherVoucherManager.AddChild(AccountingPermissions.OtherVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var refundVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.RefundVoucherManager,
                G(AccountingPermissions.RefundVoucherManager), MultiTenancySides.Tenant);
        refundVoucherManager.AddChild(AccountingPermissions.RefundVoucherManagerView,
                G(AccountingPermissions.RefundVoucherManagerView), MultiTenancySides.Tenant);
        refundVoucherManager.AddChild(AccountingPermissions.RefundVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        refundVoucherManager.AddChild(AccountingPermissions.RefundVoucherManagerCreate,
                G(AccountingPermissions.RefundVoucherManagerCreate), MultiTenancySides.Tenant);
        refundVoucherManager.AddChild(AccountingPermissions.RefundVoucherManagerUpdate,
                G(AccountingPermissions.RefundVoucherManagerUpdate), MultiTenancySides.Tenant);
        refundVoucherManager.AddChild(AccountingPermissions.RefundVoucherManagerDelete,
                G(AccountingPermissions.RefundVoucherManagerDelete), MultiTenancySides.Tenant);
        refundVoucherManager.AddChild(AccountingPermissions.RefundVoucherManagerRecoding,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var taxVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.TaxVoucherManager,
                G(AccountingPermissions.TaxVoucherManager), MultiTenancySides.Tenant);
        taxVoucherManager.AddChild(AccountingPermissions.TaxVoucherManagerView,
                G(AccountingPermissions.TaxVoucherManagerView), MultiTenancySides.Tenant);
        taxVoucherManager.AddChild(AccountingPermissions.TaxVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        taxVoucherManager.AddChild(AccountingPermissions.TaxVoucherManagerCreate,
                G(AccountingPermissions.TaxVoucherManagerCreate), MultiTenancySides.Tenant);
        taxVoucherManager.AddChild(AccountingPermissions.TaxVoucherManagerUpdate,
                G(AccountingPermissions.TaxVoucherManagerUpdate), MultiTenancySides.Tenant);
        taxVoucherManager.AddChild(AccountingPermissions.TaxVoucherManagerDelete,
                G(AccountingPermissions.TaxVoucherManagerDelete), MultiTenancySides.Tenant);
        taxVoucherManager.AddChild(AccountingPermissions.TaxVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var deptCleaningVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.DeptCleaningVoucherManager,
                G(AccountingPermissions.DeptCleaningVoucherManager), MultiTenancySides.Tenant);
        deptCleaningVoucherManager.AddChild(AccountingPermissions.DeptCleaningVoucherManagerView,
                G(AccountingPermissions.DeptCleaningVoucherManagerView), MultiTenancySides.Tenant);
        deptCleaningVoucherManager.AddChild(AccountingPermissions.DeptCleaningVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        deptCleaningVoucherManager.AddChild(AccountingPermissions.DeptCleaningVoucherManagerCreate,
                G(AccountingPermissions.DeptCleaningVoucherManagerCreate), MultiTenancySides.Tenant);
        deptCleaningVoucherManager.AddChild(AccountingPermissions.DeptCleaningVoucherManagerUpdate,
                G(AccountingPermissions.DeptCleaningVoucherManagerUpdate), MultiTenancySides.Tenant);
        deptCleaningVoucherManager.AddChild(AccountingPermissions.DeptCleaningVoucherManagerDelete,
                G(AccountingPermissions.DeptCleaningVoucherManagerDelete), MultiTenancySides.Tenant);
        deptCleaningVoucherManager.AddChild(AccountingPermissions.DeptCleaningVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var resetVoucherManager = accVoucherGroup.AddPermission(AccountingPermissions.ResetVoucherNumberManager,
                G(AccountingPermissions.ResetVoucherNumberManager), MultiTenancySides.Tenant);
        resetVoucherManager.AddChild(AccountingPermissions.ResetVoucherNumberManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        resetVoucherManager.AddChild(AccountingPermissions.ResetVoucherNumberManagerExcute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);

        var autoAccForwardManager = accVoucherGroup.AddPermission(AccountingPermissions.AutoAccForwardManager,
                G(AccountingPermissions.AutoAccForwardManager), MultiTenancySides.Tenant);
        autoAccForwardManager.AddChild(AccountingPermissions.AutoAccForwardManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        autoAccForwardManager.AddChild(AccountingPermissions.AutoAccForwardManagerExcute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);

        var recordingVoucherBookManager = accVoucherGroup.AddPermission(AccountingPermissions.RecordingVoucherBookManager,
                G(AccountingPermissions.RecordingVoucherBookManager), MultiTenancySides.Tenant);
        recordingVoucherBookManager.AddChild(AccountingPermissions.RecordingVoucherBookManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        recordingVoucherBookManager.AddChild(AccountingPermissions.RecordingVoucherBookManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        recordingVoucherBookManager.AddChild(AccountingPermissions.RecordingVoucherBookManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        recordingVoucherBookManager.AddChild(AccountingPermissions.RecordingVoucherBookManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

    }
    private void ConfigProductVoucherGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.ProductVoucherGroup,
                                        G(AccountingPermissions.ProductVoucherGroup),
                                        MultiTenancySides.Tenant);

        var warehouseReceiptVoucherManager = group.AddPermission(AccountingPermissions.WarehouseReceiptVoucherManager,
                G(AccountingPermissions.WarehouseReceiptVoucherManager), MultiTenancySides.Tenant);
        warehouseReceiptVoucherManager.AddChild(AccountingPermissions.WarehouseReceiptVoucherManagerView,
                G(AccountingPermissions.WarehouseReceiptVoucherManagerView), MultiTenancySides.Tenant);
        warehouseReceiptVoucherManager.AddChild(AccountingPermissions.WarehouseReceiptVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        warehouseReceiptVoucherManager.AddChild(AccountingPermissions.WarehouseReceiptVoucherManagerCreate,
                G(AccountingPermissions.WarehouseReceiptVoucherManagerCreate), MultiTenancySides.Tenant);
        warehouseReceiptVoucherManager.AddChild(AccountingPermissions.WarehouseReceiptVoucherManagerUpdate,
                G(AccountingPermissions.WarehouseReceiptVoucherManagerUpdate), MultiTenancySides.Tenant);
        warehouseReceiptVoucherManager.AddChild(AccountingPermissions.WarehouseReceiptVoucherManagerDelete,
                G(AccountingPermissions.WarehouseReceiptVoucherManagerDelete), MultiTenancySides.Tenant);
        warehouseReceiptVoucherManager.AddChild(AccountingPermissions.WarehouseReceiptVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var stockOutVoucherManager = group.AddPermission(AccountingPermissions.StockOutVoucherManager,
                G(AccountingPermissions.StockOutVoucherManager), MultiTenancySides.Tenant);
        stockOutVoucherManager.AddChild(AccountingPermissions.StockOutVoucherManagerView,
                G(AccountingPermissions.StockOutVoucherManagerView), MultiTenancySides.Tenant);
        stockOutVoucherManager.AddChild(AccountingPermissions.StockOutVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        stockOutVoucherManager.AddChild(AccountingPermissions.StockOutVoucherManagerCreate,
                G(AccountingPermissions.StockOutVoucherManagerCreate), MultiTenancySides.Tenant);
        stockOutVoucherManager.AddChild(AccountingPermissions.StockOutVoucherManagerUpdate,
                G(AccountingPermissions.StockOutVoucherManagerUpdate), MultiTenancySides.Tenant);
        stockOutVoucherManager.AddChild(AccountingPermissions.StockOutVoucherManagerDelete,
                G(AccountingPermissions.StockOutVoucherManagerDelete), MultiTenancySides.Tenant);
        stockOutVoucherManager.AddChild(AccountingPermissions.StockOutVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var purchaseReturnsVoucherManager = group.AddPermission(AccountingPermissions.PurchaseReturnsVoucherManager,
                G(AccountingPermissions.PurchaseReturnsVoucherManager), MultiTenancySides.Tenant);
        purchaseReturnsVoucherManager.AddChild(AccountingPermissions.PurchaseReturnsVoucherManagerView,
                G(AccountingPermissions.PurchaseReturnsVoucherManagerView), MultiTenancySides.Tenant);
        purchaseReturnsVoucherManager.AddChild(AccountingPermissions.PurchaseReturnsVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        purchaseReturnsVoucherManager.AddChild(AccountingPermissions.PurchaseReturnsVoucherManagerCreate,
                G(AccountingPermissions.PurchaseReturnsVoucherManagerCreate), MultiTenancySides.Tenant);
        purchaseReturnsVoucherManager.AddChild(AccountingPermissions.PurchaseReturnsVoucherManagerUpdate,
                G(AccountingPermissions.PurchaseReturnsVoucherManagerUpdate), MultiTenancySides.Tenant);
        purchaseReturnsVoucherManager.AddChild(AccountingPermissions.PurchaseReturnsVoucherManagerDelete,
                G(AccountingPermissions.PurchaseReturnsVoucherManagerDelete), MultiTenancySides.Tenant);
        purchaseReturnsVoucherManager.AddChild(AccountingPermissions.PurchaseReturnsVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var salesVoucherManager = group.AddPermission(AccountingPermissions.SalesVoucherManager,
                G(AccountingPermissions.SalesVoucherManager), MultiTenancySides.Tenant);
        salesVoucherManager.AddChild(AccountingPermissions.SalesVoucherManagerView,
                G(AccountingPermissions.SalesVoucherManagerView), MultiTenancySides.Tenant);
        salesVoucherManager.AddChild(AccountingPermissions.SalesVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        salesVoucherManager.AddChild(AccountingPermissions.SalesVoucherManagerCreate,
                G(AccountingPermissions.SalesVoucherManagerCreate), MultiTenancySides.Tenant);
        salesVoucherManager.AddChild(AccountingPermissions.SalesVoucherManagerUpdate,
                G(AccountingPermissions.SalesVoucherManagerUpdate), MultiTenancySides.Tenant);
        salesVoucherManager.AddChild(AccountingPermissions.SalesVoucherManagerDelete,
                G(AccountingPermissions.SalesVoucherManagerDelete), MultiTenancySides.Tenant);
        salesVoucherManager.AddChild(AccountingPermissions.SalesVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var salesBarcodeVoucherManager = group.AddPermission(AccountingPermissions.SalesBarcodeVoucherManager,
                G(AccountingPermissions.SalesBarcodeVoucherManager), MultiTenancySides.Tenant);
        salesBarcodeVoucherManager.AddChild(AccountingPermissions.SalesBarcodeVoucherManagerView,
                G(AccountingPermissions.SalesVoucherManagerView), MultiTenancySides.Tenant);
        salesBarcodeVoucherManager.AddChild(AccountingPermissions.SalesBarcodeVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        salesBarcodeVoucherManager.AddChild(AccountingPermissions.SalesBarcodeVoucherManagerCreate,
                G(AccountingPermissions.SalesVoucherManagerCreate), MultiTenancySides.Tenant);
        salesBarcodeVoucherManager.AddChild(AccountingPermissions.SalesBarcodeVoucherManagerUpdate,
                G(AccountingPermissions.SalesVoucherManagerUpdate), MultiTenancySides.Tenant);
        salesBarcodeVoucherManager.AddChild(AccountingPermissions.SalesBarcodeVoucherManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);
        salesBarcodeVoucherManager.AddChild(AccountingPermissions.SalesBarcodeVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var importsVoucherManager = group.AddPermission(AccountingPermissions.ImportsVoucherManager,
                G(AccountingPermissions.ImportsVoucherManager), MultiTenancySides.Tenant);
        importsVoucherManager.AddChild(AccountingPermissions.ImportsVoucherManagerView,
                G(AccountingPermissions.ImportsVoucherManagerView), MultiTenancySides.Tenant);
        importsVoucherManager.AddChild(AccountingPermissions.ImportsVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        importsVoucherManager.AddChild(AccountingPermissions.ImportsVoucherManagerCreate,
                G(AccountingPermissions.ImportsVoucherManagerCreate), MultiTenancySides.Tenant);
        importsVoucherManager.AddChild(AccountingPermissions.ImportsVoucherManagerUpdate,
                G(AccountingPermissions.ImportsVoucherManagerUpdate), MultiTenancySides.Tenant);
        importsVoucherManager.AddChild(AccountingPermissions.ImportsVoucherManagerDelete,
                G(AccountingPermissions.ImportsVoucherManagerDelete), MultiTenancySides.Tenant);
        importsVoucherManager.AddChild(AccountingPermissions.ImportsVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var productTransferVoucherManager = group.AddPermission(AccountingPermissions.ProductTransferVoucherManager,
                G(AccountingPermissions.ProductTransferVoucherManager), MultiTenancySides.Tenant);
        productTransferVoucherManager.AddChild(AccountingPermissions.ProductTransferVoucherManagerView,
                G(AccountingPermissions.ProductTransferVoucherManagerView), MultiTenancySides.Tenant);
        productTransferVoucherManager.AddChild(AccountingPermissions.ProductTransferVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        productTransferVoucherManager.AddChild(AccountingPermissions.ProductTransferVoucherManagerCreate,
                G(AccountingPermissions.ProductTransferVoucherManagerCreate), MultiTenancySides.Tenant);
        productTransferVoucherManager.AddChild(AccountingPermissions.ProductTransferVoucherManagerUpdate,
                G(AccountingPermissions.ProductTransferVoucherManagerUpdate), MultiTenancySides.Tenant);
        productTransferVoucherManager.AddChild(AccountingPermissions.ProductTransferVoucherManagerDelete,
                G(AccountingPermissions.ProductTransferVoucherManagerDelete), MultiTenancySides.Tenant);
        productTransferVoucherManager.AddChild(AccountingPermissions.ProductTransferVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var productChangeVoucherManager = group.AddPermission(AccountingPermissions.ProductChangeVoucherManager,
                G(AccountingPermissions.ProductChangeVoucherManager), MultiTenancySides.Tenant);
        productChangeVoucherManager.AddChild(AccountingPermissions.ProductChangeVoucherManagerView,
                G(AccountingPermissions.ProductChangeVoucherManagerView), MultiTenancySides.Tenant);
        productChangeVoucherManager.AddChild(AccountingPermissions.ProductChangeVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        productChangeVoucherManager.AddChild(AccountingPermissions.ProductChangeVoucherManagerCreate,
                G(AccountingPermissions.ProductChangeVoucherManagerCreate), MultiTenancySides.Tenant);
        productChangeVoucherManager.AddChild(AccountingPermissions.ProductChangeVoucherManagerUpdate,
                G(AccountingPermissions.ProductChangeVoucherManagerUpdate), MultiTenancySides.Tenant);
        productChangeVoucherManager.AddChild(AccountingPermissions.ProductChangeVoucherManagerDelete,
                G(AccountingPermissions.ProductChangeVoucherManagerDelete), MultiTenancySides.Tenant);
        productChangeVoucherManager.AddChild(AccountingPermissions.ProductChangeVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var assemblyExportVoucherManager = group.AddPermission(AccountingPermissions.AssemblyExportVoucherManager,
                G(AccountingPermissions.AssemblyExportVoucherManager), MultiTenancySides.Tenant);
        assemblyExportVoucherManager.AddChild(AccountingPermissions.AssemblyExportVoucherManagerView,
                G(AccountingPermissions.AssemblyExportVoucherManagerView), MultiTenancySides.Tenant);
        assemblyExportVoucherManager.AddChild(AccountingPermissions.AssemblyExportVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        assemblyExportVoucherManager.AddChild(AccountingPermissions.AssemblyExportVoucherManagerCreate,
                G(AccountingPermissions.AssemblyExportVoucherManagerCreate), MultiTenancySides.Tenant);
        assemblyExportVoucherManager.AddChild(AccountingPermissions.AssemblyExportVoucherManagerUpdate,
                G(AccountingPermissions.AssemblyExportVoucherManagerUpdate), MultiTenancySides.Tenant);
        assemblyExportVoucherManager.AddChild(AccountingPermissions.AssemblyExportVoucherManagerDelete,
                G(AccountingPermissions.AssemblyExportVoucherManagerDelete), MultiTenancySides.Tenant);
        assemblyExportVoucherManager.AddChild(AccountingPermissions.AssemblyExportVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var dismantlingExportVoucherManager = group.AddPermission(AccountingPermissions.DismantlingVoucherManager,
                G(AccountingPermissions.DismantlingVoucherManager), MultiTenancySides.Tenant);
        dismantlingExportVoucherManager.AddChild(AccountingPermissions.DismantlingVoucherManagerView,
                G(AccountingPermissions.DismantlingVoucherManagerView), MultiTenancySides.Tenant);
        dismantlingExportVoucherManager.AddChild(AccountingPermissions.DismantlingVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        dismantlingExportVoucherManager.AddChild(AccountingPermissions.DismantlingVoucherManagerCreate,
                G(AccountingPermissions.DismantlingVoucherManagerCreate), MultiTenancySides.Tenant);
        dismantlingExportVoucherManager.AddChild(AccountingPermissions.DismantlingVoucherManagerUpdate,
                G(AccountingPermissions.DismantlingVoucherManagerUpdate), MultiTenancySides.Tenant);
        dismantlingExportVoucherManager.AddChild(AccountingPermissions.DismantlingVoucherManagerDelete,
                G(AccountingPermissions.DismantlingVoucherManagerDelete), MultiTenancySides.Tenant);
        dismantlingExportVoucherManager.AddChild(AccountingPermissions.DismantlingVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var realityStockOutVoucherManager = group.AddPermission(AccountingPermissions.RealityStockOutVoucherManager,
                G(AccountingPermissions.RealityStockOutVoucherManager), MultiTenancySides.Tenant);
        realityStockOutVoucherManager.AddChild(AccountingPermissions.RealityStockOutVoucherManagerView,
                G(AccountingPermissions.RealityStockOutVoucherManagerView), MultiTenancySides.Tenant);
        realityStockOutVoucherManager.AddChild(AccountingPermissions.RealityStockOutVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        realityStockOutVoucherManager.AddChild(AccountingPermissions.RealityStockOutVoucherManagerCreate,
                G(AccountingPermissions.RealityStockOutVoucherManagerCreate), MultiTenancySides.Tenant);
        realityStockOutVoucherManager.AddChild(AccountingPermissions.RealityStockOutVoucherManagerUpdate,
                G(AccountingPermissions.RealityStockOutVoucherManagerUpdate), MultiTenancySides.Tenant);
        realityStockOutVoucherManager.AddChild(AccountingPermissions.RealityStockOutVoucherManagerDelete,
                G(AccountingPermissions.RealityStockOutVoucherManagerDelete), MultiTenancySides.Tenant);
        realityStockOutVoucherManager.AddChild(AccountingPermissions.RealityStockOutVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var expenseImportVoucherManager = group.AddPermission(AccountingPermissions.ExpenseImportVoucherManager,
                G(AccountingPermissions.ExpenseImportVoucherManager), MultiTenancySides.Tenant);
        expenseImportVoucherManager.AddChild(AccountingPermissions.ExpenseImportVoucherManagerView,
                G(AccountingPermissions.ExpenseImportVoucherManagerView), MultiTenancySides.Tenant);
        expenseImportVoucherManager.AddChild(AccountingPermissions.ExpenseImportVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        expenseImportVoucherManager.AddChild(AccountingPermissions.ExpenseImportVoucherManagerCreate,
                G(AccountingPermissions.ExpenseImportVoucherManagerCreate), MultiTenancySides.Tenant);
        expenseImportVoucherManager.AddChild(AccountingPermissions.ExpenseImportVoucherManagerUpdate,
                G(AccountingPermissions.ExpenseImportVoucherManagerUpdate), MultiTenancySides.Tenant);
        expenseImportVoucherManager.AddChild(AccountingPermissions.ExpenseImportVoucherManagerDelete,
                G(AccountingPermissions.ExpenseImportVoucherManagerDelete), MultiTenancySides.Tenant);
        expenseImportVoucherManager.AddChild(AccountingPermissions.ExpenseImportVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var directExImportVoucherManager = group.AddPermission(AccountingPermissions.DirectExImportVoucherManager,
                G(AccountingPermissions.DirectExImportVoucherManager), MultiTenancySides.Tenant);
        directExImportVoucherManager.AddChild(AccountingPermissions.DirectExImportVoucherManagerView,
                G(AccountingPermissions.DirectExImportVoucherManagerView), MultiTenancySides.Tenant);
        directExImportVoucherManager.AddChild(AccountingPermissions.DirectExImportVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        directExImportVoucherManager.AddChild(AccountingPermissions.DirectExImportVoucherManagerCreate,
                G(AccountingPermissions.DirectExImportVoucherManagerCreate), MultiTenancySides.Tenant);
        directExImportVoucherManager.AddChild(AccountingPermissions.DirectExImportVoucherManagerUpdate,
                G(AccountingPermissions.DirectExImportVoucherManagerUpdate), MultiTenancySides.Tenant);
        directExImportVoucherManager.AddChild(AccountingPermissions.DirectExImportVoucherManagerDelete,
                G(AccountingPermissions.DirectExImportVoucherManagerDelete), MultiTenancySides.Tenant);
        directExImportVoucherManager.AddChild(AccountingPermissions.DirectExImportVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var notWarehouseExImportVoucherManager = group.AddPermission(AccountingPermissions.NotWarehouseExImportVoucherManager,
                G(AccountingPermissions.NotWarehouseExImportVoucherManager), MultiTenancySides.Tenant);
        notWarehouseExImportVoucherManager.AddChild(AccountingPermissions.NotWarehouseExImportVoucherManagerView,
                G(AccountingPermissions.NotWarehouseExImportVoucherManagerView), MultiTenancySides.Tenant);
        notWarehouseExImportVoucherManager.AddChild(AccountingPermissions.NotWarehouseExImportVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        notWarehouseExImportVoucherManager.AddChild(AccountingPermissions.NotWarehouseExImportVoucherManagerCreate,
                G(AccountingPermissions.NotWarehouseExImportVoucherManagerCreate), MultiTenancySides.Tenant);
        notWarehouseExImportVoucherManager.AddChild(AccountingPermissions.NotWarehouseExImportVoucherManagerUpdate,
                G(AccountingPermissions.NotWarehouseExImportVoucherManagerUpdate), MultiTenancySides.Tenant);
        notWarehouseExImportVoucherManager.AddChild(AccountingPermissions.NotWarehouseExImportVoucherManagerDelete,
                G(AccountingPermissions.NotWarehouseExImportVoucherManagerDelete), MultiTenancySides.Tenant);
        notWarehouseExImportVoucherManager.AddChild(AccountingPermissions.NotWarehouseExImportVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var finishedProductVoucherManager = group.AddPermission(AccountingPermissions.FinishedProductVoucherManager,
                G(AccountingPermissions.FinishedProductVoucherManager), MultiTenancySides.Tenant);
        finishedProductVoucherManager.AddChild(AccountingPermissions.FinishedProductVoucherManagerView,
                G(AccountingPermissions.FinishedProductVoucherManagerView), MultiTenancySides.Tenant);
        finishedProductVoucherManager.AddChild(AccountingPermissions.FinishedProductVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        finishedProductVoucherManager.AddChild(AccountingPermissions.FinishedProductVoucherManagerCreate,
                G(AccountingPermissions.FinishedProductVoucherManagerCreate), MultiTenancySides.Tenant);
        finishedProductVoucherManager.AddChild(AccountingPermissions.FinishedProductVoucherManagerUpdate,
                G(AccountingPermissions.FinishedProductVoucherManagerUpdate), MultiTenancySides.Tenant);
        finishedProductVoucherManager.AddChild(AccountingPermissions.FinishedProductVoucherManagerDelete,
                G(AccountingPermissions.FinishedProductVoucherManagerDelete), MultiTenancySides.Tenant);
        finishedProductVoucherManager.AddChild(AccountingPermissions.FinishedProductVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var evictionVoucherManager = group.AddPermission(AccountingPermissions.EvictionVoucherManager,
                G(AccountingPermissions.EvictionVoucherManager), MultiTenancySides.Tenant);
        evictionVoucherManager.AddChild(AccountingPermissions.EvictionVoucherManagerView,
                G(AccountingPermissions.EvictionVoucherManagerView), MultiTenancySides.Tenant);
        evictionVoucherManager.AddChild(AccountingPermissions.EvictionVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        evictionVoucherManager.AddChild(AccountingPermissions.EvictionVoucherManagerCreate,
                G(AccountingPermissions.EvictionVoucherManagerCreate), MultiTenancySides.Tenant);
        evictionVoucherManager.AddChild(AccountingPermissions.EvictionVoucherManagerUpdate,
                G(AccountingPermissions.EvictionVoucherManagerUpdate), MultiTenancySides.Tenant);
        evictionVoucherManager.AddChild(AccountingPermissions.EvictionVoucherManagerDelete,
                G(AccountingPermissions.EvictionVoucherManagerDelete), MultiTenancySides.Tenant);
        evictionVoucherManager.AddChild(AccountingPermissions.EvictionVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var serviceInvoiceVoucherManager = group.AddPermission(AccountingPermissions.ServiceInvoiceVoucherManager,
                G(AccountingPermissions.ServiceInvoiceVoucherManager), MultiTenancySides.Tenant);
        serviceInvoiceVoucherManager.AddChild(AccountingPermissions.ServiceInvoiceVoucherManagerView,
                G(AccountingPermissions.ServiceInvoiceVoucherManagerView), MultiTenancySides.Tenant);
        serviceInvoiceVoucherManager.AddChild(AccountingPermissions.ServiceInvoiceVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        serviceInvoiceVoucherManager.AddChild(AccountingPermissions.ServiceInvoiceVoucherManagerCreate,
                G(AccountingPermissions.ServiceInvoiceVoucherManagerCreate), MultiTenancySides.Tenant);
        serviceInvoiceVoucherManager.AddChild(AccountingPermissions.ServiceInvoiceVoucherManagerUpdate,
                G(AccountingPermissions.ServiceInvoiceVoucherManagerUpdate), MultiTenancySides.Tenant);
        serviceInvoiceVoucherManager.AddChild(AccountingPermissions.ServiceInvoiceVoucherManagerDelete,
                G(AccountingPermissions.ServiceInvoiceVoucherManagerDelete), MultiTenancySides.Tenant);
        serviceInvoiceVoucherManager.AddChild(AccountingPermissions.ServiceInvoiceVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var purchaseOrderManager = group.AddPermission(AccountingPermissions.PurchaseOrderManager,
                G(AccountingPermissions.PurchaseOrderManager), MultiTenancySides.Tenant);
        purchaseOrderManager.AddChild(AccountingPermissions.PurchaseOrderManagerView,
                G(AccountingPermissions.PurchaseOrderManagerView), MultiTenancySides.Tenant);
        purchaseOrderManager.AddChild(AccountingPermissions.PurchaseOrderManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        purchaseOrderManager.AddChild(AccountingPermissions.PurchaseOrderManagerCreate,
                G(AccountingPermissions.PurchaseOrderManagerCreate), MultiTenancySides.Tenant);
        purchaseOrderManager.AddChild(AccountingPermissions.PurchaseOrderManagerUpdate,
                G(AccountingPermissions.PurchaseOrderManagerUpdate), MultiTenancySides.Tenant);
        purchaseOrderManager.AddChild(AccountingPermissions.PurchaseOrderManagerDelete,
                G(AccountingPermissions.PurchaseOrderManagerDelete), MultiTenancySides.Tenant);
        purchaseOrderManager.AddChild(AccountingPermissions.PurchaseOrderManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var quotationManager = group.AddPermission(AccountingPermissions.QuotationManager,
                G(AccountingPermissions.QuotationManager), MultiTenancySides.Tenant);
        quotationManager.AddChild(AccountingPermissions.QuotationManagerView,
                G(AccountingPermissions.QuotationManagerView), MultiTenancySides.Tenant);
        quotationManager.AddChild(AccountingPermissions.QuotationManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        quotationManager.AddChild(AccountingPermissions.QuotationManagerCreate,
                G(AccountingPermissions.QuotationManagerCreate), MultiTenancySides.Tenant);
        quotationManager.AddChild(AccountingPermissions.QuotationManagerUpdate,
                G(AccountingPermissions.QuotationManagerUpdate), MultiTenancySides.Tenant);
        quotationManager.AddChild(AccountingPermissions.QuotationManagerDelete,
                G(AccountingPermissions.QuotationManagerDelete), MultiTenancySides.Tenant);
        quotationManager.AddChild(AccountingPermissions.QuotationManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var salesOrderManager = group.AddPermission(AccountingPermissions.SalesOrderManager,
                G(AccountingPermissions.SalesOrderManager), MultiTenancySides.Tenant);
        salesOrderManager.AddChild(AccountingPermissions.SalesOrderManagerView,
                G(AccountingPermissions.SalesOrderManagerView), MultiTenancySides.Tenant);
        salesOrderManager.AddChild(AccountingPermissions.SalesOrderManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        salesOrderManager.AddChild(AccountingPermissions.SalesOrderManagerCreate,
                G(AccountingPermissions.SalesOrderManagerCreate), MultiTenancySides.Tenant);
        salesOrderManager.AddChild(AccountingPermissions.SalesOrderManagerUpdate,
                G(AccountingPermissions.SalesOrderManagerUpdate), MultiTenancySides.Tenant);
        salesOrderManager.AddChild(AccountingPermissions.SalesOrderManagerDelete,
                G(AccountingPermissions.SalesOrderManagerDelete), MultiTenancySides.Tenant);
        salesOrderManager.AddChild(AccountingPermissions.SalesOrderManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var directSalesVoucherManager = group.AddPermission(AccountingPermissions.DirectSalesVoucherManager,
                G(AccountingPermissions.DirectSalesVoucherManager), MultiTenancySides.Tenant);
        directSalesVoucherManager.AddChild(AccountingPermissions.DirectSalesVoucherManagerView,
                G(AccountingPermissions.DirectSalesVoucherManagerView), MultiTenancySides.Tenant);
        directSalesVoucherManager.AddChild(AccountingPermissions.DirectSalesVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        directSalesVoucherManager.AddChild(AccountingPermissions.DirectSalesVoucherManagerCreate,
                G(AccountingPermissions.DirectSalesVoucherManagerCreate), MultiTenancySides.Tenant);
        directSalesVoucherManager.AddChild(AccountingPermissions.DirectSalesVoucherManagerUpdate,
                G(AccountingPermissions.DirectSalesVoucherManagerUpdate), MultiTenancySides.Tenant);
        directSalesVoucherManager.AddChild(AccountingPermissions.DirectSalesVoucherManagerDelete,
                G(AccountingPermissions.DirectSalesVoucherManagerDelete), MultiTenancySides.Tenant);
        directSalesVoucherManager.AddChild(AccountingPermissions.DirectSalesVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var salesReturnVoucherManager = group.AddPermission(AccountingPermissions.SalesReturnVoucherManager,
                G(AccountingPermissions.SalesReturnVoucherManager), MultiTenancySides.Tenant);
        salesReturnVoucherManager.AddChild(AccountingPermissions.SalesReturnVoucherManagerView,
                G(AccountingPermissions.SalesReturnVoucherManagerView), MultiTenancySides.Tenant);
        salesReturnVoucherManager.AddChild(AccountingPermissions.SalesReturnVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        salesReturnVoucherManager.AddChild(AccountingPermissions.SalesReturnVoucherManagerCreate,
                G(AccountingPermissions.SalesReturnVoucherManagerCreate), MultiTenancySides.Tenant);
        salesReturnVoucherManager.AddChild(AccountingPermissions.SalesReturnVoucherManagerUpdate,
                G(AccountingPermissions.SalesReturnVoucherManagerUpdate), MultiTenancySides.Tenant);
        salesReturnVoucherManager.AddChild(AccountingPermissions.SalesReturnVoucherManagerDelete,
                G(AccountingPermissions.SalesReturnVoucherManagerDelete), MultiTenancySides.Tenant);
        salesReturnVoucherManager.AddChild(AccountingPermissions.SalesReturnVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var voucherPaymentManager = group.AddPermission(AccountingPermissions.VoucherPaymentManager,
                G(AccountingPermissions.VoucherPaymentManager), MultiTenancySides.Tenant);
        voucherPaymentManager.AddChild(AccountingPermissions.VoucherPaymentManagerView,
                G(AccountingPermissions.VoucherPaymentManagerView), MultiTenancySides.Tenant);
        voucherPaymentManager.AddChild(AccountingPermissions.VoucherPaymentManagerCreate,
                G(AccountingPermissions.VoucherPaymentManagerCreate), MultiTenancySides.Tenant);

        var voucherPaymentBeginningManager = group.AddPermission(AccountingPermissions.VoucherPaymentBeginningManager,
                G(AccountingPermissions.VoucherPaymentBeginningManager), MultiTenancySides.Tenant);
        voucherPaymentBeginningManager.AddChild(AccountingPermissions.VoucherPaymentBeginningManagerView,
                G(AccountingPermissions.VoucherPaymentBeginningManagerView), MultiTenancySides.Tenant);
        voucherPaymentBeginningManager.AddChild(AccountingPermissions.VoucherPaymentBeginningManagerCreate,
                G(AccountingPermissions.VoucherPaymentBeginningManagerCreate), MultiTenancySides.Tenant);
        voucherPaymentBeginningManager.AddChild(AccountingPermissions.VoucherPaymentBeginningManagerUpdate,
                G(AccountingPermissions.VoucherPaymentBeginningManagerUpdate), MultiTenancySides.Tenant);
        voucherPaymentBeginningManager.AddChild(AccountingPermissions.VoucherPaymentBeginningManagerDelete,
                G(AccountingPermissions.VoucherPaymentBeginningManagerDelete), MultiTenancySides.Tenant);

        var stockOutPriceManager = group.AddPermission(AccountingPermissions.StockOutPriceManager,
                G(AccountingPermissions.StockOutPriceManager), MultiTenancySides.Tenant);
        stockOutPriceManager.AddChild(AccountingPermissions.StockOutPriceManagerView,
                G(AccountingPermissions.StockOutPriceManagerView), MultiTenancySides.Tenant);
        stockOutPriceManager.AddChild(AccountingPermissions.StockOutPriceManagerCreate,
                G(AccountingPermissions.StockOutPriceManagerCreate), MultiTenancySides.Tenant);
        stockOutPriceManager.AddChild(AccountingPermissions.StockOutPriceManagerUpdate,
                G(AccountingPermissions.StockOutPriceManagerUpdate), MultiTenancySides.Tenant);
        stockOutPriceManager.AddChild(AccountingPermissions.StockOutPriceManagerDelete,
                G(AccountingPermissions.StockOutPriceManagerDelete), MultiTenancySides.Tenant);

        var manager = group.AddPermission(AccountingPermissions.InventoryRecordManager,
                G(AccountingPermissions.InventoryRecordManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.InventoryRecordManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.InventoryRecordManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.InventoryRecordManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.InventoryRecordManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

    }

    private void ConfigGeneralAccVoucherGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.GeneralAccVoucherGroup,
                                        G(AccountingPermissions.GeneralAccVoucherGroup),
                                        MultiTenancySides.Tenant);

        var transferAccVoucherManager = group.AddPermission(AccountingPermissions.TransferAccVoucherManager,
                G(AccountingPermissions.TransferAccVoucherManager), MultiTenancySides.Tenant);
        transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerView,
                G(AccountingPermissions.TransferAccVoucherManagerView), MultiTenancySides.Tenant);
        transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerViewUserNew,
                G(AccountingPermissions.TransferAccVoucherManagerViewUserNew), MultiTenancySides.Tenant);
        transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerCreate,
                G(AccountingPermissions.TransferAccVoucherManagerCreate), MultiTenancySides.Tenant);
        transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerUpdate,
                G(AccountingPermissions.TransferAccVoucherManagerUpdate), MultiTenancySides.Tenant);
        transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerDelete,
                G(AccountingPermissions.TransferAccVoucherManagerDelete), MultiTenancySides.Tenant);
        transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);
        /*transferAccVoucherManager.AddChild(AccountingPermissions.TransferAccVoucherManagerUnRecording,
                G(AccountingPermissions.ActionVoucherUnRecording), MultiTenancySides.Tenant);*/     

        var previousForwardEntryManager = group.AddPermission(AccountingPermissions.PreviousForwardEntryManager,
                G(AccountingPermissions.PreviousForwardEntryManager), MultiTenancySides.Tenant);
        previousForwardEntryManager.AddChild(AccountingPermissions.PreviousForwardEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        previousForwardEntryManager.AddChild(AccountingPermissions.PreviousForwardEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        previousForwardEntryManager.AddChild(AccountingPermissions.PreviousForwardEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        previousForwardEntryManager.AddChild(AccountingPermissions.PreviousForwardEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var ratioAllocateEntryManager = group.AddPermission(AccountingPermissions.RatioAllocateEntryManager,
                G(AccountingPermissions.RatioAllocateEntryManager), MultiTenancySides.Tenant);
        ratioAllocateEntryManager.AddChild(AccountingPermissions.RatioAllocateEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        ratioAllocateEntryManager.AddChild(AccountingPermissions.RatioAllocateEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        ratioAllocateEntryManager.AddChild(AccountingPermissions.RatioAllocateEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        ratioAllocateEntryManager.AddChild(AccountingPermissions.RatioAllocateEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var reduceEntryManager = group.AddPermission(AccountingPermissions.ReduceEntryManager,
                G(AccountingPermissions.ReduceEntryManager), MultiTenancySides.Tenant);
        reduceEntryManager.AddChild(AccountingPermissions.ReduceEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        reduceEntryManager.AddChild(AccountingPermissions.ReduceEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        reduceEntryManager.AddChild(AccountingPermissions.ReduceEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        reduceEntryManager.AddChild(AccountingPermissions.ReduceEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var forwardEntryManager = group.AddPermission(AccountingPermissions.ForwardEntryManager,
                G(AccountingPermissions.ForwardEntryManager), MultiTenancySides.Tenant);
        forwardEntryManager.AddChild(AccountingPermissions.ForwardEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        forwardEntryManager.AddChild(AccountingPermissions.ForwardEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        forwardEntryManager.AddChild(AccountingPermissions.ForwardEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        forwardEntryManager.AddChild(AccountingPermissions.ForwardEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var taxForwardEntryManager = group.AddPermission(AccountingPermissions.TaxForwardEntryManager,
                G(AccountingPermissions.TaxForwardEntryManager), MultiTenancySides.Tenant);
        taxForwardEntryManager.AddChild(AccountingPermissions.TaxForwardEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        taxForwardEntryManager.AddChild(AccountingPermissions.TaxForwardEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        taxForwardEntryManager.AddChild(AccountingPermissions.TaxForwardEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        taxForwardEntryManager.AddChild(AccountingPermissions.TaxForwardEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var businessForwardEntryManager = group.AddPermission(AccountingPermissions.BusinessForwardEntryManager,
                G(AccountingPermissions.BusinessForwardEntryManager), MultiTenancySides.Tenant);
        businessForwardEntryManager.AddChild(AccountingPermissions.BusinessForwardEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        businessForwardEntryManager.AddChild(AccountingPermissions.BusinessForwardEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        businessForwardEntryManager.AddChild(AccountingPermissions.BusinessForwardEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        businessForwardEntryManager.AddChild(AccountingPermissions.BusinessForwardEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var forwardBalanceManager = group.AddPermission(AccountingPermissions.ForwardBalanceManager,
                G(AccountingPermissions.ForwardBalanceManager), MultiTenancySides.Tenant);
        forwardBalanceManager.AddChild(AccountingPermissions.ForwardBalanceManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        forwardBalanceManager.AddChild(AccountingPermissions.ForwardBalanceManagerExecute,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);

        var lockVoucherManager = group.AddPermission(AccountingPermissions.LockVoucherManager,
                G(AccountingPermissions.LockVoucherManager), MultiTenancySides.Tenant);
        lockVoucherManager.AddChild(AccountingPermissions.LockVoucherManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        lockVoucherManager.AddChild(AccountingPermissions.LockVoucherManagerExecute,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);

        var receiveStandardAccManager = group.AddPermission(AccountingPermissions.ReceiveStandardAccManager,
                G(AccountingPermissions.ReceiveStandardAccManager), MultiTenancySides.Tenant);
        receiveStandardAccManager.AddChild(AccountingPermissions.ReceiveStandardAccManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        receiveStandardAccManager.AddChild(AccountingPermissions.ReceiveStandardAccManagerExecute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);

        var maintainceCheckManager = group.AddPermission(AccountingPermissions.MaintainceCheckManager,
                G(AccountingPermissions.MaintainceCheckManager), MultiTenancySides.Tenant);
        maintainceCheckManager.AddChild(AccountingPermissions.MaintainceCheckManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        maintainceCheckManager.AddChild(AccountingPermissions.MaintainceCheckManagerExecute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);

        var exchangeRateManager = group.AddPermission(AccountingPermissions.ExchangeRateManager,
                G(AccountingPermissions.ExchangeRateManager), MultiTenancySides.Tenant);
        exchangeRateManager.AddChild(AccountingPermissions.ExchangeRateManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        exchangeRateManager.AddChild(AccountingPermissions.ExchangeRateManagerExecute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);
    }

    private void ConfigFixedAssetGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.FixedAsset,
                                        G(AccountingPermissions.FixedAsset), MultiTenancySides.Tenant);

        var depreciationAssetVoucherManager = group.AddPermission(AccountingPermissions.DepreciationAssetVoucherManager,
                G(AccountingPermissions.DepreciationAssetVoucherManager), MultiTenancySides.Tenant);
        depreciationAssetVoucherManager.AddChild(AccountingPermissions.DepreciationAssetVoucherManagerView,
                G(AccountingPermissions.DepreciationAssetVoucherManagerView), MultiTenancySides.Tenant);
        depreciationAssetVoucherManager.AddChild(AccountingPermissions.DepreciationAssetVoucherManagerViewUserNew,
                G(AccountingPermissions.DepreciationAssetVoucherManagerViewUserNew), MultiTenancySides.Tenant);
        depreciationAssetVoucherManager.AddChild(AccountingPermissions.DepreciationAssetVoucherManagerCreate,
                G(AccountingPermissions.DepreciationAssetVoucherManagerCreate), MultiTenancySides.Tenant);
        depreciationAssetVoucherManager.AddChild(AccountingPermissions.DepreciationAssetVoucherManagerUpdate,
                G(AccountingPermissions.DepreciationAssetVoucherManagerUpdate), MultiTenancySides.Tenant);
        depreciationAssetVoucherManager.AddChild(AccountingPermissions.DepreciationAssetVoucherManagerDelete,
                G(AccountingPermissions.DepreciationAssetVoucherManagerDelete), MultiTenancySides.Tenant);

        var assetGroupManager = group.AddPermission(AccountingPermissions.AssetGroupManager,
                G(AccountingPermissions.AssetGroupManager), MultiTenancySides.Tenant);
        assetGroupManager.AddChild(AccountingPermissions.AssetGroupManagerView,
                G(AccountingPermissions.AssetGroupManagerView), MultiTenancySides.Tenant);
        assetGroupManager.AddChild(AccountingPermissions.AssetGroupManagerCreate,
                G(AccountingPermissions.AssetGroupManagerCreate), MultiTenancySides.Tenant);
        assetGroupManager.AddChild(AccountingPermissions.AssetGroupManagerUpdate,
                G(AccountingPermissions.AssetGroupManagerUpdate), MultiTenancySides.Tenant);
        assetGroupManager.AddChild(AccountingPermissions.AssetGroupManagerDelete,
                G(AccountingPermissions.AssetGroupManagerDelete), MultiTenancySides.Tenant);

        var purposeManager = group.AddPermission(AccountingPermissions.PurposeManager,
                G(AccountingPermissions.PurposeManager), MultiTenancySides.Tenant);
        purposeManager.AddChild(AccountingPermissions.PurposeManagerView,
                G(AccountingPermissions.PurposeManagerView), MultiTenancySides.Tenant);
        purposeManager.AddChild(AccountingPermissions.PurposeManagerCreate,
                G(AccountingPermissions.PurposeManagerCreate), MultiTenancySides.Tenant);
        purposeManager.AddChild(AccountingPermissions.PurposeManagerUpdate,
                G(AccountingPermissions.PurposeManagerUpdate), MultiTenancySides.Tenant);
        purposeManager.AddChild(AccountingPermissions.PurposeManagerDelete,
                G(AccountingPermissions.PurposeManagerDelete), MultiTenancySides.Tenant);

        var reasonManager = group.AddPermission(AccountingPermissions.ReasonManager,
                G(AccountingPermissions.ReasonManager), MultiTenancySides.Tenant);
        reasonManager.AddChild(AccountingPermissions.ReasonManagerView,
                G(AccountingPermissions.ReasonManagerView), MultiTenancySides.Tenant);
        reasonManager.AddChild(AccountingPermissions.ReasonManagerCreate,
                G(AccountingPermissions.ReasonManagerCreate), MultiTenancySides.Tenant);
        reasonManager.AddChild(AccountingPermissions.ReasonManagerUpdate,
                G(AccountingPermissions.ReasonManagerUpdate), MultiTenancySides.Tenant);
        reasonManager.AddChild(AccountingPermissions.ReasonManagerDelete,
                G(AccountingPermissions.ReasonManagerDelete), MultiTenancySides.Tenant);

        var capitalManager = group.AddPermission(AccountingPermissions.CapitalManager,
                G(AccountingPermissions.CapitalManager), MultiTenancySides.Tenant);
        capitalManager.AddChild(AccountingPermissions.CapitalManagerView,
                G(AccountingPermissions.CapitalManagerView), MultiTenancySides.Tenant);
        capitalManager.AddChild(AccountingPermissions.CapitalManagerCreate,
                G(AccountingPermissions.CapitalManagerCreate), MultiTenancySides.Tenant);
        capitalManager.AddChild(AccountingPermissions.CapitalManagerUpdate,
                G(AccountingPermissions.CapitalManagerUpdate), MultiTenancySides.Tenant);
        capitalManager.AddChild(AccountingPermissions.CapitalManagerDelete,
                G(AccountingPermissions.CapitalManagerDelete), MultiTenancySides.Tenant);

        var assetManager = group.AddPermission(AccountingPermissions.AssetManager,
                G(AccountingPermissions.AssetManager), MultiTenancySides.Tenant);
        assetManager.AddChild(AccountingPermissions.AssetManagerView,
                G(AccountingPermissions.AssetManagerView), MultiTenancySides.Tenant);
        assetManager.AddChild(AccountingPermissions.AssetManagerCreate,
                G(AccountingPermissions.AssetManagerCreate), MultiTenancySides.Tenant);
        assetManager.AddChild(AccountingPermissions.AssetManagerUpdate,
                G(AccountingPermissions.AssetManagerUpdate), MultiTenancySides.Tenant);
        assetManager.AddChild(AccountingPermissions.AssetManagerDelete,
                G(AccountingPermissions.AssetManagerDelete), MultiTenancySides.Tenant);

        var adjustDepreciationManager = group.AddPermission(AccountingPermissions.AdjustDepreciationManager,
                G(AccountingPermissions.AdjustDepreciationManager), MultiTenancySides.Tenant);
        adjustDepreciationManager.AddChild(AccountingPermissions.AdjustDepreciationManagerView,
                G(AccountingPermissions.AdjustDepreciationManagerView), MultiTenancySides.Tenant);
        adjustDepreciationManager.AddChild(AccountingPermissions.AdjustDepreciationManagerCreate,
                G(AccountingPermissions.AdjustDepreciationManagerCreate), MultiTenancySides.Tenant);
        adjustDepreciationManager.AddChild(AccountingPermissions.AdjustDepreciationManagerUpdate,
                G(AccountingPermissions.AdjustDepreciationManagerUpdate), MultiTenancySides.Tenant);
        adjustDepreciationManager.AddChild(AccountingPermissions.AdjustDepreciationManagerDelete,
                G(AccountingPermissions.AdjustDepreciationManagerDelete), MultiTenancySides.Tenant);

        var assetDepreciationAccountingManager = group.AddPermission(AccountingPermissions.AssetDepreciationAccountingManager,
                G(AccountingPermissions.AssetDepreciationAccountingManager), MultiTenancySides.Tenant);
        assetDepreciationAccountingManager.AddChild(AccountingPermissions.AssetDepreciationAccountingManagerView,
                G(AccountingPermissions.AssetDepreciationAccountingManagerView), MultiTenancySides.Tenant);
        assetDepreciationAccountingManager.AddChild(AccountingPermissions.AssetDepreciationAccountingManagerUpdate,
                G(AccountingPermissions.AssetDepreciationAccountingManagerUpdate), MultiTenancySides.Tenant);

        var assetDepreciationManager = group.AddPermission(AccountingPermissions.AssetDepreciationManager,
                G(AccountingPermissions.AssetDepreciationManager), MultiTenancySides.Tenant);
        assetDepreciationManager.AddChild(AccountingPermissions.AssetDepreciationManagerView,
                G(AccountingPermissions.AssetDepreciationManagerView), MultiTenancySides.Tenant);
        assetDepreciationManager.AddChild(AccountingPermissions.AssetDepreciationManagerExcute,
                G(AccountingPermissions.AssetDepreciationManagerExcute), MultiTenancySides.Tenant);
    }
    private void ConfigAllocateToolGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.AllocateTool,
                                        G(AccountingPermissions.AllocateTool), MultiTenancySides.Tenant);

        var allocateToolVoucherManager = group.AddPermission(AccountingPermissions.AllocateToolVoucherManager,
                G(AccountingPermissions.AllocateToolVoucherManager), MultiTenancySides.Tenant);
        allocateToolVoucherManager.AddChild(AccountingPermissions.AllocateToolVoucherManagerView,
                G(AccountingPermissions.AllocateToolVoucherManagerView), MultiTenancySides.Tenant);
        allocateToolVoucherManager.AddChild(AccountingPermissions.AllocateToolVoucherManagerViewUserNew,
                G(AccountingPermissions.AllocateToolVoucherManagerViewUserNew), MultiTenancySides.Tenant);
        allocateToolVoucherManager.AddChild(AccountingPermissions.AllocateToolVoucherManagerCreate,
                G(AccountingPermissions.AllocateToolVoucherManagerCreate), MultiTenancySides.Tenant);
        allocateToolVoucherManager.AddChild(AccountingPermissions.AllocateToolVoucherManagerUpdate,
                G(AccountingPermissions.AllocateToolVoucherManagerUpdate), MultiTenancySides.Tenant);
        allocateToolVoucherManager.AddChild(AccountingPermissions.AllocateToolVoucherManagerDelete,
                G(AccountingPermissions.AllocateToolVoucherManagerDelete), MultiTenancySides.Tenant);

        var toolGroupManager = group.AddPermission(AccountingPermissions.ToolGroupManager,
                G(AccountingPermissions.ToolGroupManager), MultiTenancySides.Tenant);
        toolGroupManager.AddChild(AccountingPermissions.ToolGroupManagerView,
                G(AccountingPermissions.ToolGroupManagerView), MultiTenancySides.Tenant);
        toolGroupManager.AddChild(AccountingPermissions.ToolGroupManagerCreate,
                G(AccountingPermissions.ToolGroupManagerCreate), MultiTenancySides.Tenant);
        toolGroupManager.AddChild(AccountingPermissions.ToolGroupManagerUpdate,
                G(AccountingPermissions.ToolGroupManagerUpdate), MultiTenancySides.Tenant);
        toolGroupManager.AddChild(AccountingPermissions.ToolGroupManagerDelete,
                G(AccountingPermissions.ToolGroupManagerDelete), MultiTenancySides.Tenant);

        var toolsManager = group.AddPermission(AccountingPermissions.ToolsManager,
                G(AccountingPermissions.ToolsManager), MultiTenancySides.Tenant);
        toolsManager.AddChild(AccountingPermissions.ToolsManagerView,
                G(AccountingPermissions.ToolsManagerView), MultiTenancySides.Tenant);
        toolsManager.AddChild(AccountingPermissions.ToolsManagerCreate,
                G(AccountingPermissions.ToolsManagerCreate), MultiTenancySides.Tenant);
        toolsManager.AddChild(AccountingPermissions.ToolsManagerUpdate,
                G(AccountingPermissions.ToolsManagerUpdate), MultiTenancySides.Tenant);
        toolsManager.AddChild(AccountingPermissions.ToolsManagerDelete,
                G(AccountingPermissions.ToolsManagerDelete), MultiTenancySides.Tenant);

        var adjustAllocationManager = group.AddPermission(AccountingPermissions.AdjustAllocationManager,
                G(AccountingPermissions.AdjustAllocationManager), MultiTenancySides.Tenant);
        adjustAllocationManager.AddChild(AccountingPermissions.AdjustAllocationManagerView,
                G(AccountingPermissions.AdjustAllocationManagerView), MultiTenancySides.Tenant);
        adjustAllocationManager.AddChild(AccountingPermissions.AdjustAllocationManagerCreate,
                G(AccountingPermissions.AdjustAllocationManagerCreate), MultiTenancySides.Tenant);
        adjustAllocationManager.AddChild(AccountingPermissions.AdjustAllocationManagerUpdate,
                G(AccountingPermissions.AdjustAllocationManagerUpdate), MultiTenancySides.Tenant);
        adjustAllocationManager.AddChild(AccountingPermissions.AdjustAllocationManagerDelete,
                G(AccountingPermissions.AdjustAllocationManagerDelete), MultiTenancySides.Tenant);

        var toolAllocationAccountingManager = group.AddPermission(AccountingPermissions.ToolAllocationAccountingManager,
                G(AccountingPermissions.ToolAllocationAccountingManager), MultiTenancySides.Tenant);
        toolAllocationAccountingManager.AddChild(AccountingPermissions.ToolAllocationAccountingManagerView,
                G(AccountingPermissions.ToolAllocationAccountingManagerView), MultiTenancySides.Tenant);
        toolAllocationAccountingManager.AddChild(AccountingPermissions.ToolAllocationAccountingManagerUpdate,
                G(AccountingPermissions.ToolAllocationAccountingManagerUpdate), MultiTenancySides.Tenant);

        var toolAllocationManager = group.AddPermission(AccountingPermissions.ToolAllocationManager,
                G(AccountingPermissions.ToolAllocationManager), MultiTenancySides.Tenant);
        toolAllocationManager.AddChild(AccountingPermissions.ToolAllocationManagerView,
                G(AccountingPermissions.ToolAllocationManagerView), MultiTenancySides.Tenant);
        toolAllocationManager.AddChild(AccountingPermissions.ToolAllocationManagerExcute,
                G(AccountingPermissions.ToolAllocationManagerExcute), MultiTenancySides.Tenant);
    }
    private void ConfigProductCostGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.ProductCost,
                                G(AccountingPermissions.ProductCost), MultiTenancySides.Tenant);
        
        var transferCostVoucherManager = group.AddPermission(AccountingPermissions.TransferCostVoucherManager,
                G(AccountingPermissions.TransferCostVoucherManager), MultiTenancySides.Tenant);
        transferCostVoucherManager.AddChild(AccountingPermissions.TransferCostVoucherManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        transferCostVoucherManager.AddChild(AccountingPermissions.TransferCostVoucherManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        transferCostVoucherManager.AddChild(AccountingPermissions.TransferCostVoucherManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        transferCostVoucherManager.AddChild(AccountingPermissions.TransferCostVoucherManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        transferCostVoucherManager.AddChild(AccountingPermissions.TransferCostVoucherManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);
        transferCostVoucherManager.AddChild(AccountingPermissions.TransferCostVoucherManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);

        var finishedProductQuotaManager = group.AddPermission(AccountingPermissions.FinishedProductQuotaManager,
                G(AccountingPermissions.FinishedProductQuotaManager), MultiTenancySides.Tenant);
        finishedProductQuotaManager.AddChild(AccountingPermissions.FinishedProductQuotaManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        finishedProductQuotaManager.AddChild(AccountingPermissions.FinishedProductQuotaManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        finishedProductQuotaManager.AddChild(AccountingPermissions.FinishedProductQuotaManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        finishedProductQuotaManager.AddChild(AccountingPermissions.FinishedProductQuotaManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var groupCoefficientManager = group.AddPermission(AccountingPermissions.GroupCoefficientManager,
                G(AccountingPermissions.GroupCoefficientManager), MultiTenancySides.Tenant);
        groupCoefficientManager.AddChild(AccountingPermissions.GroupCoefficientManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        groupCoefficientManager.AddChild(AccountingPermissions.GroupCoefficientManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        groupCoefficientManager.AddChild(AccountingPermissions.GroupCoefficientManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        groupCoefficientManager.AddChild(AccountingPermissions.GroupCoefficientManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var groupCoefficientWorkManager = group.AddPermission(AccountingPermissions.GroupCoefficientWorkManager,
                G(AccountingPermissions.GroupCoefficientWorkManager), MultiTenancySides.Tenant);
        groupCoefficientWorkManager.AddChild(AccountingPermissions.GroupCoefficientWorkManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        groupCoefficientWorkManager.AddChild(AccountingPermissions.GroupCoefficientWorkManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        groupCoefficientWorkManager.AddChild(AccountingPermissions.GroupCoefficientWorkManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        groupCoefficientWorkManager.AddChild(AccountingPermissions.GroupCoefficientWorkManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var pExpenseForwardAccEntryManager = group.AddPermission(AccountingPermissions.PExpenseForwardAccEntryManager,
                G(AccountingPermissions.PExpenseForwardAccEntryManager), MultiTenancySides.Tenant);
        pExpenseForwardAccEntryManager.AddChild(AccountingPermissions.PExpenseForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        pExpenseForwardAccEntryManager.AddChild(AccountingPermissions.PExpenseForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        pExpenseForwardAccEntryManager.AddChild(AccountingPermissions.PExpenseForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        pExpenseForwardAccEntryManager.AddChild(AccountingPermissions.PExpenseForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var pQuotaForwardAccEntryManager = group.AddPermission(AccountingPermissions.PQuotaForwardAccEntryManager,
                G(AccountingPermissions.PQuotaForwardAccEntryManager), MultiTenancySides.Tenant);
        pQuotaForwardAccEntryManager.AddChild(AccountingPermissions.PQuotaForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        pQuotaForwardAccEntryManager.AddChild(AccountingPermissions.PQuotaForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        pQuotaForwardAccEntryManager.AddChild(AccountingPermissions.PQuotaForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        pQuotaForwardAccEntryManager.AddChild(AccountingPermissions.PQuotaForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var pCoefficientForwardAccEntryManager = group.AddPermission(AccountingPermissions.PCoefficientForwardAccEntryManager,
                G(AccountingPermissions.PCoefficientForwardAccEntryManager), MultiTenancySides.Tenant);
        pCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.PCoefficientForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        pCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.PCoefficientForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        pCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.PCoefficientForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        pCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.PCoefficientForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var pRatioForwardAccEntryManager = group.AddPermission(AccountingPermissions.PRatioForwardAccEntryManager,
                G(AccountingPermissions.PRatioForwardAccEntryManager), MultiTenancySides.Tenant);
        pRatioForwardAccEntryManager.AddChild(AccountingPermissions.PRatioForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        pRatioForwardAccEntryManager.AddChild(AccountingPermissions.PRatioForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        pRatioForwardAccEntryManager.AddChild(AccountingPermissions.PRatioForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        pRatioForwardAccEntryManager.AddChild(AccountingPermissions.PRatioForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var pBookThzManager = group.AddPermission(AccountingPermissions.PBookThzManager,
                G(AccountingPermissions.PBookThzManager), MultiTenancySides.Tenant);
        pBookThzManager.AddChild(AccountingPermissions.PBookThzManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        pBookThzManager.AddChild(AccountingPermissions.PBookThzManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        pBookThzManager.AddChild(AccountingPermissions.PBookThzManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        pBookThzManager.AddChild(AccountingPermissions.PBookThzManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var wBookThzManager = group.AddPermission(AccountingPermissions.WBookThzManager,
                G(AccountingPermissions.WBookThzManager), MultiTenancySides.Tenant);
        wBookThzManager.AddChild(AccountingPermissions.WBookThzManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        wBookThzManager.AddChild(AccountingPermissions.WBookThzManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        wBookThzManager.AddChild(AccountingPermissions.WBookThzManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        wBookThzManager.AddChild(AccountingPermissions.WBookThzManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var unFinishedProductManager = group.AddPermission(AccountingPermissions.UnFinishedProductManager,
                G(AccountingPermissions.UnFinishedProductManager), MultiTenancySides.Tenant);
        unFinishedProductManager.AddChild(AccountingPermissions.UnFinishedProductManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        unFinishedProductManager.AddChild(AccountingPermissions.UnFinishedProductManagerViewUserNew,
                G(AccountingPermissions.ActionViewByUserNew), MultiTenancySides.Tenant);
        unFinishedProductManager.AddChild(AccountingPermissions.UnFinishedProductManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        unFinishedProductManager.AddChild(AccountingPermissions.UnFinishedProductManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        unFinishedProductManager.AddChild(AccountingPermissions.UnFinishedProductManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);
        unFinishedProductManager.AddChild(AccountingPermissions.UnFinishedProductManagerRecording,
                G(AccountingPermissions.ActionVoucherRecording), MultiTenancySides.Tenant);


        var wExpenseForwardAccEntryManager = group.AddPermission(AccountingPermissions.WExpenseForwardAccEntryManager,
                G(AccountingPermissions.WExpenseForwardAccEntryManager), MultiTenancySides.Tenant);
        wExpenseForwardAccEntryManager.AddChild(AccountingPermissions.WExpenseForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        wExpenseForwardAccEntryManager.AddChild(AccountingPermissions.WExpenseForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        wExpenseForwardAccEntryManager.AddChild(AccountingPermissions.WExpenseForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        wExpenseForwardAccEntryManager.AddChild(AccountingPermissions.WExpenseForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var wCoefficientForwardAccEntryManager = group.AddPermission(AccountingPermissions.WCoefficientForwardAccEntryManager,
                G(AccountingPermissions.WCoefficientForwardAccEntryManager), MultiTenancySides.Tenant);
        wCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.WCoefficientForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        wCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.WCoefficientForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        wCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.WCoefficientForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        wCoefficientForwardAccEntryManager.AddChild(AccountingPermissions.WCoefficientForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var wRatioForwardAccEntryManager = group.AddPermission(AccountingPermissions.WRatioForwardAccEntryManager,
                G(AccountingPermissions.WRatioForwardAccEntryManager), MultiTenancySides.Tenant);
        wRatioForwardAccEntryManager.AddChild(AccountingPermissions.WRatioForwardAccEntryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        wRatioForwardAccEntryManager.AddChild(AccountingPermissions.WRatioForwardAccEntryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        wRatioForwardAccEntryManager.AddChild(AccountingPermissions.WRatioForwardAccEntryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        wRatioForwardAccEntryManager.AddChild(AccountingPermissions.WRatioForwardAccEntryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var calcProductCostManager = group.AddPermission(AccountingPermissions.CalcProductCostManager,
                G(AccountingPermissions.CalcProductCostManager), MultiTenancySides.Tenant);
        calcProductCostManager.AddChild(AccountingPermissions.CalcProductCostManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        calcProductCostManager.AddChild(AccountingPermissions.CalcProductCostManagerExcute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);
        calcProductCostManager.AddChild(AccountingPermissions.CalcProductCostManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);

        var calcWorkCostManager = group.AddPermission(AccountingPermissions.CalcWorkCostManager,
                G(AccountingPermissions.CalcWorkCostManager), MultiTenancySides.Tenant);
        calcWorkCostManager.AddChild(AccountingPermissions.CalcWorkCostManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        calcWorkCostManager.AddChild(AccountingPermissions.CalcWorkCostManagerExcute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);
        calcWorkCostManager.AddChild(AccountingPermissions.CalcWorkCostManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);

        var updateCostPriceManager = group.AddPermission(AccountingPermissions.UpdateCostPriceManager,
                G(AccountingPermissions.UpdateCostPriceManager), MultiTenancySides.Tenant);
        updateCostPriceManager.AddChild(AccountingPermissions.UpdateCostPriceManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        updateCostPriceManager.AddChild(AccountingPermissions.UpdateCostPriceManagerExcute,
                G(AccountingPermissions.ActionExcute), MultiTenancySides.Tenant);
    }

    private void ConfigDashboardGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.Dashboard,
                                        G(AccountingPermissions.Dashboard), MultiTenancySides.Tenant);
        var dashboardManager = group.AddPermission(AccountingPermissions.DashboardManager,
                G(AccountingPermissions.DashboardManager), MultiTenancySides.Tenant);
        dashboardManager.AddChild(AccountingPermissions.RevenueDashboard,
                G(AccountingPermissions.RevenueDashboard), MultiTenancySides.Tenant);
        dashboardManager.AddChild(AccountingPermissions.ExpenseDashboard,
                G(AccountingPermissions.ExpenseDashboard), MultiTenancySides.Tenant);
        dashboardManager.AddChild(AccountingPermissions.FinanceDashboard,
                G(AccountingPermissions.FinanceDashboard), MultiTenancySides.Tenant);
        dashboardManager.AddChild(AccountingPermissions.InventoryDashboard,
                G(AccountingPermissions.InventoryDashboard), MultiTenancySides.Tenant);
        dashboardManager.AddChild(AccountingPermissions.ReceivableDashboard,
                G(AccountingPermissions.ReceivableDashboard), MultiTenancySides.Tenant);
        dashboardManager.AddChild(AccountingPermissions.LiabilityDashboard,
                G(AccountingPermissions.LiabilityDashboard), MultiTenancySides.Tenant);
    }

    private void ConfigInvoiceGroup(IPermissionDefinitionContext context)
    {
        var invoiceGroup = context.AddGroup(AccountingPermissions.Invoice,
                                        G(AccountingPermissions.Invoice),
                                        MultiTenancySides.Tenant);

        var invoiceStatusManager = invoiceGroup.AddPermission(AccountingPermissions.InvoiceStatusManager,
                G(AccountingPermissions.InvoiceStatusManager), MultiTenancySides.Tenant);
        invoiceStatusManager.AddChild(AccountingPermissions.InvoiceStatusManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        invoiceStatusManager.AddChild(AccountingPermissions.InvoiceStatusManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        invoiceStatusManager.AddChild(AccountingPermissions.InvoiceStatusManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        invoiceStatusManager.AddChild(AccountingPermissions.InvoiceStatusManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var invoiceSupplierManager = invoiceGroup.AddPermission(AccountingPermissions.InvoiceSupplierManager,
                G(AccountingPermissions.InvoiceSupplierManager), MultiTenancySides.Tenant);
        invoiceSupplierManager.AddChild(AccountingPermissions.InvoiceSupplierManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        invoiceSupplierManager.AddChild(AccountingPermissions.InvoiceSupplierManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        invoiceSupplierManager.AddChild(AccountingPermissions.InvoiceSupplierManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        invoiceSupplierManager.AddChild(AccountingPermissions.InvoiceSupplierManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var vatInvoiceManager = invoiceGroup.AddPermission(AccountingPermissions.VatInvoiceManager,
                G(AccountingPermissions.VatInvoiceManager), MultiTenancySides.Tenant);
        vatInvoiceManager.AddChild(AccountingPermissions.VatInvoiceManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        vatInvoiceManager.AddChild(AccountingPermissions.VatInvoiceManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        vatInvoiceManager.AddChild(AccountingPermissions.VatInvoiceManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        vatInvoiceManager.AddChild(AccountingPermissions.VatInvoiceManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);
    }
    private void ConfigSalaryGroup(IPermissionDefinitionContext context)
    {
        var group = context.AddGroup(AccountingPermissions.Salary,
                                        G(AccountingPermissions.Salary),
                                        MultiTenancySides.Tenant);

        var positionManager = group.AddPermission(AccountingPermissions.PositionManager,
                G(AccountingPermissions.PositionManager), MultiTenancySides.Tenant);
        positionManager.AddChild(AccountingPermissions.PositionManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        positionManager.AddChild(AccountingPermissions.PositionManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        positionManager.AddChild(AccountingPermissions.PositionManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        positionManager.AddChild(AccountingPermissions.PositionManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        var manager = group.AddPermission(AccountingPermissions.EmployeeManager,
                G(AccountingPermissions.EmployeeManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.EmployeeManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.EmployeeManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.EmployeeManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.EmployeeManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        manager = group.AddPermission(AccountingPermissions.SalaryPeriodManager,
                G(AccountingPermissions.SalaryPeriodManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryPeriodManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryPeriodManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryPeriodManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryPeriodManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        manager = group.AddPermission(AccountingPermissions.SalaryCategoryManager,
                G(AccountingPermissions.SalaryCategoryManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryCategoryManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryCategoryManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryCategoryManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryCategoryManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        manager = group.AddPermission(AccountingPermissions.SalaryEmployeeManager,
                G(AccountingPermissions.SalaryEmployeeManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryEmployeeManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryEmployeeManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryEmployeeManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalaryEmployeeManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        manager = group.AddPermission(AccountingPermissions.SalarySheetTypeManager,
                G(AccountingPermissions.SalarySheetTypeManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetTypeManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetTypeManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetTypeManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetTypeManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);

        manager = group.AddPermission(AccountingPermissions.SalarySheetManager,
                G(AccountingPermissions.SalarySheetManager), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetManagerView,
                G(AccountingPermissions.ActionView), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetManagerCreate,
                G(AccountingPermissions.ActionCreate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetManagerUpdate,
                G(AccountingPermissions.ActionUpdate), MultiTenancySides.Tenant);
        manager.AddChild(AccountingPermissions.SalarySheetManagerDelete,
                G(AccountingPermissions.ActionDelete), MultiTenancySides.Tenant);
    }
    private LocalizableString G(string name)
    {
        return L($"{AccountingPermissions.Prefix}:{name}");
    }
    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<AccountingResource>(name);
    }
}
