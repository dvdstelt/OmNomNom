using Catalog.ServiceComposition.Events;
using ITOps.Shared;
using Marketing.Data;
using Marketing.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

class ProductLoadedSubscriber(MarketingDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/product/{productId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductLoaded>((@event, request) =>
        {
            var productCollection = dbContext.Database.GetCollection<Product>();
            var product = productCollection.Query().Where(s => s.ProductId == @event.ProductId).Single();

            @event.Product.Stars = product.Stars;
            @event.Product.ReviewCount = product.ReviewCount;

            return Task.CompletedTask;
        });
    }
}