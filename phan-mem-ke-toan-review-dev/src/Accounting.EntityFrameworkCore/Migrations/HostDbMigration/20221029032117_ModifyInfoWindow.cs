using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class ModifyInfoWindow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Required",
                table: "InfoWindowDetail",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AfterLoad",
                table: "InfoWindow",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Required",
                table: "InfoWindowDetail");

            migrationBuilder.DropColumn(
                name: "AfterLoad",
                table: "InfoWindow");
        }
    }
}
