using Accounting.Categories.Accounts;
using Accounting.Categories.AssetTools;
using Accounting.Categories.Contracts;
using Accounting.Categories.CostProductions;
using Accounting.Categories.OrgUnits;
using Accounting.Categories.Others;
using Accounting.Categories.Others.InvoiceBooks;
using Accounting.Categories.Others.PaymentTerms;
using Accounting.Categories.Partners;
using Accounting.Categories.Products;
using Accounting.Categories.Salaries;
using Accounting.Generals;
using Accounting.Invoices;
using Accounting.Licenses;
using Accounting.Permissions;
using Accounting.Reports;
using Accounting.Reports.Statements.T133.Tenants;
using Accounting.Reports.Statements.T200.Tenants;
using Accounting.Vouchers;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.Modeling;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.MultiTenancy;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;

namespace Accounting.EntityFrameworkCore
{
    public class TenancyDbContext : AbpDbContext<TenancyDbContext>
    {
        public TenancyDbContext(DbContextOptions<TenancyDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.SetMultiTenancySide(MultiTenancySides.Tenant);

            base.OnModelCreating(builder);

            builder.ConfigurePermissionManagement();
            builder.ConfigureAuditLogging();
            builder.ConfigureIdentity();

            builder.Entity<AccountSystem>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccountSystem", AccountingConsts.DbSchema);
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
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ParentAccId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Province).HasMaxLength(AccountingConsts.ProvinceFieldLength);
                b.Property(e => e.AccType).HasMaxLength(2);
                b.Property(e => e.ParentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new { e.OrgCode, e.AccCode, e.Year })
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

