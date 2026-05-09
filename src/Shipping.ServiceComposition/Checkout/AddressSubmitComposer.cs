using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data.Models;
using Shipping.Endpoint.Messages.Commands;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

public class AddressSubmitComposer(IMessageSession messageSession, IWorkflowStore workflow) : ICompositionRequestsHandler
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

        var message = new SubmitShippingAddress
        {
            OrderId = submitted.OrderId,
            FullName = slice.FullName,
            Street = slice.Street,
            ZipCode = slice.ZipCode,
            Town = slice.Town,
            Country = slice.Country
        };
        await messageSession.Send(message);
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
