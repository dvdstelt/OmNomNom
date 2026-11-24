using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

// Body is a raw JSON array of ShoppingCartItem.
[CompositionHandler]
public class OrderSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/cart/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] List<CartItemForm> items)
    {
        var ct = http.HttpContext!.RequestAborted;

        var slice = new OrderItemsSlice(items
            .Select(i => new OrderItemLine(i.ProductId, i.Quantity))
            .ToList());
        await workflow.Write(orderId, OrderItemsWorkflowSlice.Key, slice, ct);
    }
}
