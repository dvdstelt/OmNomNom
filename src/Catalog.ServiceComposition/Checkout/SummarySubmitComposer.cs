using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

// User-facing trigger for the atomic checkout submit. Snapshots the
// cart into the CompleteOrderSlice (so the items travel with the
// trigger command instead of via a separate SubmitOrderItems hop),
// then asks the WorkflowSubmitter to commit the outbox transaction.
// After this returns successfully, the per-boundary commands and
// CompleteOrder are guaranteed to dispatch from checkout.db's outbox
// via the Checkout.Endpoint processor.
[CompositionHandler]
public class SummarySubmitComposer(IWorkflowStore store, IWorkflowSubmitter submitter, IHttpContextAccessor http)
{
    [HttpPost("/buy/summary/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        // Defensive 410: the GET-side composer also 410s on a missing
        // slice, but the cart could have been reaped *between* the
        // summary loading and the customer clicking "Place order".
        // Without this we'd silently submit an empty order.
        var cart = await store.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct);
        if (cart is null)
        {
            request.SetActionResult(new StatusCodeResult(StatusCodes.Status410Gone));
            return;
        }

        await store.Write(orderId, CompleteOrderWorkflowSlice.Key, new CompleteOrderSlice(cart.Items), ct);
        await submitter.Submit(orderId, ct);
    }
}
