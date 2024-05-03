using System.Dynamic;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Events;

namespace Shipping.ServiceComposition.Checkout;

public class SummaryHandler(ShippingDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var orderCollection = dbContext.Database.GetCollection<Order>();
        var deliveryOptionsCollection = dbContext.Database.GetCollection<DeliveryOption>();
        var order = orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault();
        var deliveryOptionId = order.DeliveryOptionId;
        var deliveryOption = deliveryOptionsCollection.Query().Where(s => s.DeliveryOptionId == deliveryOptionId)
            .Single();

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