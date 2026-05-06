using Finance.Data;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Checkout;

public class DeliverySummaryLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/buy/payment/{orderId}")]
    [HttpGet("/buy/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliverySummaryLoaded>(async (@event, request) =>
        {
            var deliveryOption = await dbContext.DeliveryOptions
                .SingleAsync(s => s.DeliveryOptionId == @event.DeliveryOptionId, request.HttpContext.RequestAborted);

            @event.DeliveryOption.Price = deliveryOption.Price;

            var vm = request.GetComposedResponseModel();
            DynamicHelper.TrySetTotalPrice(vm, deliveryOption.Price);
        });
    }
}
