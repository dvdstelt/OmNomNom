using Microsoft.EntityFrameworkCore;

namespace Catalog.Data.Seed;

// Apply pending migrations and, on a fresh database, drop in the demo
// seed. The "Products is empty" check keeps the initial seed idempotent
// so subsequent startups don't duplicate rows; deleting the .db file
// (per the demo reset workflow) brings everything back from scratch.
//
// On top of that, BackfillProductMetadataAsync patches new columns
// onto pre-seeded rows after a non-fresh migration. This is what lets
// older demo databases pick up Brewery/Country (added in the
// AddBreweryAndCountry migration) without forcing the user to delete
// the .db file.
public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (!await dbContext.Products.AnyAsync(cancellationToken))
        {
            await dbContext.Products.AddRangeAsync(SeedData.Products(), cancellationToken);
            await dbContext.InventoryDeltas.AddRangeAsync(SeedData.ProductInventory(), cancellationToken);
            await dbContext.InventorySnapshots.AddRangeAsync(SeedData.InventorySnapshots(), cancellationToken);
            await dbContext.Orders.AddAsync(SeedData.Orders(), cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
            return;
        }

        await BackfillProductMetadataAsync(dbContext, cancellationToken);
    }

    static async Task BackfillProductMetadataAsync(
        CatalogDbContext dbContext,
        CancellationToken cancellationToken)
    {
        var seedById = SeedData.Products().ToDictionary(p => p.ProductId);

        var stale = await dbContext.Products
            .Where(p => p.Brewery == "" || p.Country == "")
            .ToListAsync(cancellationToken);

        if (stale.Count == 0)
            return;

        foreach (var product in stale)
        {
            if (!seedById.TryGetValue(product.ProductId, out var seed))
                continue;

            if (string.IsNullOrEmpty(product.Brewery)) product.Brewery = seed.Brewery;
            if (string.IsNullOrEmpty(product.Country)) product.Country = seed.Country;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
