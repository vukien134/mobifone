using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addTenantStatementTaxData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantStatementTaxData",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Extend = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DeductPre = table.Column<decimal>(type: "numeric", nullable: true),
                    IncreasePre = table.Column<decimal>(type: "numeric", nullable: true),
                    ReducePre = table.Column<decimal>(type: "numeric", nullable: true),
                    SuggestionReturn = table.Column<decimal>(type: "numeric", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "text", nullable: true),
                    LastModifierName = table.Column<string>(type: "text", nullable: true),
                    OrgCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantStatementTaxData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantStatementTaxData");
        }
    }
}
