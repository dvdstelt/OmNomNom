using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

[CompositionHandler]
public class DeliveryOptionSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] SelectedDeliveryOption body)
    {
        var ct = http.HttpContext!.RequestAborted;

        var slice = new DeliveryOptionSlice(body.DeliveryOptionId);
        await workflow.Write(orderId, DeliveryOptionWorkflowSlice.Key, slice, ct);
    }

    public class SelectedDeliveryOption
    {
        public Guid DeliveryOptionId { get; set; }
    }
}
