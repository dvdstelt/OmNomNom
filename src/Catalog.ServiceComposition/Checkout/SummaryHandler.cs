using System.Dynamic;
using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Checkout;

public class SummaryHandler(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var orderCollection = dbContext.Database.GetCollection<Order>();
        var order = orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault();

        var productsModel = MapToDictionary(order.Products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new SummaryLoaded()
        {
            Products = productsModel
        });

        vm.OrderId = orderId;
        vm.Products = productsModel;
    }

    IDictionary<Guid,dynamic> MapToDictionary(List<OrderItem> products)
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