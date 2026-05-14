using Catalog.Endpoint.Messages.Events;
using Finance.Endpoint.Messages.Events;

namespace Finance.Endpoint.Handlers;

public class OrderAcceptedHandler : IHandleMessages<OrderAccepted>
{
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
