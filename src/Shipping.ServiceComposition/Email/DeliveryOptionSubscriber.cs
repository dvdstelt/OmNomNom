using Finance.ServiceComposition.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.ServiceComposition.Events;

namespace Shipping.ServiceComposition.Email;

public class DeliveryOptionLoadedSubscriber(ShippingDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/email/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliveryOptionLoaded>(async (@event, request) =>
        {
            var result = await dbContext.DeliveryOptions
                .SingleAsync(s => s.DeliveryOptionId == @event.DeliveryOptionId, request.HttpContext.RequestAborted);

            @event.DeliveryOption.Name = result.Name;
            @event.DeliveryOption.Description = result.Description;
        });
    }
}
