using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddProductVoucherCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductVoucherCost",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    ProductVoucherId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    CostType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitExchange = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditExchange = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ClearingPartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ClearingWorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CaseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    NoteE = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "text", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductVoucherCost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductVoucherCode_ProductVoucher",
                        column: x => x.ProductVoucherId,
                        principalTable: "ProductVoucher",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductVoucherCost_ProductVoucherId",
                table: "ProductVoucherCost",
                column: "ProductVoucherId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductVoucherCost");
        }
    }
}
