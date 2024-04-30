using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;

namespace Finance.Endpoint.Handlers;

public class SubmitDeliveryOptionHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitDeliveryOption>
{
    public Task Handle(SubmitDeliveryOption message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();

        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).SingleOrDefault();

        if (order == null)
        {
            order = new Order
            {
                OrderId = message.OrderId
            };
        }

        order.DeliveryOptionId = message.DeliveryOptionId;
        orderCollection.Upsert(order);

        return Task.CompletedTask;
    }
}