using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

public class OnSummaryLoaded(CatalogDbContext dbContext) : ICompositionEventsHandler<SummaryLoaded>
{
    public async Task Handle(SummaryLoaded @event, HttpRequest request)
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
    }
}
