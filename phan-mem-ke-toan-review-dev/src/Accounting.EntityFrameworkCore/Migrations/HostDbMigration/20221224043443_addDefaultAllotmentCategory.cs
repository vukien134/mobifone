using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addDefaultAllotmentCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultAllotmentForwardCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    DecideApply = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    FProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Type = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    OrdGrp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ForwardType = table.Column<int>(type: "integer", nullable: false),
                    LstCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    GroupCoefficientCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductionPeriodCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitCredit = table.Column<int>(type: "integer", nullable: false),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Active = table.Column<int>(type: "integer", nullable: false),
                    RecordBook = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AttachProduct = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    QuantityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NormType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductionPeriodType = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultAllotmentForwardCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultAllotmentForwardCategory_Code",
                table: "DefaultAllotmentForwardCategory",
                column: "Code");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultAllotmentForwardCategory");
        }
    }
}
