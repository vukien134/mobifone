using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyWarehouseBooks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseBook_ProductVoucher_ProductVoucherId",
                table: "WarehouseBook");

            migrationBuilder.AlterColumn<string>(
                name: "ProductVoucherId",
                table: "WarehouseBook",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseBook_ProductVoucher",
                table: "WarehouseBook",
                column: "ProductVoucherId",
                principalTable: "ProductVoucher",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseBook_ProductVoucher",
                table: "WarehouseBook");

            migrationBuilder.AlterColumn<string>(
                name: "ProductVoucherId",
                table: "WarehouseBook",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseBook_ProductVoucher_ProductVoucherId",
                table: "WarehouseBook",
                column: "ProductVoucherId",
                principalTable: "ProductVoucher",
                principalColumn: "Id");
        }
    }
}
