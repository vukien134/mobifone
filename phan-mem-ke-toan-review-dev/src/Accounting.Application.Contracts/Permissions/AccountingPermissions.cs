using System.Runtime.CompilerServices;

namespace Accounting.Permissions;

public static class AccountingPermissions
{
    public const string GroupName = "Accounting";
    public const string Prefix = "Permission";

    public const string ActionView = "View";
    public const string ActionCreate = "Create";
    public const string ActionUpdate = "Update";
    public const string ActionDelete = "Delete";
    public const string ActionPrint = "Print";
    public const string ActionExcute = "Excute";
    public const string ActionSave = "Save";
    public const string ActionDashboard = "Dashboard";
    public const string ActionVoucherRecording = "VoucherRecording";
    public const string ActionVoucherUnRecording = "VoucherUnRecording";
    public const string ActionViewByUserNew = "ViewByUserNew";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public const string CategoryGroup = "Category";
    public const string BalanceGroup = "Balance";
    public const string SystemGroup = "System";
    public const string PermissionSubGroup = "Permission";
    public const string AccVoucherGroup = "AccVoucher";
    public const string ProductVoucherGroup = "ProductVoucher";
    public const string GeneralAccVoucherGroup = "GeneralAccVoucher";
    public const string FixedAsset = "FixedAsset";
    public const string AllocateTool = "AllocateTool";
    public const string ProductCost = "ProductCost";
    public const string Dashboard = "Dashboard";
    public const string Invoice = "Invoice";
    public const string Salary = "Salary";

    public const string RoleManager = "Role_Management";
    public const string RoleManagerView = RoleManager + "_" + ActionView;
    public const string RoleManagerCreate = RoleManager + "_Create";
    public const string RoleManagerUpdate = RoleManager + "_Update";
    public const string RoleManagerDelete = RoleManager + "_Delete";
    public const string RoleManagerAssign = RoleManager + "_Assign";

    public const string UserManager = "User_Management";
    public const string UserManagerView = UserManager + "_" + ActionView;
    public const string UserManagerCreate = UserManager + "_Create";
    public const string UserManagerUpdate = UserManager + "_Update";
    public const string UserManagerDelete = UserManager + "_Delete";

    public const string AccountSystemManager = "AccountSystem_Management";
    public const string AccountSystemManagerView = AccountSystemManager + "_" + ActionView;
    public const string AccountSystemManagerCreate = AccountSystemManager + $"_{ActionCreate}";
    public const string AccountSystemManagerUpdate = AccountSystemManager + $"_{ActionUpdate}";
    public const string AccountSystemManagerDelete = AccountSystemManager + $"_{ActionDelete}";

    public const string CaseManager = "Case_Management";
    public const string CaseManagerView = CaseManager + "_" + ActionView;
    public const string CaseManagerCreate = CaseManager + "_Create";
    public const string CaseManagerUpdate = CaseManager + "_Update";
    public const string CaseManagerDelete = CaseManager + "_Delete";

    public const string PersonManager = "Person_Management";
    public const string PersonManagerView = PersonManager + "_" + ActionView;
    public const string PersonManagerCreate = PersonManager + "_Create";
    public const string PersonManagerUpdate = PersonManager + "_Update";
    public const string PersonManagerDelete = PersonManager + "_Delete";

    public const string SectionManager = "Section_Management";
    public const string SectionManagerView = SectionManager + "_" + ActionView;
    public const string SectionManagerCreate = SectionManager + "_Create";
    public const string SectionManagerUpdate = SectionManager + "_Update";
    public const string SectionManagerDelete = SectionManager + "_Delete";


    public const string OrgUnitManager = "OrgUnit_Management";
    public const string OrgUnitManagerView = OrgUnitManager + "_" + ActionView;
    public const string OrgUnitManagerCreate = OrgUnitManager + "_Create";
    public const string OrgUnitManagerUpdate = OrgUnitManager + "_Update";
    public const string OrgUnitManagerDelete = OrgUnitManager + "_Delete";

    public const string PartnerGroupManager = "PartnerGroup_Management";
    public const string PartnerGroupManagerView = PartnerGroupManager + "_" + ActionView;
    public const string PartnerGroupManagerCreate = PartnerGroupManager + "_Create";
    public const string PartnerGroupManagerUpdate = PartnerGroupManager + "_Update";
    public const string PartnerGroupManagerDelete = PartnerGroupManager + "_Delete";

    public const string PartnerManager = "Partner_Management";
    public const string PartnerManagerView = PartnerManager + "_" + ActionView;
    public const string PartnerManagerCreate = PartnerManager + "_Create";
    public const string PartnerManagerUpdate = PartnerManager + "_Update";
    public const string PartnerManagerDelete = PartnerManager + "_Delete";

    public const string YearCategoryManager = "YearCategory_Management";
    public const string YearCategoryManagerView = YearCategoryManager + "_" + ActionView;
    public const string YearCategoryManagerCreate = YearCategoryManager + $"_{ActionCreate}";
    public const string YearCategoryManagerUpdate = YearCategoryManager + $"_{ActionUpdate}";
    public const string YearCategoryManagerDelete = YearCategoryManager + $"_{ActionDelete}";

    public const string TenantSettingManager = "TenantSetting_Management";
    public const string TenantSettingManagerView = TenantSettingManager + "_" + ActionView;
    public const string TenantSettingManagerCreate = TenantSettingManager + $"_{ActionCreate}";
    public const string TenantSettingManagerUpdate = TenantSettingManager + $"_{ActionUpdate}";
    public const string TenantSettingManagerDelete = TenantSettingManager + $"_{ActionDelete}";

    public const string WorkPlaceManager = "WorkPlace_Management";
    public const string WorkPlaceManagerView = WorkPlaceManager + "_" + ActionView;
    public const string WorkPlaceManagerCreate = WorkPlaceManager + $"_{ActionCreate}";
    public const string WorkPlaceManagerUpdate = WorkPlaceManager + $"_{ActionUpdate}";
    public const string WorkPlaceManagerDelete = WorkPlaceManager + $"_{ActionDelete}";

    public const string SaleChannelManager = "SaleChannel_Management";
    public const string SaleChannelManagerView = SaleChannelManager + "_" + ActionView;
    public const string SaleChannelManagerCreate = SaleChannelManager + $"_{ActionCreate}";
    public const string SaleChannelManagerUpdate = SaleChannelManager + $"_{ActionUpdate}";
    public const string SaleChannelManagerDelete = SaleChannelManager + $"_{ActionDelete}";

    public const string ProductGroupManager = "ProductGroup_Management";
    public const string ProductGroupManagerView = ProductGroupManager + "_" + ActionView;
    public const string ProductGroupManagerCreate = ProductGroupManager + $"_{ActionCreate}";
    public const string ProductGroupManagerUpdate = ProductGroupManager + $"_{ActionUpdate}";
    public const string ProductGroupManagerDelete = ProductGroupManager + $"_{ActionDelete}";

    public const string ProductManager = "Product_Management";
    public const string ProductManagerView = ProductManager + "_" + ActionView;
    public const string ProductManagerCreate = ProductManager + $"_{ActionCreate}";
    public const string ProductManagerUpdate = ProductManager + $"_{ActionUpdate}";
    public const string ProductManagerDelete = ProductManager + $"_{ActionDelete}";

    public const string ExciseTaxManager = "ExciseTax_Management";
    public const string ExciseTaxManagerView = ExciseTaxManager + "_" + ActionView;
    public const string ExciseTaxManagerCreate = ExciseTaxManager + $"_{ActionCreate}";
    public const string ExciseTaxManagerUpdate = ExciseTaxManager + $"_{ActionUpdate}";
    public const string ExciseTaxManagerDelete = ExciseTaxManager + $"_{ActionDelete}";

    public const string ContractManager = "Contract_Management";
    public const string ContractManagerView = ContractManager + "_" + ActionView;
    public const string ContractManagerCreate = ContractManager + $"_{ActionCreate}";
    public const string ContractManagerUpdate = ContractManager + $"_{ActionUpdate}";
    public const string ContractManagerDelete = ContractManager + $"_{ActionDelete}";

    public const string ProductLotManager = "ProductLot_Management";
    public const string ProductLotManagerView = ProductLotManager + "_" + ActionView;
    public const string ProductLotManagerCreate = ProductLotManager + $"_{ActionCreate}";
    public const string ProductLotManagerUpdate = ProductLotManager + $"_{ActionUpdate}";
    public const string ProductLotManagerDelete = ProductLotManager + $"_{ActionDelete}";

    public const string ProductOriginManager = "ProductOrigin_Management";
    public const string ProductOriginManagerView = ProductOriginManager + "_" + ActionView;
    public const string ProductOriginManagerCreate = ProductOriginManager + $"_{ActionCreate}";
    public const string ProductOriginManagerUpdate = ProductOriginManager + $"_{ActionUpdate}";
    public const string ProductOriginManagerDelete = ProductOriginManager + $"_{ActionDelete}";

