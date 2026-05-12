using Microsoft.EntityFrameworkCore;

namespace Catalog.Data.Seed;

// Create the schema directly from the model and, on a fresh database,
// drop in the demo seed. We deliberately don't ship EF migrations:
// the demo's contract is "delete the .db file when the schema
// changes", so EnsureCreatedAsync is the right tool.
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        await dbContext.Products.AddRangeAsync(SeedData.Products(), cancellationToken);
        await dbContext.InventoryDeltas.AddRangeAsync(SeedData.ProductInventory(), cancellationToken);
        await dbContext.Orders.AddAsync(SeedData.Orders(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
