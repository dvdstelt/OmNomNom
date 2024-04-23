using Catalog.Data;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using Catalog.Endpoint.Messages.Events;
using NServiceBus.Logging;

namespace Catalog.Endpoint.Handlers;

public class CompleteOrderHandler(CatalogDbContext dbContext) : IHandleMessages<CompleteOrder>
{
    static ILog log = LogManager.GetLogger<CompleteOrderHandler>();

    public async Task Handle(CompleteOrder message, IMessageHandlerContext context)
    {
        log.InfoFormat("{OrderId} - Finalizing order", message.OrderId);

        // Let's verify if there's stock
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var inventoryDeltas = dbContext.Database.GetCollection<InventoryDelta>();

        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).Single();

        // Verify for each product that is being ordered if there's enough in stock
        var itemsNotFulfilled = new OrderItemsNotFulfilled();
        itemsNotFulfilled.OrderId = message.OrderId;
        foreach (var orderItem in order.Products)
        {
            var stock = inventoryDeltas.Query().Where(s => s.ProductId == orderItem.ProductId).ToList();
            if (stock.Sum(s => s.Delta) < orderItem.Quantity)
            {
                itemsNotFulfilled.ItemsNotInStock.Add(new OrderItemsNotFulfilled.OrderItem()
                {
                    ProductId = orderItem.ProductId,
                    QuantityInStock = stock.Sum(s => s.Delta),
                    QuantityWanted = orderItem.Quantity
                });
            }
            else
            {
                var delta = new InventoryDelta();
                delta.Delta = orderItem.Quantity * -1;
                delta.ProductId = orderItem.ProductId;
                delta.TimeStamp = DateTime.UtcNow;
                inventoryDeltas.Insert(delta);
            }
        }

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
        
        var orderAccepted = new OrderAccepted() { OrderId = message.OrderId };
        await context.Publish(orderAccepted);
        
        log.InfoFormat("{OrderId} - Order accepted", message.OrderId);
    }
}