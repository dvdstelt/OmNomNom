using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using ITOps.Shared;
using LiteDB;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

class ProductsLoadedSubscriber : ICompositionEventsSubscriber
{
    readonly ILiteDbContext dbContext;

    public ProductsLoadedSubscriber(FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet("/products")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductsLoaded>(async (@event, request) =>
        {
            var col = dbContext.Database.GetCollection<Product>();
            // Can't find a way to query LiteDb with Contains, so we'll do it like this for now
            var resultSet = col.Query().ToList();

            foreach (var product in @event.Products)
            {
                var matchingProduct  = resultSet.Single(s => s.ProductId == product.Key);
                product.Value.Price = matchingProduct.Price;
                product.Value.Discount = matchingProduct.Discount;
            }
        });
    }
}