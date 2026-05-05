using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

public class ShoppingCartHandler(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    [HttpGet("/buy/shipping/{orderId}")]
    [HttpGet("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders
            .Include(o => o.Products)
            .SingleOrDefaultAsync(s => s.OrderId == orderId, ct);

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
