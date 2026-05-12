using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentInfo.ServiceComposition.Workflow;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace PaymentInfo.ServiceComposition.Checkout;

[CompositionHandler]
public class PaymentSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/buy/payment/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] PaymentForm form)
    {
        var ct = http.HttpContext!.RequestAborted;

        var slice = new PaymentSlice(form.CreditCardId);
        await workflow.Write(orderId, PaymentWorkflowSlice.Key, slice, ct);
    }
}