    public const string ProductionPeriodManager = "ProductionPeriod_Management";
    public const string ProductionPeriodManagerView = ProductionPeriodManager + "_" + ActionView;
    public const string ProductionPeriodManagerCreate = ProductionPeriodManager + $"_{ActionCreate}";
    public const string ProductionPeriodManagerUpdate = ProductionPeriodManager + $"_{ActionUpdate}";
    public const string ProductionPeriodManagerDelete = ProductionPeriodManager + $"_{ActionDelete}";

    public const string UnitManager = "Unit_Management";
    public const string UnitManagerView = UnitManager + "_" + ActionView;
    public const string UnitManagerCreate = UnitManager + $"_{ActionCreate}";
    public const string UnitManagerUpdate = UnitManager + $"_{ActionUpdate}";
    public const string UnitManagerDelete = UnitManager + $"_{ActionDelete}";

    public const string DepartmentManager = "Department_Management";
    public const string DepartmentManagerView = DepartmentManager + "_" + ActionView;
    public const string DepartmentManagerCreate = DepartmentManager + $"_{ActionCreate}";
    public const string DepartmentManagerUpdate = DepartmentManager + $"_{ActionUpdate}";
    public const string DepartmentManagerDelete = DepartmentManager + $"_{ActionDelete}";

    public const string WarehouseManager = "Warehouse_Management";
    public const string WarehouseManagerView = WarehouseManager + "_" + ActionView;
    public const string WarehouseManagerCreate = WarehouseManager + $"_{ActionCreate}";
    public const string WarehouseManagerUpdate = WarehouseManager + $"_{ActionUpdate}";
    public const string WarehouseManagerDelete = WarehouseManager + $"_{ActionDelete}";

    public const string TaxCategoryManager = "TaxCategory_Management";
    public const string TaxCategoryManagerView = TaxCategoryManager + "_" + ActionView;
    public const string TaxCategoryManagerCreate = TaxCategoryManager + $"_{ActionCreate}";
    public const string TaxCategoryManagerUpdate = TaxCategoryManager + $"_{ActionUpdate}";
    public const string TaxCategoryManagerDelete = TaxCategoryManager + $"_{ActionDelete}";

    public const string PaymentTermManager = "PaymentTerm_Management";
    public const string PaymentTermManagerView = PaymentTermManager + "_" + ActionView;
    public const string PaymentTermManagerCreate = PaymentTermManager + $"_{ActionCreate}";
    public const string PaymentTermManagerUpdate = PaymentTermManager + $"_{ActionUpdate}";
    public const string PaymentTermManagerDelete = PaymentTermManager + $"_{ActionDelete}";

    public const string FProductWorkManager = "FProductWork_Management";
    public const string FProductWorkManagerView = FProductWorkManager + "_" + ActionView;
    public const string FProductWorkManagerCreate = FProductWorkManager + $"_{ActionCreate}";
    public const string FProductWorkManagerUpdate = FProductWorkManager + $"_{ActionUpdate}";
    public const string FProductWorkManagerDelete = FProductWorkManager + $"_{ActionDelete}";

    public const string BusinessCategoryManager = "BusinessCategory_Management";
    public const string BusinessCategoryManagerView = BusinessCategoryManager + "_" + ActionView;
    public const string BusinessCategoryManagerCreate = BusinessCategoryManager + $"_{ActionCreate}";
    public const string BusinessCategoryManagerUpdate = BusinessCategoryManager + $"_{ActionUpdate}";
    public const string BusinessCategoryManagerDelete = BusinessCategoryManager + $"_{ActionDelete}";

    public const string InvoiceBookManager = "InvoiceBook_Management";
    public const string InvoiceBookManagerView = InvoiceBookManager + "_" + ActionView;
    public const string InvoiceBookManagerCreate = InvoiceBookManager + $"_{ActionCreate}";
    public const string InvoiceBookManagerUpdate = InvoiceBookManager + $"_{ActionUpdate}";
    public const string InvoiceBookManagerDelete = InvoiceBookManager + $"_{ActionDelete}";

    public const string DiscountPriceManager = "DiscountPrice_Management";
    public const string DiscountPriceManagerView = DiscountPriceManager + "_" + ActionView;
    public const string DiscountPriceManagerCreate = DiscountPriceManager + $"_{ActionCreate}";
    public const string DiscountPriceManagerUpdate = DiscountPriceManager + $"_{ActionUpdate}";
    public const string DiscountPriceManagerDelete = DiscountPriceManager + $"_{ActionDelete}";

    public const string CurrencyManager = "Currency_Management";
    public const string CurrencyManagerView = CurrencyManager + "_" + ActionView;
    public const string CurrencyManagerCreate = CurrencyManager + $"_{ActionCreate}";
    public const string CurrencyManagerUpdate = CurrencyManager + $"_{ActionUpdate}";
    public const string CurrencyManagerDelete = CurrencyManager + $"_{ActionDelete}";

    public const string FeeTypeManager = "FeeType_Management";
    public const string FeeTypeManagerView = FeeTypeManager + "_" + ActionView;
    public const string FeeTypeManagerCreate = FeeTypeManager + $"_{ActionCreate}";
    public const string FeeTypeManagerUpdate = FeeTypeManager + $"_{ActionUpdate}";
    public const string FeeTypeManagerDelete = FeeTypeManager + $"_{ActionDelete}";

    public const string BarcodeManager = "Barcode_Management";
    public const string BarcodeManagerView = BarcodeManager + $"_{ActionView}";
    public const string BarcodeManagerPrint = BarcodeManager + $"_{ActionPrint}";

    public const string AccountOpeningBalanceManager = "AccountOpeningBalance_Management";
    public const string AccountOpeningBalanceManagerView = AccountOpeningBalanceManager + $"_{ActionView}";
    public const string AccountOpeningBalanceManagerCreate = AccountOpeningBalanceManager + $"_{ActionCreate}";
    public const string AccountOpeningBalanceManagerUpdate = AccountOpeningBalanceManager + $"_{ActionUpdate}";
    public const string AccountOpeningBalanceManagerDelete = AccountOpeningBalanceManager + $"_{ActionDelete}";

    public const string ProductOpeningBalanceManager = "ProductOpeningBalance_Management";
    public const string ProductOpeningBalanceManagerView = ProductOpeningBalanceManager + $"_{ActionView}";
    public const string ProductOpeningBalanceManagerCreate = ProductOpeningBalanceManager + $"_{ActionCreate}";
    public const string ProductOpeningBalanceManagerUpdate = ProductOpeningBalanceManager + $"_{ActionUpdate}";
    public const string ProductOpeningBalanceManagerDelete = ProductOpeningBalanceManager + $"_{ActionDelete}";

