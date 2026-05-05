using Microsoft.EntityFrameworkCore;

namespace Shipping.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        ShippingDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.MigrateAsync(cancellationToken);

        if (await dbContext.DeliveryOptions.AnyAsync(cancellationToken))
            return;

        await dbContext.DeliveryOptions.AddRangeAsync(SeedData.DeliveryOptions(), cancellationToken);
        await dbContext.Orders.AddAsync(SeedData.Orders(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
