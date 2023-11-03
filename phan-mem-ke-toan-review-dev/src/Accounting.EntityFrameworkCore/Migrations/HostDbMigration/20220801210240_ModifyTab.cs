using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class ModifyTab : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlApiUpdate",
                table: "Tab",
                newName: "UrlApiDetail");

            migrationBuilder.RenameColumn(
                name: "UrlApiInsert",
                table: "Tab",
                newName: "UrlApiData");

            migrationBuilder.RenameColumn(
                name: "UrlApiDelete",
                table: "Tab",
                newName: "UrlApiCrud");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UrlApiDetail",
                table: "Tab",
                newName: "UrlApiUpdate");

            migrationBuilder.RenameColumn(
                name: "UrlApiData",
                table: "Tab",
                newName: "UrlApiInsert");

            migrationBuilder.RenameColumn(
                name: "UrlApiCrud",
                table: "Tab",
                newName: "UrlApiDelete");
        }
    }
}
