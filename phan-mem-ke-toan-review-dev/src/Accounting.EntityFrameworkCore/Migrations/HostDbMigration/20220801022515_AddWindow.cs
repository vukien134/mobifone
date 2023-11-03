using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddWindow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JavaScriptCode",
                table: "MenuAccounting",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Window",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    WindowType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    MaxRowEditInForm = table.Column<int>(type: "integer", nullable: true),
                    OrdRowTab = table.Column<int>(type: "integer", nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Window", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tab",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TabType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TabView = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UrlApiInsert = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UrlApiUpdate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UrlApiDelete = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrderBy = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tab", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tab_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Field",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Row = table.Column<int>(type: "integer", nullable: true),
                    Col = table.Column<int>(type: "integer", nullable: true),
                    FieldWidth = table.Column<int>(type: "integer", nullable: true),
                    FieldHeight = table.Column<int>(type: "integer", nullable: true),
                    ColumnWidth = table.Column<int>(type: "integer", nullable: true),
                    LabelWidth = table.Column<int>(type: "integer", nullable: true),
                    LabelPosition = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Format = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ReadOnly = table.Column<bool>(type: "boolean", nullable: true),
                    ColumnHidden = table.Column<bool>(type: "boolean", nullable: true),
                    FormHidden = table.Column<bool>(type: "boolean", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TabId = table.Column<string>(type: "character varying(24)", nullable: false),
                    FieldId = table.Column<string>(type: "character varying(24)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Field", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Field_Field_FieldId",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Field_Tab",
                        column: x => x.TabId,
                        principalTable: "Tab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Field_FieldId",
                table: "Field",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_Field_TabId",
                table: "Field",
                column: "TabId");

            migrationBuilder.CreateIndex(
                name: "IX_Tab_Code",
                table: "Tab",
                column: "Code",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "WindowId" });

            migrationBuilder.CreateIndex(
                name: "IX_Tab_WindowId",
                table: "Tab",
                column: "WindowId");

            migrationBuilder.CreateIndex(
                name: "IX_Window_Code",
                table: "Window",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Field");

            migrationBuilder.DropTable(
                name: "Tab");

            migrationBuilder.DropTable(
                name: "Window");

            migrationBuilder.DropColumn(
                name: "JavaScriptCode",
                table: "MenuAccounting");
        }
    }
}
