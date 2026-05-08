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
        _ = await (
            from o in dbContext.Orders
            join c in dbContext.CreditCards on o.CreditCardId equals c.CreditCardId
            where o.OrderId == message.OrderId
            select new { o, c }).SingleAsync(context.CancellationToken);

        // Send a message to IT/Ops to talk to payment gateway and subtract money.
        // For now we'll assume it all worked and we'll move on.

        var @event = new PaymentSucceeded
        {
            OrderId = message.OrderId
        };
        await context.Publish(@event);
    }
}
