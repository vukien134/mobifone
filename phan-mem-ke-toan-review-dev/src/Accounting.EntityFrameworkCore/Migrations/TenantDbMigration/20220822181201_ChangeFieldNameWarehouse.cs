using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ChangeFieldNameWarehouse : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Warehouse_OrgCode_Code",
                table: "Warehouse");

            migrationBuilder.RenameColumn(
                name: "ParentWarehouseId",
                table: "Warehouse",
                newName: "ParentId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WarehouseBook",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ManufaturingCountry",
                table: "ProductLot",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Ledger",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

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
            //        CreatorName = table.Column<string>(type: "text", nullable: true),
            //        LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            //        OrgCode = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Unit", x => x.Id);
            //    });

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_OrgCode_Code",
                table: "Warehouse",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "NameTemp", "ParentId", "WarehouseAcc", "WarehouseRank", "WarehouseType" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Unit_OrgCode_Code",
            //    table: "Unit",
            //    columns: new[] { "OrgCode", "Code" },
            //    unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WarehouseBook_ProductVoucher_ProductVoucherId",
                table: "WarehouseBook",
                column: "ProductVoucherId",
                principalTable: "ProductVoucher",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WarehouseBook_ProductVoucher_ProductVoucherId",
                table: "WarehouseBook");

            migrationBuilder.DropTable(
                name: "Unit");

            migrationBuilder.DropIndex(
                name: "IX_Warehouse_OrgCode_Code",
                table: "Warehouse");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Warehouse",
                newName: "ParentWarehouseId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WarehouseBook",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ManufaturingCountry",
                table: "ProductLot",
                type: "timestamp without time zone",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Ledger",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Warehouse_OrgCode_Code",
                table: "Warehouse",
                columns: new[] { "OrgCode", "Code" },
                unique: true)
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "NameTemp", "ParentWarehouseId", "WarehouseAcc", "WarehouseRank", "WarehouseType" });
        }
    }
}
