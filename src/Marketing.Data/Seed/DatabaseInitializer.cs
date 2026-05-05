using Microsoft.EntityFrameworkCore;

namespace Marketing.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        MarketingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        await dbContext.Products.AddRangeAsync(SeedData.Products(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
