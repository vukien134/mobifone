using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddInvoiceAuth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceAuth",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    InvoiceType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceCodeId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    InvoiceSeries = table.Column<Guid>(type: "uuid", maxLength: 15, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    IssuedDate = table.Column<DateTime>(type: "date", nullable: true),
                    SubmmittedDate = table.Column<DateTime>(type: "date", nullable: true),
                    ContractNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractDate = table.Column<DateTime>(type: "date", nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    InvoiceNote = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AdjustmentType = table.Column<int>(type: "integer", nullable: true),
                    OriginalInvoiceId = table.Column<Guid>(type: "uuid", nullable: true),
                    AdditionalReferenceDes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    AdditionalReferenceDate = table.Column<DateTime>(type: "date", nullable: true),
                    BuyerDisplayName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BuyerLegalName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BuyerTaxCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BuyerAddressLine = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BuyerEmail = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BuyerBankAccount = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BuyerBankName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PaymentMethodName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SellerTaxCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SellerLegalName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SellerAddressLine = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    SellerBankAccount = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SignedDate = table.Column<DateTime>(type: "date", nullable: true),
                    InvoiceTemplate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    OrderNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DocumentNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DocumentDate = table.Column<DateTime>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    AdjustmentInvoiceNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Signature = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    DeliveryOrderNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DeliveryOrderDate = table.Column<DateTime>(type: "date", nullable: true),
                    DeliveryBy = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TransportationMethod = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FromWarehouseName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ToWarehouseName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TotalAmountWithoutVat = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountByWord = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAuth", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InvoiceAuthDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    InvoiceAuthId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ItemCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ItemName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    TotalAmountWithoutVat = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VatPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    VatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Promotion = table.Column<bool>(type: "boolean", nullable: true),
                    TaxCategoryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvoiceAuthDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvoiceAuthDetail_InvoiceAuth",
                        column: x => x.InvoiceAuthId,
                        principalTable: "InvoiceAuth",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAuth_InvoiceId",
                table: "InvoiceAuth",
                column: "InvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceAuthDetail_InvoiceAuthId",
                table: "InvoiceAuthDetail",
                column: "InvoiceAuthId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceAuthDetail");

            migrationBuilder.DropTable(
                name: "InvoiceAuth");
        }
    }
}
