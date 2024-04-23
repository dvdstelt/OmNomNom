using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using ITOps.Shared;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

class ProductsLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    readonly ILiteDbContext dbContext = dbContext;

    [HttpGet("/products")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductsLoaded>((@event, request) =>
        {
            var productsCollection = dbContext.Database.GetCollection<Product>();
            // TODO: Figure out if `Contains` is possible with LiteDb
            var resultSet = productsCollection.Query().ToList();

            foreach (var product in @event.Products)
            {
                var matchingProduct  = resultSet.Single(s => s.ProductId == product.Key);
                product.Value.Price = matchingProduct.Price;
                product.Value.Discount = matchingProduct.Discount;
            }

            return Task.CompletedTask;
        });
    }
}