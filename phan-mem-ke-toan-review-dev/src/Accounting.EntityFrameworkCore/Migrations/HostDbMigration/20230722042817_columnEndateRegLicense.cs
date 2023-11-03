using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class columnEndateRegLicense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "EndDate",
                table: "RegLicense",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CompanyType",
                table: "PackageMobi",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "RegLicense");

            migrationBuilder.DropColumn(
                name: "CompanyType",
                table: "PackageMobi");
        }
    }
}
