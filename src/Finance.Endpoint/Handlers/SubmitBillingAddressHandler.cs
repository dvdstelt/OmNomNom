using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;

namespace Finance.Endpoint.Handlers;

public class SubmitBillingAddressHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitBillingAddress>
{
    public Task Handle(SubmitBillingAddress message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).SingleOrDefault() ?? new Order();

        order.OrderId = message.OrderId;
        order.BillingAddress = new();
        order.BillingAddress.FullName = message.FullName;
        order.BillingAddress.Street = message.Street;
        order.BillingAddress.ZipCode = message.ZipCode;
        order.BillingAddress.Town = message.Town;
        order.BillingAddress.Country = message.Country;

        orderCollection.Upsert(order);

        return Task.CompletedTask;
    }
}