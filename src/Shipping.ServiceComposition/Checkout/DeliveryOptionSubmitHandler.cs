using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.ServiceComposition.Checkout;

public class DeliveryOptionSubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<SelectedDeliveryOption>();

        var message = new SubmitDeliveryOption()
        {
            OrderId = submitted.OrderId,
            DeliveryOptionId = submitted.DeliveryOptionId
        };

        await messageSession.Send(message);
    }

    class SelectedDeliveryOption
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public Guid DeliveryOptionId { get; set; }
    }
}