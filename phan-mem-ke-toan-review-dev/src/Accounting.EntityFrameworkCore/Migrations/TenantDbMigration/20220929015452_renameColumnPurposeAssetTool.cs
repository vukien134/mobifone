using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class renameColumnPurposeAssetTool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ReduceAcc",
                table: "AssetTool",
                newName: "UpDownCode");

            migrationBuilder.RenameColumn(
                name: "PurposeAcc",
                table: "AssetTool",
                newName: "PurposeCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpDownCode",
                table: "AssetTool",
                newName: "ReduceAcc");

            migrationBuilder.RenameColumn(
                name: "PurposeCode",
                table: "AssetTool",
                newName: "PurposeAcc");
        }
    }
}
