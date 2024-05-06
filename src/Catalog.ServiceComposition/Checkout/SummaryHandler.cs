using System.Dynamic;
using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Catalog.ServiceComposition.Helpers;
using ITOps.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

public class SummaryHandler(CatalogDbContext dbContext, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var order = dbContext.Where<Order>(s => s.OrderId == orderId).SingleOrDefault();

        if (order == null)
        {
            order = await cacheHelper.GetOrder(orderId);
        }

        var productsModel = MapToDictionary(order.Products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new SummaryLoaded()
        {
            OrderId = orderId,
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
        vm.Products = productsModel;
    }

    IDictionary<Guid, dynamic> MapToDictionary(List<OrderItem> products)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var product in products)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = product.ProductId;
            vm.Quantity = product.Quantity;

            productsViewModel[product.ProductId] = vm;
        }

        return productsViewModel;
    }
}