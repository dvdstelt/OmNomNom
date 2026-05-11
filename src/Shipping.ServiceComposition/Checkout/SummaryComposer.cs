using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.ServiceComposition.Events;
using Shipping.ServiceComposition.Helpers;

namespace Shipping.ServiceComposition.Checkout;

public class SummaryComposer(ShippingDbContext dbContext, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/payment/{orderId}")]
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        // DeliveryOptionSubmitComposer writes the chosen DeliveryOptionId to
        // the cache synchronously, before sending the saga command, so the
        // cache is the freshest source while the saga is still in flight.
        // Fall back to the DB on a cache miss (sliding 30-min expiry); if
        // both are empty, skip the delivery section so the rest of the
        // composed response — credit cards, cart summary — still renders.
        var cached = await cacheHelper.GetOrder(orderId);
        Guid? deliveryOptionId = cached.DeliveryOptionId;
        if (deliveryOptionId is null)
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
        await context.RaiseEvent(new DeliverySummaryLoaded()
        {
            DeliveryOptionId = deliveryOption.DeliveryOptionId,
            DeliveryOption = delivery
        });

        vm.DeliveryOption = delivery;
    }
}
