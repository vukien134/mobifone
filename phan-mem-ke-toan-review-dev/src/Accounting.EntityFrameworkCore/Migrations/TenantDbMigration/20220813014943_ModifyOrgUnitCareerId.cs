using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyOrgUnitCareerId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Career_OrgCode_Code",
                table: "Career");

            migrationBuilder.DropColumn(
                name: "CareerCode",
                table: "OrgUnit");

            migrationBuilder.DropColumn(
                name: "OrgCode",
                table: "Career");

            migrationBuilder.AddColumn<string>(
                name: "CareerId",
                table: "OrgUnit",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Career_Code",
                table: "Career",
                column: "Code",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Career_Code",
                table: "Career");

            migrationBuilder.DropColumn(
                name: "CareerId",
                table: "OrgUnit");

            migrationBuilder.AddColumn<string>(
                name: "CareerCode",
                table: "OrgUnit",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OrgCode",
                table: "Career",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Career_OrgCode_Code",
                table: "Career",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name" });
        }
    }
}
