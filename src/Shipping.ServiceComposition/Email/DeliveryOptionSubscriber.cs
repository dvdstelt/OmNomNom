using Finance.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Events;

namespace Shipping.ServiceComposition.Email;

public class DeliveryOptionLoadedSubscriber(ShippingDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/email/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<DeliveryOptionLoaded>((@event, request) =>
        {
            var collection = dbContext.Database.GetCollection<DeliveryOption>();
            var results = collection.Query().Where(s => s.DeliveryOptionId == @event.DeliveryOptionId).Single();

            @event.DeliveryOption.Name = results.Name;
            @event.DeliveryOption.Description = results.Description;
            
            return Task.CompletedTask;
        });
    }
}