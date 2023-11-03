using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddChangeQuickSearchField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QuickSearchField",
                table: "Tab");

            migrationBuilder.AddColumn<bool>(
                name: "HasQuickSearch",
                table: "Tab",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasQuickSearch",
                table: "Tab");

            migrationBuilder.AddColumn<string>(
                name: "QuickSearchField",
                table: "Tab",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true);
        }
    }
}
