using System.Security.Cryptography;
using Catalog.ServiceComposition.Events;
using ITOps.Shared;
using Marketing.Data;
using Marketing.Data.Models;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

class ProductsLoadedSubscriber(MarketingDbContext dbContext) : ICompositionEventsSubscriber
{
    readonly ILiteDbContext dbContext = dbContext;

    [HttpGet("/products")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductsLoaded>((@event, request) =>
        {
            var productCollection = dbContext.Database.GetCollection<Product>();
            // Can't find a way to query LiteDb with Contains, so we'll do it like this for now
            var resultSet = productCollection.Query().ToList();

            foreach (var product in @event.Products)
            {
                var matchingProduct  = resultSet.Single(s => s.ProductId == product.Key);
                product.Value.Stars = matchingProduct.Stars;
                product.Value.ReviewCount = matchingProduct.ReviewCount;
            }

            return Task.CompletedTask;
        });
    }
}