using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "WarehouseBook",
            //    type: "character varying(2)",
            //    maxLength: 2,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "text",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "ProductVoucher",
            //    type: "character varying(2)",
            //    maxLength: 2,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "character varying(50)",
            //    oldMaxLength: 50,
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "Status",
            //    table: "Ledger",
            //    type: "character varying(2)",
            //    maxLength: 2,
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "text",
            //    oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "AccVoucher",
                type: "character varying(2)",
                maxLength: 2,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "AccVoucher");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "WarehouseBook",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "ProductVoucher",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Ledger",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(2)",
                oldMaxLength: 2,
                oldNullable: true);
        }
    }
}
