using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.DeliveryOptions;

public class DeliveryOptionsLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/buy/shipping/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliveryOptionsLoaded>(async (@event, request) =>
        {
            var results = await dbContext.DeliveryOptions
                .ToListAsync(request.HttpContext.RequestAborted);

            foreach (var deliveryOption in @event.DeliveryOptions)
            {
                var matchingOption = results.Single(s => s.DeliveryOptionId == deliveryOption.Key);
                deliveryOption.Value.Price = matchingOption.Price;
            }
        });
    }
}
