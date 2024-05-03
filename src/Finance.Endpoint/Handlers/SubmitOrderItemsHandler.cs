using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using OrderItem = Finance.Data.Models.OrderItem;

namespace Finance.Endpoint.Handlers;

public class SubmitOrderItemsHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitOrderItems>
{
    public Task Handle(SubmitOrderItems message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var productCollection = dbContext.Database.GetCollection<Product>();

        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).SingleOrDefault();

        if (order == null)
        {
            order = new Order
            {
                OrderId = message.OrderId
            };
        }
        order.Items = new();
        order.LocationId = message.LocationId;
        
        foreach (var item in message.Items)
        {
            // NOTE: Never ever do this! Don't retrieve price after submitting the order.
            var product = productCollection.Query().Where(s => s.ProductId == item.ProductId).Single();
            order.Items.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity, Price = product.Price});
        }
        orderCollection.Upsert(order);

        return Task.CompletedTask;
    }
}