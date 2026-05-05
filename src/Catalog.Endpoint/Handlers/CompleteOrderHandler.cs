using Catalog.Data;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using Catalog.Endpoint.Messages.Events;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Logging;

namespace Catalog.Endpoint.Handlers;

public class CompleteOrderHandler(CatalogDbContext dbContext) : IHandleMessages<CompleteOrder>
{
    static ILog log = LogManager.GetLogger<CompleteOrderHandler>();

    public async Task Handle(CompleteOrder message, IMessageHandlerContext context)
    {
        log.InfoFormat("{OrderId} - Finalizing order", message.OrderId);

        var order = await dbContext.Orders
            .Include(o => o.Products)
            .SingleAsync(s => s.OrderId == message.OrderId, context.CancellationToken);

        // Verify for each product that is being ordered if there's enough in stock
        var itemsNotFulfilled = new OrderItemsNotFulfilled
        {
            OrderId = message.OrderId
        };

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
            }
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);

        if (itemsNotFulfilled.ItemsNotInStock.Count > 0)
        {
            log.InfoFormat("{OrderId} - Some items could not be fulfilled since they were out of stock.", message.OrderId);
            await context.Publish(itemsNotFulfilled);
        }

        if (itemsNotFulfilled.ItemsNotInStock.Count == order.Products.Count)
        {
            // Nothing could be fulfilled. What to do?
            log.InfoFormat("{OrderId} - Entire order could not be fulfilled since everything was out of stock.", message.OrderId);
            return;
        }

        // We should figure out what could be ordered and what not, but don't want to make the
        // solution too difficult. Let's continue...

        var orderAccepted = new OrderAccepted { OrderId = message.OrderId };
        await context.Publish(orderAccepted);

        log.InfoFormat("{OrderId} - Order accepted", message.OrderId);
    }
}
