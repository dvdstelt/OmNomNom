using Shipping.Data;
using Shipping.Data.Models;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.Endpoint.Handlers;

public class SubmitShippingAddressHandler(ShippingDbContext dbContext) : IHandleMessages<SubmitShippingAddress>
{
    readonly ShippingDbContext dbContext = dbContext;

    public Task Handle(SubmitShippingAddress message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).SingleOrDefault() ?? new Order();

        order.OrderId = message.OrderId;
        order.Address.FullName = message.FullName;
        order.Address.Street = message.Street;
        order.Address.ZipCode = message.ZipCode;
        order.Address.Town = message.Town;
        order.Address.Country = message.Country;

        orderCollection.Upsert(order);

        return Task.CompletedTask;
    }
}