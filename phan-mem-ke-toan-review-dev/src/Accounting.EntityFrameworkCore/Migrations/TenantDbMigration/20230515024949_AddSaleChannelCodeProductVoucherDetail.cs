using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class AddSaleChannelCodeProductVoucherDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SalechannelCode",
                table: "ProductVoucherDetail",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SalechannelCode",
                table: "ProductVoucherDetail");
        }
    }
}
