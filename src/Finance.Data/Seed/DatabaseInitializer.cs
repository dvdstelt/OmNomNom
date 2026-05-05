using Microsoft.EntityFrameworkCore;

namespace Finance.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        FinanceDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        await dbContext.Products.AddRangeAsync(SeedData.Products(), cancellationToken);
        await dbContext.DeliveryOptions.AddRangeAsync(SeedData.DeliveryOptions(), cancellationToken);
        await dbContext.Orders.AddAsync(SeedData.Orders(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
