using Catalog.ServiceComposition.Events;
using Finance.Data;
using Microsoft.AspNetCore.Http;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Products;

public class OnProductLoaded(FinanceDbContext dbContext) : ICompositionEventsHandler<ProductLoaded>
{
    public async Task Handle(ProductLoaded @event, HttpRequest request)
    {
        var price = await dbContext.CurrentPriceAsync(@event.ProductId, request.HttpContext.RequestAborted);

        // PriceId is the on-screen price the cart locks in at add time.
        @event.Product.PriceId = price.PriceId;
        @event.Product.Price = price.Price;
        @event.Product.Discount = price.Discount;
    }
}