    public const string ReceiptVoucherManager = "ReceiptVoucher_Management";
    public const string ReceiptVoucherManagerView = ReceiptVoucherManager + $"_{ActionView}";
    public const string ReceiptVoucherManagerViewUserNew = ReceiptVoucherManager + $"_{ActionViewByUserNew}";
    public const string ReceiptVoucherManagerCreate = ReceiptVoucherManager + $"_{ActionCreate}";
    public const string ReceiptVoucherManagerUpdate = ReceiptVoucherManager + $"_{ActionUpdate}";
    public const string ReceiptVoucherManagerDelete = ReceiptVoucherManager + $"_{ActionDelete}";
    public const string ReceiptVoucherManagerRecording = ReceiptVoucherManager + $"_{ActionVoucherRecording}";
    public const string ReceiptVoucherManagerUnRecording = ReceiptVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string PaymentVoucherManager = "PaymentVoucher_Management";
    public const string PaymentVoucherManagerView = PaymentVoucherManager + $"_{ActionView}";
    public const string PaymentVoucherManagerViewUserNew = PaymentVoucherManager + $"_{ActionViewByUserNew}";
    public const string PaymentVoucherManagerCreate = PaymentVoucherManager + $"_{ActionCreate}";
    public const string PaymentVoucherManagerUpdate = PaymentVoucherManager + $"_{ActionUpdate}";
    public const string PaymentVoucherManagerDelete = PaymentVoucherManager + $"_{ActionDelete}";
    public const string PaymentVoucherManagerRecording = PaymentVoucherManager + $"_{ActionVoucherRecording}";
    public const string PaymentVoucherManagerUnRecording = PaymentVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string CreditNoteVoucherManager = "CreditNoteVoucher_Management";
    public const string CreditNoteVoucherManagerView = CreditNoteVoucherManager + $"_{ActionView}";
    public const string CreditNoteVoucherManagerViewUserNew = CreditNoteVoucherManager + $"_{ActionViewByUserNew}";
    public const string CreditNoteVoucherManagerCreate = CreditNoteVoucherManager + $"_{ActionCreate}";
    public const string CreditNoteVoucherManagerUpdate = CreditNoteVoucherManager + $"_{ActionUpdate}";
    public const string CreditNoteVoucherManagerDelete = CreditNoteVoucherManager + $"_{ActionDelete}";
    public const string CreditNoteVoucherManagerRecording = CreditNoteVoucherManager + $"_{ActionVoucherRecording}";
    public const string CreditNoteVoucherManagerUnRecording = CreditNoteVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string DebitNoteVoucherManager = "DebitNoteVoucher_Management";
    public const string DebitNoteVoucherManagerView = DebitNoteVoucherManager + $"_{ActionView}";
    public const string DebitNoteVoucherManagerViewUserNew = DebitNoteVoucherManager + $"_{ActionViewByUserNew}";
    public const string DebitNoteVoucherManagerCreate = DebitNoteVoucherManager + $"_{ActionCreate}";
    public const string DebitNoteVoucherManagerUpdate = DebitNoteVoucherManager + $"_{ActionUpdate}";
    public const string DebitNoteVoucherManagerDelete = DebitNoteVoucherManager + $"_{ActionDelete}";
    public const string DebitNoteVoucherManagerRecording = DebitNoteVoucherManager + $"_{ActionVoucherRecording}";
    public const string DebitNoteVoucherManagerUnRecording = DebitNoteVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string OtherVoucherManager = "OtherVoucher_Management";
    public const string OtherVoucherManagerView = OtherVoucherManager + $"_{ActionView}";
    public const string OtherVoucherManagerViewUserNew = OtherVoucherManager + $"_{ActionViewByUserNew}";
    public const string OtherVoucherManagerCreate = OtherVoucherManager + $"_{ActionCreate}";
    public const string OtherVoucherManagerUpdate = OtherVoucherManager + $"_{ActionUpdate}";
    public const string OtherVoucherManagerDelete = OtherVoucherManager + $"_{ActionDelete}";
    public const string OtherVoucherManagerRecording = OtherVoucherManager + $"_{ActionVoucherRecording}";
    public const string OtherVoucherManagerUnRecording = OtherVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string RefundVoucherManager = "RefundVoucher_Management";
    public const string RefundVoucherManagerView = RefundVoucherManager + $"_{ActionView}";
    public const string RefundVoucherManagerViewUserNew = RefundVoucherManager + $"_{ActionViewByUserNew}";
    public const string RefundVoucherManagerCreate = RefundVoucherManager + $"_{ActionCreate}";
    public const string RefundVoucherManagerUpdate = RefundVoucherManager + $"_{ActionUpdate}";
    public const string RefundVoucherManagerDelete = RefundVoucherManager + $"_{ActionDelete}";
    public const string RefundVoucherManagerRecoding = RefundVoucherManager + $"_{ActionVoucherRecording}";
    public const string RefundVoucherManagerUnRecoding = RefundVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string TaxVoucherManager = "TaxVoucher_Management";
    public const string TaxVoucherManagerView = TaxVoucherManager + $"_{ActionView}";
    public const string TaxVoucherManagerViewUserNew = TaxVoucherManager + $"_{ActionViewByUserNew}";
    public const string TaxVoucherManagerCreate = TaxVoucherManager + $"_{ActionCreate}";
    public const string TaxVoucherManagerUpdate = TaxVoucherManager + $"_{ActionUpdate}";
    public const string TaxVoucherManagerDelete = TaxVoucherManager + $"_{ActionDelete}";
    public const string TaxVoucherManagerRecording = TaxVoucherManager + $"_{ActionVoucherRecording}";
    public const string TaxVoucherManagerUnRecording = TaxVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string DeptCleaningVoucherManager = "DeptCleaningVoucher_Management";
    public const string DeptCleaningVoucherManagerView = DeptCleaningVoucherManager + $"_{ActionView}";
    public const string DeptCleaningVoucherManagerViewUserNew = DeptCleaningVoucherManager + $"_{ActionViewByUserNew}";
    public const string DeptCleaningVoucherManagerCreate = DeptCleaningVoucherManager + $"_{ActionCreate}";
    public const string DeptCleaningVoucherManagerUpdate = DeptCleaningVoucherManager + $"_{ActionUpdate}";
    public const string DeptCleaningVoucherManagerDelete = DeptCleaningVoucherManager + $"_{ActionDelete}";
    public const string DeptCleaningVoucherManagerRecording = DeptCleaningVoucherManager + $"_{ActionVoucherRecording}";
    public const string DeptCleaningVoucherManagerUnRecording = DeptCleaningVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string TransferAccVoucherManager = "TransferAccVoucher_Management";
    public const string TransferAccVoucherManagerView = TransferAccVoucherManager + $"_{ActionView}";
    public const string TransferAccVoucherManagerViewUserNew = TransferAccVoucherManager + $"_{ActionViewByUserNew}";
    public const string TransferAccVoucherManagerCreate = TransferAccVoucherManager + $"_{ActionCreate}";
    public const string TransferAccVoucherManagerUpdate = TransferAccVoucherManager + $"_{ActionUpdate}";
    public const string TransferAccVoucherManagerDelete = TransferAccVoucherManager + $"_{ActionDelete}";
    public const string TransferAccVoucherManagerRecording = TransferAccVoucherManager + $"_{ActionVoucherRecording}";

    public const string TransferCostVoucherManager = "TransferCostVoucher_Management";
    public const string TransferCostVoucherManagerView = TransferCostVoucherManager + $"_{ActionView}";
    public const string TransferCostVoucherManagerViewUserNew = TransferCostVoucherManager + $"_{ActionViewByUserNew}";
    public const string TransferCostVoucherManagerCreate = TransferCostVoucherManager + $"_{ActionCreate}";
    public const string TransferCostVoucherManagerUpdate = TransferCostVoucherManager + $"_{ActionUpdate}";
    public const string TransferCostVoucherManagerDelete = TransferCostVoucherManager + $"_{ActionDelete}";
    public const string TransferCostVoucherManagerRecording = TransferCostVoucherManager + $"_{ActionVoucherRecording}";
    /*    public const string TransferAccVoucherManagerUnRecording = TransferAccVoucherManager + $"_{ActionVoucherUnRecording}";*/

    public const string WarehouseReceiptVoucherManager = "WarehouseReceiptVoucher_Management";
    public const string WarehouseReceiptVoucherManagerView = WarehouseReceiptVoucherManager + $"_{ActionView}";
    public const string WarehouseReceiptVoucherManagerViewUserNew = WarehouseReceiptVoucherManager + $"_{ActionViewByUserNew}";
    public const string WarehouseReceiptVoucherManagerCreate = WarehouseReceiptVoucherManager + $"_{ActionCreate}";
    public const string WarehouseReceiptVoucherManagerUpdate = WarehouseReceiptVoucherManager + $"_{ActionUpdate}";
    public const string WarehouseReceiptVoucherManagerDelete = WarehouseReceiptVoucherManager + $"_{ActionDelete}";
    public const string WarehouseReceiptVoucherManagerRecording = WarehouseReceiptVoucherManager + $"_{ActionVoucherRecording}";
    public const string WarehouseReceiptVoucherManagerUnRecording = WarehouseReceiptVoucherManager + $"_{ActionVoucherUnRecording}";

    public const string StockOutVoucherManager = "StockOutVoucher_Management";
    public const string StockOutVoucherManagerView = StockOutVoucherManager + $"_{ActionView}";
    public const string StockOutVoucherManagerViewUserNew = StockOutVoucherManager + $"_{ActionViewByUserNew}";
    public const string StockOutVoucherManagerCreate = StockOutVoucherManager + $"_{ActionCreate}";
    public const string StockOutVoucherManagerUpdate = StockOutVoucherManager + $"_{ActionUpdate}";
    public const string StockOutVoucherManagerDelete = StockOutVoucherManager + $"_{ActionDelete}";
    public const string StockOutVoucherManagerRecording = StockOutVoucherManager + $"_{ActionVoucherRecording}";

    public const string DepreciationAssetVoucherManager = "DepreciationAssetVoucher_Management";
    public const string DepreciationAssetVoucherManagerView = DepreciationAssetVoucherManager + $"_{ActionView}";
    public const string DepreciationAssetVoucherManagerViewUserNew = DepreciationAssetVoucherManager + $"_{ActionViewByUserNew}";
    public const string DepreciationAssetVoucherManagerCreate = DepreciationAssetVoucherManager + $"_{ActionCreate}";
    public const string DepreciationAssetVoucherManagerUpdate = DepreciationAssetVoucherManager + $"_{ActionUpdate}";
    public const string DepreciationAssetVoucherManagerDelete = DepreciationAssetVoucherManager + $"_{ActionDelete}";
    public const string DepreciationAssetVoucherManagerRecording = DepreciationAssetVoucherManager + $"_{ActionVoucherRecording}";

