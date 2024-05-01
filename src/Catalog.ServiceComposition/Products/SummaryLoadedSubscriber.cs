using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

public class SummaryLoadedSubscriber(CatalogDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("buy/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<SummaryLoaded>((@event, request) =>
        {
            var productCollection = dbContext.Database.GetCollection<Product>();
            var productIds = @event.Products.Keys.ToList();
            var products = productCollection.Query().Where(s => productIds.Contains(s.ProductId)).ToList();

            foreach (var productItem in @event.Products)
            {
                var product = products.SingleOrDefault(product => product.ProductId == productItem.Key);
                productItem.Value.Name = product?.Name;
                productItem.Value.ImageUrl = product?.ImageUrl;
            }

            return Task.CompletedTask;
        });
    }
}