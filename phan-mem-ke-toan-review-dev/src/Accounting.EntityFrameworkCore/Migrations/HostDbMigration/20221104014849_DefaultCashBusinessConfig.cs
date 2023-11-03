using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class DefaultCashBusinessConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultBusinessResult",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
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
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultBusinessResult", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultCashFollowStatement",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
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
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultCashFollowStatement", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultBusinessResult_UsingDecision_Ord",
                table: "DefaultBusinessResult",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultCashFollowStatement_UsingDecision_Ord",
                table: "DefaultCashFollowStatement",
                columns: new[] { "UsingDecision", "Ord" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultBusinessResult");

            migrationBuilder.DropTable(
                name: "DefaultCashFollowStatement");
        }
    }
}
