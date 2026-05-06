using Microsoft.EntityFrameworkCore;

namespace PaymentInfo.Data.Seed;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(
        PaymentInfoDbContext dbContext,
        CancellationToken cancellationToken = default)
    {
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (await dbContext.CreditCards.AnyAsync(cancellationToken))
            return;

        await dbContext.CreditCards.AddRangeAsync(SeedData.Products(), cancellationToken);
        await dbContext.Orders.AddAsync(SeedData.Orders(), cancellationToken);

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
