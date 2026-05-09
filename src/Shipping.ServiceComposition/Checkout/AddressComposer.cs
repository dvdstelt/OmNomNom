using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

public class AddressComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
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
