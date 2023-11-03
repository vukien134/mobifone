using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddTaxCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BusinessCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsAccVoucher = table.Column<bool>(type: "boolean", nullable: false),
                    IsProductVoucher = table.Column<bool>(type: "boolean", nullable: false),
                    Prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Separator = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Suffix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
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
                    table.PrimaryKey("PK_BusinessCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TaxCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Percentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    OutOrIn = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Deduct = table.Column<int>(type: "integer", nullable: false),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    IsDirect = table.Column<bool>(type: "boolean", nullable: false),
                    Percetage0 = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
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
                    table.PrimaryKey("PK_TaxCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessCategory_OrgCode_Code",
                table: "BusinessCategory",
                columns: new[] { "OrgCode", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaxCategory_OrgCode_Code",
                table: "TaxCategory",
                columns: new[] { "OrgCode", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusinessCategory");

            migrationBuilder.DropTable(
                name: "TaxCategory");
        }
    }
}
