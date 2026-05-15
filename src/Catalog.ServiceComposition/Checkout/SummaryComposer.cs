using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Catalog.ServiceComposition.Checkout;

[CompositionHandler]
public class SummaryComposer(IWorkflowStore workflow, IHttpContextAccessor http)
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var ct = request.HttpContext.RequestAborted;

        // Reaching the summary page with no cart slice means the
        // workflow store reaped it (WorkflowCleanupHostedService runs
        // every 5 minutes and removes slices idle for >20 minutes), or
        // the orderId in the URL is bogus. Either way, falling back to
        // an empty cart would silently submit an empty order on
        // "Place order" - so we 410 Gone and let the SPA tell the user
        // the session expired.
        var cart = await workflow.Read<CartSlice>(orderId, CartWorkflowSlice.Key, ct);
        if (cart is null)
        {
            request.SetActionResult(new StatusCodeResult(StatusCodes.Status410Gone));
            return;
        }

        var productsModel = Mapper.MapToDictionary(cart);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new SummaryLoaded
        {
            OrderId = orderId,
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
        vm.Products = productsModel;
    }
}
