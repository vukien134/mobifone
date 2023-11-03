using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class TabRegLicenceInfo2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegLicenseInfo",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    TaxCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TypeLic = table.Column<int>(type: "integer", nullable: true),
                    Month = table.Column<int>(type: "integer", nullable: true),
                    CompanyQuantity = table.Column<int>(type: "integer", nullable: true),
                    LicXml = table.Column<string>(type: "text", nullable: true),
                    CustomerTenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsApproval = table.Column<bool>(type: "boolean", nullable: true),
                    Key = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RegDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    UserQuantity = table.Column<int>(type: "integer", nullable: true),
                    CompanyType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegLicenseInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegLicenseInfo_CustomerTenantId",
                table: "RegLicenseInfo",
                column: "CustomerTenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
            name: "RegLicenseInfo");
        }
    }
}
