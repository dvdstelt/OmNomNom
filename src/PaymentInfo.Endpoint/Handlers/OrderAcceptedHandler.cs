using Catalog.Endpoint.Messages.Events;
using Microsoft.EntityFrameworkCore;
using NServiceBus.Logging;
using PaymentInfo.Data;
using PaymentInfo.Endpoint.Messages.Events;

namespace PaymentInfo.Endpoint.Handlers;

public class OrderAcceptedHandler(PaymentInfoDbContext dbContext) : IHandleMessages<OrderAccepted>
{
    static ILog log = LogManager.GetLogger<OrderAcceptedHandler>();

    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        var order = await dbContext.Orders
            .SingleAsync(s => s.OrderId == message.OrderId, context.CancellationToken);
        var creditCard = await dbContext.CreditCards
            .SingleAsync(s => s.CreditCardId == order.CreditCardId, context.CancellationToken);

        // Send a message to IT/Ops to talk to payment gateway and subtract money.
        // For now we'll assume it all worked and we'll move on.

        var @event = new PaymentSucceeded
        {
            OrderId = message.OrderId
        };
        await context.Publish(@event);
    }
}
