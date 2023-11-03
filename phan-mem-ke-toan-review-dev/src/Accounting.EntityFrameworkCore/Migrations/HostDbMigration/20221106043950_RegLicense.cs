using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class RegLicense : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegLicense",
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
                    RegDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ApprovalDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegLicense", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RegLicense_CustomerTenantId",
                table: "RegLicense",
                column: "CustomerTenantId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegLicense");
        }
    }
}
