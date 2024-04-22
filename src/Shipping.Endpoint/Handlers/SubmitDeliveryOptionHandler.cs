using Shipping.Data;
using Shipping.Data.Models;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.Endpoint.Handlers;

public class SubmitDeliveryOptionHandler(ShippingDbContext dbContext) : IHandleMessages<SubmitDeliveryOption>
{
    readonly ShippingDbContext dbContext = dbContext;

    public Task Handle(SubmitDeliveryOption message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).SingleOrDefault() ?? new Order();

        order.DeliveryOptionId = message.DeliveryOptionId;

        orderCollection.Upsert(order);

        return Task.CompletedTask;
    }
}