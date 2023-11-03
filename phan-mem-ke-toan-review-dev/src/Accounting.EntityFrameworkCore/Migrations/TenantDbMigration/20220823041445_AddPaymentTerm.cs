using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddPaymentTerm : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentTerm",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
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
                    table.PrimaryKey("PK_PaymentTerm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentTermDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PaymentTermId = table.Column<string>(type: "character varying(24)", nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    Days = table.Column<int>(type: "integer", nullable: true),
                    Percentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
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
                    table.PrimaryKey("PK_PaymentTermDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentTermDetail_PaymentTerm",
                        column: x => x.PaymentTermId,
                        principalTable: "PaymentTerm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTerm_OrgCode_Code",
                table: "PaymentTerm",
                columns: new[] { "OrgCode", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PaymentTermDetail_PaymentTermId",
                table: "PaymentTermDetail",
                column: "PaymentTermId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentTermDetail");

            migrationBuilder.DropTable(
                name: "PaymentTerm");
        }
    }
}
