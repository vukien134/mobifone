using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class MCustomerRegister : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "CustomerRegister",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CustomerRegister_AccessCode",
                table: "CustomerRegister",
                column: "AccessCode",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_CustomerRegister_AccessCode",
                table: "CustomerRegister");

            migrationBuilder.DropColumn(
                name: "Note",
                table: "CustomerRegister");
        }
    }
}
