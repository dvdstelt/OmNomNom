using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentInfo.ServiceComposition.Workflow;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace PaymentInfo.ServiceComposition.Checkout;

public class PaymentSubmitComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<CreditCard>();
        var ct = request.HttpContext.RequestAborted;

        var slice = new PaymentSlice(submitted.Body.CreditCardId);
        await workflow.Write(submitted.OrderId, PaymentWorkflowSlice.Key, slice, ct);
    }

    class CreditCard
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public PaymentBody Body { get; set; } = null!;
    }

    class PaymentBody
    {
        public Guid CreditCardId { get; set; }
    }
}
