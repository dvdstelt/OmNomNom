﻿using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.ShoppingCart;

public class ShoppingCartHandler(CacheHelper cacheHelper, CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        if (orderId == Guid.Empty)
        {
            vm.OrderId = Guid.NewGuid();
            vm.CartItems = new Dictionary<Guid, dynamic>();
            return;
        }

        var order = await cacheHelper.GetOrder(orderId);

        var productsCollection = dbContext.Database.GetCollection<Product>();
        // TODO: Figure out if `Contains` is possible with LiteDb
        var products = productsCollection.Query().ToList();
        var orderedProducts = Mapper.MapToDictionary(order, products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new CartLoaded()
        {
            OrderedProducts = orderedProducts
        });

        vm.OrderId = orderId;
        vm.CartItems = orderedProducts.Values.ToList();
    }
}