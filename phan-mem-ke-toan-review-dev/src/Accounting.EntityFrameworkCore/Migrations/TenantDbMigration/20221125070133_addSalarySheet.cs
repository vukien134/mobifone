using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addSalarySheet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalarySheetType",
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
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalarySheetType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalarySheetTypeDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SalarySheetTypeId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Caption = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Formular = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    DataType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Format = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_SalarySheetTypeDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalarySheetTypeDetails_SalarySheetType",
                        column: x => x.SalarySheetTypeId,
                        principalTable: "SalarySheetType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalarySheetType_OrgCode_Code",
                table: "SalarySheetType",
                columns: new[] { "OrgCode", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_SalarySheetTypeDetail_SalarySheetTypeId",
                table: "SalarySheetTypeDetail",
                column: "SalarySheetTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalarySheetTypeDetail");

            migrationBuilder.DropTable(
                name: "SalarySheetType");
        }
    }
}
