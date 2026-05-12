using Finance.Data;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Checkout;

public class DeliverySummaryLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsHandler<DeliverySummaryLoaded>
{
    public async Task Handle(DeliverySummaryLoaded @event, HttpRequest request)
    {
        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == @event.DeliveryOptionId, request.HttpContext.RequestAborted);

        @event.DeliveryOption.Price = deliveryOption.Price;

        var vm = request.GetComposedResponseModel();
        DynamicHelper.TrySetTotalPrice(vm, deliveryOption.Price);
    }
}
