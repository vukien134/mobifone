using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddUserNameField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "WarehouseBook",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "WarehouseBook",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Warehouse",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "Warehouse",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "VoucherCategory",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "VoucherCategory",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductVoucherVat",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductVoucherVat",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductVoucherReceipt",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductVoucherReceipt",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductVoucherDetailReceipt",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductVoucherDetailReceipt",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductVoucherDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductVoucherDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductVoucherAssembly",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductVoucherAssembly",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductVoucher",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductVoucher",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductUnit",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductUnit",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductPrice",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductPrice",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "ProductOpeningBalance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "ProductOpeningBalance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Product",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "Product",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "OrgUnit",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "OrgUnit",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Ledger",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "Ledger",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "FProductWork",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "FProductWork",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "Currency",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "Currency",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "BankPartner",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "BankPartner",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccVoucherDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccVoucherDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccVoucher",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccVoucher",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccTaxDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccTaxDetail",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccSection",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccSection",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccPartner",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccPartner",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccountSystem",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccountSystem",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatorName",
                table: "AccOpeningBalance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifierName",
                table: "AccOpeningBalance",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "WarehouseBook");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "WarehouseBook");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "Warehouse");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "VoucherCategory");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "VoucherCategory");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductVoucherVat");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductVoucherVat");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductVoucherReceipt");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductVoucherReceipt");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductVoucherDetailReceipt");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductVoucherDetailReceipt");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductVoucherDetail");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductVoucherDetail");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductVoucherAssembly");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductVoucherAssembly");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductUnit");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductUnit");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductPrice");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductPrice");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "ProductOpeningBalance");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "ProductOpeningBalance");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "OrgUnit");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "OrgUnit");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Ledger");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "Ledger");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "FProductWork");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "FProductWork");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "Currency");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "BankPartner");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "BankPartner");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccVoucherDetail");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccVoucherDetail");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccVoucher");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccVoucher");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccTaxDetail");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccTaxDetail");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccSection");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccSection");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccPartner");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccPartner");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccountSystem");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccountSystem");

            migrationBuilder.DropColumn(
                name: "CreatorName",
                table: "AccOpeningBalance");

            migrationBuilder.DropColumn(
                name: "LastModifierName",
                table: "AccOpeningBalance");
        }
    }
}
