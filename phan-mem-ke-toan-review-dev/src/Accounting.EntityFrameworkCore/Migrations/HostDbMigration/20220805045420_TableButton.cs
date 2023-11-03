using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class TableButton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Button",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Icon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IconColor = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Caption = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OnClick = table.Column<string>(type: "text", nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ReportId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    MenuClick = table.Column<string>(type: "text", nullable: true),
                    IsGroup = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    ShortCut = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    ReportTemplateId = table.Column<string>(type: "character varying(24)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Button", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Button_ReportTemplate_ReportTemplateId",
                        column: x => x.ReportTemplateId,
                        principalTable: "ReportTemplate",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Button_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Button_Code",
                table: "Button",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Button_ReportTemplateId",
                table: "Button",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_Button_WindowId",
                table: "Button",
                column: "WindowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Button");
        }
    }
}
