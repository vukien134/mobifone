﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class DefaultAccBalanceSheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultAccBalanceSheet",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    UsingDecision = table.Column<int>(type: "integer", nullable: true),
                    Printable = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Bold = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    DebitOrCredit = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Type = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Acc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TargetCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Htkt = table.Column<int>(type: "integer", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DescriptionE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    OpeningAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    EndingAmount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    OpeningAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    EndingAmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_DefaultAccBalanceSheet", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultAccBalanceSheet_UsingDecision_Ord",
                table: "DefaultAccBalanceSheet",
                columns: new[] { "UsingDecision", "Ord" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultAccBalanceSheet");
        }
    }
}
