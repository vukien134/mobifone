using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addSalaryBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SalaryBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SalarySheetTypeId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    SalaryPeriodId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    EmployeeCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DepartmentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SalaryCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NumberValue = table.Column<decimal>(type: "numeric", nullable: true),
                    StringValue = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_SalaryBook", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SalaryBook_OrgCode_SalarySheetTypeId_SalaryPeriodId_Departm~",
                table: "SalaryBook",
                columns: new[] { "OrgCode", "SalarySheetTypeId", "SalaryPeriodId", "DepartmentCode", "EmployeeCode", "SalaryCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SalaryBook");
        }
    }
}
