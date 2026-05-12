using Catalog.ServiceComposition.Events;
using Finance.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

public class ProductLoadedSubscriber(FinanceDbContext dbContext) : ICompositionEventsHandler<ProductLoaded>
{
    public async Task Handle(ProductLoaded @event, HttpRequest request)
    {
        var product = await dbContext.Products
            .SingleAsync(s => s.ProductId == @event.ProductId, request.HttpContext.RequestAborted);

        @event.Product.Price = product.Price;
        @event.Product.Discount = product.Discount;
    }
}
