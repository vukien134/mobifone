using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class modifyCustomerRegiter3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "CustomerRegister",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TaxCode",
                table: "CustomerRegister",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsingDecision",
                table: "CustomerRegister",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "CustomerRegister");

            migrationBuilder.DropColumn(
                name: "TaxCode",
                table: "CustomerRegister");

            migrationBuilder.DropColumn(
                name: "UsingDecision",
                table: "CustomerRegister");
        }
    }
}
