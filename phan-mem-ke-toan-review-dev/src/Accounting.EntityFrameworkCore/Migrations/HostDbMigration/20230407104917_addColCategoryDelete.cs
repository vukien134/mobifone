using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addColCategoryDelete : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BusinessType",
                table: "CategoryDelete",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ord",
                table: "CategoryDelete",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BusinessType",
                table: "CategoryDelete");

            migrationBuilder.DropColumn(
                name: "Ord",
                table: "CategoryDelete");
        }
    }
}
