using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyQuantityColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxImportQuantity",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxExportQuantity",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityCur",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ImportQuantity",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExportQuantity",
                table: "WarehouseBook",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "VoucherExciseTax",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "ProductVoucherDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductVoucherDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "ProductVoucherAssembly",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductVoucherAssembly",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantity",
                table: "ProductVoucher",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductOpeningBalance",
                type: "numeric(22,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinQuantity",
                table: "Product",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxQuantity",
                table: "Product",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "Ledger",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxPromotionQuantity",
                table: "Ledger",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Ledger",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PromotionQuantity",
                table: "Ledger",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InvoiceAuthDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ShortQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OverQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InventoryQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AuditQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityLoss",
                table: "FProductWorkNormDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "FProductWorkNormDetail",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "FProductWorkNorm",
                type: "numeric(22,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "AssetToolAccessory",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "AssetTool",
                type: "numeric(22,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(12,8)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxImportQuantity",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxExportQuantity",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityCur",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ImportQuantity",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ExportQuantity",
                table: "WarehouseBook",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "VoucherExciseTax",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "ProductVoucherDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductVoucherDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "ProductVoucherAssembly",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductVoucherAssembly",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalQuantity",
                table: "ProductVoucher",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "ProductOpeningBalance",
                type: "numeric(12,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "MinQuantity",
                table: "Product",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "MaxQuantity",
                table: "Product",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxQuantity",
                table: "Ledger",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "TrxPromotionQuantity",
                table: "Ledger",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "Ledger",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PromotionQuantity",
                table: "Ledger",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "InvoiceAuthDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "ShortQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "OverQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "InventoryQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "AuditQuantity",
                table: "InventoryRecordDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "QuantityLoss",
                table: "FProductWorkNormDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "FProductWorkNormDetail",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "FProductWorkNorm",
                type: "numeric(12,8)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "AssetToolAccessory",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Quantity",
                table: "AssetTool",
                type: "numeric(12,8)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(22,8)",
                oldNullable: true);
        }
    }
}
