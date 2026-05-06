using Catalog.ServiceComposition.Events;
using Finance.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

public class ProductLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/product/{productId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductLoaded>(async (@event, request) =>
        {
            var product = await dbContext.Products
                .SingleAsync(s => s.ProductId == @event.ProductId, request.HttpContext.RequestAborted);

            @event.Product.Price = product.Price;
            @event.Product.Discount = product.Discount;
        });
    }
}
