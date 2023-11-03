using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addDefaultVoucherCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DefaultVoucherCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NameE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    VoucherGroup = table.Column<int>(type: "integer", nullable: false),
                    VoucherType = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    VoucherOrd = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CurrencyCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AttachBusiness = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IncreaseNumberMethod = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    ProductType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    ChkDuplicateVoucherNumber = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsTransfer = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsAssembly = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    PriceCalculatingMethod = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    IsSavingLedger = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsSavingWarehouseBook = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsCalculateBalanceAcc = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    IsCalculateBalanceProduct = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Prefix = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    SeparatorCharacter = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    Suffix = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    BookClosingDate = table.Column<DateTime>(type: "date", nullable: true),
                    BusinessBeginningDate = table.Column<DateTime>(type: "date", nullable: true),
                    BusinessEndingDate = table.Column<DateTime>(type: "date", nullable: true),
                    TaxType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    VoucherKind = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AttachPartnerPrice = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DefaultVoucherCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DefaultVoucherCategory_Code",
                table: "DefaultVoucherCategory",
                column: "Code",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DefaultVoucherCategory");
        }
    }
}
