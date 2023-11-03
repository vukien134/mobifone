using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class TenantInRegLicense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "RegLicenseInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "RegLicenseInfo",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                table: "RegLicenseInfo",
                type: "uuid",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "RegLicenseInfo");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "RegLicenseInfo");

            migrationBuilder.DropColumn(
                name: "TenantId",
                table: "RegLicenseInfo");
        }
    }
}
