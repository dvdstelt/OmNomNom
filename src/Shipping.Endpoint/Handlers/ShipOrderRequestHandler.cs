using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Messages;

namespace Shipping.Endpoint.Handlers;

public class ShipOrderRequestHandler : IHandleMessages<ShipOrderRequest>
{
    public async Task Handle(ShipOrderRequest message, IMessageHandlerContext context)
    {
        await Task.Delay(1000, context.CancellationToken);
        
        var reply = new ShipOrderReply();
        await context.Reply(reply);
    }
}