using Finance.Data.Models;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

public class AddressComposer(IWorkflowStore workflow) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        // The slice holds whatever the user entered earlier in this
        // checkout. If they haven't filled in the address yet, fall
        // back to a stand-in "previous order" address - in a real
        // system this would be looked up from the customer's history.
        var slice = await workflow.Read<BillingAddressSlice>(orderId, BillingAddressWorkflowSlice.Key, ct);

        vm.BillingAddress = slice is not null
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
