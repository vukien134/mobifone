using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class TenantStatementTax : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TenantStatementTax",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord0 = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Sign = table.Column<int>(type: "integer", nullable: true),
                    NumberCode1 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NumberCode2 = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Amount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PrintWhen = table.Column<string>(type: "text", nullable: true),
                    Id11 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Id12 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Id21 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Id22 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    En1 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    En2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Re1 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Re2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Va1 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Va2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Mt1 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Mt2 = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AssignValue = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_TenantStatementTax", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantStatementTax_Ord",
                table: "TenantStatementTax",
                column: "Ord");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantStatementTax");
        }
    }
}
