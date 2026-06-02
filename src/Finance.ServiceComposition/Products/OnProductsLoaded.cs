using Catalog.ServiceComposition.Events;
using Finance.Data;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

class OnProductsLoaded(FinanceDbContext dbContext) : ICompositionEventsHandler<ProductsLoaded>
{
    public async Task Handle(ProductsLoaded @event, HttpRequest request)
    {
        var productIds = @event.Products.Keys.ToList();
        var prices = await dbContext.CurrentPricesAsync(productIds, request.HttpContext.RequestAborted);

        foreach (var product in @event.Products)
        {
            var price = prices[product.Key];
            product.Value.PriceId = price.PriceId;
            product.Value.Price = price.Price;
            product.Value.Discount = price.Discount;
        }
    }
}
