using Catalog.Endpoint.Messages.Commands;
using Catalog.Endpoint.Messages.Events;
using NServiceBus.Logging;
using PaymentInfo.Data;
using PaymentInfo.Data.Models;
using PaymentInfo.Endpoint.Messages.Events;

namespace PaymentInfo.Endpoint.Handlers;

public class OrderAcceptedHandler(PaymentInfoDbContext dbContext) : IHandleMessages<OrderAccepted>
{
    static ILog log = LogManager.GetLogger<OrderAcceptedHandler>();

    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var creditCardCollection = dbContext.Database.GetCollection<CreditCard>();
        
        var order = orderCollection.Query().Where(s => s.OrderId == message.OrderId).Single();
        var creditCard = creditCardCollection.Query().Where(s => s.CreditCardId == order.CreditCardId).Single();

        // Send a message to IT/Ops to talk to payment gateway and subtract money.
        // For now we'll assume it all worked and we'll move on.
        
        var @event = new PaymentSucceeded()
        {
            OrderId = message.OrderId
        };
        await context.Publish(@event);
    }
}