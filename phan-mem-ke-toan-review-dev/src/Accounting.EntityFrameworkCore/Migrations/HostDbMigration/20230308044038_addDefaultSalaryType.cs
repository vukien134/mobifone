using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addDefaultSalaryType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultSalaryCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SalaryType = table.Column<int>(type: "integer", nullable: true),
                    Nature = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Status = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    TaxType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultSalaryCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultSalarySheetType",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultSalarySheetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DefaultSalarySheetTypeDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SalarySheetTypeId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Caption = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Format = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultSalarySheetTypeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DefaultSalarySheetTypeDetails_DefaultSalarySheetType",
                        column: x => x.SalarySheetTypeId,
                        principalTable: "DefaultSalarySheetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultSalaryCategory_Code",
                table: "DefaultSalaryCategory",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultSalarySheetType_Code",
                table: "DefaultSalarySheetType",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_DefaultSalarySheetTypeDetail_SalarySheetTypeId",
                table: "DefaultSalarySheetTypeDetail",
                column: "SalarySheetTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultSalaryCategory");

            migrationBuilder.DropTable(
                name: "DefaultSalarySheetTypeDetail");

            migrationBuilder.DropTable(
                name: "DefaultSalarySheetType");
        }
    }
}
