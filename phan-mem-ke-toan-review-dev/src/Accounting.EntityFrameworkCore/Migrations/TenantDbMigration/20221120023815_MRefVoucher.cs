﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    public partial class MRefVoucher : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefVoucher",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    SrcId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    DestId = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: true),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatorName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LastModifierName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OrgCode = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefVoucher", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefVoucher");
        }
    }
}
