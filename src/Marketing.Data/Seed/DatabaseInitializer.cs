using Microsoft.EntityFrameworkCore;

namespace Marketing.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        MarketingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        await dbContext.Products.AddRangeAsync(SeedData.Products(), cancellationToken);
        await dbContext.OrderActivity.AddRangeAsync(SeedData.OrderActivity(), cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
