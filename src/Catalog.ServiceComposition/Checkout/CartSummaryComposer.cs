using System.Dynamic;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

// Lean side-summary handler for the address and payment screens. The
// OrderSummaryCard widget only renders Items / Discount / Shipping /
// Total, so we don't need product names or images here. Reads
// (ProductId, Quantity) pairs from the cart slice and lets Finance
// attach price/discount via a CartSummaryLoaded subscriber.
//
// Shipping is intentionally excluded: that screen renders the full
// `CartItemList` and still needs name/imageUrl, so it stays on the
// existing `ShoppingCartComposer` path.
public class CartSummaryComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    [HttpGet("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;

        var orderedProducts = new Dictionary<Guid, dynamic>();
        foreach (var line in cart.Items)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = line.ProductId;
            vm.Quantity = line.Quantity;
            orderedProducts[line.ProductId] = vm;
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
