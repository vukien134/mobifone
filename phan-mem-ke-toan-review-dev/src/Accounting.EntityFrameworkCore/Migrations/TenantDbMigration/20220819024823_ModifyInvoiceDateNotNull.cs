using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyInvoiceDateNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "WarehouseBook",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AlterColumn<decimal>(
                name: "VatPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "PITPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinQuantity",
                table: "Product",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxQuantity",
                table: "Product",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ImportTaxPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "ExciseTaxPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "Ledger",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "date");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "WarehouseBook",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "VatPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PITPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MinQuantity",
                table: "Product",
                type: "numeric(12,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxQuantity",
                table: "Product",
                type: "numeric(12,8)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ImportTaxPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExciseTaxPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "DiscountPercentage",
                table: "Product",
                type: "numeric(12,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,4)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "InvoiceDate",
                table: "Ledger",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "date",
                oldNullable: true);
        }
    }
}
