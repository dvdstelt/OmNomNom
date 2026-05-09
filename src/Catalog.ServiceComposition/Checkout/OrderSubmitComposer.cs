using System.Text;
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
// SubmitOrderItems will be dispatched later by WorkflowSubmitHandler
// as part of the atomic checkout submit.
public class OrderSubmitComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var data = await request.Bind<ShoppingCart>();
        var ct = request.HttpContext.RequestAborted;

        request.Body.Position = 0;
        using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        var items = JsonSerializer.Deserialize<List<ShoppingCartItem>>(body,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];

        var slice = new CartSlice(items
            .Select(i => new CartLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(data.OrderId, CartWorkflowSlice.Key, slice, ct);
    }

    class ShoppingCart
    {
        [FromRoute] public Guid OrderId { get; set; }
    }

    class ShoppingCartItem
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