    public const string AllocateToolVoucherManager = "AllocateToolVoucher_Management";
    public const string AllocateToolVoucherManagerView = AllocateToolVoucherManager + $"_{ActionView}";
    public const string AllocateToolVoucherManagerViewUserNew = AllocateToolVoucherManager + $"_{ActionViewByUserNew}";
    public const string AllocateToolVoucherManagerCreate = AllocateToolVoucherManager + $"_{ActionCreate}";
    public const string AllocateToolVoucherManagerUpdate = AllocateToolVoucherManager + $"_{ActionUpdate}";
    public const string AllocateToolVoucherManagerDelete = AllocateToolVoucherManager + $"_{ActionDelete}";
    public const string AllocateToolVoucherManagerRecording = AllocateToolVoucherManager + $"_{ActionVoucherRecording}";

    public const string PurchaseReturnsVoucherManager = "PurchaseReturnsVoucher_Management";
    public const string PurchaseReturnsVoucherManagerView = PurchaseReturnsVoucherManager + $"_{ActionView}";
    public const string PurchaseReturnsVoucherManagerViewUserNew = PurchaseReturnsVoucherManager + $"_{ActionViewByUserNew}";
    public const string PurchaseReturnsVoucherManagerCreate = PurchaseReturnsVoucherManager + $"_{ActionCreate}";
    public const string PurchaseReturnsVoucherManagerUpdate = PurchaseReturnsVoucherManager + $"_{ActionUpdate}";
    public const string PurchaseReturnsVoucherManagerDelete = PurchaseReturnsVoucherManager + $"_{ActionDelete}";
    public const string PurchaseReturnsVoucherManagerRecording = PurchaseReturnsVoucherManager + $"_{ActionVoucherRecording}";

    public const string SalesVoucherManager = "SalesVoucher_Management";
    public const string SalesVoucherManagerView = SalesVoucherManager + $"_{ActionView}";
    public const string SalesVoucherManagerViewUserNew = SalesVoucherManager + $"_{ActionViewByUserNew}";
    public const string SalesVoucherManagerCreate = SalesVoucherManager + $"_{ActionCreate}";
    public const string SalesVoucherManagerUpdate = SalesVoucherManager + $"_{ActionUpdate}";
    public const string SalesVoucherManagerDelete = SalesVoucherManager + $"_{ActionDelete}";
    public const string SalesVoucherManagerRecording = SalesVoucherManager + $"_{ActionVoucherRecording}";

    public const string SalesBarcodeVoucherManager = "SalesBarcodeVoucher_Management";
    public const string SalesBarcodeVoucherManagerView = SalesBarcodeVoucherManager + $"_{ActionView}";
    public const string SalesBarcodeVoucherManagerViewUserNew = SalesBarcodeVoucherManager + $"_{ActionViewByUserNew}";
    public const string SalesBarcodeVoucherManagerCreate = SalesBarcodeVoucherManager + $"_{ActionCreate}";
    public const string SalesBarcodeVoucherManagerUpdate = SalesBarcodeVoucherManager + $"_{ActionUpdate}";
    public const string SalesBarcodeVoucherManagerDelete = SalesBarcodeVoucherManager + $"_{ActionDelete}";
    public const string SalesBarcodeVoucherManagerRecording = SalesBarcodeVoucherManager + $"_{ActionVoucherRecording}";

    public const string ImportsVoucherManager = "ImportsVoucher_Management";
    public const string ImportsVoucherManagerView = ImportsVoucherManager + $"_{ActionView}";
    public const string ImportsVoucherManagerViewUserNew = ImportsVoucherManager + $"_{ActionViewByUserNew}";
    public const string ImportsVoucherManagerCreate = ImportsVoucherManager + $"_{ActionCreate}";
    public const string ImportsVoucherManagerUpdate = ImportsVoucherManager + $"_{ActionUpdate}";
    public const string ImportsVoucherManagerDelete = ImportsVoucherManager + $"_{ActionDelete}";
    public const string ImportsVoucherManagerRecording = ImportsVoucherManager + $"_{ActionVoucherRecording}";

    public const string ProductTransferVoucherManager = "ProductTransferVoucher_Management";
    public const string ProductTransferVoucherManagerView = ProductTransferVoucherManager + $"_{ActionView}";
    public const string ProductTransferVoucherManagerViewUserNew = ProductTransferVoucherManager + $"_{ActionViewByUserNew}";
    public const string ProductTransferVoucherManagerCreate = ProductTransferVoucherManager + $"_{ActionCreate}";
    public const string ProductTransferVoucherManagerUpdate = ProductTransferVoucherManager + $"_{ActionUpdate}";
    public const string ProductTransferVoucherManagerDelete = ProductTransferVoucherManager + $"_{ActionDelete}";
    public const string ProductTransferVoucherManagerRecording = ProductTransferVoucherManager + $"_{ActionVoucherRecording}";

    public const string ProductChangeVoucherManager = "ProductChangeVoucher_Management";
    public const string ProductChangeVoucherManagerView = ProductChangeVoucherManager + $"_{ActionView}";
    public const string ProductChangeVoucherManagerViewUserNew = ProductChangeVoucherManager + $"_{ActionViewByUserNew}";
    public const string ProductChangeVoucherManagerCreate = ProductChangeVoucherManager + $"_{ActionCreate}";
    public const string ProductChangeVoucherManagerUpdate = ProductChangeVoucherManager + $"_{ActionUpdate}";
    public const string ProductChangeVoucherManagerDelete = ProductChangeVoucherManager + $"_{ActionDelete}";
    public const string ProductChangeVoucherManagerRecording = ProductChangeVoucherManager + $"_{ActionVoucherRecording}";

    public const string AssemblyExportVoucherManager = "AssemblyExportVoucher_Management";
    public const string AssemblyExportVoucherManagerView = AssemblyExportVoucherManager + $"_{ActionView}";
    public const string AssemblyExportVoucherManagerViewUserNew = AssemblyExportVoucherManager + $"_{ActionViewByUserNew}";
    public const string AssemblyExportVoucherManagerCreate = AssemblyExportVoucherManager + $"_{ActionCreate}";
    public const string AssemblyExportVoucherManagerUpdate = AssemblyExportVoucherManager + $"_{ActionUpdate}";
    public const string AssemblyExportVoucherManagerDelete = AssemblyExportVoucherManager + $"_{ActionDelete}";
    public const string AssemblyExportVoucherManagerRecording = AssemblyExportVoucherManager + $"_{ActionVoucherRecording}";

    public const string DismantlingVoucherManager = "DismantlingVoucher_Management";
    public const string DismantlingVoucherManagerView = DismantlingVoucherManager + $"_{ActionView}";
    public const string DismantlingVoucherManagerViewUserNew = DismantlingVoucherManager + $"_{ActionViewByUserNew}";
    public const string DismantlingVoucherManagerCreate = DismantlingVoucherManager + $"_{ActionCreate}";
    public const string DismantlingVoucherManagerUpdate = DismantlingVoucherManager + $"_{ActionUpdate}";
    public const string DismantlingVoucherManagerDelete = DismantlingVoucherManager + $"_{ActionDelete}";
    public const string DismantlingVoucherManagerRecording = DismantlingVoucherManager + $"_{ActionVoucherRecording}";

    public const string RealityStockOutVoucherManager = "RealityStockOutVoucher_Management";
    public const string RealityStockOutVoucherManagerView = RealityStockOutVoucherManager + $"_{ActionView}";
    public const string RealityStockOutVoucherManagerViewUserNew = RealityStockOutVoucherManager + $"_{ActionViewByUserNew}";
    public const string RealityStockOutVoucherManagerCreate = RealityStockOutVoucherManager + $"_{ActionCreate}";
    public const string RealityStockOutVoucherManagerUpdate = RealityStockOutVoucherManager + $"_{ActionUpdate}";
    public const string RealityStockOutVoucherManagerDelete = RealityStockOutVoucherManager + $"_{ActionDelete}";
    public const string RealityStockOutVoucherManagerRecording = RealityStockOutVoucherManager + $"_{ActionVoucherRecording}";

    public const string ExpenseImportVoucherManager = "ExpenseImportVoucher_Management";
    public const string ExpenseImportVoucherManagerView = ExpenseImportVoucherManager + $"_{ActionView}";
    public const string ExpenseImportVoucherManagerViewUserNew = ExpenseImportVoucherManager + $"_{ActionViewByUserNew}";
    public const string ExpenseImportVoucherManagerCreate = ExpenseImportVoucherManager + $"_{ActionCreate}";
    public const string ExpenseImportVoucherManagerUpdate = ExpenseImportVoucherManager + $"_{ActionUpdate}";
    public const string ExpenseImportVoucherManagerDelete = ExpenseImportVoucherManager + $"_{ActionDelete}";
    public const string ExpenseImportVoucherManagerRecording = ExpenseImportVoucherManager + $"_{ActionVoucherRecording}";

