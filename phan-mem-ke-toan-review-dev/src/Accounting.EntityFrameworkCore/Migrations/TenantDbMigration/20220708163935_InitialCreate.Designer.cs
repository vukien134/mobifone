﻿// <auto-generated />
using System;
using Accounting.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;

#nullable disable

namespace Accounting.Migrations.TenantDbMigration
{
    [DbContext(typeof(TenancyDbContext))]
    [Migration("20220708163935_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("_Abp_DatabaseProvider", EfCoreDatabaseProvider.PostgreSql)
                .HasAnnotation("_Abp_MultiTenancySide", MultiTenancySides.Tenant)
                .HasAnnotation("ProductVersion", "6.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Accounting.Categories.Accounts.AccountSystem", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.Property<string>("AccCode")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("AccName")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("AccNameEn")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("AccNameTemp")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<string>("AccNameTempE")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<int>("AccPattern")
                        .HasColumnType("integer");

                    b.Property<int>("AccRank")
                        .HasColumnType("integer");

                    b.Property<string>("AccSectionCode")
                        .HasMaxLength(30)
                        .HasColumnType("character varying(30)");

                    b.Property<string>("AssetOrEquity")
                        .HasMaxLength(2)
                        .HasColumnType("character varying(2)");

                    b.Property<string>("AttachAccSection")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("AttachContract")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("AttachCurrency")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("AttachPartner")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("AttachProductCost")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("AttachVoucher")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("AttachWorkPlace")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<string>("BankAccountNumber")
                        .HasMaxLength(20)
                        .HasColumnType("character varying(20)");

                    b.Property<string>("BankName")
                        .HasMaxLength(250)
                        .HasColumnType("character varying(250)");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("CreationTime");

                    b.Property<Guid?>("CreatorId")
                        .HasColumnType("uuid")
                        .HasColumnName("CreatorId");

                    b.Property<string>("IsBalanceSheetAcc")
                        .HasMaxLength(1)
                        .HasColumnType("character varying(1)");

                    b.Property<DateTime?>("LastModificationTime")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("LastModificationTime");

                    b.Property<Guid?>("LastModifierId")
                        .HasColumnType("uuid")
                        .HasColumnName("LastModifierId");

                    b.Property<string>("OrgCode")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)");

                    b.Property<string>("ParentAccId")
                        .HasMaxLength(24)
                        .HasColumnType("character varying(24)");

                    b.Property<string>("Province")
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("uuid")
                        .HasColumnName("TenantId");

                    b.Property<int>("Year")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("AccountSystem", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}