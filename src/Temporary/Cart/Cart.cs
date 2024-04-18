using System.Dynamic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using ServiceComposer.AspNetCore;
using Temporary;
using Temporary.Caching;

namespace OmNomNom.Website.ViewModelComposition;

public class Cart : ICompositionRequestsHandler
{
    readonly CacheHelper cacheHelper;

    public Cart(CacheHelper cacheHelper)
    {
        this.cacheHelper = cacheHelper;
    }

    [HttpGet("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address
        if (!Guid.TryParse(orderIdString, out var orderId))
            orderId = Guid.NewGuid();

        vm.OrderId = orderId;

        var cart = await cacheHelper.GetCart(orderId);

        vm.CartItems = cart.Items;
        vm.TotalCartPrice = 110; // TODO: Calculate this while retrieving
    }
}