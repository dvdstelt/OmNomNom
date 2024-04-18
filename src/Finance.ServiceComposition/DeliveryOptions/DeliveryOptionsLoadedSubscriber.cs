using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Events;

namespace Finance.ServiceComposition.DeliveryOptions;

public class DeliveryOptionsLoadedSubscriber : ICompositionEventsSubscriber
{
    private readonly FinanceDbContext dbContext;

    public DeliveryOptionsLoadedSubscriber(FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet("/deliveryoptions")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliveryOptionsLoaded>((@event, request) =>
        {
            var collection = dbContext.Database.GetCollection<DeliveryOption>();
            var results = collection.Query().ToList();

            foreach (var deliveryOption in @event.DeliveryOptions)
            {
                var matchingOption  = results.Single(s => s.DeliveryOptionId == deliveryOption.Key);
                deliveryOption.Value.Price = matchingOption.Price;
            }
            
            return Task.CompletedTask;
        });
    }
}