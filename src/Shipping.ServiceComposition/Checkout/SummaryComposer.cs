using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.ServiceComposition.Events;
using Shipping.ServiceComposition.Workflow;
using WorkflowComposer;

namespace Shipping.ServiceComposition.Checkout;

[CompositionHandler]
public class SummaryComposer(ShippingDbContext dbContext, IWorkflowStore workflow, IHttpContextAccessor http)
{
    // ServiceComposer 5.x allows only one Http* attribute per method;
    // payment and summary screens get their own entry points that share
    // LoadAsync. Both screens need the same shipping summary slice.
    [HttpGet("/buy/payment/{orderId}")]
    public Task HandlePayment(Guid orderId) => LoadAsync(orderId);

    [HttpGet("/buy/summary/{orderId}")]
    public Task HandleSummary(Guid orderId) => LoadAsync(orderId);

    async Task LoadAsync(Guid orderId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        // DeliveryOptionSubmitHandler writes the chosen DeliveryOptionId
        // synchronously to the workflow slice before sending the saga
        // command, so the slice is the freshest source while the saga
        // is still in flight. Fall back to the DB for orders submitted
        // previously; if both are empty, skip the delivery section so
        // the rest of the composed response - credit cards, cart
        // summary - still renders.
        Guid? deliveryOptionId = null;
        var slice = await workflow.Read<DeliveryOptionSlice>(orderId, DeliveryOptionWorkflowSlice.Key, ct);
        if (slice is not null)
        {
            deliveryOptionId = slice.DeliveryOptionId;
        }
        else
        {
            var order = await dbContext.Orders
                .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);
            deliveryOptionId = order?.DeliveryOptionId;
        }
        if (deliveryOptionId is not { } resolvedId) return;

        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == resolvedId, ct);

        dynamic delivery = new ExpandoObject();
        delivery.DeliveryOptionName = deliveryOption.Name;
        delivery.DeliveryOptionDescription = deliveryOption.Description;

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliverySummaryLoaded
        {
            DeliveryOptionId = deliveryOption.DeliveryOptionId,
            DeliveryOption = delivery
        });

        vm.DeliveryOption = delivery;
    }
}
