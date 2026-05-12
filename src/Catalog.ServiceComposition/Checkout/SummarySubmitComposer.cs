using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

// User-facing trigger for the atomic checkout submit. Writes the
// CompleteOrder marker slice (so it becomes part of the dispatch
// bundle) and then asks the WorkflowSubmitter to commit the
// outbox transaction. After this returns successfully, the
// per-boundary commands and CompleteOrder are guaranteed to
// dispatch from checkout.db's outbox via the Checkout.Endpoint
// processor.
[CompositionHandler]
public class SummarySubmitComposer(IWorkflowStore store, IWorkflowSubmitter submitter, IHttpContextAccessor http)
{
    [HttpPost("/buy/summary/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var ct = http.HttpContext!.RequestAborted;

        await store.Write(orderId, CompleteOrderWorkflowSlice.Key, new CompleteOrderSlice(), ct);
        await submitter.Submit(orderId, ct);
    }
}
