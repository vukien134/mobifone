using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ChangeDebitAccColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DebitAcc2",
                table: "WarehouseBook",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreditAcc2",
                table: "WarehouseBook",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldMaxLength: 30,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "DebitAcc2",
                table: "WarehouseBook",
                type: "numeric",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "CreditAcc2",
                table: "WarehouseBook",
                type: "numeric",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
