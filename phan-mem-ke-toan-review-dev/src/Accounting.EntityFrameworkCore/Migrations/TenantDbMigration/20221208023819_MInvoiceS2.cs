using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class MInvoiceS2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DecreaseAmount",
                table: "InvoiceAuthDetail",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DecreasePercentage",
                table: "InvoiceAuthDetail",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ExportDate",
                table: "InvoiceAuth",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ImportDate",
                table: "InvoiceAuth",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DecreaseAmount",
                table: "InvoiceAuthDetail");

            migrationBuilder.DropColumn(
                name: "DecreasePercentage",
                table: "InvoiceAuthDetail");

            migrationBuilder.DropColumn(
                name: "ExportDate",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "ImportDate",
                table: "InvoiceAuth");
        }
    }
}
