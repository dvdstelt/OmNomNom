using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

// Powers the shipping screen, which renders the full `CartItemList`
// (product image + name) on top of the order summary. Reads from the
// SQLite Orders row first; if the SubmitOrderItems message hasn't been
// processed yet, falls back to the in-flight cart in the distributed
// cache so the page renders cleanly while the read model catches up.
//
// Address and payment use the leaner `CartSummaryHandler` instead.
public class ShoppingCartHandler(CatalogDbContext dbContext, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders
            .Include(o => o.Products)
            .SingleOrDefaultAsync(s => s.OrderId == orderId, ct);

        if (order == null)
        {
            order = await cacheHelper.GetOrder(orderId);
        }

        var products = await dbContext.Products.ToListAsync(ct);
        var orderedProducts = ShoppingCart.Mapper.MapToDictionary(order, products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartLoaded
        {
            OrderedProducts = orderedProducts
        });

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
        vm.CartItems = orderedProducts.Values.ToList();
    }
}
