using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

[CompositionHandler]
public class AddressComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpGet("/buy/address/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        // Slice holds the user's just-entered address; if empty, fall
        // back to a stand-in "previous order" address.
        var slice = await workflow.Read<ShippingAddressSlice>(orderId, ShippingAddressWorkflowSlice.Key, ct);

        vm.ShippingAddress = slice is not null
            ? new Address
            {
                FullName = slice.FullName,
                Street = slice.Street,
                ZipCode = slice.ZipCode,
                Town = slice.Town,
                Country = slice.Country
            }
            : RetrieveAddressFromPreviousOrder(orderId);
    }

    static Address RetrieveAddressFromPreviousOrder(Guid orderId)
    {
        // Hahaha, we don't look it up, we just return the address
        // of the best football team in The Netherlands!
        return new Address
        {
            FullName = "Dennis van der Stelt",
            Street = "Van Zandvlietplein 1",
            ZipCode = "3077 AA",
            Town = "Rotterdam",
            Country = "The Netherlands"
        };
    }
}
