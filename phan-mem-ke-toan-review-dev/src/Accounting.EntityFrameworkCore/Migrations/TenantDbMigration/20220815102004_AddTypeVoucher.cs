using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddTypeVoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrgUnitPermission_UserId",
                table: "OrgUnitPermission");

            migrationBuilder.CreateTable(
                name: "Department",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NameTemp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ParentId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    DepartmentType = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_Department", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherExciseTax",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AccVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExciseTaxCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceGroup = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    InvoiceBookCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "date", nullable: true),
                    InvoiceSymbol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceSerial = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InvoiceNumber = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitExchange = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditExchange = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    CleaningPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CleaningFProducWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CleaningContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CleaningSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CleaningWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AmountWithoutTaxCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AmountWithoutTax = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ExciseTaxPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NoteE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    ProductCode0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductName0 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
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
                    table.PrimaryKey("PK_VoucherExciseTax", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherExciseTax_AccVoucher",
                        column: x => x.AccVoucherId,
                        principalTable: "AccVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VoucherExciseTax_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ListVoucher = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    ListGroup = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherType", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnitPermission_UserId",
                table: "OrgUnitPermission",
                column: "UserId")
                .Annotation("Npgsql:IndexInclude", new[] { "OrgUnitId" });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherExciseTax_AccVoucherId",
                table: "VoucherExciseTax",
                column: "AccVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherExciseTax_ProductVoucherId",
                table: "VoucherExciseTax",
                column: "ProductVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherType_Code",
                table: "VoucherType",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Department");

            migrationBuilder.DropTable(
                name: "VoucherExciseTax");

            migrationBuilder.DropTable(
                name: "VoucherType");

            migrationBuilder.DropIndex(
                name: "IX_OrgUnitPermission_UserId",
                table: "OrgUnitPermission");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnitPermission_UserId",
                table: "OrgUnitPermission",
                column: "UserId",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "OrgUnitId" });
        }
    }
}
