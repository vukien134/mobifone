using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyBookThz : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditFProductWorkCode",
                table: "BookThz");

            migrationBuilder.DropColumn(
                name: "DebitFProducWorkCode",
                table: "BookThz");

            migrationBuilder.AddColumn<string>(
                name: "AttachCreditFProductWork",
                table: "BookThz",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AttachDebitFProducWork",
                table: "BookThz",
                type: "character varying(1)",
                maxLength: 1,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttachCreditFProductWork",
                table: "BookThz");

            migrationBuilder.DropColumn(
                name: "AttachDebitFProducWork",
                table: "BookThz");

            migrationBuilder.AddColumn<string>(
                name: "CreditFProductWorkCode",
                table: "BookThz",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DebitFProducWorkCode",
                table: "BookThz",
                type: "character varying(30)",
                maxLength: 30,
                nullable: true);
        }
    }
}
