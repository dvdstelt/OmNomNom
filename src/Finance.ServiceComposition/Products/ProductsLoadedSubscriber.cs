using Catalog.ServiceComposition.Events;
using Finance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

class ProductsLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsHandler<ProductsLoaded>
{
    public async Task Handle(ProductsLoaded @event, HttpRequest request)
    {
        var productIds = @event.Products.Keys.ToList();
        var resultSet = await dbContext.Products
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(request.HttpContext.RequestAborted);

        foreach (var product in @event.Products)
        {
            var matchingProduct = resultSet.Single(s => s.ProductId == product.Key);
            product.Value.Price = matchingProduct.Price;
            product.Value.Discount = matchingProduct.Discount;
        }
    }
}
