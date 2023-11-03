using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AssetToolDetailDepreciation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetToolDetailDepreciation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AssetOrTool = table.Column<string>(type: "text", nullable: true),
                    AssetToolId = table.Column<string>(type: "text", nullable: true),
                    Ord0 = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepreciationBeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DepreciationAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetToolDetailDepreciation", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetToolDetailDepreciation_OrgCode_AssetToolId_Ord0_Deprec~",
                table: "AssetToolDetailDepreciation",
                columns: new[] { "OrgCode", "AssetToolId", "Ord0", "DepreciationBeginDate", "DepreciationAmount" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetToolDetailDepreciation");
        }
    }
}
