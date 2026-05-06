using Catalog.ServiceComposition.Events;
using Marketing.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

class ProductsLoadedSubscriber(MarketingDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("/products")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<ProductsLoaded>(async (@event, request) =>
        {
            var productIds = @event.Products.Keys.ToList();
            var resultSet = await dbContext.Products
                .Where(s => productIds.Contains(s.ProductId))
                .ToListAsync(request.HttpContext.RequestAborted);

            foreach (var product in @event.Products)
            {
                var matchingProduct = resultSet.Single(s => s.ProductId == product.Key);
                product.Value.Stars = matchingProduct.Stars;
                product.Value.ReviewCount = matchingProduct.ReviewCount;
            }
        });
    }
}
