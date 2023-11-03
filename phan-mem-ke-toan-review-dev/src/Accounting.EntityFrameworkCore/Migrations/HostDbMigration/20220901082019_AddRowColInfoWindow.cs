using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddRowColInfoWindow : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Col",
                table: "InfoWindowDetail",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Row",
                table: "InfoWindowDetail",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Col",
                table: "InfoWindowDetail");

            migrationBuilder.DropColumn(
                name: "Row",
                table: "InfoWindowDetail");
        }
    }
}
