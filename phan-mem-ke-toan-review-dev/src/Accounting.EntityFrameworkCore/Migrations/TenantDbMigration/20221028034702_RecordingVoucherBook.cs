using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class RecordingVoucherBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecordingVoucherBook",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    VoucherDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    FromDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    ToDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    LstVoucherCode = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TypeDescription = table.Column<int>(type: "integer", nullable: false),
                    TypeFilter = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_RecordingVoucherBook", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RecordingVoucherBook_OrgCode_Year_VoucherDate",
                table: "RecordingVoucherBook",
                columns: new[] { "OrgCode", "Year", "VoucherDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecordingVoucherBook");
        }
    }
}
