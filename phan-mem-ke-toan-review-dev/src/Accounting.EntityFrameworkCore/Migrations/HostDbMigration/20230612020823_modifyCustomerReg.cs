using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class modifyCustomerReg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "CustomerRegister",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegNumMonth",
                table: "CustomerRegister",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegNumUser",
                table: "CustomerRegister",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "CustomerRegister");

            migrationBuilder.DropColumn(
                name: "RegNumMonth",
                table: "CustomerRegister");

            migrationBuilder.DropColumn(
                name: "RegNumUser",
                table: "CustomerRegister");
        }
    }
}
