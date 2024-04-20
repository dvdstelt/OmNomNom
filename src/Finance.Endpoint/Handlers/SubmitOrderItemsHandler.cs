using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using OrderItem = Finance.Data.Models.OrderItem;

namespace Finance.Endpoint.Handlers;

public class SubmitOrderItemsHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitOrderItems>
{
    readonly FinanceDbContext dbContext = dbContext;

    public Task Handle(SubmitOrderItems message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();

        var order = new Order
        {
            OrderId = message.OrderId
        };
        foreach (var item in message.Items)
        {
            order.Items.Add(new OrderItem() { ProductId = item.ProductId, Quantity = item.Quantity, PriceId = item.PriceId });
        }
        orderCollection.Insert(order);

        return Task.CompletedTask;
    }
}