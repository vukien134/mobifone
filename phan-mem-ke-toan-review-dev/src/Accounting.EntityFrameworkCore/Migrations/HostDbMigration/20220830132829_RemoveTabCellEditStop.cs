using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class RemoveTabCellEditStop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CellEditStop",
                table: "Tab");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CellEditStop",
                table: "Tab",
                type: "text",
                nullable: true);
        }
    }
}
