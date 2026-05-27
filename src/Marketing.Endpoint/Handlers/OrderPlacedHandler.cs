using Catalog.Endpoint.Messages.Events;
using Marketing.Data;
using Marketing.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Marketing.Endpoint.Handlers;

// Append the order activity to the log and bump the all-time
// popularity counter (OrderCount). Trending is intentionally NOT
// bumped here: it's a derived value owned by TrendingRecomputeService,
// which periodically rewrites it from the OrderActivity log. If we
// also bumped Trending from this handler we'd race the recompute -
// the recompute reads the activity totals and the products table at
// two different points in time, and a handler bump that lands between
// those two reads gets silently overwritten. Letting the recompute be
// the only writer of Trending costs at most one recompute interval of
// staleness and removes the race entirely.
//
// OrderCount is safe to bump here: it's a monotonic counter with no
// recompute touching it, so additive writes from this handler don't
// race anyone.
//
// No idempotency dedup here - the NServiceBus Outbox is the next thing
// being added and will handle exactly-once for us.
[Handler]
public class OrderPlacedHandler(MarketingDbContext dbContext, ILogger<OrderPlacedHandler> log)
{
    public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        if (message.Items.Count == 0)
            return;

        var productIds = message.Items.Select(i => i.ProductId).ToList();
        var products = await dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId, context.CancellationToken);

        foreach (var item in message.Items)
        {
            dbContext.OrderActivity.Add(new OrderActivity
            {
                ProductId = item.ProductId,
                Quantity = item.FulfilledQuantity,
                OccurredAt = message.OccurredAt
            });

            if (products.TryGetValue(item.ProductId, out var product))
            {
                product.OrderCount += item.FulfilledQuantity;
            }
            else
            {
                // Catalog and Marketing have diverged on which products exist.
                // Activity is preserved in the log, but OrderCount silently
                // understates this product's popularity until someone reseeds.
                log.LogError(
                    "OrderPlaced for unknown ProductId {ProductId} (OrderId {OrderId}); activity logged but OrderCount not bumped. Marketing seed is out of sync with Catalog.",
                    item.ProductId,
                    message.OrderId);
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
