using Catalog.ServiceComposition.Events;
using Marketing.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Marketing.ServiceComposition.Products;

class OnProductLoaded(MarketingDbContext dbContext) : ICompositionEventsHandler<ProductLoaded>
{
    public async Task Handle(ProductLoaded @event, HttpRequest request)
    {
        var product = await dbContext.Products
            .SingleAsync(s => s.ProductId == @event.ProductId, request.HttpContext.RequestAborted);

        @event.Product.Rating = product.Rating;
        @event.Product.RatingCount = product.RatingCount;
        @event.Product.OrderCount = product.OrderCount;
        @event.Product.Trending = product.Trending;
    }
}
