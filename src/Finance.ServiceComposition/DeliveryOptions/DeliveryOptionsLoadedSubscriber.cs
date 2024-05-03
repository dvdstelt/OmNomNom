using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.DeliveryOptions;

public class DeliveryOptionsLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/buy/shipping/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliveryOptionsLoaded>((@event, request) =>
        {
            var collection = dbContext.Database.GetCollection<DeliveryOption>();
            var results = collection.Query().ToList();

            foreach (var deliveryOption in @event.DeliveryOptions)
            {
                var matchingOption = results.Single(s => s.Id.DeliveryOptionId == deliveryOption.Key && s.Id.LocationId == @event.LocationId);
                deliveryOption.Value.Price = matchingOption.Price;
            }
            
            return Task.CompletedTask;
        });
    }
}