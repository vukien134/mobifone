using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddVoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccOpeningBalance",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    AccCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AccSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Debit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DebitCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Credit = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreditCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DebitCum = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DebitCumCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreditCum = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreditCumCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccOpeningBalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccSection",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AttachProductCost = table.Column<string>(type: "text", nullable: true),
                    SectionType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccSection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccVoucher",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherGroup = table.Column<int>(type: "integer", nullable: false),
                    BusinessCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BusinessAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: false),
                    PaymentTermsCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Representative = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BankNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BankName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OriginVoucher = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CurrencyCode = table.Column<string>(type: "text", nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: false),
                    TotalAmountWithouVatCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmountWithouVat = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmountVatCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmountVat = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AccType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccVoucher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ledger",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    VoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Ord0Extra = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherGroup = table.Column<int>(type: "integer", nullable: false),
                    BusinessCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BusinessAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CheckDuplicate = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceNbr = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecordingVoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: false),
                    Days = table.Column<int>(type: "integer", nullable: true),
                    PaymentTermsCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    PartnerCode0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Representative = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OriginVoucher = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    DebitCurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditCurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    CreditPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NoteE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransWarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    TrxPromotionQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    PromotionQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceSymbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "date", nullable: false),
                    InvoicePartnerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InvoicePartnerAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CheckDuplicate0 = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    SalesChannelCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SecurityNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ledger", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Specification = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Barcode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AttachProductLot = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachProductOrigin = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachWorkPlace = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    ProductType = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ExciseTaxCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductCostAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RevenueAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DiscountAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SaleReturnsAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductionPeriodCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductGroupCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    MinQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: false),
                    MaxQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: false),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    ImportTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CareerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PITPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductOpeningBalance",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    WarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AccCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: false),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductOpeningBalance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductVoucher",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherGroup = table.Column<int>(type: "integer", nullable: false),
                    BusinessCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BusinessAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: false),
                    PaymentTermsCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Representative = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Tel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Place = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OriginVoucher = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    TotalAmountWithoutVatCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalAmountWithoutVat = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalDiscountAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalDiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalVatAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalVatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    TotalAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalProductAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalProductAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalExciseTaxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalExciseTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    ExportNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalExpenseAmountCur0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalExpenseAmount0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalImportTaxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalImportTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalExpenseAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalExpenseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    EmployeeCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DeliveryDate = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    PaymentTermsId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    SalesChannelCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BillNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DevaluationPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    TotalDevaluationAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalDevaluationAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PriceDebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PriceCreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PriceDecreasingDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    IsCreatedEInvoice = table.Column<bool>(type: "boolean", nullable: false),
                    InfoFilter = table.Column<string>(type: "text", nullable: true),
                    RefType = table.Column<int>(type: "integer", nullable: true),
                    ImportDate = table.Column<DateTime>(type: "date", nullable: false),
                    ExportDate = table.Column<DateTime>(type: "date", nullable: false),
                    Vehicle = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OtherDepartment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CommandNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TotalExpenseAmountCur1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalExpenseAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucher", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WarehouseBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Ord0Extra = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherGroup = table.Column<int>(type: "integer", nullable: false),
                    BusinessCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BusinessAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: false),
                    PaymentTermsCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    PartnerCode0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Representative = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Tel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Place = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    OriginVoucher = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransferingUnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransWarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    TrxPrice = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    TrxPriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    QuantityCur = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmountCur0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmount0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmountCur1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExprenseAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    VatAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DiscountAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ImportTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ImportTaxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ImportTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ExciseTaxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExciseTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxPriceCur2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    TrxPrice2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    PriceCur2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    AmountCur2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitAcc2 = table.Column<decimal>(type: "numeric", maxLength: 30, nullable: true),
                    CreditAcc2 = table.Column<decimal>(type: "numeric", maxLength: 30, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NoteE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    PriceCur0 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price0 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    ImportAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxImportQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    ImportQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    ImportAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ImportAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExportAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxExportQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    ExportQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    ExportAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExportAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    FixedPrice = table.Column<bool>(type: "boolean", nullable: false),
                    CalculateTransfering = table.Column<bool>(type: "boolean", nullable: false),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    InvoiceSymbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoicePartnerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    InvoicePartnerAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "date", nullable: false),
                    SalesChannelCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BillNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VatPriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    VatPrice = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    DevaluationPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DevaluationPrice = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    DevaluationPriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    DevaluationAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DevaluationAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VarianceAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ProductName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TotalAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalDiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarehouseBook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AccVoucherDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AccVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitExchageRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    ClearingPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecordingVoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccVoucherDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccVoucherDetail_AccVoucher",
                        column: x => x.AccVoucherId,
                        principalTable: "AccVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ProductId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PurchasePrice = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    PurchasePriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    SalePriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPrice", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPrice_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductUnit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ProductId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsBasicUnit = table.Column<bool>(type: "boolean", nullable: false),
                    ExchangeRate = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    PurchasePriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    SalePrice = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    SalePriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductUnit", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductUnit_Product",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AccTaxDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AccVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CheckDuplicate = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: false),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "date", nullable: false),
                    InvoiceGroup = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    InvoiceSymbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitExchangerate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    ClearingPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AmountWithoutVatCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountWithoutVat = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NoteE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecordingVoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TotalAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    InvoiceLink = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SecurityNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccTaxDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AccTaxDetail_AccVoucher",
                        column: x => x.AccVoucherId,
                        principalTable: "AccVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AccTaxDetail_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVoucherAssembly",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    AssemblyWarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AssemblyProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AssemblyUnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AssemblyWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AssemblyProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucherAssembly", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVoucherAssembly_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVoucherDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ProductName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransUnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransWarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TrxQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    TrxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TrxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    FixedPrice = table.Column<decimal>(type: "numeric(22,8)", nullable: false),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TransProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PriceCur2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    AmountCur2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitAcc2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc2 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InsuranceDate = table.Column<DateTime>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NoteE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    HTPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    RevenueAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatPriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    VatPrice = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    DevaluationPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DevaluationPriceCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DevaluationPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DevaluationAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DevaluationAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VarianceAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    RefId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    ProductVoucherDetailId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    VatTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DecreasePercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DecreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountWithVat = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountAfterDecrease = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucherDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVoucherDetail_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVoucherReceipt",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DiscountDebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DiscountCreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DiscountDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    DiscountDebitAcc0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DiscountCreditAcc0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DiscountAmountCur0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountAmount0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountDescription0 = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ImportTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ImportDebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ImportCreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ImportDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ExciseTaxDebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ExciseTaxCreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ExciseTaxDescription = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucherReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVoucherReceipt_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVoucherVat",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceSymbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceSerial = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    VatAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VatProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    VatPartnerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BuyerBankNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    SellerBankNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucherVat", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVoucherVat_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductVoucherDetailReceipt",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherDetailId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    VatAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ImportTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ImportTaxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ImportTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    ExciseTaxAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExciseTaxAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmountCur0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmount0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmountCur1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExpenseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucherDetailReceipt", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVoucherDetailReceipt_ProductVoucherDetail",
                        column: x => x.ProductVoucherDetailId,
                        principalTable: "ProductVoucherDetail",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccOpeningBalance_OrgCode_AccCode_AccSectionCode_ContractCo~",
                table: "AccOpeningBalance",
                columns: new[] { "OrgCode", "AccCode", "AccSectionCode", "ContractCode", "CurrencyCode", "PartnerCode", "WorkPlaceCode", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AccSection_OrgCode_Code",
                table: "AccSection",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "SectionType" });

            migrationBuilder.CreateIndex(
                name: "IX_AccTaxDetail_AccVoucherId",
                table: "AccTaxDetail",
                column: "AccVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_AccTaxDetail_OrgCode_AccVoucherId",
                table: "AccTaxDetail",
                columns: new[] { "OrgCode", "AccVoucherId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccTaxDetail_OrgCode_ProductVoucherId",
                table: "AccTaxDetail",
                columns: new[] { "OrgCode", "ProductVoucherId" });

            migrationBuilder.CreateIndex(
                name: "IX_AccTaxDetail_ProductVoucherId",
                table: "AccTaxDetail",
                column: "ProductVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_AccVoucher_OrgCode_Year_VoucherCode_VoucherGroup_VoucherDat~",
                table: "AccVoucher",
                columns: new[] { "OrgCode", "Year", "VoucherCode", "VoucherGroup", "VoucherDate", "VoucherNumber" })
                .Annotation("Npgsql:IndexInclude", new[] { "PartnerCode0", "PartnerName0", "DebitOrCredit", "CurrencyCode", "ExchangeRate" });

            migrationBuilder.CreateIndex(
                name: "IX_AccVoucherDetail_AccVoucherId",
                table: "AccVoucherDetail",
                column: "AccVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_AccVoucherDetail_OrgCode_AccVoucherId",
                table: "AccVoucherDetail",
                columns: new[] { "OrgCode", "AccVoucherId" })
                .Annotation("Npgsql:IndexInclude", new[] { "Ord0" });

            migrationBuilder.CreateIndex(
                name: "IX_Ledger_VoucherId",
                table: "Ledger",
                column: "VoucherId")
                .Annotation("Npgsql:IndexInclude", new[] { "Ord0", "VoucherCode", "VoucherDate", "VoucherGroup", "Year", "OrgCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Product_OrgCode_Code",
                table: "Product",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "ProductGroupCode", "ProductAcc", "ProductCostAcc", "RevenueAcc", "SaleReturnsAcc" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductOpeningBalance_OrgCode_ProductCode_ProductLotCode_Pr~",
                table: "ProductOpeningBalance",
                columns: new[] { "OrgCode", "ProductCode", "ProductLotCode", "ProductOriginCode", "WarehouseCode", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPrice_ProductId",
                table: "ProductPrice",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductUnit_ProductId",
                table: "ProductUnit",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucher_OrgCode_VoucherCode_VoucherGroup_VoucherDate~",
                table: "ProductVoucher",
                columns: new[] { "OrgCode", "VoucherCode", "VoucherGroup", "VoucherDate", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherAssembly_ProductVoucherId",
                table: "ProductVoucherAssembly",
                column: "ProductVoucherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherDetail_OrgCode_ProductVoucherDetailId",
                table: "ProductVoucherDetail",
                columns: new[] { "OrgCode", "ProductVoucherDetailId" })
                .Annotation("Npgsql:IndexInclude", new[] { "Ord0" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherDetail_ProductVoucherId",
                table: "ProductVoucherDetail",
                column: "ProductVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherDetailReceipt_OrgCode_ProductVoucherDetailId",
                table: "ProductVoucherDetailReceipt",
                columns: new[] { "OrgCode", "ProductVoucherDetailId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherDetailReceipt_ProductVoucherDetailId",
                table: "ProductVoucherDetailReceipt",
                column: "ProductVoucherDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherReceipt_ProductVoucherId",
                table: "ProductVoucherReceipt",
                column: "ProductVoucherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherVat_ProductVoucherId",
                table: "ProductVoucherVat",
                column: "ProductVoucherId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WarehouseBook_ProductVoucherId",
                table: "WarehouseBook",
                column: "ProductVoucherId")
                .Annotation("Npgsql:IndexInclude", new[] { "OrgCode", "Ord0", "VoucherCode", "VoucherGroup", "VoucherDate", "VoucherNumber" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccOpeningBalance");

            migrationBuilder.DropTable(
                name: "AccSection");

            migrationBuilder.DropTable(
                name: "AccTaxDetail");

            migrationBuilder.DropTable(
                name: "AccVoucherDetail");

            migrationBuilder.DropTable(
                name: "Ledger");

            migrationBuilder.DropTable(
                name: "ProductOpeningBalance");

            migrationBuilder.DropTable(
                name: "ProductPrice");

            migrationBuilder.DropTable(
                name: "ProductUnit");

            migrationBuilder.DropTable(
                name: "ProductVoucherAssembly");

            migrationBuilder.DropTable(
                name: "ProductVoucherDetailReceipt");

            migrationBuilder.DropTable(
                name: "ProductVoucherReceipt");

            migrationBuilder.DropTable(
                name: "ProductVoucherVat");

            migrationBuilder.DropTable(
                name: "WarehouseBook");

            migrationBuilder.DropTable(
                name: "AccVoucher");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ProductVoucherDetail");

            migrationBuilder.DropTable(
                name: "ProductVoucher");
        }
    }
}
