using Catalog.Data;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using OrderItem = Catalog.Data.Models.OrderItem;

namespace Catalog.Endpoint.Handlers;

public class SubmitOrderItemsHandler(CatalogDbContext dbContext) : IHandleMessages<SubmitOrderItems>
{
    public Task Handle(SubmitOrderItems message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();

        var order = new Order
        {
            OrderId = message.OrderId,
            LocationId = message.LocationId,
        };
        foreach (var item in message.Items)
        {
            order.Products.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity });
        }
        orderCollection.Upsert(order);

        return Task.CompletedTask;
    }
}