using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Messages;

namespace Shipping.Endpoint.Handlers;

public class ShipOrderRequestHandler : IHandleMessages<ShipOrderRequest>
{
    public async Task Handle(ShipOrderRequest message, IMessageHandlerContext context)
    {
        // Actually ship the order.
        // This means we need to talk to a 3rd party API that initiates this
        // We don't do that from sagas. Ask me why ;-)
        await Task.Delay(1000, context.CancellationToken);
        
        var reply = new ShipOrderReply();
        await context.Reply(reply);
    }
}