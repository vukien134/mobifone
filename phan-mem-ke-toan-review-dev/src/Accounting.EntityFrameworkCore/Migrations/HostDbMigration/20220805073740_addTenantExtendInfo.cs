using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addTenantExtendInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Field_Field_FieldId",
                table: "Field");

            migrationBuilder.DropIndex(
                name: "IX_Field_FieldId",
                table: "Field");

            migrationBuilder.DropColumn(
                name: "FieldId",
                table: "Field");

            migrationBuilder.AddColumn<int>(
                name: "TenantType",
                table: "MenuAccounting",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TenantExtendInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantType = table.Column<int>(type: "integer", nullable: true),
                    LicenseXml = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantExtendInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantExtendInfo_TenantId",
                table: "TenantExtendInfo",
                column: "TenantId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantExtendInfo");

            migrationBuilder.DropColumn(
                name: "TenantType",
                table: "MenuAccounting");

            migrationBuilder.AddColumn<string>(
                name: "FieldId",
                table: "Field",
                type: "character varying(24)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Field_FieldId",
                table: "Field",
                column: "FieldId");

            migrationBuilder.AddForeignKey(
                name: "FK_Field_Field_FieldId",
                table: "Field",
                column: "FieldId",
                principalTable: "Field",
                principalColumn: "Id");
        }
    }
}
