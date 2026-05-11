using Catalog.ServiceComposition.Events;
using Marketing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

// Sorts the candidate ProductIds by a Marketing-owned signal and writes
// the result back into the event. Catalog never names a Marketing sort
// dimension; this subscriber reads ?sort= straight off the request and
// owns the vocabulary end to end.
class ProductCandidatesAvailableSubscriber(MarketingDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/products")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductCandidatesAvailable>(async (@event, request) =>
        {
            if (@event.CandidateIds.Count == 0) return;

            if (!request.Query.TryGetValue("sort", out var values) || values.Count == 0)
                return;

            var ct = request.HttpContext.RequestAborted;
            var ids = @event.CandidateIds.ToList();
            var query = dbContext.Products.Where(p => ids.Contains(p.ProductId));

            // ThenBy(ProductId) is a stable tiebreaker so paginated rank order is reproducible across requests.
            query = values[0]?.ToLowerInvariant() switch
            {
                "rating" => query.OrderByDescending(p => p.Rating).ThenBy(p => p.ProductId),
                "ordercount" => query.OrderByDescending(p => p.OrderCount).ThenBy(p => p.ProductId),
                "trending" => query.OrderByDescending(p => p.Trending).ThenBy(p => p.ProductId),
                _ => null
            };

            if (query is null) return;

            @event.OrderedIds = await query
                .Skip((@event.Page - 1) * @event.Size)
                .Take(@event.Size)
                .Select(p => p.ProductId)
                .ToListAsync(ct);
        });
    }
}
