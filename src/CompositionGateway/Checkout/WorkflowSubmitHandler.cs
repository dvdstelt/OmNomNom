using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace CompositionGateway.Checkout;

// User-facing trigger for the atomic checkout submit. Writes the
// CompleteOrder marker slice (so it becomes part of the dispatch
// bundle) and then asks the WorkflowSubmitter to commit the
// outbox transaction. After this returns successfully, the
// per-boundary commands and CompleteOrder are guaranteed to
// dispatch from checkout.db's outbox via the Checkout.Endpoint
// processor.
public class WorkflowSubmitHandler(IWorkflowStore store, IWorkflowSubmitter submitter) : ICompositionRequestsHandler
{
    [HttpPost("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        await store.Write(orderId, CompleteOrderWorkflowSlice.Key, new CompleteOrderSlice(), ct);
        await submitter.Submit(orderId, ct);
    }
}
