using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Shipping : ICompositionRequestsHandler
{
    [HttpGet("/buy/shipping/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address 
        var orderId = Guid.Parse("526f0a1d-2900-49ba-9d70-987e9f590b04");
        
        vm.OrderId = orderId;

        var deliveryOptions = new Dictionary<int, dynamic>();
        deliveryOptions[0] = CreateDeliveryOption(Guid.Parse("071f3894-762e-4fb2-b55c-8c65f2641f5b"), "Standard shipping", "Next business day", 5);
        deliveryOptions[1] = CreateDeliveryOption(Guid.Parse("155e3818-ff4d-43f1-9000-6d6bb2d2f736"), "Standard shipping", "Next business day", 5);
        deliveryOptions[2] = CreateDeliveryOption(Guid.Parse("3835fb8d-be88-4d39-9eb8-043cac3c9695"), "Standard shipping", "Next business day", 5);
        vm.DeliveryOptions = deliveryOptions.Values.ToList();

        var products = new Dictionary<int, dynamic>();
        products[0] = CreateCartItem(Guid.Parse("ff899e9d-4033-48d4-b189-e6ef4a3dc25b"), "Fremont Stout", 10, 3);;
        products[1] = CreateCartItem(Guid.Parse("0b3dcc85-110b-4491-9946-d20c0a51917b"), "Heineken", 0.5m, 24);;
        vm.CartItems = products.Values.ToList();
        vm.TotalPrice = 42.0m;
        
        return Task.CompletedTask;
    }
    
    dynamic CreateDeliveryOption(Guid id, string name, string description, decimal price)
    {
        dynamic deliveryOption = new ExpandoObject();
        deliveryOption.Id = id;
        deliveryOption.Name = name;
        deliveryOption.Description = description;
        deliveryOption.Price = price;
        return deliveryOption;
    }
    
    dynamic CreateCartItem(Guid id, string name, decimal price, int quantity)
    {
        dynamic product = new ExpandoObject();
        product.Id = id;
        product.Name = name;
        product.Price = price;
        product.Quantity = quantity;
        return product;
    }
}