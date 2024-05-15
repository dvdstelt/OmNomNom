using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.Checkout;

public class DeliverySummaryLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/buy/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliverySummaryLoaded>((@event, request) =>
        {
            var deliveryOptionCollection = dbContext.Database.GetCollection<DeliveryOption>();
            var deliveryOption = deliveryOptionCollection.Query()
                .Where(s => s.DeliveryOptionId == @event.DeliveryOptionId).Single();

            @event.DeliveryOption.Price = deliveryOption.Price;

            var vm = request.GetComposedResponseModel();
            DynamicHelper.TrySetTotalPrice(vm, deliveryOption.Price);

            return Task.CompletedTask;
        });
    }
}