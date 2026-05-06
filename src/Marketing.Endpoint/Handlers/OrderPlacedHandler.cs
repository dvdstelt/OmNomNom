using Catalog.Endpoint.Messages.Events;
using Marketing.Data;
using Marketing.Data.Models;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Logging;

namespace Marketing.Endpoint.Handlers;

// Append the order activity to the log, bump the all-time popularity
// counter, and bump the rolling trending counter (a fresh event is
// always within the 30-day window so the bump is always valid).
// Decay is handled separately by TrendingRecomputeService.
//
// No idempotency dedup here - the NServiceBus Outbox is the next thing
// being added and will handle exactly-once for us.
public class OrderPlacedHandler(MarketingDbContext dbContext) : IHandleMessages<OrderPlaced>
{
    static readonly ILog log = LogManager.GetLogger<OrderPlacedHandler>();

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
                Quantity = item.Quantity,
                OccurredAt = message.OccurredAt
            });

            if (products.TryGetValue(item.ProductId, out var product))
            {
                product.OrderCount += item.Quantity;
                product.Trending += item.Quantity;
            }
            else
            {
                log.WarnFormat("OrderPlaced for unknown ProductId {ProductId}; activity logged but counters not bumped", item.ProductId);
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
