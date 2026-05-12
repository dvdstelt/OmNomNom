using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

[CompositionHandler]
public class AddressSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] BillingAddressForm form)
    {
        var ct = http.HttpContext!.RequestAborted;

        await workflow.Write(
            orderId,
            BillingAddressWorkflowSlice.Key,
            new BillingAddressSlice(form.BillingAddress),
            ct);
    }
}
