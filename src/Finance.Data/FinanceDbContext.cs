using Finance.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Data;

public class FinanceDbContext(DbContextOptions<FinanceDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<DeliveryOption> DeliveryOptions => Set<DeliveryOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ProductId / OrderId / DeliveryOptionId are picked up as PKs by
        // the <Type>Id convention.

        modelBuilder.Entity<OrderItem>().HasKey(oi => new { oi.OrderId, oi.ProductId });

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Items)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // BillingAddress is an owned value object: its columns are
        // inlined into the Orders table. Optional so an Order can exist
        // before the billing address has been submitted.
        modelBuilder.Entity<Order>()
            .OwnsOne(o => o.BillingAddress);

        // Optional FK to DeliveryOption; the order can exist before a
        // delivery option is chosen. Restrict on delete: don't let a
        // DeliveryOption disappear out from under existing orders.
        modelBuilder.Entity<Order>()
            .HasOne<DeliveryOption>()
            .WithMany()
            .HasForeignKey(o => o.DeliveryOptionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
