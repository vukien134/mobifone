using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addDefaultCurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultCurrency",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NameE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ExchangeRate = table.Column<decimal>(type: "numeric(18,6)", nullable: true),
                    ExchangeRateDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ExchangeMethod = table.Column<bool>(type: "boolean", nullable: false),
                    Default = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    OddCurrencyVN = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    OddCurrencyEN = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultCurrency", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultCurrency_Code",
                table: "DefaultCurrency",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultCurrency");
        }
    }
}
