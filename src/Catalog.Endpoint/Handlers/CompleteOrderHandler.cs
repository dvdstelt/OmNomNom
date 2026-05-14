using Catalog.Data;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using Catalog.Endpoint.Messages.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderItem = Catalog.Data.Models.OrderItem;

namespace Catalog.Endpoint.Handlers;

public class CompleteOrderHandler(CatalogDbContext dbContext, ILogger<CompleteOrderHandler> log)
    : IHandleMessages<CompleteOrder>
{
    public async Task Handle(CompleteOrder message, IMessageHandlerContext context)
    {
        var ct = context.CancellationToken;
        log.LogInformation("{OrderId} - Finalizing order", message.OrderId);

        var order = await UpsertOrderAsync(message, ct);
        var result = await TryFulfillAsync(order, ct);
        await dbContext.SaveChangesAsync(ct);

        await PublishOutcomeAsync(context, message.OrderId, result);
    }

    async Task<Order> UpsertOrderAsync(CompleteOrder message, CancellationToken ct)
    {
        // Idempotent against re-delivery: if a row for this OrderId
        // already exists, replace its line items rather than letting the
        // composite (OrderId, ProductId) PK throw.
        var order = await dbContext.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.OrderId == message.OrderId, ct);

        var items = message.Items.Select(i => new OrderItem
        {
            ProductId = i.ProductId,
            Quantity = i.Quantity
        });

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            order.ReplaceItems(items);
            dbContext.Orders.Add(order);
        }
        else
        {
            order.ReplaceItems(items);
        }

        return order;
    }

    async Task<FulfillmentResult> TryFulfillAsync(Order order, CancellationToken ct)
    {
        var fulfilled = new List<OrderedItem>();
        var notFulfilled = new List<OrderItemsNotFulfilled.OrderItem>();

        foreach (var item in order.Products)
        {
            var inStock = await dbContext.InventoryDeltas.CurrentStockAsync(item.ProductId, ct);

            if (inStock < item.Quantity)
            {
                notFulfilled.Add(new OrderItemsNotFulfilled.OrderItem
                {
                    ProductId = item.ProductId,
                    QuantityWanted = item.Quantity,
                    QuantityInStock = inStock
                });
            }
            else
            {
                dbContext.InventoryDeltas.Add(Inventory.Reserve(item.ProductId, item.Quantity));
                fulfilled.Add(new OrderedItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
                });
            }
        }

        return new FulfillmentResult(fulfilled, notFulfilled);
    }

    async Task PublishOutcomeAsync(IMessageHandlerContext context, Guid orderId, FulfillmentResult result)
    {
        if (result.NotFulfilled.Count > 0)
        {
            log.LogInformation("{OrderId} - Some items could not be fulfilled since they were out of stock.", orderId);
            await context.Publish(new OrderItemsNotFulfilled
            {
                OrderId = orderId,
                ItemsNotInStock = [.. result.NotFulfilled]
            });
        }

        if (result.Fulfilled.Count == 0)
        {
            log.LogInformation("{OrderId} - Entire order could not be fulfilled since everything was out of stock.", orderId);
            return;
        }

        await context.Publish(new OrderAccepted { OrderId = orderId });

        // OrderPlaced carries only the in-stock line items so analytics
        // subscribers (Marketing's popularity/trending counters) reflect
        // what was actually ordered, not what the customer asked for.
        await context.Publish(new OrderPlaced
        {
            OrderId = orderId,
            OccurredAt = DateTime.UtcNow,
            Items = [.. result.Fulfilled]
        });

        log.LogInformation("{OrderId} - Order accepted", orderId);
    }

    sealed record FulfillmentResult(
        IReadOnlyList<OrderedItem> Fulfilled,
        IReadOnlyList<OrderItemsNotFulfilled.OrderItem> NotFulfilled);
}
