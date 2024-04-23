using Shipping.Endpoint.Messages.Commands;
using Shipping.Endpoint.Messages.Messages;

namespace Shipping.Endpoint.Handlers;

public class ShipOrderRequestHandler : IHandleMessages<ShipOrderRequest>
{
    public async Task Handle(ShipOrderRequest message, IMessageHandlerContext context)
    {
        var reply = new ShipOrderReply();
        await context.Reply(reply);
    }
}