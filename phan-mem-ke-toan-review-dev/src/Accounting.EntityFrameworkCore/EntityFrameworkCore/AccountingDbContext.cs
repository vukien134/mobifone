using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.CategoryDeletes;
using Accounting.Categories.CostProductions;
using Accounting.Categories.Menus;
using Accounting.Categories.Others;
using Accounting.Categories.Salaries;
using Accounting.Configs;
using Accounting.Licenses;
using Accounting.Reports;
using Accounting.Reports.Statements.T133.Defaults;
using Accounting.Reports.Statements.T200.Defaults;
using Accounting.Tenants;
using Accounting.Vouchers;
using Accounting.Windows;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.BackgroundJobs.EntityFrameworkCore;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.IdentityServer.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;

namespace Accounting.EntityFrameworkCore;

[ReplaceDbContext(typeof(IIdentityDbContext))]
[ReplaceDbContext(typeof(ITenantManagementDbContext))]
[ConnectionStringName("Default")]
public class AccountingDbContext :
    AbpDbContext<AccountingDbContext>,
    IIdentityDbContext,
    ITenantManagementDbContext
{
    /* Add DbSet properties for your Aggregate Roots / Entities here. */

    #region Entities from the modules

    /* Notice: We only implemented IIdentityDbContext and ITenantManagementDbContext
     * and replaced them for this DbContext. This allows you to perform JOIN
     * queries for the entities of these modules over the repositories easily. You
     * typically don't need that for other modules. But, if you need, you can
     * implement the DbContext interface of the needed module and use ReplaceDbContext
     * attribute just like IIdentityDbContext and ITenantManagementDbContext.
     *
     * More info: Replacing a DbContext of a module ensures that the related module
     * uses this DbContext on runtime. Otherwise, it will use its own DbContext class.
     */

    //Identity
    public DbSet<IdentityUser> Users { get; set; }
    public DbSet<IdentityRole> Roles { get; set; }
    public DbSet<IdentityClaimType> ClaimTypes { get; set; }
    public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
    public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
    public DbSet<IdentityLinkUser> LinkUsers { get; set; }

    // Tenant Management
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantConnectionString> TenantConnectionStrings { get; set; }

    #endregion

    public AccountingDbContext(DbContextOptions<AccountingDbContext> options)
        : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.SetMultiTenancySide(MultiTenancySides.Host);
        base.OnModelCreating(builder);

        /* Include modules to your migration db context */

        builder.ConfigurePermissionManagement();
        builder.ConfigureSettingManagement();
        builder.ConfigureBackgroundJobs();
        builder.ConfigureAuditLogging();
        builder.ConfigureIdentity();
        builder.ConfigureIdentityServer();
        builder.ConfigureFeatureManagement();
        builder.ConfigureTenantManagement();

        /* Configure your own tables/entities inside here */

        //builder.Entity<YourEntity>(b =>
        //{
        //    b.ToTable(AccountingConsts.DbTablePrefix + "YourEntities", AccountingConsts.DbSchema);
        //    b.ConfigureByConvention(); //auto configure for the base class props
        //    //...
        //});
        builder.Entity<MenuAccounting>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "MenuAccounting", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Detail).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Icon).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Url).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.JavaScriptCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.ViewPermission).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.HasIndex(e => e.Code)
                .IsUnique()
                .IncludeProperties(e => new
                {
                    e.Name,
                    e.windowId,
                    e.ParentId
                });
        });
        builder.Entity<Window>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Window", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.WindowType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.FormLayout).HasColumnType(AccountingConsts.TextColumnType);
            b.Property(e => e.UrlApiExportExcel).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.UrlApiImportExcel).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.InfoWindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasIndex(e => e.Code)
                .IsUnique();
        });

        builder.Entity<Tab>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Tab", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.OrderBy).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.TabTable).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.TabType).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.TabView).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.UrlApiDetail).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.UrlApiCrud).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.UrlApiData).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.UrlApiTabDetail).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
            b.HasIndex(e => e.Code)
                .IsUnique()
                .IncludeProperties(e => new
                {
                    e.Name,
                    e.WindowId
                });
            b.HasOne(e => e.Window)
                    .WithMany(p => p.Tabs)
                    .HasForeignKey(d => d.WindowId)
                    .HasConstraintName("FK_Tab_Window")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.WindowId);
        });
        builder.Entity<Field>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Field", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Format).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.LabelPosition).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.FieldType).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.TypeEditor).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.TypeFilter).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.Template).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.ReferenceId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
            b.Property(e => e.FieldExpression).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.DefaultValue).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Css).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.HasOne(e => e.Tab)
                    .WithMany(p => p.Fields)
                    .HasForeignKey(d => d.TabId)
                    .HasConstraintName("FK_Field_Tab")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.TabId);
        });
        builder.Entity<Reference>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Reference", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.RefType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.ListValue).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.Property(e => e.UrlApiData).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.ValueField).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.DisplayField).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => e.Code).IsUnique();
        });
        builder.Entity<ReferenceDetail>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ReferenceDetail", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Format).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Template).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.ReferenceId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasOne(e => e.Reference)
                    .WithMany(p => p.ReferenceDetails)
                    .HasForeignKey(d => d.ReferenceId)
                    .HasConstraintName("FK_ReferenceDetail_Reference")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.ReferenceId)
                .IncludeProperties(e => new
                {
                    e.Ord,
                    e.FieldName,
                    e.Caption,
                    e.Format
                });
        });

        builder.Entity<ReportTemplate>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ReportTemplate", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.FileTemplate).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.GridType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.InfoWindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.ReportType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.UrlApiData).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.ViewType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.HasIndex(e => e.Code).IsUnique();
        });
        builder.Entity<ReportTemplateColumn>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ReportTemplateColumn", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.FieldType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.Format).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.ReportTemplateId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.VndNt).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.HasOne(e => e.ReportTemplate)
                    .WithMany(p => p.ReportTemplateColumns)
                    .HasForeignKey(d => d.ReportTemplateId)
                    .HasConstraintName("FK_ReportTemplateColumn_ReportTemplate")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.ReportTemplateId)
                .IncludeProperties(e => new
                {
                    e.Ord,
                    e.FieldName,
                    e.Caption,
                    e.Format
                });
        });
        builder.Entity<ReportMenuShortcut>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ReportMenuShortcut", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.Icon).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.IconColor).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.OriginReportId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Parameter).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.ReferenceReportId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.ReferenceWindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.VisibleWhen).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.HasIndex(e => new
            {
                e.OriginReportId,
                e.ReferenceReportId,
                e.ReferenceWindowId
            }).IncludeProperties(e => new
            {
                e.Name,
                e.Caption,
                e.Icon,
                e.IconColor
            });
        });

        builder.Entity<ImportExcelTemplate>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ImportExcelTemplate", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.UrlApi).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.HelpContent).HasColumnType(AccountingConsts.TextColumnType);
            b.HasIndex(e => e.Code).IsUnique();
            b.HasOne(e => e.Window)
                    .WithMany(p => p.ImportExcelTemplates)
                    .HasForeignKey(d => d.WindowId)
                    .HasConstraintName("FK_ImportExcelTemplate_Window")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<ImportExcelTemplateColumn>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ImportExcelTemplateColumn", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.FieldType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.DefaultValue).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.ExcelCol).HasMaxLength(5);
            b.Property(e => e.ImportExcelTemplateId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasOne(e => e.ImportExcelTemplate)
                    .WithMany(p => p.ImportExcelTemplateColumns)
                    .HasForeignKey(d => d.ImportExcelTemplateId)
                    .HasConstraintName("FK_ImportExcelTemplateColumn_ImportExcelTemplate")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.ImportExcelTemplateId)
                .IncludeProperties(e => new
                {
                    e.Ord,
                    e.FieldName,
                    e.Caption,
                    e.FieldType,
                    e.ExcelCol,
                    e.DefaultValue
                });
        });

        builder.Entity<ExportExcelTemplate>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ExportExcelTemplate", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.UrlApi).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Title).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasIndex(e => e.Code).IsUnique();
            b.HasOne(e => e.Window)
                    .WithMany(p => p.ExportExcelTemplates)
                    .HasForeignKey(d => d.WindowId)
                    .HasConstraintName("FK_ExportExcelTemplate_Window")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<ExportExcelTemplateColumn>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ExportExcelTemplateColumn", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.FieldType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.Format).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.ExportExcelTemplateId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasOne(e => e.ExportExcelTemplate)
                    .WithMany(p => p.ExportExcelTemplateColumns)
                    .HasForeignKey(d => d.ExportExcelTemplateId)
                    .HasConstraintName("FK_ExportExcelTemplateColumn_ExportExcelTemplate")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.ExportExcelTemplateId)
                .IncludeProperties(e => new
                {
                    e.Ord,
                    e.FieldName,
                    e.Caption,
                    e.FieldType,
                    e.Format
                });
        });

        builder.Entity<InfoWindow>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "InfoWindow", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.UrlApi).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.HasIndex(e => e.Code).IsUnique();
            b.HasOne(e => e.ReportTemplate)
                    .WithMany(p => p.InfoWindows)
                    .HasForeignKey(d => d.ReportTemplateId)
                    .HasConstraintName("FK_InfoWindow_ReportTemplate")
                    .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<InfoWindowDetail>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "InfoWindowDetail", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Format).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.InfoWindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.DefaultValue).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.LabelPosition).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.ReferenceId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.TypeEditor).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.ValueChange).HasColumnType(AccountingConsts.TextColumnType);
            b.HasOne(e => e.InfoWindow)
                    .WithMany(p => p.InfoWindowDetails)
                    .HasForeignKey(d => d.InfoWindowId)
                    .HasConstraintName("FK_InfoWindowDetail_InfoWindow")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => e.InfoWindowId)
                .IncludeProperties(e => new
                {
                    e.Ord,
                    e.FieldName,
                    e.Caption,
                    e.DefaultValue
                });
        });

        builder.Entity<VoucherTemplate>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "VoucherTemplate", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.UrlApi).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.FileTemplate).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasIndex(e => e.Code).IsUnique();
            b.HasOne(e => e.Window)
                    .WithMany(p => p.VoucherTemplates)
                    .HasForeignKey(d => d.WindowId)
                    .HasConstraintName("FK_VoucherTemplate_Window")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Language>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Language", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => e.Code).IsUnique();
        });
        builder.Entity<LanguageDetail>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "LanguageDetail", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Key).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Value).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.LanguageId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasOne(e => e.Language)
                    .WithMany(p => p.LanguageDetails)
                    .HasForeignKey(d => d.LanguageId)
                    .HasConstraintName("FK_LanguageDetail_Language")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasIndex(e => new
            {
                e.LanguageId,
                e.Key
            })
                .IncludeProperties(e => new
                {
                    e.Value
                });
        });

        builder.Entity<Button>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Button", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Icon).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.IconColor).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.IsGroup).HasMaxLength(1);
            b.Property(e => e.ReportTemplateId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.ShortCut).HasMaxLength(5);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasIndex(e => e.Code).IsUnique();
            b.HasOne(e => e.Window)
                    .WithMany(p => p.Buttons)
                    .HasForeignKey(d => d.WindowId)
                    .HasConstraintName("FK_Button_Window")
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.ReportTemplate)
                    .WithMany(p => p.Buttons)
                    .HasForeignKey(d => d.ReportTemplateId)
                    .HasConstraintName("FK_ReportTemplate_Window")
                    .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<TenantExtendInfo>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "TenantExtendInfo", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.LicenseXml).HasColumnType(AccountingConsts.TextColumnType);
            b.HasIndex(e => e.TenantId).IsUnique();
        });
        builder.Entity<EventSetting>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "EventSetting", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Content).HasColumnType(AccountingConsts.TextColumnType);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.TypeEvent).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.EventObject).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => e.Code).IsUnique();
        });
        builder.Entity<RegisterEvent>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "RegisterEvent", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.WindowId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.TabId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.FieldId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.EventSettingId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.HasIndex(e => e.WindowId);
            b.HasIndex(e => e.TabId);
            b.HasIndex(e => e.FieldId);
            b.HasIndex(e => e.EventSettingId);
            b.HasOne(e => e.Window)
                    .WithMany(p => p.RegisterEvents)
                    .HasForeignKey(d => d.WindowId)
                    .HasConstraintName("FK_RegisterEvent_Window")
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Tab)
                    .WithMany(p => p.RegisterEvents)
                    .HasForeignKey(d => d.TabId)
                    .HasConstraintName("FK_RegisterEvent_Tab")
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.Field)
                    .WithMany(p => p.RegisterEvents)
                    .HasForeignKey(d => d.FieldId)
                    .HasConstraintName("FK_RegisterEvent_Field")
                    .OnDelete(DeleteBehavior.Cascade);
            b.HasOne(e => e.EventSetting)
                    .WithMany(p => p.RegisterEvents)
                    .HasForeignKey(d => d.EventSettingId)
                    .HasConstraintName("FK_RegisterEvent_Event")
                    .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<Circulars>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "Circulars", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.TitleE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.TitleV).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => e.Code);
        });
        builder.Entity<DefaultAccBalanceSheet>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultAccBalanceSheet", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.CarryingCurrency).HasMaxLength(1);
            b.Property(e => e.DebitOrCredit).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Edit).HasMaxLength(1);
            b.Property(e => e.EndingAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.EndingAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.IsSummary).HasMaxLength(1);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.OpeningAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.OpeningAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.TargetCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Type).HasMaxLength(1);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });

        builder.Entity<DefaultCashFollowStatement>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultCashFollowStatement", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.CarryingCurrency).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Edit).HasMaxLength(1);
            b.Property(e => e.LastPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.ThisPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.AccumulatedLastPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.AccumulatedThisPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.IsSummary).HasMaxLength(1);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.TargetCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });

        builder.Entity<DefaultBusinessResult>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultBusinessResult", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.CarryingCurrency).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Edit).HasMaxLength(1);
            b.Property(e => e.LastPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.ThisPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.AccumulatedLastPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.AccumulatedThisPeriod).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.IsSummary).HasMaxLength(1);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.TargetCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });

        builder.Entity<RegLicense>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "RegLicense", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.LicXml).HasColumnType(AccountingConsts.TextColumnType);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Key).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CompanyType).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
            b.HasIndex(e => new
            {
                e.CustomerTenantId
            });
        });

        builder.Entity<DefaultFStatement133L01>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L01", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description1).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Description2).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement133L02>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L02", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.DebitOrCredit).HasMaxLength(1);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ClosingAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.OpeningAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement133L03>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L03", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.DebitOrCredit).HasMaxLength(1);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ClosingAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.OpeningAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.IncreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DecreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement133L04>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L04", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.DebitOrCredit).HasMaxLength(1);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ClosingAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.OpeningAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.IncreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DecreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement133L05>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L05", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.DebitOrCredit).HasMaxLength(1);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.DebitBalance1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditBalance1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Debit).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Credit).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebitBalance2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditBalance2).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement133L06>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L06", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.DebitOrCredit).HasMaxLength(1);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ClosingAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.OpeningAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.IncreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DecreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement133L07>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement133L07", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(1);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Amount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount3).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount4).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount5).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount6).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount7).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount8).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Total).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultStatementTax>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultStatementTax", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Ord0).HasMaxLength(10);
            b.Property(e => e.AssignValue).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Condition).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Amount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Amount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.En1).HasMaxLength(2);
            b.Property(e => e.En2).HasMaxLength(2);
            b.Property(e => e.Re1).HasMaxLength(2);
            b.Property(e => e.Re2).HasMaxLength(2);
            b.Property(e => e.Va1).HasMaxLength(2);
            b.Property(e => e.Va2).HasMaxLength(2);
            b.Property(e => e.Mt1).HasMaxLength(2);
            b.Property(e => e.Mt2).HasMaxLength(2);
            b.Property(e => e.Id11).HasMaxLength(10);
            b.Property(e => e.Id12).HasMaxLength(10);
            b.Property(e => e.Id21).HasMaxLength(10);
            b.Property(e => e.Id22).HasMaxLength(10);
            b.Property(e => e.NumberCode1).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.NumberCode2).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.HasIndex(e => new
            {

                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L01>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L01", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L02>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L02", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(500);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.ClosingAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.OpeningAmount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L03>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L03", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.OriginalPriceAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.PreventivePriceAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.RecordingPriceAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.OriginalPrice2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.OriginalPrice1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventivePrice2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventivePrice1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.RecordingPrice2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.RecordingPrice1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L04>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L04", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.PreventiveAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ValueAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ValueAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.ValueAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventiveAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventiveAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L05>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L05", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L06>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L06", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(500);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L07>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L07", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.OriginalPriceAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.PreventivePriceAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.OriginalPrice2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.OriginalPrice1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventivePrice2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventivePrice1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L08>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L08", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L09>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L09", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Condition).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.HH1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.HH2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.HH3).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.HH4).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.HH5).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.HH6).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.HH7).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Total).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L10>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L10", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Condition).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.VH1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.VH2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.VH3).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.VH4).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.VH5).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.VH6).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.VH7).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Total).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L11>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L11", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Condition).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.TC1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.TC2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.TC3).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.TC4).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.TC5).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.TC6).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.TC7).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Total).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L12>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L12", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L13>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L13", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.ValueAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.ValueAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebtPayingAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebtPayingAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Up).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Down).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.InterestAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.InterestAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L14>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L14", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L15>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L15", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L16>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L16", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.PreventiveAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ValueAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ValueAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.ValueAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventiveAmount1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.PreventiveAmount2).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L17>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L17", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Type).HasMaxLength(2);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Condition).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.CreditBalance1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditBalance2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Debit).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Credit).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebitBalance1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebitBalance2).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L18>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L18", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<DefaultFStatement200L19>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFStatement200L19", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Title).HasMaxLength(2);
            b.Property(e => e.GroupId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitOrCredit).HasMaxLength(2);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.NV1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV3).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV4).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV5).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV6).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV7).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NV8).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Total).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.HasIndex(e => new
            {
                e.UsingDecision,
                e.Ord
            });
        });
        builder.Entity<ConfigForwardYear>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ConfigForwardYear", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.FieldNot).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.FieldValues).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.TableName).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Title).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.BusinessType).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.Ord
            });
        });
        builder.Entity<ConfigForwardOrg>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "ConfigForwardOrg", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.TableName).HasMaxLength(AccountingConsts.OtherFieldLength50);
        });
        builder.Entity<DbServer>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DbServer", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.UserName).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Password).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.DatabaseName).HasMaxLength(AccountingConsts.OtherFieldLength50);
        });
        builder.Entity<CustomerRegister>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "CustomerRegister", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.AccessCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Email).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Status).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Note).HasColumnType(AccountingConsts.TextColumnType);
            b.Property(e => e.CompanyType).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.PhoneNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
            b.Property(e => e.CompanyName).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.FullName).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.StartDate).HasColumnType(AccountingConsts.DateColumnType);
            b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
            b.HasIndex(e => e.AccessCode).IsUnique();
        });
        builder.Entity<DefaultAccountSystem>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultAccountSystem", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.AccCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.AccName).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.AccNameEn).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.AccNameTemp).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.AccNameTempE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.AccSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.AssetOrEquity).HasMaxLength(2);
            b.Property(e => e.AttachAccSection).HasMaxLength(1);
            b.Property(e => e.AttachContract).HasMaxLength(1);
            b.Property(e => e.AttachCurrency).HasMaxLength(1);
            b.Property(e => e.AttachPartner).HasMaxLength(1);
            b.Property(e => e.AttachProductCost).HasMaxLength(1);
            b.Property(e => e.AttachVoucher).HasMaxLength(1);
            b.Property(e => e.AttachWorkPlace).HasMaxLength(1);
            b.Property(e => e.BankAccountNumber).HasMaxLength(20);
            b.Property(e => e.BankName).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.IsBalanceSheetAcc).HasMaxLength(1);
            b.Property(e => e.ParentAccId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Province).HasMaxLength(AccountingConsts.ProvinceFieldLength);
            b.Property(e => e.AccType).HasMaxLength(2);
            b.Property(e => e.ParentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new { e.UsingDecision, e.AccCode })
                .IsUnique()
                .IncludeProperties(e => new
                {
                    e.AccName,
                    e.AccNameEn,
                    e.AccRank,
                    e.AttachPartner,
                    e.AttachAccSection,
                    e.AttachContract,
                    e.AttachCurrency,
                    e.AttachProductCost,
                    e.AttachVoucher,
                    e.AttachWorkPlace
                });
        });
        builder.Entity<DefaultThnvnn>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultThnvnn", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Bold).HasMaxLength(1);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Printable).HasMaxLength(1);
            b.Property(e => e.Acc).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Method).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Condition).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.CreditBalance1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.CreditBalance2).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Debit).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Credit).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebitBalance1).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.DebitBalance2).HasColumnType(AccountingConsts.AmountColumnType);
            b.HasIndex(e => new
            {
                e.Ord
            });
        });
        builder.Entity<LinkCode>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "LinkCode", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.FieldCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.RefFieldCode).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.RefTableName).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.HasIndex(e => e.FieldCode);
        });
        builder.Entity<PackageMobi>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "PackageMobi", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Type).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.UserQuantity).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.CompanyQuantity).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.Property(e => e.CompanyType).HasMaxLength(AccountingConsts.OtherFieldLength50);
            b.HasIndex(e => e.Code);
        });
        builder.Entity<DefaultTenantSetting>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultTenantSetting", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Key).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Type).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Value).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.SettingType).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.Key
            }).IsUnique().IncludeProperties(e => new
            {
                e.Value
            });
        });
        builder.Entity<DefaultVoucherCategory>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultVoucherCategory", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.AttachBusiness).HasMaxLength(1);
            b.Property(e => e.AttachPartnerPrice).HasMaxLength(1);
            b.Property(e => e.BookClosingDate).HasColumnType(AccountingConsts.DateColumnType);
            b.Property(e => e.BusinessBeginningDate).HasColumnType(AccountingConsts.DateColumnType);
            b.Property(e => e.BusinessEndingDate).HasColumnType(AccountingConsts.DateColumnType);
            b.Property(e => e.ChkDuplicateVoucherNumber).HasMaxLength(1);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.IncreaseNumberMethod).HasMaxLength(2);
            b.Property(e => e.IsAssembly).HasMaxLength(1);
            b.Property(e => e.IsCalculateBalanceAcc).HasMaxLength(1);
            b.Property(e => e.IsCalculateBalanceProduct).HasMaxLength(1);
            b.Property(e => e.IsSavingLedger).HasMaxLength(1);
            b.Property(e => e.IsSavingWarehouseBook).HasMaxLength(1);
            b.Property(e => e.IsTransfer).HasMaxLength(1);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.NameE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Prefix).HasMaxLength(5);
            b.Property(e => e.PriceCalculatingMethod).HasMaxLength(2);
            b.Property(e => e.ProductType).HasMaxLength(2);
            b.Property(e => e.SeparatorCharacter).HasMaxLength(1);
            b.Property(e => e.Suffix).HasMaxLength(5);
            b.Property(e => e.TaxType).HasMaxLength(2);
            b.Property(e => e.VoucherKind).HasMaxLength(2);
            b.Property(e => e.VoucherOrd).HasMaxLength(AccountingConsts.Ord0FieldLength);
            b.Property(e => e.VoucherType).HasMaxLength(10);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique();
        });
        builder.Entity<DefaultCurrency>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultCurrency", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Default).HasMaxLength(2);
            b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.NameE).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.OddCurrencyEN).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.OddCurrencyVN).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique();
        });
        builder.Entity<DefaultExciseTax>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultExciseTax", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
            b.Property(e => e.Htkk).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.Htkk0).HasMaxLength(AccountingConsts.OtherFieldLength30);
            b.Property(e => e.HtkkName).HasMaxLength(AccountingConsts.OtherFieldLength);
        });
        builder.Entity<DefaultVoucherType>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultVoucherType", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.Property(e => e.ListGroup).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.Property(e => e.ListVoucher).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique();
        });
        builder.Entity<DefaultHtkkNumberCode>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultHtkkNumberCode", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.CircularCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new
            {
                e.CircularCode,
                e.NumberCode
            });
        });
        builder.Entity<DefaultAssetToolGroup>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultAssetToolGroup", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.AssetOrTool).HasMaxLength(2);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.AssetOrTool,
                e.Code
            });
        });
        builder.Entity<DefaultTaxCategory>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultTaxCategory", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.OutOrIn).HasMaxLength(1);
            b.Property(e => e.Percentage).HasColumnType(AccountingConsts.PercentageColumnType);
            b.Property(e => e.Percetage0).HasColumnType(AccountingConsts.PercentageColumnType);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique();
        });
        builder.Entity<DefaultBusinessCategory>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultBusinessCategory", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Prefix).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.Separator).HasMaxLength(1);
            b.Property(e => e.Suffix).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique();
        });
        builder.Entity<DefaultCareer>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultCareer", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique().IncludeProperties(e => new
            {
                e.Name
            });
        });
        builder.Entity<DefaultFixError>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultFixError", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.ErrorId).HasMaxLength(AccountingConsts.OtherFieldLength10);
            b.Property(e => e.ErrorName).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.Property(e => e.KeyError).HasMaxLength(AccountingConsts.OtherFieldLength30);
        });
        builder.Entity<DefaultAllotmentForwardCategory>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultAllotmentForwardCategory", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.FProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Type).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.OrdGrp).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.LstCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.GroupCoefficientCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ProductionPeriodCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.DebitSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CreditSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
            b.Property(e => e.RecordBook).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.AttachProduct).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.QuantityType).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.NormType).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new
            {
                e.Code,
            });
        });
        builder.Entity<CategoryDelete>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "CategoryDelete", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.TabName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.FieldCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.RefFieldCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ConditionField).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.ConditionValue).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.TabName,
            });
        });
        builder.Entity<DefaultReason>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultReason", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.ReasonType).HasMaxLength(1);
            b.HasIndex(e => new
            {
                e.Code
            });
        });
        builder.Entity<DefaultCapital>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultCapital", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            });
        });
        builder.Entity<DefaultSalarySheetType>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultSalarySheetType", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            });
        });
        builder.Entity<DefaultSalarySheetTypeDetail>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultSalarySheetTypeDetail", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.SalarySheetTypeId).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Caption).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.DataType).HasMaxLength(AccountingConsts.OtherFieldLength20);
            b.Property(e => e.Format).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.NameFieldLength);
            b.HasIndex(e => new
            {
                e.SalarySheetTypeId
            });
            b.HasOne(e => e.SalarySheetType)
                .WithMany(e => e.SalarySheetTypeDetails)
                .HasForeignKey(e => e.SalarySheetTypeId)
                .HasConstraintName("FK_DefaultSalarySheetTypeDetails_DefaultSalarySheetType")
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
        builder.Entity<DefaultSalaryCategory>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultSalaryCategory", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props                
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
            b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
            b.Property(e => e.Nature).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Status).HasMaxLength(1);
            b.Property(e => e.TaxType).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            });
        });

        builder.Entity<DefaultAccSection>(b =>
        {
            b.HasKey(e => e.Id);
            b.ToTable(AccountingConsts.DbTablePrefix + "DefaultAccSection", AccountingConsts.DbSchema);
            b.ConfigureByConvention(); //auto configure for the base class props
            b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
            b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
            b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
            b.Property(e => e.SectionType).HasMaxLength(AccountingConsts.TypeFieldLength);
            b.HasIndex(e => new
            {
                e.Code
            }).IsUnique().IncludeProperties(e => new
            {
                e.Name,
                e.SectionType
            });
        });
    }

    public DbSet<MenuAccounting> MenuAccountings { get; set; }
    public DbSet<Window> Windows { get; set; }
    public DbSet<Tab> Tabs { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Reference> References { get; set; }
    public DbSet<ReferenceDetail> ReferenceDetails { get; set; }
    public DbSet<ReportTemplate> ReportTemplates { get; set; }
    public DbSet<ReportTemplateColumn> ReportTemplateColumns { get; set; }
    public DbSet<ReportMenuShortcut> ReportMenuShortcuts { get; set; }
    public DbSet<ImportExcelTemplate> ImportExcelTemplates { get; set; }
    public DbSet<ImportExcelTemplateColumn> ImportExcelTemplateColumns { get; set; }
    public DbSet<InfoWindow> InfoWindows { get; set; }
    public DbSet<InfoWindowDetail> InfoWindowDetails { get; set; }
    public DbSet<ExportExcelTemplate> ExportExcelTemplates { get; set; }
    public DbSet<ExportExcelTemplateColumn> ExportExcelTemplateColumns { get; set; }
    public DbSet<VoucherTemplate> VoucherTemplates { get; set; }
    public DbSet<Button> Buttons { get; set; }
    public DbSet<TenantExtendInfo> TenantExtendInfos { get; set; }
    public DbSet<EventSetting> EventSettings { get; set; }
    public DbSet<RegisterEvent> RegisterEvents { get; set; }
    public DbSet<Circulars> Circularses { get; set; }
    public DbSet<DefaultAccBalanceSheet> DefaultAccBalanceSheets { get; set; }
    public DbSet<DefaultCashFollowStatement> DefaultCashFollowStatements { get; set; }
    public DbSet<DefaultBusinessResult> DefaultBusinessResults { get; set; }
    public DbSet<RegLicense> RegLicenses { get; set; }
    public DbSet<DefaultFStatement133L01> DefaultFStatement133L01s { get; set; }
    public DbSet<DefaultFStatement133L02> DefaultFStatement133L02s { get; set; }
    public DbSet<DefaultFStatement133L03> DefaultFStatement133L03s { get; set; }
    public DbSet<DefaultFStatement133L04> DefaultFStatement133L04s { get; set; }
    public DbSet<DefaultFStatement133L05> DefaultFStatement133L05s { get; set; }
    public DbSet<DefaultFStatement133L06> DefaultFStatement133L06s { get; set; }
    public DbSet<DefaultFStatement133L07> DefaultFStatement133L07s { get; set; }
    public DbSet<DefaultStatementTax> DefaultStatementTaxes { get; set; }
    public DbSet<DefaultFStatement200L01> DefaultFStatement200L01s { get; set; }
    public DbSet<DefaultFStatement200L02> DefaultFStatement200L02s { get; set; }
    public DbSet<DefaultFStatement200L03> DefaultFStatement200L03s { get; set; }
    public DbSet<DefaultFStatement200L04> DefaultFStatement200L04s { get; set; }
    public DbSet<DefaultFStatement200L05> DefaultFStatement200L05s { get; set; }
    public DbSet<DefaultFStatement200L06> DefaultFStatement200L06s { get; set; }
    public DbSet<DefaultFStatement200L07> DefaultFStatement200L07s { get; set; }
    public DbSet<DefaultFStatement200L08> DefaultFStatement200L08s { get; set; }
    public DbSet<DefaultFStatement200L09> DefaultFStatement200L09s { get; set; }
    public DbSet<DefaultFStatement200L10> DefaultFStatement200L10s { get; set; }
    public DbSet<DefaultFStatement200L11> DefaultFStatement200L11s { get; set; }
    public DbSet<DefaultFStatement200L12> DefaultFStatement200L12s { get; set; }
    public DbSet<DefaultFStatement200L13> DefaultFStatement200L13s { get; set; }
    public DbSet<DefaultFStatement200L14> DefaultFStatement200L14s { get; set; }
    public DbSet<DefaultFStatement200L15> DefaultFStatement200L15s { get; set; }
    public DbSet<DefaultFStatement200L16> DefaultFStatement200L16s { get; set; }
    public DbSet<DefaultFStatement200L17> DefaultFStatement200L17s { get; set; }
    public DbSet<DefaultFStatement200L18> DefaultFStatement200L18s { get; set; }
    public DbSet<DefaultFStatement200L19> DefaultFStatement200L19s { get; set; }
    public DbSet<ConfigForwardYear> ConfigForwardYears { get; set; }
    public DbSet<ConfigForwardOrg> ConfigForwardOrgs { get; set; }
    public DbSet<DbServer> DataServers { get; set; }
    public DbSet<CustomerRegister> CustomerRegisters { get; set; }
    public DbSet<DefaultAccountSystem> DefaultAccountSystems { get; set; }
    public DbSet<DefaultThnvnn> DefaultThnvnns { get; set; }
    public DbSet<LinkCode> LinkCodes { get; set; }
    public DbSet<DefaultTenantSetting> DefaultTenantSettings { get; set; }
    public DbSet<DefaultVoucherCategory> DefaultVoucherCategories { get; set; }
    public DbSet<DefaultCurrency> DefaultCurrencies { get; set; }
    public DbSet<DefaultExciseTax> DefaultExciseTaxes { get; set; }
    public DbSet<DefaultVoucherType> DefaultVoucherTypes { get; set; }
    public DbSet<DefaultHtkkNumberCode> DefaultHtkkNumberCodes { get; set; }
    public DbSet<DefaultAssetToolGroup> DefaultAssetToolGroups { get; set; }
    public DbSet<DefaultTaxCategory> DefaultTaxCategories { get; set; }
    public DbSet<DefaultBusinessCategory> DefaultBusinessCategories { get; set; }
    public DbSet<DefaultCareer> DefaultCareers { get; set; }
    public DbSet<DefaultFixError> DefaultFixErrors { get; set; }
    public DbSet<DefaultAllotmentForwardCategory> DefaultAllotmentForwardCategories { get; set; }
    public DbSet<CategoryDelete> CategoryDeletes { get; set; }
    public DbSet<DefaultReason> DefaultReasons { get; set; }
    public DbSet<DefaultCapital> DefaultCapitals { get; set; }
    public DbSet<PackageMobi> PackageMobi { get; set; }
    public DbSet<DefaultSalarySheetType> DefaultSalarySheetTypes { get; set; }
    public DbSet<DefaultSalarySheetTypeDetail> DefaultSalarySheetTypeDetails { get; set; }
    public DbSet<DefaultSalaryCategory> DefaultSalaryCategories { get; set; }
    public DbSet<DefaultAccSection> DefaultAccSections { get; set; }

}
