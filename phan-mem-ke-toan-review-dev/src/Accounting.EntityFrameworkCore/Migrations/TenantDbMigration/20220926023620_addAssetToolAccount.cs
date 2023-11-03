using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addAssetToolAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AssetToolAccount",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    AssetOrTool = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AssetToolId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    DepreciationDate = table.Column<DateTime>(type: "date", nullable: true),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_AssetToolAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetToolAccount_AssetTool",
                        column: x => x.AssetToolId,
                        principalTable: "AssetTool",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetToolAccount_AssetToolId",
                table: "AssetToolAccount",
                column: "AssetToolId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetToolAccount");
        }
    }
}
