﻿using Finance.Endpoint.Messages.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class DeliveryOptionSubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<SelectedDeliveryOption>();

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