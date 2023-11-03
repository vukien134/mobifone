using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addDepartmentParentCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DebitExchangerate",
                table: "AccTaxDetail",
                newName: "DebitExchangeRate");

            migrationBuilder.AddColumn<string>(
                name: "ParentCode",
                table: "Department",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InvoiceBookCode",
                table: "AccTaxDetail",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Department_OrgCode_Code",
                table: "Department",
                columns: new[] { "OrgCode", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Department_OrgCode_Code",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "ParentCode",
                table: "Department");

            migrationBuilder.DropColumn(
                name: "InvoiceBookCode",
                table: "AccTaxDetail");

            migrationBuilder.RenameColumn(
                name: "DebitExchangeRate",
                table: "AccTaxDetail",
                newName: "DebitExchangerate");
        }
    }
}
