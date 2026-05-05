using Catalog.ServiceComposition.Events;
using Marketing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

class ProductLoadedSubscriber(MarketingDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/product/{productId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductLoaded>(async (@event, request) =>
        {
            var product = await dbContext.Products
                .SingleAsync(s => s.ProductId == @event.ProductId, request.HttpContext.RequestAborted);

            @event.Product.Stars = product.Stars;
            @event.Product.ReviewCount = product.ReviewCount;
        });
    }
}
