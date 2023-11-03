using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.HostDbMigration
{
    public partial class addCategoryDeletes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CategoryDelete",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    TabName = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    Name = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    FieldCode = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ConditionField = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    ConditionValue = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: true),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryDelete", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryDelete_TabName",
                table: "CategoryDelete",
                column: "TabName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryDelete");
        }
    }
}
