using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

public class SummaryLoadedSubscriber(CatalogDbContext dbContext) : ICompositionEventsSubscriber
{
    [HttpGet("buy/summary/{orderId}")]
    public void Subscribe(ICompositionEventsPublisher publisher)
    {
        publisher.Subscribe<SummaryLoaded>(async (@event, request) =>
        {
            var productIds = @event.Products.Keys.ToList();
            var products = await dbContext.Products
                .Where(s => productIds.Contains(s.ProductId))
                .ToListAsync(request.HttpContext.RequestAborted);

            foreach (var productItem in @event.Products)
            {
                var product = products.SingleOrDefault(p => p.ProductId == productItem.Key);
                productItem.Value.Name = product?.Name;
                productItem.Value.ImageUrl = product?.ImageUrl;
            }
        });
    }
}
