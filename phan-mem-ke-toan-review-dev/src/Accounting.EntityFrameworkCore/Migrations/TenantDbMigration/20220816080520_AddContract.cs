using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddContract : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contract",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    SignedDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    BeginDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EndDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
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
                    table.PrimaryKey("PK_Contract", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContractDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ContractId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric", nullable: true),
                    TrxQuantity = table.Column<decimal>(type: "numeric", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric", nullable: true),
                    Price = table.Column<decimal>(type: "numeric", nullable: true),
                    TrxPriceCur = table.Column<decimal>(type: "numeric", nullable: true),
                    TrxPrice = table.Column<decimal>(type: "numeric", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric", nullable: true),
                    TrxAmountCur = table.Column<decimal>(type: "numeric", nullable: true),
                    TrxAmount = table.Column<decimal>(type: "numeric", nullable: true),
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
                    table.PrimaryKey("PK_ContractDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContractDetail_Contract",
                        column: x => x.ContractId,
                        principalTable: "Contract",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ledger_OrgCode_VoucherId",
                table: "Ledger",
                columns: new[] { "OrgCode", "VoucherId" });

            migrationBuilder.CreateIndex(
                name: "IX_Contract_OrgCode_Code",
                table: "Contract",
                columns: new[] { "OrgCode", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContractDetail_ContractId",
                table: "ContractDetail",
                column: "ContractId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContractDetail");

            migrationBuilder.DropTable(
                name: "Contract");

            migrationBuilder.DropIndex(
                name: "IX_Ledger_OrgCode_VoucherId",
                table: "Ledger");
        }
    }
}
