using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddSoTHZ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SoTHZ",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    FinanceDecision = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    FProductOrWork = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitSection = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitFProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditSection = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditFProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TSum = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TGet = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_SoTHZ", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SoTHZ_OrgCode",
                table: "SoTHZ",
                column: "OrgCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SoTHZ");
        }
    }
}
