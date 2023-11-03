using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class TableRelationButton : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Button_ReportTemplate_ReportTemplateId",
                table: "Button");

            migrationBuilder.DropColumn(
                name: "ReportId",
                table: "Button");

            migrationBuilder.AlterColumn<string>(
                name: "WindowId",
                table: "Button",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportTemplate_Window",
                table: "Button",
                column: "ReportTemplateId",
                principalTable: "ReportTemplate",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportTemplate_Window",
                table: "Button");

            migrationBuilder.AlterColumn<string>(
                name: "WindowId",
                table: "Button",
                type: "character varying(24)",
                maxLength: 24,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(24)",
                oldMaxLength: 24,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReportId",
                table: "Button",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Button_ReportTemplate_ReportTemplateId",
                table: "Button",
                column: "ReportTemplateId",
                principalTable: "ReportTemplate",
                principalColumn: "Id");
        }
    }
}
