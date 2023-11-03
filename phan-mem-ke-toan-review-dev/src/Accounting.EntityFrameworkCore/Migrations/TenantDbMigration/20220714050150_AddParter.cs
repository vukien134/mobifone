using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddParter : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrgCode",
                table: "AccountSystem",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccCode",
                table: "AccountSystem",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AccPartner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PartnerGroupId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    PartnerType = table.Column<int>(type: "integer", maxLength: 250, nullable: false),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Tel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OtherContact = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ContactPerson = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Fax = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Representative = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    DebtCeiling = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    DebtCeilingCur = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    InfoFilter = table.Column<string>(type: "text", nullable: true),
                    InvoiceSearchLink = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccPartner", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrgUnit",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NameE = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    TaxAuthorityCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TaxAuthorityName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Signee = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SubmittingOrganiztion = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    SubmittingAddress = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Wards = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    District = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Province = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Producer = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ChiefAccountant = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Director = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CareerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrgUnit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Warehouse",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    NameTemp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    WarehouseType = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    WarehouseRank = table.Column<int>(type: "integer", nullable: false),
                    ParentWarehouseId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    WarehouseAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Warehouse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankPartner",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord0 = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    PartnerId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BankAccNumber = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Tel = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankPartner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankPartner_AccPartner",
                        column: x => x.PartnerId,
                        principalTable: "AccPartner",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AccountSystem_OrgCode_AccCode",
                table: "AccountSystem",
                columns: new[] { "OrgCode", "AccCode" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "AccName", "AccNameEn", "AccRank", "AttachPartner", "AttachAccSection", "AttachContract", "AttachCurrency", "AttachProductCost", "AttachVoucher", "AttachWorkPlace" });

            migrationBuilder.CreateIndex(
                name: "IX_AccPartner_OrgCode_Code",
                table: "AccPartner",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "TaxCode", "PartnerGroupId", "PartnerType" });

            migrationBuilder.CreateIndex(
                name: "IX_BankPartner_PartnerId",
                table: "BankPartner",
                column: "PartnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrgUnit_Code",
                table: "OrgUnit",
                column: "Code",
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "NameE", "TaxCode" });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_OrgCode_Code",
                table: "Warehouse",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "NameTemp", "ParentWarehouseId", "WarehouseAcc", "WarehouseRank", "WarehouseType" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankPartner");

            migrationBuilder.DropTable(
                name: "OrgUnit");

            migrationBuilder.DropTable(
                name: "Warehouse");

            migrationBuilder.DropTable(
                name: "AccPartner");

            migrationBuilder.DropIndex(
                name: "IX_AccountSystem_OrgCode_AccCode",
                table: "AccountSystem");

            migrationBuilder.AlterColumn<string>(
                name: "OrgCode",
                table: "AccountSystem",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AccCode",
                table: "AccountSystem",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30,
                oldNullable: true);
        }
    }
}
