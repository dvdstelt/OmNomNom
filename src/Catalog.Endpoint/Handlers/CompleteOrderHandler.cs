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
        log.LogInformation("{OrderId} - Finalizing order", message.OrderId);

        // Idempotent upsert: if a row for this OrderId already exists,
        // replace its line items rather than creating a duplicate. The
        // composite (OrderId, ProductId) PK on OrderItem would otherwise
        // throw on a re-delivery.
        var order = await dbContext.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.OrderId == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }
        else
        {
            order.Products.Clear();
        }

        foreach (var item in message.Items)
        {
            order.Products.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        // Verify for each product that is being ordered if there's enough in stock
        var itemsNotFulfilled = new OrderItemsNotFulfilled
        {
            OrderId = message.OrderId,
            ItemsNotInStock = []
        };
        var fulfilledItems = new List<OrderedItem>();

        foreach (var orderItem in order.Products)
        {
            var inStock = await dbContext.InventoryDeltas
                .Where(s => s.ProductId == orderItem.ProductId)
                .SumAsync(s => s.Delta, context.CancellationToken);

            if (inStock < orderItem.Quantity)
            {
                itemsNotFulfilled.ItemsNotInStock.Add(new OrderItemsNotFulfilled.OrderItem
                {
                    ProductId = orderItem.ProductId,
                    QuantityInStock = inStock,
                    QuantityWanted = orderItem.Quantity
                });
            }
            else
            {
                dbContext.InventoryDeltas.Add(new InventoryDelta
                {
                    Delta = orderItem.Quantity * -1,
                    ProductId = orderItem.ProductId,
                    TimeStamp = DateTime.UtcNow
                });
                fulfilledItems.Add(new OrderedItem
                {
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity
                });
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);

        if (itemsNotFulfilled.ItemsNotInStock.Count > 0)
        {
            log.LogInformation("{OrderId} - Some items could not be fulfilled since they were out of stock.", message.OrderId);
            await context.Publish(itemsNotFulfilled);
        }

        if (itemsNotFulfilled.ItemsNotInStock.Count == order.Products.Count)
        {
            // Nothing could be fulfilled. What to do?
            log.LogInformation("{OrderId} - Entire order could not be fulfilled since everything was out of stock.", message.OrderId);
            return;
        }

        // We should figure out what could be ordered and what not, but don't want to make the
        // solution too difficult. Let's continue...

        var orderAccepted = new OrderAccepted { OrderId = message.OrderId };
        await context.Publish(orderAccepted);

        // OrderPlaced carries the line items so analytics subscribers
        // (Marketing's popularity/trending counters) don't have to read
        // from Catalog. Only the in-stock items are reported - the
        // out-of-stock ones never actually got ordered.
        await context.Publish(new OrderPlaced
        {
            OrderId = message.OrderId,
            OccurredAt = DateTime.UtcNow,
            Items = fulfilledItems
        });

        log.LogInformation("{OrderId} - Order accepted", message.OrderId);
    }
}
