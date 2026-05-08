using Marketing.Contracts;
using Marketing.Data;
using Microsoft.EntityFrameworkCore;

namespace Marketing.ServiceComposition.Products;

// Implementation of IProductRanker. Translates the rank field enum
// into a column on Marketing.Product, restricts the query to the
// caller's candidate set, and projects the ProductIds in sort order.
internal sealed class ProductRanker(MarketingDbContext dbContext) : IProductRanker
{
    public async Task<IReadOnlyList<Guid>> RankAsync(
        IReadOnlyCollection<Guid> candidateIds,
        ProductRankBy field,
        CancellationToken ct)
    {
        if (candidateIds.Count == 0)
            return [];

        var ids = candidateIds.ToList();
        var query = dbContext.Products.Where(p => ids.Contains(p.ProductId));

        // ThenBy(ProductId) is a stable tiebreaker so paginated rank order is reproducible across requests.
        query = field switch
        {
            ProductRankBy.Rating => query.OrderByDescending(p => p.Rating).ThenBy(p => p.ProductId),
            ProductRankBy.OrderCount => query.OrderByDescending(p => p.OrderCount).ThenBy(p => p.ProductId),
            ProductRankBy.Trending => query.OrderByDescending(p => p.Trending).ThenBy(p => p.ProductId),
            _ => throw new ArgumentOutOfRangeException(nameof(field), field, "Unknown rank field")
        };

        return await query
            .Select(p => p.ProductId)
            .ToListAsync(ct);
    }
}
