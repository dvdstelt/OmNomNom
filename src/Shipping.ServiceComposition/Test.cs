using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Endpoint.Messages.Events;

namespace Shipping.ServiceComposition;

public class Test(IMessageSession messageSession) : ICompositionRequestsHandler
{
    [HttpGet("/test")]
    public async Task Handle(HttpRequest request)
    {
        var message = new OrderShipped()
        {
            OrderId = Guid.NewGuid()
        };

        await messageSession.Publish(message);
    }
}