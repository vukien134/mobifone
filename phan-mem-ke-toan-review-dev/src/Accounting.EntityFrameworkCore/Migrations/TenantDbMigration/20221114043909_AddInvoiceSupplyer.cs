using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddInvoiceSupplyer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InvoiceSupplier",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Active = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    Link = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    UserName = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Password = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PartnerGuid = table.Column<Guid>(type: "uuid", nullable: true),
                    PartnerToken = table.Column<string>(type: "text", nullable: true),
                    AppId = table.Column<Guid>(type: "uuid", nullable: true),
                    Url = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    UserSevice = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    PassSevice = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CertificateSerial = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CheckCircular = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    TaxCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
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
                    table.PrimaryKey("PK_InvoiceSupplier", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvoiceSupplier_OrgCode_Code",
                table: "InvoiceSupplier",
                columns: new[] { "OrgCode", "Code" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvoiceSupplier");
        }
    }
}
