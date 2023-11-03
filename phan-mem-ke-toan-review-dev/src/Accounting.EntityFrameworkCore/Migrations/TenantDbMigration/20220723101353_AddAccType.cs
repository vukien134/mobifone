using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddAccType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrgCode",
                table: "OrgUnit");

            migrationBuilder.AddColumn<string>(
                name: "AccType",
                table: "AccountSystem",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                table: "AccountSystem",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccType",
                table: "AccountSystem");

            migrationBuilder.DropColumn(
                name: "ParentCode",
                table: "AccountSystem");

            migrationBuilder.AddColumn<string>(
                name: "OrgCode",
                table: "OrgUnit",
                type: "text",
                nullable: true);
        }
    }
}
