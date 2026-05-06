using System.Dynamic;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

// Lean side-summary handler for the address and payment screens. The
// `OrderSummaryCard` widget only renders Items / Discount / Shipping /
// Total, so we don't need product names or images here. Reads the cart
// from the distributed cache (the source of truth for an in-flight
// checkout), emits (ProductId, Quantity) pairs, and lets Finance attach
// price/discount via a `CartSummaryLoaded` subscriber.
//
// Shipping is intentionally excluded: that screen renders the full
// `CartItemList` and still needs name/imageUrl, so it stays on the
// existing `ShoppingCartHandler` path.
public class CartSummaryHandler(CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    [HttpGet("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var order = await cacheHelper.GetOrder(orderId);

        var orderedProducts = new Dictionary<Guid, dynamic>();
        foreach (var item in order.Products)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = item.ProductId;
            vm.Quantity = item.Quantity;
            orderedProducts[item.ProductId] = vm;
        }

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartSummaryLoaded
        {
            OrderId = orderId,
            OrderedProducts = orderedProducts
        });

        var responseModel = request.GetComposedResponseModel();
        responseModel.OrderId = orderId;
        responseModel.CartItems = orderedProducts.Values.ToList();
    }
}
