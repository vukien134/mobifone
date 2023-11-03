using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddTenantFSatement133 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "TenantLicense",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L01",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description1 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Description2 = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L01", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L02",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<int>(type: "integer", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ClosingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L02", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L03",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<int>(type: "integer", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ClosingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IncreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DecreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L03", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L04",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<int>(type: "integer", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ClosingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IncreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DecreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L04", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L05",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<int>(type: "integer", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Debit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Credit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L05", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L06",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<int>(type: "integer", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ClosingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IncreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DecreaseAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L06", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TenantFStatement133L07",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<int>(type: "integer", maxLength: 1, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Amount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount3 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount4 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount5 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount6 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount7 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount8 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_TenantFStatement133L07", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L01_UsingDecision_Ord",
                table: "TenantFStatement133L01",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L02_UsingDecision_Ord",
                table: "TenantFStatement133L02",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L03_UsingDecision_Ord",
                table: "TenantFStatement133L03",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L04_UsingDecision_Ord",
                table: "TenantFStatement133L04",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L05_UsingDecision_Ord",
                table: "TenantFStatement133L05",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L06_UsingDecision_Ord",
                table: "TenantFStatement133L06",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_TenantFStatement133L07_UsingDecision_Ord",
                table: "TenantFStatement133L07",
                columns: new[] { "UsingDecision", "Ord" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantFStatement133L01");

            migrationBuilder.DropTable(
                name: "TenantFStatement133L02");

            migrationBuilder.DropTable(
                name: "TenantFStatement133L03");

            migrationBuilder.DropTable(
                name: "TenantFStatement133L04");

            migrationBuilder.DropTable(
                name: "TenantFStatement133L05");

            migrationBuilder.DropTable(
                name: "TenantFStatement133L06");

            migrationBuilder.DropTable(
                name: "TenantFStatement133L07");

            migrationBuilder.DropColumn(
                name: "Key",
                table: "TenantLicense");
        }
    }
}
