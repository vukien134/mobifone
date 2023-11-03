using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addInfoCalcStockOut : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InfoCalcPriceStockOut",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExcutionUser = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FromDate = table.Column<DateTime>(type: "date", nullable: true),
                    ToDate = table.Column<DateTime>(type: "date", nullable: true),
                    ProductionPeriodCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WarehouseCose = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductGroupCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOriginCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CalculatingMethod = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Continuous = table.Column<bool>(type: "boolean", nullable: false),
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
                    table.PrimaryKey("PK_InfoCalcPriceStockOut", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InfoCalcPriceStockOutDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Status = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    InfoCalcPriceStockOutId = table.Column<string>(type: "character varying(24)", nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_InfoCalcPriceStockOutDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfoCalcPriceStockOutDetail_InfoCalcPriceStockOut",
                        column: x => x.InfoCalcPriceStockOutId,
                        principalTable: "InfoCalcPriceStockOut",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfoCalcPriceStockOut_OrgCode_Status_BeginDate_EndDate",
                table: "InfoCalcPriceStockOut",
                columns: new[] { "OrgCode", "Status", "BeginDate", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_InfoCalcPriceStockOutDetail_InfoCalcPriceStockOutId",
                table: "InfoCalcPriceStockOutDetail",
                column: "InfoCalcPriceStockOutId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoCalcPriceStockOutDetail");

            migrationBuilder.DropTable(
                name: "InfoCalcPriceStockOut");
        }
    }
}
