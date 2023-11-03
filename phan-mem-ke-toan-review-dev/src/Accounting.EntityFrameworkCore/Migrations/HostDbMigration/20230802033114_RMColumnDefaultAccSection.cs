using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class RMColumnDefaultAccSection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestDefault",
                table: "DefaultAccSection");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestDefault",
                table: "DefaultAccSection",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }
    }
}
