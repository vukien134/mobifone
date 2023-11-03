using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addPriceWarehouseVoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceCur",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "WarehouseBook");

            migrationBuilder.DropColumn(
                name: "PriceCur",
                table: "WarehouseBook");
        }
    }
}
