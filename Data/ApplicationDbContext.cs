using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TestP.Models;

namespace TestP.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<SiteSettings> SiteSettingsTable { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Product>(entity =>
        {
            entity.Property(p => p.Price).HasColumnType("decimal(18,2)");
            entity.Property(p => p.Id).HasDefaultValueSql("NEWID()");
            entity.Property(p => p.StockQuantity).HasDefaultValue(0);
        });
        builder.Entity<CartItem>(entity =>
        {
            entity.HasKey(ci => ci.Id);
            entity.HasOne(ci => ci.User)
                .WithMany()
                .HasForeignKey(ci => ci.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(ci => ci.Product)
                .WithMany()
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Restrict); //Prevents deletion of product if it exists in cart
            entity.Property(ci => ci.UnitPrice)
            .HasColumnType("decimal(18,2)");
        });

        builder.Entity<Order>(entity =>
        {
            entity.OwnsOne(o => o.CustomerAddress);
            entity.OwnsOne(o => o.DeliveryDetails);
            entity.OwnsOne(o => o.PaymentDetails);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(o => o.DeliveryFees).HasColumnType("decimal(18,2)");

        });
        builder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(oi => oi.Id);
            entity.HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(oi => oi.Product)
                .WithMany()
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict); //Prevents deletion of product if it exists in order
            entity.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
            // entity.Property(oi => oi.TotalPrice).HasColumnType("decimal(18,2)");
            // entity.Ignore(oi => oi.TotalPrice);
        });

        builder.Entity<SiteSettings>(entity =>
        {
            entity.HasKey(ss => ss.Id);
            entity.Property(ss => ss.StandardShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(ss => ss.ExpressShippingCost).HasColumnType("decimal(18,2)");
            entity.Property(ss => ss.DefaultTaxRate).HasColumnType("decimal(18,2)");
        });
    }


}