    public const string DirectExImportVoucherManager = "DirectExImportVoucher_Management";
    public const string DirectExImportVoucherManagerView = DirectExImportVoucherManager + $"_{ActionView}";
    public const string DirectExImportVoucherManagerViewUserNew = DirectExImportVoucherManager + $"_{ActionViewByUserNew}";
    public const string DirectExImportVoucherManagerCreate = DirectExImportVoucherManager + $"_{ActionCreate}";
    public const string DirectExImportVoucherManagerUpdate = DirectExImportVoucherManager + $"_{ActionUpdate}";
    public const string DirectExImportVoucherManagerDelete = DirectExImportVoucherManager + $"_{ActionDelete}";
    public const string DirectExImportVoucherManagerRecording = DirectExImportVoucherManager + $"_{ActionVoucherRecording}";

    public const string NotWarehouseExImportVoucherManager = "NotWarehouseExImportVoucher_Management";
    public const string NotWarehouseExImportVoucherManagerView = NotWarehouseExImportVoucherManager + $"_{ActionView}";
    public const string NotWarehouseExImportVoucherManagerViewUserNew = NotWarehouseExImportVoucherManager + $"_{ActionViewByUserNew}";
    public const string NotWarehouseExImportVoucherManagerCreate = NotWarehouseExImportVoucherManager + $"_{ActionCreate}";
    public const string NotWarehouseExImportVoucherManagerUpdate = NotWarehouseExImportVoucherManager + $"_{ActionUpdate}";
    public const string NotWarehouseExImportVoucherManagerDelete = NotWarehouseExImportVoucherManager + $"_{ActionDelete}";
    public const string NotWarehouseExImportVoucherManagerRecording = NotWarehouseExImportVoucherManager + $"_{ActionVoucherRecording}";

    public const string FinishedProductVoucherManager = "FinishedProductVoucher_Management";
    public const string FinishedProductVoucherManagerView = FinishedProductVoucherManager + $"_{ActionView}";
    public const string FinishedProductVoucherManagerViewUserNew = FinishedProductVoucherManager + $"_{ActionViewByUserNew}";
    public const string FinishedProductVoucherManagerCreate = FinishedProductVoucherManager + $"_{ActionCreate}";
    public const string FinishedProductVoucherManagerUpdate = FinishedProductVoucherManager + $"_{ActionUpdate}";
    public const string FinishedProductVoucherManagerDelete = FinishedProductVoucherManager + $"_{ActionDelete}";
    public const string FinishedProductVoucherManagerRecording = FinishedProductVoucherManager + $"_{ActionVoucherRecording}";

    public const string EvictionVoucherManager = "EvictionVoucher_Management";
    public const string EvictionVoucherManagerView = EvictionVoucherManager + $"_{ActionView}";
    public const string EvictionVoucherManagerViewUserNew = EvictionVoucherManager + $"_{ActionViewByUserNew}";
    public const string EvictionVoucherManagerCreate = EvictionVoucherManager + $"_{ActionCreate}";
    public const string EvictionVoucherManagerUpdate = EvictionVoucherManager + $"_{ActionUpdate}";
    public const string EvictionVoucherManagerDelete = EvictionVoucherManager + $"_{ActionDelete}";
    public const string EvictionVoucherManagerRecording = EvictionVoucherManager + $"_{ActionVoucherRecording}";

    public const string ServiceInvoiceVoucherManager = "ServiceInvoiceVoucher_Management";
    public const string ServiceInvoiceVoucherManagerView = ServiceInvoiceVoucherManager + $"_{ActionView}";
    public const string ServiceInvoiceVoucherManagerViewUserNew = ServiceInvoiceVoucherManager + $"_{ActionViewByUserNew}";
    public const string ServiceInvoiceVoucherManagerCreate = ServiceInvoiceVoucherManager + $"_{ActionCreate}";
    public const string ServiceInvoiceVoucherManagerUpdate = ServiceInvoiceVoucherManager + $"_{ActionUpdate}";
    public const string ServiceInvoiceVoucherManagerDelete = ServiceInvoiceVoucherManager + $"_{ActionDelete}";
    public const string ServiceInvoiceVoucherManagerRecording = ServiceInvoiceVoucherManager + $"_{ActionVoucherRecording}";

    public const string PurchaseOrderManager = "PurchaseOrder_Management";
    public const string PurchaseOrderManagerView = PurchaseOrderManager + $"_{ActionView}";
    public const string PurchaseOrderManagerViewUserNew = PurchaseOrderManager + $"_{ActionViewByUserNew}";
    public const string PurchaseOrderManagerCreate = PurchaseOrderManager + $"_{ActionCreate}";
    public const string PurchaseOrderManagerUpdate = PurchaseOrderManager + $"_{ActionUpdate}";
    public const string PurchaseOrderManagerDelete = PurchaseOrderManager + $"_{ActionDelete}";
    public const string PurchaseOrderManagerRecording = PurchaseOrderManager + $"_{ActionVoucherRecording}";

    public const string QuotationManager = "Quotation_Management";
    public const string QuotationManagerView = QuotationManager + $"_{ActionView}";
    public const string QuotationManagerViewUserNew = QuotationManager + $"_{ActionViewByUserNew}";
    public const string QuotationManagerCreate = QuotationManager + $"_{ActionCreate}";
    public const string QuotationManagerUpdate = QuotationManager + $"_{ActionUpdate}";
    public const string QuotationManagerDelete = QuotationManager + $"_{ActionDelete}";
    public const string QuotationManagerRecording = QuotationManager + $"_{ActionVoucherRecording}";

    public const string SalesOrderManager = "SalesOrder_Management";
    public const string SalesOrderManagerView = SalesOrderManager + $"_{ActionView}";
    public const string SalesOrderManagerViewUserNew = SalesOrderManager + $"_{ActionViewByUserNew}";
    public const string SalesOrderManagerCreate = SalesOrderManager + $"_{ActionCreate}";
    public const string SalesOrderManagerUpdate = SalesOrderManager + $"_{ActionUpdate}";
    public const string SalesOrderManagerDelete = SalesOrderManager + $"_{ActionDelete}";
    public const string SalesOrderManagerRecording = SalesOrderManager + $"_{ActionVoucherRecording}";

    public const string DirectSalesVoucherManager = "DirectSalesVoucher_Management";
    public const string DirectSalesVoucherManagerView = DirectSalesVoucherManager + $"_{ActionView}";
    public const string DirectSalesVoucherManagerViewUserNew = DirectSalesVoucherManager + $"_{ActionViewByUserNew}";
    public const string DirectSalesVoucherManagerCreate = DirectSalesVoucherManager + $"_{ActionCreate}";
    public const string DirectSalesVoucherManagerUpdate = DirectSalesVoucherManager + $"_{ActionUpdate}";
    public const string DirectSalesVoucherManagerDelete = DirectSalesVoucherManager + $"_{ActionDelete}";
    public const string DirectSalesVoucherManagerRecording = DirectSalesVoucherManager + $"_{ActionVoucherRecording}";

    public const string SalesReturnVoucherManager = "SalesReturnVoucher_Management";
    public const string SalesReturnVoucherManagerView = SalesReturnVoucherManager + $"_{ActionView}";
    public const string SalesReturnVoucherManagerViewUserNew = SalesReturnVoucherManager + $"_{ActionViewByUserNew}";
    public const string SalesReturnVoucherManagerCreate = SalesReturnVoucherManager + $"_{ActionCreate}";
    public const string SalesReturnVoucherManagerUpdate = SalesReturnVoucherManager + $"_{ActionUpdate}";
    public const string SalesReturnVoucherManagerDelete = SalesReturnVoucherManager + $"_{ActionDelete}";
    public const string SalesReturnVoucherManagerRecording = SalesReturnVoucherManager + $"_{ActionVoucherRecording}";

    public const string VoucherPaymentManager = "VoucherPayment_Management";
    public const string VoucherPaymentManagerView = VoucherPaymentManager + $"_{ActionView}";
    public const string VoucherPaymentManagerCreate = VoucherPaymentManager + $"_{ActionCreate}";

    public const string VoucherPaymentBeginningManager = "VoucherPaymentBeginning_Management";
    public const string VoucherPaymentBeginningManagerView = VoucherPaymentBeginningManager + $"_{ActionView}";
    public const string VoucherPaymentBeginningManagerCreate = VoucherPaymentBeginningManager + $"_{ActionCreate}";
    public const string VoucherPaymentBeginningManagerUpdate = VoucherPaymentBeginningManager + $"_{ActionUpdate}";
    public const string VoucherPaymentBeginningManagerDelete = VoucherPaymentBeginningManager + $"_{ActionDelete}";

