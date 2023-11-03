using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyVocherType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ListVoucher",
                table: "VoucherType",
                type: "character varying(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "DiscountPrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: true),
                    EndDate = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_DiscountPrice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionPeriod",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_ProductionPeriod", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductLot",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ManufacturingDate = table.Column<DateTime>(type: "date", nullable: true),
                    ManufaturingCountry = table.Column<DateTime>(type: "timestamp without time zone", maxLength: 50, nullable: true),
                    ImportDate = table.Column<DateTime>(type: "date", nullable: true),
                    ExpireDate = table.Column<DateTime>(type: "date", nullable: true),
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
                    table.PrimaryKey("PK_ProductLot", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DiscountPriceDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    DiscountPriceId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    PriceCur2 = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    DiscountPercentage = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    DiscountAmountPrice = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    DiscountAmountPriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
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
                    table.PrimaryKey("PK_DiscountPriceDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountPriceDetail_DiscountPrice",
                        column: x => x.DiscountPriceId,
                        principalTable: "DiscountPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DiscountPricePartner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    DiscountPriceId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_DiscountPricePartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountPricePartner_DiscountPrice",
                        column: x => x.DiscountPriceId,
                        principalTable: "DiscountPrice",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountPrice_OrgCode_Code",
                table: "DiscountPrice",
                columns: new[] { "OrgCode", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DiscountPriceDetail_DiscountPriceId",
                table: "DiscountPriceDetail",
                column: "DiscountPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountPricePartner_DiscountPriceId",
                table: "DiscountPricePartner",
                column: "DiscountPriceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionPeriod_OrgCode_Code",
                table: "ProductionPeriod",
                columns: new[] { "OrgCode", "Code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductLot_OrgCode_Code",
                table: "ProductLot",
                columns: new[] { "OrgCode", "Code" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountPriceDetail");

            migrationBuilder.DropTable(
                name: "DiscountPricePartner");

            migrationBuilder.DropTable(
                name: "ProductionPeriod");

            migrationBuilder.DropTable(
                name: "ProductLot");

            migrationBuilder.DropTable(
                name: "DiscountPrice");

            migrationBuilder.AlterColumn<string>(
                name: "ListVoucher",
                table: "VoucherType",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(250)",
                oldMaxLength: 250,
                oldNullable: true);
        }
    }
}
