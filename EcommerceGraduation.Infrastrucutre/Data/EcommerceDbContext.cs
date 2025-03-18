using System;
using EcommerceGraduation.Core.Entities;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EcommerceGraduation.Infrastructure.Data;

public partial class EcommerceDbContext : IdentityDbContext<Customer, IdentityRole<string>, string>
{
    private readonly IConfiguration _configuration;

    public EcommerceDbContext()
    {
    }

    public EcommerceDbContext(DbContextOptions<EcommerceDbContext> options, IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }
    public virtual DbSet<Brand> Brands { get; set; }
    // public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Invoice> Invoices { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<ProductImage> ProductImages { get; set; }
    public virtual DbSet<ProductReview> ProductReviews { get; set; }
    public virtual DbSet<Shipping> Shippings { get; set; }
    public virtual DbSet<SubCategory> SubCategories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(
                _configuration["ConnectionStrings:EcommerceDatabase"],
                options => options.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                ));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder); // Required for Identity configuration

        // Rename Identity tables (optional, but keeps your naming consistent)
        modelBuilder.Entity<Customer>().ToTable("Customer");
        modelBuilder.Entity<IdentityRole<string>>().ToTable("Roles");
        modelBuilder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
        modelBuilder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");
        modelBuilder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("Id").HasMaxLength(20);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.ToTable("Customer", tb => tb.HasTrigger("trg_Delete_Customer"));
        });

        // AuditLog configuration
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__AuditLog__5E5499A88188D4A4");
            entity.Property(e => e.ChangeDate).HasDefaultValueSql("(getdate())");
            entity.HasOne(d => d.ChangedByNavigation)
                  .WithMany(p => p.AuditLogs)
                  .HasForeignKey(e => e.ChangedBy) // Use CustomerId
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("FK__AuditLog__Change__4C6B5938");
        });

        // Category configuration
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryCode).HasName("PK__Category__371BA954FEB97F11");
        });

        // Invoice configuration
        modelBuilder.Entity<Invoice>(entity =>
        {
            entity.HasKey(e => e.InvoiceId).HasName("PK__Invoices__D796AAD549EF6932");
            entity.Property(e => e.InvoiceDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.ShippingValue).HasDefaultValue(50m);
            entity.Property(e => e.Total).HasComputedColumnSql("(([TotalBeforeDiscount]-[TotalBeforeDiscount]*([DiscountPercent]/(100)))+[ShippingValue])", true);
            entity.Property(e => e.TotalAfterDiscount).HasComputedColumnSql("([TotalBeforeDiscount]-[TotalBeforeDiscount]*([DiscountPercent]/(100)))", true);
            entity.Property(e => e.TotalDiscount).HasComputedColumnSql("([TotalBeforeDiscount]*([DiscountPercent]/(100)))", true);
            entity.HasOne(d => d.CustomerCodeNavigation) // Updated navigation property name
                  .WithMany(p => p.Invoices)
                  .HasForeignKey(e => e.CustomerCode) // Use CustomerId
                  .HasConstraintName("FK__Invoices__Custom__671F4F74");
            entity.HasOne(d => d.OrderNumberNavigation)
                  .WithMany(p => p.Invoices)
                  .HasConstraintName("FK__Invoices__OrderN__662B2B3B");
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderNumber).HasName("PK__Orders__CAC5E742C8EA43C2");
            entity.Property(e => e.OrderDate).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.TotalAmount).HasDefaultValue(0m);
            entity.HasOne(d => d.CustomerCodeNavigation) // Updated navigation property name
                  .WithMany(p => p.Orders)
                  .HasForeignKey(e => e.CustomerCode) // Use CustomerId
                  .HasConstraintName("FK_Orders_Customer");
        });

        // OrderDetail configuration
        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK__OrderDet__D3B9D30C8E4F1F3C");
            entity.ToTable("OrderDetail", tb =>
            {
                tb.HasTrigger("UpdateInvoiceTotals");
                tb.HasTrigger("trg_UpdateTotalAmount");
            });
            entity.Property(e => e.DiscountPercent).HasDefaultValue(0m);
            entity.Property(e => e.DiscountValue).HasComputedColumnSql("(([Price]*[DiscountPercent])/(100))", true);
            entity.Property(e => e.FinalPrice).HasComputedColumnSql("([Price]-([Price]*[DiscountPercent])/(100))", true);
            entity.Property(e => e.SubTotal).HasComputedColumnSql("([Quantity]*([Price]-([Price]*[DiscountPercent])/(100)))", true);
            entity.HasOne(d => d.OrderNumberNavigation)
                  .WithMany(p => p.OrderDetails)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_orderdetail_orders");
            entity.HasOne(d => d.Product)
                  .WithMany(p => p.OrderDetails)
                  .OnDelete(DeleteBehavior.SetNull)
                  .HasConstraintName("FK__OrderDeta__Produ__68487DD7");
        });

        // Payment configuration
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payment__9B556A58B5CEF764");
            entity.Property(e => e.PaymentDate).HasDefaultValueSql("(getdate())");
            entity.HasOne(d => d.OrderNumberNavigation)
                  .WithMany(p => p.Payments)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_payment_orders");
        });

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId).HasName("PK__Product__B40CC6EDFA1F3B04");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.HasOne(d => d.BrandCodeNavigation)
                  .WithMany(p => p.Products)
                  .HasConstraintName("FK_Product_Brand");
            entity.HasOne(d => d.CategoryCodeNavigation)
                  .WithMany(p => p.Products)
                  .HasConstraintName("FK_Product_Category");
            entity.HasOne(d => d.SubCategoryCodeNavigation)
                  .WithMany(p => p.Products)
                  .OnDelete(DeleteBehavior.ClientSetNull)
                  .HasConstraintName("FK_Product_SubCategory");
        });

        // ProductImage configuration
        modelBuilder.Entity<ProductImage>(entity =>
        {
            entity.HasKey(e => e.ImageId).HasName("PK__ProductI__7516F4EC3EA58CB2");
            entity.HasOne(d => d.Product)
                  .WithMany(p => p.ProductImages)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__ProductIm__Produ__5EBF139D");
        });

        // ProductReview configuration
        modelBuilder.Entity<ProductReview>(entity =>
        {
            entity.HasKey(e => e.ReviewId).HasName("PK__ProductR__74BC79AEA53AB25F");
            entity.Property(e => e.ReviewDate).HasDefaultValueSql("(getdate())");
            entity.HasOne(d => d.CustomerCodeNavigation) // Updated navigation property name
                  .WithMany(p => p.ProductReviews)
                  .HasForeignKey(e => e.CustomerCode) // Use CustomerId
                  .HasConstraintName("FK_Reviews_Customer");
            entity.HasOne(d => d.Product)
                  .WithMany(p => p.ProductReviews)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("FK__ProductRe__Produ__7E37BEF6");
        });

        // Shipping configuration
        modelBuilder.Entity<Shipping>(entity =>
        {
            entity.HasKey(e => e.ShippingId).HasName("PK__Shipping__5FACD46094216DD1");
            entity.HasOne(d => d.OrderNumberNavigation)
                  .WithMany(p => p.Shippings)
                  .OnDelete(DeleteBehavior.Cascade)
                  .HasConstraintName("fk_shipping_orders");
        });

        // SubCategory configuration
        modelBuilder.Entity<SubCategory>(entity =>
        {
            entity.HasKey(e => e.SubCategoryCode).HasName("PK__SubCateg__E7EB078530608626");
            entity.HasOne(d => d.CategoryCodeNavigation)
                  .WithMany(p => p.SubCategories)
                  .HasConstraintName("FK_SubCategory_Category");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}