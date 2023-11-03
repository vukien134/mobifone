using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class UpdateFStatement133 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Bold",
                table: "DefaultFStatement133L07",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bold",
                table: "DefaultFStatement133L06",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bold",
                table: "DefaultFStatement133L05",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bold",
                table: "DefaultFStatement133L04",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bold",
                table: "DefaultFStatement133L03",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Bold",
                table: "DefaultFStatement133L02",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldMaxLength: 1,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Bold",
                table: "DefaultFStatement133L07",
                type: "integer",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Bold",
                table: "DefaultFStatement133L06",
                type: "integer",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Bold",
                table: "DefaultFStatement133L05",
                type: "integer",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Bold",
                table: "DefaultFStatement133L04",
                type: "integer",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Bold",
                table: "DefaultFStatement133L03",
                type: "integer",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Bold",
                table: "DefaultFStatement133L02",
                type: "integer",
                maxLength: 1,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(1)",
                oldMaxLength: 1,
                oldNullable: true);
        }
    }
}
