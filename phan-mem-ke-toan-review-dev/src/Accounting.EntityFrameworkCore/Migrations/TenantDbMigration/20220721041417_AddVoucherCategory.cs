using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddVoucherCategory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FProductWork",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FProductOrWork = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NameTemp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FPWType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Rank = table.Column<int>(type: "integer", nullable: true),
                    ParentCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BeginningDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndingDate = table.Column<DateTime>(type: "date", nullable: false),
                    WorkOwner = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    ParentId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FProductWork", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherCategory",
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
                    BookClosingDate = table.Column<DateTime>(type: "date", nullable: false),
                    BusinessBeginningDate = table.Column<DateTime>(type: "date", nullable: false),
                    BusinessEndingDate = table.Column<DateTime>(type: "date", nullable: false),
                    TaxType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    VoucherKind = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    AttachPartnerPrice = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherCategory", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FProductWork_OrgCode_Code",
                table: "FProductWork",
                columns: new[] { "OrgCode", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherCategory_OrgCode_Code",
                table: "VoucherCategory",
                columns: new[] { "OrgCode", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FProductWork");

            migrationBuilder.DropTable(
                name: "VoucherCategory");
        }
    }
}
