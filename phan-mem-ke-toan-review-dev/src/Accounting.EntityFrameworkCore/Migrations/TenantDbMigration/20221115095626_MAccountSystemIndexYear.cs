using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class MAccountSystemIndexYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountSystem_OrgCode_AccCode",
                table: "AccountSystem");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSystem_OrgCode_AccCode_Year",
                table: "AccountSystem",
                columns: new[] { "OrgCode", "AccCode", "Year" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "AccName", "AccNameEn", "AccRank", "AttachPartner", "AttachAccSection", "AttachContract", "AttachCurrency", "AttachProductCost", "AttachVoucher", "AttachWorkPlace" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AccountSystem_OrgCode_AccCode_Year",
                table: "AccountSystem");

            migrationBuilder.CreateIndex(
                name: "IX_AccountSystem_OrgCode_AccCode",
                table: "AccountSystem",
                columns: new[] { "OrgCode", "AccCode" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "AccName", "AccNameEn", "AccRank", "AttachPartner", "AttachAccSection", "AttachContract", "AttachCurrency", "AttachProductCost", "AttachVoucher", "AttachWorkPlace" });
        }
    }
}
