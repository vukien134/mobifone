using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class MAccOpeningBalanceIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccOpeningBalance_OrgCode_AccCode_AccSectionCode_ContractCo~",
                table: "AccOpeningBalance");

            migrationBuilder.CreateIndex(
                name: "IX_AccOpeningBalance_OrgCode_AccCode_AccSectionCode_ContractCo~",
                table: "AccOpeningBalance",
                columns: new[] { "OrgCode", "AccCode", "AccSectionCode", "ContractCode", "CurrencyCode", "PartnerCode", "WorkPlaceCode", "FProductWorkCode", "Year" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccOpeningBalance_OrgCode_AccCode_AccSectionCode_ContractCo~",
                table: "AccOpeningBalance");

            migrationBuilder.CreateIndex(
                name: "IX_AccOpeningBalance_OrgCode_AccCode_AccSectionCode_ContractCo~",
                table: "AccOpeningBalance",
                columns: new[] { "OrgCode", "AccCode", "AccSectionCode", "ContractCode", "CurrencyCode", "PartnerCode", "WorkPlaceCode", "Year" },
                unique: true);
        }
    }
}
