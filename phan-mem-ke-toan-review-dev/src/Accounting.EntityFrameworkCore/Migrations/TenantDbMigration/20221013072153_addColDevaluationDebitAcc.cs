using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addColDevaluationDebitAcc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DevaluationCreditAcc",
                table: "ProductVoucher",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevaluationDebitAcc",
                table: "ProductVoucher",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DevaluationCreditAcc",
                table: "ProductVoucher");

            migrationBuilder.DropColumn(
                name: "DevaluationDebitAcc",
                table: "ProductVoucher");
        }
    }
}
