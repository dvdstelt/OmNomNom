using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Endpoint.Messages.Commands;
using Shipping.ServiceComposition.Helpers;

namespace Shipping.ServiceComposition.Checkout;

public class DeliveryOptionSubmitHandler(IMessageSession messageSession, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<SelectedDeliveryOption>();

        var order = await cacheHelper.GetOrder(submitted.OrderId);
        order.DeliveryOptionId = submitted.Body.DeliveryOptionId;
        await cacheHelper.StoreOrder(order);
        
        var message = new SubmitDeliveryOption()
        {
            OrderId = submitted.OrderId,
            DeliveryOptionId = submitted.Body.DeliveryOptionId
        };

        await messageSession.Send(message);
    }

    class SelectedDeliveryOption
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public BodyModel Body { get; set; }
    }

    class BodyModel
    {
        public Guid DeliveryOptionId { get; set; }
    }
}