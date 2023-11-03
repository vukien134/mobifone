using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addLinkCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LinkCode",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    FieldCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RefTableName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RefFieldCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsChkDel = table.Column<bool>(type: "boolean", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LinkCode", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LinkCode_FieldCode",
                table: "LinkCode",
                column: "FieldCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LinkCode");
        }
    }
}
