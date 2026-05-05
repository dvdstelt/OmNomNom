using Microsoft.EntityFrameworkCore;
using Shipping.Data.Models;

namespace Shipping.Data;

public class ShippingDbContext(DbContextOptions<ShippingDbContext> options) : DbContext(options)
{
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<DeliveryOption> DeliveryOptions => Set<DeliveryOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // OrderId / DeliveryOptionId picked up as PKs by convention.

        // Owned address inlines into the Orders table; nullable so an
        // Order can exist before the customer submits the address.
        modelBuilder.Entity<Order>().OwnsOne(o => o.Address);

        // Optional FK to DeliveryOption; the order can exist before the
        // customer picks shipping. Restrict on delete so a DeliveryOption
        // can't disappear from under existing orders.
        modelBuilder.Entity<Order>()
            .HasOne<DeliveryOption>()
            .WithMany()
            .HasForeignKey(o => o.DeliveryOptionId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