    public const string StockOutPriceManager = "StockOutPrice_Management";
    public const string StockOutPriceManagerView = StockOutPriceManager + $"_{ActionView}";
    public const string StockOutPriceManagerCreate = StockOutPriceManager + $"_{ActionCreate}";
    public const string StockOutPriceManagerUpdate = StockOutPriceManager + $"_{ActionUpdate}";
    public const string StockOutPriceManagerDelete = StockOutPriceManager + $"_{ActionDelete}";

    public const string AssetGroupManager = "AssetGroup_Management";
    public const string AssetGroupManagerView = AssetGroupManager + $"_{ActionView}";
    public const string AssetGroupManagerCreate = AssetGroupManager + $"_{ActionCreate}";
    public const string AssetGroupManagerUpdate = AssetGroupManager + $"_{ActionUpdate}";
    public const string AssetGroupManagerDelete = AssetGroupManager + $"_{ActionDelete}";

    public const string ToolGroupManager = "ToolGroup_Management";
    public const string ToolGroupManagerView = ToolGroupManager + $"_{ActionView}";
    public const string ToolGroupManagerCreate = ToolGroupManager + $"_{ActionCreate}";
    public const string ToolGroupManagerUpdate = ToolGroupManager + $"_{ActionUpdate}";
    public const string ToolGroupManagerDelete = ToolGroupManager + $"_{ActionDelete}";

    public const string PurposeManager = "Purpose_Management";
    public const string PurposeManagerView = PurposeManager + $"_{ActionView}";
    public const string PurposeManagerCreate = PurposeManager + $"_{ActionCreate}";
    public const string PurposeManagerUpdate = PurposeManager + $"_{ActionUpdate}";
    public const string PurposeManagerDelete = PurposeManager + $"_{ActionDelete}";

    public const string ReasonManager = "Reason_Management";
    public const string ReasonManagerView = ReasonManager + $"_{ActionView}";
    public const string ReasonManagerCreate = ReasonManager + $"_{ActionCreate}";
    public const string ReasonManagerUpdate = ReasonManager + $"_{ActionUpdate}";
    public const string ReasonManagerDelete = ReasonManager + $"_{ActionDelete}";

    public const string CapitalManager = "Capital_Management";
    public const string CapitalManagerView = CapitalManager + $"_{ActionView}";
    public const string CapitalManagerCreate = CapitalManager + $"_{ActionCreate}";
    public const string CapitalManagerUpdate = CapitalManager + $"_{ActionUpdate}";
    public const string CapitalManagerDelete = CapitalManager + $"_{ActionDelete}";

    public const string AssetManager = "Asset_Management";
    public const string AssetManagerView = AssetManager + $"_{ActionView}";
    public const string AssetManagerCreate = AssetManager + $"_{ActionCreate}";
    public const string AssetManagerUpdate = AssetManager + $"_{ActionUpdate}";
    public const string AssetManagerDelete = AssetManager + $"_{ActionDelete}";

    public const string AdjustDepreciationManager = "AdjustDepreciation_Management";
    public const string AdjustDepreciationManagerView = AdjustDepreciationManager + $"_{ActionView}";
    public const string AdjustDepreciationManagerCreate = AdjustDepreciationManager + $"_{ActionCreate}";
    public const string AdjustDepreciationManagerUpdate = AdjustDepreciationManager + $"_{ActionUpdate}";
    public const string AdjustDepreciationManagerDelete = AdjustDepreciationManager + $"_{ActionDelete}";

    public const string AssetDepreciationAccountingManager = "AssetDepreciationAccounting_Management";
    public const string AssetDepreciationAccountingManagerView = AssetDepreciationAccountingManager + $"_{ActionView}";
    public const string AssetDepreciationAccountingManagerUpdate = AssetDepreciationAccountingManager + $"_{ActionUpdate}";

    public const string AssetDepreciationManager = "AssetDepreciation_Management";
    public const string AssetDepreciationManagerView = AssetDepreciationManager + $"_{ActionView}";
    public const string AssetDepreciationManagerExcute = AssetDepreciationManager + $"_{ActionExcute}";

    public const string ToolAllocationManager = "ToolAllocation_Management";
    public const string ToolAllocationManagerView = ToolAllocationManager + $"_{ActionView}";
    public const string ToolAllocationManagerExcute = ToolAllocationManager + $"_{ActionExcute}";

    public const string ToolsManager = "Tools_Management";
    public const string ToolsManagerView = ToolsManager + $"_{ActionView}";
    public const string ToolsManagerCreate = ToolsManager + $"_{ActionCreate}";
    public const string ToolsManagerUpdate = ToolsManager + $"_{ActionUpdate}";
    public const string ToolsManagerDelete = ToolsManager + $"_{ActionDelete}";

    public const string AdjustAllocationManager = "AdjustAllocation_Management";
    public const string AdjustAllocationManagerView = AdjustAllocationManager + $"_{ActionView}";
    public const string AdjustAllocationManagerCreate = AdjustAllocationManager + $"_{ActionCreate}";
    public const string AdjustAllocationManagerUpdate = AdjustAllocationManager + $"_{ActionUpdate}";
    public const string AdjustAllocationManagerDelete = AdjustAllocationManager + $"_{ActionDelete}";

    public const string ToolAllocationAccountingManager = "ToolAllocationAccounting_Management";
    public const string ToolAllocationAccountingManagerView = ToolAllocationAccountingManager + $"_{ActionView}";
    public const string ToolAllocationAccountingManagerUpdate = ToolAllocationAccountingManager + $"_{ActionUpdate}";

    public const string FinishedProductQuotaManager = "FinishedProductQuota_Management";
    public const string FinishedProductQuotaManagerView = FinishedProductQuotaManager + $"_{ActionView}";
    public const string FinishedProductQuotaManagerCreate = FinishedProductQuotaManager + $"_{ActionCreate}";
    public const string FinishedProductQuotaManagerUpdate = FinishedProductQuotaManager + $"_{ActionUpdate}";
    public const string FinishedProductQuotaManagerDelete = FinishedProductQuotaManager + $"_{ActionDelete}";

    public const string GroupCoefficientManager = "GroupCoefficient_Management";
    public const string GroupCoefficientManagerView = GroupCoefficientManager + $"_{ActionView}";
    public const string GroupCoefficientManagerCreate = GroupCoefficientManager + $"_{ActionCreate}";
    public const string GroupCoefficientManagerUpdate = GroupCoefficientManager + $"_{ActionUpdate}";
    public const string GroupCoefficientManagerDelete = GroupCoefficientManager + $"_{ActionDelete}";

    public const string GroupCoefficientWorkManager = "GroupCoefficientWork_Management";
    public const string GroupCoefficientWorkManagerView = GroupCoefficientWorkManager + $"_{ActionView}";
    public const string GroupCoefficientWorkManagerCreate = GroupCoefficientWorkManager + $"_{ActionCreate}";
    public const string GroupCoefficientWorkManagerUpdate = GroupCoefficientWorkManager + $"_{ActionUpdate}";
    public const string GroupCoefficientWorkManagerDelete = GroupCoefficientWorkManager + $"_{ActionDelete}";

    public const string PExpenseForwardAccEntryManager = "PExpenseForwardAccEntry_Management";
    public const string PExpenseForwardAccEntryManagerView = PExpenseForwardAccEntryManager + $"_{ActionView}";
    public const string PExpenseForwardAccEntryManagerCreate = PExpenseForwardAccEntryManager + $"_{ActionCreate}";
    public const string PExpenseForwardAccEntryManagerUpdate = PExpenseForwardAccEntryManager + $"_{ActionUpdate}";
    public const string PExpenseForwardAccEntryManagerDelete = PExpenseForwardAccEntryManager + $"_{ActionDelete}";

    public const string PQuotaForwardAccEntryManager = "PQuotaForwardAccEntry_Management";
    public const string PQuotaForwardAccEntryManagerView = PQuotaForwardAccEntryManager + $"_{ActionView}";
    public const string PQuotaForwardAccEntryManagerCreate = PQuotaForwardAccEntryManager + $"_{ActionCreate}";
    public const string PQuotaForwardAccEntryManagerUpdate = PQuotaForwardAccEntryManager + $"_{ActionUpdate}";
    public const string PQuotaForwardAccEntryManagerDelete = PQuotaForwardAccEntryManager + $"_{ActionDelete}";

    public const string PCoefficientForwardAccEntryManager = "PCoefficientForwardAccEntry_Management";
    public const string PCoefficientForwardAccEntryManagerView = PCoefficientForwardAccEntryManager + $"_{ActionView}";
    public const string PCoefficientForwardAccEntryManagerCreate = PCoefficientForwardAccEntryManager + $"_{ActionCreate}";
    public const string PCoefficientForwardAccEntryManagerUpdate = PCoefficientForwardAccEntryManager + $"_{ActionUpdate}";
    public const string PCoefficientForwardAccEntryManagerDelete = PCoefficientForwardAccEntryManager + $"_{ActionDelete}";

