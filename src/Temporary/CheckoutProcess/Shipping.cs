using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;
using Temporary.Caching;

namespace OmNomNom.Website.ViewModelComposition;

public class Shipping : ICompositionRequestsHandler
{
    private readonly CacheHelper storage;

    public Shipping(CacheHelper storage)
    {
        this.storage = storage;
    }

    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve existing data
        if (!Guid.TryParse(orderIdString, out var orderId))
            orderId = Guid.NewGuid();

        vm.OrderId = orderId;
        var order = await storage.GetCart(orderId);
        var deliveryOptions = DeliveryOptions.GetAllDeliveryOptions();
        vm.DeliveryOptions = deliveryOptions;
        vm.SelectedId = order.DeliveryOptionId ?? deliveryOptions[0].Id;
    }
}