using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.ServiceComposition.Events;

namespace Shipping.ServiceComposition.Checkout;

public class SummaryHandler(ShippingDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/payment/{orderId}")]
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        // The shipping POST that brought the customer here only sends a
        // SubmitDeliveryOption command; the saga that writes the row runs
        // asynchronously, so the Order may not yet exist (or may exist with
        // a null DeliveryOptionId). Skip the delivery section in that case
        // and let the rest of the composed response — credit cards, cart
        // summary, etc. — render normally. A later refresh will pick the
        // row up.
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);
        if (order?.DeliveryOptionId is not { } deliveryOptionId) return;

        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == deliveryOptionId, ct);

        dynamic delivery = new ExpandoObject();
        delivery.DeliveryOptionName = deliveryOption.Name;
        delivery.DeliveryOptionDescription = deliveryOption.Description;

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliverySummaryLoaded()
        {
            DeliveryOptionId = deliveryOption.DeliveryOptionId,
            DeliveryOption = delivery
        });

        vm.DeliveryOption = delivery;
    }
}
