using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class RemoveInfoWindowWindowId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InfoWindow_Window",
                table: "InfoWindow");

            migrationBuilder.DropIndex(
                name: "IX_InfoWindow_WindowId",
                table: "InfoWindow");

            migrationBuilder.DropColumn(
                name: "WindowId",
                table: "InfoWindow");

            migrationBuilder.AddColumn<string>(
                name: "InfoWindowId",
                table: "Window",
                type: "character varying(24)",
                maxLength: 24,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InfoWindowId",
                table: "Window");

            migrationBuilder.AddColumn<string>(
                name: "WindowId",
                table: "InfoWindow",
                type: "character varying(24)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InfoWindow_WindowId",
                table: "InfoWindow",
                column: "WindowId");

            migrationBuilder.AddForeignKey(
                name: "FK_InfoWindow_Window",
                table: "InfoWindow",
                column: "WindowId",
                principalTable: "Window",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
