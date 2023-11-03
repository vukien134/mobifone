using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class addTable20221005 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AllotmentForwardCategory",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    DecideApply = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    FProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Type = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    OrdGrp = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ForwardType = table.Column<int>(type: "integer", nullable: false),
                    LstCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    GroupCoefficientCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductionPeriodCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    DebitCredit = table.Column<int>(type: "integer", nullable: false),
                    CreditAcc = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreditSectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Note = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Active = table.Column<int>(type: "integer", nullable: false),
                    RecordBook = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    AttachProduct = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    QuantityType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    NormType = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductionPeriodType = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_AllotmentForwardCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ConfigCostPrice",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    Fifo = table.Column<int>(type: "integer", nullable: false),
                    ConsecutiveMonth = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_ConfigCostPrice", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FProductWorkNorm",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_FProductWorkNorm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupCoefficient",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ApplicableDate1 = table.Column<DateTime>(type: "date", nullable: true),
                    ApplicableDate2 = table.Column<DateTime>(type: "date", nullable: true),
                    Description = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
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
                    table.PrimaryKey("PK_GroupCoefficient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FProductWorkNormDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FProductWorkNormId = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    BeginDate = table.Column<DateTime>(type: "date", nullable: false),
                    EndDate = table.Column<DateTime>(type: "date", nullable: false),
                    Ord0 = table.Column<string>(type: "text", nullable: true),
                    AccCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WorkPlaceCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    SectionCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WarehouseCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductLotCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ProductOrigin = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    UnitCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Quantity = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    QuantityLoss = table.Column<decimal>(type: "numeric(12,8)", nullable: true),
                    PercentLoss = table.Column<decimal>(type: "numeric(12,4)", nullable: true),
                    PriceCur = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    Price = table.Column<decimal>(type: "numeric(22,8)", nullable: true),
                    AmountCur = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    Amount = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    ApplicableDate1 = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ApplicableDate2 = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FProductWorkNormDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FProductWorkNormDetail_FProductWorkNorm",
                        column: x => x.FProductWorkNormId,
                        principalTable: "FProductWorkNorm",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GroupCoefficientDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    GroupCoefficientId = table.Column<string>(type: "text", nullable: false),
                    FProductWork = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    FProductWorkCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    GroupCoefficientCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    January = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    February = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    March = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    April = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    May = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    June = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    July = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    August = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    September = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    October = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    November = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    December = table.Column<decimal>(type: "numeric(18,4)", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupCoefficientDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GroupCoefficientDetail_GroupCoefficient",
                        column: x => x.GroupCoefficientId,
                        principalTable: "GroupCoefficient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AllotmentForwardCategory_OrgCode_Code",
                table: "AllotmentForwardCategory",
                columns: new[] { "OrgCode", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_ConfigCostPrice_OrgCode",
                table: "ConfigCostPrice",
                column: "OrgCode");

            migrationBuilder.CreateIndex(
                name: "IX_FProductWorkNorm_OrgCode_Year_FProductWorkCode",
                table: "FProductWorkNorm",
                columns: new[] { "OrgCode", "Year", "FProductWorkCode" });

            migrationBuilder.CreateIndex(
                name: "IX_FProductWorkNormDetail_FProductWorkNormId",
                table: "FProductWorkNormDetail",
                column: "FProductWorkNormId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupCoefficient_OrgCode_Code",
                table: "GroupCoefficient",
                columns: new[] { "OrgCode", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_GroupCoefficientDetail_GroupCoefficientId",
                table: "GroupCoefficientDetail",
                column: "GroupCoefficientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AllotmentForwardCategory");

            migrationBuilder.DropTable(
                name: "ConfigCostPrice");

            migrationBuilder.DropTable(
                name: "FProductWorkNormDetail");

            migrationBuilder.DropTable(
                name: "GroupCoefficientDetail");

            migrationBuilder.DropTable(
                name: "FProductWorkNorm");

            migrationBuilder.DropTable(
                name: "GroupCoefficient");
        }
    }
}
