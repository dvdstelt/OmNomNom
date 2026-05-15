using Finance.Data;
using Finance.Data.Domain;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Checkout;

public class OnDeliverySummaryLoaded(FinanceDbContext dbContext, OrderSubtotalReader orderReader)
    : ICompositionEventsHandler<DeliverySummaryLoaded>
{
    public async Task Handle(DeliverySummaryLoaded @event, HttpRequest request)
    {
        var ct = request.HttpContext.RequestAborted;
        var orderId = Guid.Parse((string)request.RouteValues["orderId"]!);

        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == @event.DeliveryOptionId, ct);

        var itemsSubtotal = await orderReader.GetItemsSubtotalAsync(orderId, ct);
        var effectivePrice = ShippingFees.EffectivePrice(deliveryOption, itemsSubtotal);

        @event.DeliveryOption.Price = effectivePrice;

        var vm = request.GetComposedResponseModel();
        DynamicHelper.TrySetTotalPrice(vm, effectivePrice);
    }
}
