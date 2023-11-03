using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class ConfigTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExportExcelTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UrlApi = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportExcelTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportExcelTemplate_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportExcelTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", nullable: false),
                    UrlApi = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    RowBegin = table.Column<int>(type: "integer", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportExcelTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportExcelTemplate_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Language",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Language", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportMenuShortcut",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Icon = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    IconColor = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    VisibleWhen = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    Parameter = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    OriginReportId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    ReferenceReportId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    ReferenceWindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportMenuShortcut", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FileTemplate = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    UrlApiData = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    ReportType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    GridType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    InfoWindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplate", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VoucherTemplate",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FileTemplate = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    UrlApi = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherTemplate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherTemplate_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExportExcelTemplateColumn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    ExportExcelTemplateId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExportExcelTemplateColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExportExcelTemplateColumn_ExportExcelTemplate",
                        column: x => x.ExportExcelTemplateId,
                        principalTable: "ExportExcelTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImportExcelTemplateColumn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    ImportExcelTemplateId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    FieldType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ExcelCol = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImportExcelTemplateColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImportExcelTemplateColumn_ImportExcelTemplate",
                        column: x => x.ImportExcelTemplateId,
                        principalTable: "ImportExcelTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LanguageDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    LanguageId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Key = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    Value = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LanguageDetail_Language",
                        column: x => x.LanguageId,
                        principalTable: "Language",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InfoWindow",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    TypeInfoWindow = table.Column<string>(type: "text", nullable: true),
                    UrlApi = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    MaxRowInForm = table.Column<int>(type: "integer", nullable: true),
                    WindowId = table.Column<string>(type: "character varying(24)", nullable: true),
                    ReportTemplateId = table.Column<string>(type: "character varying(24)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoWindow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfoWindow_ReportTemplate",
                        column: x => x.ReportTemplateId,
                        principalTable: "ReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InfoWindow_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReportTemplateColumn",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: false),
                    ReportTemplateId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    FieldType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Hidden = table.Column<bool>(type: "boolean", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportTemplateColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportTemplateColumn_ReportTemplate",
                        column: x => x.ReportTemplateId,
                        principalTable: "ReportTemplate",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InfoWindowDetail",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Ord = table.Column<int>(type: "integer", nullable: true),
                    InfoWindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    FieldName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Caption = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    TypeEditor = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    LabelPosition = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    LabelWidth = table.Column<string>(type: "text", nullable: true),
                    Format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    Width = table.Column<int>(type: "integer", nullable: true),
                    Height = table.Column<int>(type: "integer", nullable: true),
                    ReferenceId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    DefaultValue = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    Hidden = table.Column<bool>(type: "boolean", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InfoWindowDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InfoWindowDetail_InfoWindow",
                        column: x => x.InfoWindowId,
                        principalTable: "InfoWindow",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExportExcelTemplate_Code",
                table: "ExportExcelTemplate",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExportExcelTemplate_WindowId",
                table: "ExportExcelTemplate",
                column: "WindowId");

            migrationBuilder.CreateIndex(
                name: "IX_ExportExcelTemplateColumn_ExportExcelTemplateId",
                table: "ExportExcelTemplateColumn",
                column: "ExportExcelTemplateId")
                .Annotation("Npgsql:IndexInclude", new[] { "Ord", "FieldName", "Caption", "FieldType", "Format" });

            migrationBuilder.CreateIndex(
                name: "IX_ImportExcelTemplate_Code",
                table: "ImportExcelTemplate",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImportExcelTemplate_WindowId",
                table: "ImportExcelTemplate",
                column: "WindowId");

            migrationBuilder.CreateIndex(
                name: "IX_ImportExcelTemplateColumn_ImportExcelTemplateId",
                table: "ImportExcelTemplateColumn",
                column: "ImportExcelTemplateId")
                .Annotation("Npgsql:IndexInclude", new[] { "Ord", "FieldName", "Caption", "FieldType", "ExcelCol", "DefaultValue" });

            migrationBuilder.CreateIndex(
                name: "IX_InfoWindow_Code",
                table: "InfoWindow",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_InfoWindow_ReportTemplateId",
                table: "InfoWindow",
                column: "ReportTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InfoWindow_WindowId",
                table: "InfoWindow",
                column: "WindowId");

            migrationBuilder.CreateIndex(
                name: "IX_InfoWindowDetail_InfoWindowId",
                table: "InfoWindowDetail",
                column: "InfoWindowId")
                .Annotation("Npgsql:IndexInclude", new[] { "Ord", "FieldName", "Caption", "DefaultValue" });

            migrationBuilder.CreateIndex(
                name: "IX_Language_Code",
                table: "Language",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LanguageDetail_LanguageId_Key",
                table: "LanguageDetail",
                columns: new[] { "LanguageId", "Key" })
                .Annotation("Npgsql:IndexInclude", new[] { "Value" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportMenuShortcut_OriginReportId_ReferenceReportId_Referen~",
                table: "ReportMenuShortcut",
                columns: new[] { "OriginReportId", "ReferenceReportId", "ReferenceWindowId" })
                .Annotation("Npgsql:IndexInclude", new[] { "Name", "Caption", "Icon", "IconColor" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplate_Code",
                table: "ReportTemplate",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReportTemplateColumn_ReportTemplateId",
                table: "ReportTemplateColumn",
                column: "ReportTemplateId")
                .Annotation("Npgsql:IndexInclude", new[] { "Ord", "FieldName", "Caption", "Format" });

            migrationBuilder.CreateIndex(
                name: "IX_VoucherTemplate_Code",
                table: "VoucherTemplate",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoucherTemplate_WindowId",
                table: "VoucherTemplate",
                column: "WindowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExportExcelTemplateColumn");

            migrationBuilder.DropTable(
                name: "ImportExcelTemplateColumn");

            migrationBuilder.DropTable(
                name: "InfoWindowDetail");

            migrationBuilder.DropTable(
                name: "LanguageDetail");

            migrationBuilder.DropTable(
                name: "ReportMenuShortcut");

            migrationBuilder.DropTable(
                name: "ReportTemplateColumn");

            migrationBuilder.DropTable(
                name: "VoucherTemplate");

            migrationBuilder.DropTable(
                name: "ExportExcelTemplate");

            migrationBuilder.DropTable(
                name: "ImportExcelTemplate");

            migrationBuilder.DropTable(
                name: "InfoWindow");

            migrationBuilder.DropTable(
                name: "Language");

            migrationBuilder.DropTable(
                name: "ReportTemplate");
        }
    }
}
