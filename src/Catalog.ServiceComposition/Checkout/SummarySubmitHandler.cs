using Catalog.Endpoint.Messages.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

public class SummarySubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
    [HttpPost("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var data = await request.Bind<SummaryData>();

        var message = new CompleteOrder()
        {
            OrderId = data.OrderId
        };
        await messageSession.Send(message);
    }

    class SummaryData
    {
        [FromRoute] public Guid OrderId { get; set; }
    }
}