using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.ServiceComposition.DeliveryOptions;
using Shipping.ServiceComposition.Events;
using Shipping.ServiceComposition.Helpers;

namespace Shipping.ServiceComposition.Checkout;

public class DeliveryOptionsHandler(ShippingDbContext dbContext, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var deliveryOptionsCollection = dbContext.Database.GetCollection<DeliveryOption>();
        var orderCollection = dbContext.Database.GetCollection<Order>();
        
        // Get all available delivery options
        var deliveryOptions = deliveryOptionsCollection.Query().ToList();
        var selectedDeliveryOption =
            orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault()?.DeliveryOptionId;
        if (selectedDeliveryOption == null)
        {
            // See if there's something in cache
            var order = await cacheHelper.GetOrder(orderId);
            if (order.DeliveryOptionId != Guid.Empty)
                selectedDeliveryOption = order.DeliveryOptionId;
        }
        
        var optionsModel = Mapper.MapToDictionary(deliveryOptions);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionsLoaded()
        {
            DeliveryOptions = optionsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.DeliveryOptions = optionsModel.Select(kvp => kvp.Value);
        vm.SelectedDeliveryOption = selectedDeliveryOption;
    }
}