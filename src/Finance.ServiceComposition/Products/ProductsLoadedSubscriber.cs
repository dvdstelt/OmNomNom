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
    [HttpGet("/email/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductsLoaded>((@event, request) =>
        {
            var productIds = @event.Products.Keys.ToList();
            var resultSet = dbContext.Where<Product>(s => productIds.Contains(s.ProductId)).ToList();

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