using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;
using Temporary.Caching;

namespace OmNomNom.Website.ViewModelComposition;

public class AddressHandler : ICompositionRequestsHandler
{
    readonly CacheHelper cacheHelper;

    public AddressHandler(CacheHelper cacheHelper)
    {
        this.cacheHelper = cacheHelper;
    }

    [HttpGet("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address
        if (!Guid.TryParse(orderIdString, out var orderId))
            orderId = Guid.NewGuid();

        var order = await cacheHelper.GetCart(orderId);
        vm.OrderId = orderId;
        vm.FullName = order.FullName;
        vm.ShippingAddress = order.ShippingAddress ?? new Address();
        vm.BillingAddress = order.BillingAddress ?? new Address();
        vm.IsBillingAddressSame = order.ShippingAddress == null || order.ShippingAddress.Equals(order.BillingAddress);
    }
}