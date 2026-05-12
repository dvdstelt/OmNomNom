using Finance.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;

namespace Shipping.ServiceComposition.Email;

public class DeliveryOptionLoadedSubscriber(ShippingDbContext dbContext) : ICompositionEventsHandler<DeliveryOptionLoaded>
{
    public async Task Handle(DeliveryOptionLoaded @event, HttpRequest request)
    {
        var result = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == @event.DeliveryOptionId, request.HttpContext.RequestAborted);

        @event.DeliveryOption.Name = result.Name;
        @event.DeliveryOption.Description = result.Description;
    }
}
