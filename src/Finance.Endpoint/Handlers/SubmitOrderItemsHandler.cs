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

        var order = new Order
        {
            OrderId = message.OrderId
        };
        foreach (var item in message.Items)
        {
            // NOTE: Never ever do this! Don't retrieve price after submitting the order.
            var product = productCollection.Query().Where(s => s.ProductId == item.ProductId).Single();
            order.Items.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity, Price = product.Price});
        }
        orderCollection.Insert(order);

        return Task.CompletedTask;
    }
}