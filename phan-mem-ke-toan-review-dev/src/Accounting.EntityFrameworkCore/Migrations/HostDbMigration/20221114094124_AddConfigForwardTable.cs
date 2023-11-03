using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddConfigForwardTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ConfigForwardOrg",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    TableName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigForwardOrg", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigForwardYear",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    TableName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FieldNot = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FieldValues = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfigForwardYear", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigForwardYear_Ord",
                table: "ConfigForwardYear",
                column: "Ord");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConfigForwardOrg");

            migrationBuilder.DropTable(
                name: "ConfigForwardYear");
        }
    }
}
