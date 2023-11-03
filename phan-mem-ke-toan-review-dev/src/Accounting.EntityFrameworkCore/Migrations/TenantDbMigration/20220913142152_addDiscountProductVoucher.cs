using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addDiscountProductVoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DiscountCreditAcc",
                table: "ProductVoucher",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DiscountDebitAcc",
                table: "ProductVoucher",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPercentage",
                table: "ProductVoucher",
                type: "numeric(12,4)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentCreditAcc",
                table: "ProductVoucher",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentDebitAcc",
                table: "ProductVoucher",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DiscountCreditAcc",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "DiscountDebitAcc",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "DiscountPercentage",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "PaymentCreditAcc",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "PaymentDebitAcc",
                table: "ProductVoucher");
        }
    }
}
