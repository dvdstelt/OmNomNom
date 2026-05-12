using Catalog.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Data;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<InventoryDelta> InventoryDeltas => Set<InventoryDelta>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Convention picks up Product.ProductId and Order.OrderId as the
        // primary keys via the <Type>Id pattern; OrderItem has a
        // composite key so it needs to be wired up explicitly.
        modelBuilder.Entity<OrderItem>().HasKey(oi => new { oi.OrderId, oi.ProductId });

        modelBuilder.Entity<Order>()
            .HasMany(o => o.Products)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
