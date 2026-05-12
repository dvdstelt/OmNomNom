using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

[CompositionHandler]
public class AddressSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] ShippingAddressForm form)
    {
        var ct = http.HttpContext!.RequestAborted;

        var slice = new ShippingAddressSlice(
            form.ShippingAddress.FullName,
            form.ShippingAddress.Street,
            form.ShippingAddress.ZipCode,
            form.ShippingAddress.Town,
            form.ShippingAddress.Country);
        await workflow.Write(orderId, ShippingAddressWorkflowSlice.Key, slice, ct);
    }
}