    public const string PRatioForwardAccEntryManager = "PRatioForwardAccEntry_Management";
    public const string PRatioForwardAccEntryManagerView = PRatioForwardAccEntryManager + $"_{ActionView}";
    public const string PRatioForwardAccEntryManagerCreate = PRatioForwardAccEntryManager + $"_{ActionCreate}";
    public const string PRatioForwardAccEntryManagerUpdate = PRatioForwardAccEntryManager + $"_{ActionUpdate}";
    public const string PRatioForwardAccEntryManagerDelete = PRatioForwardAccEntryManager + $"_{ActionDelete}";

    public const string PBookThzManager = "PBookThz_Management";
    public const string PBookThzManagerView = PBookThzManager + $"_{ActionView}";
    public const string PBookThzManagerCreate = PBookThzManager + $"_{ActionCreate}";
    public const string PBookThzManagerUpdate = PBookThzManager + $"_{ActionUpdate}";
    public const string PBookThzManagerDelete = PBookThzManager + $"_{ActionDelete}";

    public const string WBookThzManager = "WBookThz_Management";
    public const string WBookThzManagerView = WBookThzManager + $"_{ActionView}";
    public const string WBookThzManagerCreate = WBookThzManager + $"_{ActionCreate}";
    public const string WBookThzManagerUpdate = WBookThzManager + $"_{ActionUpdate}";
    public const string WBookThzManagerDelete = WBookThzManager + $"_{ActionDelete}";

    public const string UnFinishedProductManager = "UnFinishedProduct_Management";
    public const string UnFinishedProductManagerView = UnFinishedProductManager + $"_{ActionView}";
    public const string UnFinishedProductManagerViewUserNew = UnFinishedProductManager + $"_{ActionViewByUserNew}";
    public const string UnFinishedProductManagerCreate = UnFinishedProductManager + $"_{ActionCreate}";
    public const string UnFinishedProductManagerUpdate = UnFinishedProductManager + $"_{ActionUpdate}";
    public const string UnFinishedProductManagerDelete = UnFinishedProductManager + $"_{ActionDelete}";
    public const string UnFinishedProductManagerRecording = UnFinishedProductManager + $"_{ActionVoucherRecording}";
    public const string UnFinishedProductManagerUnRecording = UnFinishedProductManager + $"_{ActionVoucherUnRecording}";



    public const string WExpenseForwardAccEntryManager = "WExpenseForwardAccEntry_Management";
    public const string WExpenseForwardAccEntryManagerView = WExpenseForwardAccEntryManager + $"_{ActionView}";
    public const string WExpenseForwardAccEntryManagerCreate = WExpenseForwardAccEntryManager + $"_{ActionCreate}";
    public const string WExpenseForwardAccEntryManagerUpdate = WExpenseForwardAccEntryManager + $"_{ActionUpdate}";
    public const string WExpenseForwardAccEntryManagerDelete = WExpenseForwardAccEntryManager + $"_{ActionDelete}";

    public const string WCoefficientForwardAccEntryManager = "WCoefficientForwardAccEntry_Management";
    public const string WCoefficientForwardAccEntryManagerView = WCoefficientForwardAccEntryManager + $"_{ActionView}";
    public const string WCoefficientForwardAccEntryManagerCreate = WCoefficientForwardAccEntryManager + $"_{ActionCreate}";
    public const string WCoefficientForwardAccEntryManagerUpdate = WCoefficientForwardAccEntryManager + $"_{ActionUpdate}";
    public const string WCoefficientForwardAccEntryManagerDelete = WCoefficientForwardAccEntryManager + $"_{ActionDelete}";

    public const string WRatioForwardAccEntryManager = "WRatioForwardAccEntry_Management";
    public const string WRatioForwardAccEntryManagerView = WRatioForwardAccEntryManager + $"_{ActionView}";
    public const string WRatioForwardAccEntryManagerCreate = WRatioForwardAccEntryManager + $"_{ActionCreate}";
    public const string WRatioForwardAccEntryManagerUpdate = WRatioForwardAccEntryManager + $"_{ActionUpdate}";
    public const string WRatioForwardAccEntryManagerDelete = WRatioForwardAccEntryManager + $"_{ActionDelete}";

    public const string CalcProductCostManager = "CalcProductCost_Management";
    public const string CalcProductCostManagerView = CalcProductCostManager + $"_{ActionView}";
    public const string CalcProductCostManagerExcute = CalcProductCostManager + $"_{ActionExcute}";
    public const string CalcProductCostManagerCreate = CalcProductCostManager + $"_{ActionCreate}";

    public const string CalcWorkCostManager = "CalcWorkCost_Management";
    public const string CalcWorkCostManagerView = CalcWorkCostManager + $"_{ActionView}";
    public const string CalcWorkCostManagerExcute = CalcWorkCostManager + $"_{ActionExcute}";
    public const string CalcWorkCostManagerCreate = CalcWorkCostManager + $"_{ActionCreate}";

    public const string ResetVoucherNumberManager = "ResetVoucherNumber_Management";
    public const string ResetVoucherNumberManagerView = ResetVoucherNumberManager + $"_{ActionView}";
    public const string ResetVoucherNumberManagerExcute = ResetVoucherNumberManager + $"_{ActionExcute}";

    public const string AutoAccForwardManager = "AutoAccForward_Management";
    public const string AutoAccForwardManagerView = AutoAccForwardManager + $"_{ActionView}";
    public const string AutoAccForwardManagerExcute = AutoAccForwardManager + $"_{ActionExcute}";

    public const string RecordingVoucherBookManager = "RecordingVoucherBook_Management";
    public const string RecordingVoucherBookManagerView = RecordingVoucherBookManager + $"_{ActionView}";
    public const string RecordingVoucherBookManagerCreate = RecordingVoucherBookManager + $"_{ActionCreate}";
    public const string RecordingVoucherBookManagerUpdate = RecordingVoucherBookManager + $"_{ActionUpdate}";
    public const string RecordingVoucherBookManagerDelete = RecordingVoucherBookManager + $"_{ActionDelete}";

    public const string UpdateCostPriceManager = "UpdateCostPrice_Management";
    public const string UpdateCostPriceManagerView = UpdateCostPriceManager + $"_{ActionView}";
    public const string UpdateCostPriceManagerExcute = UpdateCostPriceManager + $"_{ActionExcute}";

    public const string PreviousForwardEntryManager = "PreviousForwardEntry_Management";
    public const string PreviousForwardEntryManagerView = PreviousForwardEntryManager + $"_{ActionView}";
    public const string PreviousForwardEntryManagerCreate = PreviousForwardEntryManager + $"_{ActionCreate}";
    public const string PreviousForwardEntryManagerUpdate = PreviousForwardEntryManager + $"_{ActionUpdate}";
    public const string PreviousForwardEntryManagerDelete = PreviousForwardEntryManager + $"_{ActionDelete}";

    public const string RatioAllocateEntryManager = "RatioAllocateEntry_Management";
    public const string RatioAllocateEntryManagerView = RatioAllocateEntryManager + $"_{ActionView}";
    public const string RatioAllocateEntryManagerCreate = RatioAllocateEntryManager + $"_{ActionCreate}";
    public const string RatioAllocateEntryManagerUpdate = RatioAllocateEntryManager + $"_{ActionUpdate}";
    public const string RatioAllocateEntryManagerDelete = RatioAllocateEntryManager + $"_{ActionDelete}";

    public const string ReduceEntryManager = "ReduceEntry_Management";
    public const string ReduceEntryManagerView = ReduceEntryManager + $"_{ActionView}";
    public const string ReduceEntryManagerCreate = ReduceEntryManager + $"_{ActionCreate}";
    public const string ReduceEntryManagerUpdate = ReduceEntryManager + $"_{ActionUpdate}";
    public const string ReduceEntryManagerDelete = ReduceEntryManager + $"_{ActionDelete}";

    public const string ForwardEntryManager = "ForwardEntry_Management";
    public const string ForwardEntryManagerView = ForwardEntryManager + $"_{ActionView}";
    public const string ForwardEntryManagerCreate = ForwardEntryManager + $"_{ActionCreate}";
    public const string ForwardEntryManagerUpdate = ForwardEntryManager + $"_{ActionUpdate}";
    public const string ForwardEntryManagerDelete = ForwardEntryManager + $"_{ActionDelete}";

