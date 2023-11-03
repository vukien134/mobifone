using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddTenantCashBusinessConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantBusinessResult",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TargetCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Htkt = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ThisPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AccumulatedLastPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AccumulatedThisPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CarryingCurrency = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsSummary = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Edit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
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
                    table.PrimaryKey("PK_TenantBusinessResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantCashFollowStatement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TargetCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Htkt = table.Column<int>(type: "integer", nullable: true),
                    Method = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    LastPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ThisPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AccumulatedLastPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    AccumulatedThisPeriod = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CarryingCurrency = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsSummary = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Edit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Negative = table.Column<int>(type: "integer", nullable: true),
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
                    table.PrimaryKey("PK_TenantCashFollowStatement", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantBusinessResult_OrgCode_Year_UsingDecision",
                table: "TenantBusinessResult",
                columns: new[] { "OrgCode", "Year", "UsingDecision" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantCashFollowStatement_OrgCode_Year_UsingDecision",
                table: "TenantCashFollowStatement",
                columns: new[] { "OrgCode", "Year", "UsingDecision" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantBusinessResult");

            migrationBuilder.DropTable(
                name: "TenantCashFollowStatement");
        }
    }
}
