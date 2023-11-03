using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddLocked : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "WarehouseBook",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "ProductVoucher",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Ledger",
                type: "boolean",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "InvoiceSeries",
                table: "InvoiceAuth",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BillCode",
                table: "InvoiceAuth",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BuyerMobile",
                table: "InvoiceAuth",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReservationCode",
                table: "InvoiceAuth",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "AccVoucher",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "WarehouseBook");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Ledger");

            migrationBuilder.DropColumn(
                name: "BillCode",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "BuyerMobile",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "ReservationCode",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "Locked",
                table: "AccVoucher");

            migrationBuilder.AlterColumn<Guid>(
                name: "InvoiceSeries",
                table: "InvoiceAuth",
                type: "uuid",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);
        }
    }
}