            builder.Entity<OrgUnit>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "OrgUnit", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.CareerId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ChiefAccountant).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Director).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.District).HasMaxLength(AccountingConsts.DistrictFieldLength);
                b.Property(e => e.NameE).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Producer).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.Province).HasMaxLength(AccountingConsts.ProvinceFieldLength);
                b.Property(e => e.Signee).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.SubmittingAddress).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.SubmittingOrganiztion).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.TaxAuthorityCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxAuthorityName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
                b.Property(e => e.Wards).HasMaxLength(AccountingConsts.WardsFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.Code)
                    .IsUnique()
                    .IncludeProperties(e => new
                    {
                        e.Name,
                        e.NameE,
                        e.TaxCode
                    });
            });

            builder.Entity<Warehouse>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Warehouse", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.NameTemp).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.WarehouseAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WarehouseType).HasMaxLength(2);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name,
                    e.NameTemp,
                    e.ParentId,
                    e.WarehouseAcc,
                    e.WarehouseRank,
                    e.WarehouseType
                });
            });

            builder.Entity<AccPartner>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccPartner", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContactPerson).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.DebtCeiling).HasColumnType("decimal(18,4)");
                b.Property(e => e.DebtCeilingCur).HasColumnType("decimal(18,4)");
                b.Property(e => e.Email).HasMaxLength(AccountingConsts.EmailFieldLength);
                b.Property(e => e.Fax).HasMaxLength(AccountingConsts.FaxFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.InfoFilter).HasColumnType("text");
                b.Property(e => e.InvoiceSearchLink).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.OtherContact).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.PartnerGroupId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.PartnerType).HasMaxLength(AccountingConsts.TypeFieldLength);
                b.Property(e => e.Representative).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
                b.Property(e => e.Tel).HasMaxLength(AccountingConsts.TelFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name,
                    e.TaxCode,
                    e.PartnerGroupId,
                    e.PartnerType
                });
                b.HasOne(e => e.PartnerGroup)
                    .WithMany(p => p.AccPartners)
                    .HasForeignKey(d => d.PartnerGroupId)
                    .HasConstraintName("FK_AccPartner_PartnerGroup")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<BankPartner>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "BankPartner", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.BankAccNumber).HasMaxLength(AccountingConsts.OtherFieldLength30);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Tel).HasMaxLength(AccountingConsts.TelFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasOne(e => e.AccPartner)
                    .WithMany(p => p.BankPartners)
                    .HasForeignKey(d => d.PartnerId)
                    .HasConstraintName("FK_BankPartner_AccPartner")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasIndex(e => e.PartnerId);
            });

            builder.Entity<Product>(b =>
            {
                b.HasKey(b => b.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Product", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AttachProductLot).HasMaxLength(1);
                b.Property(e => e.AttachProductOrigin).HasMaxLength(1);
                b.Property(e => e.AttachWorkPlace).HasMaxLength(1);
                b.Property(e => e.Barcode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CareerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ExciseTaxCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ImportTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.MaxQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.MinQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.PITPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ProductAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductCostAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductGroupCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductionPeriodCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductType).HasMaxLength(AccountingConsts.TypeFieldLength);
                b.Property(e => e.RevenueAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SaleReturnsAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Specification).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ProductGroupId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name,
                    e.ProductGroupCode,
                    e.ProductAcc,
                    e.ProductCostAcc,
                    e.RevenueAcc,
                    e.SaleReturnsAcc
                });
                b.HasOne(e => e.ProductGroup)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ProductGroupId)
                    .HasConstraintName("FK_Product_ProductGroup")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductUnit>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductUnit", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.PurchasePrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PurchasePriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.SalePrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.SalePriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.ProductId);
                b.HasOne(e => e.Product)
                    .WithMany(p => p.ProductUnits)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductUnit_Product")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductPrice>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductPrice", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.PurchasePrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PurchasePriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.SalePrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.SalePriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.ProductId);
                b.HasOne(e => e.Product)
                    .WithMany(p => p.ProductPrices)
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_ProductPrice_Product")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AccSection>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccSection", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.SectionType).HasMaxLength(AccountingConsts.TypeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name,
                    e.SectionType
                });
            });

            builder.Entity<AccCase>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccCase", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CaseType).HasMaxLength(AccountingConsts.TypeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name,
                    e.CaseType
                });
            });

            builder.Entity<AccOpeningBalance>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccOpeningBalance", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AccSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Credit).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CreditCum).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CreditCumCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CreditCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Debit).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DebitCum).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DebitCumCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DebitCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.AccCode,
                    e.AccSectionCode,
                    e.ContractCode,
                    e.CurrencyCode,
                    e.PartnerCode,
                    e.WorkPlaceCode,
                    e.FProductWorkCode,
                    e.Year
                }).IsUnique();
            });

            builder.Entity<ProductOpeningBalance>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductOpeningBalance", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.ProductCode,
                    e.ProductLotCode,
                    e.ProductOriginCode,
                    e.WarehouseCode,
                    e.Year
                }).IsUnique();
            });

            builder.Entity<AccVoucher>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccVoucher", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccType).HasMaxLength(AccountingConsts.TypeFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.BankName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.BankNumber).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.BusinessAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BusinessCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitOrCredit).HasMaxLength(1);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.InvoiceFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.OriginVoucher).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.PartnerCode0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.PaymentTermsCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Representative).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountVatCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountWithoutVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountWithoutVatCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Status).HasMaxLength(2);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year,
                    e.VoucherCode,
                    e.VoucherGroup,
                    e.VoucherDate,
                    e.VoucherNumber
                }).IncludeProperties(e => new
                {
                    e.PartnerCode0,
                    e.PartnerName0,
                    e.DebitOrCredit,
                    e.CurrencyCode,
                    e.ExchangeRate
                });
            });

            builder.Entity<AccVoucherDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccVoucherDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitExchageRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.RecordingVoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.AccVoucherId
                }).IncludeProperties(e => new
                {
                    e.Ord0
                });
                b.HasOne(e => e.AccVoucher)
                    .WithMany(e => e.AccVoucherDetails)
                    .HasForeignKey(e => e.AccVoucherId)
                    .HasConstraintName("FK_AccVoucherDetail_AccVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AccTaxDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AccTaxDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountWithoutVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountWithoutVatCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CheckDuplicate).HasMaxLength(1);
                b.Property(e => e.ClearingContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.InvoiceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.InvoiceGroup).HasMaxLength(2);
                b.Property(e => e.InvoiceLink).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.InvoiceFieldLength);
                b.Property(e => e.InvoiceSymbol).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.NoteE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.RecordingVoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SecurityNo).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.AccVoucherId
                });
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.ProductVoucherId
                });
                b.HasOne(e => e.AccVoucher)
                    .WithMany(p => p.AccTaxDetails)
                    .HasForeignKey(d => d.AccVoucherId)
                    .HasConstraintName("FK_AccTaxDetail_AccVoucher")
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(p => p.AccTaxDetails)
                    .HasForeignKey(d => d.ProductVoucherId)
                    .HasConstraintName("FK_AccTaxDetail_ProductVoucher")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<Ledger>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Ledger", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(p => p.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(p => p.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(p => p.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(p => p.BusinessAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.BusinessCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CheckDuplicate).HasMaxLength(1);
                b.Property(p => p.CheckDuplicate0).HasMaxLength(1);
                b.Property(p => p.ClearingContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.ClearingFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.ClearingPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.ClearingSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(p => p.CreditAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(p => p.CreditContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditCurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(p => p.CreditFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CreditWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.CurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(p => p.DebitContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitCurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(p => p.DebitFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitOrCredit).HasMaxLength(1);
                b.Property(p => p.DebitPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DebitWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.Description).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(p => p.DescriptionE).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(p => p.ExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(p => p.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(p => p.InvoiceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(p => p.InvoiceNbr).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.InvoiceNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.InvoicePartnerAddress).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(p => p.InvoicePartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(p => p.InvoiceSymbol).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(p => p.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(p => p.NoteE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(p => p.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(p => p.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(p => p.OriginVoucher).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(p => p.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.PartnerCode0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.PartnerName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(p => p.PaymentTermsCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(p => p.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(p => p.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.ProductName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(p => p.ProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.PromotionQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(p => p.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(p => p.RecordingVoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.Representative).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(p => p.SalesChannelCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.SecurityNo).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(p => p.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
                b.Property(p => p.TransWarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.TrxPromotionQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(p => p.TrxQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(p => p.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(p => p.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(p => p.VoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(p => p.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(p => p.Ord0Extra).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Status).HasMaxLength(2);
                b.HasIndex(p => p.VoucherId)
                    .IncludeProperties(p => new
                    {
                        p.Ord0,
                        p.VoucherCode,
                        p.VoucherDate,
                        p.VoucherGroup,
                        p.Year,
                        p.OrgCode
                    });
                b.HasIndex(p => new
                {
                    p.OrgCode,
                    p.VoucherId
                });
            });

            builder.Entity<WarehouseBook>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "WarehouseBook", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Amount2).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur2).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.BillNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BusinessAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BusinessCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc2).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc2).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitOrCredit).HasMaxLength(1);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.DiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.ExciseTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExciseTaxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ExpenseAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmount0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmount1).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmountCur0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmountCur1).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExportAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExportAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExportAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExportQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.ExprenseAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ImportAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ImportAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ImportAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ImportQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.ImportTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ImportTaxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ImportTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.InvoiceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.InvoicePartnerAddress).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.InvoicePartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.InvoiceSymbol).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.NoteE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.Ord0Extra).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.OriginVoucher).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PaymentTermsCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Place).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.Price0).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Price2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur0).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.DevaluationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DevaluationAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DevaluationPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.DevaluationPrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.DevaluationPriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.QuantityCur).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.Representative).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.SalesChannelCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
                b.Property(e => e.Tel).HasMaxLength(AccountingConsts.TelFieldLength);
                b.Property(e => e.TotalAmount2).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalDiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TransferingUnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransWarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TrxExportQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.TrxImportQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.TrxPrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.TrxPrice2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.TrxPriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.TrxPriceCur2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.TrxQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VarianceAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.VatPrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.VatPriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Status).HasMaxLength(2);
                b.HasIndex(e => e.ProductVoucherId)
                    .IncludeProperties(e => new
                    {
                        e.OrgCode,
                        e.Ord0,
                        e.VoucherCode,
                        e.VoucherGroup,
                        e.VoucherDate,
                        e.VoucherNumber
                    });
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.WarehouseBooks)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_WarehouseBook_ProductVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductVoucher>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucher", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.BillNumber).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.BusinessAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BusinessCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CommandNumber).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.CurrencyCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitOrCredit).HasMaxLength(1);
                b.Property(e => e.DeliveryDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.DescriptionE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.EmployeeCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.ExportDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ExportNumber).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ImportDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.InfoFilter).HasColumnType(AccountingConsts.TextColumnType);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.OriginVoucher).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.OtherDepartment).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.PartnerCode0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.PaymentTermsCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PaymentTermsId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Place).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.PriceCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PriceDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PriceDecreasingDescription).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.DevaluationPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Representative).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.SalesChannelCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Status).HasMaxLength(2);
                b.Property(e => e.Tel).HasMaxLength(AccountingConsts.TelFieldLength);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountWithoutVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountWithoutVatCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalDiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalDiscountAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExciseTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExciseTaxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExpenseAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExpenseAmount0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExpenseAmount1).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExpenseAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExpenseAmountCur0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalExpenseAmountCur1).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalImportTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalImportTaxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalDevaluationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalDevaluationAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalProductAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalProductAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.TotalVatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalVatAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Vehicle).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.DiscountCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PaymentCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PaymentDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExcutionStatus).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.DevaluationDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DevaluationCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.VoucherCode,
                    e.VoucherGroup,
                    e.VoucherDate,
                    e.Year
                });
            });

            builder.Entity<ProductVoucherDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucherDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Amount2).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountAfterDecrease).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur2).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountWithVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc2).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc2).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DecreaseAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DecreasePercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.DevaluationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DevaluationAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DevaluationPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.DevaluationPrice).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DevaluationPriceCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.FixedPrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.HTPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.InsuranceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.NoteE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Price2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductVoucherDetailId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.RefId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.RevenueAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransUnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TransWarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TrxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TrxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TrxQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VarianceAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatPrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.VatPriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.VatTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SalechannelCode).HasMaxLength(AccountingConsts.CodeFieldLength); // Thêm dòng này.
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.ProductVoucherDetailId
                }).IncludeProperties(e => new
                {
                    e.Ord0
                });
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.ProductVoucherDetails)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_ProductVoucherDetail_ProductVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductVoucherDetailReceipt>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucherDetailReceipt", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.DiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ExciseTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExciseTaxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ExpenseAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmount0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmount1).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmountCur0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExpenseAmountCur1).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ImportTaxAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ImportTaxAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ImportTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ProductVoucherDetailId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatAmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.ProductVoucherDetailId
                });
                b.HasOne(e => e.ProductVoucherDetail)
                    .WithMany(e => e.ProductVoucherDetailReceipts)
                    .HasForeignKey(e => e.ProductVoucherDetailId)
                    .HasConstraintName("FK_ProductVoucherDetailReceipt_ProductVoucherDetail")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductVoucherAssembly>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucherAssembly", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AssemblyProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AssemblyProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AssemblyUnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AssemblyWarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AssemblyWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.TrxQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.ProductVoucherId).IsUnique();
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.ProductVoucherAssemblies)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_ProductVoucherAssembly_ProductVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<ProductVoucherReceipt>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucherReceipt", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.DiscountAmount0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountAmountCur0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountCreditAcc0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountDebitAcc0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountDescription).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.DiscountDescription0).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.ExciseTaxCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExciseTaxDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExciseTaxDescription).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ImportCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ImportDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ImportDescription).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.ImportTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.ProductVoucherId).IsUnique();
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.ProductVoucherReceipts)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_ProductVoucherReceipt_ProductVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<ProductVoucherVat>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucherVat", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.BuyerBankNumber).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.InvoiceFieldLength);
                b.Property(e => e.InvoiceSerial).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.InvoiceSymbol).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.SellerBankNumber).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.TaxCodeFieldLength);
                b.Property(e => e.VatAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VatPartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.VatProductName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.ProductVoucherId).IsUnique();
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.ProductVoucherVats)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_ProductVoucherVat_ProductVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<VoucherCategory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherCategory", AccountingConsts.DbSchema);
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
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Prefix).HasMaxLength(5);
                b.Property(e => e.PriceCalculatingMethod).HasMaxLength(2);
                b.Property(e => e.ProductType).HasMaxLength(2);
                b.Property(e => e.SeparatorCharacter).HasMaxLength(1);
                b.Property(e => e.Suffix).HasMaxLength(5);
                b.Property(e => e.TaxType).HasMaxLength(2);
                b.Property(e => e.VoucherKind).HasMaxLength(2);
                b.Property(e => e.VoucherOrd).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.VoucherType).HasMaxLength(10);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });

            builder.Entity<FProductWork>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "FProductWork", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.BeginningDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.EndingDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.FProductOrWork).HasMaxLength(2);
                b.Property(e => e.FPWType).HasMaxLength(2);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.NameTemp).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.ParentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.WorkOwner).HasMaxLength(AccountingConsts.PersonNameLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });

            builder.Entity<Currency>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Currency", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Default).HasMaxLength(2);
                b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.NameE).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.OddCurrencyEN).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.Property(e => e.OddCurrencyVN).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });

            builder.Entity<WorkPlace>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "WorkPlace", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name
                });
            });
            builder.Entity<YearCategory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "YearCategory", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year
                }).IsUnique().IncludeProperties(e => new
                {
                    e.BeginDate,
                    e.EndDate
                });
            });

            builder.Entity<Career>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Career", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name
                });
            });

            builder.Entity<ProductOrigin>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductOrigin", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Name
                });
            });

            builder.Entity<TenantSetting>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantSetting", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Key).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Type).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Value).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.SettingType).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Key
                }).IsUnique().IncludeProperties(e => new
                {
                    e.Value
                });
            });
            builder.Entity<OrgUnitPermission>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "OrgUnitPermission", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgUnitId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.HasIndex(e => e.UserId).IncludeProperties(e => e.OrgUnitId);
                b.HasOne(e => e.OrgUnit)
                    .WithMany(e => e.OrgUnitPermissions)
                    .HasForeignKey(e => e.OrgUnitId)
                    .HasConstraintName("FK_OrgUnitPermission_OrgUnit")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<PartnerGroup>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "PartnerGroup", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
            });

            builder.Entity<ProductVoucherCost>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductVoucherCost", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ClearingWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CostType).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditExchange).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitExchange).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.NoteE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.ProductVoucherCostDetails)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_ProductVoucherCode_ProductVoucher")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<BusinessCategory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "BusinessCategory", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Prefix).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.Property(e => e.Separator).HasMaxLength(1);
                b.Property(e => e.Suffix).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<TaxCategory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TaxCategory", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.OutOrIn).HasMaxLength(1);
                b.Property(e => e.Percentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Percetage0).HasColumnType(AccountingConsts.PercentageColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<VoucherExciseTax>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherExciseTax", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.AmountWithoutTax).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountWithoutTaxCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CleaningContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CleaningFProducWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CleaningPartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CleaningSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CleaningWorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditExchange).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.DebitExchange).HasColumnType(AccountingConsts.ExchangeRateColumnType);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExciseTaxCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.InvoiceBookCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.InvoiceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.InvoiceGroup).HasMaxLength(2);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.InvoiceFieldLength);
                b.Property(e => e.InvoiceSerial).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.InvoiceSymbol).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.NoteE).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.ProductCode0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductName0).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ProductVoucherId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Type).HasMaxLength(2);
                b.Property(e => e.UnitCode0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => e.AccVoucherId);
                b.HasIndex(e => e.ProductVoucherId);
                b.HasOne(e => e.ProductVoucher)
                    .WithMany(e => e.VoucherExciseTaxes)
                    .HasForeignKey(e => e.ProductVoucherId)
                    .HasConstraintName("FK_VoucherExciseTax_ProductVoucher")
                    .OnDelete(DeleteBehavior.Cascade);
                b.HasOne(e => e.AccVoucher)
                    .WithMany(e => e.VoucherExciseTaxes)
                    .HasForeignKey(e => e.AccVoucherId)
                    .HasConstraintName("FK_VoucherExciseTax_AccVoucher")
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Department>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Department", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.NameTemp).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ParentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<VoucherType>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherType", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ListGroup).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ListVoucher).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => e.Code).IsUnique();
            });
            builder.Entity<Unit>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Unit", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<Contract>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Contract", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ContractType).HasMaxLength(2);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<ContractDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ContractDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.ContractId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => e.ContractId);
                b.HasOne(e => e.Contract)
                    .WithMany(e => e.ContractDetails)
                    .HasForeignKey(e => e.ContractId)
                    .HasConstraintName("FK_ContractDetail_Contract")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<SaleChannel>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SaleChannel", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
            });
            builder.Entity<ProductGroup>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductGroup", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ParentId).HasMaxLength(AccountingConsts.IdFieldLength);
            });
            builder.Entity<ExciseTax>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ExciseTax", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ExciseTaxPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Htkk).HasMaxLength(AccountingConsts.OtherFieldLength30);
                b.Property(e => e.Htkk0).HasMaxLength(AccountingConsts.OtherFieldLength30);
                b.Property(e => e.HtkkName).HasMaxLength(AccountingConsts.OtherFieldLength);
            });
            builder.Entity<ProductLot>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductLot", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.ExpireDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ImportDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ManufacturingDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ManufaturingCountry).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<DiscountPrice>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "DiscountPrice", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<DiscountPriceDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "DiscountPriceDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.DiscountPriceId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.DiscountAmountPrice).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.DiscountAmountPriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Price2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.PriceCur2).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => e.DiscountPriceId);
                b.HasOne(e => e.DiscountPrice)
                    .WithMany(e => e.DiscountPriceDetails)
                    .HasForeignKey(e => e.DiscountPriceId)
                    .HasConstraintName("FK_DiscountPriceDetail_DiscountPrice")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<DiscountPricePartner>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "DiscountPricePartner", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.DiscountPriceId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => e.DiscountPriceId);
                b.HasOne(e => e.DiscountPrice)
                    .WithMany(e => e.DiscountPricePartners)
                    .HasForeignKey(e => e.DiscountPriceId)
                    .HasConstraintName("FK_DiscountPricePartner_DiscountPrice")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<ProductionPeriod>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ProductionPeriod", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<PaymentTerm>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "PaymentTerm", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<PaymentTermDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "PaymentTermDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Percentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.HasOne(e => e.PaymentTerm)
                    .WithMany(e => e.PaymentTermDetails)
                    .HasForeignKey(e => e.PaymentTermId)
                    .HasConstraintName("FK_PaymentTermDetail_PaymentTerm")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<InvoiceBook>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InvoiceBook", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.InvoiceSerial).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.InvoiceTemplate).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.PurchaseDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<FeeType>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "FeeType", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Type).HasMaxLength(2);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                }).IsUnique();
            });
            builder.Entity<VoucherNumber>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherNumber", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BusinessCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.TotalNumberRecord).HasColumnType(AccountingConsts.AmountColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.VoucherCode
                });
            });
            builder.Entity<VoucherPaymentBook>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherPaymentBook", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DeadlinePayment).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.AmountReceivable).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AmountReceived).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AccType).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year
                });
            });
            builder.Entity<VoucherPaymentBeginning>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherPaymentBeginning", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AccCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.TotalAmountWithoutVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountDiscount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.PaymentType).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year
                });
            });
            builder.Entity<VoucherPaymentBeginningDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "VoucherPaymentBeginningDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.VoucherPaymentBeginningId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.DeadlinePayment).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode
                });
                b.HasOne(e => e.VoucherPaymentBeginning)
                    .WithMany(e => e.VoucherPaymentBeginningDetails)
                    .HasForeignKey(e => e.VoucherPaymentBeginningId)
                    .HasConstraintName("FK_VoucherPaymentBeginningDetail_VoucherPaymentBeginning")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<AssetToolGroup>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolGroup", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<AssetTool>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetTool", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.AssetToolGroupId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.AssetToolAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AssetToolCard).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CalculatingMethod).HasMaxLength(2);
                b.Property(e => e.Content).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Country).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.DepreciationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DepreciationAmount0).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DepreciationType).HasMaxLength(2);
                b.Property(e => e.FollowDepreciation).HasMaxLength(2);
                b.Property(e => e.Impoverishment).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.OriginalPrice).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.PurposeCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.UpDownCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ReduceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ReduceDetail).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Remaining).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Wattage).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<AssetToolDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.CapitalCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.AssetToolId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.CalculatingAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepreciationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DepreciationCreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepreciationDebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Impoverishment).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.IsCalculating).HasMaxLength(2);
                b.Property(e => e.Number).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.OriginalPrice).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Remaining).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.UpDownCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.UpDownDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => e.AssetToolId);
                b.HasOne(e => e.AssetTool)
                    .WithMany(e => e.AssetToolDetails)
                    .HasForeignKey(e => e.AssetToolId)
                    .HasConstraintName("FK_AssetToolDetail_AssetTool")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<AssetToolStoppingDepreciation>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolStoppingDepreciation", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.AssetToolId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.HasIndex(e => e.AssetToolId);
                b.HasOne(e => e.AssetTool)
                    .WithMany(e => e.AssetToolStoppingDepreciations)
                    .HasForeignKey(e => e.AssetToolId)
                    .HasConstraintName("FK_AssetToolStoppingDepreciation_AssetTool")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<AssetToolAccessory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolAccessory", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.AssetToolId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => e.AssetToolId);
                b.HasOne(e => e.AssetTool)
                    .WithMany(e => e.AssetToolAccessories)
                    .HasForeignKey(e => e.AssetToolId)
                    .HasConstraintName("FK_AssetToolAccessory_AssetTool")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<AssetToolAccount>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolAccount", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.AssetToolId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepreciationDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => e.AssetToolId);
                b.HasOne(e => e.AssetTool)
                    .WithMany(e => e.AssetToolAccounts)
                    .HasForeignKey(e => e.AssetToolId)
                    .HasConstraintName("FK_AssetToolAccount_AssetTool")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AssetToolDepreciation>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolDepreciation", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.AssetToolCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.AssetToolId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.CaseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepreciationDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CapitalCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepreciationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DepreciationBeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.DepreciationDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.DepreciationDownAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DepreciationEdit).HasMaxLength(2);
                b.Property(e => e.DepreciationUpAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Edit).HasMaxLength(2);
                b.Property(e => e.UpDownDate).HasColumnType(AccountingConsts.DateColumnType);
                b.HasIndex(e => e.AssetToolId);
                b.HasOne(e => e.AssetTool)
                    .WithMany(e => e.AssetToolDepreciations)
                    .HasForeignKey(e => e.AssetToolId)
                    .HasConstraintName("FK_AssetToolDepreciation_AssetTool")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<InfoCalcPriceStockOut>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InfoCalcPriceStockOut", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CalculatingMethod).HasMaxLength(2);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ExcutionUser).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.FromDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductGroupCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductionPeriodCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Status).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ToDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.WarehouseCose).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Status,
                    e.BeginDate,
                    e.EndDate
                });
            });
            builder.Entity<InfoCalcPriceStockOutDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InfoCalcPriceStockOutDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Status).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ErrorMsg).HasColumnType(AccountingConsts.TextColumnType);
                b.HasOne(e => e.InfoCalcPriceStockOut)
                    .WithMany(e => e.InfoCalcPriceStockOutDetails)
                    .HasForeignKey(e => e.InfoCalcPriceStockOutId)
                    .HasConstraintName("FK_InfoCalcPriceStockOutDetail_InfoCalcPriceStockOut")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<AdjustDepreciation>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AdjustDepreciation", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.AssetOrTool).HasMaxLength(2);
                b.Property(e => e.AssetToolCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.AssetToolCode
                });
            });
            builder.Entity<AdjustDepreciationDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AdjustDepreciationDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.AdjustDepreciationId).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AssetToolDetailId).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => e.AdjustDepreciationId);
                b.HasOne(e => e.AdjustDepreciation)
                    .WithMany(e => e.AdjustDepreciationDetails)
                    .HasForeignKey(e => e.AdjustDepreciationId)
                    .HasConstraintName("FK_AdjustDepreciationDetail_AdjustDepreciation")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Reason>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Reason", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ReasonType).HasMaxLength(1);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<Capital>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Capital", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<Purpose>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Purpose", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<FProductWorkNorm>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "FProductWorkNorm", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year,
                    e.FProductWorkCode,
                });
            });

            builder.Entity<FProductWorkNormDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "FProductWorkNormDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.AccCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductOrigin).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.QuantityLoss).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.PercentLoss).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.PriceCur).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.AmountCur).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.HasIndex(e => e.FProductWorkNormId);
                b.HasOne(e => e.FProductWorkNorm)
                    .WithMany(e => e.FProductWorkNormDetails)
                    .HasForeignKey(e => e.FProductWorkNormId)
                    .HasConstraintName("FK_FProductWorkNormDetail_FProductWorkNorm")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<GroupCoefficient>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "GroupCoefficient", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.FProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.ApplicableDate1).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ApplicableDate2).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code,
                });
            });

            builder.Entity<GroupCoefficientDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "GroupCoefficientDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.FProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.GroupCoefficientCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => e.GroupCoefficientId);
                b.HasOne(e => e.GroupCoefficient)
                    .WithMany(e => e.GroupCoefficientDetails)
                    .HasForeignKey(e => e.GroupCoefficientId)
                    .HasConstraintName("FK_GroupCoefficientDetail_GroupCoefficient")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });

            builder.Entity<AllotmentForwardCategory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AllotmentForwardCategory", AccountingConsts.DbSchema);
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
                    e.OrgCode,
                    e.Code,
                });
            });

            builder.Entity<ConfigCostPrice>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "ConfigCostPrice", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasIndex(e => new
                {
                    e.OrgCode
                });
            });

            builder.Entity<InfoZ>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InfoZ", AccountingConsts.DbSchema);
                b.Property(e => e.FProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BeginM).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndM).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.OrdGrp).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Type).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AllotmentForwardCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductionPeriodCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditFProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Quantity).HasColumnType("decimal(18,4)");
                b.Property(e => e.AmountCur).HasColumnType("decimal(18,4)");
                b.Property(e => e.Amount).HasColumnType("decimal(18,4)");
                b.Property(e => e.RecordBook).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Ratio).HasColumnType("decimal(18,4)");
                b.Property(e => e.BeginQuantity).HasColumnType("decimal(22,4)");
                b.Property(e => e.BeginAmount).HasColumnType("decimal(22,4)");
                b.Property(e => e.EndQuantity).HasColumnType("decimal(22,4)");
                b.Property(e => e.EndAmount).HasColumnType("decimal(22,4)");
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasIndex(e => new
                {
                    e.OrgCode
                });
            });

            builder.Entity<InfoExportAuto>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InfoExportAuto", AccountingConsts.DbSchema);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BeginDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.EndDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.OrdGrp).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Type).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductionPeriodCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.OrdRec).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.HasIndex(e => new
                {
                    e.OrgCode
                });
            });

            builder.Entity<BookThz>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "BookThz", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AttachCreditFProductWork).HasMaxLength(1);
                b.Property(e => e.CreditSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AttachDebitFProducWork).HasMaxLength(1);
                b.Property(e => e.DebitSectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FieldType).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductOrWork).HasMaxLength(1);
                b.Property(e => e.TGet).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TSum).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year,
                    e.DecideApply
                });
            });

            builder.Entity<RecordingVoucherBook>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "RecordingVoucherBook", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.LstVoucherCode).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year,
                    e.VoucherDate
                });
            });

            builder.Entity<TenantAccBalanceSheet>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantAccBalanceSheet", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
                    e.OrgCode,
                    e.Year,
                    e.UsingDecision
                });
            });

            builder.Entity<Person>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Person", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Name
                });
            });

            builder.Entity<TenantCashFollowStatement>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantCashFollowStatement", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
                    e.OrgCode,
                    e.Year,
                    e.UsingDecision
                });
            });

            builder.Entity<TenantBusinessResult>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantBusinessResult", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
                    e.OrgCode,
                    e.Year,
                    e.UsingDecision
                });
            });
            builder.Entity<TenantLicense>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantLicense", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LicXml).HasColumnType(AccountingConsts.TextColumnType);
            });
            builder.Entity<TenantFStatement133L01>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L01", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement133L02>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L02", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement133L03>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L03", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement133L04>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L04", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement133L05>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L05", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement133L06>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L06", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement133L07>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement133L07", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantStatementTax>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantStatementTax", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Bold).HasMaxLength(1);
                b.Property(e => e.Ord0).HasMaxLength(10);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Printable).HasMaxLength(1);
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
            builder.Entity<TenantFStatement200L01>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L01", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L02>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L02", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L03>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L03", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L04>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L04", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L05>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L05", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L06>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L06", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L07>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L07", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L08>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L08", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L09>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L09", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L10>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L10", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L11>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L11", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L12>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L12", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L13>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L13", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L14>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L14", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L15>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L15", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L16>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L16", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L17>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L17", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Bold).HasMaxLength(1);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.DebitOrCredit).HasMaxLength(2);
                b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.NumberCode).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.Type).HasMaxLength(2);
                b.Property(e => e.Printable).HasMaxLength(1);
                b.Property(e => e.Title).HasMaxLength(2);
                b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
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
            builder.Entity<TenantFStatement200L18>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L18", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<TenantFStatement200L19>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantFStatement200L19", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
            builder.Entity<InvoiceAuth>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InvoiceAuth", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.AdditionalReferenceDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.AdditionalReferenceDes).HasMaxLength(500);
                b.Property(e => e.AdjustmentInvoiceNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.AmountByWord).HasMaxLength(500);
                b.Property(e => e.BuyerAddressLine).HasMaxLength(500);
                b.Property(e => e.BuyerBankAccount).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BuyerBankName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.BuyerDisplayName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.BuyerEmail).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.BuyerLegalName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.BuyerTaxCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ContractDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ContractNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CurrencyCode).HasMaxLength(10);
                b.Property(e => e.DeliveryBy).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.DeliveryOrderDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.DeliveryOrderNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DocumentDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.DocumentNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ExchangeRate).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.FromWarehouseName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.InvoiceName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.InvoiceNote).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.InvoiceNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.InvoiceSeries).HasMaxLength(15);
                b.Property(e => e.InvoiceTemplate).HasMaxLength(10);
                b.Property(e => e.InvoiceType).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.IssuedDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.OrderNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PaymentMethodName).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.SellerAddressLine).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.SellerBankAccount).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SellerLegalName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.SellerTaxCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Signature).HasMaxLength(500);
                b.Property(e => e.SignedDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Status).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SubmmittedDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountWithoutVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ToWarehouseName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.TransportationMethod).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SupplierCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.OtherDepartment).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.BusinessCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CommandNumber).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Representative).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => e.InvoiceId);
            });
            builder.Entity<InvoiceAuthDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InvoiceAuthDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.DiscountAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.DiscountPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.Property(e => e.InvoiceAuthId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ItemCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ItemName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.Quantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.TaxCategoryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TotalAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalAmountWithoutVat).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.UnitCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.UnitName).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VatAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.VatPercentage).HasColumnType(AccountingConsts.PercentageColumnType);
                b.HasIndex(e => new
                {
                    e.InvoiceAuthId
                });
                b.HasOne(e => e.InvoiceAuth)
                    .WithMany(e => e.InvoiceAuthDetails)
                    .HasForeignKey(e => e.InvoiceAuthId)
                    .HasConstraintName("FK_InvoiceAuthDetail_InvoiceAuth")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<InvoiceSupplier>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InvoiceSupplier", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TaxCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Active).HasMaxLength(2);
                b.Property(e => e.CheckCircular).HasMaxLength(2);
                b.Property(e => e.Link).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Url).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.UserName).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Password).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.UserSevice).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.PassSevice).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.CertificateSerial).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<SoTHZ>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SoTHZ", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.FinanceDecision).HasMaxLength(2);
                b.Property(e => e.FProductOrWork).HasMaxLength(2);
                b.Property(e => e.FieldName).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FieldType).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitFProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DebitSection).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditFProductWork).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditSection).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.TSum).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.TGet).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode
                });
            });
            builder.Entity<RefVoucher>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "RefVoucher", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.SrcId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.DestId).HasMaxLength(AccountingConsts.IdFieldLength);
            });
            builder.Entity<TenantStatementTaxData>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantStatementTaxData", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Extend).HasMaxLength(AccountingConsts.InvoiceFieldLength);
            });
            builder.Entity<InvoiceStatus>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InvoiceStatus", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Color).HasMaxLength(AccountingConsts.OtherFieldLength10);
            });
            builder.Entity<TenantThnvnn>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "TenantThnvnn", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
                    e.OrgCode,
                    e.Year,
                    e.Ord
                });
            });
            builder.Entity<InventoryRecord>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InventoryRecord", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Description).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.OtherContact1).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.OtherContact2).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.OtherContact3).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Position1).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Position2).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Position3).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Representative1).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Representative2).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.Representative3).HasMaxLength(AccountingConsts.OtherFieldLength50);
                b.Property(e => e.TotalAuditAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalInventoryAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalOverAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TotalShortAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.TransDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherNumber).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Year
                });
            });
            builder.Entity<InventoryRecordDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "InventoryRecordDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.InventoryRecordId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Acc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.AuditAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.AuditQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.InventoryAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.InventoryQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.OverAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.OverQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.Property(e => e.Price).HasColumnType(AccountingConsts.PriceColumnType);
                b.Property(e => e.ProductCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductLotCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ProductOriginCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.FProductWorkCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Quality1).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.Ord0FieldLength);
                b.Property(e => e.SectionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WarehouseCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.WorkPlaceCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.ShortAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.ShortQuantity).HasColumnType(AccountingConsts.QuantityColumnType);
                b.HasIndex(e => new
                {
                    e.InventoryRecordId
                });
                b.HasOne(e => e.InventoryRecord)
                    .WithMany(e => e.InventoryRecordDetails)
                    .HasForeignKey(e => e.InventoryRecordId)
                    .HasConstraintName("FK_InventoryRecordDetail_InventoryRecord")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<Position>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Position", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<Employee>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "Employee", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.PartnerCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Address).HasMaxLength(AccountingConsts.AddressFieldLength);
                b.Property(e => e.BasicSalary).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.BirthDay).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Gender).HasMaxLength(AccountingConsts.OtherFieldLength20);
                b.Property(e => e.InsuranceSalary).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.PositionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Tel).HasMaxLength(AccountingConsts.TelFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<SalaryPeriod>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SalaryPeriod", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.FromDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.ToDate).HasColumnType(AccountingConsts.DateColumnType);
                b.Property(e => e.Note).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<SalaryCategory>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SalaryCategory", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.Nature).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Status).HasMaxLength(1);
                b.Property(e => e.TaxType).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<SalaryEmployee>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SalaryEmployee", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.EmployeeCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SalaryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Amount).HasColumnType(AccountingConsts.AmountColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.EmployeeCode
                });
            });
            builder.Entity<FixError>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "FixError", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.ErrorId).HasMaxLength(AccountingConsts.OtherFieldLength10);
                b.Property(e => e.ErrorName).HasMaxLength(AccountingConsts.NoteFieldLength);
                b.Property(e => e.KeyError).HasMaxLength(AccountingConsts.OtherFieldLength30);
            });
            builder.Entity<SalarySheetType>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SalarySheetType", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.DebitAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.CreditAcc).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.VoucherCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.Name).HasMaxLength(AccountingConsts.NameFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.Code
                });
            });
            builder.Entity<SalarySheetTypeDetail>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SalarySheetTypeDetail", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.SalarySheetTypeId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
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
                    .HasConstraintName("FK_SalarySheetTypeDetails_SalarySheetType")
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade);
            });
            builder.Entity<SalaryBook>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "SalaryBook", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.DepartmentCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.EmployeeCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SalaryCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.SalaryPeriodId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.SalarySheetTypeId).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.StringValue).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.Formular).HasMaxLength(AccountingConsts.OtherFieldLength);
                b.Property(e => e.PositionCode).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.SalarySheetTypeId,
                    e.SalaryPeriodId,
                    e.DepartmentCode,
                    e.EmployeeCode,
                    e.SalaryCode
                });
            });
            builder.Entity<AssetToolDetailDepreciation>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "AssetToolDetailDepreciation", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.OrgCode).HasMaxLength(AccountingConsts.OrgCodeFieldLength);
                b.Property(e => e.CreatorName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.LastModifierName).HasMaxLength(AccountingConsts.UserNameLength);
                b.Property(e => e.Ord0).HasMaxLength(AccountingConsts.CodeFieldLength);
                b.Property(e => e.DepreciationAmount).HasColumnType(AccountingConsts.AmountColumnType);
                b.HasIndex(e => new
                {
                    e.OrgCode,
                    e.AssetToolId,
                    e.Ord0,
                    e.DepreciationBeginDate,
                    e.DepreciationAmount
                });
            });
            builder.Entity<RegLicenseInfo>(b =>
            {
                b.HasKey(e => e.Id);
                b.ToTable(AccountingConsts.DbTablePrefix + "RegLicenseInfo", AccountingConsts.DbSchema);
                b.ConfigureByConvention(); //auto configure for the base class props                
                b.Property(e => e.Id).HasMaxLength(AccountingConsts.IdFieldLength);
                b.Property(e => e.Code).HasMaxLength(AccountingConsts.NameFieldLength);
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
        }
        //Identity
        public DbSet<IdentityUser> Users { get; set; }
        public DbSet<IdentityRole> Roles { get; set; }
        public DbSet<IdentityClaimType> ClaimTypes { get; set; }
        public DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        public DbSet<IdentitySecurityLog> SecurityLogs { get; set; }
        public DbSet<IdentityLinkUser> LinkUsers { get; set; }
        #region DbSet
        public DbSet<AccountSystem> AccountSystems { get; set; }
        public DbSet<OrgUnit> OrgUnits { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<AccPartner> AccPartners { get; set; }
        public DbSet<BankPartner> BankPartners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductUnit> ProductUnits { get; set; }
        public DbSet<ProductPrice> ProductPrices { get; set; }
        public DbSet<AccSection> AccSections { get; set; }
        public DbSet<AccCase> AccCases { get; set; }
        public DbSet<AccOpeningBalance> AccOpeningBalances { get; set; }
        public DbSet<ProductOpeningBalance> ProductOpeningBalances { get; set; }
        public DbSet<AccVoucher> AccVouchers { get; set; }
        public DbSet<AccVoucherDetail> AccVoucherDetails { get; set; }
        public DbSet<AccTaxDetail> AccTaxDetails { get; set; }
        public DbSet<Ledger> Ledgers { get; set; }
        public DbSet<WarehouseBook> WarehouseBooks { get; set; }
        public DbSet<ProductVoucher> ProductVouchers { get; set; }
        public DbSet<ProductVoucherDetail> ProductVoucherDetails { get; set; }
        public DbSet<ProductVoucherDetailReceipt> ProductVoucherDetailReceipts { get; set; }
        public DbSet<ProductVoucherAssembly> ProductVoucherAssemblies { get; set; }
        public DbSet<ProductVoucherReceipt> ProductVoucherReceipts { get; set; }
        public DbSet<ProductVoucherVat> ProductVoucherVats { get; set; }
        public DbSet<VoucherCategory> VoucherCategories { get; set; }
        public DbSet<FProductWork> FProductWorks { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<WorkPlace> WorkPlaces { get; set; }
        public DbSet<YearCategory> YearCategories { get; set; }
        public DbSet<Career> Careers { get; set; }
        public DbSet<ProductOrigin> ProductOrigins { get; set; }
        public DbSet<TenantSetting> TenantSettings { get; set; }
        public DbSet<OrgUnitPermission> OrgUnitPermissions { get; set; }
        public DbSet<PartnerGroup> PartnerGroups { get; set; }
        public DbSet<ProductVoucherCost> ProductVoucherCosts { get; set; }
        public DbSet<BusinessCategory> BusinessCategories { get; set; }
        public DbSet<TaxCategory> TaxCategories { get; set; }
        public DbSet<VoucherExciseTax> VoucherExciseTaxes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<VoucherType> VoucherTypes { get; set; }
        public DbSet<Unit> Units { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<ContractDetail> ContractDetails { get; set; }
        public DbSet<SaleChannel> SaleChannels { get; set; }
        public DbSet<ProductGroup> ProductGroups { get; set; }
        public DbSet<ExciseTax> ExciseTaxes { get; set; }
        public DbSet<ProductLot> ProductLots { get; set; }
        public DbSet<DiscountPrice> DiscountPrices { get; set; }
        public DbSet<DiscountPriceDetail> DiscountPriceDetails { get; set; }
        public DbSet<DiscountPricePartner> DiscountPricePartners { get; set; }
        public DbSet<ProductionPeriod> ProductionPeriods { get; set; }
        public DbSet<PaymentTerm> PaymentTerms { get; set; }
        public DbSet<PaymentTermDetail> PaymentTermDetails { get; set; }
        public DbSet<InvoiceBook> InvoiceBooks { get; set; }
        public DbSet<FeeType> FeeTypes { get; set; }
        public DbSet<VoucherNumber> VoucherNumbers { get; set; }
        public DbSet<VoucherPaymentBook> VoucherPaymentBooks { get; set; }
        public DbSet<VoucherPaymentBeginning> VoucherPaymentBeginnings { get; set; }
        public DbSet<VoucherPaymentBeginningDetail> VoucherPaymentBeginningDetails { get; set; }
        public DbSet<AssetToolGroup> AssetToolGroups { get; set; }
        public DbSet<AssetTool> AssetTools { get; set; }
        public DbSet<AssetToolDetail> AssetToolDetails { get; set; }
        public DbSet<AssetToolAccessory> AssetToolAccessories { get; set; }
        public DbSet<AssetToolStoppingDepreciation> AssetToolStoppingDepreciations { get; set; }
        public DbSet<AssetToolAccount> AssetToolAccounts { get; set; }
        public DbSet<AssetToolDepreciation> AssetToolDepreciations { get; set; }
        public DbSet<InfoCalcPriceStockOut> InfoCalcPriceStockOuts { get; set; }
        public DbSet<InfoCalcPriceStockOutDetail> InfoCalcPriceStockOutDetails { get; set; }
        public DbSet<AdjustDepreciation> AdjustDepreciations { get; set; }
        public DbSet<AdjustDepreciationDetail> AdjustDepreciationDetails { get; set; }
        public DbSet<Reason> Reasons { get; set; }
        public DbSet<Capital> Capitals { get; set; }
        public DbSet<Purpose> Purposes { get; set; }
        public DbSet<FProductWorkNorm> FProductWorkNorms { get; set; }
        public DbSet<FProductWorkNormDetail> FProductWorkNormDetails { get; set; }
        public DbSet<GroupCoefficient> GroupCoefficients { get; set; }
        public DbSet<GroupCoefficientDetail> GroupCoefficientDetails { get; set; }
        public DbSet<AllotmentForwardCategory> AllotmentForwardCategories { get; set; }
        public DbSet<ConfigCostPrice> ConfigCostPrices { get; set; }
        public DbSet<InfoZ> InfoZs { get; set; }
        public DbSet<InfoExportAuto> InfoExportAutos { get; set; }
        public DbSet<BookThz> BookThzs { get; set; }
        public DbSet<RecordingVoucherBook> RecordingVoucherBooks { get; set; }
        public DbSet<TenantAccBalanceSheet> TenantAccBalanceSheets { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<TenantCashFollowStatement> TenantCashFollowStatements { get; set; }
        public DbSet<TenantBusinessResult> TenantBusinessResults { get; set; }
        public DbSet<TenantLicense> TenantLicenses { get; set; }
        public DbSet<TenantFStatement133L01> TenantFStatement133L01s { get; set; }
        public DbSet<TenantFStatement133L02> TenantFStatement133L02s { get; set; }
        public DbSet<TenantFStatement133L03> TenantFStatement133L03s { get; set; }
        public DbSet<TenantFStatement133L04> TenantFStatement133L04s { get; set; }
        public DbSet<TenantFStatement133L05> TenantFStatement133L05s { get; set; }
        public DbSet<TenantFStatement133L06> TenantFStatement133L06s { get; set; }
        public DbSet<TenantFStatement133L07> TenantFStatement133L07s { get; set; }
        public DbSet<TenantStatementTax> TenantStatementTaxes { get; set; }
        public DbSet<TenantFStatement200L01> TenantFStatement200L01s { get; set; }
        public DbSet<TenantFStatement200L02> TenantFStatement200L02s { get; set; }
        public DbSet<TenantFStatement200L03> TenantFStatement200L03s { get; set; }
        public DbSet<TenantFStatement200L04> TenantFStatement200L04s { get; set; }
        public DbSet<TenantFStatement200L05> TenantFStatement200L05s { get; set; }
        public DbSet<TenantFStatement200L06> TenantFStatement200L06s { get; set; }
        public DbSet<TenantFStatement200L07> TenantFStatement200L07s { get; set; }
        public DbSet<TenantFStatement200L08> TenantFStatement200L08s { get; set; }
        public DbSet<TenantFStatement200L09> TenantFStatement200L09s { get; set; }
        public DbSet<TenantFStatement200L10> TenantFStatement200L10s { get; set; }
        public DbSet<TenantFStatement200L11> TenantFStatement200L11s { get; set; }
        public DbSet<TenantFStatement200L12> TenantFStatement200L12s { get; set; }
        public DbSet<TenantFStatement200L13> TenantFStatement200L13s { get; set; }
        public DbSet<TenantFStatement200L14> TenantFStatement200L14s { get; set; }
        public DbSet<TenantFStatement200L15> TenantFStatement200L15s { get; set; }
        public DbSet<TenantFStatement200L16> TenantFStatement200L16s { get; set; }
        public DbSet<TenantFStatement200L17> TenantFStatement200L17s { get; set; }
        public DbSet<TenantFStatement200L18> TenantFStatement200L18s { get; set; }
        public DbSet<TenantFStatement200L19> TenantFStatement200L19s { get; set; }
        public DbSet<InvoiceAuth> InvoiceAuths { get; set; }
        public DbSet<InvoiceAuthDetail> InvoiceAuthDetails { get; set; }
        public DbSet<InvoiceSupplier> InvoiceSuppliers { get; set; }
        public DbSet<SoTHZ> SoTHZs { get; set; }
        public DbSet<RefVoucher> RefVouchers { get; set; }
        public DbSet<TenantStatementTaxData> TenantStatementTaxDatas { get; set; }
        public DbSet<InvoiceStatus> InvoiceStatuses { get; set; }
        public DbSet<TenantThnvnn> TenantThnvnns { get; set; }
        public DbSet<InventoryRecord> InventoryRecords { get; set; }
        public DbSet<InventoryRecordDetail> InventoryRecordDetails { get; set; }
        public DbSet<Position> Positions { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<SalaryPeriod> SalaryPeriods { get; set; }
        public DbSet<SalaryCategory> SalaryCategories { get; set; }
        public DbSet<SalaryEmployee> SalaryEmployees { get; set; }
        public DbSet<FixError> FixErrors { get; set; }
        public DbSet<SalarySheetType> SalarySheetTypes { get; set; }
        public DbSet<SalarySheetTypeDetail> SalarySheetTypeDetails { get; set; }
        public DbSet<SalaryBook> SalaryBooks { get; set; }
        public DbSet<AssetToolDetailDepreciation> AssetToolDetailDepreciations { get; set; }
        public DbSet<RegLicenseInfo> RegLicenseInfo { get; set; }
        #endregion
    }
}