    public const string TaxForwardEntryManager = "TaxForwardEntry_Management";
    public const string TaxForwardEntryManagerView = TaxForwardEntryManager + $"_{ActionView}";
    public const string TaxForwardEntryManagerCreate = TaxForwardEntryManager + $"_{ActionCreate}";
    public const string TaxForwardEntryManagerUpdate = TaxForwardEntryManager + $"_{ActionUpdate}";
    public const string TaxForwardEntryManagerDelete = TaxForwardEntryManager + $"_{ActionDelete}";

    public const string BusinessForwardEntryManager = "BusinessForwardEntry_Management";
    public const string BusinessForwardEntryManagerView = BusinessForwardEntryManager + $"_{ActionView}";
    public const string BusinessForwardEntryManagerCreate = BusinessForwardEntryManager + $"_{ActionCreate}";
    public const string BusinessForwardEntryManagerUpdate = BusinessForwardEntryManager + $"_{ActionUpdate}";
    public const string BusinessForwardEntryManagerDelete = BusinessForwardEntryManager + $"_{ActionDelete}";

    public const string ForwardBalanceManager = "ForwardBalance_Management";
    public const string ForwardBalanceManagerView = ForwardBalanceManager + $"_{ActionView}";
    public const string ForwardBalanceManagerExecute = ForwardBalanceManager + $"_{ActionExcute}";

    public const string LockVoucherManager = "LockVoucher_Management";
    public const string LockVoucherManagerView = LockVoucherManager + $"_{ActionView}";
    public const string LockVoucherManagerExecute = LockVoucherManager + $"_{ActionExcute}";

    public const string DashboardManager = "Dashboard";
    public const string RevenueDashboard = "Revenue_Dashboard";
    public const string ExpenseDashboard = "Expense_Dashboard";
    public const string FinanceDashboard = "Finance_Dashboard";
    public const string InventoryDashboard = "Inventory_Dashboard";
    public const string ReceivableDashboard = "Receivable_Dashboard";
    public const string LiabilityDashboard = "Liability_Dashboard";

    public const string ReceiveStandardAccManager = "ReceiveStandardAcc_Management";
    public const string ReceiveStandardAccManagerView = ReceiveStandardAccManager + $"_{ActionView}";
    public const string ReceiveStandardAccManagerExecute = ReceiveStandardAccManager + $"_{ActionExcute}";

    public const string InvoiceStatusManager = "InvoiceStatus_Management";
    public const string InvoiceStatusManagerView = InvoiceStatusManager + $"_{ActionView}";
    public const string InvoiceStatusManagerCreate = InvoiceStatusManager + $"_{ActionCreate}";
    public const string InvoiceStatusManagerUpdate = InvoiceStatusManager + $"_{ActionUpdate}";
    public const string InvoiceStatusManagerDelete = InvoiceStatusManager + $"_{ActionDelete}";

    public const string MaintainceCheckManager = "MaintainceCheck_Management";
    public const string MaintainceCheckManagerView = MaintainceCheckManager + $"_{ActionView}";
    public const string MaintainceCheckManagerExecute = MaintainceCheckManager + $"_{ActionExcute}";

    public const string ExchangeRateManager = "ExchangeRate_Management";
    public const string ExchangeRateManagerView = ExchangeRateManager + $"_{ActionView}";
    public const string ExchangeRateManagerExecute = ExchangeRateManager + $"_{ActionExcute}";

    public const string PositionManager = "Position_Management";
    public const string PositionManagerView = PositionManager + $"_{ActionView}";
    public const string PositionManagerCreate = PositionManager + $"_{ActionCreate}";
    public const string PositionManagerUpdate = PositionManager + $"_{ActionUpdate}";
    public const string PositionManagerDelete = PositionManager + $"_{ActionDelete}";

    public const string VatInvoiceManager = "VatInvoice_Management";
    public const string VatInvoiceManagerView = VatInvoiceManager + $"_{ActionView}";
    public const string VatInvoiceManagerCreate = VatInvoiceManager + $"_{ActionCreate}";
    public const string VatInvoiceManagerUpdate = VatInvoiceManager + $"_{ActionUpdate}";
    public const string VatInvoiceManagerDelete = VatInvoiceManager + $"_{ActionDelete}";

    public const string EmployeeManager = "Employee_Management";
    public const string EmployeeManagerView = EmployeeManager + $"_{ActionView}";
    public const string EmployeeManagerCreate = EmployeeManager + $"_{ActionCreate}";
    public const string EmployeeManagerUpdate = EmployeeManager + $"_{ActionUpdate}";
    public const string EmployeeManagerDelete = EmployeeManager + $"_{ActionDelete}";

    public const string SalaryPeriodManager = "SalaryPeriod_Management";
    public const string SalaryPeriodManagerView = SalaryPeriodManager + $"_{ActionView}";
    public const string SalaryPeriodManagerCreate = SalaryPeriodManager + $"_{ActionCreate}";
    public const string SalaryPeriodManagerUpdate = SalaryPeriodManager + $"_{ActionUpdate}";
    public const string SalaryPeriodManagerDelete = SalaryPeriodManager + $"_{ActionDelete}";

    public const string SalaryCategoryManager = "SalaryCategory_Management";
    public const string SalaryCategoryManagerView = SalaryCategoryManager + $"_{ActionView}";
    public const string SalaryCategoryManagerCreate = SalaryCategoryManager + $"_{ActionCreate}";
    public const string SalaryCategoryManagerUpdate = SalaryCategoryManager + $"_{ActionUpdate}";
    public const string SalaryCategoryManagerDelete = SalaryCategoryManager + $"_{ActionDelete}";

    public const string SalaryEmployeeManager = "SalaryEmployee_Management";
    public const string SalaryEmployeeManagerView = SalaryEmployeeManager + $"_{ActionView}";
    public const string SalaryEmployeeManagerCreate = SalaryEmployeeManager + $"_{ActionCreate}";
    public const string SalaryEmployeeManagerUpdate = SalaryEmployeeManager + $"_{ActionUpdate}";
    public const string SalaryEmployeeManagerDelete = SalaryEmployeeManager + $"_{ActionDelete}";

    public const string SalarySheetTypeManager = "SalarySheetType_Management";
    public const string SalarySheetTypeManagerView = SalarySheetTypeManager + $"_{ActionView}";
    public const string SalarySheetTypeManagerCreate = SalarySheetTypeManager + $"_{ActionCreate}";
    public const string SalarySheetTypeManagerUpdate = SalarySheetTypeManager + $"_{ActionUpdate}";
    public const string SalarySheetTypeManagerDelete = SalarySheetTypeManager + $"_{ActionDelete}";

    public const string SalarySheetManager = "SalarySheet_Management";
    public const string SalarySheetManagerView = SalarySheetManager + $"_{ActionView}";
    public const string SalarySheetManagerCreate = SalarySheetManager + $"_{ActionCreate}";
    public const string SalarySheetManagerUpdate = SalarySheetManager + $"_{ActionUpdate}";
    public const string SalarySheetManagerDelete = SalarySheetManager + $"_{ActionDelete}";

    public const string InventoryRecordManager = "InventoryRecord_Management";
    public const string InventoryRecordManagerView = InventoryRecordManager + $"_{ActionView}";
    public const string InventoryRecordManagerCreate = InventoryRecordManager + $"_{ActionCreate}";
    public const string InventoryRecordManagerUpdate = InventoryRecordManager + $"_{ActionUpdate}";
    public const string InventoryRecordManagerDelete = InventoryRecordManager + $"_{ActionDelete}";

    public const string VoucherCategoryManager = "VoucherCategory_Management";
    public const string VoucherCategoryManagerView = VoucherCategoryManager + "_" + ActionView;
    public const string VoucherCategoryManagerCreate = VoucherCategoryManager + "_Create";
    public const string VoucherCategoryManagerUpdate = VoucherCategoryManager + "_Update";
    public const string VoucherCategoryManagerDelete = VoucherCategoryManager + "_Delete";

    public const string InvoiceSupplierManager = "InvoiceSupplier_Management";
    public const string InvoiceSupplierManagerView = InvoiceSupplierManager + $"_{ActionView}";
    public const string InvoiceSupplierManagerCreate = InvoiceSupplierManager + $"_{ActionCreate}";
    public const string InvoiceSupplierManagerUpdate = InvoiceSupplierManager + $"_{ActionUpdate}";
    public const string InvoiceSupplierManagerDelete = InvoiceSupplierManager + $"_{ActionDelete}";

    public const string SystemDiaryManager = "SystemDiary_Management";
    public const string SystemDiaryManagerView = SystemDiaryManager + $"_{ActionView}";

    public const string VoucherRecordingManager = "VoucherRecording_Management";
    public const string VoucherRecordingManagerSave = VoucherRecordingManager + $"_{ActionSave}";

    public const string DataDeletingManager = "DataDeleting_Management";
    public const string DataDeletingManagerView = DataDeletingManager + $"_{ActionView}";
    public const string DataDeletingManagerExecute = DataDeletingManager + $"_{ActionExcute}";
}
