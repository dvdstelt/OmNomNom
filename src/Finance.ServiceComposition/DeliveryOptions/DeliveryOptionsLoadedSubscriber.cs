using Finance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.DeliveryOptions;

public class DeliveryOptionsLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsHandler<DeliveryOptionsLoaded>
{
    public async Task Handle(DeliveryOptionsLoaded @event, HttpRequest request)
    {
        var results = await dbContext.DeliveryOptions
            .ToListAsync(request.HttpContext.RequestAborted);

        foreach (var deliveryOption in @event.DeliveryOptions)
        {
            var matchingOption = results.Single(s => s.DeliveryOptionId == deliveryOption.Key);
            deliveryOption.Value.Price = matchingOption.Price;
        }
    }
}
