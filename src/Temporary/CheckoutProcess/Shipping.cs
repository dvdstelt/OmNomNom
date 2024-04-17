using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class Shipping : ICompositionRequestsHandler
{
    private readonly OrderStorage storage;

    public Shipping(OrderStorage storage)
    {
        this.storage = storage;
    }

    [HttpGet("/buy/shipping/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve existing data
        if (!Guid.TryParse(orderIdString, out var orderId))
            orderId = Guid.NewGuid();

        vm.OrderId = orderId;
        var order = storage.GetOrder(orderId);
        var deliveryOptions = DeliveryOptions.GetAllDeliveryOptions();
        vm.DeliveryOptions = deliveryOptions;
        vm.SelectedId = order.DeliveryOptionId ?? deliveryOptions[0].Id;

        return Task.CompletedTask;
    }
}