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

        var order = await dbContext.Orders.SingleAsync(s => s.OrderId == orderId, ct);
        // The customer reaches /buy/payment after picking shipping, so
        // DeliveryOptionId should be set; .Value matches the pre-SQLite
        // assumption that this row is non-null at this point.
        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(s => s.DeliveryOptionId == order.DeliveryOptionId!.Value, ct);

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
