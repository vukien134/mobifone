using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addTenantThnvnn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InventoryRecord",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: true),
                    TransDate = table.Column<DateTime>(type: "date", nullable: true),
                    Representative1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Position1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OtherContact1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Representative2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Position2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OtherContact2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Representative3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Position3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OtherContact3 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TotalAuditAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalInventoryAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalOverAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TotalShortAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_InventoryRecord", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantThnvnn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Acc = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Debit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Credit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantThnvnn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InventoryRecordDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    InventoryRecordId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "text", nullable: true),
                    WarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    AuditQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    AuditAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    InventoryQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    InventoryAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    OverQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    OverAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ShortQuantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    ShortAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Quality1 = table.Column<int>(type: "integer", maxLength: 30, nullable: true),
                    Quality2 = table.Column<int>(type: "integer", nullable: true),
                    Quality3 = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_InventoryRecordDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InventoryRecordDetail_InventoryRecord",
                        column: x => x.InventoryRecordId,
                        principalTable: "InventoryRecord",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecord_OrgCode_Year",
                table: "InventoryRecord",
                columns: new[] { "OrgCode", "Year" });

            migrationBuilder.CreateIndex(
                name: "IX_InventoryRecordDetail_InventoryRecordId",
                table: "InventoryRecordDetail",
                column: "InventoryRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantThnvnn_OrgCode_Year_Ord",
                table: "TenantThnvnn",
                columns: new[] { "OrgCode", "Year", "Ord" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InventoryRecordDetail");

            migrationBuilder.DropTable(
                name: "TenantThnvnn");

            migrationBuilder.DropTable(
                name: "InventoryRecord");
        }
    }
}
