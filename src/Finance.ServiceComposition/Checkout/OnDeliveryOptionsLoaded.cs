using Finance.Data;
using Finance.Data.Domain;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Checkout;

public class OnDeliveryOptionsLoaded(FinanceDbContext dbContext, OrderSubtotalReader orderReader)
    : ICompositionEventsHandler<DeliveryOptionsLoaded>
{
    public async Task Handle(DeliveryOptionsLoaded @event, HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;
        var orderId = Guid.Parse((string)request.RouteValues["orderId"]!);

        var options = await dbContext.DeliveryOptions.ToListAsync(ct);
        var itemsSubtotal = await orderReader.GetItemsSubtotalAsync(orderId, ct);

        foreach (var deliveryOption in @event.DeliveryOptions)
        {
            var match = options.Single(s => s.DeliveryOptionId == deliveryOption.Key);
            deliveryOption.Value.Price = ShippingFees.EffectivePrice(match, itemsSubtotal);
        }
    }
}
