using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class AddressHandler : ICompositionRequestsHandler
{
    private readonly OrderStorage storage;

    public AddressHandler(OrderStorage storage)
    {
        this.storage = storage;
    }

    [HttpGet("/buy/address/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address 
        if (!Guid.TryParse(orderIdString, out var orderId))
            orderId = Guid.NewGuid();

        vm.OrderId = orderId;
        var order = storage.GetOrder(orderId);
        vm.FullName = order.FullName;
        vm.ShippingAddress = order.ShippingAddress ?? new Address();
        vm.BillingAddress = order.BillingAddress ?? new Address();
        vm.IsBillingAddressSame = order.ShippingAddress == null || order.ShippingAddress.Equals(order.BillingAddress);

        return Task.CompletedTask;
    }
}