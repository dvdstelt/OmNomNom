using Finance.Data.Models;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

public class AddressSubmitComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
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
