using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class BookThz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookThz",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    DecideApply = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    ProductOrWork = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitFProducWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_BookThz", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookThz_OrgCode_Year_DecideApply",
                table: "BookThz",
                columns: new[] { "OrgCode", "Year", "DecideApply" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookThz");
        }
    }
}
