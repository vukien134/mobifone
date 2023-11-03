using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class MDefaultThnvnn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultThnvnn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Acc = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Debit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Credit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultThnvnn", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultThnvnn_Ord",
                table: "DefaultThnvnn",
                column: "Ord");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultThnvnn");
        }
    }
}
