using Catalog.Data;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.ShoppingCart;

public class ShoppingCartExistsHandler(CacheHelper cacheHelper, CatalogDbContext dbContext): ICompositionRequestsHandler
{
    [HttpGet("/existingCart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        
        if (orderId == Guid.Empty)
        {
            vm.OrderId = Guid.NewGuid();
            vm.CartItems = 0;
            return;
        }
        
        var order = await cacheHelper.GetOrder(orderId);
        vm.OrderId = orderId;
        vm.ItemsInCart = order.Products.Count;
    }
}