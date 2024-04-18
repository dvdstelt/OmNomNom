using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using ITOps.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

public class ProductLoadedSubscriber : ICompositionEventsSubscriber
{
    readonly ILiteDbContext dbContext;

    public ProductLoadedSubscriber(FinanceDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    [HttpGet("/product/{productId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductLoaded>(async (@event, request) =>
        {
            var productCollection = dbContext.Database.GetCollection<Product>();
            var product = productCollection.Query().Where(s => s.ProductId == @event.ProductId).Single();

            @event.Product.Price = product.Price;
            @event.Product.Discount = product.Discount;
        });
    }
}