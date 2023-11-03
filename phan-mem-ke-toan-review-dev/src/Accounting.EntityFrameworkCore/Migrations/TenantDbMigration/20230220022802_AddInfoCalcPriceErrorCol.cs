using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddInfoCalcPriceErrorCol : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorMsg",
                table: "InfoCalcPriceStockOutDetail",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsError",
                table: "InfoCalcPriceStockOutDetail",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorMsg",
                table: "InfoCalcPriceStockOutDetail");

            migrationBuilder.DropColumn(
                name: "IsError",
                table: "InfoCalcPriceStockOutDetail");
        }
    }
}
