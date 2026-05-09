using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

public class AddressSubmitComposer(IMessageSession messageSession, IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<OrderAddressDetails>();
        var ct = request.HttpContext.RequestAborted;

        var slice = new BillingAddressSlice(
            submitted.Details.BillingAddress.FullName,
            submitted.Details.BillingAddress.Street,
            submitted.Details.BillingAddress.ZipCode,
            submitted.Details.BillingAddress.Town,
            submitted.Details.BillingAddress.Country);
        await workflow.Write(submitted.OrderId, BillingAddressWorkflowSlice.Key, slice, ct);

        var message = new SubmitBillingAddress
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
            public Address BillingAddress { get; set; } = null!;
        }
    }
}
