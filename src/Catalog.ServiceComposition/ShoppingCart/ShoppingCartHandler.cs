using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.ShoppingCart;

public class ShoppingCartHandler(CacheHelper cacheHelper, CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var order = await cacheHelper.GetOrder(orderId);

        var productsCollection = dbContext.Database.GetCollection<Product>();
        var productIds = order.Products.Select(s => s.ProductId).ToList();
        var products = productsCollection.Query().Where(s => productIds.Contains(s.ProductId)).ToList();
        var orderedProducts = Mapper.MapToDictionary(order, products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartLoaded()
        {
            OrderedProducts = orderedProducts
        });

        vm.OrderId = orderId;
        vm.CartItems = orderedProducts.Values.ToList();
    }
}