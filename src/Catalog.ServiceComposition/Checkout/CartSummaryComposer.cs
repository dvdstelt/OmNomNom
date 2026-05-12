using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
// existing `ShippingScreenCartComposer` path.
[CompositionHandler]
public class CartSummaryComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    // ServiceComposer 5.x allows only one Http* attribute per method,
    // so the address and payment screens get their own entry points
    // that both delegate to LoadAsync. The work is identical.
    [HttpGet("/buy/address/{orderId}")]
    public Task HandleAddress(Guid orderId) => LoadAsync(orderId);

    [HttpGet("/buy/payment/{orderId}")]
    public Task HandlePayment(Guid orderId) => LoadAsync(orderId);

    async Task LoadAsync(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct)
                   ?? CartSlice.Empty;

        var orderedProducts = Mapper.MapToDictionary(cart);

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
