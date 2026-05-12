using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

public class AddressSubmitComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<OrderAddressDetails>();
        var ct = request.HttpContext.RequestAborted;

        var slice = new ShippingAddressSlice(
            submitted.Details.ShippingAddress.FullName,
            submitted.Details.ShippingAddress.Street,
            submitted.Details.ShippingAddress.ZipCode,
            submitted.Details.ShippingAddress.Town,
            submitted.Details.ShippingAddress.Country);
        await workflow.Write(submitted.OrderId, ShippingAddressWorkflowSlice.Key, slice, ct);
    }

    class OrderAddressDetails
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public OrderAddress Details { get; set; } = null!;

        public class OrderAddress
        {
            public Address ShippingAddress { get; set; } = null!;
        }
    }
}
