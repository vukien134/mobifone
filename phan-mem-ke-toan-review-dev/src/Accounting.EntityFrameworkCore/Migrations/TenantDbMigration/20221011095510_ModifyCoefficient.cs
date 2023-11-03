using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class ModifyCoefficient : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelCode",
                table: "WarehouseBook",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "September",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "October",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "November",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "May",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "March",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "June",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "July",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "January",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "February",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "December",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "August",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.AlterColumn<decimal>(
                name: "April",
                table: "GroupCoefficientDetail",
                type: "numeric",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,4)");

            migrationBuilder.CreateTable(
                name: "InfoExportAuto",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    VoucherCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    ProductionPeriodCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OrdGrp = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    OrdRec = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "text", nullable: true),
                    LastModifierName = table.Column<string>(type: "text", nullable: true),
                    OrgCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoExportAuto", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InfoZ",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    BeginM = table.Column<DateTime>(type: "date", nullable: false),
                    EndM = table.Column<DateTime>(type: "date", nullable: false),
                    OrdGrp = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Type = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AllotmentForwardCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductionPeriodCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditFProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    PartnerCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ContractCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    AmountVat = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    RecordBook = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Ratio = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    BeginQuantity = table.Column<decimal>(type: "numeric(22,4)", nullable: false),
                    BeginAmount = table.Column<decimal>(type: "numeric(22,4)", nullable: false),
                    EndQuantity = table.Column<decimal>(type: "numeric(22,4)", nullable: false),
                    EndAmount = table.Column<decimal>(type: "numeric(22,4)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "text", nullable: true),
                    LastModifierName = table.Column<string>(type: "text", nullable: true),
                    OrgCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoZ", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InfoExportAuto_OrgCode",
                table: "InfoExportAuto",
                column: "OrgCode");

            migrationBuilder.CreateIndex(
                name: "IX_InfoZ_OrgCode",
                table: "InfoZ",
                column: "OrgCode");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InfoExportAuto");

            migrationBuilder.DropTable(
                name: "InfoZ");

            migrationBuilder.DropColumn(
                name: "ChannelCode",
                table: "WarehouseBook");

            migrationBuilder.AlterColumn<decimal>(
                name: "September",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "October",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "November",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "May",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "March",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "June",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "July",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "January",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "February",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "December",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "August",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "April",
                table: "GroupCoefficientDetail",
                type: "numeric(18,4)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "numeric",
                oldNullable: true);
        }
    }
}
