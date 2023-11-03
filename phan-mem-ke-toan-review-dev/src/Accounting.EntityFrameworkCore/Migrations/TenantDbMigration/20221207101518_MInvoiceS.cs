using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class MInvoiceS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "WarehouseCode",
                table: "InvoiceAuthDetail",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BusinessCode",
                table: "InvoiceAuth",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommandNumber",
                table: "InvoiceAuth",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartmentCode",
                table: "InvoiceAuth",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherDepartment",
                table: "InvoiceAuth",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartnerName",
                table: "InvoiceAuth",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Representative",
                table: "InvoiceAuth",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WarehouseCode",
                table: "InvoiceAuthDetail");

            migrationBuilder.DropColumn(
                name: "BusinessCode",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "CommandNumber",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "DepartmentCode",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "OtherDepartment",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "PartnerName",
                table: "InvoiceAuth");

            migrationBuilder.DropColumn(
                name: "Representative",
                table: "InvoiceAuth");
        }
    }
}
