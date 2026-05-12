using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

[CompositionHandler]
public class AddressSubmitComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(Guid orderId, [FromBody] OrderAddress details)
    {
        var ct = http.HttpContext!.RequestAborted;

        var slice = new ShippingAddressSlice(
            details.ShippingAddress.FullName,
            details.ShippingAddress.Street,
            details.ShippingAddress.ZipCode,
            details.ShippingAddress.Town,
            details.ShippingAddress.Country);
        await workflow.Write(orderId, ShippingAddressWorkflowSlice.Key, slice, ct);
    }

    public class OrderAddress
    {
        public Address ShippingAddress { get; set; } = null!;
    }
}
