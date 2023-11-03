using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class AddEvent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    Code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    TypeEvent = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    EventObject = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventSetting", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RegisterEvent",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    WindowId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    TabId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    FieldId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    EventSettingId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegisterEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RegisterEvent_Event",
                        column: x => x.EventSettingId,
                        principalTable: "EventSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegisterEvent_Field",
                        column: x => x.FieldId,
                        principalTable: "Field",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegisterEvent_Tab",
                        column: x => x.TabId,
                        principalTable: "Tab",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RegisterEvent_Window",
                        column: x => x.WindowId,
                        principalTable: "Window",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventSetting_Code",
                table: "EventSetting",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegisterEvent_EventSettingId",
                table: "RegisterEvent",
                column: "EventSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterEvent_FieldId",
                table: "RegisterEvent",
                column: "FieldId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterEvent_TabId",
                table: "RegisterEvent",
                column: "TabId");

            migrationBuilder.CreateIndex(
                name: "IX_RegisterEvent_WindowId",
                table: "RegisterEvent",
                column: "WindowId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegisterEvent");

            migrationBuilder.DropTable(
                name: "EventSetting");
        }
    }
}
