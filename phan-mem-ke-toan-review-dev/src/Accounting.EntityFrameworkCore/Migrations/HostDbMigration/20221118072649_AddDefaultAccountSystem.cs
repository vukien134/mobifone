using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddDefaultAccountSystem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultAccountSystem",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    AccCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AccPattern = table.Column<int>(type: "integer", nullable: false),
                    AssetOrEquity = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AccName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AccNameEn = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AccRank = table.Column<int>(type: "integer", nullable: false),
                    AccType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    ParentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AccNameTemp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    AccNameTempE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    BankName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AttachPartner = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachContract = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachAccSection = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AccSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AttachCurrency = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachWorkPlace = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachProductCost = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsBalanceSheetAcc = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    AttachVoucher = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    ParentAccId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultAccountSystem", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultAccountSystem_UsingDecision_AccCode",
                table: "DefaultAccountSystem",
                columns: new[] { "UsingDecision", "AccCode" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "AccName", "AccNameEn", "AccRank", "AttachPartner", "AttachAccSection", "AttachContract", "AttachCurrency", "AttachProductCost", "AttachVoucher", "AttachWorkPlace" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultAccountSystem");
        }
    }
}
