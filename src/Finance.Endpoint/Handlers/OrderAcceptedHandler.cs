using Catalog.Endpoint.Messages.Events;
using Finance.Endpoint.Messages.Events;
using NServiceBus.Logging;

namespace Finance.Endpoint.Handlers;

public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
{
    static ILog log = LogManager.GetLogger<OrderAcceptedHandler>();

    public async Task Handle(OrderAccepted message, IMessageHandlerContext context)
    {
        // Send a message to IT/Ops to talk to payment gateway and subtract money.
        // For now we'll assume it all worked and we'll move on.

        var @event = new PaymentSucceeded
        {
            OrderId = message.OrderId
        };
        await context.Publish(@event);
    }
}
