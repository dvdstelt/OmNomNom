using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

[CompositionHandler]
public class AddressComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpGet("/buy/address/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        // The slice holds whatever the user entered earlier in this
        // checkout. If they haven't filled in the address yet, fall
        // back to a stand-in "previous order" address - in a real
        // system this would be looked up from the customer's history.
        var slice = await workflow.Read<BillingAddressSlice>(orderId, BillingAddressWorkflowSlice.Key, ct);

        vm.BillingAddress = slice?.Address ?? RetrieveAddressFromPreviousOrder(orderId);
    }

    static BillingAddressData RetrieveAddressFromPreviousOrder(Guid orderId) =>
        // Hahaha, we don't look it up, we just return the address
        // of the best football team in The Netherlands!
        new(
            FullName: "Dennis van der Stelt",
            Street: "Van Zandvlietplein 1",
            ZipCode: "3077 AA",
            Town: "Rotterdam",
            Country: "The Netherlands");
}
