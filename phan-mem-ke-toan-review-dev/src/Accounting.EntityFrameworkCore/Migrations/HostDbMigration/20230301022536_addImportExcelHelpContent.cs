using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addImportExcelHelpContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HelpContent",
                table: "ImportExcelTemplate",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HelpContent",
                table: "ImportExcelTemplate");
        }
    }
}
