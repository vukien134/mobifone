using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddReference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReferenceId",
                table: "Field",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Reference",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RefType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ValueField = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DisplayField = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UrlApiData = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    ListValue = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ListType = table.Column<string>(type: "text", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", maxLength: 30, nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reference", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Format = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceDetail_Reference",
                        column: x => x.ReferenceId,
                        principalTable: "Reference",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Reference_Code",
                table: "Reference",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceDetail_ReferenceId",
                table: "ReferenceDetail",
                column: "ReferenceId")
                .Annotation("Npgsql:IndexInclude", new[] { "Ord", "FieldName", "Caption", "Format" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReferenceDetail");

            migrationBuilder.DropTable(
                name: "Reference");

            migrationBuilder.DropColumn(
                name: "ReferenceId",
                table: "Field");
        }
    }
}
