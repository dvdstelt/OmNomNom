namespace Marketing.Contracts;

// Lets Catalog ask Marketing for an ordered ID list when the user is
// sorting on a Marketing-owned signal (rating, popularity, trending).
// Lives in a contracts library so Catalog never source-references
// MarketingDbContext or any Marketing.Data type, even though both
// load into the same gateway process today.
public enum ProductRankBy
{
    Rating,
    OrderCount,
    Trending
}

public interface IProductRanker
{
    // Returns the candidate ProductIds sorted descending by `field`.
    // IDs not present in Marketing's Products table are dropped from
    // the result; Catalog falls back to a stable secondary order
    // (by name) for whatever the ranker returned.
    Task<IReadOnlyList<Guid>> RankAsync(
        IReadOnlyCollection<Guid> candidateIds,
        ProductRankBy field,
        CancellationToken ct);
}
