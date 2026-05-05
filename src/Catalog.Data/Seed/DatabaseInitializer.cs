using Microsoft.EntityFrameworkCore;

namespace Catalog.Data.Seed;

// Apply pending migrations and, on a fresh database, drop in the demo
// seed. The "Products is empty" check keeps it idempotent so subsequent
// startups don't duplicate rows; deleting the .db file (per the demo
// reset workflow) brings everything back from scratch.
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        await dbContext.Products.AddRangeAsync(SeedData.Products(), cancellationToken);
        await dbContext.InventoryDeltas.AddRangeAsync(SeedData.ProductInventory(), cancellationToken);
        await dbContext.InventorySnapshots.AddRangeAsync(SeedData.InventorySnapshots(), cancellationToken);
        await dbContext.Orders.AddAsync(SeedData.Orders(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
