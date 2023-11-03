using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addUpdatePayment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalAmountWithouVatCur",
                table: "AccVoucher",
                newName: "TotalAmountWithoutVatCur");

            migrationBuilder.RenameColumn(
                name: "TotalAmountWithouVat",
                table: "AccVoucher",
                newName: "TotalAmountWithoutVat");

            migrationBuilder.CreateTable(
                name: "VoucherPaymentBeginning",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: true),
                    VoucherNumber = table.Column<string>(type: "text", nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName = table.Column<string>(type: "text", nullable: true),
                    TotalAmountWithoutVat = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmountDiscount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmountVat = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    AccCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PaymentType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_VoucherPaymentBeginning", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherPaymentBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AccVoucherId = table.Column<string>(type: "text", nullable: true),
                    DocumentId = table.Column<string>(type: "text", nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    VoucherNumber = table.Column<string>(type: "text", nullable: true),
                    AccCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    VatAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DeadlinePayment = table.Column<DateTime>(type: "date", nullable: true),
                    AmountReceivable = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Times = table.Column<int>(type: "integer", nullable: false),
                    AmountReceived = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    AccType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_VoucherPaymentBook", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherPaymentBeginningDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    VoucherPaymentBeginningId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    DeadlinePayment = table.Column<DateTime>(type: "date", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Tems = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_VoucherPaymentBeginningDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherPaymentBeginningDetail_VoucherPaymentBeginning",
                        column: x => x.VoucherPaymentBeginningId,
                        principalTable: "VoucherPaymentBeginning",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherPaymentBeginning_OrgCode_Year",
                table: "VoucherPaymentBeginning",
                columns: new[] { "OrgCode", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherPaymentBeginningDetail_OrgCode",
                table: "VoucherPaymentBeginningDetail",
                column: "OrgCode");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherPaymentBeginningDetail_VoucherPaymentBeginningId",
                table: "VoucherPaymentBeginningDetail",
                column: "VoucherPaymentBeginningId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherPaymentBook_OrgCode_Year",
                table: "VoucherPaymentBook",
                columns: new[] { "OrgCode", "Year" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VoucherPaymentBeginningDetail");

            migrationBuilder.DropTable(
                name: "VoucherPaymentBook");

            migrationBuilder.DropTable(
                name: "VoucherPaymentBeginning");

            migrationBuilder.RenameColumn(
                name: "TotalAmountWithoutVatCur",
                table: "AccVoucher",
                newName: "TotalAmountWithouVatCur");

            migrationBuilder.RenameColumn(
                name: "TotalAmountWithoutVat",
                table: "AccVoucher",
                newName: "TotalAmountWithouVat");
        }
    }
}
