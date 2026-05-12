using System.Text.Json;
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
// The body is a raw JSON array of ShoppingCartItem. Read it directly
// from request.Body instead of [FromBody] - ServiceComposer 5.2's
// source generator can't currently encode generic parameter types
// (List<T>, T[]) into hintNames, and we don't want to wrap the
// frontend contract just to work around that.
[CompositionHandler]
public class OrderSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    [HttpPost("/cart/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;
        var items = await JsonSerializer.DeserializeAsync<List<ShoppingCartItem>>(request.Body, JsonOptions, ct) ?? [];

        var slice = new CartSlice(items
            .Select(i => new CartLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(orderId, CartWorkflowSlice.Key, slice, ct);
    }

    public class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
