using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class MDefaultFStatement200 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L01",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L01", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L02",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ClosingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L02", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L03",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    OriginalPriceAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    RecordingPriceAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PreventivePriceAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OriginalPrice2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    RecordingPrice2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventivePrice2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    OriginalPrice1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    RecordingPrice1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventivePrice1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L03", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L04",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ValueAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PreventiveAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ValueAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventiveAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ValueAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventiveAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L04", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L05",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L05", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L06",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L06", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L07",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    OriginalPriceAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PreventivePriceAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OriginalPrice2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventivePrice2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    OriginalPrice1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventivePrice1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L07", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L08",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L08", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L09",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NumberCode = table.Column<string>(type: "text", nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    HH1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    HH2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    HH3 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    HH4 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    HH5 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    HH6 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    HH7 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L09", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L10",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    VH1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VH2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VH3 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VH4 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VH5 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VH6 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    VH7 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L10", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L11",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TC1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TC2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TC3 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TC4 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TC5 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TC6 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    TC7 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L11", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L12",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L12", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L13",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ValueAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    InterestAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebtPayingAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ValueAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    InterestAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebtPayingAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Up = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Down = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L13", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L14",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L14", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L15",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L15", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L16",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ValueAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PreventiveAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ValueAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventiveAmount1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ValueAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PreventiveAmount2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L16", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L17",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Type = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Method = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Condition = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Debit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Credit = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    DebitBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    CreditBalance2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L17", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L18",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L18", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultFStatement200L19",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Sort = table.Column<int>(type: "integer", nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    GroupId = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NV1 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV2 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV3 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV4 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV5 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV6 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV7 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    NV8 = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Total = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Title = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultFStatement200L19", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L01_UsingDecision_Ord",
                table: "DefaultFStatement200L01",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L02_UsingDecision_Ord",
                table: "DefaultFStatement200L02",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L03_UsingDecision_Ord",
                table: "DefaultFStatement200L03",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L04_UsingDecision_Ord",
                table: "DefaultFStatement200L04",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L05_UsingDecision_Ord",
                table: "DefaultFStatement200L05",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L06_UsingDecision_Ord",
                table: "DefaultFStatement200L06",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L07_UsingDecision_Ord",
                table: "DefaultFStatement200L07",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L08_UsingDecision_Ord",
                table: "DefaultFStatement200L08",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L09_UsingDecision_Ord",
                table: "DefaultFStatement200L09",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L10_UsingDecision_Ord",
                table: "DefaultFStatement200L10",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L11_UsingDecision_Ord",
                table: "DefaultFStatement200L11",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L12_UsingDecision_Ord",
                table: "DefaultFStatement200L12",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L13_UsingDecision_Ord",
                table: "DefaultFStatement200L13",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L14_UsingDecision_Ord",
                table: "DefaultFStatement200L14",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L15_UsingDecision_Ord",
                table: "DefaultFStatement200L15",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L16_UsingDecision_Ord",
                table: "DefaultFStatement200L16",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L17_UsingDecision_Ord",
                table: "DefaultFStatement200L17",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L18_UsingDecision_Ord",
                table: "DefaultFStatement200L18",
                columns: new[] { "UsingDecision", "Ord" });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultFStatement200L19_UsingDecision_Ord",
                table: "DefaultFStatement200L19",
                columns: new[] { "UsingDecision", "Ord" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultFStatement200L01");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L02");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L03");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L04");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L05");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L06");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L07");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L08");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L09");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L10");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L11");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L12");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L13");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L14");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L15");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L16");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L17");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L18");

            migrationBuilder.DropTable(
                name: "DefaultFStatement200L19");
        }
    }
}
