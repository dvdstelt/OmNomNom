using Catalog.Data;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using Catalog.Endpoint.Messages.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderItem = Catalog.Data.Models.OrderItem;

namespace Catalog.Endpoint.Handlers;

// Convention-based handler (NServiceBus 10.2+): the [Handler] marker
// plus a public Handle(message, IMessageHandlerContext) shape is enough.
// No IHandleMessages<T> interface needed; registration happens via the
// source-generated path triggered by AddHandler<T>() in Program.cs.
[Handler]
public class CompleteOrderHandler(CatalogDbContext dbContext, ILogger<CompleteOrderHandler> log)
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
        var unfulfilled = new List<UnfulfilledItem>();

        foreach (var item in order.Products)
        {
            var inStock = await dbContext.InventoryDeltas.CurrentStockAsync(item.ProductId, ct);

            if (inStock < item.Quantity)
            {
                unfulfilled.Add(new UnfulfilledItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity
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

        return new FulfillmentResult(fulfilled, unfulfilled);
    }

    async Task PublishOutcomeAsync(IMessageHandlerContext context, Guid orderId, FulfillmentResult result)
    {
        if (result.Fulfilled.Count == 0)
        {
            log.LogInformation("{OrderId} - Order cancelled: nothing was in stock.", orderId);
            await context.Publish(new OrderCancelled
            {
                OrderId = orderId,
                UnfulfilledItems = [.. result.Unfulfilled]
            });
            return;
        }

        if (result.Unfulfilled.Count > 0)
        {
            log.LogInformation("{OrderId} - Order partially fulfilled: some items were out of stock.", orderId);
        }

        await context.Publish(new OrderAccepted { OrderId = orderId });

        // OrderPlaced carries both lists: fulfilled drives Marketing's
        // popularity counters and the shipping flow, unfulfilled lets
        // Finance reduce the charge and the email list what was missed.
        await context.Publish(new OrderPlaced
        {
            OrderId = orderId,
            OccurredAt = DateTime.UtcNow,
            Items = [.. result.Fulfilled],
            UnfulfilledItems = [.. result.Unfulfilled]
        });

        log.LogInformation("{OrderId} - Order accepted", orderId);
    }

    sealed record FulfillmentResult(
        IReadOnlyList<OrderedItem> Fulfilled,
        IReadOnlyList<UnfulfilledItem> Unfulfilled);
}
