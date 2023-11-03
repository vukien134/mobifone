using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class MAddUnits : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Unit",
            //    columns: table => new
            //    {
            //        Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
            //        Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
            //        CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
            //        LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
            //        TenantId = table.Column<Guid>(type: "uuid", nullable: true),
            //        CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            //        LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            //        OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Unit", x => x.Id);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Unit_OrgCode_Code",
            //    table: "Unit",
            //    columns: new[] { "OrgCode", "Code" },
            //    unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Unit");
        }
    }
}
