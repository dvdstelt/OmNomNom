using Finance.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Finance.Data;

// Resolves the "current" price for a product from its append-only price
// history: the entry with the highest ValidFrom (PriceId breaks ties so
// the result is deterministic). Demo-scale data, so the latest-per-product
// pick happens in memory rather than as a translated GROUP BY.
public static class ProductPriceQueries
{
    public static async Task<ProductPrice> CurrentPriceAsync(
        this FinanceDbContext dbContext, Guid productId, CancellationToken ct)
    {
        var rows = await dbContext.ProductPrices
            .Where(p => p.ProductId == productId)
            .ToListAsync(ct);
        return Latest(rows.GroupBy(p => p.ProductId).Single());
    }

    public static async Task<Dictionary<Guid, ProductPrice>> CurrentPricesAsync(
        this FinanceDbContext dbContext, IReadOnlyCollection<Guid> productIds, CancellationToken ct)
    {
        var rows = await dbContext.ProductPrices
            .Where(p => productIds.Contains(p.ProductId))
            .ToListAsync(ct);
        return rows.GroupBy(p => p.ProductId)
            .ToDictionary(g => g.Key, Latest);
    }

    static ProductPrice Latest(IEnumerable<ProductPrice> prices) =>
        prices.OrderByDescending(p => p.ValidFrom).ThenByDescending(p => p.PriceId).First();
}
