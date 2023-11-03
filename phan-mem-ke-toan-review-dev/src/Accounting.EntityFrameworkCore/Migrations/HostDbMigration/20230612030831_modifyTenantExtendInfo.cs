using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class modifyTenantExtendInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "TenantExtendInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegNumCompany",
                table: "TenantExtendInfo",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegNumMonth",
                table: "TenantExtendInfo",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RegNumUser",
                table: "TenantExtendInfo",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "TenantExtendInfo");

            migrationBuilder.DropColumn(
                name: "RegNumCompany",
                table: "TenantExtendInfo");

            migrationBuilder.DropColumn(
                name: "RegNumMonth",
                table: "TenantExtendInfo");

            migrationBuilder.DropColumn(
                name: "RegNumUser",
                table: "TenantExtendInfo");
        }
    }
}
