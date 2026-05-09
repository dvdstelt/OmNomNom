using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

public class DeliveryOptionSubmitComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<SelectedDeliveryOption>();
        var ct = request.HttpContext.RequestAborted;

        var slice = new DeliveryOptionSlice(submitted.Body.DeliveryOptionId);
        await workflow.Write(submitted.OrderId, DeliveryOptionWorkflowSlice.Key, slice, ct);
    }

    class SelectedDeliveryOption
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public BodyModel Body { get; set; } = null!;
    }

    class BodyModel
    {
        public Guid DeliveryOptionId { get; set; }
    }
}
