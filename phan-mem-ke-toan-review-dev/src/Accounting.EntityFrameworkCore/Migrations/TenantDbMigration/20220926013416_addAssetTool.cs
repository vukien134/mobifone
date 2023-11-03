using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addAssetTool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetTool",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    AssetOrTool = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AssetToolGroupId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    AssetToolCard = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ProductionYear = table.Column<int>(type: "integer", nullable: true),
                    Wattage = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    AssetToolAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PurposeAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepreciationType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ReduceAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ReduceDetail = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReduceDate = table.Column<DateTime>(type: "date", nullable: true),
                    Content = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CalculatingMethod = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    FollowDepreciation = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Impoverishment = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Remaining = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DepreciationAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DepreciationAmount0 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_AssetTool", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AssetToolAccessory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    AssetOrTool = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AssetToolId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_AssetToolAccessory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetToolAccessory_AssetTool",
                        column: x => x.AssetToolId,
                        principalTable: "AssetTool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetToolDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AssetOrTool = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AssetToolId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "date", nullable: true),
                    UpDownCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UpDownDate = table.Column<DateTime>(type: "date", nullable: true),
                    Number = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CapitalCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OriginalPrice = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Impoverishment = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    MonthNumber0 = table.Column<int>(type: "integer", nullable: true),
                    IsCalculating = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Remaining = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: true),
                    CalculatingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    MonthNumber = table.Column<int>(type: "integer", nullable: true),
                    DepreciationAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    DepreciationDebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepreciationCreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_AssetToolDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetToolDetail_AssetTool",
                        column: x => x.AssetToolId,
                        principalTable: "AssetTool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetToolStoppingDepreciation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AssetOrTool = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AssetToolId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_AssetToolStoppingDepreciation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetToolStoppingDepreciation_AssetTool",
                        column: x => x.AssetToolId,
                        principalTable: "AssetTool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetTool_OrgCode_Code",
                table: "AssetTool",
                columns: new[] { "OrgCode", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_AssetToolAccessory_AssetToolId",
                table: "AssetToolAccessory",
                column: "AssetToolId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetToolDetail_AssetToolId",
                table: "AssetToolDetail",
                column: "AssetToolId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetToolStoppingDepreciation_AssetToolId",
                table: "AssetToolStoppingDepreciation",
                column: "AssetToolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetToolAccessory");

            migrationBuilder.DropTable(
                name: "AssetToolDetail");

            migrationBuilder.DropTable(
                name: "AssetToolStoppingDepreciation");

            migrationBuilder.DropTable(
                name: "AssetTool");
        }
    }
}
