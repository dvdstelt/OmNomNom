using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

// Cart-page submit. The user clicked "Proceed to checkout" on /cart;
// the body carries the final line items (cart-page +/- and Remove
// edits arrive here for the first time). Updates the cart slice;
// SubmitOrderItems will be dispatched later by SummarySubmitComposer
// as part of the atomic checkout submit.
//
// The body is a raw JSON array of ShoppingCartItem.
[CompositionHandler]
public class OrderSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] List<CartItemForm> items)
    {
        var ct = http.HttpContext!.RequestAborted;

        var slice = new CartSlice(items
            .Select(i => new CartLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(orderId, CartWorkflowSlice.Key, slice, ct);
    }
}
